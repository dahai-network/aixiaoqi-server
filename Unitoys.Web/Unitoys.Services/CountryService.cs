using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.IServices;
using Unitoys.Model;
using System.Data.Entity;

namespace Unitoys.Services
{
    public class CountryService : BaseService<UT_Country>, ICountryService
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
        public async Task<KeyValuePair<int, List<UT_Country>>> SearchAsync(int page, int row, ContinentsType? continents, string countryName, string countryCode, bool? isHot)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = db.UT_Country.Where(x => true);

                if (continents != null)
                {
                    query = query.Where(x => x.Continents == continents);
                }
                if (!string.IsNullOrEmpty(countryName))
                {
                    query = query.Where(x => x.CountryName.Contains(countryName));
                }
                if (!string.IsNullOrEmpty(countryCode))
                {
                    query = query.Where(x => x.CountryCode.Contains(countryCode));
                }
                if (isHot.HasValue)
                {
                    query = query.Where(x => x.IsHot == isHot.Value);
                }

                var result = await query.OrderBy(x => x.DisplayOrder).Skip((page - 1) * row).Take(row).ToListAsync();

                var count = await query.CountAsync();

                return new KeyValuePair<int, List<UT_Country>>(count, result);
            }
        }
    }
}
