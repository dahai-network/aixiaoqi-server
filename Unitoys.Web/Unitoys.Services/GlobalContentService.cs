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
    public class GlobalContentService : BaseService<UT_GlobalContent>, IGlobalContentService
    {
        public async Task<KeyValuePair<int, List<UT_GlobalContent>>> SearchAsync(int page, int rows, string name, GlobalContentType? globalContentType, int? createStartDate, int? createEndDate)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = db.UT_GlobalContent.Where(x => true);

                if (!string.IsNullOrEmpty(name))
                {
                    query = query.Where(x => x.Name.Contains(name));
                }

                if (globalContentType.HasValue)
                {
                    query = query.Where(x => x.GlobalContentType == globalContentType);
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

                return new KeyValuePair<int, List<UT_GlobalContent>>(count, result);
            }
        }
    }
}
