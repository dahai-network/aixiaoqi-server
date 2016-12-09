using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace Unitoys.Core
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class WebApiStatisticsTrackerAttribute : ActionFilterAttribute
    {
        private const string StopwatchKey = "StopwatchFilter.Value";
        private const string MonLogKey = "MonLog.Key";

        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);

            WebApiMonitorLog MonLog = new WebApiMonitorLog();
            MonLog.ExecuteStartTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.ffff", DateTimeFormatInfo.InvariantInfo));
            MonLog.ControllerName = actionContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            MonLog.ActionName = actionContext.ActionDescriptor.ActionName;

            actionContext.Request.Properties[StopwatchKey] = Stopwatch.StartNew();

            actionContext.Request.Properties[MonLogKey] = MonLog;

            var curContext = System.Web.HttpContext.Current;

            StringBuilder requestStr = new StringBuilder();
            requestStr.AppendLine();
            if (curContext != null)
            {
                requestStr.AppendLine("Form ：" + curContext.Request.Form.ToString());
                requestStr.AppendLine("QueryString ：" + curContext.Request.QueryString.ToString());
            }
            requestStr.AppendLine("Content ：" + actionContext.Request.Content.ReadAsStringAsync().Result);
            requestStr.AppendLine("InputStream ：" + new System.IO.StreamReader(System.Web.HttpContext.Current.Request.InputStream).ReadToEnd());
            MonLog.RequestStr = requestStr.ToString();
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);

            Stopwatch stopwatch = (Stopwatch)actionExecutedContext.Request.Properties[StopwatchKey];

            WebApiMonitorLog MonLog = actionExecutedContext.Request.Properties[MonLogKey] as WebApiMonitorLog;
            MonLog.ExecuteEndTime = DateTime.Now;
            MonLog.ExecuteElapsedMilliseconds = stopwatch.ElapsedMilliseconds;

            string responseStr = actionExecutedContext.Response.Content.ReadAsStringAsync().Result;
            MonLog.ResponseStr = System.Environment.NewLine + responseStr;

            LoggerHelper.WebApiMonitor(MonLog.GetLoginfo());
        }
    }

    /// <summary>
    /// 监控日志对象
    /// </summary>
    public class WebApiMonitorLog
    {
        public string ControllerName
        {
            get;
            set;
        }
        public string ActionName
        {
            get;
            set;
        }

        public DateTime ExecuteStartTime
        {
            get;
            set;
        }
        public DateTime ExecuteEndTime
        {
            get;
            set;
        }

        public long ExecuteElapsedMilliseconds
        {
            get;
            set;
        }

        public string RequestStr { get; set; }
        public string ResponseStr { get; set; }
        /// <summary>
        /// 获取监控指标日志
        /// </summary>
        /// <param name="mtype"></param>
        /// <returns></returns>
        public string GetLoginfo()
        {
            string Action = "Action执行时间监控：";
            string Name = "Action";

            string Msg = @"
            {0}
            ControllerName：{1}Controller
            {6}Name:{2}
            开始时间：{3}
            结束时间：{4}
            总 时 间：{5}毫秒
            请求内容：{7}
            返回内容：{8}";

            return string.Format(Msg,
                Action,
                ControllerName,
                ActionName,
                ExecuteStartTime,
                ExecuteEndTime,
                ExecuteElapsedMilliseconds,
                Name,
                RequestStr,
                ResponseStr);
        }
    }
}
