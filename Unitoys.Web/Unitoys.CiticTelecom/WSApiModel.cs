using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.CiticTelecom
{
    public class TranslatePackagePlanModel
    {
        /// <summary>
        /// 0 – success,
        /// others- refer to “Status Mapping
        /// Table”
        /// </summary>
        public int status { get; set; }

        public string planName { get; set; }

        public string balance { get; set; }

        public string startTime { get; set; }

        public string endTime { get; set; }
    }
    public class TranslateSubscriptionModel
    {
        public int status { get; set; }

        /// <summary>
        /// Only applicable if status = 0
        /// 1 – On
        /// 2 - Off
        /// </summary>
        public int resultCode { get; set; }
    }
    public class TranslateBalanceModel
    {
        /// <summary>
        ///0 – Success (refer to 2.6.4ErrorCodeTable)
        /// </summary>
        public int retCode { get; set; }
    }
    public class TranslateStatusModel
    {
        /// <summary>
        ///0 – success,
        ///others- refer to “Status Mapping
        ///Table”
        ///
        /// 0 – success,
        ///1 – success but the plan is not
        ///activated
        ///others- refer to “Status Mapping
        ///Table”
        /// </summary>
        public int status { get; set; }

        

    }
}
