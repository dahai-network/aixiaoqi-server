using cn.jpush.api.device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Core.JiGuang
{
    public class DeviceApi
    {
        private static String app_key = "203b0b8a6747e85d18779ce0";
        private static String master_secret = "ea2a851deaba1b8eb1272f35";
        public void GetClient()
        {
            DeviceClient client = new DeviceClient(app_key, master_secret);
        }
    }
}
