using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    public class UT_OrderUsage : UT_Entity
    {
        /// <summary>
        /// 订单项ID
        /// </summary>
        public Guid OrderId { get; set; }
        /// <summary>
        /// 已使用多少流量
        /// </summary>
        public int UsedFlow { get; set; }
        /// <summary>
        /// 已使用多少通话分钟数
        /// </summary>
        public int UsedCallMinutes { get; set; }
        /// <summary>
        /// 开始使用时间
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 结束使用时间
        /// </summary>
        public DateTime EndDate { get; set; }

        public virtual UT_Order UT_Order { get; set; }
    }
}
