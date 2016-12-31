using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.ESIM_MVNO
{
    public class ResponseModel<T>
    {
        public string status { get; set; }
        public string msg { get; set; }
        public T data { get; set; }
    }
    public class BuyProduct
    {
        public string orderId { get; set; }
        public string beginTime { get; set; }
        public string endTime { get; set; }
        public EsimResource esimResource { get; set; }
    }

    public class EsimResource
    {
        /// <summary>
        /// 
        /// </summary>
        public string imsi { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ki { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string opc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string iccid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string plmn { get; set; }
    }
}
