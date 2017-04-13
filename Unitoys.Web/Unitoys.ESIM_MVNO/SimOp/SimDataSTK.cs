using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.ESIM_MVNO.Model;

namespace Unitoys.ESIM_MVNO
{
    /// <summary>
    /// Sim卡数据
    /// </summary>
    public class SimDataSTK
    {
        private string 敏感数据加密密钥 = "962CDEF35F3C5024";//"B01CE14D86EFB356";
        private string 传输密钥 = "82E64288CDBB0FA4";//"D7A49F2689AF0AF6";
        private string 会话密钥随机八位 = "C501CBE8A849B3E7";
        private string TAR = "BFBFBF";

        private string simData { get; set; }
        public SimDataSTK(string EmptyCardSerialNumber, WriteCardSTK writeCardSTK)
        {
            //1.敏感数据加密
            writeCardSTK.ki = DesHelper.DesEncodeECB(writeCardSTK.ki, 敏感数据加密密钥);
            writeCardSTK.opc = DesHelper.DesEncodeECB(writeCardSTK.opc, 敏感数据加密密钥);
            //writeCardSTK.EXP_DATE = DesHelper.DesEncodeECB(writeCardSTK.EXP_DATE, 敏感数据加密密钥);

            //获取组装的TLV格式16进制
            string writeCardSTKTlvHex = GetWriteCardSTKTlv(writeCardSTK);

            //2.加上会话密钥
            //3.会话密钥加密明文（敏感数据项已加密）
            //4.用传输密钥加密会话密钥
            string data = DesHelper.DesEncodeECB(会话密钥随机八位, 传输密钥) + DesHelper.DesEncodeECB(writeCardSTKTlvHex, 会话密钥随机八位);

            //Base64算法
            //string data = DesHelper.DesEncodeECB(会话密钥随机八位, 传输密钥) + DesHelper.DesEncodeECB(DesHelper.DesEncodeECB(writeCardSTKTlvHex, 敏感数据加密密钥), 会话密钥随机八位);

            //5.加上传输命令头（TAR）
            data = TAR + data;

            this.simData = data;
        }

        public string GetData()
        {
            //return simData;
            return StringToHexString(Convert.ToBase64String(DataHelper.HexToByte(simData)), Encoding.UTF8);
        }
        private string StringToHexString(string s, Encoding encode)
        {
            byte[] b = encode.GetBytes(s);//按照指定编码将string编程字节数组
            string result = string.Empty;
            for (int i = 0; i < b.Length; i++)//逐字节变为16进制字符
            {
                result += Convert.ToString(b[i], 16);
            }
            return result;
        }

        /// <summary>
        /// 获取写卡数据格式的TLV数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private string GetWriteCardSTKTlv(WriteCardSTK model)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("010A" + ReverseStrTwo(model.iccid));
            sb.Append("0209" + "08" + ReverseStrTwo("9" + model.imsi));
            sb.Append("0310" + model.ki);

            //sb.Append("1500");
            sb.Append("0410" + model.opc);
            if (!string.IsNullOrEmpty(model.SMSP))
            {
                sb.Append("05" + DataHelper.DecToHex(model.SMSP.Length / 2) + model.SMSP);//+ model.SMSP
            }
            if (!string.IsNullOrEmpty(model.ISDN))
            {
                sb.Append("06" + DataHelper.DecToHex(model.ISDN.Length / 2) + model.ISDN);//+ model.SMSP
            }
            if (!string.IsNullOrEmpty(model.PLMN))
            {
                sb.Append("07" + DataHelper.DecToHex(model.PLMN.Length / 2) + ReverseStrTwo(model.PLMN));
            }
            else
            {
                //sb.Append("1A00");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 倒转每两位字符串顺序
        /// </summary>
        /// <param name="str"></param>
        private string ReverseStrTwo(string str)
        {
            string reverseStr = "";
            for (int i = 0; i < str.Length / 2; i++)
            {

                reverseStr += new string(str.ToCharArray(i * 2, 2).Reverse().ToArray());

            }
            return reverseStr;
        }

        /// <summary>
        /// 获取填充补位的16进制
        /// </summary>
        /// <param name="length">总字节</param>
        /// <returns></returns>
        private string GetPaddingHex(int length)
        {
            int remaining = length % 8;

            //如果加密数据是8的整数倍，则在数据块后增加一个8字节数据块’0x80 00 00 00 00 00 00 00’
            //若原始数据不是8的整数倍，则在该数据块后填补一个值为16进制’0x80’的字节，其余字节用16进制’0x00’的字节补齐为8字节
            if (remaining == 0)
            {
                return "8000000000000000";
            }
            else
            {
                string paddingStr = "80";

                for (int i = 0; i < 8 - (remaining) - 1; i++)
                {
                    paddingStr += "00";
                }

                return paddingStr;
            }
        }
    }
}
