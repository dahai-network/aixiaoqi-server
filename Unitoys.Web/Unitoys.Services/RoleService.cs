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
    public class RoleService : BaseService<UT_Role>, IRoleService
    {
        /// <summary>
        /// Override删除角色方法
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override async Task<bool> DeleteAsync(UT_Role entity)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                if (entity != null)
                {      
                    //判断该角色是否与管理员建立关系，如果建立关系则不能删除。
                    if(await db.UT_ManageUsersRole.CountAsync(x => x.RoleId == entity.ID) > 0)
                    {
                        return false;
                    }
                    db.UT_Role.Attach(entity);
                    db.Entry<UT_Role>(entity).State = EntityState.Deleted;

                    return await db.SaveChangesAsync() > 0;
                }
                return false;
            }
        }
    }
}
