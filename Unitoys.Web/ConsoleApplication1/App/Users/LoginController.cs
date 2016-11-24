using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Unitoys.Core;
using Unitoys.IServices;
using Unitoys.Model;
using Unitoys.Web.Models;

namespace Unitoys.WebApi
{
    public class LoginController : ApiController
    {
        private IUserService userService;
        private IUserLoginInfoService userLoginInfoService;
        public LoginController(IUserService userService, IUserLoginInfoService userLoginInfoService)
        {
            this.userService = userService;
            this.userLoginInfoService = userLoginInfoService;
        }
        /// <summary>
        /// 检查用户登录（这个检查登录可能要加个密钥参数来检测请求是否合法）
        /// </summary>
        /// <returns></returns>
        public HttpResponseMessage Get([FromUri]AuthQuery authQueryint,string LoginName,string PassWord)
        {

            UT_Users user = userService.CheckUserLogin(LoginName, PassWord);
            if (user != null){

                LoginUserInfo modal = new LoginUserInfo() { ID = user.ID, LoginName = user.LoginName, TrueName = user.TrueName };
                var token = CommonHelper.RandomLoginToken();
                WebUtil.SetApiUserSession(token, modal);
                //添加登录记录
                userLoginInfoService.Insert(new UT_UserLoginInfo() {  UserId = modal.ID, LoginName = modal.LoginName, LoginIp = WebHelper.GetIP(), Entrance = "WEBAPI", LoginTime = DateTime.Now });

                return Request.CreateResponse(HttpStatusCode.OK, new { Type = "Success", Msg = "获取成功", data = new { LoginName = user.LoginName, TrueName = user.TrueName, token = token } }); 
            }
            return Request.CreateResponse(HttpStatusCode.NotFound, new { Type = "Error", Msg = "帐号或者密码错误！" }); 
        }

        /// <summary>
        /// 判断是否登录
        /// </summary>
        /// <param name="token">token信令</param>
        /// <returns></returns>
        public HttpResponseMessage Get([FromUri]AuthQuery authQueryint, string token)
        {
            LoginUserInfo modal = WebUtil.GetApiUserSession(token);
            if (modal != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { code = 0, Msg = "已登录！", data = new { LoginName = modal.LoginName, TrueName = modal.TrueName, token = token } });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new { code = 102, Msg = "未登录！" });
            }

        }
    }
}
