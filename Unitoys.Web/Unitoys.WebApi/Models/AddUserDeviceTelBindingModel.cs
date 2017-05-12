using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Model;

namespace Unitoys.WebApi.Models
{
    public class AddUserDeviceTelBindingModel
    {
        public string Tel { get; set; }
        public string ICCID { get; set; }
    }
}
