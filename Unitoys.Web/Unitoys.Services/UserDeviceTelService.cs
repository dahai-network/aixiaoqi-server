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
    public class UserDeviceTelService : BaseService<UT_UserDeviceTel>, IUserDeviceTelService
    {
        public async Task<KeyValuePair<int, List<UT_UserDeviceTel>>> SearchAsync(int page, int rows, string tel, int? createStartDate, int? createEndDate)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = db.UT_UserDeviceTel.Include(x => x.UT_Users).Where(x => true);

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

                return new KeyValuePair<int, List<UT_UserDeviceTel>>(count, result);
            }
        }

        public async Task<UT_UserDeviceTel> GetFirst(Guid userId)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                return await db.UT_UserDeviceTel.OrderByDescending(x => x.UpdateDate).FirstOrDefaultAsync(x => x.UserId == userId && x.IsConfirmed == true);
            }
        }

        public async Task<KeyValuePair<bool, string>> CheckConfirmed(Guid userId, string ICCID)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var model = await db.UT_UserDeviceTel.FirstOrDefaultAsync(x => x.UserId == userId && x.ICCID == ICCID && x.IsConfirmed == true);
                if (model == null)
                    return new KeyValuePair<bool, string>(false, "");
                return new KeyValuePair<bool, string>(true, model.Tel);
            }
        }
    }
}
