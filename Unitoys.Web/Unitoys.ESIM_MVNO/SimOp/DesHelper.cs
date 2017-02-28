using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.ESIM_MVNO
{
    public class DesHelper
    {
        #region 3DES-CBC模式**

        /// <summary>
        /// DES3 CBC模式加密
        /// </summary>
        /// <param name="key">密钥</param>
        /// <param name="iv">IV</param>
        /// <param name="data">明文的byte数组</param>
        /// <returns>密文的byte数组</returns>
        public static byte[] Des3EncodeCBC(byte[] key, byte[] iv, byte[] data)
        {
            //复制于MSDN

            try
            {
                // Create a MemoryStream.
                MemoryStream mStream = new MemoryStream();

                TripleDESCryptoServiceProvider tdsp = new TripleDESCryptoServiceProvider();
                tdsp.Mode = CipherMode.CBC;             //默认值
                tdsp.Padding = PaddingMode.None;       //默认值

                // Create a CryptoStream using the MemoryStream 
                // and the passed key and initialization vector (IV).
                CryptoStream cStream = new CryptoStream(mStream,
                    tdsp.CreateEncryptor(key, iv),
                    CryptoStreamMode.Write);

                // Write the byte array to the crypto stream and flush it.
                cStream.Write(data, 0, data.Length);
                cStream.FlushFinalBlock();

                // Get an array of bytes from the 
                // MemoryStream that holds the 
                // encrypted data.
                byte[] ret = mStream.ToArray();

                // Close the streams.
                cStream.Close();
                mStream.Close();

                // Return the encrypted buffer.
                return ret;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
                return null;
            }
        }

        /// <summary>
        /// DES3 CBC模式解密
        /// </summary>
        /// <param name="key">密钥</param>
        /// <param name="iv">IV</param>
        /// <param name="data">密文的byte数组</param>
        /// <returns>明文的byte数组</returns>
        public static byte[] Des3DecodeCBC(byte[] key, byte[] iv, byte[] data)
        {
            try
            {
                // Create a new MemoryStream using the passed 
                // array of encrypted data.
                MemoryStream msDecrypt = new MemoryStream(data);

                TripleDESCryptoServiceProvider tdsp = new TripleDESCryptoServiceProvider();
                tdsp.Mode = CipherMode.CBC;
                tdsp.Padding = PaddingMode.PKCS7;

                // Create a CryptoStream using the MemoryStream 
                // and the passed key and initialization vector (IV).
                CryptoStream csDecrypt = new CryptoStream(msDecrypt,
                    tdsp.CreateDecryptor(key, iv),
                    CryptoStreamMode.Read);

                // Create buffer to hold the decrypted data.
                byte[] fromEncrypt = new byte[data.Length];

                // Read the decrypted data out of the crypto stream
                // and place it into the temporary buffer.
                csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);

                //Convert the buffer into a string and return it.
                return fromEncrypt;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
                return null;
            }
        }

        #endregion

        #region 3DES-ECB模式

        /// <summary>
        /// DES3 ECB模式加密
        /// </summary>
        /// <param name="key">密钥</param>
        /// <param name="iv">IV(当模式为ECB时，IV无用)</param>
        /// <param name="str">明文的byte数组</param>
        /// <returns>密文的byte数组</returns>
        public static byte[] Des3EncodeECB(byte[] key, byte[] iv, byte[] data)
        {
            try
            {
                // Create a MemoryStream.
                MemoryStream mStream = new MemoryStream();

                TripleDESCryptoServiceProvider tdsp = new TripleDESCryptoServiceProvider();
                tdsp.Mode = CipherMode.ECB;
                tdsp.Padding = PaddingMode.PKCS7;

                // Create a CryptoStream using the MemoryStream 
                // and the passed key and initialization vector (IV).
                CryptoStream cStream = new CryptoStream(mStream,
                    tdsp.CreateEncryptor(key, iv),
                    CryptoStreamMode.Write);

                // Write the byte array to the crypto stream and flush it.
                cStream.Write(data, 0, data.Length);
                cStream.FlushFinalBlock();

                // Get an array of bytes from the 
                // MemoryStream that holds the 
                // encrypted data.
                byte[] ret = mStream.ToArray();

                // Close the streams.
                cStream.Close();
                mStream.Close();

                // Return the encrypted buffer.
                return ret;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
                return null;
            }

        }

        /// <summary>
        /// DES3 ECB模式解密
        /// </summary>
        /// <param name="key">密钥</param>
        /// <param name="iv">IV(当模式为ECB时，IV无用)</param>
        /// <param name="str">密文的byte数组</param>
        /// <returns>明文的byte数组</returns>
        public static byte[] Des3DecodeECB(byte[] key, byte[] iv, byte[] data)
        {
            try
            {
                // Create a new MemoryStream using the passed 
                // array of encrypted data.
                MemoryStream msDecrypt = new MemoryStream(data);

                TripleDESCryptoServiceProvider tdsp = new TripleDESCryptoServiceProvider();
                tdsp.Mode = CipherMode.ECB;
                tdsp.Padding = PaddingMode.PKCS7;

                // Create a CryptoStream using the MemoryStream 
                // and the passed key and initialization vector (IV).
                CryptoStream csDecrypt = new CryptoStream(msDecrypt,
                    tdsp.CreateDecryptor(key, iv),
                    CryptoStreamMode.Read);

                // Create buffer to hold the decrypted data.
                byte[] fromEncrypt = new byte[data.Length];

                // Read the decrypted data out of the crypto stream
                // and place it into the temporary buffer.
                csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);

                //Convert the buffer into a string and return it.
                return fromEncrypt;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
                return null;
            }
        }

        /// <summary>
        /// 3DES加密及弱密钥处理
        /// </summary>
        /// <param name="key"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] ThreeDES(byte[] key, byte[] str)
        {
            TripleDESCryptoServiceProvider tdsc = new TripleDESCryptoServiceProvider();
            tdsc.Padding = PaddingMode.None;

            //byte[] IV = { 0xB0, 0xA2, 0xB8, 0xA3, 0xDA, 0xCC, 0xDA, 0xCC };
            //指定密匙长度，默认为192位  
            //tdsc.KeySize = 128;
            //使用指定的key和IV（加密向量）  
            Type t = Type.GetType("System.Security.Cryptography.CryptoAPITransformMode");
            object obj = t.GetField("Encrypt", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly).GetValue(t);
            MethodInfo mi = tdsc.GetType().GetMethod("_NewEncryptor", BindingFlags.Instance | BindingFlags.NonPublic);
            ICryptoTransform desCrypt = (ICryptoTransform)mi.Invoke(tdsc, new object[] { key, CipherMode.ECB, null, 0, obj });
            //tdsc.IV = IV;
            //加密模式，偏移  
            tdsc.Mode = CipherMode.ECB;

            //进行加密转换运算  
            //ICryptoTransform ct = tdsc.CreateDecryptor();  
            //8很关键，加密结果是8字节数组  
            byte[] results = desCrypt.TransformFinalBlock(str, 0, 8);

            return results;
        }
        #endregion

        #region DES-ECB
        /// <summary>
        /// Des-ECB加密
        /// </summary>
        /// <param name="dataHex">加密16进制数据</param>
        /// <param name="keyHex">加密16进制Key</param>
        /// <returns></returns>
        public static string DesEncodeECB(string dataHex, string keyHex) //数据为十六进制
        {
            try
            {
                //不足8位补0
                var dataLength = DataHelper.HexToByte(dataHex).Length;
                if (dataLength % 8 > 0)
                {
                    for (int i = 0; i < 8 - dataLength % 8; i++)
                    {
                        dataHex += "00";
                    }
                }
                byte[] in_data = DataHelper.HexToByte(dataHex);
                byte[] DES_KEY = DataHelper.HexToByte(keyHex);

                DES desEncrypt = new DESCryptoServiceProvider();
                desEncrypt.Mode = CipherMode.ECB;
                //desEncrypt.Key = ASCIIEncoding.ASCII.GetBytes(str_DES_KEY);
                desEncrypt.Key = DES_KEY;
                desEncrypt.Padding = PaddingMode.Zeros;

                byte[] R;

                using (MemoryStream stream = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(stream, desEncrypt.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(in_data, 0, in_data.Length);
                        //return Convert.ToBase64String(stream.ToArray());
                        R = stream.ToArray();
                    }
                }

                string return_str = "";
                foreach (byte b in R)
                {
                    return_str += b.ToString("X2");
                }
                //return_str = return_str.Substring(0, 16);
                return return_str;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static string DesDecryptECB(string dataHex, string keyHex)//数据和密钥为十六进制
        {
            byte[] shuju = DataHelper.HexToByte(dataHex);
            byte[] keys = DataHelper.HexToByte(keyHex);
            DES desDecrypt = new DESCryptoServiceProvider();
            desDecrypt.Mode = CipherMode.ECB;
            desDecrypt.Key = keys;
            //desDecrypt.Padding = System.Security.Cryptography.PaddingMode.None;
            desDecrypt.Padding = PaddingMode.Zeros;
            byte[] Buffer = shuju;
            ICryptoTransform transForm = desDecrypt.CreateDecryptor();
            byte[] R;
            R = transForm.TransformFinalBlock(Buffer, 0, Buffer.Length);
            string return_str = "";
            foreach (byte b in R)
            {
                return_str += b.ToString("X2");
            }
            return return_str;
        }
        #endregion
    }
}
