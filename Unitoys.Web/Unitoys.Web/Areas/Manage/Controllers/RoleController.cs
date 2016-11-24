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
    [RequireRolesOrPermissions(UnitoysPermissionStore.Can_View_Role)]
    public class RoleController : BaseController
    {
        private IRoleService _roleService;
        public RoleController() { }
        public RoleController(IRoleService roleService)
        {
            this._roleService = roleService;
        }

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetRoleList(int page, int rows)
        {
            var result = await _roleService.GetEntitiesForPagingAsync("UT_Role", page, rows, "CreateDate", "desc", "1=1");

            int totalNum = result.Key;

            //过滤掉不必要的字段
            var pageRows = from i in result.Value as List<UT_Role>
                           select new
                           {
                               ID = i.ID,
                               CreateDate = i.CreateDate.ToString(),
                               Name = i.Name,
                               Description = i.Description
                           };

            var jsonResult = new { total = totalNum, rows = pageRows };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取所有角色
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetAllRole()
        {
            var result = await _roleService.GetAll();

            //过滤掉不必要的字段
            var pageRows = from i in result
                           select new
                           {
                               ID = i.ID,
                               Name = i.Name,
                               Description = i.Description
                           };

            return Json(pageRows, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="roleName">角色名称</param>
        /// <param name="description">角色描述</param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Add_Role)]
        public async Task<ActionResult> AddRole(string roleName, string description)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if(string.IsNullOrEmpty(roleName))
            {
                result.Success = false;
                result.Msg = "角色名称不能为空！";
            }
            else if(roleName.Length > 50)
            {
                result.Success = false;
                result.Msg = "角色名称不能长于50字符！";
            }
            else if(await _roleService.GetEntitiesCountAsync(x => x.Name == roleName) > 0)
            {
                result.Success = false;
                result.Msg = "该角色名称已存在！";
            }
            else
            {
                UT_Role role = new UT_Role()
                {
                    Name = roleName,
                    Description = description,
                    CreateDate = DateTime.Now
                };

                if(await _roleService.InsertAsync(role))
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
        /// 删除角色
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Delete_Role)]
        public async Task<ActionResult> DeleteRole(Guid roleId)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if(roleId == null || roleId == Guid.Empty)
            {
                result.Success = false;
                result.Msg = "角色ID不能为空！";
            }
            else
            {
                UT_Role role = await _roleService.GetEntityByIdAsync(roleId);

                if(role == null)
                {
                    result.Success = false;
                    result.Msg = "此角色不存在！";
                }
                else
                {
                    if(await _roleService.DeleteAsync(role))
                    {
                        result.Success = true;
                        result.Msg = "删除成功！";
                    }
                    else
                    {
                        result.Success = false;
                        result.Msg = "角色与用户已经建立关系，不能删除！";
                    }
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
	}
}