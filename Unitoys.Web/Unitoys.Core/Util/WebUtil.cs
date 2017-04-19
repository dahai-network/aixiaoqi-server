using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Top.Api;
using Top.Api.Request;
using Top.Api.Response;
using Unitoys.Core;
using Unitoys.Model;

namespace Unitoys.Core
{
    public class WebUtil
    {

        private static SessionHelper session = new SessionHelper();
        private static ApiSessionHelper apiSession = new ApiSessionHelper();

        #region 管理员Session
        /// <summary>
        /// 获取管理员Session
        /// </summary>
        /// <returns></returns>
        public static LoginUserInfo GetManageUserSession()
        {
            return session["MUser"] as LoginUserInfo;
        }
        /// <summary>
        /// 设置管理员Session
        /// </summary>
        /// <param name="modal"></param>
        public static void SetManageUserSession(LoginUserInfo modal)
        {
            session["MUser"] = modal;
        }
        /// <summary>
        /// 移除管理员Session
        /// </summary>
        public static void RemoveManageUserSession()
        {
            session.Remove("MUser");
        }

        /// <summary>
        /// 查看改帐号是否有在其他地方登录
        /// </summary>
        /// <returns></returns>
        public static bool isRepeatLogin(Guid ID)
        {
            return session.isRepeatLogin("MUser", ID);
        }

        #endregion

        #region 用户Session
        /// <summary>
        /// 获取用户Session
        /// </summary>
        /// <returns></returns>
        public static LoginUserInfo GetApiUserSession(string token)
        {
            return apiSession[token] as LoginUserInfo;
        }
        /// <summary>
        /// 获取用户Session
        /// </summary>
        /// <returns></returns>
        public static bool ApiUserSessionExiste(string key)
        {
            return apiSession[key] == null;
        }
        /// <summary>
        /// 获取用户Session
        /// </summary>
        /// <returns></returns>
        public static LoginUserInfo GetApiUserSession()
        {
            string token = HttpContext.Current.Request.Headers["token"];

            //todo 由于测试api中header发送有问题，暂时使用此种方式进行测试
            if (HttpContext.Current.Request.QueryString["expires"] == "1471316792")
            {
                token = HttpContext.Current.Request.QueryString["token"];
            }
            return apiSession[token] as LoginUserInfo;
        }

        /// <summary>
        /// 获取用户Session
        /// </summary>
        /// <returns></returns>
        public static string GetApiKeyByTel(string tel)
        {
            return apiSession.GetStrSession(tel);
        }

        /// <summary>
        /// 获取用户Session
        /// </summary>
        /// <returns></returns>
        public static IDictionary<string, string> GetApiKeyByTels(IEnumerable<string> tels)
        {
            return apiSession.GetStrSession(tels);
        }
        /// <summary>
        /// 设置用户Session
        /// </summary>
        /// <param name="modal"></param>
        public static void SetApiUserSession(string token, LoginUserInfo modal)
        {
            apiSession[token] = modal;
        }
        /// <summary>
        /// 移除用户Session
        /// </summary>
        public static void RemoveApiUserSession(string token)
        {
            apiSession.Remove(token);
        }
        /// <summary>
        /// 移除用户Session
        /// </summary>
        public static void RemoveApiUserSession()
        {
            string token = HttpContext.Current.Request.Headers["token"];

            //todo 由于测试api中header发送有问题，暂时使用此种方式进行测试
            if (HttpContext.Current.Request.QueryString["expires"] == "1471316792")
            {
                token = HttpContext.Current.Request.QueryString["token"];
            }
            apiSession.Remove(token);
        }
        /// <summary>
        /// 判断用户Session
        /// </summary>
        public static long ExistsSession(string key)
        {
            return apiSession.Exists(key);
        }
        #endregion

        #region 上传文件，上传图片
        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="image">图片</param>
        /// <returns>成功返回图片路径，失败-1：没有图片，-2：格式不对，-3：超过限制大小</returns>
        public static async Task<string> UploadImgAsync(HttpPostedFileBase image)
        {
            var beginTime = CommonHelper.GetDateTimeMS();
            if (image != null && image.ContentLength > 0)
            {
                string fileName = image.FileName;
                string extension = Path.GetExtension(fileName);
                if (!ValidateHelper.IsImgFileName(fileName) || !CommonHelper.IsInArray(extension, ".jpg,.jpeg,.png,.gif"))
                    return "-2";

                int fileSize = image.ContentLength;
                if (fileSize > UTConfig.SiteConfig.MaxFilePicSize)
                    return "-3";

                //存放的路径
                string filePath = String.Format("/Unitoys/{0}/{1}/", CommonHelper.GetYear(), CommonHelper.GetMonth());

                //新的文件名
                string newFileName = string.Format("{0}{1}", DateTime.Now.ToString("yyMMddHHmmssfffffff"), extension);

                //图片类型
                string contentType = extension == ".jpg" ? "image/jpeg" : extension == "png" ? "image/png" : "image/jpeg";


                //存储到阿里云OSS。
                var resultAliyun = AliyunOSS.PutFileAsync(filePath + newFileName, image.InputStream, contentType);

                //存储到七牛
                //var resultQinqiu = QiniuOSS.PutFileAsync(filePath + newFileName, image.InputStream);

                //存储在本地服务器
                Task.Run(() =>
                {
                    string dirPath = IOHelper.GetMapPath(filePath);
                    if (!Directory.Exists(dirPath))
                        Directory.CreateDirectory(dirPath);
                    image.SaveAs(dirPath + newFileName);
                }).Wait();


                if (await resultAliyun)
                {
                    return filePath + newFileName;
                }
                else
                {
                    return "-1";
                }
            }
            else
            {
                return "-1";
            }
        }
        #endregion

