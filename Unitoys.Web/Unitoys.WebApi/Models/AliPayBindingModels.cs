using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.WebApi.Models
{
    public class NotifyAsyncBindingModel
    {
        /// <summary>
        /// 异步通知ID
        /// </summary>
        public string notify_id { get; set; }
        /// <summary>
        /// 签名
        /// </summary>
        public string sign { get; set; }
        /// <summary>
        /// 商户订单号
        /// </summary>
        public string out_trade_no { get; set; }
        /// <summary>
        /// 支付宝交易号
        /// </summary>
        public string trade_no { get; set; }
        /// <summary>
        /// 交易号
        /// </summary>
        public string trade_status { get; set; }
    }
}
