using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Unitoys.Core;
using Unitoys.Core.Security;
using Unitoys.IServices;

namespace Unitoys.Web.Areas.Manage.Controllers
{
    [RequireRolesOrPermissions(UnitoysPermissionStore.Can_View_ManageUserRole)]
    public class ManageUserRoleController : BaseController
    {
        private IManageUsersRoleService _manageUsersRoleService;
        public ManageUserRoleController(IManageUsersRoleService manageUsersRoleService)
        {
            this._manageUsersRoleService = manageUsersRoleService;
        }

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 根据管理员ID获取该管理员拥有的角色
        /// </summary>
        /// <param name="userId">管理员ID</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetRoleListByUserId(Guid userId)
        {
            var result = await _manageUsersRoleService.GetRoleListByUserId(userId);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改管理员拥有的角色
        /// </summary>
        /// <param name="roleId">管理员ID</param>
        /// <param name="selectedPermissions">已选择的角色</param>
        /// <param name="unselectedPermissions">未选择的角色</param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Modify_ManageUserRole)]
        public async Task<ActionResult> ModifyManageUserRole(Guid userId, List<Guid> selectedIds, List<Guid> unselectedIds)
        {
            var result = await _manageUsersRoleService.ModifyManageUserRole(userId, selectedIds, unselectedIds);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
	}
}