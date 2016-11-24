using Aliyun.OSS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Core
{
    public class AliyunOSS
    {
        const string bucketName = "unitoys";
        const string accessKeyId = "5Ed6MLmrrukPtr26";
        const string accessKeySecret = "kiNiyHw9rGyo5nQMu6KLjI7MlbOX1t";
        const string endpoint = "http://oss-cn-shenzhen-internal.aliyuncs.com/";
                                        
     
        /// <summary>
        /// 上传指定的文件到指定的OSS的存储空间
        /// </summary>
        /// <param name="bucketName">指定的存储空间名称</param>
        /// <param name="key">文件的在OSS上保存的名称</param>
        /// <param name="fileToUpload">指定上传文件的本地路径</param>
        public static async Task<bool> PutFileAsync(string filePath, Stream uploadStream, string contentType)
        {
            var client = new OssClient(endpoint, accessKeyId, accessKeySecret);
            filePath = filePath.Substring(1);
            uploadStream.Position = 0;
            var metadata = new ObjectMetadata();
            metadata.ContentType = contentType;
            metadata.ContentLength = uploadStream.Length;

            try
            {
            
                //新增了一个BeginPutObject方法。之前以为官方提供的方法参数过多，导致metadata无法接收，所有把bucketName和filePath合并到一个参数。
                var result = await Task<PutObjectResult>.Factory.FromAsync(
                                client.BeginPutObject,
                                client.EndPutObject,
                                 bucketName, filePath , uploadStream, metadata);
 
                return true;
                
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("aliyunOSS:", ex);
                return false;
            }

        }

        public static void DeleteFileAsync(string filePath)
        {
            var client = new OssClient(endpoint, accessKeyId, accessKeySecret);
            client.DeleteObject(bucketName, filePath);
        }
    }
}
