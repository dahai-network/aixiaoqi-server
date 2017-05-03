using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 黑名单
    /// </summary>
    public class UT_BlackList : UT_Entity
    {
        public UT_BlackList()
        {

        }
        /// <summary>
        /// 加黑号码
        /// </summary>
        public string BlackNum { get; set; }
        /// <summary>
        /// 关联用户ID
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 绑定日期
        /// </summary>
        public int CreateDate { get; set; }
        /// <summary>
        /// 乐观并发
        /// </summary>
        [Timestamp]
        public Byte[] RowVersion { get; set; }
        public virtual UT_Users UT_Users { get; set; }
    }
}
