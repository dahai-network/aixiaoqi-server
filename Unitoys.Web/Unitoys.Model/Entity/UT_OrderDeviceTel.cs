using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 服务订单
    /// </summary>
    public class UT_OrderDeviceTel : UT_Entity
    {
        /// <summary>
        /// 套餐ID
        /// </summary>
        public Guid OrderId { get; set; }
        /// <summary>
        /// 设备内号码
        /// </summary>
        public string Tel { get; set; }
        /// <summary>
        /// 月套餐费
        /// </summary>
        public decimal MonthPackageFee { get; set; }
        public virtual UT_Order UT_Order { get; set; }
    }
}
