using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Unitoys.Core;
using Unitoys.Core.AliPay;
using Unitoys.IServices;
using Unitoys.Model;
using Unitoys.WebApi.Models;

namespace Unitoys.WebApi.Controllers
{
    public class AliPayController : ApiController
    {
        private IOrderService _orderService;
        private IPaymentService _paymentService;
        public AliPayController(IOrderService orderService, IPaymentService paymentService)
        {
            this._orderService = orderService;
            this._paymentService = paymentService;
        }

        /// <summary>
        /// 支付宝支付成功异步通知
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        [HttpPost]
        //[HttpGet]
        [NoAuthenticate]
        public async Task<HttpResponseMessage> NotifyAsync()
        {
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context

            NameValueCollection getCollection = context.Request.Form; //此签名要求Partner及Sign均通过QueryString传递
            //1. 获取Post参数。
            SortedDictionary<string, string> sPara = GetRequestPost(getCollection);

            //写日志记录（若要调试，请取消下面两行注释）
            string sWord = "sign_type:" + getCollection["sign_type"] + ",sign:" + getCollection["sign"] + "\n ";
            foreach (var item in getCollection)
            {
                sWord += item + ":" + getCollection[item + ""] + "\n";
            }
            Unitoys.Core.AliPay.Core.LogResult(sWord);

            var result_msg = "fail";

            if (sPara.Count > 0)
            {
                Notify aliNotify = new Notify();

                //2. 验证
                bool verifyResult = aliNotify.Verify(sPara, getCollection["notify_id"], getCollection["sign"]);

                if (verifyResult)
                {
                    //交易状态
                    string trade_status = getCollection["trade_status"];
                    //3. 如果验证成功，则判断该交易状态。
                    if (trade_status == "TRADE_SUCCESS")
                    {
                        //3.1 付款完成后。

                        //获取商户订单号，有可能为Order或者Payment，先过滤前缀判断。
                        string orderOrPayment = getCollection["out_trade_no"];

                        //订单金额（单位：分）
                        decimal total_amount = Convert.ToDecimal(
                           getCollection["total_amount"]);

                        if (orderOrPayment.StartsWith("8022", StringComparison.OrdinalIgnoreCase))
                        {
                            //处理订单完成。
                            if (await _orderService.OnAfterOrderSuccess(orderOrPayment, total_amount))
                            {
                                result_msg = "success";
                            }
                        }
                        else if (orderOrPayment.StartsWith("9022", StringComparison.OrdinalIgnoreCase))
                        {
                            //处理付款完成。
                            if (await _paymentService.OnAfterPaymentSuccess(orderOrPayment, total_amount))
                            {
                                result_msg = "success";
                            }
                        }
                    }
                    else if (trade_status == "WAIT_BUYER_PAY")
                    {
                        result_msg = "success";
                    }

                }
                else
                {
                    LoggerHelper.Error("支付验证信息不通过" + sWord);
                }
            }
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            resp.Content = new StringContent(result_msg, Encoding.UTF8, "text/plain");
            return resp;
        }

        /// <summary>
        /// 获取支付宝POST过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        private SortedDictionary<string, string> GetRequestPost(NameValueCollection getCollection)
        {
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();


            String[] requestItem = getCollection.AllKeys;

            for (int i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], getCollection[requestItem[i]]);
            }

            return sArray;
        }
    }
}
