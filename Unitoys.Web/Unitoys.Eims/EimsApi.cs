using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Unitoys.Core;

namespace Unitoys.Eims
{
    public class EimsApi
    {
        #region 0.初始化
        /// <summary>
        /// EIMS的IP地址
        /// </summary>
        static string host = "120.25.91.50";//"203.186.75.167";
        /// <summary>
        /// EIMS的网页后台端口
        /// </summary>
        static string port = "20003";
        /// <summary>
        /// 用户名
        /// </summary>
        static string username = "ejoin-eims";
        /// <summary>
        /// 密码
        /// </summary>
        static string password = "123456";
        /// <summary>
        /// 版本号
        /// </summary>
        static string version = "1.0";
        /// <summary>
        /// 短信报告Url
        /// </summary>
        public string SMSSendReportUrl
        {
            get
            {
                return UTConfig.SiteConfig.SiteHost + "/api/Eims/SMSReport";
            }
        }
        /// <summary>
        /// at&ussd报告Url
        /// </summary>
        public string ATUSSDReportUrl
        {
            get
            {
                return UTConfig.SiteConfig.SiteHost + "/api/Eims/ATUSSDReport";
            }
        }
        #endregion

        #region 1.SMS-发送,状态报告,状态查询,查询
        /*
         注：
1.	当不指定设备端口或者sim卡时，系统自动选择最佳路由发送短信给每一个接收者；
2.	当指定设备端口或者sim卡，并且接收者只有一个时，短信将会从每一个指定的端口或sim卡发送出去；
3.	当指定设备端口或者sim卡，并且指定多个接收者时，系统选择指定的端口和sim卡发送短信给每一个接收者；
4.	编码集= utf8时，表明content是utf-8的字符串，编码集=base64时，表明content是utf-8的BASE64编码的字符串；
         */
        //短信发送
        /// <summary>
        /// 客户端通过网络用HTTP POST请求提交短信发送请求给EIMS，任务信息通过JSON格式的数据携带
        /// </summary>
        /// <returns></returns>
        public async Task<TranslateEimsSMSSendModel> SMSSend(List<SMSTaskModel> list)
        {
            string uri = "/eims_post_sms.html";
            return await EimsApiAccess<TranslateEimsSMSSendModel>(uri, new Dictionary<string, dynamic>()
            {
                {"sr_url",this.SMSSendReportUrl},//状态报告url
                {"sms_url",null},//收到的短信提交url
                {"task_num",list.Count},
                {"tasks",list}
            }, "Post");
        }

        /*
         注：
1.	tid对应于任务请求中的任务ID；
2.	sending,sent,failed均为该任务的累计统计数据；
3.	sdr（成功详情记录）为两次报告之间的记录。
4.	fdr（失败详情记录）为两次报告之间的记录。
         */
        /// <summary>
        /// 状态报告,Eims发给客户端，客户端的处理方法
        /// 创建页面来接收回调
        /// http://host:port/eims_post_smsreport.html
        /// host: 客户端的IP地址
        /// port: 客户端的网页后台端口，默认为80，可选。
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        //public async Task<TranslateEimsModel> SMSReport(object requestData)
        //{
        //    return null;
        //}

        /// <summary>
        /// 状态查询Get
        /// 客户端通过网络用HTTP GET请求提交短信发送状态查询请求给EIMS。
        /// 备注：上述的tid、devname、iccid，至少有一个是非空的。
        /// 可以支持单个属性的查找也可以支持多个属性同时查找。当只有devname而port为空时，查询的就是整个设备的所有端口。
        /// </summary>
        /// <param name="tid"></param>
        /// <param name="devname"></param>
        /// <param name="port"></param>
        /// <param name="iccid"></param>
        /// <returns></returns>
        public async Task<TranslateEimsSMSReportQueryModel> SMSReportQuery(string tid, string devname, string port, string iccid)
        {
            string uri = string.Format("/eims_query_smsreport.html?version={0}&username={1}&password={2}&tid={3}&devname={4}&port={5}&iccid={6}"
                , version
                , username
                , password
                , tid
                , devname
                , port
                , iccid
                );
            return await EimsApiAccess<TranslateEimsSMSReportQueryModel>(uri, null, "Get");
        }

        //Get
        /// <summary>
        /// 短信查询
        /// 客户可以通过HTTP的GET请求主动查询EIMS接收到的SMS。
        /// </summary>
        /// <param name="sms_num">
        /// 指定要查询的短信数
        /// 0：表示查询所有短信
        /// </param>
        /// <param name="start">
        /// 查询开始的时间戳
        /// 若是这个为空，默认查询前10分钟收到的短信
        /// </param>
        /// <param name="devname">设备名称</param>
        /// <param name="port">
        /// 端口号
        /// 一个或多个（逗号，短横线连接）发送端口（从1开始）
        /// </param>
        /// <param name="iccid">
        /// sim卡唯一标识
        /// 一个或多个（逗号分隔）发送sim卡的iccid
        /// </param>
        /// <returns></returns>
        public async Task<TranslateEimsSMSQueryModel> SMSQuery(int sms_num = 0, int start = 0, string devname = "", string port = "", string iccid = "")
        {
            //int startTP = 0;
            //if (start.HasValue)
            //{
            //    startTP = CommonHelper.ConvertDateTimeInt(start.Value);
            //}
            //string uri = string.Format("/eims_get_sms.html?sms_num={0}&start={1}&devname={2}&port={3}&iccid={4}"
            //    , sms_num
            //    , startTP
            //    , devname
            //    , port
            //    , iccid
            //    );
            string uri = "/eims_get_sms.html";
            return await EimsApiAccess<TranslateEimsSMSQueryModel>(uri, new Dictionary<string, dynamic>()
            {
                {"sms_num",sms_num},
                {"start",start},
                {"devname",devname},
                {"port",port},
                {"iccid",iccid}
            }, "Get");
        }
        #endregion

