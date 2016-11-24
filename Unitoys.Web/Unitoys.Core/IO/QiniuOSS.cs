using Qiniu.IO;
using Qiniu.IO.Resumable;
using Qiniu.RS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Core
{
    public class QiniuOSS
    {


        const string bucketName = "aliniu";

        static QiniuOSS()
        {
            Qiniu.Conf.Config.ACCESS_KEY = "2xibNQUgu5hS31RmeO9MQDx-ICVhtwHGHz-GS54K";
            Qiniu.Conf.Config.SECRET_KEY = "fe113-9qHqBHPG8C7W20E7LIqQpsdHrMl7ZtiOrB";
        }
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="key"></param>
        /// <param name="stream"></param>
        public static async Task<bool> PutFileAsync(string key, Stream stream)
        {
            stream.Position = 0;
            key = key.Substring(1);
            var policy = new PutPolicy(bucketName, 3600);
            string upToken = policy.Token();
            PutExtra extra = new PutExtra();

            IOClient client = new IOClient();

            try
            {

                PutRet pr =await Task.Run<PutRet>(() =>
                {
                    return client.Put(upToken, key, stream, extra);
                }); 
                if (pr.OK)
                {
                    return true;
                }
                else
                {
                    LoggerHelper.Error("七牛存储返回错误:url" + key, pr.Exception);
                    return false;
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("七牛存储返回错误:url" + key, ex);
                return false;
            }
        }
    }
}
