using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Model;

namespace Unitoys.WebApi.Models
{
    public class AddPaymentBindingModel
    {
        public decimal Amount { get; set; }
        /// <summary>
        /// 支付方式
        /// 1支付宝/2微信
        /// </summary>
        public PaymentMethodType PaymentMethod { get; set; }
    }
}
