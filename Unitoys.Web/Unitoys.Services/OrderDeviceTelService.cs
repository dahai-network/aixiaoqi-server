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
    public class OrderDeviceTelService : BaseService<UT_OrderDeviceTel>, IOrderDeviceTelService
    {
        public async Task<KeyValuePair<int, List<UT_OrderDeviceTel>>> SearchAsync(int page, int rows, string tel, DateTime? createStartDate, DateTime? createEndDate)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = db.UT_OrderDeviceTel.Where(x => true);

                if (!string.IsNullOrEmpty(tel))
                {
                    query = query.Where(x => x.Tel.Contains(tel));
                }

                //var result = await query.OrderByDescending(x => x.CreateDate).Skip((page - 1) * rows).Take(rows).ToListAsync();
                var result = await query.Skip((page - 1) * rows).Take(rows).ToListAsync();

                var count = await query.CountAsync();

                return new KeyValuePair<int, List<UT_OrderDeviceTel>>(count, result);
            }
        }
    }
}
