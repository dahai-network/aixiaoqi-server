using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Http;
using Unitoys.Core;
using Unitoys.IServices;
using Unitoys.Model;
using Unitoys.WebApi.Models;

namespace Unitoys.WebApi.Controllers
{
    public class MessageController : ApiController
    {
        private IUserService _userService;
        private IMessageService _messageService;
        private IMessageLikeService _messageLikeService;
        private IMessagePhotoService _messagePhotoService;
        private IMessageCommentService _messageCommentService;
        public MessageController(IUserService userService, IMessageService messageService, IMessageLikeService messageLikeService, IMessagePhotoService messagePhotoService, IMessageCommentService messageCommentService)
        {
            this._userService = userService;
            this._messageService = messageService;
            this._messageLikeService = messageLikeService;
            this._messagePhotoService = messagePhotoService;
            this._messageCommentService = messageCommentService;
        }

        /// <summary>
        /// 添加动态消息
        /// </summary>
        /// <param name="authQueryint">身份验证模型</param>
        /// <param name="queryModel">动态消息模型</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> AddMessage([FromUri]AddMessageBindingModel model)
        {
            string errorMsg = "";

            var currentUser = WebUtil.GetApiUserSession();
         
            var httpRequest = HttpContext.Current.Request;            

            //1. 校验参数。
            if (httpRequest.Files.Count > 6)
            {
                errorMsg = "发表动态消息的图片不能超过6张。";
            }
            else if(model.Content.Trim().Equals("")){
                errorMsg = "您还没有输入内容。";
            }
            else
            {                
                using(TransactionScope transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    UT_Message message = new UT_Message();
                    message.UserId = currentUser.ID;
                    message.CreateDate = DateTime.Now;
                    message.Ip = WebHelper.GetIP(Request);
                    message.Country = model.Country == "" ? "未知" : model.Country;
                    message.Location = model.Location == "" ? "未知" : model.Location;
                    message.Content = model.Content;

                    //2. 开启创建Message的Task。
                    bool isInsertMessageSucceed = await _messageService.InsertAsync(message);

                    if(isInsertMessageSucceed)
                    {
                        //3. 创建图片对象。
                        if (httpRequest.Files.Count > 0)
                        {
                            List<Task<string>> uploadTasks = new List<Task<string>>();

                            for (int i = 0; i < httpRequest.Files.Count; i++)
                            {
                                HttpPostedFile image = httpRequest.Files[i];

                                uploadTasks.Add(WebUtil.UploadImgAsync(image));
                            }

                            //4. 等待图片上传完成。
                            string[] uploadTaskResult = await Task.WhenAll(uploadTasks);

                            //5. 建立插入动态消息图片多任务Task。
                            List<Task<bool>> insertMessagePhotoTasks = new List<Task<bool>>();

                            foreach (var uploadImageUrl in uploadTaskResult)
                            {
                                //6. 判断图片是否上传成功。
                                if (uploadImageUrl != "-1" && uploadImageUrl != "-2" && uploadImageUrl != "-3")
                                {
                                    UT_MessagePhoto messagePhoto = new UT_MessagePhoto();
                                    messagePhoto.MessageId = message.ID;
                                    messagePhoto.Path = uploadImageUrl;
                                    messagePhoto.CreateDate = DateTime.Now;
                                    //添加异步任务。
                                    insertMessagePhotoTasks.Add(_messagePhotoService.InsertAsync(messagePhoto));
                                }
                            }

                            //7. 等待用户动态消息图片异步任务完成。
                            await Task.WhenAll(insertMessagePhotoTasks);
                         }
                    }
                    transactionScope.Complete();
                }
                return Ok(new { status = 1, msg = "添加动态消息成功！" });
            }
            return Ok(new { status = 0, msg = errorMsg });
        }

        /// <summary>
        /// 发表评论
        /// </summary>
        /// <param name="authQueryint">身份验证模型</param>
        /// <param name="queryModel">用户消息评论模型</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> AddMessageComment([FromBody]AddMessageCommentBindingModel model)
        {
            var currentUser = WebUtil.GetApiUserSession();

            var messageComment = await _messageCommentService.AddMessageComment(currentUser.ID, model.MessageID, model.Content);

            if (messageComment != null)
            {
                return Ok(new { status = 1, msg = "评论成功" });
            }
            return Ok(new { status = 0, msg = "评论失败" });
        }

        /// <summary>
        /// 点赞
        /// </summary>
        /// <param name="authQueryint">身份验证模型</param>
        /// <param name="messageId">用户动态消息ID</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> Like(Guid messageId)
        {
            var currentUser = WebUtil.GetApiUserSession();

            int likeCount = await _messageLikeService.LikeOrUnlike(currentUser.ID, messageId);

            return Ok(new { status = 1, data = new { likeCount = likeCount } });
        }

