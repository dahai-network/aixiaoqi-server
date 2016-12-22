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
    public class PackageService : BaseService<UT_Package>, IPackageService
    {
        public async Task<KeyValuePair<int, List<UT_Package>>> SearchAsync(int page, int rows, string packageName, string countryId, string operators, CategoryType? category)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = db.UT_Package.Include(x => x.UT_Country).Where(x => true);

                if (!string.IsNullOrEmpty(packageName))
                {
                    query = query.Where(x => x.PackageName.Contains(packageName));
                }

                if (!string.IsNullOrEmpty(countryId) && countryId != "-1")
                {
                    query = query.Where(x => x.CountryId.Equals(countryId));
                }

                if (!string.IsNullOrEmpty(operators))
                {
                    query = query.Where(x => x.Operators.Contains(operators));
                }

                if (category.HasValue)
                {
                    query = query.Where(x => x.Category == category);
                }

                query = query.Where(x => x.IsDeleted == false);
                var result = await query.OrderBy(x => x.DisplayOrder).Skip((page - 1) * rows).Take(rows).ToListAsync();

                var count = await query.CountAsync();

                return new KeyValuePair<int, List<UT_Package>>(count, result);
            }
        }

        /// <summary>
        /// 根据ID获取套餐和套餐中的国家
        /// </summary>
        /// <returns></returns>
        public async Task<UT_Package> GetEntityAndCountryByIdAsync(Guid ID)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                return await db.UT_Package.Include(x => x.UT_Country).FirstOrDefaultAsync(x => x.ID == ID);
            }
        }
    }
}
