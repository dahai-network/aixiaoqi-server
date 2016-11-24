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

namespace Unitoys.CiticTelecom
{
    public class WSApi
    {
        //static long timestamp = ConvertDateTimeInt(DateTime.Now);
        static string nonce = "353881508";//353881507
        static string secret_key = "DH1616TEST20160621";

        #region 1.PackagePlan套餐接口-添加,激活,查询,删除

        /// <summary>
        /// 添加套餐
        /// </summary>
        /// <returns>status</returns>
        public async Task<TranslateStatusModel> PackagePlanAdd(string imsi, string planName, int noOfDays, int tariff, string currency)
        {
            //var imsi = "454011091000089";
            //var planName = "XM_MAC_500M_7D_VOL";
            //var noOfDays = 7;
            //var tariff = 100;
            //var currency = "CNY";
            return await WSApiAccess<TranslateStatusModel>("/DahaiProvision/WS/PackagePlanAdd", new Dictionary<string, string>()
            {
                {"imsi",imsi},
                { "planName", planName },
                { "noOfDays", noOfDays+"" },
                { "tariff", tariff+"" },
                { "currency", currency },
                { "remark", "" }
            });
        }

        /// <summary>
        /// 激活套餐
        /// </summary>
        /// <returns>status</returns>
        public async Task<TranslateStatusModel> PackagePlanActivate(string imsi, string planName, int noOfDays, int tariff, string currency)
        {
            return await WSApiAccess<TranslateStatusModel>("/DahaiProvision/WS/PackagePlanActivate", new Dictionary<string, string>()
            {
                {"imsi",imsi},
                { "planName", planName },
                { "noOfDays", noOfDays+"" },
                { "tariff", tariff+"" },
                { "currency", currency },
                { "remark", "" }
            });
        }

        /// <summary>
        /// 查询套餐
        /// </summary>
        /// <returns>TranslateModel</returns>
        public async Task<TranslatePackagePlanModel> PackagePlanQuery(string imsi)
        {
            return await WSApiAccess<TranslatePackagePlanModel>("/DahaiProvision/WS/PackagePlanQuery", new Dictionary<string, string>()
            {
                {"imsi",imsi}
            });
        }

        /// <summary>
        /// 删除套餐
        /// </summary>
        /// <returns>status</returns>
        public async Task<TranslateStatusModel> PackagePlanDelete(string imsi, string planName)
        {
            return await WSApiAccess<TranslateStatusModel>("/DahaiProvision/WS/PackagePlanDelete", new Dictionary<string, string>()
            {
                {"imsi",imsi},
                { "planName", planName }
            });
        }



        

        #endregion

        #region 2.SubscriptionUpdate套餐接口-更新,查询

        /// <summary>
        /// 订阅更新
        /// </summary>
        /// <param name="imsi"></param>
        /// <param name="actionCode">
        /// 1: Turn On
        /// 2: Turn Off
        /// </param>
        /// <returns>status</returns>
        public async Task<TranslateStatusModel> SubscriptionUpdate(string imsi, int actionCode)
        {
            //var imsi = "454011091000089";
            //var planName = "XM_MAC_500M_7D_VOL";
            //var noOfDays = 7;
            //var tariff = 100;
            //var currency = "CNY";
            return await WSApiAccess<TranslateStatusModel>("/DahaiProvision/WS/SubscriptionUpdate", new Dictionary<string, string>()
            {
                {"imsi",imsi},
                { "actionCode", actionCode+"" }
            });
        }

        /// <summary>
        /// 订阅查询
        /// </summary>
        /// <returns>TranslateSubscriptionModel</returns>
        public async Task<TranslateSubscriptionModel> SubscriptionQuery(string imsi)
        {
            return await WSApiAccess<TranslateSubscriptionModel>("/DahaiProvision/WS/SubscriptionQuery", new Dictionary<string, string>()
            {
                {"imsi",imsi}
            });
        }




        

        #endregion

        #region 3.Keep Alive保持连接接口-保持连接
        /*Customer calls this function in a certain period of time frequently such as five minutes
once to ensure the connectivity between customer side and CITIC side is properly.*/

        /// <summary>
        /// 保持连接
        /// </summary>
        /// <param name="imsi"></param>
        /// <param name="actionCode">
        /// 1: Turn On
        /// 2: Turn Off
        /// </param>
        /// <returns>status</returns>
        public async Task<TranslateStatusModel> KeepAlive()
        {
            return await WSApiAccess<TranslateStatusModel>("/DahaiProvision/WS/KeepAlive", new Dictionary<string, string>()
            {

            });
        }
        #endregion

