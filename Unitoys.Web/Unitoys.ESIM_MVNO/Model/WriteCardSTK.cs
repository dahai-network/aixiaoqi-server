using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.ESIM_MVNO.Model
{
    public class WriteCardSTK
    {
        public string iccid { get; set; }
        public string imsi { get; set; }
        public string ki { get; set; }
        /// <summary>
        /// 14
        /// 3-10
        /// 短信中心号码
        /// </summary>
        public string SMSC { get; set; }
        public string OP { get; set; }
        public string opc { get; set; }
        /// <summary>
        /// 用户签约号码，手机号
        /// 例：18012341234F
        /// </summary>
        public string ISDN { get; set; }
        /// <summary>
        /// 18
        /// SMS参数（包含短信中心号码等）
        /// </summary>
        public string SMSP { get; set; }
        /// <summary>
        /// 19
        /// MSISDN配置信息（包含用户手机号等）
        /// </summary>
        public string MSISDN { get; set; }
        /// <summary>
        /// 1A
        /// 优选网络列表
        /// 同：移动写卡PLMP
        /// </summary>
        public string PLMN { get; set; }
        public string EXP_DATE { get; set; }
    }
}
