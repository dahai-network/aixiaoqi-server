using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.ESIM_MVNO
{
    public class Diversify
    {
        public static String getTransKey(String workKey, String random)
        {
            //这里的workKey参数即前面提到的MK，random即前面提到的分散数据，都是16进制的数据  
            if (null == workKey || 32 != workKey.Length || null == random || 16 != random.Length)
            {
                return null;
            }

            try
            {
                // 计算过程密钥左8字节  
                byte[] byteKey = DataHelper.HexToByte(workKey);//Hex类为专门做byte与Hex转换，此处不贴代码了，网上很多，若不愿做转化可直接传入byte数组作为参数  
                byte[] byteRandomRight = new byte[8];
                byte[] byteRandomLeft = DataHelper.HexToByte(random);
                for (int i = 0; i < byteRandomRight.Length; i++)
                {
                    byteRandomRight[i] = (byte)~byteRandomLeft[i];
                }

                //byte[] wkLeft = Des3.ThreeDES(byteKey, byteRandomLeft);// TriDES.encryptWithPKCS5Padding(byteRandomLeft, byteKey);
                //byte[] wkRight = Des3.ThreeDES(byteKey, byteRandomRight);//TriDES.encryptWithPKCS5Padding(byteRandomRight, byteKey);

                byte[] wkLeft = DesHelper.Des3EncodeECB(byteKey, null, byteRandomLeft);// TriDES.encryptWithPKCS5Padding(byteRandomLeft, byteKey);
                byte[] wkRight = DesHelper.Des3EncodeECB(byteKey, null, byteRandomRight);//

                byte[] result = new byte[16];
                for (int i = 0; i < wkLeft.Length; i++)
                {
                    result[i] = wkLeft[i];
                }

                for (int i = 8; i < result.Length; i++)
                {
                    result[i] = wkRight[i - 8];
                }

                return DataHelper.ByteToHex(result, result.Length);
            }
            catch (Exception e)
            {
            }

            return null;
        }
    }
}
