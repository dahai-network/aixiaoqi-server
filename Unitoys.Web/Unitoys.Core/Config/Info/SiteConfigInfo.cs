using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Core.Config
{
    /// <summary>
    /// 网站基础配置类
    /// </summary>
    [Serializable]
    public class SiteConfigInfo : IConfigInfo
    {
        /// <summary>
        /// 网站名称
        /// </summary>
        public string SiteName { get; set; }
        /// <summary>
        /// 网站标题
        /// </summary>
        public string SiteTitle { get; set; }
        /// <summary>
        /// seo关键字
        /// </summary>
        public string SiteKeyword { get; set; }
        /// <summary>
        /// seo描述
        /// </summary>
        public string SiteDescription { get; set; }
        /// <summary>
        /// web服务器地址(带http://xx.xx.com)
        /// </summary>
        public string SiteHost { get; set; }
        /// <summary>
        /// 主域名
        /// </summary>
        public string SiteDomain { get; set; }
        /// <summary>
        /// 图片CDN地址、带有图片处理功能(不能以"/"结尾)
        /// </summary>
        public string ImageHandleHost { get; set; }
        /// <summary>
        /// 上传的图片最大限制多少 
        /// </summary>
        public int MaxFilePicSize { get; set; }
        /// <summary>
        /// 支付宝异步付款通知返回地址
        /// </summary>
        public string NotifyUrl { get; set; }
        /// <summary>
        /// app调用酒店web页面链接地址
        /// </summary>
        public string HotelUrl { get; set; }
        /// <summary>
        /// app调用机票web页面链接地址
        /// </summary>
        public string FlightTicketUrl { get; set; }
        /// <summary>
        /// app调用的服务器ip或者域名
        /// </summary>
        public string ApiUrl { get; set; }
        /// <summary>
        /// 软交换打出IP
        /// </summary>
        public string AsteriskOutIp { get; set; }
        /// <summary>
        /// 软交换打出端口
        /// </summary>
        public string AsteriskOutPort { get; set; }
        /// <summary>
        /// 软交换打入IP
        /// </summary>
        public string AsteriskInIp { get; set; }
        /// <summary>
        /// 软交换打入端口
        /// </summary>
        public string AsteriskInPort { get; set; }
        /// <summary>
        /// 公共密钥
        /// </summary>
        public string PublicKey { get; set; }
        /// <summary>
        /// 直拨每分钟的费用
        /// </summary>
        public decimal CallDirectPricePerMinutes { get; set; }
        /// <summary>
        /// 回拨每分钟的费用
        /// </summary>
        public decimal CallBackPricePerMinutes { get; set; }
        /// <summary>
        /// RedisIp
        /// </summary>
        public string RedisIp { get; set; }
        /// <summary>
        /// 用户协议地址
        /// </summary>
        public string UserAgreementUrl { get; set; }
        /// <summary>
        /// 更新配置的时间戳
        /// </summary>
        public string UpdateConfigTime { get; set; }
        /// <summary>
        /// Ios版本号
        /// </summary>
        public string IosVersion { get; set; }
        /// <summary>
        /// 是否开启赠送用户金额
        /// </summary>
        public string IsOpenRegGift { get; set; }
        /// <summary>
        /// 注册赠送用户金额
        /// </summary>
        public decimal RegGiftAmount { get; set; }
        /// <summary>
        /// MVNO合作伙伴
        /// </summary>
        public string MVNOpartner { get; set; }
        /// <summary>
        /// MVNO合作伙伴私钥
        /// </summary>
        public string MVNOPartnerKey { get; set; }
        /// <summary>
        /// MVNO调用地址
        /// </summary>
        public string MVNOHost { get; set; }

    }
}
