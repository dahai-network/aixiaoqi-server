using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 意见反馈信息
    /// </summary>
    public class UT_Feedback : UT_Entity
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid? UserId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 反馈信息
        /// </summary>
        public string Info { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// 机型
        /// </summary>
        public string Model { get; set; }
        /// <summary>
        /// 入口
        /// </summary>
        public string Entrance { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Mail { get; set; }
        public virtual UT_Users UT_Users { get; set; }
    }
}
