using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Model;

namespace Unitoys.IServices
{
    public interface IOrderByZCConfirmationService : IBaseService<UT_OrderByZCConfirmation>
    {
        /// <summary>
        /// 绑定众筹订单验证成功号码
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="callPhone">联系电话</param>
        /// <returns>0失败/1成功/2已绑定</returns>
        Task<int> Bind(Guid userId, string tel);
        /// <summary>
        /// 验证用户是否绑定
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="tel">联系电话</param>
        /// <returns></returns>
        Task<bool> CheckUserTelExist(Guid userId, string tel);
    }
}
