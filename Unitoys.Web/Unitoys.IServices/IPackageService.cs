using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Model;

namespace Unitoys.IServices
{
    public interface IPackageService : IBaseService<UT_Package>
    {
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="rows">页数</param>
        /// <param name="packageName">套餐名称</param>
        /// <param name="countryId">国家ID</param>
        /// <param name="operators">运营商</param>
        /// <param name="category">类型</param>
        /// <returns></returns>
        Task<KeyValuePair<int, List<UT_Package>>> SearchAsync(int page, int rows, string packageName, string countryId, string operators, CategoryType? category);

        /// <summary>
        /// 根据ID获取套餐和套餐中的国家
        /// </summary>
        /// <returns></returns>
        Task<UT_Package> GetEntityAndCountryByIdAsync(Guid ID);
    }
}
