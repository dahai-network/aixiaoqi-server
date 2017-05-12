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
    public class UserReceiveService : BaseService<UT_UserReceive>, IUserReceiveService
    {
        public async Task<KeyValuePair<int, List<UT_UserReceive>>> SearchAsync(int page, int rows, string tel, int? createStartDate, int? createEndDate)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = db.UT_UserReceive.Include(x => x.UT_Users).Where(x => true);

                if (!string.IsNullOrEmpty(tel))
                {
                    query = query.Where(x => x.UT_Users.Tel.Contains(tel));
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

                return new KeyValuePair<int, List<UT_UserReceive>>(count, result);
            }
        }

        public bool Haveed(Guid userId, Guid packageId)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                return CheckHaveed(db, userId, packageId);
            }
        }
        public static bool CheckHaveed(UnitoysEntities db, Guid userId, Guid packageId)
        {
            return db.UT_UserReceive.Any(x => x.UserId == userId && x.PackageId == packageId && x.UT_Order.OrderStatus == OrderStatusType.Used);
        }
    }
}
