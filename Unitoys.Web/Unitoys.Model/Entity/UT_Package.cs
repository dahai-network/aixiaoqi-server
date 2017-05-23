using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 套餐表
    /// </summary>
    public class UT_Package : UT_Entity
    {
        public UT_Package()
        {
            this.UT_Order = new HashSet<UT_Order>();
            this.UT_UserReceive = new HashSet<UT_UserReceive>();
        }
        /// <summary>
        /// 套餐名称
        /// </summary>
        public string PackageName { get; set; }
        /// <summary>
        /// 套餐编号
        /// 保存与ESim对应的产品编号
        /// </summary>
        public string PackageNum { get; set; }
        /// <summary>
        /// 套餐价格
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 套餐包含的总流量
        /// </summary>
        public int Flow { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        //public string Desction { get; set; }
        /// <summary>
        /// 特色
        /// </summary>
        public string Features { get; set; }
        /// <summary>
        /// 详情
        /// </summary>
        public string Details { get; set; }
        /// <summary>
        /// 使用简介
        /// </summary>
        public string UseDescr { get; set; }
        /// <summary>
        /// Logo图片
        /// </summary>
        public string LogoPic { get; set; }
        /// <summary>
        /// 图片
        /// </summary>
        public string Pic { get; set; }
        /// <summary>
        /// 有效天数
        /// </summary>
        public int ExpireDays { get; set; }
        /// <summary>
        /// 所属国家
        /// </summary>
        public Guid? CountryId { get; set; }
        /// <summary>
        /// 运营商
        /// </summary>
        public string Operators { get; set; }
        /// <summary>
        /// 是否锁定 0:正常  1：锁定下架
        /// </summary>
        public int Lock4 { get; set; }
        /// <summary>
        /// 是否逻辑删除
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// 通话分钟数
        /// 0为无限；-1代表无通话分钟数
        /// </summary>
        public int CallMinutes { get; set; }
        /// <summary>
        /// 显示顺序
        /// </summary>
        public int DisplayOrder { get; set; }
        /// <summary>
        /// 是否能购买多个
        /// </summary>
        public bool IsCanBuyMultiple { get; set; }
        /// <summary>
        /// 是否支持4G
        /// </summary>
        public bool IsSupport4G { get; set; }
        /// <summary>
        /// 是否需要Apn
        /// </summary>
        public bool IsApn { get; set; }
        /// <summary>
        /// Apn名称
        /// </summary>
        public string ApnName { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        public CategoryType Category { get; set; }
        /// <summary>
        /// 是否流量类型
        /// </summary>
        public bool IsCategoryFlow { get; set; }
        /// <summary>
        /// 是否通话类型
        /// </summary>
        public bool IsCategoryCall { get; set; }
        /// <summary>
        /// 是否双卡双待类型
        /// </summary>
        public bool IsCategoryDualSimStandby { get; set; }
        /// <summary>
        /// 是否大王卡类型
        /// </summary>
        public bool IsCategoryKingCard { get; set; }
        /// <summary>
        /// 已领图片
        /// </summary>
        public string PicHaveed { get; set; }
        /// <summary>
        /// 描述标题图
        /// </summary>
        public string DescTitlePic { get; set; }
        /// <summary>
        /// 描述图
        /// </summary>
        public string DescPic { get; set; }
        /// <summary>
        /// 原价
        /// </summary>
        public decimal OriginalPrice { get; set; }

        public virtual ICollection<UT_Order> UT_Order { get; set; }
        public virtual UT_Country UT_Country { get; set; }
        public virtual ICollection<UT_UserReceive> UT_UserReceive { get; set; }
    }
    public enum CategoryType
    {
        [Description("流量")]
        Flow = 0,
        [Description("通话")]
        Call = 1,
        [Description("大王卡")]
        KingCard = 2,
        [Description("双卡双待")]
        DualSimStandby = 3,
        [Description("免费领取")]
        FreeReceive = 4,
        [Description("省心服务")]
        Relaxed = 5,
    }
}
