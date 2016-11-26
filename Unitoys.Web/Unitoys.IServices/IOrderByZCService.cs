using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Model;

namespace Unitoys.IServices
{
    public interface IOrderByZCService : IBaseService<UT_OrderByZC>
    {
        /// <summary>
        /// 绑定众筹订单
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="callPhone">联系电话</param>
        /// <returns>0失败/1成功/2不存在/3已被绑定</returns>
        Task<int> BindOrder(Guid userId, string callPhone);
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="row">页数</param>
        /// <param name="orderNum">订单号</param>
        /// <param name="name">姓名</param>
        /// <param name="callPhone">联系号码</param>
        /// <param name="address">地址</param>
        /// <param name="payStatus">付款状态</param>
        /// <returns></returns>
        Task<KeyValuePair<int, List<UT_OrderByZC>>> SearchAsync(int page, int row, string orderNum, string name, string callPhone, string address, PayStatusType? payStatus);
        /// <summary>
        /// 查询用户订单
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="row">页数</param>
        /// <param name="userId">用户</param>
        /// <returns></returns>
        Task<KeyValuePair<int, List<UT_OrderByZC>>> GetUserOrderByZCList(int page, int row, Guid userId);
        /// <summary>
        /// 根据ID获取订单和订单中的选号订单
        /// </summary>
        /// <returns></returns>
        Task<UT_OrderByZC> GetEntityAndOrderByZCSelectionNumberByIdAsync(Guid ID);
    }
}
