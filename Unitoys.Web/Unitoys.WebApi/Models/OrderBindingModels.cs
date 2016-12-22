﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }

    public class ActivationBindingModel
    {
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
}
