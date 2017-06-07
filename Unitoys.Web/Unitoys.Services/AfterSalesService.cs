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
    public class AfterSalesService : BaseService<UT_AfterSales>, IAfterSalesService
    {
        public async Task<KeyValuePair<int, List<UT_AfterSales>>> SearchAsync(int page, int rows, string name, string mobilePhoneAfterSalesNum, DeviceType? ProductModel, int? createStartDate, int? createEndDate)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = db.UT_AfterSales.Where(x => true);

                if (!string.IsNullOrEmpty(mobilePhoneAfterSalesNum))
                {
                    query = query.Where(x => x.MobilePhone.Contains(mobilePhoneAfterSalesNum) || x.AfterSalesNum.Contains(mobilePhoneAfterSalesNum));
                }

                if (ProductModel.HasValue)
                {
                    query = query.Where(x => x.ProductModel == ProductModel);
                }

                if (!string.IsNullOrEmpty(name))
                {
                    query = query.Where(x => x.Contact.Contains(name));
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

                return new KeyValuePair<int, List<UT_AfterSales>>(count, result);
            }
        }
    }
}
