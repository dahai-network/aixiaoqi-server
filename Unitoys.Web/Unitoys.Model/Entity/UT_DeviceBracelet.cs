using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 终端手环设备管理
    /// </summary>
    public class UT_DeviceBracelet : UT_Entity
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 设备号，唯一的
        /// </summary>
        public string IMEI { get; set; }
        /// <summary>
        /// 绑定日期
        /// </summary>
        public int CreateDate { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public int UpdateDate { get; set; }
        /// <summary>
        /// 固件版本号
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// 电量
        /// </summary>
        public string Power { get; set; }
        /// <summary>
        /// 连接时间
        /// </summary>
        public int ConnectDate { get; set; }
        /// <summary>
        /// 设备类型
        /// </summary>
        public DeviceType DeviceType { get; set; }

        public virtual UT_Users UT_Users { get; set; }
    }

    /// <summary>
    /// 设备类型
    /// </summary>
    public enum DeviceType
    {
        //手环
        [Description("手环设备")]
        Bracelet = 0,
        //双待王
        [Description("钥匙扣设备")]
        KeyChain = 1
    }
}
