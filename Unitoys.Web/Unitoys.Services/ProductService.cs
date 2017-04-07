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
    public class ProductService : BaseService<UT_Product>, IProductService
    {
        public async Task<KeyValuePair<int, List<UT_Product>>> SearchAsync(int page, int rows, string titile, string url, int? createStartDate, int? createEndDate)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = db.UT_Product.Where(x => true);

                if (!string.IsNullOrEmpty(titile))
                {
                    query = query.Where(x => x.Title.Contains(titile));
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

                return new KeyValuePair<int, List<UT_Product>>(count, result);
            }
        }
    }
}
