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
        Task<KeyValuePair<string, KeyValuePair<int, UT_Order>>> AddOrder(Guid userId, Guid packageId, int quantity, PaymentMethodType PaymentMethod, decimal MonthPackageFee, Guid? PackageAttributeId, DateTime? BeginDateTime);
        Task<KeyValuePair<string, KeyValuePair<int, UT_Order>>> AddReceiveOrder(Guid userId, Guid packageId);
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
        Task<int> OnAfterOrderSuccess(string orderNum, decimal payAmount);
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

        /// <summary>
        /// 激活订单（MVNO购卡）
        /// </summary>
        /// <param name="UserID">用户ID</param>
        /// <param name="OrderID">订单ID</param>
        /// <param name="BeginTime">开始生效时间戳</param>
        /// <param name="BeginDateTime">开始生效日期格式</param>
        /// <returns>
        /// -2：激活失败_超过最晚激活日期
        /// -3：激活失败_激活类型异常（非流量套餐）
        /// -4：激活套餐失败_可能套餐已过期 "暂时无法激活,请联系客服"
        /// -5：卡激活过程异常（购卡异常）
        /// -1：失败
        /// -0：成功
        /// -6：套餐激活成功,更新数据库失败
        /// </returns>
        Task<int> Activation(Guid UserID, Guid OrderID, int? BeginTime, DateTime? BeginDateTime);

        /// <summary>
        /// 激活订单（MVNO购卡）
        /// </summary>
        /// <param name="UserID">用户ID</param>
        /// <param name="order">订单实体</param>
        /// <param name="package">套餐实体，减少多余的访问，如果原来已获取到，便传入，不传入则根据order的套餐来获取</param>
        /// <param name="user">用户实体，减少多余的访问，如果原来已获取到，便传入，不传入则根据order的用户来获取</param>
        /// <param name="BeginTime">开始生效时间戳</param>
        /// <param name="BeginDateTime">开始生效日期格式</param>
        /// <param name="db">数据库上下午，用于复用与事务</param>
        /// <returns>
        /// 2：激活失败_超过最晚激活日期
        /// 3：激活失败_激活类型异常（非流量套餐）
        /// 4：激活套餐失败_可能套餐已过期 "暂时无法激活,请联系客服"
        /// 5：卡激活过程异常（购卡异常）
        /// 1：成功
        /// 0：失败
        /// </returns>
        //Task<int> Activation(Guid UserID, UT_Order order, UT_Package package, UT_Users user, int? BeginTime, DateTime? BeginDateTime, UnitoysEntities db);
    }
}
