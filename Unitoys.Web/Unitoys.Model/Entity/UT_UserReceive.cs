using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 用户领取记录
    /// </summary>
    public class UT_UserReceive : UT_Entity
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid? UserId { get; set; }
        /// <summary>
        /// 套餐ID
        /// </summary>
        public Guid PackageId { get; set; }
        /// <summary>
        /// 套餐ID
        /// </summary>
        public Guid OrderId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public int CreateDate { get; set; }
        public virtual UT_Users UT_Users { get; set; }
        public virtual UT_Package UT_Package { get; set; }
        public virtual UT_Order UT_Order { get; set; }
    }
}
