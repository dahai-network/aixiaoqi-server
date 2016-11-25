using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 众筹订单选号表
    /// </summary>
    public class UT_OrderByZCSelectionNumber : UT_Entity
    {
        /// <summary>
        /// 订单项ID
        /// </summary>
        public Guid OrderByZCId { get; set; }
        /// <summary>
        /// 订单编号/交易号
        /// </summary>
        public string OrderByZCSelectionNumberNum { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        public string IdentityNumber { get; set; }
        /// <summary>
        /// 选号ID
        /// </summary>
        public Guid ZCSelectionNumberId { get; set; }
        /// <summary>
        /// 付款状态，0：未付款 1：已付款
        /// </summary>
        public PayStatusType PayStatus { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public PaymentMethodType PaymentMethod { get; set; }

        public virtual UT_ZCSelectionNumber UT_ZCSelectionNumber { get; set; }

        public virtual UT_OrderByZC UT_OrderByZC { get; set; }
    }
}
