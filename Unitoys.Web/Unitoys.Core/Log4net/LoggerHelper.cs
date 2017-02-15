using Aliyun.Api.SLS;
using Aliyun.Api.SLS.Data;
using Aliyun.Api.SLS.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Core
{
    public class LoggerHelper
    {
        static readonly log4net.ILog loginfo = log4net.LogManager.GetLogger("loginfo");
        static readonly log4net.ILog logerror = log4net.LogManager.GetLogger("logerror");
        static readonly log4net.ILog logmonitor = log4net.LogManager.GetLogger("logmonitor");
        static readonly log4net.ILog logwebapimonitor = log4net.LogManager.GetLogger("webapilogmonitor");
        static readonly log4net.ILog logoperation = log4net.LogManager.GetLogger("logoperation");

        static String endpoint = UTConfig.SiteConfig.AliyunLogEndpoint; //选择与上面步骤创建 project 所属区域匹配的日志服务 Endpoint
        static String accessKeyId = "LTAItvn17baV1JR6";  //使用你的阿里云访问秘钥     
        static String accessKeySecret = "ZofsUP8iJnSsb6JCqAloLurDpFc9zD"; //使用你的阿里云访问秘钥 AccessKeySecret
        static String project = "unitoys";      //上面步骤创建的项目名称
        static String logstore = "unitoys_webapi_monitor";    //上面步骤创建的日志库名称
        //构建一个客户端实例
        static SLSClient client = new SLSClient(endpoint, accessKeyId, accessKeySecret);

        public static void Error(string ErrorMsg, Exception ex = null)
        {
            if (ex != null)
            {
                logerror.Error(ErrorMsg, ex);
            }
            else
            {
                logerror.Error(ErrorMsg);
            }
        }

        public static void Info(string Msg)
        {
            loginfo.Info(Msg);
        }

        public static void Monitor(string Msg)
        {
            logmonitor.Info(Msg);
        }

        public static void WebApiMonitor(WebApiMonitorLog model)
        {
            //logwebapimonitor.Info(Msg);

            //写入日志
            List<LogItem> logs = new List<LogItem>();

            LogItem item = new LogItem();
            item.Time = (uint)CommonHelper.GetDateTimeInt();
            item.PushBack(new LogContent("ControllerName", model.ControllerName));
            item.PushBack(new LogContent("Name", model.ActionName));
            item.PushBack(new LogContent("开始时间", model.ExecuteStartTime + ""));
            item.PushBack(new LogContent("结束时间", model.ExecuteEndTime + ""));
            item.PushBack(new LogContent("总时间", model.ExecuteElapsedMilliseconds + "毫秒"));
            item.PushBack(new LogContent("请求内容", model.RequestStr));
            item.PushBack(new LogContent("返回内容", model.ResponseStr));
            logs.Add(item);

            String topic = model.UserTel;    //选择合适的日志主题
            String source = model.Source;    //选择合适的日志来源（如 IP 地址）
            client.PutLogs(new PutLogsRequest(project, logstore, topic, source, logs));
        }
        public static void OperationRecord(Unitoys.Model.UT_OperationRecord Msg)
        {
            logoperation.Info(Msg);
        }
    }
}
