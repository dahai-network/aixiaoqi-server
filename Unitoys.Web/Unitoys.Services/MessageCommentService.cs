using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.IServices;
using Unitoys.Model;
using System.Data.Entity;

namespace Unitoys.Services
{
    public class MessageCommentService : BaseService<UT_MessageComment>, IMessageCommentService
    {
        /// <summary>
        /// 发表评论
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="messageId">用户动态消息ID</param>
        /// <param name="content">评论内容</param>
        /// <returns></returns>
        public async Task<UT_MessageComment> AddMessageComment(Guid userId, Guid messageId, string content)
        {
            using(UnitoysEntities db = new UnitoysEntities())
            {
                //1. 首先判断用户的动态消息是否存在，有可能存在删除的同时新增一条用户动态消息评论。
                var message = await db.UT_Message.FindAsync(messageId);

                //2. 如果用户动态消息存在，则添加评论。
                if(message != null)
                {
                    UT_MessageComment messageComment = new UT_MessageComment()
                    {
                        UserId = userId,
                        MessageId = messageId,
                        CreateDate = DateTime.Now,
                        Content = content
                    };
                    db.UT_MessageComment.Add(messageComment);

                    return await db.SaveChangesAsync() > 0 ? messageComment : null;
                }

                return null;
            }
        }
    }
}