        #region 2.AT&USSD-发送,报告
        /// <summary>
        /// 发送AT&USSD
        /// 客户端通过网络用HTTP POST请求提交AT&USSD请求给EIMS，任务信息通过JSON格式的数据携带。
        /// 备注：
        /// 1、	devname、iccid，至少有一个是非空的。
        ///     可以支持单个属性的发送也可以支持多个属性同时发送。当只有devname而port为空时，就是往整个设备的所有端口发送。
        /// 2、	编码集= utf8时，表明content是utf-8的字符串，编码集=base64时，表明content是utf-8的BASE64编码的字符串；
        /// </summary>
        /// <param name="type">at/ussd</param>
        /// <param name="devname">设备名称</param>
        /// <param name="port">一个或多个（逗号，短横线连接）发送端口（从1开始）</param>
        /// <param name="iccid">一个或多个（逗号分隔）发送sim卡的iccid</param>
        /// <param name="content">命令内容</param>
        /// <param name="chs">编码集（utf8|base64）</param>
        /// <returns></returns>
        public async Task<TranslateEimsATUSSDModel> ATUSSDSend(string type, string devname, string port, string iccid, string content, string chs)
        {
            if (string.IsNullOrEmpty(devname) && string.IsNullOrEmpty(iccid))
            {
                throw new Exception("devname、iccid，至少有一个是非空的。");
            }
            string uri = "/eims_post_atussd_report.html";//"/eims_post_atussd.html";
            return await EimsApiAccess<TranslateEimsATUSSDModel>(uri, new Dictionary<string, dynamic>()
            {
                {"sr_url",this.ATUSSDReportUrl},//AT&USSD报告url
                {"type",type},
                {"devname",devname},
                {"port",port},
                {"iccid",iccid},
                {"content",content},
                {"chs",chs},
            }, "Post");
        }

        /// <summary>
        /// AT&USSD报告,Eims发给客户端，客户端的处理方法
        /// EIMS会通过HTTP  POST请求，向客户端推送AT&USSD的结果报告。
        /// 创建页面来接收回调
        /// http://host:port/eims_post_atussd_report.html
        /// host: 客户端的IP地址
        /// port: 客户端的网页后台端口，默认为80，可选。

        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        //public async Task<object> ATUSSDReport(object requestData)
        //{
        //    return null;
        //}
        #endregion

        #region 3.Http访问封装
        /// <summary>
        /// EimsApi访问
        /// </summary>
        /// <param name="uri">例：/DahaiProvision/WS/PackagePlanAdd</param>
        /// <param name="jsonData">Json字典
        /// <param name="accessMethod">访问方法Get/Post</param>
        /// <returns></returns>
        private static async Task<T> EimsApiAccess<T>(string uri, Dictionary<string, dynamic> jsonData, string accessMethod)
        {
            using (HttpClient client = new HttpClient())
            {
                if (jsonData == null)
                {
                    jsonData = new Dictionary<string, dynamic>();
                }
                //if (accessMethod == "Post")
                //{
                var timestamp = CommonHelper.GetDateTimeInt().ToString();
                jsonData.Add("version", version);
                jsonData.Add("username", username);
                jsonData.Add("password", SecureHelper.MD5(password + timestamp));
                jsonData.Add("key", timestamp);
                //}

                //删除null的值
                List<string> keyRemovelist = new List<string>();
                foreach (var item in jsonData)
                {
                    if (item.Value == null)
                    {
                        keyRemovelist.Add(item.Key);
                    }
                }
                foreach (var item in keyRemovelist)
                {
                    jsonData.Remove(item);
                }

                var requestJson = JsonConvert.SerializeObject(jsonData);

                HttpContent httpContent = new StringContent(requestJson);

                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

                string url = string.Format("http://{0}:{1}", host, port) + uri;

                var responseMsg = new HttpResponseMessage();
                if (accessMethod == "Post")
                {
                    responseMsg = await client.PostAsync(url, httpContent);
                }
                else if (accessMethod == "Get")
                {
                    if (url.Contains("?"))
                    {
                        url += "&" + string.Join("&", jsonData.Select(c => c.Key + "=" + c.Value));
                    }
                    else
                    {
                        url += "?" + string.Join("&", jsonData.Select(c => c.Key + "=" + c.Value));
                    }
                    responseMsg = await client.GetAsync(url);
                }

                if (responseMsg.IsSuccessStatusCode)
                {
                    if (responseMsg.Content.Headers.ContentType.MediaType == "application/json")
                    {
                        T model = await responseMsg.Content.ReadAsAsync<T>();
                        return model;
                    }

                }
                string msg = await responseMsg.Content.ReadAsStringAsync();
                throw new Exception("Http响应失败,响应内容如下" + msg);
            }
        }
        #endregion
    }
}
