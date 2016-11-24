using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.ESIM_263mobile
{
    public class QueryProductList
    {
        public string returnCode { get; set; }

        public string returnMsg { get; set; }

        public ICollection<Product> productList { get; set; }
    }
    public class Product
    {
        /// <summary>
        /// 产品唯一编号
        /// </summary>
        public string productId { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string productName { get; set; }
        /// <summary>
        /// 产品英文名
        /// </summary>
        public string englishName { get; set; }
        /// <summary>
        /// 产品可使用最大时长
        /// </summary>
        public int duration { get; set; }
        /// <summary>
        /// 产品可使用最大流量
        /// </summary>
        public int trafficSize { get; set; }
        /// <summary>
        /// 产品定价
        /// </summary>
        public decimal productPrice { get; set; }
        /// <summary>
        /// 产品可服务区域
        /// </summary>
        public string areaName { get; set; }
        /// <summary>
        /// 产品类型(0包流量；1包天)
        /// </summary>
        public int productType { get; set; }
    }

    public class BuyProduct
    {
        public string returnCode { get; set; }

        public string returnMsg { get; set; }

        public string orderId { get; set; }
        public string beginTime { get; set; }
        public DateTime endTime { get; set; }
        public ImsiResource imsiResource { get; set; }
    }
    public class ImsiResource
    {
        /// <summary>
        /// 
        /// </summary>
        public string imsi { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ki { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string opc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string iccid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string apn { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string spn { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string simType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string msisdn { get; set; }
    }

    public class Remain
    {
        public string returnCode { get; set; }

        public string returnMsg { get; set; }
        public string beginTime { get; set; }
        public string endTime { get; set; }
        /// <summary>
        /// 服务剩余时间
        /// </summary>
        public int remainTime { get; set; }
        /// <summary>
        /// 服务剩余流量
        /// </summary>
        public int remainSize { get; set; }
        /// <summary>
        /// 产品类型
        /// </summary>
        public int productType { get; set; }
    }

    public class NotifyAccess
    {
        public string returnCode { get; set; }

        public string returnMsg { get; set; }
        /// <summary>
        /// 登网时间
        /// YYYY-MM-DD HH24:MM:SS
        /// </summary>
        public string accessTime { get; set; }
        /// <summary>
        /// 服务结束时间
        /// YYYY-MM-DD HH24:MM:SS
        /// </summary>
        public DateTime endTime { get; set; }
    }

    public class BasicResponse
    {
        public string returnCode { get; set; }

        public string returnMsg { get; set; }
    }

    public class ReactiveProduct
    {
        public string returnCode { get; set; }
        public string returnMsg { get; set; }
        /// <summary>
        /// 订单编号
        /// 续订成功的新订单号
        /// </summary>
        public string orderId { get; set; }
        /// <summary>
        /// 订购时的生效时间
        /// </summary>
        public string beginTime { get; set; }
        /// <summary>
        /// 服务结束时间
        /// </summary>
        public DateTime endTime { get; set; }
    }

    public class GetIMSI
    {
        public string returnCode { get; set; }
        public string returnMsg { get; set; }
        public string beginTime { get; set; }
        public DateTime endTime { get; set; }
        public ImsiResource imsiResource { get; set; }
    }

    public class QueryOrder
    {
        public string returnCode { get; set; }
        public string returnMsg { get; set; }
        public string beginTime { get; set; }
        public DateTime endTime { get; set; }
        /// <summary>
        /// 1：已生效；
        /// 2：已过期；
        /// 3：已退订
        /// </summary>
        public int orderStatus { get; set; }
        public string productId { get; set; }
        /// <summary>
        /// 0：未登网；
        /// 1：已登网
        /// </summary>
        public int isAccess { get; set; }
        /// <summary>
        /// 订单流量明细
        /// </summary>
        public DailyUsage[] dailyUsageList { get; set; }
    }

    public class DailyUsage
    {
        public string usageDate { get; set; }
        public long trafficSize { get; set; }
    }

    public class MsgReceiver
    {
        /// <summary>
        /// 消息接收者
        /// 1，	如果是http接口方式填URL接口地址；支持https协议，但是证书必须是受信任的；如果是微信，填微信账号；
        /// 2，所有消息接收都用统一url
        /// </summary>
        public string receiver { get; set; }
        /// <summary>
        /// 接受消息方式
        /// 1:http接口;2:短信;3:邮件,4:APP;5:微信目前只支持1
        /// </summary>
        public int receiveType { get; set; }
        /// <summary>
        /// 需要订阅的消息种类
        /// </summary>
        public MessageIdType messageId { get; set; }
    }

    public enum MessageIdType
    {
        /// <summary>
        /// acessInfo
        /// </summary>
        登网通知 = 1011,
        /// <summary>
        /// remainInfo
        /// </summary>
        余量提醒 = 1017,
        /// <summary>
        /// resourceRecycle
        /// </summary>
        资源回收 = 2000,
    }

    public class AcessInfo
    {
        public long messageId { get; set; }
        public string imsi { get; set; }
        public string orderId { get; set; }
        /// <summary>
        /// 登网时间
        /// </summary>
        public DateTime accessTime { get; set; }
    }

    public class RemainInfo
    {
        public long messageId { get; set; }
        public string imsi { get; set; }
        public string orderId { get; set; }
        /// <summary>
        /// 剩余流量
        /// </summary>
        public long remainSize { get; set; }
    }

    public class ResourceRecycle
    {
        public long messageId { get; set; }
        public string imsi { get; set; }
        public string orderId { get; set; }
    }

    public class RefundOrder
    {
        public string returnCode { get; set; }
        public string returnMsg { get; set; }
        public float refundAmount { get; set; }
        public DateTime refundTime { get; set; }
    }
}
