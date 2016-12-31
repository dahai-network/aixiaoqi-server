using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.ESIM_MVNO.Model
{
    public class WriteCard
    {
        /// <summary>
        /// 01
        /// 要下载的海外ICCID
        /// 格式为卡片文件存储格式
        /// 举例：010A98680021436587092143
        /// 
        /// 注：卡片文件存储格式就是每两个字符反转98680021436587092143
        /// 10个字节
        /// </summary>
        public string iccid { get; set; }
        /// <summary>
        /// 02
        /// 要下载的海外IMSI
        /// 格式为卡片文件存储格式
        /// 举例：0209084906102143658709
        /// 
        /// 注：值089固定
        /// 如：08+反转（9+454070012300051）
        /// </summary>
        public string imsi { get; set; }
        /// <summary>
        /// 03
        /// KI或K
        /// </summary>
        public string ki { get; set; }
        /// <summary>
        /// 04
        /// OPC
        /// </summary>
        public string opc { get; set; }
        /// <summary>
        /// 05
        /// USIM应用下漫游列表（Preferred network PLMN）
        /// 如漫游列表为 46000，45419，45416，45403，45404，
        /// 数据组织按卡内文件存储格式编写052564F000C0C054F491C0C054F461C0C054F430C0C054F440C0C0
        /// </summary>
        public string HPLMN { get; set; }
        /// <summary>
        /// 06
        /// 禁止漫游列表（Forbidden network PLMN）
        /// 如禁止漫游列表为 46000,45412,45413，
        /// 数据组织按卡内文件存储格式编写060964F00054F42154F431
        /// </summary>
        public string FPLMN { get; set; }
        /// <summary>
        /// 07
        /// GSM应用下的漫游列表
        /// 如漫游列表为46000，45412，45413 数据组织按卡内文件存储格式编写
        /// 070964F00054F42154F431
        /// </summary>
        public string PLMN { get; set; }
    }
}
