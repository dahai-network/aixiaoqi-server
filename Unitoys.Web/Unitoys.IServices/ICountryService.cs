using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Model;

namespace Unitoys.IServices
{
    public interface ICountryService : IBaseService<UT_Country>
    {
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="row">页数</param>
        /// <param name="continents">洲</param>
        /// <param name="countryName">国家名称</param>
        /// <param name="countryCode">国家代码</param>
        /// <param name="isHot">是否热门</param>
        /// <returns></returns>
        Task<KeyValuePair<int, List<UT_Country>>> SearchAsync(int page, int row, ContinentsType? continents, string countryName, string countryCode, bool? isHot);
    }
}
