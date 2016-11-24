using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Model;

namespace Unitoys.IServices
{
    public interface IRolePermissionService : IBaseService<UT_RolePermission>
    {
        /// <summary>
        /// 根据角色ID获取该角色拥有的权限
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <returns></returns>
        Task<List<Guid>> GetPermissionIdsByRoleId(Guid roleId);

        /// <summary>
        /// 修改角色拥有的权限
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <param name="selectedPermissions">已选择的权限</param>
        /// <param name="unselectedPermissions">未选择的权限</param>
        /// <returns></returns>
        Task<bool> ModifyRolePermissions(Guid roleId, List<Guid> selectedPermissionIds, List<Guid> unselectedPermissionIds);
    }
}
