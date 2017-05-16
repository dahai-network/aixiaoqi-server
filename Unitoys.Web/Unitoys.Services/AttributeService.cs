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
    public class AttributeService : BaseService<UT_Attribute>, IAttributeService
    {
        //public async Task<KeyValuePair<int, List<UT_Attribute>>> SearchAsync(int page, int rows, string tel)
        //{
        //    using (UnitoysEntities db = new UnitoysEntities())
        //    {
        //        var query = db.UT_Attribute.Include(x => x.UT_Users).Where(x => true);
        //        if (!string.IsNullOrEmpty(tel))
        //        {
        //            query = query.Where(x => x.UT_Users.Tel.Contains(tel));
        //        }
        //        if (createStartDate.HasValue)
        //        {
        //            query = query.Where(x => x.CreateDate >= createStartDate);
        //        }

        //        if (createEndDate.HasValue)
        //        {
        //            query = query.Where(x => x.CreateDate <= createEndDate);
        //        }

        //        var result = await query.OrderByDescending(x => x.CreateDate).Skip((page - 1) * rows).Take(rows).ToListAsync();

        //        var count = await query.CountAsync();

        //        return new KeyValuePair<int, List<UT_Attribute>>(count, result);
        //    }
        //}
    }
}
