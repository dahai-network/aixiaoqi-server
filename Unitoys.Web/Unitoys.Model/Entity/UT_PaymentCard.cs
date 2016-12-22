using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 用户充值卡
    /// </summary>
    public class UT_PaymentCard : UT_Entity
    {

        /// <summary>
        /// 卡号
        /// </summary>
        public string CardNum { get; set; }
        /// <summary>
        /// 卡密
        /// </summary>
        public string CardPwd { get; set; }
        /// <summary>
        /// 创建管理
        /// </summary>
        public Guid ManageUserId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 充值用户
        /// </summary>
        public Guid? UserId { get; set; }
        /// <summary>
        /// 充值时间
        /// </summary>
        public int? PaymentDate { get; set; }
        /// <summary>
        /// 最晚有效时间
        /// </summary>
        public int LastEffectiveDate { get; set; }
        /// <summary>
        /// 充值金额
        /// </summary>
        public int Price { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public PaymentCardStatusType Status { get; set; }
        public virtual UT_Users UT_Users { get; set; }
        public virtual UT_ManageUsers UT_ManageUsers { get; set; }
        [Timestamp]
        public Byte[] RowVersion { get; set; }
    }
    public enum PaymentCardStatusType
    {
        /// <summary>
        /// 未使用
        /// </summary>
        Enable = 0,
        /// <summary>
        /// 已使用
        /// </summary>
        Disabled = 1,
        /// <summary>
        /// 失效
        /// </summary>
        Invalid = 2
    }
}
