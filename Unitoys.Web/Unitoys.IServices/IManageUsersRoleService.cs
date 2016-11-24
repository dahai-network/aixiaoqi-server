using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Model;

namespace Unitoys.IServices
{
    public interface IManageUsersRoleService : IBaseService<UT_ManageUsersRole>
    {
        /// <summary>
        /// 根据管理员ID获取该管理员拥有的角色
        /// </summary>
        /// <param name="userId">管理员ID</param>
        /// <returns></returns>
        Task<List<Guid>> GetRoleListByUserId(Guid userId);

        /// <summary>
        /// 修改管理员拥有的角色
        /// </summary>
        /// <param name="roleId">管理员ID</param>
        /// <param name="selectedPermissions">已选择的角色</param>
        /// <param name="unselectedPermissions">未选择的角色</param>
        /// <returns></returns>
        Task<bool> ModifyManageUserRole(Guid userId, List<Guid> selectedRoleIds, List<Guid> unselectedRoleIds);
    }
}
