using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 国家费率表
    /// </summary>
    public class UT_Country : UT_Entity
    {
        public UT_Country()
        {
            this.UT_Package = new HashSet<UT_Package>();
        }
        /// <summary>
        /// 国家名称
        /// </summary>
        public string CountryName { get; set; }
        /// <summary>
        /// 国家代码
        /// </summary>
        public string CountryCode { get; set; }
        /// <summary>
        /// 图片
        /// </summary>
        public string Pic { get; set; }
        /// <summary>
        /// logo图
        /// </summary>
        public string LogoPic { get; set; }
        /// <summary>
        /// 费率
        /// </summary>
        public decimal Rate { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        public virtual ICollection<UT_Package> UT_Package { get; set; }
        /// <summary>
        /// 是否热门
        /// </summary>
        public bool IsHot { get; set; }
        /// <summary>
        /// 大洲
        /// </summary>
        public ContinentsType Continents { get; set; }
        /// <summary>
        /// 显示顺序
        /// </summary>
        public int DisplayOrder { get; set; }
    }
    public enum ContinentsType
    {
        /// <summary>
        /// 亚洲
        /// </summary>
        [Description("亚洲")]
        Asia = 0,
        /// <summary>
        /// 非洲
        /// </summary>
        [Description("非洲")]
        Africa = 1,
        /// <summary>
        /// 欧洲
        /// </summary>
        [Description("欧洲")]
        Europe = 2,
        /// <summary>
        /// 北美
        /// </summary>
        [Description("北美洲")]
        NorthAmerica = 3,
        /// <summary>
        /// 南美洲
        /// </summary>
        [Description("南美洲")]
        SouthAmerica = 4,
        /// <summary>
        /// 大洋洲
        /// </summary>
        [Description("大洋洲")]
        Oceania = 5,
        /// <summary>
        /// 南极洲
        /// </summary>
        [Description("南极洲")]
        Antarctica = 6
    }
}
