using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 众筹订单表
    /// </summary>
    public class UT_OrderByZC : UT_Entity
    {
        public UT_OrderByZC()
        {
            this.UT_OrderByZCSelectionNumber = new HashSet<UT_OrderByZCSelectionNumber>();
        }
        /// <summary>
        /// 订单编号/交易号
        /// </summary>
        public string OrderByZCNum { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid? UserId { get; set; }
        /// <summary>
        /// 订单项数量
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// 订单项单价
        /// </summary>
        public Decimal UnitPrice { get; set; }
        /// <summary>
        /// 订单项总价
        /// </summary>
        public Decimal TotalPrice { get; set; }
        /// <summary>
        /// 订单日期
        /// </summary>
        public int OrderDate { get; set; }
        /// <summary>
        /// 收货姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 联系号码
        /// </summary>
        public string CallPhone { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 回报属性
        /// </summary>
        public string GiftProperties { get; set; }
        public virtual UT_Users UT_Users { get; set; }
        public virtual ICollection<UT_OrderByZCSelectionNumber> UT_OrderByZCSelectionNumber { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
    //public enum PayStatusType
    //{
    //    [Description("未付款")]
    //    NoPayment = 0,
    //    [Description("已付款")]
    //    YesPayment = 1,
    //    [Description("异常付款")]
    //    AbnormalPayment = -1
    //}

    //public enum OrderStatusType
    //{
    //    /// <summary>
    //    /// 未激活
    //    /// </summary>
    //    [Description("未激活")]
    //    Unactivated = 0,
    //    /// <summary>
    //    /// 已激活
    //    /// </summary>
    //    [Description("已激活")]
    //    Used = 1,
    //    /// <summary>
    //    /// 已过期
    //    /// </summary>
    //    [Description("已过期")]
    //    HasExpired = 2,
    //    /// <summary>
    //    /// 已取消
    //    /// </summary>
    //    [Description("已取消")]
    //    Cancel = 3,
    //    /// <summary>
    //    /// 激活失败
    //    /// </summary>
    //    [Description("激活失败")]
    //    UnactivatError = 4,
    //}
}
