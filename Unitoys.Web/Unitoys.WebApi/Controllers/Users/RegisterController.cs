using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Unitoys.Core;
using Unitoys.IServices;
using Unitoys.Model;

namespace Unitoys.WebApi.Controllers
{
    /// <summary>
    /// 用户注册
    /// </summary>
    public class RegisterController : ApiController
    {
        private IUserService _userService;
        private ISMSConfirmationService _smsConfirmationService;
        private IUserLoginRecordService _userLoginInfoService;
        private IOrderService _orderService;

        public RegisterController(IUserService userService, ISMSConfirmationService smsConfirmationService, IUserLoginRecordService userLoginInfoService, IOrderService orderService)
        {
            this._userService = userService;
            this._smsConfirmationService = smsConfirmationService;
            this._userLoginInfoService = userLoginInfoService;
            _orderService = orderService;
        }

        /// <summary>
        /// 快速注册
        /// </summary>
        /// <param name="authQueryint"></param>
        /// <param name="tel">手机号码</param>
        /// <param name="passWord">密码</param>
        /// <param name="smsVerCode">验证码</param>
        /// <returns></returns>
        [HttpPost]
        [NoLogin]
        public async Task<IHttpActionResult> Post([FromBody]QueryRegUser queryModel)
        {
            var errorMsg = "";

            if (!ValidateHelper.IsMobile(queryModel.tel))
            {
                errorMsg = "手机号码格式不正确！";
            }
            else if (queryModel.passWord.Length < 6 || queryModel.passWord.Length > 12)
            {
                errorMsg = "密码长度必须在6~12位之间！";
            }
            else if (!ValidateHelper.IsNumeric(queryModel.smsVerCode))
            {
                errorMsg = "验证码无效！";
            }
            else if (_userService.CheckTelExist(queryModel.tel))
            {
                errorMsg = "您输入的手机号码已注册！";
            }
            else
            {
                //判断手机验证码是否正确。
                UT_SMSConfirmation smsConfirmation = await _smsConfirmationService.GetEntityAsync(x => x.Tel == queryModel.tel && x.Code == queryModel.smsVerCode && x.Type == 1 && !x.IsConfirmed);

                //判断当前时间是否到达验证码过期时间。
                if (smsConfirmation != null)
                {
                    if (DateTime.Now > smsConfirmation.ExpireDate)
                    {
                        errorMsg = "此验证码已经过期，请重新发送验证码。";
                    }
                    else
                    {
                        UT_Users model = new UT_Users();
                        model.NickName = queryModel.nickName == null ? "" : queryModel.nickName;
                        model.Tel = queryModel.tel;
                        model.PassWord = SecureHelper.MD5(queryModel.passWord);
                        model.Amount = 0;
                        model.CreateDate = DateTime.Now;
                        model.GroupId = Guid.Parse("688a3245-2628-4488-bf35-9c029ff80988"); //默认会员组
                        model.Status = 0;
                        model.Score = 0;
                        model.UserHead = "/Unitoys/2015/12/1512291755292460937.png";

                        switch (PhoneServerByMySqlServices.SetSip_Buddies(model.Tel))
                        {
                            case 2:
                                return Ok(new { status = 0, msg = "系统繁忙，请重试" });
                            case 0:
                                return Ok(new { status = 0, msg = "注册失败，请重试" });
                        }

                        if (await _userService.RegisterAsync(model, smsConfirmation))
                        {
                            //默认运动目标8000
                            if (await _userService.ModifyUserInfoAndUserShape(model.ID, null, null, null, null, null, 8000))
                            {

                            }
                            return Ok(new { status = 1, msg = "注册成功" });
                        }
                    }
                }
                else
                {
                    errorMsg = "验证码错误！";
                }
            }
            return Ok(new { status = 0, msg = errorMsg });
        }

        [HttpGet]
        [NoAuthenticate]
        public async Task<IHttpActionResult> setaaa()
        {
            var list = await _userService.GetAll();
            foreach (var item in list)
            {
                if (PhoneServerByMySqlServices.SetSip_Buddies(item.Tel) == 0)
                {
                    return Ok(new { aaaa = "0" });
                };
            }
            return Ok(new { aaaa = "1" });
        }

        //TODO 方便测试注册，删除功能
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="authQueryint"></param>
        /// <param name="tel">手机号码</param>
        /// <returns></returns>
        [HttpPost]
        [NoLogin]
        public async Task<IHttpActionResult> Delete([FromBody]QueryRegUser queryModel)
        {
            var errorMsg = "";
            UT_Users entity = await _userService.GetEntityAsync(x => x.Tel == queryModel.tel &&
                x.CreateDate > new DateTime(2015, 1, 1));

            var list = await _userLoginInfoService.GetEntitiesAsync(x => x.UserId == entity.ID);
            var listOrder = await _orderService.GetEntitiesAsync(x => x.UserId == entity.ID);
            foreach (var item in list)
            {
                await _userLoginInfoService.DeleteAsync(item);
            }
            foreach (var item in listOrder)
            {
                await _orderService.DeleteAsync(item);
            }
            if (await _userService.DeleteAsync(entity))
            {
                return Ok(new { status = 1, msg = "删除成功" });
            }

            errorMsg = "删除失败";
            return Ok(new { status = 0, msg = errorMsg });
        }

        
    }

    public class QueryRegUser
    {
        public string nickName { get; set; }
        public string tel { get; set; }
        public string passWord { get; set; }
        //public string confirmPassWord { get; set; }
        public string smsVerCode { get; set; }
    }

}
