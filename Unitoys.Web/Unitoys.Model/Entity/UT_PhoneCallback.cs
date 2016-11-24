using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 电话回拨记录
    /// </summary>
    public class UT_PhoneCallback : UT_Entity
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 被叫号码
        /// </summary>
        public string To { get; set; }
        /// <summary>
        /// 主叫号码
        /// </summary>
        public string From { get; set; }
        /// <summary>
        /// 最大通话时长
        /// </summary>
        public int MaximumPhoneCallTime { get; set; }   
        /// <summary>
        /// 1：隐号 2：透传
        /// </summary>
        public int Priority { get; set; }
        /// <summary>
        /// 呼叫发起时间
        /// </summary>
        public DateTime RequestTime { get; set; }
        /// <summary>
        /// 是否逻辑删除
        /// </summary>
        public bool IsDeleted { get; set; }

        public virtual UT_Users UT_Users { get; set; }
    }
}
