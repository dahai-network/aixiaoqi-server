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
    public class PermissionService : BaseService<UT_Permission>, IPermissionService
    {
        /// <summary>
        /// Override删除权限方法
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override async Task<bool> DeleteAsync(UT_Permission entity)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                if (entity != null)
                {
                    //判断该权限是否与角色建立关系，如果建立关系则不能删除。
                    if (await db.UT_RolePermission.CountAsync(x => x.PermissionId == entity.ID) > 0)
                    {
                        return false;
                    }
                    db.UT_Permission.Attach(entity);
                    db.Entry<UT_Permission>(entity).State = EntityState.Deleted;

                    return await db.SaveChangesAsync() > 0;
                }
                return false;
            }
        }
    }
}
