using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Core
{
    public class UpYunOSS
    {
        const string buckeTname = "aliniu";
        const string userName = "aliniutest";
        const string passWord = "ali168.com";

        private static UpYun upYun = new UpYun(buckeTname, userName, passWord);

        public static async Task<bool> PutFileAsync(string path, Stream uploadStream, bool auto_mkdir)
        {
            byte[] bytes = StreamToBytes(uploadStream);
            return await PutFileAsync(path, bytes, auto_mkdir);
        }
        public static async Task<bool> PutFileAsync(string path, byte[] data, bool auto_mkdir)
        {
            return await upYun.writeFileAsync(path, data, auto_mkdir);
        }
        private static byte[] StreamToBytes(Stream stream)
        {
            stream.Position = 0;
            int size = Convert.ToInt32(stream.Length);
            int read = 0;
            MemoryStream ms = new MemoryStream();
            byte[] buffer = new byte[size];
            do
            {
                buffer = new byte[size];
                read = stream.Read(buffer, 0, size);
                ms.Write(buffer, 0, read);
            } while (read >= size);


            return ms.ToArray();

        }
        public static bool PutFile(string path, Stream uploadStream, bool auto_mkdir)
        {
            byte[] bytes = StreamToBytes(uploadStream);
            return PutFile(path, bytes, auto_mkdir);
        }
        public static bool PutFile(string path, byte[] data, bool auto_mkdir)
        {
            return upYun.writeFile(path, data, auto_mkdir);
        }
    }
}
