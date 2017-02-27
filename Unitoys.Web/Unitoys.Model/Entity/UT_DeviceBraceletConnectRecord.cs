using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 手环设备TCP连接记录
    /// </summary>
    public class UT_DeviceBraceletConnectRecord : UT_Entity
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// TCP会话ID
        /// </summary>
        public string SessionId { get; set; }
        /// <summary>
        /// 手环的设备号
        /// </summary>
        public string IMEI { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public int CreateDate { get; set; }
        /// <summary>
        /// 连接时间
        /// </summary>
        public int ConnectDate { get; set; }
        /// <summary>
        /// 断开时间
        /// </summary>
        public int? DisconnectDate { get; set; }
        /// <summary>
        /// 分配的设备名和端口号
        /// </summary>
        public string EjoinDevNameAndPort { get; set; }
        public virtual UT_Users UT_Users { get; set; }
    }
}
