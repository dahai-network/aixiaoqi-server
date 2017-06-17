using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 用户设备使用记录
    /// </summary>
    public class UT_DeviceBraceletUsageRecord : UT_Entity
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
        /// 固件版本号
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// 电量
        /// </summary>
        public string Power { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public int CreateDate { get; set; }
        public virtual UT_Users UT_Users { get; set; }
    }
}
