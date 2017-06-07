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
    public class AgentService : BaseService<UT_Agent>, IAgentService
    {
        public async Task<KeyValuePair<int, List<UT_Agent>>> SearchAsync(int page, int rows, string companyName, int? createStartDate, int? createEndDate)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = db.UT_Agent.Where(x => true);
                if (!string.IsNullOrEmpty(companyName))
                {
                    query = query.Where(x => x.CompanyName.Contains(companyName));
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

                return new KeyValuePair<int, List<UT_Agent>>(count, result);
            }
        }
    }
}
