using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.IServices;
using Unitoys.Core;
using Unitoys.Model;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Reflection;

namespace Unitoys.Services
{
    public class DeviceBraceletUsageRecordService : BaseService<UT_DeviceBraceletUsageRecord>, IDeviceBraceletUsageRecordService
    {
        public async Task<KeyValuePair<int, List<UT_DeviceBraceletUsageRecord>>> SearchAsync(int page, int rows, string iMEI)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = db.UT_DeviceBraceletUsageRecord.Include(x => x.UT_Users).Where(x => true);
                if (!string.IsNullOrEmpty(iMEI))
                {
                    query = query.Where(x => x.IMEI.Contains(iMEI));
                }

                query = query.OrderByDescending(x => x.CreateDate);

                var result = await query.Skip((page - 1) * rows).Take(rows).ToListAsync();

                var count = await query.CountAsync();

                return new KeyValuePair<int, List<UT_DeviceBraceletUsageRecord>>(count, result);
            }
        }
    }
}
