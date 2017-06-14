using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 用户日志
    /// </summary>
    public class UT_UserLog : UT_Entity
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid? UserId { get; set; }
        /// <summary>
        /// 日志路径，可存储多个路径，以逗号分隔
        /// </summary>
        public string LogFileUrl { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public int CreateDate { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public int UpdateDate { get; set; }
        public virtual UT_Users UT_Users { get; set; }
    }
}
