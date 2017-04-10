using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Model;

namespace Unitoys.IServices
{
    public interface IOrderService : IBaseService<UT_Order>
    {
        /// <summary>
        /// 添加订单
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="orderName">订单名</param>
        /// <param name="packageId">套餐ID</param>
        /// <param name="quantity">数量</param>
        /// <param name="unitPrice">单价</param>
        /// <param name="orderDate">订单日期</param>
        /// <param name="PaymentMethod">支付方式</param>
        /// <returns>0失败/1成功/2套餐被锁定</returns>
        Task<KeyValuePair<int, UT_Order>> AddOrder(Guid userId, Guid packageId, int quantity, PaymentMethodType PaymentMethod);
        /// <summary>
        /// 通过用户余额支付套餐订单
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="orderId">订单ID</param>
        /// <returns></returns>
        Task<int> PayOrderByUserAmount(Guid userId, Guid orderId);
        /// <summary>
        /// 订单支付成功后调用
        /// </summary>
        /// <param name="orderId">订单编号</param>
        /// <param name="payAmount">支付金额
        /// </param>
        /// <returns></returns>
        Task<bool> OnAfterOrderSuccess(string orderNum, decimal payAmount);
        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="orderId">订单ID</param>
        /// <returns></returns>
        Task<int> CancelOrder(Guid userId, Guid orderId);
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="row">页数</param>
        /// <param name="orderNum">订单号</param>
        /// <param name="tel">用户手机号</param>
        /// <param name="packageName">套餐名</param>
        /// <param name="payStatus">付款状态</param>
        /// <param name="orderStatus">订单状态</param>
        /// <returns></returns>
        Task<KeyValuePair<int, List<UT_Order>>> SearchAsync(int page, int row, string orderNum, string tel, string packageName, PayStatusType? payStatus, OrderStatusType? orderStatus);

        /// <summary>
        /// 查询用户订单,只返回已支付的
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="row">页数</param>
        /// <param name="userId">用户</param>
        /// <param name="payStatus">支付状态</param>
        /// <returns></returns>
        Task<KeyValuePair<int, List<UT_Order>>> GetUserOrderList(int page, int row, Guid userId, PayStatusType? payStatus, OrderStatusType? orderStatus, CategoryType? PackageCategory, bool? packageIsCategoryFlow, bool? packageIsCategoryCall, bool? packageIsCategoryDualSimStandby, bool? packageIsCategoryKingCard);

        /// <summary>
        /// 是否包含正在使用的套餐
        /// </summary>
        /// <returns></returns>
        Task<bool> IsStatusUsed(Guid ID);

        /// <summary>
        /// 根据ID获取订单和订单中的套餐
        /// </summary>
        /// <returns></returns>
        Task<UT_Order> GetEntityAndPackageByIdAsync(Guid ID);
        /// <summary>
        /// 根据套餐类型判断当前用户是否有正在使用的套餐
        /// </summary>
        /// <returns></returns>
        Task<bool> IsStatusUsed(Guid userId, CategoryType? PackageCategory, bool? packageIsCategoryFlow, bool? packageIsCategoryCall, bool? packageIsCategoryDualSimStandby, bool? packageIsCategoryKingCard);
    }
}
