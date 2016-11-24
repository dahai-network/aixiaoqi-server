using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Unitoys.Core;
using WebApiThrottle;

namespace Unitoys.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            //错误处理
            config.Filters.Add(new WebApiExceptionFilter());
            //签名验证，用户登录token验证
            config.Filters.Add(new AuthenticationAttribute());
            //监控日志
            config.Filters.Add(new WebApiStatisticsTrackerAttribute());

            //webapi 限制访问频率
            //config.MessageHandlers.Add(new ThrottlingHandler()
            //{
            //    Policy = new ThrottlePolicy()
            //    {
            //        IpThrottling = true,
            //        EndpointThrottling = true,
            //        EndpointRules = new Dictionary<string, RateLimits>
            //        { 
            //            { "api/Confirmation/SendSMS", new RateLimits { PerMinute = 2 } }
            //        }
            //    },
            //    Repository = new CacheRepository()
            //});

            config.Routes.MapHttpRoute(
                name: "ClientApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
