using Jayrock.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Unitoys.Core;

namespace Unitoys.WebApi.Controllers.Util
{
    public class ESIMController:ApiController
    {

        private readonly string _appid = "mcbed8gi0a4u3sv1";

        private readonly string _secret = "4d0804gxzybzfp5z";

        [HttpGet]
        [NoAuthenticate]
        public IHttpActionResult GetProductList([FromUri]string code)
        {
            using(HttpClient client = new HttpClient())
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

                    var result = client.GetStringAsync(String.Format("http://test.yiroaming.com:8080/pbs/api/getProductList?code={0}", code)).Result;

                    return Ok(result);
                }
                catch
                {
                    return Ok(new
                    {
                        status = 0,
                        msg = "error"
                    });
                }
                
            }            
        }
        [HttpGet]
        [NoAuthenticate]
        [NonAction]
        public IHttpActionResult BuyProduct()
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
                        productId = "DH10001",
                        phone = "18850161016",
                        tradeId = "201607211532",
                        beginTime = CommonHelper.GetDateTime("yyyyMMddHHmm"),
                        quantity = "1"
                    });

                    HttpContent httpContent = new StringContent(requestJson);

                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");


                    var result = client.PostAsync(String.Format("http://test.yiroaming.com:8080/pbs/api/buyProduct"), httpContent).Result.Content.ReadAsStringAsync().Result;

                    ResultJson resultJson = JsonConvert.DeserializeObject<ResultJson>(result);

                    return Ok(result);
                }
                catch
                {
                    return Ok(new
                    {
                        status = 0,
                        msg = "error"
                    });
                }

            }
        }
        [HttpGet]
        [NoAuthenticate]
        [NonAction]
        public IHttpActionResult cancel([FromUri]string orderid)
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

                    return Ok(result);
                }
                catch
                {
                    return Ok(new
                    {
                        status = 0,
                        msg = "error"
                    });
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
