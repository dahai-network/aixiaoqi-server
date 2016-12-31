using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Core;
using Unitoys.ESIM_MVNO.Model;

namespace Unitoys.ESIM_MVNO
{
    public class MVNOServiceApi
    {
        private static string partner = UTConfig.SiteConfig.MVNOpartner;
        private static string partner_key = UTConfig.SiteConfig.MVNOPartnerKey;
        private static string host = UTConfig.SiteConfig.MVNOHost;

        /// <summary>
        /// 订购产品
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="productId">产品编号</param>
        /// <param name="beginTime">服务生效时间、激活时间</param>
        /// <param name="quantity">订购数量天数，默认1天</param>
        /// <returns></returns>
        public async Task<ResponseModel<BuyProduct>> BuyProduct(string userId, string productId, string beginTime, int quantity = 1)
        {
            var requestJson = JsonConvert.SerializeObject(new
            {
                userId = userId,
                productId = productId,
                quantity = quantity,
                beginTime = beginTime,
                //rsaPublic = publicKey,
                //activeType = activeType
            });
            var model = await HttpPost<ResponseModel<BuyProduct>>("/api/channel/buyProduct", requestJson);

            return model;
        }

        /// <summary>
        /// 查询订单
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="productId">产品编号</param>
        /// <param name="beginTime">服务生效时间、激活时间</param>
        /// <param name="quantity">订购数量天数，默认1天</param>
        /// <returns></returns>
        public async Task<ResponseModel<BuyProduct>> QueryOrder(string orderid)
        {
            var requestJson = JsonConvert.SerializeObject(new
            {
                OrderId = orderid,
                //activeType = activeType
            });
            var model = await HttpPost<ResponseModel<BuyProduct>>("/api/channel/QueryOrder", requestJson);

            return model;
        }
        public static async Task<T> HttpPost<T>(string url, string requestJson)
        {
            HttpClient client = new HttpClient();

            int time = CommonHelper.GetDateTimeInt();

            client.DefaultRequestHeaders.Add("partner", partner);
            client.DefaultRequestHeaders.Add("time", time + "");
            client.DefaultRequestHeaders.Add("sign", SecureHelper.MD5(partner + time + partner_key));
            HttpContent httpContent = new StringContent(requestJson);

            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var responseMsg = await client.PostAsync(String.Format(host + url), httpContent);

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
