using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 通话记录
    /// </summary>
    public class UT_SpeakRecord : UT_Entity
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 主叫号码
        /// </summary>
        public string DeviceName { get; set; }
        /// <summary>
        /// 被叫号码
        /// </summary>
        public string CalledTelNum { get; set; }
        /// <summary>
        /// 拨打前剩余通话秒数
        /// </summary>
        public int CallAgoRemainingCallSeconds { get; set; }
        /// <summary>
        /// 开始拨打时间
        /// </summary>
        public DateTime CallStartTime { get; set; }
        /// <summary>
        /// 结束通话时间
        /// </summary>
        public DateTime CallStopTime { get; set; }
        /// <summary>
        /// 通话时间
        /// </summary>
        public int CallSessionTime { get; set; }
        /// <summary>
        /// 本次通话费用
        /// </summary>
        public decimal TotalAmount { get; set; }
        /// <summary>
        /// 拨打源IP
        /// </summary>
        public string CallSourceIp { get; set; }
        /// <summary>
        /// 服务器IP
        /// </summary>
        public string CallServerIp { get; set; }
        /// <summary>
        /// 挂断方：source主叫挂断，dest被叫挂断
        /// </summary>
        public string Acctterminatedirection { get; set; }
        /// <summary>
        /// 通话类型，1：回拨，2：直拨
        /// </summary>
        public int CallType { get; set; }
        public SpeakRecordStatus Status { get; set; }

        public virtual UT_Users UT_Users { get; set; }
    }
    public enum SpeakRecordStatus
    {
        /// <summary>
        /// 正常
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 漏接
        /// </summary>
        Missing = 1,
    }
}
