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
    public class ZCSelectionNumberService : BaseService<UT_ZCSelectionNumber>, IZCSelectionNumberService
    {
        public async Task<KeyValuePair<int, List<UT_ZCSelectionNumber>>> SearchAsync(int page, int rows, string provinceName, string cityName, string mobileNumber, string userTel)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = db.UT_ZCSelectionNumber.Where(x => true).Include(x => x.UT_OrderByZCSelectionNumber);

                if (!string.IsNullOrEmpty(provinceName))
                {
                    query = query.Where(x => x.ProvinceName.Contains(provinceName));
                }

                if (!string.IsNullOrEmpty(cityName))
                {
                    query = query.Where(x => x.CityName.Contains(cityName));
                }

                if (!string.IsNullOrEmpty(mobileNumber))
                {
                    query = query.Where(x => x.MobileNumber.Contains(mobileNumber));
                }
                if (!string.IsNullOrEmpty(userTel))
                {
                    query = query.Where(x => x.OrderByZCSelectionNumberId != null && x.UT_OrderByZCSelectionNumber.UT_OrderByZC.UT_Users.Tel.Contains(userTel));
                }
                var result = await query.OrderByDescending(x => x.CreateDate).Skip((page - 1) * rows).Take(rows).ToListAsync();

                var count = await query.CountAsync();

                return new KeyValuePair<int, List<UT_ZCSelectionNumber>>(count, result);
            }
        }
    }
}
