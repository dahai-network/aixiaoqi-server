using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Model;

namespace Unitoys.IServices
{
    public interface IOrderByZCSelectionNumberService : IBaseService<UT_OrderByZCSelectionNumber>
    {
        /// <summary>
        /// 添加订单
        /// </summary>
        /// <param name="userId">用户</param>
        /// <param name="orderByZCId">订单</param>
        /// <param name="Name">姓名</param>
        /// <param name="IdentityNumber">身份证号</param>
        /// <param name="MobileNumber">选择的号码</param>
        /// <param name="PaymentMethod">支付方式</param>
        /// <returns></returns>
        Task<UT_OrderByZCSelectionNumber> AddOrder(Guid userId, Guid orderByZCId, string Name, string IdentityNumber, string MobileNumber, PaymentMethodType? PaymentMethod);

        /// <summary>
        /// 订单支付成功后调用
        /// </summary>
        /// <param name="orderNum">订单编号</param>
        /// <param name="payAmount">支付金额</param>
        /// <returns></returns>
        Task<bool> OnAfterOrderSuccess(string orderNum, decimal payAmount);

        /// <summary>
        /// 通过用户余额支付套餐订单
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="orderId">订单ID</param>
        /// <returns></returns>
        Task<int> PayOrderByUserAmount(Guid userId, Guid orderId);

        /// <summary>
        /// 根据条件获取订单选号和订单
        /// </summary>
        /// <returns></returns>
        Task<UT_OrderByZCSelectionNumber> GetEntityAndOrderByZCAsync(System.Linq.Expressions.Expression<Func<UT_OrderByZCSelectionNumber, bool>> exp);
    }
}
