using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Core.Util
{
    public class ESIMUtil
    {
        private readonly static string _appid = "mcbed8gi0a4u3sv1";

        private readonly static string _secret = "4d0804gxzybzfp5z";

        /// <summary>
        /// 购买产品(套餐)
        /// </summary>
        /// <param name="productId">产品编号（与ESim上的对应）</param>
        /// <param name="phone">购买者手机号</param>
        /// <param name="tradeId">交易号</param>
        /// <param name="beginTime">套餐开始时间</param>
        /// <param name="quantity">购买套餐数量</param>
        /// <returns></returns>
        public static async Task<ResultJson> BuyProduct(string productId, string phone, string tradeId, int beginTime, int quantity)
        {
            //TODO 先临时写死套餐订单卡激活测试数据
            return new ResultJson()
            {
                errorCode = "0",
                msg = null,
                tradeld = "8022201610081108534716796",
                orderid = "1000746",
                data = "FEFEFE7124C1C60816A650613F1F08A2BB01C96BE50203F00212B2F4ABD24B45C291C9F0B92A66A8CDD62FD28A150D639F05FA913F9BF31882BFF6490366EA9EB1EA411654D086F7ECE0020052DA120089575DAF20CA22328E2167EDB98AC1E488DABADE2BF6BA76D2C5B265139A4E06B1E5D69A9B55E25A024009D33D093FBA2729B753A9CFF510C2C152"
            };
            //productId = "DH10001";
            //phone = "18850161016";
            //tradeId = "201607211532";
            //beginTime = CommonHelper.GetDateTime("yyyyMMddHHmm");
            //quantity = 1;
            using (HttpClient client = new HttpClient())
            {

                try
                {
                    var nonce = CommonHelper.Random();

                    var timestamp = CommonHelper.GetDateTimeInt().ToString();
                    ///生成签名
                    var signature = SecureHelper.SHA1_Hash(String.Format("{0}|{1}|{2}", _secret, nonce, timestamp)).ToLower();
                    //添加到请求头
                    client.DefaultRequestHeaders.Add("appid", _appid);
                    client.DefaultRequestHeaders.Add("nonce", nonce);
                    client.DefaultRequestHeaders.Add("timestamp", timestamp);
                    client.DefaultRequestHeaders.Add("signature", signature);

                    var requestJson = JsonConvert.SerializeObject(new
                    {
                        productId = productId,
                        phone = phone,
                        tradeId = tradeId,
                        beginTime = CommonHelper.GetTime(beginTime + "").ToString("yyyyMMddHHmm"),
                        quantity = quantity
                    });

                    HttpContent httpContent = new StringContent(requestJson);

                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");


                    var result = await client.PostAsync(String.Format("http://test.yiroaming.com:8080/pbs/api/buyProduct"), httpContent).Result.Content.ReadAsStringAsync();

                    ResultJson resultJson = JsonConvert.DeserializeObject<ResultJson>(result);

                    return resultJson;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
        }
        public static ResultJson cancel(string orderid)
        {
            using (HttpClient client = new HttpClient())
            {

                try
                {
                    var nonce = CommonHelper.Random();

                    var timestamp = CommonHelper.GetDateTimeInt().ToString();
                    ///生成签名
                    var signature = SecureHelper.SHA1_Hash(String.Format("{0}|{1}|{2}", _secret, nonce, timestamp)).ToLower();
                    //添加到请求头
                    client.DefaultRequestHeaders.Add("appid", _appid);
                    client.DefaultRequestHeaders.Add("nonce", nonce);
                    client.DefaultRequestHeaders.Add("timestamp", timestamp);
                    client.DefaultRequestHeaders.Add("signature", signature);

                    var requestJson = JsonConvert.SerializeObject(new
                    {
                        orderId = orderid
                    });

                    HttpContent httpContent = new StringContent(requestJson);

                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");


                    var result = client.PostAsync(String.Format("http://test.yiroaming.com:8080/pbs/api/cancel"), httpContent).Result.Content.ReadAsStringAsync().Result;

                    ResultJson resultJson = JsonConvert.DeserializeObject<ResultJson>(result);

                    return resultJson;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
        }
    }
    public class ResultJson
    {
        public string errorCode { get; set; }

        public string msg { get; set; }

        public string tradeld { get; set; }

        public string orderid { get; set; }

        public string data { get; set; }
    }
}
