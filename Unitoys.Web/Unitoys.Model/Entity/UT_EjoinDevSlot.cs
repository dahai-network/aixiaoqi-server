using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 设备端口
    /// </summary>
    public class UT_EjoinDevSlot : UT_Entity
    {
        /// <summary>
        /// 端口的IMEI码
        /// </summary>
        public string IMEI { get; set; }
        /// <summary>
        /// 端口号
        /// </summary>
        public int PortNum { get; set; }
        /// <summary>
        /// SIM卡ICCID
        /// </summary>
        public string ICCID { get; set; }
        /// <summary>
        /// SIM卡号码
        /// </summary>
        public string SimNum { get; set; }

        /// <summary>
        /// 设备端口状态
        /// </summary>
        public DevPortStatus Status { get; set; }
        public Guid? EjoinDevId { get; set; }
        public virtual UT_EjoinDev UT_EjoinDev { get; set; }

        public Guid? UserId { get; set; }
        public virtual UT_Users UT_Users { get; set; }
    }
    public enum DevPortStatus
    {
        [Description("无SIM卡")]
        NOSIM = "0",
        [Description("有SIM卡，没有注册")]
        NOREG = "1",
        [Description("SIM卡注册中")]
        REGING = "2",
        [Description("SIM卡注册成功")]
        REGSUCCESS = "3",
        [Description("正在呼叫")]
        CALLING = "4",
        [Description("告警")]
        WARNING = "5",
        [Description("注册失败")]
        REGERROR = "6",
        [Description("设备锁卡")]
        DEVLOCKCARD = "7",
        [Description("运营商锁卡")]
        OPTLOCKCARD = "8",
        [Description("读SIM卡错误")]
        READERROR = "9",
        [Description("端口被用户禁用")]
        PORTDISABLE = "A"
    }
}
