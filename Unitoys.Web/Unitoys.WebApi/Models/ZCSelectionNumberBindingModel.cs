using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.WebApi.Models
{
    public class ZCSelectionNumberGetBindingModel
    {
        public string Province { get; set; }
        public string City { get; set; }
        public string MobileNumber { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
}