        /*
         Customer Usage Alert Web Service
         */
        #region 4.Balance平衡接口-Remind,DataActivationTimeRemind

        /// <summary>
        /// CITIC call this function when customer’s user reach predefined low data usage cases
        /// </summary>
        /// <param name="imsi"></param>
        /// <param name="alertType">
        /// Alert Types:
        /// 21%Balance - 21% of usage
        /// balance
        /// 1%Balance - 1% of usage balance
        /// SpeedRestrict - speed restriction
        /// usage tier reached
        /// </param>
        /// <returns></returns>
        public async Task<TranslateBalanceModel> BalanceRemind(string imsi, string alertType)
        {
            return await WSApiAccess<TranslateBalanceModel>("/DahaiProvision/citic/balanceRemind", new Dictionary<string, string>()
            {
                {"imsi",imsi},
                {"alertType",alertType}
            });
        }

        /// <summary>
        /// CITIC call this function when customer’s user reach predefined low data usage cases
        /// </summary>
        /// <param name="imsi"></param>
        /// <param name="alertType">
        /// Only applicable if status = 0
        /// Data activation time when the user
        /// create PDP request
        /// (Format: YYYY-MM-DD
        /// HH24:MI:SS)
        /// e.g.2015-07-02 15:00:00
        /// </param>
        /// <returns></returns>
        public async Task<TranslateBalanceModel> DataActivationTimeRemind(string imsi, string authenticationTime)
        {
            return await WSApiAccess<TranslateBalanceModel>("/DahaiProvision/citic/authenticationTimeRemind", new Dictionary<string, string>()
            {
                {"imsi",imsi},
                {"authenticationTime",authenticationTime}
            });
        }

       
        #endregion


        //MD5
        static public string MD5_Hash(string str_md5_in)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bytes_md5_in = UTF8Encoding.Default.GetBytes(str_md5_in);
            byte[] bytes_md5_out = md5.ComputeHash(bytes_md5_in);
            string str_md5_out = BitConverter.ToString(bytes_md5_out);
            //str_md5_out = str_md5_out.Replace("-", "");
            return str_md5_out;
        }

        //SHA1
        static public string SHA1_Hash(string str_sha1_in)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] bytes_sha1_in = UTF8Encoding.Default.GetBytes(str_sha1_in);
            byte[] bytes_sha1_out = sha1.ComputeHash(bytes_sha1_in);
            string str_sha1_out = BitConverter.ToString(bytes_sha1_out);
            str_sha1_out = str_sha1_out.Replace("-", "");
            return str_sha1_out;
        }
        public static long ConvertDateTimeInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (long)(time - startTime).TotalSeconds;
        }

        /// <summary>
        /// WSApi访问
        /// </summary>
        /// <param name="uri">例：/DahaiProvision/WS/PackagePlanAdd</param>
        /// <param name="jsonData">Json字典
        /// 以下已包含
        /// timestamp
        /// nonce
        /// signature：除备注外参数相加生成
        /// </param>
        /// <returns></returns>
        private static async Task<T> WSApiAccess<T>(string uri, Dictionary<string, string> jsonData)
        {
            using (HttpClient client = new HttpClient())
            {
                long timestamp = 1470273696;
                jsonData.Add("timestamp", timestamp + "");
                jsonData.Add("nonce", nonce);
                string strContent = "";
                foreach (var item in jsonData)
                {
                    if (item.Key != "remark")
                    {
                        strContent += item.Value;
                    }
                }
                var signature = SHA1_Hash(strContent + secret_key).ToLower();
                jsonData.Add("signature", signature);

                var requestJson = JsonConvert.SerializeObject(jsonData);

                HttpContent httpContent = new StringContent(requestJson);

                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;


                var responseMsg = await client.PostAsync("https://202.68.194.18:18051" + uri, httpContent);


                if (responseMsg.IsSuccessStatusCode)
                {
                    T model = await responseMsg.Content.ReadAsAsync<T>();
                    return model;
                }
                string msg = await responseMsg.Content.ReadAsStringAsync();
                throw new Exception("Http响应失败,响应内容如下" + msg);
            }
        }

    }
}
