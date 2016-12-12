using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using Unitoys.Core;

namespace Unitoys.Core
{
    public class WebApiExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {

            string ErrorMsg = string.Format("在执行{0}时产生异常", context.Request.RequestUri.ToString());

            var curContext = System.Web.HttpContext.Current;
            System.Text.StringBuilder requestStr = new System.Text.StringBuilder();
            requestStr.AppendLine();
            if (curContext != null)
            {
                requestStr.AppendLine("Form ：" + curContext.Request.Form.ToString());
                requestStr.AppendLine("QueryString ：" + curContext.Request.QueryString.ToString());
            }
            requestStr.AppendLine("Content ：" + context.Request.Content.ReadAsStringAsync().Result);
            requestStr.AppendLine("InputStream ：" + new System.IO.StreamReader(System.Web.HttpContext.Current.Request.InputStream).ReadToEnd());

            var user = WebUtil.GetApiUserSession();
            if (user != null)
            {
                requestStr.AppendLine("user：" + user.Tel);
                requestStr.AppendLine("partner：" + System.Web.HttpContext.Current.Request.Headers["partner"]);
                requestStr.AppendLine("expires：" + System.Web.HttpContext.Current.Request.Headers["expires"]);
                requestStr.AppendLine("sign：" + System.Web.HttpContext.Current.Request.Headers["sign"]);
            }

            LoggerHelper.Error(ErrorMsg + Environment.NewLine + requestStr.ToString(), context.Exception);

            string json = "{\"status\":\"998\",\"msg\":\"请求失败！请稍后重试\"}";
            var content = new System.Net.Http.StringContent(json, System.Text.Encoding.GetEncoding("UTF-8"), "application/json");

            context.Response = new System.Net.Http.HttpResponseMessage()
            {
                Content = content
            };

            base.OnException(context);
        }
    }
}