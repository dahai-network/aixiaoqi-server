using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Unitoys.Core;
using Unitoys.IServices;
using Unitoys.Model;

namespace Unitoys.Web.Areas.Manage.Controllers
{
    public class LoginController : Controller
    {

        private IManageUserService _manageUserService;

        public LoginController() { }

        public LoginController(IManageUserService manageUserService)
        {
            this._manageUserService = manageUserService;
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 对用户登录的操作进行验证
        /// </summary>
        /// <param name="username">用户账号</param>
        /// <param name="password">用户密码</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult CheckLogin(string loginName, string passWord)
        {
            string result = "";
            
            if (string.IsNullOrEmpty(loginName))
            {
                result = "用户名不能为空";
            }
            else if (string.IsNullOrEmpty(passWord))
            {
                result = "密码不能为空";
            }
            else
            {
                UT_ManageUsers user = _manageUserService.CheckUserLogin(loginName, passWord);


                if (user != null)
                {
                    WebUtil.SetManageUserSession(new LoginUserInfo() { ID = user.ID, LoginName = user.LoginName, TrueName = user.TrueName, LoginIp = WebHelper.GetIP() });
                    result = "OK";
                }
                else
                {
                    result = "帐号不存在或者密码错误！";
                }

            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Logout()
        {
            WebUtil.RemoveManageUserSession();
            return Redirect("/Manage/Login");
        }
    }
}
