using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 短信
    /// </summary>
    public class UT_SMS : UT_Entity
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 任务ID(接收的短信任务ID为null)
        /// </summary>
        public int? TId { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string DevName { get; set; }
        /// <summary>
        /// 发送端口
        /// 一个或多个（逗号，短横线连接）发送端口（从1开始）
        /// </summary>
        public string Port { get; set; }
        /// <summary>
        /// sim卡的iccid
        /// 一个或多个（逗号分隔）发送
        /// </summary>
        public string IccId { get; set; }
        /// <summary>
        /// 一个或多个（逗号连接）短信接收者号码
        /// </summary>
        public string To { get; set; }
        /// <summary>
        /// 发送人
        /// </summary>
        public string Fm { get; set; }
        /// <summary>
        /// 短信内容
        /// </summary>
        public string SMSContent { get; set; }
        /// <summary>
        /// 发送时间/接收时间(时间错)
        /// </summary>
        public int SMSTime { get; set; }
        /// <summary>
        /// 已读未读
        /// </summary>
        public bool IsRead { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public SMSStatusType Status { get; set; }
        /// <summary>
        /// 失败原因
        /// </summary>
        public string ErrorMsg { get; set; }
        /// <summary>
        /// 1发送/0接收
        /// </summary>
        public bool IsSend { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public int UpdateDate { get; set; }
        public virtual UT_Users UT_Users { get; set; }
    }

    /// <summary>
    /// 短信发送类型
    /// </summary>
    //public enum MsgType
    //{
    //    Text = 0,
    //    MMS = 1
    //}

    /// <summary>
    /// 状态
    /// </summary>
    public enum SMSStatusType
    {
        /// <summary>
        /// 处理中
        /// </summary>
        Int = 0,
        /// <summary>
        /// 处理成功
        /// </summary>
        Success = 1,
        /// <summary>
        /// 处理失败
        /// </summary>
        Error = 2
    }
}
