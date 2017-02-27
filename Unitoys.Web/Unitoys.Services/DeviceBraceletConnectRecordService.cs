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
    public class DeviceBraceletConnectRecordService : BaseService<UT_DeviceBraceletConnectRecord>, IDeviceBraceletConnectRecordService
    {
        public async Task<KeyValuePair<int, List<UT_DeviceBraceletConnectRecord>>> SearchAsync(int page, int rows, string iMEI, string tel, int? createStartDate, int? createEndDate, bool? isOnLine)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = db.UT_DeviceBraceletConnectRecord.Include(x => x.UT_Users).Where(x => true);
                if (!string.IsNullOrEmpty(tel))
                {
                    query = query.Where(x => x.UT_Users.Tel.Contains(tel));
                }
                if (!string.IsNullOrEmpty(iMEI))
                {
                    query = query.Where(x => x.IMEI.Contains(iMEI));
                }
                if (createStartDate.HasValue)
                {
                    query = query.Where(x => x.CreateDate >= createStartDate);
                }

                if (createEndDate.HasValue)
                {
                    query = query.Where(x => x.CreateDate <= createEndDate);
                }
                if (isOnLine.HasValue)
                {
                    query = query.Where(x => x.DisconnectDate.HasValue == !isOnLine);
                }

                var result = await query.OrderByDescending(x => x.CreateDate).Skip((page - 1) * rows).Take(rows).ToListAsync();

                var count = await query.CountAsync();

                return new KeyValuePair<int, List<UT_DeviceBraceletConnectRecord>>(count, result);
            }
        }

        public async Task<bool> CheckUserIdExist(Guid UserId)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                return await db.UT_DeviceBraceletConnectRecord.AnyAsync(a => a.UserId == UserId);
            }
        }
        public async Task<bool> CheckIMEIByNotUserExist(Guid UserId, string IMEI)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                return await db.UT_DeviceBraceletConnectRecord.AnyAsync(a => a.IMEI == IMEI && a.UserId != UserId);
            }
        }
    }
}
