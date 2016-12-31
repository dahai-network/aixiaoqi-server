using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.ESIM_MVNO
{
    public class DataHelper
    {
        public static string ByteToHex(byte comByte)
        {
            return comByte.ToString("X2") + " ";
        }
        public static string ByteToHex(byte[] comByte, int len)
        {
            string returnStr = "";
            if (comByte != null)
            {
                for (int i = 0; i < len; i++)
                {
                    returnStr += comByte[i].ToString("X2");
                }
            }
            return returnStr;
        }
        public static byte[] HexToByte(string msg)
        {
            msg = msg.Replace(" ", "");

            byte[] comBuffer = new byte[msg.Length / 2];
            for (int i = 0; i < msg.Length; i += 2)
            {
                comBuffer[i / 2] = (byte)Convert.ToByte(msg.Substring(i, 2), 16);
            }

            return comBuffer;
        }
        public static string HEXToASCII(string data)
        {
            data = data.Replace(" ", "");
            byte[] comBuffer = new byte[data.Length / 2];
            for (int i = 0; i < data.Length; i += 2)
            {
                comBuffer[i / 2] = (byte)Convert.ToByte(data.Substring(i, 2), 16);
            }
            string result = Encoding.Default.GetString(comBuffer);
            return result;
        }
        public static string ASCIIToHEX(string data)
        {
            StringBuilder result = new StringBuilder(data.Length * 2);
            for (int i = 0; i < data.Length; i++)
            {
                result.Append(((int)data[i]).ToString("X2") + " ");
            }
            return Convert.ToString(result);
        }
        /// <summary>
        /// 设备号进制转换：10进制转16进制
        /// </summary>
        /// <param name="dec"></param>
        /// <returns></returns>
        public static string DecToHex(int dec)
        {
            string result = dec.ToString("X2");
            //修改
            if (result.Length % 2 > 0)
            {
                result = "0" + result;
            }
            return result;
            //while (result.Length < 4)
            //{
            //    result = "0" + result;
            //}
            //return result.Substring(0, 2) + " " + result.Substring(2, 2);
        }
        public static string DevHexToDec(string hex)
        {
            hex = hex.Replace(" ", "");
            int temp = 0;
            for (int i = 0; i < hex.Length; i++)
            {
                temp += Convert.ToInt32(hex.Substring(i, 1)) * Convert.ToInt32(Math.Pow(16, 3 - i));//0001
            }
            return temp.ToString();
        }
        public static int HexToDec(string hex)
        {
            return Convert.ToInt32(hex, 16);
        }

        public static byte[] DecToHexToByte(int dec)
        {
            return DataHelper.HexToByte(DataHelper.DecToHex(dec));
        }

        public static int BytesToInt32(byte[] bytes)
        {
            return bytes.Length == 1 ? (int)bytes[0] : BitConverter.ToInt32(bytes, 0);
        }
        public static ushort BytesToUInt16(byte[] bytes)
        {
            return bytes.Length == 1 ? (ushort)bytes[0] : BitConverter.ToUInt16(bytes, 0);
        }
        public static string ByteToASCIIString(byte[] bytes)
        {
            //vsw str默认后面带\0
            return System.Text.Encoding.ASCII.GetString(bytes).Replace("\0", "");
        }
        public static string GetIp(byte[] bytes)
        {
            return string.Join(".", bytes);
        }
    }
}
