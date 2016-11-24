using Senparc.Weixin;
using Senparc.Weixin.Exceptions;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Unitoys.Core;
using Unitoys.IServices;
using Unitoys.Model;

namespace Unitoys.Web.Areas.wx.Controllers
{

    [AllowAnonymous]
    public class OAuth2Controller : Controller
    {
        private IUserWxService _userWxService;
        private IUserService _userService;

        public OAuth2Controller(IUserWxService userWxService, IUserService userService)
        {
            this._userWxService = userWxService;
            this._userService = userService;
        }

        //下面换成账号对应的信息，也可以放入web.config等地方方便配置和更换
        private string appId = ConfigurationManager.AppSettings["WeixinAppId"];
        private string secret = ConfigurationManager.AppSettings["WeixinAppSecret"];

        public ActionResult Index()
        {
            return Redirect(OAuthApi.GetAuthorizeUrl(appId, UTConfig.SiteConfig.SiteHost + "/wx/oauth2/UserInfoCallback", "UnitoysWxApi", OAuthScope.snsapi_userinfo));
        }
        /// <summary>
        /// OAuthScope.snsapi_userinfo方式回调
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public async Task<ActionResult> UserInfoCallback(string code, string state)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Content("您拒绝了授权！");
            }

            if (state != "UnitoysWxApi")
            {
                return Content("验证失败！请从正规途径进入！");
            }

            OAuthAccessTokenResult result = null;

            //通过，用code换取access_token
            try
            {
                result = OAuthApi.GetAccessToken(appId, secret, code);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
            if (result.errcode != ReturnCode.请求成功)
            {
                return Content("错误：" + result.errmsg);
            }

            WebHelper.SetCookie("openid", result.openid);
            WebHelper.SetCookie("login_access_token", result.access_token);

            if (_userWxService.CheckOpenIdExist(result.openid))
            {
                return Redirect("/wx/userinfo");
            }
            return Redirect("/wx/oauth2/BindUserLogin");
           
        }
        public async Task<ActionResult> BindUserLogin()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> UserBindWx(string tel, string password)
        {
            if (!ValidateHelper.IsMobile(tel) || (password.Length < 6 || password.Length > 12))
            {
                return Content("您输入的账号信息不正确");
            }
            var login_access_token = WebHelper.GetCookie("login_access_token");
            var openid = WebHelper.GetCookie("openid");

            if (openid == "")
            {
                return Content("请从微信公众号进入绑定！");
            }

            UT_Users user = await _userService.CheckUserLoginTelAsync(tel, password);

             if (user != null)
             {
                 if (user.Status == 0)
                 {
                     //因为第一步选择的是OAuthScope.snsapi_userinfo，这里可以进一步获取用户详细信息
                     try
                     {
                         

                         OAuthUserInfo userInfo = OAuthApi.GetUserInfo(login_access_token, openid);


                         if (_userWxService.CheckOpenIdExist(userInfo.openid))
                         {
                             return Redirect("/wx/userinfo");
                         }
                         else if (_userWxService.CheckUserBindWx(user.ID))
                         {
                             return Content("您输入的账号已经绑定了其他微信用户");
                         }
                         else
                         {
                             UT_UsersWx model = new UT_UsersWx();
                             model.HeadImgUrl = userInfo.headimgurl;
                             model.NickName = userInfo.nickname;
                             model.OpenId = userInfo.openid;
                             model.Sex = userInfo.sex;
                             model.UnionId = userInfo.unionid;
                             model.UserId = user.ID;
                             if (await _userWxService.InsertAsync(model))
                             {
                                 return Content("0");
                             }
                             else
                             {
                                 return Content("绑定失败！");
                             }
                         }
                     }
                     catch (ErrorJsonResultException ex)
                     {
                         return Content(ex.Message);
                     }
                 }
             }
           
            
            return Content("账号密码错误！");
        }
    }
}