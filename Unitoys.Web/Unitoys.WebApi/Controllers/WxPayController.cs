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
using Unitoys.Core.Util;
using Unitoys.IServices;
using Unitoys.Model;
using Unitoys.WebApi.Models;
using WxPayAPI;
using WxPayAPI.AppPay;

namespace Unitoys.WebApi.Controllers
{
    public class WxPayController : ApiController
    {
        private IOrderService _orderService;
        private IPaymentService _paymentService;
        private IOrderByZCSelectionNumberService _orderByZCSelectionNumberService;
        public WxPayController(IOrderService orderService, IPaymentService paymentService, IOrderByZCSelectionNumberService orderByZCSelectionNumberService)
        {
            this._orderService = orderService;
            this._paymentService = paymentService;
            this._orderByZCSelectionNumberService = orderByZCSelectionNumberService;
        }

        /**
         * 生成直接支付url，支付url有效期为2小时,模式二
         * @param productId 商品ID
         * @return 模式二URL
         */
        [HttpGet]
        public string GetPayUrl(string productId)
        {
            Log.Info(this.GetType().ToString(), "Native pay mode 2 url is producing...");

            WxPayData data = new WxPayData();
            data.SetValue("body", "test");//商品描述
            data.SetValue("attach", "test");//附加数据
            data.SetValue("out_trade_no", WxPayApi.GenerateOutTradeNo());//随机字符串
            data.SetValue("total_fee", 1);//总金额
            data.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss"));//交易起始时间
            data.SetValue("time_expire", DateTime.Now.AddMinutes(10).ToString("yyyyMMddHHmmss"));//交易结束时间
            data.SetValue("goods_tag", "jjj");//商品标记
            data.SetValue("trade_type", "NATIVE");//交易类型
            data.SetValue("product_id", productId);//商品ID

            WxPayData result = WxPayApi.UnifiedOrder(data);//调用统一下单接口
            string url = result.GetValue("code_url").ToString();//获得统一下单接口返回的二维码链接

            Log.Info(this.GetType().ToString(), "Get native pay mode 2 url : " + url);
            return url;
        }

