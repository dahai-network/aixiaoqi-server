using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Unitoys.Core;
using Unitoys.IServices;
using Unitoys.Model;

namespace Unitoys.WebApi.Controllers
{
    public class UserController : ApiController
    {
        private IUserService _userService;
        private IPaymentService _paymentService;
        private IUserBillService _userBillService;
        private ISMSConfirmationService _smsConfirmationService;
        private IOrderService _orderService;
        public UserController(IUserService userService, IPaymentService paymentService, IUserBillService userBillService, ISMSConfirmationService smsConfirmationService, IOrderService orderService)
        {
            this._userService = userService;
            this._paymentService = paymentService;
            this._userBillService = userBillService;
            this._smsConfirmationService = smsConfirmationService;
            this._orderService = orderService;
        }

        /// <summary>
        /// 更新用户基本信息
        /// </summary>
        /// <param name="authQueryint"></param>
        /// <param name="email">Email</param>
        /// <param name="trueName">真实姓名</param>
        /// <param name="qq">QQ号码</param>
        /// <param name="sex">性别</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> UpdateUserInfo([FromBody]QueryUpdateUser queryModel)
        {
            string errorMsg = "";

            var currentUser = WebUtil.GetApiUserSession();

            if (currentUser == null)
            {
                errorMsg = "用户不能为空！";
            }
            else if (!ValidateHelper.IsEmail(queryModel.email))
            {
                errorMsg = "邮箱地址或者格式错误！";
            }
            else if (queryModel.trueName.Length > 10)
            {
                errorMsg = "真实姓名不能长于10个字符！";
            }
            else if (queryModel.nickName.Length > 20)
            {
                errorMsg = "昵称不能长于10个字符！";
            }
            else if (!ValidateHelper.IsNumeric(queryModel.qq) || queryModel.qq.Length > 12)
            {
                errorMsg = "QQ号码错误！";
            }
            else if (queryModel.sex != 0 && queryModel.sex != 1)
            {
                errorMsg = "性别错误！";
            }
            else
            {
                UT_Users user = await _userService.GetEntityByIdAsync(currentUser.ID);

                if (user != null)
                {
                    user.Email = queryModel.email;
                    user.TrueName = queryModel.trueName;
                    user.NickName = queryModel.nickName;
                    user.QQ = queryModel.qq;
                    user.Sex = queryModel.sex;

                    //排除字段更新
                    string[] updateExcludeField = new string[] { "Tel", "UserHead", "PassWord", "Amount", "CreateDate", "Score" };

                    if (await _userService.UpdateAsync(user, updateExcludeField))
                    {
                        return Ok(new { status = 1, msg = "更新成功" });
                    }

                    errorMsg = "暂时无法更新，操作失败";
                }
            }
            return Ok(new { status = 0, msg = errorMsg });
        }
        /// <summary>
        /// 更新用户基本信息和体形
        // </summary>
        /// <param name="authQueryint"></param>
        /// <param name="email">Email</param>
        /// <param name="trueName">真实姓名</param>
        /// <param name="qq">QQ号码</param>
        /// <param name="sex">性别</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> UpdateUserInfoAndUserShape([FromBody]UserInfoAndUserShape queryModel)
        {
            string errorMsg = "";

            var currentUser = WebUtil.GetApiUserSession();

            if (currentUser == null)
            {
                errorMsg = "用户不能为空！";
            }
            else if (!string.IsNullOrEmpty(queryModel.nickName) && queryModel.nickName.Length > 20)
            {
                errorMsg = "昵称不能长于10个字符！";
            }
            else if (queryModel.sex != null && queryModel.sex != 0 && queryModel.sex != 1)
            {
                errorMsg = "性别错误！";
            }
            else
            {
                //排除字段更新
                string[] updateExcludeField = new string[] { "Tel", "UserHead", "PassWord", "Amount", "CreateDate", "Score" };

                if (await _userService.ModifyUserInfoAndUserShape(currentUser.ID, queryModel.nickName, queryModel.sex, queryModel.Birthday, queryModel.height, queryModel.weight, queryModel.movingTarget))
                {
                    return Ok(new { status = 1, msg = "更新成功" });
                }

                errorMsg = "暂时无法更新，操作失败";

            }
            return Ok(new { status = 0, msg = errorMsg });
        }

