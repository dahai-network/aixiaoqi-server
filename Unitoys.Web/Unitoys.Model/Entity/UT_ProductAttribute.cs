using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 系统属性表
    /// </summary>
    public class UT_ProductAttribute : UT_Entity
    {
        public Guid PackageId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public Guid AttributeId { get; set; }
        public Guid AttributeValueId { get; set; }
        /// <summary>
        /// 显示顺序
        /// </summary>
        public int DisplayOrder { get; set; }
        public virtual UT_Package UT_Package { get; set; }
        public virtual UT_Attribute UT_Attribute { get; set; }
        public virtual UT_AttributeValue UT_AttributeValue { get; set; }
        public int CreateDate { get; set; }
    }
}
