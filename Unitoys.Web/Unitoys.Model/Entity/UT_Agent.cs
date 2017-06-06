using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 代理商表
    /// </summary>
    public class UT_Agent : UT_Entity
    {
        /// <summary>
        /// 公司名字
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 区域
        /// </summary>
        public string Area { get; set; }
        /// <summary>
        /// 注册地址
        /// </summary>
        public string RegisteredAddress { get; set; }
        /// <summary>
        /// 法人代表
        /// </summary>
        public string CorporateRepresentative { get; set; }
        /// <summary>
        /// 公司注册时间
        /// </summary>
        public DateTime CompanyRegistrationTime { get; set; }
        /// <summary>
        /// 公司性质，枚举
        /// </summary>
        public AgentCorporationNature CorporationNature { get; set; }
        /// <summary>
        /// 团队规模-市场人员_名
        /// </summary>
        public int MarketPersonnelNum { get; set; }
        /// <summary>
        /// 团队规模-销售人员_名
        /// </summary>
        public int SalerPersonnelNum { get; set; }
        /// <summary>
        /// 团队规模-售后服务_名
        /// </summary>
        public int AfterSalesPersonalNum { get; set; }
        /// <summary>
        /// 经营范围
        /// </summary>
        public string ScopeBusiness { get; set; }
        /// <summary>
        /// 目前销售区域
        /// </summary>
        public string SalesArea { get; set; }
        /// <summary>
        /// 年销售额
        /// </summary>
        public string AnnualSales { get; set; }
        /// <summary>
        /// 主营业务（多选）逗号分隔
        /// </summary>
        public string MainBusiness { get; set; }
        /// <summary>
        /// 主要客户类型（多选）逗号分隔
        /// </summary>
        public string MainClientCategory { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        public string Contact { get; set; }
        /// <summary>
        /// 联系人手机
        /// </summary>
        public string ContactMobilePhone { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string EMail { get; set; }
        /// <summary>
        /// QQ/微信
        /// </summary>
        public string QQWeChat { get; set; }
        /// <summary>
        /// 固话
        /// </summary>
        public string Tel { get; set; }
        /// <summary>
        /// 传真
        /// </summary>
        public string FAX { get; set; }
        /// <summary>
        /// 公司网站
        /// </summary>
        public string CompanyWebSite { get; set; }
        /// <summary>
        /// 是否代理其他产品
        /// </summary>
        public bool IsAgentOtherProducts { get; set; }
        /// <summary>
        /// 所代理产品其他产品
        /// </summary>
        public string OtherProducts { get; set; }
        /// <summary>
        /// 开发市场设想
        /// </summary>
        public string DevelopMarketAssumptions { get; set; }
        /// <summary>
        /// 贵司渠道和资源
        /// </summary>
        public string CompanyChannelsResources { get; set; }
        /// <summary>
        /// 合作建议和要求
        /// </summary>
        public string CooperationSuggestion { get; set; }
        public AgentStatus Status { get; set; }
        public int CreateDate { get; set; }
    }
    public enum AgentStatus
    {
        待审核 = 0,
        审核通过 = 1,
        审核不通过 = 2,
    }
    public enum AgentCorporationNature
    {
        未知 = 0,
        国有,
        三资,
        私营,
        联营,
        个体,
    }
}
