using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.IServices;
using Unitoys.Model;

namespace Unitoys.Services
{
    public class ManageUsersRoleService : BaseService<UT_ManageUsersRole>, IManageUsersRoleService
    {
        /// <summary>
        /// 根据管理员ID获取该管理员拥有的角色
        /// </summary>
        /// <param name="userId">管理员ID</param>
        /// <returns></returns>
        public async Task<List<Guid>> GetRoleListByUserId(Guid userId)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                List<UT_ManageUsersRole> manageUsersRole = await db.UT_ManageUsersRole.Where(x => x.ManageUserId == userId).ToListAsync();

                return manageUsersRole.Select(x => x.RoleId).ToList();
            }
        }

        /// <summary>
        /// 修改管理员拥有的角色
        /// </summary>
        /// <param name="roleId">管理员ID</param>
        /// <param name="selectedPermissions">已选择的角色</param>
        /// <param name="unselectedPermissions">未选择的角色</param>
        /// <returns></returns>
        public async Task<bool> ModifyManageUserRole(Guid userId, List<Guid> selectedRoleIds, List<Guid> unselectedRoleIds)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                //1. 获取当前管理员所有的角色列表。
                List<UT_ManageUsersRole> manageUsersRoleList = await db.UT_ManageUsersRole.Where(x => x.ManageUserId == userId).ToListAsync();

                //2. 获取当前管理员所有的角色ID集合。
                List<Guid> manageUsersRoleIds = manageUsersRoleList.Select(x => x.RoleId).ToList();

                List<Guid> includeIds = new List<Guid>();
                List<Guid> excludeIds = new List<Guid>();

                if (selectedRoleIds != null && selectedRoleIds.Count > 0)
                {
                    //3. 获取当前角色拥有的权限ID集合与已选择的权限ID集合的差集。
                    includeIds = selectedRoleIds.Except(manageUsersRoleIds).ToList();
                }

                if (unselectedRoleIds != null && unselectedRoleIds.Count > 0)
                {
                    //4. 获取当前角色拥有的权限ID集合与未选择的权限ID集合的交集。
                    excludeIds = manageUsersRoleIds.Intersect(unselectedRoleIds).ToList();
                }

                //5. 为当前角色添加/删除权限。
                foreach (var roleId in includeIds)
                {
                    UT_ManageUsersRole addedManageUsersRole = new UT_ManageUsersRole()
                    {
                        CreateDate = DateTime.Now,
                        RoleId = roleId,
                        ManageUserId = userId
                    };
                    db.UT_ManageUsersRole.Add(addedManageUsersRole);
                }

                foreach (var roleId in excludeIds)
                {
                    UT_ManageUsersRole deletedManageUsersRole = manageUsersRoleList.Single(x => x.RoleId == roleId);
                    db.UT_ManageUsersRole.Attach(deletedManageUsersRole);
                    db.Entry<UT_ManageUsersRole>(deletedManageUsersRole).State = EntityState.Deleted;
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