        #region 上传文件，上传图片
        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="image">图片</param>
        /// <returns>成功返回图片路径，失败-1：没有图片，-2：格式不对，-3：超过限制大小</returns>
        public static async Task<string> UploadImgAsync(HttpPostedFile image)
        {
            //存放的路径
            string filePath = String.Format("/Unitoys/{0}/{1}/", CommonHelper.GetYear(), CommonHelper.GetMonth());

            //新的文件名
            string newFileName = DateTime.Now.ToString("yyMMddHHmmssfffffff");

            return await UploadImgAsync(image, filePath, newFileName);
        }
        #endregion

        #region 上传文件，上传图片
        /// <summary>
        /// 上传图片（保存头像会使用到这个函数）
        /// </summary>
        /// <param name="image">图片</param>
        /// <param name="fileUrl">要存放的目录+文件名（不包含后缀）</param>
        /// <returns>成功返回图片路径，失败:-1, 没有图片、格式不对:-2, 超过限制大小:-3 </returns>
        public static async Task<string> UploadImgAsync(HttpPostedFile image, string filePath, string fileName)
        {
            var beginTime = CommonHelper.GetDateTimeMS();
            if (image != null && image.ContentLength > 0)
            {
                string extension = Path.GetExtension(image.FileName);
                if (!ValidateHelper.IsImgFileName(image.FileName) || !CommonHelper.IsInArray(extension, ".jpg,.jpeg,.png,.gif"))
                    return "-2";

                int fileSize = image.ContentLength;
                if (fileSize > UTConfig.SiteConfig.MaxFilePicSize)
                    return "-3";

                //新的文件名
                string newFileName = string.Format("{0}{1}", fileName, extension);

                //图片类型
                string contentType = extension == ".jpg" ? "image/jpeg" : extension == "png" ? "image/png" : "image/jpeg";

                Stream stream = image.InputStream;


                //存储到阿里云OSS。
                var resultAliyun = AliyunOSS.PutFileAsync(filePath + newFileName, stream, contentType);

                //存储在本地服务器
                Task.Run(() =>
                {
                    string dirPath = IOHelper.GetMapPath(filePath);
                    if (!Directory.Exists(dirPath))
                        Directory.CreateDirectory(dirPath);
                    image.SaveAs(dirPath + newFileName);
                }).Wait();

                if (await resultAliyun)
                {
                    return filePath + newFileName;
                }
                else
                {
                    return "-1";
                }
            }
            else
            {
                return "-1";
            }
        }
        #endregion


        #region 发送短信
        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="toNum">号码</param>
        /// <param name="content">短信内容</param>
        /// <returns></returns>
        public static async Task<ResultJson> SendSMSAsync(string toNum, string code, int type)
        {
            const string requestUri = "http://gw.api.taobao.com/router/rest";

            const string appkey = "23437309";
            const string secret = "664d9543f5de9e1cba9d86eaec944f80";
            string SmsFreeSignName = "";
            string SmsTemplateCode = "";
            string SmsParam = "";
            if (type == 1)
            {
                //注册验证码
                SmsFreeSignName = "爱小器";
                SmsTemplateCode = "SMS_13215895";
                SmsParam = "{\"code\":\"" + code + "\",\"product\":\"" + "爱小器" + "\"}";

            }
            else if (type == 2)
            {
                //找回密码
                SmsFreeSignName = "爱小器";
                SmsTemplateCode = "SMS_13266056";
                SmsParam = "{\"code\":\"" + code + "\"}";
            }
            else if (type == 3)
            {
                //找回密码
                SmsFreeSignName = "爱小器";
                SmsTemplateCode = "SMS_31695052";
                SmsParam = "{\"code\":\"" + code + "\"}";
            }
            ITopClient client = new DefaultTopClient(requestUri, appkey, secret);
            AlibabaAliqinFcSmsNumSendRequest req = new AlibabaAliqinFcSmsNumSendRequest();
            req.Extend = toNum;
            req.SmsType = "normal";
            req.SmsFreeSignName = SmsFreeSignName;
            req.SmsParam = SmsParam;
            req.RecNum = toNum;
            req.SmsTemplateCode = SmsTemplateCode;
            AlibabaAliqinFcSmsNumSendResponse rsp = client.Execute(req);
            ResultJson model = new ResultJson()
            {
                code = rsp.ErrCode == null ? rsp.Result.ErrCode : rsp.ErrCode,
                msg = rsp.ErrMsg + "-" + rsp.SubErrMsg
            };
            return model;
        }
        public class ResultJson
        {
            public string code { get; set; }
            public string msg { get; set; }
        }
        #endregion
    }
}
