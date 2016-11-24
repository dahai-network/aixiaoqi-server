using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 运动
    /// </summary>
    public class UT_Sport : UT_Entity
    {
        /// <summary>
        /// 总步数
        /// </summary>
        public int StepNum { get; set; }
        /// <summary>
        /// 运动日期
        /// </summary>
        public int Date { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public int CreateDate { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }
        public virtual UT_Users UT_Users { get; set; }
       
    }
}
