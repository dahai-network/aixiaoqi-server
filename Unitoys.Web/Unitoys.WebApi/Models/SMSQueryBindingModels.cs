using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.WebApi.Models
{
    public class SMSQueryBindingModels
    {
        public int sms_num { get; set; }
        public int? start { get; set; }
        public string DevName { get; set; }
        public string Port { get; set; }
        public string IccId { get; set; }
    }
}
