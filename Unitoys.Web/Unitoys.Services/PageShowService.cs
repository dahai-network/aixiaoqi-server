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
    public class PageShowService : BaseService<UT_PageShow>, IPageShowService
    {
        public async Task<KeyValuePair<int, List<UT_PageShow>>> SearchAsync(int page, int rows, string name, string entryName, int? createStartDate, int? createEndDate)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = db.UT_PageShow.Where(x => true);

                if (!string.IsNullOrEmpty(name))
                {
                    query = query.Where(x => x.Name.Contains(name));
                }

                if (!string.IsNullOrEmpty(entryName))
                {
                    query = query.Where(x => x.EntryName.Contains(entryName));
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

                return new KeyValuePair<int, List<UT_PageShow>>(count, result);
            }
        }
    }
}
