using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.WebApi.Models
{
    public class AddSpeakRecordBindingModel
    {
        public string DeviceName { get; set; }
        public string CalledTelNum { get; set; }
        public DateTime CallStartTime { get; set; }
        public DateTime CallStopTime { get; set; }
        public int CallSessionTime { get; set; }
        public string CallSourceIp { get; set; }
        public string CallServerIp { get; set; }
        public string Acctterminatedirection { get; set; }
    }
}
