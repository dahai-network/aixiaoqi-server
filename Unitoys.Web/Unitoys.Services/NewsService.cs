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
    public class NewsService : BaseService<UT_News>, INewsService
    {
        public async Task<KeyValuePair<int, List<UT_News>>> SearchAsync(int page, int rows, string title, string publisher, int? createStartDate, int? createEndDate)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = db.UT_News.Where(x => true);

                if (!string.IsNullOrEmpty(title))
                {
                    query = query.Where(x => x.Content.Contains(title));
                }

                if (!string.IsNullOrEmpty(publisher))
                {
                    query = query.Where(x => x.Publisher.Contains(publisher));
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

                return new KeyValuePair<int, List<UT_News>>(count, result);
            }
        }
    }
}
