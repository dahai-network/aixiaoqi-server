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
using Unitoys.Core;

namespace Unitoys.ESIM_263mobile
{
    public class MVNOServiceApi
    {

        public static readonly MVNOServiceApi Instance = new MVNOServiceApi();

        private const string publicKey = @"MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDWCx0XF37fjlBnXt4eTbWYAUNvyKyutFcoT1axo1gts8EdsnNJaDOiFblejdR5/0Y2MuYergxapISnOHlGAGs6mkgvChVyI4ZWrj29SwIBgmsLx3ekJ/xiq2LKJuG7brL+AIWYUJM+28nXTfFAzylCFNkQoIH8iXyawxlqQ09h8QIDAQAB";

        private const string privateKey = @"MIICXQIBAAKBgQDWCx0XF37fjlBnXt4eTbWYAUNvyKyutFcoT1axo1gts8EdsnNJaDOiFblejdR5/0Y2MuYergxapISnOHlGAGs6mkgvChVyI4ZWrj29SwIBgmsLx3ekJ/xiq2LKJuG7brL+AIWYUJM+28nXTfFAzylCFNkQoIH8iXyawxlqQ09h8QIDAQABAoGBANT8OBUrRw8zMKS7zLBAyUsZLr6D4/jv8K5mzDB1BqBrduWTFY7dBkvp7Au/e8dtkbMK0NuEezyS6oDu/BYBArB52IBaRnTGFljy8XE2lUKVXfkuTqIHdJU5iht7bwvtB3TTNrqNwcBxpsOKeM9irKFFf7v0wyhJHOSZetq/yPqlAkEA+X5VWbHP7qXipP+K1pyYLqSR7zg2fYh8AaB34+VCXXsvs3arXNr7Lhg2sd8bw2vA5F9ywKJk3yk0YRSNVW56qwJBANugG9MOkNNnWTMXm5wL905I/fishvtOTYtxvZHWbrYTbYxQLoT2NqPW/I3X16CTRvbh8g2WJ/Yd2RpkCMeQ1dMCQC+YEAQaTYZDEudS8FNccBOFxWkTGiH8ZVuSFwzccTqQA1uC6dG+3GfAqr5nx04SQivOoX9p+0AvBhT27Lc9ah0CQDR0sFCfqP2lMIvgdp01ynKbQnWzl2XMlP7aQsHjanv4dfDOcd32BKTrQ1UJmYnTw15SJwMRuyewh1sjS2mG8VsCQQDDWV06wA8ABzsrMdlb4C4f1a3lyRbRdVpvoHWX+h+LyFHx1B6mNYLy69JlBOB3GzgDd6FxcWE3mZ9akQIo6Eo9";

        private const string channelKey = "65629D18AB54D2D3D2ADD4B377ABD0E2";
        private const string token = "FD0D002E91CB2B54120D6C6323C8CB55";

        private RSACryptoService rsa;
        public MVNOServiceApi()
        {
            rsa = new RSACryptoService(privateKey, publicKey);
        }
        /// <summary>
        /// 获取产品列表
        /// </summary>
        /// <param name="productId">产品唯一编号（当为空时查所有产品）</param>
        /// <returns></returns>
        public async Task<QueryProductList> QueryProductList(string productId = "")
        {
            var requestJson = JsonConvert.SerializeObject(new
            {
                token = token,
                productId = productId
            });

            return await HttpPost<QueryProductList>("/MVNO_BSS/channel/queryProductList", requestJson);
        }

        /// <summary>
        /// 订购产品
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="productId">产品编号</param>
        /// <param name="beginTime">服务生效时间、激活时间</param>
        /// <param name="quantity">订购数量天数，默认1天</param>
        /// <param name="activeType">激活类型，默认0  （1-白卡实时；2-白卡预约；3-成卡实时；4-成卡预约；0-默认激活）</param>
        /// <returns></returns>
        public async Task<BuyProduct> BuyProduct(string userId, string productId, string beginTime, int quantity = 1, int activeType = 0)
        {
            var requestJson = JsonConvert.SerializeObject(new
            {
                token = token,
                userId = userId,
                productId = productId,
                quantity = quantity,
                beginTime = beginTime,
                rsaPublic = publicKey,
                activeType = activeType
            });
            BuyProduct model = await HttpPost<BuyProduct>("/MVNO_BSS/channel/buyProduct", requestJson);
            //解密KI
            model.imsiResource.ki = rsa.Decrypt(model.imsiResource.ki);
            return model;
        }

        /// <summary>
        /// 余量查询
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="orderId">订单编号</param>
        /// <returns></returns>
        public async Task<Remain> GetRemain(string userId, string orderId)
        {
            var requestJson = JsonConvert.SerializeObject(new
            {
                token = token,
                userId = userId,
                orderId = orderId
            });
            Remain result = await HttpPost<Remain>("/MVNO_BSS/channel/getRemain", requestJson);
            return result;
        }

        /// <summary>
        /// 主动通知登网
        /// 登网通知
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="orderId">订单编号</param>
        /// <returns></returns>
        public async Task<NotifyAccess> NotifyAccess(string userId, string orderId)
        {
            var requestJson = JsonConvert.SerializeObject(new
            {
                token = token,
                userId = userId,
                orderId = orderId
            });
            NotifyAccess result = await HttpPost<NotifyAccess>("/MVNO_BSS/channel/notifyAccess", requestJson);
            return result;
        }

