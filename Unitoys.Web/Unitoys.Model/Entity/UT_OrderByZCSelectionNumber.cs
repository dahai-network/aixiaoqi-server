using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        /// 订单日期
        /// </summary>
        public int OrderDate { get; set; }
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
        public Guid? ZCSelectionNumberId { get; set; }
        /// <summary>
        /// 付款状态，0：未付款 1：已付款
        /// </summary>
        public PayStatusType PayStatus { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public PaymentMethodType? PaymentMethod { get; set; }
        /// <summary>
        /// 订单状态，0 : 未激活，1：已激活，2：已过期，3：已取消，4：激活失败
        /// </summary>
        public OrderByZCSelectionNumberStatusType OrderStatus { get; set; }
        /// <summary>
        /// 选择的号码
        /// </summary>
        public string SelectionNumber { get; set; }

        public virtual UT_ZCSelectionNumber UT_ZCSelectionNumber { get; set; }

        public virtual UT_OrderByZC UT_OrderByZC { get; set; }
    }
    public enum OrderByZCSelectionNumberStatusType
    {
        /// <summary>
        /// 未激活
        /// </summary>
        [Description("未激活")]
        Unactivated = 0,
        /// <summary>
        /// 已激活
        /// </summary>
        [Description("已激活")]
        Used = 1,
        /// <summary>
        /// 已过期
        /// </summary>
        [Description("已过期")]
        HasExpired = 2,
        /// <summary>
        /// 已取消
        /// </summary>
        [Description("已取消")]
        Cancel = 3,
        /// <summary>
        /// 激活失败
        /// </summary>
        [Description("激活失败")]
        UnactivatError = 4,
        /// <summary>
        /// 待提交
        /// </summary>
        [Description("待提交")]
        SubmitAgo = 5,
        /// <summary>
        /// 提交成功
        /// </summary>
        [Description("提交成功")]
        SubmitSuccess = 6,
    }

}
