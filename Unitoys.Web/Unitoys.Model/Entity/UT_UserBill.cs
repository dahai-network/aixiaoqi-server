using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 用户账单，消费充值记录
    /// </summary>
    public class UT_UserBill : UT_Entity
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 用户登录名
        /// </summary>
        public string LoginName { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public Decimal Amount { get; set; }
        /// <summary>
        /// 用户当前余额、完成这笔交易后的金额
        /// </summary>
        public Decimal UserAmount { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public int CreateDate { get; set; }
        /// <summary>
        /// 账单的类型：0:支出、1:收入
        /// </summary>
        public int BillType { get; set; }
        /// <summary>
        /// 消费的类型：0:充值、1:在线支付、2:余额支付、3:赠送、4:话费、5:取消订单、6:订单余额支付、7:订单退款(订单第三方支付成功,用户余额不足扣除）
        /// </summary>
        public int PayType { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Descr { get; set; }
        //public string Remark { get; set; }
    }
}
