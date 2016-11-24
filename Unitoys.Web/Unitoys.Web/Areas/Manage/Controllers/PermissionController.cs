using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Unitoys.Core;
using Unitoys.Core.Security;
using Unitoys.IServices;
using Unitoys.Model;
using Unitoys.Web.Models;

namespace Unitoys.Web.Areas.Manage.Controllers
{
    [RequireRolesOrPermissions(UnitoysPermissionStore.Can_View_Permission)]
    public class PermissionController : BaseController
    {
        private IPermissionService _permissionService;
        public PermissionController(IPermissionService permissionService)
        {
            this._permissionService = permissionService;
        }

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取权限列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetPermissionList(int page, int rows)
        {
            var resultTask = _permissionService.GetEntitiesForPagingAsync(page, rows, x => x.DisplayOrder.ToString(), "asc", x => true);

            var countTask = _permissionService.GetEntitiesCountAsync(x => true);

            //过滤掉不必要的字段
            var pageRows = from i in await resultTask
                           select new
                           {
                               ID = i.ID,
                               CreateDate = i.CreateDate.ToString(),
                               Name = i.Name,
                               Description = i.Description
                           };

            var jsonResult = new { total = await countTask, rows = pageRows };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取全部权限
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetAllPermission()
        {
            var result = await _permissionService.GetAll();

            //排序
            result = result.OrderByDescending(x => x.DisplayOrder);

            //过滤掉不必要的字段
            var pageRows = from i in result
                           select new
                           {
                               ID = i.ID,                               
                               Name = i.Name,
                               Description = i.Description,
                               DisplayOrder = i.DisplayOrder
                           };

            return Json(pageRows, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加权限
        /// </summary>
        /// <param name="roleName">权限名称</param>
        /// <param name="description">权限描述</param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Add_Permission)]
        public async Task<ActionResult> AddPermission(string permissionName, string description, int displayOrder)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (string.IsNullOrEmpty(permissionName))
            {
                result.Success = false;
                result.Msg = "权限名称不能为空！";
            }
            else if (permissionName.Length > 50)
            {
                result.Success = false;
                result.Msg = "权限名称不能长于50字符！";
            }
            else if (displayOrder <= 0)
            {
                result.Success = false;
                result.Msg = "排序号不能小于等于0！";
            }
            else if (await _permissionService.GetEntitiesCountAsync(x => x.Name == permissionName) > 0)
            {
                result.Success = false;
                result.Msg = "该权限名称已存在！";
            }
            else
            {
                UT_Permission permission = new UT_Permission()
                {
                    Name = permissionName,
                    Description = description,
                    DisplayOrder = displayOrder,
                    CreateDate = DateTime.Now
                };

                if (await _permissionService.InsertAsync(permission))
                {
                    result.Success = true;
                    result.Msg = "添加成功！";
                }
                else
                {
                    result.Success = false;
                    result.Msg = "操作失败！";
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="roleId">权限ID</param>
        /// <returns></returns>
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Delete_Permission)]
        [HttpPost]
        public async Task<ActionResult> DeletePermission(Guid roleId)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (roleId == null || roleId == Guid.Empty)
            {
                result.Success = false;
                result.Msg = "权限ID不能为空！";
            }
            else
            {
                UT_Permission permission = await _permissionService.GetEntityByIdAsync(roleId);

                if (permission == null)
                {
                    result.Success = false;
                    result.Msg = "此权限不存在！";
                }
                else
                {
                    if (await _permissionService.DeleteAsync(permission))
                    {
                        result.Success = true;
                        result.Msg = "删除成功！";
                    }
                    else
                    {
                        result.Success = false;
                        result.Msg = "权限与角色已经建立关系，不能删除！";
                    }
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
	}
}