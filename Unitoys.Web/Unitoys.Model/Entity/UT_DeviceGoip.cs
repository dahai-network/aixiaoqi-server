using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// goip设备
    /// </summary>
    public class UT_DeviceGoip : UT_Entity
    {
        /// <summary>
        /// 设备Mac编号
        /// </summary>
        public string Mac { get; set; }
        /// <summary>
        /// 设备名
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// 设备端口状态
        /// </summary>
        public DeviceGoipStatus Status { get; set; }
        /// <summary>
        /// 端口编号
        /// </summary>
        public int Port { get; set; }

        public int CreateDate { get; set; }

        public int UpdateDate { get; set; }

        public Guid? UserId { get; set; }

        public string IccId { get; set; }

        public virtual UT_Users UT_Users { get; set; }

    }
    public enum DeviceGoipStatus
    {
        /// <summary>
        /// 未使用
        /// </summary>
        Enable = 0,
        /// <summary>
        /// 已使用
        /// </summary>
        Disabled = 1
    }
}
