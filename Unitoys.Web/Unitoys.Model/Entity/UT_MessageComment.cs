using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 用户动态消息评论
    /// </summary>
    public class UT_MessageComment : UT_Entity
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 用户动态消息ID
        /// </summary>
        public Guid MessageId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 评论内容
        /// </summary>
        public string Content { get; set; }

        public virtual UT_Users UT_Users { get; set; }
        public virtual UT_Message UT_Message { get; set; }
    }
}
