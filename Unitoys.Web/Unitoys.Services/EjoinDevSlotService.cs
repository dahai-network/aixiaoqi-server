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
    public class EjoinDevSlotService : BaseService<UT_EjoinDevSlot>, IEjoinDevSlotService
    {
        public async Task<KeyValuePair<int, List<UT_EjoinDevSlot>>> SearchAsync(int page, int rows, Guid? EjoinDevId)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = db.UT_EjoinDevSlot.Include(x => x.UT_Users).Where(x => true);

                if (EjoinDevId.HasValue)
                {
                    query = query.Where(x => x.EjoinDevId == EjoinDevId);
                }

                var result = await query.OrderBy(x => x.PortNum).Skip((page - 1) * rows).Take(rows).ToListAsync();

                var count = await query.CountAsync();

                return new KeyValuePair<int, List<UT_EjoinDevSlot>>(count, result);
            }
        }
    }
}
