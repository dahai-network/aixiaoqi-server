using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Model;

namespace Unitoys.WebApi.Models
{
    public class DeviceBraceletBindingModels
    {
        public string IMEI { get; set; }
    }
    public class DeviceBraceletConnectInfoModels
    {
        public string Power { get; set; }
        public DeviceType? DeviceType { get; set; }
        public string Version { get; set; }
    }
}
