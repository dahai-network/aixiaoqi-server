using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Unitoys.Core;
using Unitoys.IServices;
using Unitoys.Model;

namespace Unitoys.WebApi.Controllers
{
    public class ClientApiController : ApiController
    {

        IDeviceGoipService _deviceGoipService;

        public ClientApiController(IDeviceGoipService deviceGoipService)
        {
            this._deviceGoipService = deviceGoipService;
        }
        
        public class QueryResult
        {
            public string msgType { get; set; }

            public int msgId { get; set; }

            public int code { get; set; }
        }
        public class QueryUpdateSlot
        {
            public string msgType { get; set; }
            public int msgId { get; set; }

            public string goipSlot { get; set; }

            public string simSlot { get; set; }
            /// <summary>
            /// 通道状态
            /// </summary>
            public string status { get; set; }
            /// <summary>
            /// sim信号值
            /// </summary>
            public int simSig { get; set; }
            /// <summary>
            /// 运营商信息
            /// </summary>
            public string provider { get; set; }
            /// <summary>
            /// sim卡余额
            /// </summary>
            public decimal balanceMoney { get; set; }
            /// <summary>
            /// sim卡的IMEI码
            /// </summary>
            public string imei { get; set; }
            /// <summary>
            /// SIM卡的IMSI
            /// </summary>
            public string imsi { get; set; }
            /// <summary>
            /// SIM卡号码
            /// </summary>
            public string simNum { get; set; }
        }
        public class QueryReportSim
        {
            public string msgType { get; set; }
            public int msgId { get; set; }
            /// <summary>
            /// 通道是否有卡，1 为有卡，0为无卡
            /// </summary>
            public int simFree { get; set; }
            /// <summary>
            /// 通道号
            /// </summary>
            public string simSlot { get; set; }
            /// <summary>
            /// 是否有建立会话通道，1 为占用，0为没有占用(就是没有和GOIP建立会话)
            /// </summary>
            public int simUsed { get; set; }
            /// <summary>
            /// 当simUsed为1的时候，此字段才有效
            /// </summary>
            public string goipSlot { get; set; }
        }
        public class QueryRegDeivce
        {
            public string msgType { get; set; }
            public int msgId { get; set; }
            public string deviceType { get; set; }
            public string mac { get; set; }
            public int did { get; set; }
            public string version { get; set; }
            public int deviceStatus { get; set; }
            public string status { get; set; }
            public int maxPorts { get; set; }
            public string ip { get; set; }
            public string userName { get; set; }
        }
    }
    
}
