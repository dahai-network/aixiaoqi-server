using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Unitoys.Model;

namespace Unitoys.Core
{
    /// <summary>  
    /// WebAPI防篡改签名验证抽象基类Attribute  
    /// </summary>  
    public abstract class AbsBaseAuthenticationAttribute : IAuthenticationFilter
    {
        public System.Threading.Tasks.Task AuthenticateAsync(HttpAuthenticationContext context, System.Threading.CancellationToken cancellationToken)
        {
            //判断访问的接口是否跳过验证。
            if (SkipAuthentication(context.ActionContext))
            {
                return Task.FromResult(0);
            }

            //获取Asp.Net对应的Request
            var request = ((HttpContextWrapper)context.Request.Properties["MS_HttpContext"]).Request;

            NameValueCollection getCollection = request.Headers; //此签名要求Partner及Sign均通过QueryString传递

            //todo 由于测试api中header发送有问题，暂时使用此种方式进行测试
            if (request.QueryString["expires"] == "1471316792")
            {
                getCollection = request.QueryString;
            }

            if (getCollection != null && getCollection.Count > 0)
            {
                string Partner = getCollection["partner"];  //合作者ID
                string Expires = getCollection["expires"];  //时间戳
                string Sign = getCollection["sign"];        //MD5签名
                string Token = getCollection["token"];
                if (!string.IsNullOrWhiteSpace(Partner)     //必须包含partner
                    && !string.IsNullOrWhiteSpace(Expires)   //必须包含expires
                    && !string.IsNullOrWhiteSpace(Sign)      //必须包含sign
                    && Regex.IsMatch(Sign, "^[0-9A-Za-z]{32}$"))//sign必须为32位Md5摘要
                {


                    //获取partner对应的key  
                    //这里暂时只做了合作key校验，不做访问权限校验，如有需要，此处可进行调整，建议RBAC
                    string partnerKey = this.GetPartnerKey(Partner);

                    //判断合作id的密钥是否存在，判断请求时间戳于现在时间对比，大于3分钟属于非法请求，时间戳是参与到了签名加密，有效避免了外界伪造签名。

                    //TODO 测试接口-暂时写死判断的时间戳为1471316792时,不判断时间
                    if (!string.IsNullOrWhiteSpace(partnerKey) && ((CommonHelper.ConvertDateTimeInt(DateTime.Now) - TypeHelper.StringToInt(Expires)) < 18000 || Expires == "1471316792"))
                    {
                        //if (!string.IsNullOrWhiteSpace(partnerKey) && (CommonHelper.ConvertDateTimeInt(DateTime.Now) - TypeHelper.StringToInt(Expires)) < 18000)
                        //{
                        if (!string.IsNullOrWhiteSpace(partnerKey))
                        {
                            //根据请求数据获取MD5签名
                            string vSign = this.GetSecuritySign(Partner, Expires, partnerKey);

                            if (string.Equals(Sign, vSign, StringComparison.OrdinalIgnoreCase))
                            {
                                //判断访问的接口是否跳过登录。
                                if (SkipLogin(context.ActionContext))
                                {
                                    return Task.FromResult(0);
                                }

                                LoginUserInfo model = WebUtil.GetApiUserSession(Token);
                                if (model == null)
                                {
                                    context.ErrorResult = new AuthenticationFailureResult(new ErrorResult() { status = -999, msg = "token incorrect" }, context.Request, HttpStatusCode.OK);
                                }
                                //验证通过
                                return Task.FromResult(0);
                            }
                        }
                    }
                }
            }
            context.ErrorResult = new AuthenticationFailureResult(new ErrorResult() { status = -401, msg = "parameter invalid" }, context.Request, HttpStatusCode.Unauthorized);
            return Task.FromResult(0);
        }

        /// <summary>
        /// 基于Challenge，如果context result为AuthenticationFailureResult，则会建立Unauthorized response返回。
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public System.Threading.Tasks.Task ChallengeAsync(HttpAuthenticationChallengeContext context, System.Threading.CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        public bool AllowMultiple
        {
            get { return false; }
        }

        /// <summary>  
        /// 获取合作号对应的合作Key,如果未能获取，则返回空字符串或者null  
        /// </summary>  
        /// <param name="partner"></param>  
        /// <returns></returns>  
        protected abstract string GetPartnerKey(string partner);

        /// <summary>
        /// 计算MD5签名
        /// </summary>
        /// <param name="partner"></param>
        /// <param name="expires"></param>
        /// <param name="partnerKey"></param>
        /// <returns></returns>
        protected abstract string GetSecuritySign(string partner, string expires, string partnerKey);

        /// <summary>
        /// 判断是否跳过验证
        /// </summary>
        /// <param name="actionContext">HttpActionContext</param>
        /// <returns></returns>
        protected abstract bool SkipAuthentication(HttpActionContext actionContext);

        /// <summary>
        /// 判断是否跳过登录
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        protected abstract bool SkipLogin(HttpActionContext actionContext);
    }

    /// <summary>
    /// 验证失败的ActionResult
    /// </summary>
    public class AuthenticationFailureResult : IHttpActionResult
    {
        public AuthenticationFailureResult(ErrorResult reasonPhrase, HttpRequestMessage request,HttpStatusCode statusCode)
        {
            ReasonPhrase = reasonPhrase;
            Request = request;
            StatusCode = statusCode;

        }

        public ErrorResult ReasonPhrase { get; private set; }

        public HttpRequestMessage Request { get; private set; }
        public HttpStatusCode StatusCode { get; private set; }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute());
        }

        private HttpResponseMessage Execute()
        {
            //建立未通过验证的Response
            return Request.CreateResponse<ErrorResult>(StatusCode, ReasonPhrase, new MediaTypeHeaderValue("application/json"));
        }
    }
    public class ErrorResult
    {
        public int status { get; set; }

        public string msg { get; set; }
    }
}