using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.WebApi.Models
{
    public class AddOrderByZCSelectionNumberBindingModel
    {
        public Guid OrderByZCId { get; set; }
        public string Name { get; set; }
        public string IdentityNumber { get; set; }
        public string MobileNumber { get; set; }
        public Unitoys.Model.PaymentMethodType? PaymentMethod { get; set; }
    }
}
