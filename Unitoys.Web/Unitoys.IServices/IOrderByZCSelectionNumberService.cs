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
    }
}
