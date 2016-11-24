using System.Web;
using System.Web.Mvc;
using Unitoys.Core;

namespace Unitoys.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //权限控制过滤器
            filters.Add(new UnitoysAuthorizeAttribute());

            filters.Add(new HandleErrorAttribute());
            //监控日志
            filters.Add(new StatisticsTrackerAttribute());
        }
    }
}