        /// <summary>
        /// 生成预支付ID，微信生成的预支付回话标识，用于后续接口调用中使用，该值有效期为2小时
        /// </summary>
        /// <param name="out_trade_no">商户系统内部的订单号,32个字符内、可包含字母</param>
        /// <param name="totalPrice">订单总金额，单位为分，不能带小数点</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> GetPayId([FromBody]WxPayGetPayIdBindingModel model)
        {
            Log.Info(this.GetType().ToString(), "Native pay mode 2 url is producing...");
            var currentUser = WebUtil.GetApiUserSession();

            if (string.IsNullOrEmpty(model.orderOrPayment))
            {
                return Ok(new { status = 0, msg = "参数不允许为空！" });
            }

            string out_trade_no = model.orderOrPayment;
            decimal TotalPrice = 0;


            WxPayData data = new WxPayData();

            //1.获取订单信息
            if (model.orderOrPayment.StartsWith("8022", StringComparison.OrdinalIgnoreCase))
            {
                UT_Order order = await _orderService.GetEntityAsync(x => x.OrderNum == out_trade_no);

                //2.验证当前TOKEN是否为订单用户
                if (order == null || currentUser == null || !currentUser.ID.Equals(order.UserId))
                {
                    return Ok(new { status = 0, msg = "调用信息错误！" });
                }
                TotalPrice = order.TotalPrice;

                data.SetValue("body", "爱小器-购买套餐");//商品描述
            }
            else if (model.orderOrPayment.StartsWith("9022", StringComparison.OrdinalIgnoreCase))
            {
                UT_Payment payment = await _paymentService.GetEntityAsync(x => x.PaymentNum == out_trade_no);

                //2.验证当前TOKEN是否为订单用户
                if (payment == null || currentUser == null || !currentUser.ID.Equals(payment.UserId))
                {
                    return Ok(new { status = 0, msg = "调用信息错误！" });
                }
                TotalPrice = payment.Amount;

                data.SetValue("body", "爱小器-账户充值");//商品描述
            }
            else if (model.orderOrPayment.StartsWith("1022", StringComparison.OrdinalIgnoreCase))
            {
                UT_OrderByZCSelectionNumber orderByZCSelectionNumber = await _orderByZCSelectionNumberService.GetEntityAndOrderByZCAsync(x => x.OrderByZCSelectionNumberNum == out_trade_no);

                //2.验证当前TOKEN是否为订单用户
                if (orderByZCSelectionNumber == null || currentUser == null || !currentUser.ID.Equals(orderByZCSelectionNumber.UT_OrderByZC.UserId))
                {
                    return Ok(new { status = 0, msg = "调用信息错误！" });
                }
                TotalPrice = orderByZCSelectionNumber.TotalPrice;

                data.SetValue("body", "爱小器-众筹订单选号");//商品描述
            }

            //3.调用微信支付,传递相关订单信息


            int totalFee = 0;
            if (!Int32.TryParse((TotalPrice * 100 + "").Replace(".00", ""), out totalFee) || totalFee == 0)
            {
                return Ok(new
             {
                 status = 0,
                 msg = "错误的订单金额"
             });
            }
            data.SetValue("attach", "test");//附加数据
            data.SetValue("out_trade_no", out_trade_no);//随机字符串
            data.SetValue("fee_type", "CNY");//货币类型
            data.SetValue("total_fee", totalFee);//总金额,单位为分
            data.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss"));//交易起始时间
            data.SetValue("time_expire", DateTime.Now.AddMinutes(10).ToString("yyyyMMddHHmmss"));//交易结束时间
            //data.SetValue("goods_tag", "jjj");//商品标记,(商品标记，代金券或立减优惠功能的参数)
            data.SetValue("trade_type", "APP");//交易类型
            //data.SetValue("product_id", productId);//商品ID

            WxPayData result = WxPayApi.UnifiedOrder(data);//调用统一下单接口
            string prepay_id = result.GetValue("prepay_id").ToString();//获得统一下单接口返回预支付回话标识


            /*
             微信支付开发文档原内容：
             3、调起支付
商户服务器生成支付订单，先调用统一下单API(详见第7节)生成预付单，获取到prepay_id后将参数再次签名传输给APP发起支付。以下是调起微信支付的关键代码：
             * 重点1：获取到prepay_id后将参数再次签名传输给APP
             * 重点2：返回nonce_str为当前使用的随机数，
             * 返回package=Sign=WXPay
             * 如果失败则尝试：package=prepay_id={0}
             */
            Log.Info(this.GetType().ToString(), "Get App pay prepay_id : " + prepay_id);

            //4.返回App（调用支付）的参数
            WxPayData dataSign_again = new WxPayData();
            dataSign_again.SetValue("appid", data.GetValue("appid"));
            dataSign_again.SetValue("noncestr", data.GetValue("nonce_str"));
            dataSign_again.SetValue("package", "Sign=WXPay");
            dataSign_again.SetValue("partnerid", data.GetValue("mch_id"));
            dataSign_again.SetValue("timestamp", WxPayApi.GenerateTimeStamp());
            dataSign_again.SetValue("prepayid", prepay_id);
            //签名
            data.SetValue("sign", dataSign_again.MakeSign());

            return Ok(new
            {
                status = 1,
                data = new
                {
                    appid = dataSign_again.GetValue("appid"),
                    noncestr = dataSign_again.GetValue("noncestr"),
                    package = dataSign_again.GetValue("package"),
                    partnerid = dataSign_again.GetValue("partnerid"),
                    timestamp = dataSign_again.GetValue("timestamp"),
                    prepayid = dataSign_again.GetValue("prepayid"),
                    sign = data.GetValue("sign")
                }
            });
            //return prepay_id;


        }

