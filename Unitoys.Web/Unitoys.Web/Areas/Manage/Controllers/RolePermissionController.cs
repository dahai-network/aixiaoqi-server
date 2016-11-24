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
    [RequireRolesOrPermissions(UnitoysPermissionStore.Can_View_RolePermission)]
    public class RolePermissionController : BaseController
    {
        private IRolePermissionService _rolePermissionService;
        public RolePermissionController(IRolePermissionService rolePermissionService)
        {
            this._rolePermissionService = rolePermissionService;
        }

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 根据角色ID获取该角色拥有的权限
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetPermissionListByRoleId(Guid roleId)
        {
            var result = await _rolePermissionService.GetPermissionIdsByRoleId(roleId);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改角色拥有的权限
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <param name="selectedPermissions">已选择的权限</param>
        /// <param name="unselectedPermissions">未选择的权限</param>
        /// <returns></returns>        
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Modify_RolePermission)]
        public async Task<ActionResult> ModifyRolePermissions(Guid roleId, List<Guid> selectedIds, List<Guid> unselectedIds)
        {
            var result = await _rolePermissionService.ModifyRolePermissions(roleId, selectedIds, unselectedIds);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
	}
}