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
    public class UserShapeService : BaseService<UT_UserShape>, IUserShapeService
    {
        UnitoysEntities db = new UnitoysEntities();
        public async Task<UT_UserShape> GetUserShapeAsync(Guid UserId)
        {
            return await db.UT_UserShape.Where(a => a.UserId == UserId).OrderByDescending(x=>x.CreateDate).FirstOrDefaultAsync();
        }
    }
}
