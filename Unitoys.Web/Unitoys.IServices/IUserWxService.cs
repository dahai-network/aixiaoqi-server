using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Model;

namespace Unitoys.IServices
{
    public interface IUserWxService : IBaseService<UT_UsersWx>
    {
        
        /// <summary>
        /// 检查该微信是否已经绑定了账号
        /// </summary>
        /// <param name="tel">用户openid</param>
        /// <returns>true：绑定，false：未绑定</returns>
        bool CheckOpenIdExist(string openId);
        /// <summary>
        /// 检查用户是否已经绑定
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        bool CheckUserBindWx(Guid id);
    }
}
