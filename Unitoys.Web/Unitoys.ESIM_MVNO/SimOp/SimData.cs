using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.ESIM_MVNO.Model;

namespace Unitoys.ESIM_MVNO
{
    //参考
    /*
     参考
     * 写卡数据：80C2000086D1818382028381860891683108100005F08B73440891556677887FF66121627133850862070003010101700000581106000505B000F1200541155CB5CA0755CE894B936D7AB7898537E80C19856C2FF4AC886EEC72ECDBAC11D2716300DA84F1DD1267463F7575AF05692A19B161793397BA135B52A0E88D325C0BDD3777170BDDDA640F210E
     * 
     * 解析   
     * 80C20000
     * 86
     * D1
     * 8183
     * 82028381860891683108100005F08B
     * 73
     * 44
     * 0891556677887FF661216271338508
     * 62
     * 07
     * 00
     * 03
     * 010101
     * 70
     * 00
     * 0058
     * 11
     * 0600
     * 05
     * 05
     * B000F1
     * 200541155CB5CA0755CE894B936D7AB7898537E80C19856C2FF4AC886EEC72ECDBAC11D2716300DA84F1DD1267463F7575AF05692A19B161793397BA135B52A0E88D325C0BDD3777170BDDDA640F210E
     * 
     * 
           
     * 
     * k1获取：
     * 根密钥与需商代码分散，
     * 
     * 
     * 加密报文加密部分：
     * 3DES-CBC（
        CNTR+
        PCNTR+
        CC+
        (0A00+写卡数据格式TLV字节数)+
        写卡数据格式TLV）
     * 
     * 
     * MAC计算（加密3DES-CBC）：
     * 加密所用原始数据：CPL、 CHL 、SPI 、Kic、 KID、 TAR、 CNTR、 PCNTR、命令数据格式SecuredData、明文Data（写卡数据TLV格式）
     * 1.获取原始数据的8个字节一组进行分组
     * 2.加密(组1)异或组2,再加密异或后的值,再异或组3,再加密异或后的值,以此类推
     * 3.最终组异或完时应再进行加密
     * 4.取最终结果的4个字节
     * 
     * 
     * 加密数据统一填充算法：
     * 数据填充算法采用如下算法：
     * 如果加密数据总字节是8的整数倍，则在数据块后增加一个8字节数据块’0x80 00 00 00 00 00 00 00’；
     * 若原始数据不是8的整数倍，则在该数据块后填补一个值为16进制’0x80’的字节，其余字节用16进制’0x00’的字节补齐为8字节。
     */
    /*
    * 使用示例
    SimData simdata = new SimData("16A0000100000001",
               new WriteCard()
               {
                   iccid = "89852071600108266015",
                   imsi = "454070012300051",
                   ki = "224513EF7BBDB7200692B01A98E2C168",
                   opc = "542B02BFDE355583350CBF13DE7B7D1A"
               });

           var writeData = simdata.GetData();
    */

    /// <summary>
    /// Sim卡数据
    /// </summary>
    public class SimData
    {
        private string rootKey = "21079FA267206FC64C42617C9515003B";//"11223344556677889900112233445566";
        /// <summary>
        /// 补位后的虚商代码
        /// </summary>
        public string VirtualBusiness { get; set; }
        /// <summary>
        /// 空卡序列号
        /// </summary>
        public string EmptyCardSerialNumber { get; set; }

