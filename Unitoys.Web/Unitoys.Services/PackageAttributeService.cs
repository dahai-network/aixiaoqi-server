using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.IServices;
using Unitoys.Core;
using Unitoys.Model;
using System.Data.Entity;

namespace Unitoys.Services
{
    public class PackageAttributeService : BaseService<UT_PackageAttribute>, IPackageAttributeService
    {
        public async Task<KeyValuePair<int, List<UT_PackageAttribute>>> SearchAsync(int page, int rows, string packageName, int? createStartDate, int? createEndDate, Guid packageId)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = db.UT_PackageAttribute.Include(x => x.UT_Package).Where(x => x.PackageId == packageId);
                if (!string.IsNullOrEmpty(packageName))
                {
                    query = query.Where(x => x.UT_Package.PackageName.Contains(packageName));
                }
                if (!string.IsNullOrEmpty(packageName))
                {
                    query = query.Where(x => x.UT_Package.PackageName.Contains(packageName));
                }
                if (createStartDate.HasValue)
                {
                    query = query.Where(x => x.CreateDate >= createStartDate);
                }

                if (createEndDate.HasValue)
                {
                    query = query.Where(x => x.CreateDate <= createEndDate);
                }

                var result = await query.OrderByDescending(x => x.CreateDate).Skip((page - 1) * rows).Take(rows).ToListAsync();

                var count = await query.CountAsync();

                return new KeyValuePair<int, List<UT_PackageAttribute>>(count, result);
            }
        }
        public async Task<KeyValuePair<int, List<UT_PackageAttribute>>> GetByPackageId(Guid packageId)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = db.UT_PackageAttribute.Where(x => x.PackageId == packageId);

                var result = await query.OrderBy(x => x.DisplayOrder).ToListAsync();

                var count = await query.CountAsync();

                return new KeyValuePair<int, List<UT_PackageAttribute>>(count, result);
            }
        }
    }
}
