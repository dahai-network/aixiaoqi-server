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
    public class EjoinDevService : BaseService<UT_EjoinDev>, IEjoinDevService
    {
        public async Task<KeyValuePair<int, List<UT_EjoinDev>>> SearchAsync(int page, int rows, string name, int? maxPort, string regIp, RegStatusType? regStatus, ModType? modType)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = db.UT_EjoinDev.Include(x => x.UT_EjoinDevSlot).Where(x => true);

                if (!string.IsNullOrEmpty(name))
                {
                    query = query.Where(x => x.Name.Contains(name));
                }

                if (maxPort.HasValue)
                {
                    query = query.Where(x => x.MaxPort == maxPort);
                }

                if (!string.IsNullOrEmpty(regIp))
                {
                    query = query.Where(x => x.RegIp.Contains(regIp));
                }

                if (regStatus.HasValue)
                {
                    query = query.Where(x => x.RegStatus == regStatus);
                }

                if (modType.HasValue)
                {
                    query = query.Where(x => x.ModType == modType);
                }

                var result = await query.OrderByDescending(x => x.RegIp).Skip((page - 1) * rows).Take(rows).ToListAsync();

                var count = await query.CountAsync();

                return new KeyValuePair<int, List<UT_EjoinDev>>(count, result);
            }
        }
    }
}
