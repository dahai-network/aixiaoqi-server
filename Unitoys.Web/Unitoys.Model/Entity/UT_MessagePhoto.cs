using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 用户动态消息的图片
    /// </summary>
    public class UT_MessagePhoto : UT_Entity
    {
        /// <summary>
        /// 用户动态消息ID
        /// </summary>
        public Guid MessageId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 图片路径
        /// </summary>
        public string Path { get; set; }
        public virtual UT_Message UT_Message { get; set; }
    }
}
