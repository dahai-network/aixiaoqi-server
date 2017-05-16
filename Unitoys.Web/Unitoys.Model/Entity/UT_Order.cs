using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 订单表
    /// </summary>
    public class UT_Order : UT_Entity
    {
        public UT_Order()
        {
            this.UT_OrderUsage = new HashSet<UT_OrderUsage>();
        }
        /// <summary>
        /// 订单编号/交易号
        /// </summary>
        public string OrderNum { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 套餐ID
        /// </summary>
        public Guid PackageId { get; set; }
        /// <summary>
        /// 套餐名称
        /// </summary>
        public string PackageName { get; set; }
        /// <summary>
        /// 套餐特色
        /// </summary>
        public string PackageFeatures { get; set; }
        /// <summary>
        /// 套餐详情
        /// </summary>
        public string PackageDetails { get; set; }
        /// <summary>
        /// 是否支持4G
        /// </summary>
        public bool PackageIsSupport4G { get; set; }
        /// <summary>
        /// 是否需要Apn
        /// </summary>
        public bool PackageIsApn { get; set; }
        /// <summary>
        /// 套餐Apn名称
        /// </summary>
        public string PackageApnName { get; set; }
        /// <summary>
        /// 套餐流量
        /// </summary>
        public int Flow { get; set; }
        /// <summary>
        /// 套餐分类
        /// </summary>
        public CategoryType PackageCategory { get; set; }
        /// <summary>
        /// 是否流量套餐
        /// </summary>
        public bool PackageIsCategoryFlow { get; set; }
        /// <summary>
        /// 是否通话套餐
        /// </summary>
        public bool PackageIsCategoryCall { get; set; }
        /// <summary>
        /// 是否双卡双待套餐
        /// </summary>
        public bool PackageIsCategoryDualSimStandby { get; set; }
        /// <summary>
        /// 是否大王卡套餐
        /// </summary>
        public bool PackageIsCategoryKingCard { get; set; }
        /// <summary>
        /// 订单项数量
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// 订单项单价
        /// </summary>
        public Decimal UnitPrice { get; set; }
        /// <summary>
        /// 订单项总价
        /// </summary>
        public Decimal TotalPrice { get; set; }
        /// <summary>
        /// 订单日期
        /// </summary>
        public int OrderDate { get; set; }
        /// <summary>
        /// 支付的用户金额
        /// </summary>
        //public Decimal PayUserAmount { get; set; }
        /// <summary>
        /// 是否用户金额支付,0不是,1是
        /// </summary>
        //public int IsPayUserAmount { get; set; }
        /// <summary>
        /// 订单付款时间
        /// </summary>
        public int? PayDate { get; set; }
        /// <summary>
        /// 付款状态，0：未付款 1：已付款
        /// </summary>
        public PayStatusType PayStatus { get; set; }
        /// <summary>
        /// 订单状态，0 : 未激活，1：已激活，2：已过期，3：已取消，4：激活失败
        /// </summary>
        public OrderStatusType OrderStatus { get; set; }
        /// <summary>
        /// 有效天数
        /// </summary>
        public int ExpireDays { get; set; }
        /// <summary>
        /// 订单号
        /// 购买ESim产品的订单号
        /// </summary>
        public string PackageOrderId { get; set; }
        /// <summary>
        /// 密钥数据
        /// 购买ESim产品的密钥数据
        /// </summary>
        //public string PackageOrderData { get; set; }
        /// <summary>
        /// 剩余通话分钟数
        /// </summary>
        public int RemainingCallMinutes { get; set; }
        /// <summary>
        /// 生效日期
        /// </summary>
        public int? EffectiveDate { get; set; }
        /// <summary>
        /// 生效日期描述
        /// </summary>
        public DateTime? EffectiveDateDesc { get; set; }
        /// <summary>
        /// 激活时间
        /// </summary>
        public int? ActivationDate { get; set; }
        public Guid? OrderDeviceTelId { get; set; }
        /// <summary>
        /// 组合ID
        /// </summary>
        public Guid? PackageAttributeId { get; set; }
        //public Guid? UserReceiveId { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public PaymentMethodType PaymentMethod { get; set; }
        public virtual UT_Package UT_Package { get; set; }
        public virtual UT_Users UT_Users { get; set; }
        public virtual ICollection<UT_OrderUsage> UT_OrderUsage { get; set; }
        //public virtual UT_UserReceive UT_UserReceive { get; set; }
        public virtual UT_OrderDeviceTel UT_OrderDeviceTel { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 原价
        /// </summary>
        public string OriginalPrice { get; set; }
        /// <summary>
        /// 乐观并发
        /// </summary>
        [Timestamp]
        public Byte[] RowVersion { get; set; }
    }
    public enum PayStatusType
    {
        [Description("未付款")]
        NoPayment = 0,
        [Description("已付款")]
        YesPayment = 1,
        [Description("异常付款")]
        AbnormalPayment = -1
    }

    public enum OrderStatusType
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
    }
}