        /// <summary>
        /// 获取当前登录用户的动态列表
        /// </summary>
        /// <param name="authQueryint">身份验证模型</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> GetCurrentUserMessage()
        {
            var currentUser = WebUtil.GetApiUserSession();

            var messageList = await _messageService.GetEntitiesAsync(x => x.UserId == currentUser.ID);

            List<MessageModel> resultList = new List<MessageModel>();

            //1. 建立获取用户名的Task。
            Task<UT_Users> userTask = _userService.GetEntityByIdAsync(currentUser.ID);

            //建立Model作为Response。
            foreach (var message in messageList)
            {
                //2. 建立获取动态消息点赞数的Task。
                Task<int> likeCountTask = _messageLikeService.GetEntitiesCountAsync(x => x.MessageId == message.ID);

                //3. 建立获取动态消息评论数的Task。
                Task<int> commentCountTask = _messageCommentService.GetEntitiesCountAsync(x => x.MessageId == message.ID);

                //4. 建立获取该条消息的评论接口
                var commentListTask = _messageCommentService.GetEntitiesForPagingAsync(1, 5, x => new { x.CreateDate }, "DESC", x => x.MessageId == message.ID);
                
                //5. 建立获取动态消息图片Path的Task。
                Task<List<string>> photoPathTask = _messagePhotoService.GetMessagePhotoPath(message.ID);

                List<MessageCommentList> commentList = new List<MessageCommentList>();
                foreach (var item in await commentListTask)
                {
                    var commentUser = await _userService.GetEntityByIdAsync(item.UserId);
                    MessageCommentList messageComment = new MessageCommentList();
                    messageComment.UserId = commentUser.ID;
                    messageComment.NickName = commentUser.NickName;
                    messageComment.Content = item.Content;
                    messageComment.CreateDate = item.CreateDate.ToString();
                    commentList.Add(messageComment);
                }

                List<string> photoPathsList = new List<string>();
                foreach (string url in await photoPathTask)
                {
                    photoPathsList.Add(url.GetCompleteUrl());
                }
                MessageModel model = new MessageModel()
                {
                    MessageId = message.ID,
                    UserId = message.UserId,
                    CreateDate = message.CreateDate,
                    Country = message.Country,
                    Location = message.Location,
                    Content = message.Content,
                    NickName = (await userTask).NickName,
                    UserHead = (await userTask).UserHead.GetUserHeadCompleteUrl(),
                    LikeCount = await likeCountTask,
                    CommentCount = await commentCountTask,
                    CommentList = commentList,
                    PhotoPaths = photoPathsList
                };
                resultList.Add(model);
            }

            return Ok(new { status = 1, data = resultList });
        }

        /// <summary>
        /// 获取所有用户的动态消息列表
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> GetAllMessage(int pageNumber = 1, int pageSize = 10, string country = "")
        {
            //查询Expression
            Expression<Func<UT_Message, bool>> exp ;
            if (country == "" || country == null)
            {
                exp =  x => true ;
            }
            else
            {
                exp = x => x.Country == country;
            }
            var messageList = await _messageService.GetEntitiesForPagingAsync(pageNumber, pageSize, x => new { x.CreateDate }, "desc", exp);

            List<MessageModel> resultList = new List<MessageModel>();

            //建立Model作为Response。
            foreach (var message in messageList)
            {
                //1. 建立获取用户名的Task。
                Task<UT_Users> userTask = _userService.GetEntityByIdAsync(message.UserId);

                //2. 建立获取动态消息点赞数的Task。
                Task<int> likeCountTask = _messageLikeService.GetEntitiesCountAsync(x => x.MessageId == message.ID);

                //3. 建立获取动态消息评论数的Task。
                Task<int> commentCountTask = _messageCommentService.GetEntitiesCountAsync(x => x.MessageId == message.ID);

                //4. 建立获取该条消息的评论接口
                var commentListTask = _messageCommentService.GetEntitiesForPagingAsync(1, 5, x => new { x.CreateDate }, "DESC", x => x.MessageId == message.ID);
                
                //5. 建立获取动态消息图片Path的Task。
                Task<List<string>> photoPathTask = _messagePhotoService.GetMessagePhotoPath(message.ID);


                List<MessageCommentList> commentList = new List<MessageCommentList>();
                foreach (var item in await commentListTask)
                {
                    var commentUser = await _userService.GetEntityByIdAsync(item.UserId);
                    MessageCommentList messageComment = new MessageCommentList();
                    messageComment.UserId = commentUser.ID;
                    messageComment.NickName = commentUser.NickName;
                    messageComment.Content = item.Content;
                    messageComment.CreateDate = item.CreateDate.ToString();
                    commentList.Add(messageComment);
                }

                List<string> photoPathsList = new List<string>();
                foreach (string url in await photoPathTask)
                {
                    photoPathsList.Add(url.GetCompleteUrl());
                }
                MessageModel model = new MessageModel()
                {
                    MessageId = message.ID,
                    UserId = message.UserId,                    
                    CreateDate = message.CreateDate,
                    Country = message.Country,
                    Location = message.Location,
                    Content = message.Content,
                    NickName = (await userTask).NickName,
                    UserHead = (await userTask).UserHead.GetUserHeadCompleteUrl(),
                    LikeCount = await likeCountTask,
                    CommentCount = await commentCountTask,
                    CommentList = commentList,
                    PhotoPaths = photoPathsList
                };
                resultList.Add(model);
            }
            return Ok(new { status = 1, data = resultList });
        }
    }

    public class MessageModel
    {
        public Guid MessageId { get; set; }
        public Guid UserId { get; set; }
        public string NickName { get; set; }
        public string UserHead { get; set; }
        public DateTime CreateDate { get; set; }
        public string Country { get; set; }
        public string Location { get; set; }
        public string Content { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public List<string> PhotoPaths { get; set; }

        public List<MessageCommentList> CommentList { get; set; }
    }

    public class MessageCommentList
    {
        public Guid UserId { get; set; }

        public string NickName { get; set; }

        public string Content { get; set; }

        public string CreateDate { get; set; }
    }
}
