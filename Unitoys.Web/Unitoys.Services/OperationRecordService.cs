using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.IServices;
using Unitoys.Model;
using System.Data.Entity;
using Unitoys.Core;

namespace Unitoys.Services
{
    public class OperationRecordService : BaseService<UT_OperationRecord>, IOperationRecordService
    {
        public async Task<KeyValuePair<int, List<UT_OperationRecord>>> SearchAsync(int page, int rows, string url, string managerLoginName, int? createStartDate, int? createEndDate)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = db.UT_OperationRecord.Include(x => x.UT_ManageUsers).Where(x => true);

                if (!string.IsNullOrEmpty(managerLoginName))
                {
                    query = query.Where(x => x.UT_ManageUsers.LoginName.Contains(managerLoginName));
                }

                if (!string.IsNullOrEmpty(managerLoginName))
                {
                    query = query.Where(x => x.UT_ManageUsers.LoginName.Contains(managerLoginName));
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

                return new KeyValuePair<int, List<UT_OperationRecord>>(count, result);
            }
        }
    }
}
