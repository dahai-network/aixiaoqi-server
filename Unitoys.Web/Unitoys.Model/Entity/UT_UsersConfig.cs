using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 终端手环设备管理
    /// </summary>
    public class UT_UsersConfig : UT_Entity
    {

        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 名称
        /// NotificaCall(来电通知)
        /// NotificaSMS(短信通知)
        /// NotificaWeChat(微信通知)
        /// NotificaQQ(QQ通知)
        /// LiftWristLight(手腕抬起亮屏)
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 状态
        /// 禁用/启用
        /// </summary>
        public UsersConfigStatus Status { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public int CreateDate { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public int UpdateDate { get; set; }

        public virtual UT_Users UT_Users { get; set; }
    }
    public enum UsersConfigStatus
    {
        /// <summary>
        /// 禁用
        /// </summary>
        Disabled = 0,
        /// <summary>
        /// 启用
        /// </summary>
        Enable = 1
    }
}
