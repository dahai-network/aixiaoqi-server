using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
    [RequireRolesOrPermissions(UnitoysPermissionStore.Can_View_ManageUser)]
    public class ManageUserController : BaseController
    {
        private IManageUserService _manageUserService;

        public ManageUserController() { }

        public ManageUserController(IManageUserService manageUserService)
        {
            this._manageUserService = manageUserService;
        }
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取管理员列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetList(int page,int rows)
        {
            var pageRowsDb = await _manageUserService.GetEntitiesForPagingAsync("UT_ManageUsers", page, rows, "CreateDate", "desc", "1=1");

            int totalNum = pageRowsDb.Key;

            //过滤掉不必要的字段
            var pageRows = from i in pageRowsDb.Value as List<UT_ManageUsers>
                       select new
                       {
                           ID=i.ID,
                           LoginName = i.LoginName,
                           TrueName = i.TrueName,
                           Lock4 = i.Lock4,
                           CreateDate = i.CreateDate.ToString()
                       };

             var jsonResult = new { total = totalNum, rows = pageRows };

            return Json(jsonResult,JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加管理员
        /// </summary>
        /// <param name="LoginName"></param>
        /// <param name="PassWord"></param>
        /// <param name="TrueName"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Add_ManageUser)]
        public async Task<ActionResult> Add(string loginName,string passWord,string trueName)
        {
            JsonAjaxResult result = new JsonAjaxResult();
            if (loginName.Trim() == "")
            {
                result.Success = false;
                result.Msg = "用户名不能为空！";
            }
            else if (passWord.Trim() == "")
            {
                result.Success = false;
                result.Msg = "密码不能为空！";
            }
            else if (trueName.Trim() == "")
            {
                result.Success = false;
                result.Msg = "真实姓名不能为空！";
            }
            else
            {

                UT_ManageUsers manageUser = new UT_ManageUsers();
                manageUser.LoginName = loginName;
                manageUser.PassWord = SecureHelper.MD5(passWord);
                manageUser.TrueName = trueName;
                manageUser.Lock4 = 0;
                manageUser.CreateDate = DateTime.Now;

                if (await _manageUserService.InsertAsync(manageUser))
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
        /// 更新管理员信息
        /// </summary>
        /// <param name="LoginName"></param>
        /// <param name="PassWord"></param>
        /// <param name="TrueName"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Modify_ManageUser)]        
        public async Task<ActionResult> Update(Guid ID, string passWord, string trueName)
        {
            JsonAjaxResult result = new JsonAjaxResult();
            if (trueName.Trim() == "")
            {
                result.Success = false;
                result.Msg = "真实姓名不能为空！";
            }
            else
            {
                UT_ManageUsers manageUser = await _manageUserService.GetEntityByIdAsync(ID);
                if (passWord.Trim() != "")
                {
                    manageUser.PassWord = SecureHelper.MD5(passWord);
                }
                manageUser.TrueName = trueName;

                if (await _manageUserService.UpdateAsync(manageUser))
                {
                    result.Success = true;
                    result.Msg = "更新成功！";
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
        /// 删除管理员
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Delete_ManageUser)]
        public async Task<ActionResult> Delete(Guid? ID)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (ID.HasValue)
            {
                if (await _manageUserService.DeleteAsync(await _manageUserService.GetEntityByIdAsync(ID.Value)))
                {
                    result.Success = true;
                    result.Msg = "删除成功！";
                }
                else
                {
                    result.Success = false;
                    result.Msg = "删除失败！";
                }
            }
            else
            {
                result.Success = false;
                result.Msg = "次数错误！";
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取所有管理员
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetAllManageUser()
        {
            var result = await _manageUserService.GetAll();

            //过滤掉不必要的字段
            var pageRows = from i in result
                           select new
                           {
                               ID = i.ID,
                               LoginName = i.LoginName,
                               TrueName = i.TrueName
                           };

            return Json(pageRows, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 锁定操作
        /// </summary>
        /// <param name="LoginName"></param>
        /// <param name="PassWord"></param>
        /// <param name="TrueName"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Modify_ManageUser)]
        public async Task<ActionResult> Lock(Guid? ID)
        {
            JsonAjaxResult result = new JsonAjaxResult();
            if (ID.HasValue)
            {

                UT_ManageUsers entity = await _manageUserService.GetEntityByIdAsync(ID.Value);
                if (entity.Lock4 != 1)
                {
                    entity.Lock4 = 1;
                    if (await _manageUserService.UpdateAsync(entity))
                    {
                        result.Success = true;
                        result.Msg = "锁定操作成功！";
                    }
                    else
                    {
                        result.Success = false;
                        result.Msg = "锁定操作失败！";
                    }
                }
                else
                {
                    result.Success = false;
                    result.Msg = "该管理员已经是锁定状态！";
                }
            }
            else
            {
                result.Success = false;
                result.Msg = "请求失败！";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 解除锁定操作
        /// </summary>
        /// <param name="LoginName"></param>
        /// <param name="PassWord"></param>
        /// <param name="TrueName"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Modify_ManageUser)]
        public async Task<ActionResult> UnLock(Guid? ID)
        {
            JsonAjaxResult result = new JsonAjaxResult();
            if (ID.HasValue)
            {

                UT_ManageUsers entity = await _manageUserService.GetEntityByIdAsync(ID.Value);
                if (entity.Lock4 != 0)
                {
                    entity.Lock4 = 0;
                    if (await _manageUserService.UpdateAsync(entity))
                    {
                        result.Success = true;
                        result.Msg = "取消锁定操作成功！";
                    }
                    else
                    {
                        result.Success = false;
                        result.Msg = "取消锁定操作失败！";
                    }
                }
                else
                {
                    result.Success = false;
                    result.Msg = "该管理员已经是未锁定状态！";
                }
            }
            else
            {
                result.Success = false;
                result.Msg = "请求失败！";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
