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
    public class RolePermissionService : BaseService<UT_RolePermission>, IRolePermissionService
    {
        /// <summary>
        /// 根据角色ID获取该角色拥有的权限
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <returns></returns>
        public async Task<List<Guid>> GetPermissionIdsByRoleId(Guid roleId)
        {
            using(UnitoysEntities db = new UnitoysEntities())
            {
                List<UT_RolePermission> rolePermissionList = await db.UT_RolePermission.Where(x => x.RoleId == roleId).ToListAsync();

                return rolePermissionList.Select(x => x.PermissionId).ToList();
            }
        }

        /// <summary>
        /// 修改角色拥有的权限
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <param name="selectedPermissions">已选择的权限</param>
        /// <param name="unselectedPermissions">未选择的权限</param>
        /// <returns></returns>
        public async Task<bool> ModifyRolePermissions(Guid roleId, List<Guid> selectedPermissionIds, List<Guid> unselectedPermissionIds)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                //1. 获取当前角色所有的权限列表。
                List<UT_RolePermission> rolePermissionList = await db.UT_RolePermission.Where(x => x.RoleId == roleId).ToListAsync();

                //2. 获取当前角色所有的权限ID集合。
                List<Guid> rolePermissionIds = rolePermissionList.Select(x => x.PermissionId).ToList();

                List<Guid> includeIds = new List<Guid>();
                List<Guid> excludeIds = new List<Guid>();

                if (selectedPermissionIds != null && selectedPermissionIds.Count > 0)
                {
                    //3. 获取当前角色拥有的权限ID集合与已选择的权限ID集合的差集。
                    includeIds = selectedPermissionIds.Except(rolePermissionIds).ToList();
                }
                
                if(unselectedPermissionIds != null && unselectedPermissionIds.Count > 0)
                {
                    //4. 获取当前角色拥有的权限ID集合与未选择的权限ID集合的交集。
                    excludeIds = rolePermissionIds.Intersect(unselectedPermissionIds).ToList();
                }
                
                //5. 为当前角色添加/删除权限。
                foreach (var permissionId in includeIds)
                {
                    UT_RolePermission addedRolePermission = new UT_RolePermission()
                    {
                        CreateDate = DateTime.Now,
                        RoleId = roleId,
                        PermissionId = permissionId
                    };
                    db.UT_RolePermission.Add(addedRolePermission);
                }

                foreach (var permissionId in excludeIds)
                {
                    UT_RolePermission deletedRolePermission = rolePermissionList.Single(x => x.PermissionId == permissionId);
                    db.UT_RolePermission.Attach(deletedRolePermission);
                    db.Entry<UT_RolePermission>(deletedRolePermission).State = EntityState.Deleted;
                }

                if (includeIds.Count == 0 && excludeIds.Count == 0)
                {
                    return true;
                }

                return await db.SaveChangesAsync() > 0;
            }
        }
    }
}
