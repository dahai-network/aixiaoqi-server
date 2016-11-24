using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.WebApi.Models
{
    public class SMSNewReportBindingModel
    {
        public string Id { get; set; }
        public string DevName { get; set; }
        public string Port { get; set; }
        public string IccId { get; set; }
        public string SMSTime { get; set; }
        public string Fm { get; set; }
        public string To { get; set; }
        public string SMSContent { get; set; }
    }
}
