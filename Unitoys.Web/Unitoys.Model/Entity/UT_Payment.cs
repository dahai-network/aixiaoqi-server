using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 付款信息
    /// </summary>
    public class UT_Payment : UT_Entity
    {
        /// <summary>
        /// 支付订单号
        /// </summary>
        public string PaymentNum { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public PaymentMethodType PaymentMethod { get; set; }
        /// <summary>
        /// 支付目的
        /// </summary>
        public string PaymentPurpose { get; set; }
        /// <summary>
        /// 支出或者收入，0：支出、1：收入
        /// </summary>
        public int PayOrReceive { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 折扣
        /// </summary>
        public decimal? Discount { get; set; }
        /// <summary>
        /// 创建支付时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 完成支付时间
        /// </summary>
        public DateTime? PaymentConfirmDate { get; set; }
        /// <summary>
        /// 状态：0:待付款、1:已付款、2:已过期、-1:异常付款
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public virtual UT_Users UT_Users { get; set; }

    }

    public enum PaymentMethodType
    {
        [Description("支付宝")]
        AliPay = 1,
        [Description("微信")]
        WxPay = 2,
        [Description("余额")]
        Balance = 3,
    }
}