        /// <summary>
        /// 微信支付回调
        /// </summary>
        /// <returns></returns>
        [NoAuthenticate]
        public async Task<string> NotifyAsync()
        {
            LoggerHelper.Info("进入APP支付回调");

            ResultNotify rn = new ResultNotify();
            //处理通知
            WxPayData notifyData = null;
            var PayDataResult = rn.ProcessNotify(out notifyData);
            LoggerHelper.Info("处理通知");
            LoggerHelper.Info("Content：" + Request.Content.ReadAsStringAsync().Result);
            try
            {
                LoggerHelper.Info("PayDataResult：json，" + PayDataResult.ToJson());
                //激活订单
                if (PayDataResult.GetValue("return_code").ToString().Equals("SUCCESS"))
                {
                    LoggerHelper.Info("激活订单处理中");
                    //1.获取订单号
                    string orderOrPayment = notifyData.GetValue("out_trade_no").ToString();

                    //订单金额（单位：分）
                    decimal totalFee = 0;
                    decimal.TryParse(notifyData.GetValue("total_fee").ToString(), out totalFee);

                    LoggerHelper.Info("orderOrPayment:" + orderOrPayment);
                    LoggerHelper.Info("totalFee/100:" + totalFee / 100);

                    //根据orderNum获取Order对象。

                    //处理订单完成支付。
                    if (orderOrPayment.StartsWith("8022", StringComparison.OrdinalIgnoreCase))
                    {
                        //2.判断订单是否已经处理(已支付)
                        UT_Order order = await _orderService.GetEntityAsync(a => a.OrderNum == orderOrPayment);
                        if (order == null)
                        {
                            PayDataResult.SetValue("return_code", "FAIL");
                            PayDataResult.SetValue("return_msg", "订单号不存在");
                        }
                        if (order.PayStatus == PayStatusType.YesPayment)
                        {
                            PayDataResult.SetValue("return_code", "FAIL");
                            PayDataResult.SetValue("return_msg", "订单已支付");
                        }
                        //3.微信支付成功
                        //处理订单完成。
                        if (await _orderService.OnAfterOrderSuccess(orderOrPayment, totalFee / 100))
                        {
                            //result_msg = "success";
                        }
                    }
                    else if (orderOrPayment.StartsWith("9022", StringComparison.OrdinalIgnoreCase))
                    {
                        UT_Payment payment = await _paymentService.GetEntityAsync(a => a.PaymentNum == orderOrPayment);
                        if (payment == null)
                        {
                            PayDataResult.SetValue("return_code", "FAIL");
                            PayDataResult.SetValue("return_msg", "订单号不存在");
                        }
                        if (payment.Status == 1)
                        {
                            PayDataResult.SetValue("return_code", "FAIL");
                            PayDataResult.SetValue("return_msg", "订单已支付");
                        }
                        //处理付款完成。
                        if (await _paymentService.OnAfterPaymentSuccess(orderOrPayment, totalFee / 100))
                        {
                            //result_msg = "success";
                        }
                    }

                    else if (orderOrPayment.StartsWith("1022", StringComparison.OrdinalIgnoreCase))
                    {
                        //2.判断订单是否已经处理(已支付)
                        UT_OrderByZCSelectionNumber order = await _orderByZCSelectionNumberService.GetEntityAsync(a => a.OrderByZCSelectionNumberNum == orderOrPayment);
                        if (order == null)
                        {
                            PayDataResult.SetValue("return_code", "FAIL");
                            PayDataResult.SetValue("return_msg", "订单号不存在");
                        }
                        if (order.PayStatus == PayStatusType.YesPayment)
                        {
                            PayDataResult.SetValue("return_code", "FAIL");
                            PayDataResult.SetValue("return_msg", "订单已支付");
                        }
                        //3.微信支付成功
                        //处理订单完成。
                        if (await _orderByZCSelectionNumberService.OnAfterOrderSuccess(orderOrPayment, totalFee / 100))
                        {

                        }
                    }
                }
                LoggerHelper.Info("try end");
            }
            catch (Exception ex)
            {
                LoggerHelper.Info("ex返回响应" + ex.Message);
                throw;
            }
            LoggerHelper.Info("返回响应");
            //返回响应
            return PayDataResult.ToXml();

        }
    }
    public class WxPayGetPayIdBindingModel
    {
        public string orderOrPayment { get; set; }
    }

}
