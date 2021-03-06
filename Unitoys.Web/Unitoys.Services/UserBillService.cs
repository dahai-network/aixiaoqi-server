﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.IServices;
using Unitoys.Model;
using Unitoys.Core;
using System.Data.Entity;

namespace Unitoys.Services
{
    public class UserBillService : BaseService<UT_UserBill>, IUserBillService
    {
        public async Task<KeyValuePair<int, List<UT_UserBill>>> SearchAsync(int page, int rows, string loginName, int? createStartDate, int? createEndDate, int? billType)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = db.UT_UserBill.Where(x => true);

                if (!string.IsNullOrEmpty(loginName))
                {
                    query = query.Where(x => x.LoginName.Contains(loginName));
                }

                if (billType.HasValue)
                {
                    query = query.Where(x => x.BillType == billType);
                }

                if (createStartDate != null)
                {
                    query = query.Where(x => x.CreateDate >= createStartDate);
                }

                if (createEndDate != null)
                {
                    query = query.Where(x => x.CreateDate <= createEndDate);
                }

                var result = await query.OrderByDescending(x => x.CreateDate).Skip((page - 1) * rows).Take(rows).ToListAsync();

                var count = await query.CountAsync();

                return new KeyValuePair<int, List<UT_UserBill>>(count, result);
            }
        }
    }
}
