using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.WebApi.Models
{
    public class AddSMSBindingModel
    {
        public int TId { get; set; }
        public string DevName { get; set; }
        public string Port { get; set; }
        public string IccId { get; set; }
        public string To { get; set; }
        public string SMSContent { get; set; }
    }

    public class SendSMSBindingModelForContent
    {
        public string To { get; set; }
        public string SMSContent { get; set; }
    }
    public class SendRetryBindingModelForContent
    {
        public Guid SMSID { get; set; }
    }

    public class AddATUSSDBindingModel
    {
        /// <summary>
        /// at或ussd
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string Devname { get; set; }
        /// <summary>
        /// 一个或多个（逗号，短横线连接）发送端口（从1开始）
        /// </summary>
        public string Port { get; set; }
        /// <summary>
        /// 一个或多个（逗号分隔）发送sim卡的iccid
        /// </summary>
        public string Iccid { get; set; }
        /// <summary>
        /// 命令内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 编码集（utf8|base64）
        /// 默认：utf8
        /// </summary>
        public string Chs { get; set; }
    }


    public class CallTransferBindingModel
    {
        /// <summary>
        /// 设备名称
        /// </summary>
        //public string Devname { get; set; }
        /// <summary>
        /// 一个或多个（逗号，短横线连接）发送端口（从1开始）
        /// </summary>
        //public string Port { get; set; }
        /// <summary>
        /// 一个或多个（逗号分隔）发送sim卡的iccid
        /// </summary>
        public string Iccid { get; set; }
    }

    public class DeleteSMSBindingModel
    {
        public Guid Id { get; set; }
        public Guid[] Ids { get; set; }
        public string Tel { get; set; }
        public string[] Tels { get; set; }
    }
}
