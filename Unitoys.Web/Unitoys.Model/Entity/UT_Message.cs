using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 用户动态消息
    /// </summary>
    public class UT_Message : UT_Entity
    {
        public UT_Message()
        {
            this.UT_MessagePhoto = new HashSet<UT_MessagePhoto>();
            this.UT_MessageComment = new HashSet<UT_MessageComment>();
            this.UT_MessageLike = new HashSet<UT_MessageLike>();
        }

        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 发表消息的IP
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// 发表消息的国家
        /// </summary>
        public string Country { get; set; }
        /// <summary>
        /// 发表消息的地址
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        /// 发表消息的内容
        /// </summary>
        public string Content { get; set; }

        public virtual UT_Users UT_Users { get; set; }

        public virtual ICollection<UT_MessagePhoto> UT_MessagePhoto { get; set; }
        public virtual ICollection<UT_MessageComment> UT_MessageComment { get; set; }
        public virtual ICollection<UT_MessageLike> UT_MessageLike { get; set; }
    }
}