        /// <summary>
        /// k1,计算所需密钥
        /// </summary>
        private string k1;
        /// <summary>
        /// 报文加密
        /// </summary>
        private string des3_cbc_encryp;
        private string data;
        public SimData(string EmptyCardSerialNumber, WriteCard writeCard)
        {
            this.EmptyCardSerialNumber = EmptyCardSerialNumber;
            VirtualBusiness = "0001002020202020";
            APDUModel model = new APDUModel()
            {
                P1 = "80C20000",
                P2 = "",//后续长度
                P3 = "D1",
                P4 = "",//后续长度，参考字段注释
                P5 = "82028381860891683108100005F08B",
                P6 = "",//后续长度，参考字段注释
                P7 = "44",//TODO 暂时写死，不清楚此短信字段应起的作用，应该是分包时使用
                P8 = "0891556677887FF661216271338508",
                UDL = "",//后续数据长度
                UDHL = "07",
                IEIa = "00",
                IEIDLa = "03",
                IEDa = "010101",//TODO 同上相关；暂时写死，不清楚此字段应起的作用，应该是分包时使用
                IEIb = "70",
                IEIDLb = "00",
                CPL = "",//后续长度，2个字节,参考：0058
                CHL = "11",
                SPI = "0600",
                KIc = "05",
                KID = "05",
                TAR = "B000F1",//TAR后报文加密，数据为TAR后所有数据明文格式
                CNTR = "0000000000",
                PCNTR = "",//TODO 暂时认为是报文加密时需补位长度
                CC = "",//mac待计算
                SecuredData = "",//0A00+写卡数据格式TLV字节数
                WriteCardTlv = writeCard,
            };

            //获取组装的TLV格式16进制
            string writeCardTlvHex = GetWriteCardTlv(writeCard);

            k1 = GetK1(VirtualBusiness, EmptyCardSerialNumber);

            //赋值MAC计算时所需的数据
            model.SecuredData = "0A00" + DataHelper.DecToHex(writeCardTlvHex.Length / 2);
            int encrypSourceDataLength = 5 + 1 + 4 + (model.SecuredData.Length / 2) + writeCardTlvHex.Length / 2;
            int CPL = (1 + 2 + 1 + 1 + 3 + encrypSourceDataLength);
            CPL += GetPaddingHex(encrypSourceDataLength).Length / 2;
            model.CPL = DataHelper.DecToHex(CPL).PadLeft(4, '0');
            model.PCNTR = DataHelper.DecToHex(GetPaddingHex(encrypSourceDataLength).Length / 2);

            //MAC
            //加密所用原始数据：CPL、 CHL 、SPI 、Kic、 KID、 TAR、 CNTR、 PCNTR、命令数据格式SecuredData、明文Data（写卡数据TLV格式）
            var CCComputeData = model.CPL + model.CHL + model.SPI + model.KIc + model.KID + model.TAR + model.CNTR + model.PCNTR + model.SecuredData + writeCardTlvHex;
            model.CC = GetMac(k1, CCComputeData + GetPaddingHex(CCComputeData.Length / 2)).Substring(0, 8);

            //报文加密部分的数据
            model.UDL = DataHelper.DecToHex(CPL + 2 + 8) + "";

            int P6 = CPL + 2 + 8 + model.UDL.Length / 2 + 16;

            if (P6 < 128)
            {
                model.P6 = "";
            }
            else if (P6 >= 128 && P6 <= 255)
            {
                model.P6 = "81";
            }
            else if (P6 >= 256 && P6 <= 65535)
            {
                model.P6 = "82";
            }
            model.P6 += DataHelper.DecToHex(P6);


            int P4 = P6 + (model.P6.Length / 2) + (model.P5.Length / 2);

            if (P4 < 128)
            {
                model.P4 = "";
            }
            else if (P4 >= 128 && P4 <= 255)
            {
                model.P4 = "81";
            }
            else if (P4 >= 256 && P4 <= 65535)
            {
                model.P4 = "82";
            }
            model.P4 += DataHelper.DecToHex(P4);


            int P2 = P4 + (model.P4.Length / 2) + (model.P3.Length / 2);
            model.P2 = DataHelper.DecToHex(P2);

            StringBuilder sb = new StringBuilder();
            sb.Append(model.CNTR);
            sb.Append(model.PCNTR);
            sb.Append(model.CC);
            sb.Append(model.SecuredData);
            sb.Append(writeCardTlvHex);
            sb.Append(GetPaddingHex(sb.Length / 2));
            byte[] key = DataHelper.HexToByte(k1);
            byte[] iv = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };      //当模式为ECB时，IV无用
            byte[] data = DataHelper.HexToByte(sb.ToString());
            var encrypData = Des3.Des3EncodeCBC(key, iv, data);
            string encrypDataStr = DataHelper.ByteToHex(encrypData, encrypData.Length);

