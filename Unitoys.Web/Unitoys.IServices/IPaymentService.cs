using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Model;

namespace Unitoys.IServices
{
    public interface IPaymentService : IBaseService<UT_Payment>
    {
        /// <summary>
        /// 订单支付成功后调用
        /// </summary>
        /// <param name="paymentNum">充值记录编号</param>
        /// <param name="payAmount">支付金额</param>
        /// <returns></returns>
        Task<bool> OnAfterPaymentSuccess(string paymentNum, decimal payAmount);
        /// <summary>
        /// 获取包含用户实体的所有充值记录
        /// </summary>
        /// <param name="page"></param>
        /// <param name="row"></param>
        /// <param name="paymentNum">充值记录编号</param>
        /// <param name="tel">手机号码</param>
        /// <param name="createStartDate">创建开始时间</param>
        /// <param name="createEndDate">创建结束时间</param>
        /// <returns></returns>
        Task<KeyValuePair<int, List<UT_Payment>>> GetPaymentsIncludeUser(int page, int row, string paymentNum, string tel, DateTime? createStartDate, DateTime? createEndDate);
        /// <summary>
        /// 创建充值订单
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Amount"></param>
        /// <returns></returns>
        Task<UT_Payment> Add(Guid id, decimal Amount, PaymentMethodType PaymentMethod);
    }
}
