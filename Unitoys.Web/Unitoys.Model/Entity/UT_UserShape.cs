using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 用户体形资料
    /// </summary>
    public class UT_UserShape : UT_Entity
    {
        /// <summary>
        /// 身高
        /// </summary>
        public double Height { get; set; }
        /// <summary>
        /// 体重
        /// </summary>
        public double Weight { get; set; }
        /// <summary>
        /// 运动目标
        /// </summary>
        public int MovingTarget { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        public Guid UserId { get; set; }

        public virtual UT_Users UT_Users { get; set; }
    }
}
