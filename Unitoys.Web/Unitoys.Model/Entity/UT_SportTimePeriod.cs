using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 运动时间
    /// </summary>
    public class UT_SportTimePeriod : UT_Entity
    {
        /// <summary>
        /// 步数
        /// </summary>
        public int StepNum { get; set; }
        /// <summary>
        /// 开始运动时间
        /// </summary>
        public int StartDateTime { get; set; }
        /// <summary>
        /// 结束运动时间
        /// </summary>
        public int EndDateTime { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }
        public virtual UT_Users UT_Users { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public int CreateDate { get; set; }
    }
}