        [HttpPost]
        public async Task<IHttpActionResult> ModifyNickName([FromBody]QueryUpdateUser queryModel)
        {
            string errorMsg = "";

            var currentUser = WebUtil.GetApiUserSession();

            if (queryModel.nickName.Length > 20)
            {
                errorMsg = "昵称不能长于10个字符！";
            }
            else
            {
                UT_Users user = await _userService.GetEntityByIdAsync(currentUser.ID);

                if (user != null)
                {
                    user.NickName = queryModel.nickName;

                    //排除字段更新
                    string[] updateExcludeField = new string[] { "Tel", "PassWord", "Amount", "UserHead", "CreateDate", "Score", "Email", "TrueName", "QQ", "Sex" };

                    if (await _userService.UpdateAsync(user, updateExcludeField))
                    {
                        return Ok(new { status = 1, msg = "更新成功" });
                    }

                    errorMsg = "暂时无法更新，操作失败";
                }
            }
            return Ok(new { status = 0, msg = errorMsg });
        }
        [HttpPost]
        public async Task<IHttpActionResult> ModifyEmail([FromBody]QueryUpdateUser queryModel)
        {
            string errorMsg = "";

            var currentUser = WebUtil.GetApiUserSession();

            if (!ValidateHelper.IsEmail(queryModel.email))
            {
                errorMsg = "您填写的邮箱格式错误！";
            }
            else
            {
                UT_Users user = await _userService.GetEntityByIdAsync(currentUser.ID);

                if (user != null)
                {
                    user.Email = queryModel.email;

                    //排除字段更新
                    string[] updateExcludeField = new string[] { "Tel", "PassWord", "Amount", "UserHead", "CreateDate", "Score", "NickName", "TrueName", "QQ", "Sex" };

                    if (await _userService.UpdateAsync(user, updateExcludeField))
                    {
                        return Ok(new { status = 1, msg = "更新成功" });
                    }

                    errorMsg = "暂时无法更新，操作失败";
                }
            }
            return Ok(new { status = 0, msg = errorMsg });
        }
        public async Task<IHttpActionResult> ModifyUserHead()
        {
            string errorMsg = "";

            var httpRequest = HttpContext.Current.Request;

            //LoggerHelper.Info(httpRequest.Files.Count + "");
            //LoggerHelper.Info(Request.Content.ReadAsStringAsync().Result);

            //LoggerHelper.Info();

            //1. 校验参数。
            if (httpRequest.Files.Count > 1)
            {
                errorMsg = "只能选择一张图片！";
            }
            if (httpRequest.Files.Count > 0)
            {
                var currentUser = WebUtil.GetApiUserSession();

                HttpPostedFile image = httpRequest.Files[0];

                //2. 等待图片上传完成。
                string uploadImageUrl = await WebUtil.UploadImgAsync(image, "/Unitoys/UserHead/", SecureHelper.MD5(currentUser.Tel));

                //3. 判断图片是否上传成功。
                if (uploadImageUrl != "-1" && uploadImageUrl != "-2" && uploadImageUrl != "-3")
                {
                    UT_Users user = await _userService.GetEntityByIdAsync(currentUser.ID);

                    if (user != null)
                    {
                        user.UserHead = uploadImageUrl + "?v=" + CommonHelper.GetDateTimeInt();

                        //排除字段更新
                        string[] updateExcludeField = new string[] { "Tel", "PassWord", "Amount", "CreateDate", "NickName", "Score", "Email", "TrueName", "QQ", "Sex" };

                        if (await _userService.UpdateAsync(user, updateExcludeField))
                        {
                            return Ok(new { status = 1, msg = "更新成功", data = new { UserHead = user.UserHead.GetUserHeadCompleteUrl() + "?r=" + new Random().Next(100) } });
                        }

                        errorMsg = "暂时无法更新，操作失败";
                    }
                }
                if (string.IsNullOrEmpty(errorMsg))
                    errorMsg = "暂时无法保存头像";

            }
            return Ok(new { status = 0, msg = errorMsg });
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="authQueryint"></param>
        /// <param name="oldPassword">旧密码</param>
        /// <param name="newPassword">新密码</param>
        /// <param name="confirmNewPassword">确认密码</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> ModifyPassword([FromBody]QueryModifyPassword queryModel)
        {
            string errorMsg = "";

            var currentUser = WebUtil.GetApiUserSession();

            if (currentUser == null)
            {
                errorMsg = "用户不能为空！";
            }
            else
            {
                //1. 首先判断原密码是否正确
                UT_Users user = await _userService.GetEntityByIdAsync(currentUser.ID);

                if (user != null)
                {
                    if (user.PassWord != SecureHelper.MD5(queryModel.oldPassword))
                    {
                        errorMsg = "原密码错误！";
                    }
                    else
                    {
                        //2. 参数验证
                        if (queryModel.newPassword.Length < 6 || queryModel.newPassword.Length > 20)
                        {
                            errorMsg = "密码长度必须在6~20位之间！";
                        }
                        else if (queryModel.newPassword != queryModel.confirmNewPassword)
                        {
                            errorMsg = "两次输入的密码不一致！";
                        }
                        else
                        {
                            //3. 更新用户密码
                            user.PassWord = SecureHelper.MD5(queryModel.newPassword);

                            if (await _userService.UpdateAsync(user, "CreateDate"))
                            {
                                return Ok(new { status = 1, msg = "修改成功" });
                            }
                        }
                    }
                }
            }
            return Ok(new { status = 0, msg = errorMsg });
        }

        /// <summary>
        /// 找回密码
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        [HttpPost]
        [NoLogin]
        public async Task<IHttpActionResult> ForgotPassword([FromBody]QueryForgotPassword queryModel)
        {
            string errorMsg = "";

            if (!ValidateHelper.IsMobile(queryModel.tel))
            {
                errorMsg = "手机号码格式不正确！";
            }
            else if (queryModel.newPassword.Length < 6 || queryModel.newPassword.Length > 20)
            {
                errorMsg = "密码长度必须在6~20位之间！";
            }
            else
            {
                //判断该手机号码是否已经注册。
                UT_Users user = await _userService.GetEntityAsync(x => x.Tel == queryModel.tel);

                if (user != null)
                {
                    //判断手机验证码是否正确。
                    UT_SMSConfirmation smsConfirmation = await _smsConfirmationService.GetEntityAsync(x => x.Tel == queryModel.tel && x.Code == queryModel.smsVerCode && x.Type == 2 && !x.IsConfirmed);

                    if (smsConfirmation != null)
                    {
                        if (DateTime.Now > smsConfirmation.ExpireDate)
                        {
                            errorMsg = "此验证码已经过期，请重新发送验证码！";
                        }
                        else
                        {
                            user.PassWord = SecureHelper.MD5(queryModel.newPassword);

                            if (await _userService.ForgotPasswordAsync(user, smsConfirmation))
                            {
                                return Ok(new { status = 1, msg = "找回密码成功" });
                            }
                        }
                    }
                    else
                    {
                        errorMsg = "验证码错误！";
                    }
                }
                else
                {
                    errorMsg = "手机号未注册！";
                }
            }
            return Ok(new { status = 0, msg = errorMsg });
        }

        /// <summary>
        /// 获取用户消费记录
        /// </summary>
        /// <param name="authQueryint"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> GetUserBill(int pageNumber = 1, int pageSize = 10, int billType = -1)
        {
            var currentUser = WebUtil.GetApiUserSession();

            //查询Expression
            Expression<Func<UT_UserBill, bool>> exp = GetUserBillSearchExpression<UT_UserBill>(currentUser.ID, billType);

            var result = _userBillService.GetEntitiesForPagingAsync(pageNumber, pageSize, x => new { x.CreateDate }, "desc", exp);

            var totalRows = _userBillService.GetEntitiesCountAsync(exp);

            //过滤掉不必要的字段
            var data = from i in await result as List<UT_UserBill>
                       select new
                       {
                           ID = i.ID,
                           Amount = i.Amount,
                           UserAmount = i.UserAmount,
                           BillType = (int)i.BillType + "",
                           Descr = i.Descr,
                           PayType = (int)i.PayType + "",
                           PayTips = CommonHelper.PayStatusTips(i.PayType),
                           CreateDate = i.CreateDate
                       };

            return Ok(new { status = 1, msg = "获取成功！", data = new { totalRows = await totalRows, list = data } });
        }

        /// <summary>
        /// 查询账户余额
        /// </summary>
        /// <param name="authQueryint"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> GetUserAmount()
        {
            var currentUser = WebUtil.GetApiUserSession();

            UT_Users user = await _userService.GetEntityByIdAsync(currentUser.ID);

            if (user != null)
            {
                return Ok(new { status = 1, data = new { amount = user.Amount } });
            }

            return Ok(new { status = 0, msg = "找不到该用户！" });
        }

        /// <summary>
        /// 获取本次可以通话的最长秒数
        /// </summary>
        /// <param name="authQueryint"></param>
        /// <param name="To">被叫号码</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> GetMaximumPhoneCallTime()
        {
            string errorMsg = "";

            var currentUser = WebUtil.GetApiUserSession();


            UT_Users user = await _userService.GetEntityByIdAsync(currentUser.ID);

            if (user != null)
            {
                //判断被叫号码的费率。TODO
                int maximumPhoneCallTime = 0;

                maximumPhoneCallTime = Convert.ToInt32(user.Amount / UTConfig.SiteConfig.CallDirectPricePerMinutes * 60);

                int dtInt = CommonHelper.GetDateTimeInt();

                //订单剩余分钟数
                //获取当前用户有效订单-已激活+已付款+剩余通话时间大于0+在有效时间内
                var orderList = await _orderService.GetEntitiesAsync(x => x.UserId == user.ID
                        && x.OrderStatus == OrderStatusType.Used
                        && x.PayStatus == PayStatusType.YesPayment
                        && x.EffectiveDate.HasValue
                        && x.EffectiveDate.Value + (x.ExpireDays * 86400) > dtInt
                        && x.RemainingCallMinutes > 0);
                if (orderList != null && orderList.Count() > 0)
                {
                    maximumPhoneCallTime += orderList.Sum(x => x.RemainingCallMinutes) * 60;
                }

                return Ok(new { status = 1, data = new { maximumPhoneCallTime = maximumPhoneCallTime.ToString() } });
            }

            return Ok(new { status = 0, msg = "找不到该用户！" });

            return Ok(new { status = 0, msg = errorMsg });
        }

        /// <summary>
        /// 获取是否在线
        /// </summary>
        /// <param name="authQueryint"></param>
        /// <param name="To">被叫号码</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> GetIsOnlineByUserTel(string userTel)
        {
            var result = PhoneServerByMySqlServices.GetIsOnline(userTel);

            return Ok(new { status = 1, data = new { IsOnline = result == 1 ? "1" : "0" } });
        }

        #region 获取查询的Expression表达式
        /// <summary>
        /// 获取查询表达式
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        private Expression<Func<TEntity, bool>> GetUserBillSearchExpression<TEntity>(Guid UserId, int billType)
        {
            Expression expression = null;
            ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "entity");

            try
            {
                Expression searchExpression = null;
                Expression inExpression;
                MemberExpression left = null;
                ConstantExpression right;

                if (UserId != Guid.Empty)
                {
                    left = Expression.Property(parameter, "UserId");
                    right = Expression.Constant(UserId, typeof(Guid));
                    inExpression = Expression.Equal(left, right);
                    searchExpression = searchExpression == null ? inExpression : Expression.And(inExpression, searchExpression);
                }
                if (billType != -1)
                {
                    left = Expression.Property(parameter, "BillType");
                    right = Expression.Constant(billType, typeof(int));
                    inExpression = Expression.Equal(left, right);
                    searchExpression = searchExpression == null ? inExpression : Expression.And(inExpression, searchExpression);
                }
                expression = expression == null ? searchExpression : Expression.And(expression, searchExpression);
            }
            catch (Exception)
            {
                return null;
            }

            return expression == null ? x => true : Expression.Lambda<Func<TEntity, bool>>(expression, parameter);
        }
        #endregion
    }

    public class QueryUpdateUser
    {
        public string email { get; set; }
        public string nickName { get; set; }
        public string trueName { get; set; }
        public string qq { get; set; }
        public int sex { get; set; }
    }
    public class UserInfoAndUserShape
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string nickName { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public int? sex { get; set; }
        /// <summary>
        /// 出生日期
        /// </summary>
        public int? Birthday { get; set; }
        /// <summary>
        /// 身高
        /// </summary>
        public double? height { get; set; }
        /// <summary>
        /// 体重
        /// </summary>
        public double? weight { get; set; }
        /// <summary>
        /// 运动目标
        /// </summary>
        public int? movingTarget { get; set; }
    }
    public class QueryModifyPassword
    {
        public string oldPassword { get; set; }
        public string newPassword { get; set; }
        public string confirmNewPassword { get; set; }
    }

    public class QueryForgotPassword
    {
        public string tel { get; set; }
        public string newPassword { get; set; }
        //public string confirmNewPassword { get; set; }
        public string smsVerCode { get; set; }
    }


}
