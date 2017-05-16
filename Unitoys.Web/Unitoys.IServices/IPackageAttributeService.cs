using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Model;

namespace Unitoys.IServices
{
    public interface IPackageAttributeService : IBaseService<UT_PackageAttribute>
    {
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="rows">页数</param>
        /// <param name="packageName">用户手机号</param>
        /// <param name="createStartDate">创建开始时间</param>
        /// <param name="createEndDate">创建结束时间</param>
        /// <returns></returns>
        Task<KeyValuePair<int, List<UT_PackageAttribute>>> SearchAsync(int page, int rows, string packageName, int? createStartDate, int? createEndDate);
        Task<KeyValuePair<int, List<UT_PackageAttribute>>> GetByPackageId(Guid packageId);
    }
}
