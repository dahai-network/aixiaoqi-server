using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Unitoys.Core
{
    /// <summary>
    /// webapi 访问权限和登录验证
    /// </summary>
    public class AuthenticationAttribute : AbsBaseAuthenticationAttribute  
    {
      
        /// <summary>
        /// 私钥
        /// </summary>
        /// <param name="partner">合作者ID</param>
        /// <returns></returns>
        protected override string GetPartnerKey(string Partner)
        {
            //TODO:从配置文件里面读取私钥
            return ConfigurationManager.AppSettings[String.Format("partner_{0}", Partner)];
        }
        /// <summary>
        /// 加密密钥值
        /// </summary>
        /// <param name="partner">合作者ID</param>
        /// <param name="expires">时间戳</param>
        /// <param name="partnerKey">私钥</param>
        /// <returns></returns>
        protected override string GetSecuritySign(string partner, string expires, string partnerKey)
        {

            return SecureHelper.MD5(String.Format("{0}{1}{2}", partner, expires, partnerKey));
        }

        protected override bool SkipAuthentication(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            return actionContext.ActionDescriptor.GetCustomAttributes<NoAuthenticateAttribute>().Any()
                || actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<NoAuthenticateAttribute>().Any();
        }

        protected override bool SkipLogin(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            return actionContext.ActionDescriptor.GetCustomAttributes<NoLoginAttribute>().Any()
                || actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<NoLoginAttribute>().Any();
        }
    }
}