            //写卡数据
            sb = new StringBuilder();
            sb.Append(model.P1);
            sb.Append(model.P2);
            sb.Append(model.P3);
            sb.Append(model.P4);
            sb.Append(model.P5);
            sb.Append(model.P6);
            sb.Append(model.P7);
            sb.Append(model.P8);
            sb.Append(model.UDL);
            sb.Append(model.UDHL);
            sb.Append(model.IEIa);
            sb.Append(model.IEIDLa);
            sb.Append(model.IEDa);
            sb.Append(model.IEIb);
            sb.Append(model.IEIDLb);
            sb.Append(model.CPL);
            sb.Append(model.CHL);
            sb.Append(model.SPI);
            sb.Append(model.KIc);
            sb.Append(model.KID);
            sb.Append(model.TAR);
            sb.Append(encrypDataStr);
            this.data = sb.ToString();
        }

        public string GetData()
        {
            return data;
        }


        /// <summary>
        /// 获取K1,两次分散
        /// </summary>
        /// <param name="VirtualBusiness">虚商代码</param>
        /// <param name="EmptyCardSerialNumber">空卡序列号</param>
        /// <returns></returns>
        private string GetK1(string VirtualBusiness, string EmptyCardSerialNumber)
        {
            //获取k1
            //分散虚商代码
            var k1 = Diversify.getTransKey(rootKey, VirtualBusiness);
            //分散空卡序列号
            k1 = Diversify.getTransKey(k1, EmptyCardSerialNumber);
            return k1;
        }

        /// <summary>
        /// 获取写卡数据格式的TLV数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private string GetWriteCardTlv(WriteCard model)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("010A" + ReverseStrTwo(model.iccid));
            sb.Append("0209" + "08" + ReverseStrTwo("9" + model.imsi));
            sb.Append("0310" + model.ki);
            sb.Append("0410" + model.opc);
            if (!string.IsNullOrEmpty(model.HPLMN))
            {
                sb.Append("05" + DataHelper.DecToHex(model.HPLMN.Length / 2) + ReverseStrTwo(model.HPLMN));
            }
            if (!string.IsNullOrEmpty(model.FPLMN))
            {
                sb.Append("06" + DataHelper.DecToHex(model.FPLMN.Length / 2) + ReverseStrTwo(model.FPLMN));
            }
            if (!string.IsNullOrEmpty(model.PLMN))
            {
                sb.Append("07" + DataHelper.DecToHex(model.PLMN.Length / 2) + ReverseStrTwo(model.PLMN));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 倒转每两位字符串顺序
        /// </summary>
        /// <param name="str"></param>
        public static string ReverseStrTwo(string str)
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

        /// <summary>
        /// 计算MAC的值
        /// </summary>
        /// <param name="dataHex"></param>
        /// <returns></returns>
        private string GetMac(string k1, string dataHex)
        {
            var data = DataHelper.HexToByte(dataHex).ToList();
            var iData = BitXor(data.GetRange(0, 8).ToArray(), new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 8);
            byte[] oData = null;
            for (int i = 1; i < data.Count / 8; i++)
            {
                oData = Des3.Des3EncodeCBC(DataHelper.HexToByte(k1), new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, iData);
                iData = BitXor(data.GetRange(8 * i, 8).ToArray(), oData, 8);
            }
            var mac = Des3.Des3EncodeCBC(DataHelper.HexToByte(k1), new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, iData);
            return DataHelper.ByteToHex(mac, mac.Length);
        }

        /// <summary>
        /// 二进制数组异或
        /// </summary>
        /// <param name="Data1"></param>
        /// <param name="Data2"></param>
        /// <param name="Len"></param>
        /// <returns></returns>
        private static byte[] BitXor(byte[] Data1, byte[] Data2, int Len)
        {
            int i;
            byte[] Dest = new byte[Len];

            for (i = 0; i < Len; i++)
                Dest[i] = (byte)(Data1[i] ^ Data2[i]);

            return Dest;
        }
    }
}
