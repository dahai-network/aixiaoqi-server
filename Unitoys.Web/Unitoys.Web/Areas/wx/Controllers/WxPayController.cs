using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.Helpers;
using Senparc.Weixin.MP.TenPayLibV3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Unitoys.Core;
using Unitoys.IServices;
using Unitoys.Model;
using WxPayAPI.Jssdk;

namespace Unitoys.Web.Areas.wx.Controllers
{
     [AllowAnonymous]
    public class WxPayController : Controller
    {

        private IOrderService _orderService;
        private IPaymentService _paymentService;
        private IUserService _userService;
        public WxPayController(IUserService userService, IOrderService orderService, IPaymentService paymentService)
        {
            this._orderService = orderService;
            this._paymentService = paymentService;
            this._userService = userService;
        }

        /// <summary>
        /// 充值页面
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Index()
        {
            var jssdkUiPackage = JSSDKHelper.GetJsSdkUiPackage(WxPayConfig.APPID, WxPayConfig.APPSECRET, Request.Url.AbsoluteUri);
            return View(jssdkUiPackage);
        }

        /// <summary>
        /// 创建充值余额的付款信息
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreatePayAmount(decimal Amount)
        {
            string errorMsg = "";

            var openid = WebHelper.GetCookie("openid");

            UT_Users user = await _userService.GetEntityByOpenIdAsync(openid);

            if (user == null)
            {
                errorMsg = "当前用户不能为空！";
            }
            else if (Amount <= 0)
            {
                errorMsg = "充值金额需要大于0元！";
            }
            else
            {
                var payMethodModel = await _paymentService.Add(user.ID, Amount, PaymentMethodType.WxPay);

                if (payMethodModel != null)
                {
                    string out_trade_no = payMethodModel.PaymentNum;

                    decimal TotalPrice = payMethodModel.Amount;

                    int totalFee = 0;
                    if (!Int32.TryParse((TotalPrice * 100 + "").Replace(".00", "").Replace(".0", ""), out totalFee) || totalFee == 0)
                    {
                        return Json(new
                        {
                            status = 0,
                            msg = "错误的订单金额"
                        });
                    }

                    string timeStamp = "";
                    string nonceStr = "";
                    string paySign = "";

                    string sp_billno = Request["order_no"];
                    //当前时间 yyyyMMdd
                    string date = DateTime.Now.ToString("yyyyMMdd");

                    if (null == sp_billno)
                    {
                        //生成订单10位序列号，此处用时间和随机数生成，商户根据自己调整，保证唯一
                        sp_billno = DateTime.Now.ToString("HHmmss") + TenPayV3Util.BuildRandomStr(28);
                    }
                    else
                    {
                        sp_billno = Request["order_no"].ToString();
                    }

                    //创建支付应答对象
                    RequestHandler packageReqHandler = new RequestHandler(null);
                    //初始化
                    packageReqHandler.Init();

                    timeStamp = TenPayV3Util.GetTimestamp();
                    nonceStr = TenPayV3Util.GetNoncestr();

                    //设置package订单参数
                    packageReqHandler.SetParameter("appid", WxPayConfig.APPID);		  //公众账号ID
                    packageReqHandler.SetParameter("mch_id", WxPayConfig.MCHID);		  //商户号
                    packageReqHandler.SetParameter("nonce_str", nonceStr);                    //随机字符串
                    packageReqHandler.SetParameter("body", "账号充值");    //商品信息
                    packageReqHandler.SetParameter("out_trade_no", out_trade_no);		//商家订单号
                    packageReqHandler.SetParameter("total_fee", totalFee.ToString());			        //商品金额,以分为单位(money * 100).ToString()
                    packageReqHandler.SetParameter("spbill_create_ip", Request.UserHostAddress);   //用户的公网ip，不是商户服务器IP
                    packageReqHandler.SetParameter("notify_url", UTConfig.SiteConfig.SiteHost + WxPayConfig.NOTIFY_URL);		    //接收财付通通知的URL
                    packageReqHandler.SetParameter("trade_type", "JSAPI");	                    //交易类型
                    packageReqHandler.SetParameter("openid", openid);	                    //用户的openId


                    string sign = packageReqHandler.CreateMd5Sign("key", WxPayConfig.KEY);
                    packageReqHandler.SetParameter("sign", sign);	                    //签名

                    string data = packageReqHandler.ParseXML();

                    var result = TenPayV3.Unifiedorder(data);
                    var res = XDocument.Parse(result);
                    if (res.Element("xml").Element("return_code").Value == "FAIL")
                    {
                        return Json(new
                        {
                            status = 0,
                            msg = "创建支付订单失败！"
                        });
                    }
                    string prepayId = res.Element("xml").Element("prepay_id").Value;

                    //设置支付参数
                    RequestHandler paySignReqHandler = new RequestHandler(null);
                    paySignReqHandler.SetParameter("appId", WxPayConfig.APPID);
                    paySignReqHandler.SetParameter("timeStamp", timeStamp);
                    paySignReqHandler.SetParameter("nonceStr", nonceStr);
                    paySignReqHandler.SetParameter("package", string.Format("prepay_id={0}", prepayId));
                    paySignReqHandler.SetParameter("signType", "MD5");
                    paySign = paySignReqHandler.CreateMd5Sign("key", WxPayConfig.KEY);


                    return Json(new
                    {
                        status = 1,
                        data = new
                        {
                            appid = WxPayConfig.APPID,
                            noncestr = nonceStr,
                            package = string.Format("prepay_id={0}", prepayId),
                            timestamp = timeStamp,
                            sign = paySign
                        }
                    });

                }
                else
                {
                    errorMsg = "充值订单添加失败！";
                }
            }

            return Json(new { status = 0, msg = errorMsg });
        }
        public ActionResult PaySuccess(decimal Amount)
        {
            ViewBag.Amount = Amount;
            return View();
        }


        /// <summary>
        /// 微信支付回调
        /// </summary>
        /// <returns></returns>
        public async Task<string> NotifyAsync()
        {
            //LoggerHelper.Info("进入公众号支付回调");

            ResultNotify rn = new ResultNotify();
            //处理通知
            WxPayData notifyData = null;
            var PayDataResult = rn.ProcessNotify(out notifyData);

            //LoggerHelper.Info("处理通知");
            //LoggerHelper.Info("Content：" + Request.InputStream);
            try
            {
                //LoggerHelper.Info("PayDataResult：json，" + PayDataResult.ToJson());
                //激活订单
                if (PayDataResult.GetValue("return_code").ToString().Equals("SUCCESS"))
                {
                    //LoggerHelper.Info("激活订单处理中");
                    //1.获取订单号
                    string orderOrPayment = notifyData.GetValue("out_trade_no").ToString();
                    //LoggerHelper.Info("orderOrPayment:" + orderOrPayment);

                    //订单金额（单位：分）
                    decimal totalFee = 0;
                    decimal.TryParse(notifyData.GetValue("total_fee").ToString(), out totalFee);

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
                }
               // LoggerHelper.Info("try end");
            }
            catch (Exception ex)
            {
               // LoggerHelper.Info("ex返回响应" + ex.Message);
                throw;
            }
           // LoggerHelper.Info("返回响应");
            //返回响应
            return PayDataResult.ToXml();

        }

    }
}