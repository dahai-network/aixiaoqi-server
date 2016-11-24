using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 用户设置呼叫转移到的号码类，俗称大号类
    /// </summary>
    public class UT_CallTransferNum : UT_Entity
    {
        /// <summary>
        /// 大号电话号码
        /// </summary>
        public string TelNum { get; set; }
        /// <summary>
        /// 大号的VOS登陆密码
        /// </summary>
        public string TelPwd { get; set; }
        /// <summary>
        /// 使用状态 0：未使用   1：已使用
        /// </summary>
        public StatusType Status { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        
        public Guid? UserId { get; set; }

        public virtual UT_Users UT_Users { get; set; }
    }

    public enum StatusType
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
