using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.WebApi.Models
{
    public class SendSMSBindingModel
    {
        public string ToNum { get; set; }
        public int Type { get; set; }
    }
}
