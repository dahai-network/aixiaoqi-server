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
    public class BannerService : BaseService<UT_Banner>, IBannerService
    {
        public async Task<KeyValuePair<int, List<UT_Banner>>> SearchAsync(int page, int rows, string title, string url, int? createStartDate, int? createEndDate)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = db.UT_Banner.Where(x => true);

                if (!string.IsNullOrEmpty(title))
                {
                    query = query.Where(x => x.Title.Contains(title));
                }

                if (!string.IsNullOrEmpty(url))
                {
                    query = query.Where(x => x.Url.Contains(url));
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

                return new KeyValuePair<int, List<UT_Banner>>(count, result);
            }
        }
    }
}
