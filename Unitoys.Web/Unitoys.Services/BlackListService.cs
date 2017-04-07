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
    public class BlackListService : BaseService<UT_BlackList>, IBlackListService
    {
        public async Task<KeyValuePair<int, List<UT_BlackList>>> SearchAsync(int page, int rows, string blackNum, string tel, int? createStartDate, int? createEndDate)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = db.UT_BlackList.Include(x => x.UT_Users).Where(x => true);
                if (!string.IsNullOrEmpty(tel))
                {
                    query = query.Where(x => x.UT_Users.Tel.Contains(tel));
                }
                if (!string.IsNullOrEmpty(blackNum))
                {
                    query = query.Where(x => x.BlackNum.Contains(blackNum));
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

                return new KeyValuePair<int, List<UT_BlackList>>(count, result);
            }
        }
        public async Task<bool> CheckBlackNumByNotUserExist(Guid UserId, string blackNum)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                return await db.UT_BlackList.AnyAsync(a => a.BlackNum == blackNum && a.UserId != UserId);
            }
        }
    }
}
