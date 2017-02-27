using Newtonsoft.Json;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.MP.Helpers;
using Senparc.Weixin.MP.TenPayLibV3;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Unitoys.Core;
using Unitoys.IServices;
using Unitoys.Model;
using Unitoys.WebApi.Controllers;
using WxPayAPI;
using WxPayAPI.Jssdk;

namespace Unitoys.Web.Areas.wx.Controllers
{
    [AllowAnonymous]
    public class UserInfoController : Controller
    {

        private IUserService _userService;
        private IOrderService _orderService;
        private IUserBillService _userBillService;
        private IPaymentService _paymentService;
        public UserInfoController(IUserService userService, IOrderService orderService, IUserBillService userBillService, IPaymentService paymentService)
        {
            this._userService = userService;
            this._orderService = orderService;
            this._userBillService = userBillService;
            this._paymentService = paymentService;
        }
        public async Task<ActionResult> toXh()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("UserAgent", "Mozilla/5.0 (Linux; Android 5.1; M571C Build/LMY47D) : AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/37.0.0.0 : Mobile MQQBrowser/6.8 TBS/036869 Safari/537.36 QQLiveBrowser/5.1.2.11019;: ");
            client.DefaultRequestHeaders.Host = "market.m.qq.com";
            client.DefaultRequestHeaders.Referrer = new Uri("http://3gimg.qq.com/webapp_scan/activity/newcardvideo/build/index.htm");

            int imei = new Random().Next(100000000, 999999999);
            int imsi = new Random().Next(100000000, 999999999);

            string result = await client.GetStringAsync("http://market.m.qq.com/flow/order.do?method=getKey&imei=99000" + imei + "&imsi=460030" + imsi + "&product=4&channel=2&");

            ResultJsonDwk resultJson = JsonConvert.DeserializeObject<ResultJsonDwk>(result);

            string url = String.Format("http://m.10010.com/mall-mobile/kingNumCard/init?tencentId={0}&key={1}&product=0&channel=5#applyInfo0", resultJson.info.uid, resultJson.info.ukey);
            return Redirect(url);
        }

        /// <summary>
        /// 个人中心首页
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Index()
        {
            //var result = AccessTokenContainer.TryGetAccessToken(appId, appSecret);
            var openid = WebHelper.GetCookie("openid");

            if (openid == "") return Redirect("/wx/oauth2");

            UT_Users user = await _userService.GetEntityByOpenIdAsync(openid);

            ViewBag.Amount = user.Amount;
            ViewBag.Tel = user.Tel;
            ViewBag.NickName = user.NickName;
            ViewBag.UserHead = user.UserHead.GetUserHeadCompleteUrl();

            return View();
        }

        /// <summary>
        /// 我的套餐
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> MyOrder()
        {
            var openid = WebHelper.GetCookie("openid");

            if (openid == "") return Redirect("/wx/oauth2");

            UT_Users user = await _userService.GetEntityByOpenIdAsync(openid);



            //如果查询条件不为空，则根据查询条件查询，反则查询所有订单。
            var searchOrders = await _orderService.GetUserOrderList(1, 100, user.ID, PayStatusType.YesPayment, null);

            IEnumerable<RazorUserOrder> result = from i in searchOrders.Value
                                                 select new RazorUserOrder
                            {
                                OrderId = i.ID,
                                OrderNum = i.OrderNum,
                                UserId = i.UserId,
                                PackageName = i.PackageName,
                                TotalPrice = i.TotalPrice.ToString(),
                                ExpireDays = GetExpireDaysDescr(i),
                                OrderStatus = (int)i.OrderStatus + "",
                                LogoPic = i.UT_Package.UT_Country.LogoPic.GetPackageCompleteUrl()
                            };
            return View(result);
        }
        /// <summary>
        /// 套餐详情
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> MyOrderDetail(Guid orderId)
        {
            var openid = WebHelper.GetCookie("openid");
            if (openid == "") return Redirect("/wx/oauth2");

            UT_Users user = await _userService.GetEntityByOpenIdAsync(openid);

            var packageResult = await _orderService.GetEntityAndPackageByIdAsync(orderId);

            if (packageResult == null)
            {
                return Content("订单不存在！");
            }
            else if (packageResult.UserId != user.ID)
            {
                return Content("订单不属于此用户！");
            }

            ViewBag.LastCanActivationDate = CommonHelper.GetTime(packageResult.OrderDate.ToString()).AddMonths(6).ToString("yyyy年MM月dd日");
            ViewBag.LogoPic = packageResult.UT_Package.UT_Country.LogoPic.GetPackageCompleteUrl();
            ViewBag.OrderStatus = packageResult.OrderStatus.GetDescription();
            ViewBag.ExpireDays = GetExpireDaysDescr(packageResult);
            ViewBag.PaymentMethod = packageResult.PaymentMethod.GetDescription();

            ViewBag.OrderDate = CommonHelper.GetTime(packageResult.OrderDate.ToString()).ToString("yyyy-MM-dd HH:mm");


            return View(packageResult);
        }
        /// <summary>
        /// 余额明细
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> BillList()
        {
            var openid = WebHelper.GetCookie("openid");
            if (openid == "") return Redirect("/wx/oauth2");
            UT_Users user = await _userService.GetEntityByOpenIdAsync(openid);

            var result = await _userBillService.GetEntitiesForPagingAsync(1, 100, x => new { x.CreateDate }, "desc", a => a.UserId == user.ID);

            return View(result);
        }

        /// <summary>
        /// 获取订单有效天数的描述
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private string GetExpireDaysDescr(UT_Order i)
        {
            if (i.OrderStatus == OrderStatusType.Unactivated)
            {
                return "有效天数：" + (i.ExpireDays * i.Quantity).ToString() + "天";
            }
            else if (i.OrderStatus == OrderStatusType.Cancel)
            {
                return "订单已取消";
            }
            else if (i.OrderStatus == OrderStatusType.Used || i.OrderStatus == OrderStatusType.UnactivatError)
            {
                return string.Format("有效期：{0} {1}", CommonHelper.GetTime(i.EffectiveDate.Value.ToString()).ToString("yyyy-MM-dd"), (i.ExpireDays * i.Quantity).ToString() + "天");
            }
            else if (i.OrderStatus == OrderStatusType.HasExpired)
            {
                return "订单已过期";
            }
            return i.OrderStatus == OrderStatusType.UnactivatError
                                         ? (i.ExpireDays * i.Quantity).ToString()
                                         : CommonHelper.GetTime(i.EffectiveDate.Value.ToString()) + " " + (i.ExpireDays * i.Quantity).ToString();
        }
    }
    public class RazorUserOrder
    {
        public Guid OrderId { get; set; }
        public string OrderNum { get; set; }
        public Guid UserId { get; set; }
        public string PackageName { get; set; }
        public string TotalPrice { get; set; }
        public string ExpireDays { get; set; }
        public string OrderStatus { get; set; }
        public string LogoPic { get; set; }
    }
}