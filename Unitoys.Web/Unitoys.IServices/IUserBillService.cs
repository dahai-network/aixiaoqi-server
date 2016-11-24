using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Model;

namespace Unitoys.IServices
{
    public interface IUserBillService : IBaseService<UT_UserBill>
    {
        /// <summary>
        /// 异步搜索
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="rows">页数</param>
        /// <param name="loginName">用户名</param>
        /// <param name="createStartDate">创建开始时间</param>
        /// <param name="createEndDate">创建结束时间</param>
        /// <param name="billType">账单类型</param>
        /// <returns></returns>
        Task<KeyValuePair<int, List<UT_UserBill>>> SearchAsync(int page, int rows, string loginName, int? createStartDate, int? createEndDate, int? billType);
    }
}
