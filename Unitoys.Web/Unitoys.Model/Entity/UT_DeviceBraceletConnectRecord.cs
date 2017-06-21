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
        /// 注册成功时间
        /// </summary>
        public int? RegSuccessDate { get; set; }
        /// <summary>
        /// 分配的设备名和端口号
        /// </summary>
        public string EjoinDevNameAndPort { get; set; }
        /// <summary>
        /// IOS-PushKitToken
        /// </summary>
        public string PushKitToken { get; set; }
        /// <summary>
        /// 断开描述
        /// </summary>
        public EnumDisconnectStatus DisconnectStatus { get; set; }
        /// <summary>
        /// 客户端类型
        /// 01 IOS
        /// 02 安卓
        /// </summary>
        public EnumClientType? ClientType { get; set; }
        public string Remark { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        //public string Remark { get; set; }
        public virtual UT_Users UT_Users { get; set; }
    }
    public enum EnumClientType
    {
        未知 = 0,
        iOS = 1,
        Android = 2
    }
    public enum EnumDisconnectStatus
    {
        不明 = 0,

        //设备相关
        设备登录移除 = 11,
        设备05超时 = 12,
        设备TCP断开 = 13,

        //用户相关
        用户收到断开连接0F = 21,
        用户05超时 = 22,
        端口05超时 = 23,
        用户登录移除 = 24,
        /// <summary>
        /// 同一IOS手机切换用户登录
        /// </summary>
        用户切换移除 = 25,
        用户秒连接移除 = 26,
        用户主动断开 = 28,

        //端口相关
        端口登录移除 = 31,
        秒连接移除端口 = 32,
    }
}
