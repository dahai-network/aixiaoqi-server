using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.ESIM_MVNO.Model
{
    public class APDUModel
    {
        /// <summary>
        /// 80C20000
        /// </summary>
        public string P1 { get; set; }
        /// <summary>
        /// 后续数据长度
        /// </summary>
        public string P2 { get; set; }
        /// <summary>
        /// D1
        /// </summary>
        public string P3 { get; set; }
        /// <summary>
        /// 后续长度
        /// '00' - '7F', 1字节
        /// '81 80' - '81 FF' ，2字节
        /// '82 01 00' - '82 FF FF' ，3字节
        /// </summary>
        public string P4 { get; set; }
        /// <summary>
        ///82028381860891683108100005F08B 
        /// </summary>
        public string P5 { get; set; }
        /// <summary>
        /// 后续长度
        /// '00' - '7F', 1字节
        /// '81 80' - '81 FF' ，2字节
        /// '82 01 00' - '82 FF FF' ，3字节
        /// </summary>
        public string P6 { get; set; }
        /// <summary>
        /// 40或者44
        /// 如果为一条短信则为40，如果为两条短信则为44
        /// </summary>
        public string P7 { get; set; }
        /// <summary>
        /// 0891556677887FF661216271338508
        /// </summary>
        public string P8 { get; set; }
        /// <summary>
        /// 后续数据长度
        /// </summary>
        public string UDL { get; set; }
        /// <summary>
        /// 07
        /// 信息标识长度
        /// </summary>
        public string UDHL { get; set; }
        /// <summary>
        /// 00
        /// 级联标识
        /// </summary>
        public string IEIa { get; set; }
        /// <summary>
        /// 03
        /// 级联信息长度
        /// </summary>
        public string IEIDLa { get; set; }
        /// <summary>
        /// 0xXX XX 01
        /// 批次、短信总数、短信索引
        /// 
        /// 参考：010101
        /// </summary>
        public string IEDa { get; set; }
        /// <summary>
        /// 70
        /// 安全头标识
        /// </summary>
        public string IEIb { get; set; }
        /// <summary>
        /// 00
        /// 信息长度
        /// </summary>
        public string IEIDLb { get; set; }
        /// <summary>
        /// 后续数据长度,从CHL到最后
        /// （注：CPL计算时包含加密的填充数据长度）
        /// </summary>
        public string CPL { get; set; }
        /// <summary>
        /// 11
        /// 安全报文头长度，从SPI到CC
        /// </summary>
        public string CHL { get; set; }
        /// <summary>
        /// 0600
        /// 只使用第一字节bit1,bit2,bit3。
        /// 
        /// 注：写卡与计算mac时需要0600都作为数据
        /// </summary>
        public string SPI { get; set; }
        /// <summary>
        /// 05
        /// 3DES CBC
        /// </summary>
        public string KIc { get; set; }
        /// <summary>
        /// 05
        /// 3DES CBC
        /// </summary>
        public string KID { get; set; }
        /// <summary>
        /// 0xB000F1
        /// 写卡
        /// </summary>
        public string TAR { get; set; }
        /// <summary>
        /// 0000000000
        /// 计数器固定值,卡片不对计数器进行合法性判断
        /// </summary>
        public string CNTR { get; set; }
        /// <summary>
        /// 0xXX
        /// 参见GSM03.48
        /// 
        /// 注：目前认为是报文加密时的补位字节
        /// </summary>
        public string PCNTR { get; set; }
        /// <summary>
        /// 4位MAC
        /// （注：CC计算时不包含加密的填充字节）
        /// 
        /// 注：上面这个注我也看不懂，反正算就是了
        /// </summary>
        public string CC { get; set; }
        /// <summary>
        /// 参见“命令数据格式”定义和注1
        /// 
        /// trust me，找不到注1的
        /// 命令数据0A
        /// </summary>
        public string SecuredData { get; set; }
        public WriteCard WriteCardTlv { get; set; }
    }
}
