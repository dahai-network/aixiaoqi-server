using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 系统属性值表
    /// </summary>
    public class UT_AttributeValue : UT_Entity
    {
        /// <summary>
        /// 属性ID
        /// </summary>
        public Guid AttributeId { get; set; }
        public string Value { get; set; }
        /// <summary>
        /// 显示顺序
        /// </summary>
        public int DisplayOrder { get; set; }
        public virtual UT_Attribute UT_Attribute { get; set; }
    }
}
