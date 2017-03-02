using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Model;

namespace Unitoys.WebApi.Models
{
    public class AddBindingModel
    {
        public Guid PackageID { get; set; }
        public int Quantity { get; set; }
        /// <summary>
        /// 支付的用户金额
        /// </summary>
        //public Decimal PayUserAmount { get; set; }
        /// <summary>
        /// 是否用户金额支付
        /// </summary>
        //public int IsPayUserAmount { get; set; }
        public Unitoys.Model.PaymentMethodType PaymentMethod { get; set; }
    }

    public class GetUserOrderListBindingModel
    {
        public Guid UserID { get; set; }
        public string PackageName { get; set; }
        public DateTime OrderStartDate { get; set; }
        public DateTime OrderEndDate { get; set; }
        public DateTime PayStartDate { get; set; }
        public DateTime PayEndDate { get; set; }
        public int? PayStatus { get; set; }
        public int? OrderStatus { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        /// <summary>
        /// 套餐类型
        /// </summary>
        public CategoryType? PackageCategory { get; set; }

        /// <summary>
        /// 是否流量套餐
        /// </summary>
        public bool? PackageIsCategoryFlow { get; set; }
        /// <summary>
        /// 是否通话套餐
        /// </summary>
        public bool? PackageIsCategoryCall { get; set; }
        /// <summary>
        /// 是否双卡双待套餐
        /// </summary>
        public bool? PackageIsCategoryDualSimStandby { get; set; }
        /// <summary>
        /// 是否大王卡套餐
        /// </summary>
        public bool? PackageIsCategoryKingCard { get; set; }
    }

    public class ActivationBindingModel
    {
        /// <summary>
        /// 空卡序列号
        /// </summary>
        public string EmptyCardSerialNumber { get; set; }
        public Guid OrderID { get; set; }
        public int BeginTime { get; set; }
    }

    public class ActivationKindCardBindingModel
    {
        public Guid OrderID { get; set; }
        public string Tel { get; set; }
    }

    public class PayOrderByUserAmountBindingModel
    {
        public Guid OrderID { get; set; }
    }

    public class PayNotifyAsyncBindingModel
    {
        public string OrderNum { get; set; }
        public string PayDate { get; set; }
        public decimal Amount { get; set; }
        public string Key { get; set; }
    }
    public class IsStatusUsedByCategoryBindingModel
    {
        public CategoryType? PackageCategory { get; set; }

        /// <summary>
        /// 是否流量套餐
        /// </summary>
        public bool? PackageIsCategoryFlow { get; set; }
        /// <summary>
        /// 是否通话套餐
        /// </summary>
        public bool? PackageIsCategoryCall { get; set; }
        /// <summary>
        /// 是否双卡双待套餐
        /// </summary>
        public bool? PackageIsCategoryDualSimStandby { get; set; }
        /// <summary>
        /// 是否大王卡套餐
        /// </summary>
        public bool? PackageIsCategoryKingCard { get; set; }
    }

}
