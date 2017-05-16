using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 套餐属性表
    /// </summary>
    public class UT_PackageAttribute : UT_Entity
    {
        public Guid PackageId { get; set; }
        /// <summary>
        /// 套餐包含的总流量
        /// </summary>
        public int? Flow { get; set; }
        /// <summary>
        /// 通话分钟数
        /// </summary>
        public int? CallMinutes { get; set; }
        /// <summary>
        /// 有效天数
        /// </summary>
        public int? ExpireDays { get; set; }
        /// <summary>
        /// 套餐包含的总流量
        /// </summary>
        public string FlowDescr { get; set; }
        /// <summary>
        /// 通话分钟数
        /// </summary>
        public string CallMinutesDescr { get; set; }
        /// <summary>
        /// 有效天数
        /// </summary>
        public string ExpireDaysDescr { get; set; }
        /// <summary>
        /// 套餐价格
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 原价
        /// </summary>
        public decimal OriginalPrice { get; set; }
        /// <summary>
        /// 显示顺序
        /// </summary>
        public int DisplayOrder { get; set; }
        public virtual UT_Package UT_Package { get; set; }
        //public virtual UT_Attribute UT_Attribute { get; set; }
        //public virtual UT_AttributeValue UT_AttributeValue { get; set; }
        public int CreateDate { get; set; }
    }
}
