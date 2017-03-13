using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Unitoys.Core;
using Unitoys.IServices;
using Unitoys.Model;
using Unitoys.Web.Models;
using Unitoys.WebApi.Models;

namespace Unitoys.WebApi.Controllers
{
    public class ConfigController : ApiController
    {
        private IUserService _userService;
        private IBannerService _bannerService;
        public ConfigController(IUserService userService, IBannerService bannerService)
        {
            this._userService = userService;
            this._bannerService = bannerService;
        }
        /// <summary>
        /// app初始化获取配置信息
        /// </summary>
        /// <param name="authQueryint"></param>
        /// <returns></returns>
        [HttpGet]
        [NoLogin]
        public IHttpActionResult GetCommonConfig()
        {
            return Ok(new
            {
                status = 1,
                msg = "success",
                data = new
                {
                    notifyUrl = UTConfig.SiteConfig.NotifyUrl,
                    hotelUrl = UTConfig.SiteConfig.HotelUrl,
                    flightTicketUrl = UTConfig.SiteConfig.FlightTicketUrl,
                    apiUrl = UTConfig.SiteConfig.ApiUrl
                }
            });
        }

        /// <summary>
        /// 获取软电话注册配置
        /// </summary>
        /// <param name="authQueryint"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetSecurityConfig()
        {
            var currentUser = WebUtil.GetApiUserSession();
            return Ok(new
            {
                status = 1,
                msg = "success",
                data = new
                {
                    Out = new
                    {
                        //打出
                        AsteriskIp = UTConfig.SiteConfig.AsteriskOutIp,
                        AsteriskPort = UTConfig.SiteConfig.AsteriskOutPort,
                        PublicPassword = SecureHelper.MD5(UTConfig.SiteConfig.PublicKey + currentUser.Tel)
                    },
                    In = new
                    {
                        //打入
                        AsteriskIp = UTConfig.SiteConfig.AsteriskInIp,
                        AsteriskPort = UTConfig.SiteConfig.AsteriskInPort,
                    },
                    VswServer = new
                    {
                        //Tcp端连接地址
                        Ip = UTConfig.SiteConfig.VswServerIp,
                        Port = UTConfig.SiteConfig.VswServerPort,
                    }

                }
            });
        }

        [HttpGet]
        //[NoAuthenticate]
        /// <summary>
        /// 获取大王卡连接
        /// </summary>
        /// <returns></returns>
        public async Task<IHttpActionResult> getDWKUrl()
        {
            //HttpClient client = new HttpClient();
            //client.DefaultRequestHeaders.Add("UserAgent", "Mozilla/5.0 (Linux; Android 5.1; M571C Build/LMY47D) : AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/37.0.0.0 : Mobile MQQBrowser/6.8 TBS/036869 Safari/537.36 QQLiveBrowser/5.1.2.11019;: ");
            //client.DefaultRequestHeaders.Host = "market.m.qq.com";
            //client.DefaultRequestHeaders.Referrer = new Uri("http://3gimg.qq.com/webapp_scan/activity/newcardvideo/build/index.htm");

            //int imei = new Random().Next(100000000, 999999999);
            //int imsi = new Random().Next(100000000, 999999999);

            //string result = await client.GetStringAsync("http://market.m.qq.com/flow/order.do?method=getKey&imei=99000" + imei + "&imsi=460030" + imsi + "&product=4&channel=2&");

            //ResultJsonDwk resultJson = JsonConvert.DeserializeObject<ResultJsonDwk>(result);

            //string url = String.Format("http://m.10010.com/mall-mobile/kingNumCard/init?tencentId={0}&key={1}&product=0&channel=5#applyInfo0", resultJson.info.uid, resultJson.info.ukey);

            return Ok(new { status = 1, data = "https://m.10010.com/queen/tencent/fill.html?product=0&channel=2" });
        }


        /// <summary>
        /// 获取当前北京时间
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [NoAuthenticate]
        public IHttpActionResult GetBeijingTime()
        {
            return Ok(new
            {
                status = 1,
                msg = "success",
                data = new
                {
                    BeijingTime = DateTime.Now.ToString("yyyy-MM-dd HH:MM:ss")
                }
            });
        }

        /// <summary>
        /// 获取首页banner图片
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [NoAuthenticate]
        public async Task<IHttpActionResult> GetBannerList()
        {
            var data = from x in await _bannerService.GetAll()
                       orderby x.DisplayOrder ascending
                       select new
                       {
                           Title = x.Title,
                           Url = x.Url,
                           Image = x.Image.GetCompleteUrl(),
                       };

            return Ok(new
            {
                status = 1,
                msg = "success",
                data = data
            });
        }

        /// <summary>
        /// 获取基本配置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [NoAuthenticate]
        public async Task<IHttpActionResult> GetBasicConfig()
        {
            return Ok(new
            {
                status = 1,
                msg = "success",
                data = new
                {
                    IosVersion = UTConfig.SiteConfig.IosVersion,
                    userAgreementUrl = UTConfig.SiteConfig.UserAgreementUrl,
                    dualSimStandbyTutorialUrl = UTConfig.SiteConfig.DualSimStandbyTutorialUrl,
                    beforeGoingAbroadTutorialUrl = UTConfig.SiteConfig.BeforeGoingAbroadTutorialUrl,
                    paymentOfTerms = "(1) 先购买，后使用，不必担心费用超支\n(2) 购买后未激活使用，可随时申请退款\n(3) 若到达后无法使用，可随时申请退款",
                    //howToUse = "●购买好爱小器手环，以及套餐后，按照如下步骤操作。\n●(1) 出国前安装“爱小器”App，登陆后点击“快速设置”，进入设置页，按照指引连接手环，激活套餐\n●(2) 出国后将手环与手机的SIM卡位置交换，即可享受无限流量\n●(3) “快速设置”页面开启电话，短信功能。可享受免费国内手机号拨打接听电话，收发短信。\n●(4) 回国在“快速设置”页面关闭电话，短信功能。回国后恢复手机与手环内SIM卡的位置。"
                }
            });
        }
    }

    public class ResultJsonDwk
    {
        public string result { get; set; }

        public string msg { get; set; }

        public DwkInfo info { get; set; }
    }
    public class DwkInfo
    {
        public string uid { get; set; }
        public string ukey { get; set; }
    }

}
