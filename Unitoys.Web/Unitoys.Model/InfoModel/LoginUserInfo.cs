using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Unitoys.Model
{
    [Serializable]
    public class LoginUserInfo
    {
        public Guid ID { get; set; }
        public string LoginName { get; set; }
        public string Tel { get; set; }

        public string TrueName { get; set; }

        public string LoginIp { get; set; }

        public string UserHead { get; set; }

        public double? Weight { get; set; }

    }
}