using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.WebApi.Models
{
    public class AddPhoneCallbackBindingModel
    {
        public string To { get; set; }
        public string From { get; set; }
        public int MaximumPhoneCallTime { get; set; }
        public int Priority { get; set; }
    }
}
