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
    public class DeviceBraceletService : BaseService<UT_DeviceBracelet>, IDeviceBraceletService
    {
        public async Task<KeyValuePair<int, List<UT_DeviceBracelet>>> SearchAsync(int page, int rows, string tel, int? createStartDate, int? createEndDate)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = db.UT_DeviceBracelet.Include(x => x.UT_Users).Where(x => true);
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

                return new KeyValuePair<int, List<UT_DeviceBracelet>>(count, result);
            }
        }

        public async Task<bool> CheckUserIdExist(Guid UserId)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                return await db.UT_DeviceBracelet.AnyAsync(a => a.UserId == UserId);
            }
        }
        public async Task<bool> CheckIMEIByNotUserExist(Guid UserId, string IMEI)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                return await db.UT_DeviceBracelet.AnyAsync(a => a.IMEI == IMEI && a.UserId != UserId);
            }
        }
    }
}
