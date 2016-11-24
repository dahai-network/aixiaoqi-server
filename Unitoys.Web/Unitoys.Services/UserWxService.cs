using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.IServices;
using Unitoys.Core;
using Unitoys.Model;
using System.Data.Entity;
using System.Diagnostics;

namespace Unitoys.Services
{
    public class UserWxService : BaseService<UT_UsersWx>, IUserWxService
    {
        UnitoysEntities db = new UnitoysEntities();

        public bool CheckOpenIdExist(string openId)
        {
            return db.UT_UsersWx.Any(a => a.OpenId == openId);
        }

        public bool CheckUserBindWx(Guid Id)
        {
            return db.UT_UsersWx.Any(a => a.UserId == Id);
        }
    }
}
