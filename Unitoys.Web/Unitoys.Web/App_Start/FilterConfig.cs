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

            //监控事件
            StatisticsTrackerAttribute statisticsTrackerAttribute = new StatisticsTrackerAttribute();
            statisticsTrackerAttribute.OnActionExecutingEvent += statisticsTrackerAttribute_OnActionExecutingEvent;
            statisticsTrackerAttribute.OnActionExecutedEvent += statisticsTrackerAttribute_OnActionExecutedEvent;

            //监控日志
            filters.Add(statisticsTrackerAttribute);
        }



        /// <summary>
        /// Action执行前
        /// </summary>
        /// <param name="filterContext"></param>
        static async void statisticsTrackerAttribute_OnActionExecutingEvent(ActionExecutingContext filterContext)
        {
            if (WebUtil.GetManageUserSession() != null && filterContext.ActionDescriptor.GetCustomAttributes(typeof(RequireRolesOrPermissionsAttribute), false).Length == 0)
                return;

            //操作日志
            if (WebUtil.GetManageUserSession() != null && filterContext.ActionDescriptor.GetCustomAttributes(typeof(RequireRolesOrPermissionsAttribute), false).Length > 0)
            {
                string controllerName = filterContext.RouteData.Values["controller"] as string;
                string actionName = filterContext.RouteData.Values["action"] as string;
                string data = "";

                if (actionName == "Delete")
                {
                    var services = System.Type.GetType("Unitoys.IServices.I" + controllerName + "Service,Unitoys.IServices");
                    if (services == null)
                    {
                        return;
                    }
                    object obj = Unitoys.Ioc.NinjectRegister.GetKernelService(services);
                    var method = obj.GetType().GetMethod("GetEntityByIdAsync");
                    var task = method.Invoke(obj, new object[] { (System.Guid)filterContext.ActionParameters["ID"] }) as System.Threading.Tasks.Task;
                    await task;
                    var taskResult = task.GetType().GetProperty("Result").GetValue(task, null);
                    if (taskResult != null)
                        data = Newtonsoft.Json.JsonConvert.SerializeObject(taskResult);
                }

                filterContext.Controller.ViewData["_thisOnActionMonitorLog_Operation_ActionParameters"] = filterContext.ActionParameters;
                filterContext.Controller.ViewData["_thisOnActionMonitorLog_Operation_Data"] = data;

            }



        }

        /// <summary>
        /// Action执行后
        /// </summary>
        /// <param name="filterContext"></param>
        static void statisticsTrackerAttribute_OnActionExecutedEvent(ActionExecutedContext filterContext)
        {
            if (WebUtil.GetManageUserSession() != null && filterContext.ActionDescriptor.GetCustomAttributes(typeof(RequireRolesOrPermissionsAttribute), false).Length == 0)
                return;
            if (WebUtil.GetManageUserSession() == null)
                return;

            string controllerName = filterContext.RouteData.Values["controller"] as string;
            string actionName = filterContext.RouteData.Values["action"] as string;
            var ActionParameter = filterContext.Controller.ViewData["_thisOnActionMonitorLog_Operation_ActionParameters"] as System.Collections.Generic.IDictionary<string, object>;
            string Data = filterContext.Controller.ViewData["_thisOnActionMonitorLog_Operation_Data"] as string;

            Unitoys.Model.UT_OperationRecord OperationRecord = new Unitoys.Model.UT_OperationRecord()
            {
                Url = controllerName + "/" + actionName,
                Parameter = Newtonsoft.Json.JsonConvert.SerializeObject(ActionParameter, new Newtonsoft.Json.JsonSerializerSettings { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore }),
                Data = Data,
                Response = Newtonsoft.Json.JsonConvert.SerializeObject(filterContext.Result, new Newtonsoft.Json.JsonSerializerSettings { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore }),
                ManageUserId = WebUtil.GetManageUserSession().ID,
                CreateDate = CommonHelper.GetDateTimeInt(),
                Remark = actionName,
            };

            LoggerHelper.OperationRecord(OperationRecord);
        }
    }
}