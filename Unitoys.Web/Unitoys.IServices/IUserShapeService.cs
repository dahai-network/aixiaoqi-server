using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Model;

namespace Unitoys.IServices
{
    public interface IUserShapeService : IBaseService<UT_UserShape>
    {
        /// <summary>
        /// 获取用户最新体形资料
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>        
        Task<UT_UserShape> GetUserShapeAsync(Guid userId);
    }
}
