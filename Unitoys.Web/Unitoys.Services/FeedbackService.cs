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
    public class FeedbackService : BaseService<UT_Feedback>, IFeedbackService
    {
        public async Task<KeyValuePair<int, List<UT_Feedback>>> SearchAsync(int page, int rows, string tel, DateTime? createStartDate, DateTime? createEndDate)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = db.UT_Feedback.Include(x => x.UT_Users).Where(x => true);

                if (!string.IsNullOrEmpty(tel))
                {
                    query = query.Where(x => x.UT_Users.Tel.Contains(tel));
                }

                if (createStartDate != null && createStartDate != DateTime.MinValue)
                {
                    query = query.Where(x => x.CreateDate >= createStartDate);
                }

                if (createEndDate != null && createEndDate != DateTime.MinValue)
                {
                    query = query.Where(x => x.CreateDate <= createEndDate);
                }

                var result = await query.OrderByDescending(x => x.CreateDate).Skip((page - 1) * rows).Take(rows).ToListAsync();

                var count = await query.CountAsync();

                return new KeyValuePair<int, List<UT_Feedback>>(count, result);
            }
        }
    }
}