        /// <summary>
        /// 退订
        /// 产品退订
        /// (1)订单已失效不可退订；
        /// (2)用户已登网不可退订。
        /// </summary>
        /// <param name="orderId">订单编号</param>
        /// <returns></returns>
        public async Task<BasicResponse> ReturnProduct(string orderId)
        {
            var requestJson = JsonConvert.SerializeObject(new
            {
                token = token,
                orderId = orderId
            });
            BasicResponse result = await HttpPost<BasicResponse>("/MVNO_BSS/channel/returnProduct", requestJson);
            return result;
        }

        /// <summary>
        /// 续订
        /// 产品续订
        /// 续定是针对包天产品在原有生命周期基础上多续一个或多个产品周期，续订部分的流量仅下一周期才可用。续订会产生一笔新订单
        /// </summary>
        /// <param name="userId">用户唯一标识</param>
        /// <param name="orderId">订单编号</param>
        /// <param name="quantity">续订数量</param>
        /// <returns></returns>
        public async Task<ReactiveProduct> ReactiveProduct(string userId, string orderId, int quantity)
        {
            var requestJson = JsonConvert.SerializeObject(new
            {
                token = token,
                userId = userId,
                orderId = orderId,
                quantity = quantity
            });
            ReactiveProduct result = await HttpPost<ReactiveProduct>("/MVNO_BSS/channel/reactiveProduct", requestJson);
            return result;
        }

        /// <summary>
        /// 误删重新获取ISMI卡信息接口
        /// IMSI下发
        /// </summary>
        /// <param name="orderId">订单编号</param>
        /// <returns></returns>
        public async Task<GetIMSI> GetIMSI(string orderId)
        {
            var requestJson = JsonConvert.SerializeObject(new
            {
                token = token,
                orderId = orderId,
                rsaPublic = publicKey,
            });
            GetIMSI result = await HttpPost<GetIMSI>("/MVNO_BSS/channel/getIMSI", requestJson);
            //解密KI
            result.imsiResource.ki = rsa.Decrypt(result.imsiResource.ki);
            return result;
        }

        /// <summary>
        /// 订单查询
        /// </summary>
        /// <param name="userId">用户唯一标识</param>
        /// <param name="orderId">订单编号</param>
        /// <returns></returns>
        public async Task<QueryOrder> QueryOrder(string userId, string orderId)
        {
            var requestJson = JsonConvert.SerializeObject(new
            {
                token = token,
                userId = userId,
                orderId = orderId,
            });
            QueryOrder result = await HttpPost<QueryOrder>("/MVNO_BSS/channel/queryOrder", requestJson);
            return result;
        }

        /// <summary>
        /// 消息订阅
        /// 3，	消息订阅属于低频次事件，每次新的订阅都是下月生效；
        /// 4，	263向对方推送时，对方需要接收处理并做应答，接收成功返回success,失败返回fail；
        /// 5，	如果对方应答不是成功或超时（超过20秒视为超时），263认为推送失败并重发（最多3次，第二次重发3分钟后，第三次重发10分钟后）尽可能提高通知的成功率，但不保证通知最终能成功。 
        /// </summary>
        /// <param name="msgReceiverList">订阅信息</param>
        /// <returns></returns>
        public async Task<BasicResponse> SubscribeMsg(MsgReceiver[] msgReceiverList)
        {
            var requestJson = JsonConvert.SerializeObject(new
            {
                token = token,
                msgReceiverList = msgReceiverList,
            });
            BasicResponse result = await HttpPost<BasicResponse>("/MVNO_BSS/channel/subscribeMsg", requestJson);
            return result;
        }

        /// <summary>
        /// 消息退订
        /// </summary>
        /// <param name="msgReceiverList">订阅信息</param>
        /// <returns></returns>
        public async Task<BasicResponse> CancelMsg(MsgReceiver[] msgReceiverList)
        {
            var requestJson = JsonConvert.SerializeObject(new
            {
                token = token,
                msgReceiverList = msgReceiverList,
            });
            BasicResponse result = await HttpPost<BasicResponse>("/MVNO_BSS/channel/cancelMsg", requestJson);
            return result;
        }

        /// <summary>
        /// 退款
        /// 1,退款接口只针对包天产品；
        /// 2,每笔订单只可退款一次；
        /// 3,已经出过账单的不能退款
        /// </summary>
        /// <param name="msgReceiverList">订阅信息</param>
        /// <returns></returns>
        public async Task<BasicResponse> RefundOrder(string userId, string orderId, string refundDate)
        {
            var requestJson = JsonConvert.SerializeObject(new
            {
                token = token,
                userId = userId,
                orderId = orderId,
                refundDate = refundDate,
            });
            BasicResponse result = await HttpPost<BasicResponse>("/MVNO_BSS/channel/refundOrder", requestJson);
            return result;
        }

        public static async Task<T> HttpPost<T>(string url, string requestJson)
        {
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Add("checkSum", GetSHA256HashFromString(requestJson + channelKey));

            HttpContent httpContent = new StringContent(requestJson);

            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var responseMsg = await client.PostAsync(String.Format("http://roam.263mobile.cn:28080" + url), httpContent);

            if (responseMsg.IsSuccessStatusCode)
            {
                T model = JsonConvert.DeserializeObject<T>(await responseMsg.Content.ReadAsStringAsync());
                return model;
            }
            string msg = await responseMsg.Content.ReadAsStringAsync();
            throw new Exception("Http响应失败,响应内容如下" + msg);
        }

        public static string GetSHA256HashFromString(string strData)
        {
            byte[] bytValue = System.Text.Encoding.UTF8.GetBytes(strData);
            try
            {
                SHA256 sha256 = new SHA256CryptoServiceProvider();
                byte[] retVal = sha256.ComputeHash(bytValue);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetSHA256HashFromString() fail,error:" + ex.Message);
            }
        }


    }
}
