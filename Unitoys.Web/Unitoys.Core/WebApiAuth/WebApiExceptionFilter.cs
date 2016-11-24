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

            LoggerHelper.Error(ErrorMsg, context.Exception);

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