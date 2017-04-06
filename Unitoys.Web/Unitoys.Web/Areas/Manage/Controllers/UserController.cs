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
    [RequireRolesOrPermissions(UnitoysPermissionStore.Can_View_User)]
    public class UserController : BaseController
    {
        private IUserService _userService;

        public UserController() { }

        public UserController(IUserService userService)
        {
            this._userService = userService;
        }

        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetList(int page, int rows, string tel, DateTime? createStartDate, DateTime? createEndDate, int? status)
        {
            var pageRowsDb = await _userService.SearchAsync(page, rows, tel, createStartDate, createEndDate, status);

            int totalNum = pageRowsDb.Key;

            //过滤掉不必要的字段
            var pageRows = from i in pageRowsDb.Value as List<UT_Users>
                           select new
                           {
                               ID = i.ID,
                               Tel = i.Tel,
                               Email = i.Email,
                               TrueName = i.TrueName,
                               QQ = i.QQ,
                               CreateDate = i.CreateDate.ToString(),
                               Status = i.Status,
                               Score = i.Score,
                               Sex = i.Sex,
                               Amount = i.Amount,
                               Remark = i.Remark
                           };

            var jsonResult = new { total = totalNum, rows = pageRows };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取所有用户列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public async Task<ActionResult> GetSelectList()
        {
            Expression<Func<UT_Users, bool>> exp = a => 1 == 1;

            var pageRowsDb = await _userService.GetEntitiesAsync(exp);

            //过滤掉不必要的字段
            var pageRows = from i in pageRowsDb
                           select new
                           {
                               ID = i.ID,
                               Tel = i.Tel
                           };

            var jsonResult = new { pageRows };

            return Json(pageRows, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="passWord"></param>
        /// <param name="trueName"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Add_User)]
        public async Task<ActionResult> Add(string passWord, string tel, string email = "", string trueName = "")
        {
            JsonAjaxResult result = new JsonAjaxResult();
            if (passWord.Trim() == "" || passWord.Length < 6)
            {
                result.Success = false;
                result.Msg = "密码不符合规范！";
            }
            else if (!ValidateHelper.IsMobile(tel))
            {
                result.Success = false;
                result.Msg = "电话号码不正确！";
            }
            else if (_userService.CheckTelExist(tel))
            {
                result.Success = false;
                result.Msg = "该号码已绑定其他用户！";
            }
            else
            {

                UT_Users user = new UT_Users();
                user.PassWord = SecureHelper.MD5(passWord);
                user.Tel = tel;
                user.Email = email;
                user.TrueName = trueName;
                user.Score = 0;
                user.Amount = 0;
                user.GroupId = Guid.Parse("688a3245-2628-4488-bf35-9c029ff80988");
                user.Status = 0;
                user.CreateDate = DateTime.Now;
                user.UserHead = "/Unitoys/2015/12/1512291755292460937.png";

                switch (Unitoys.WebApi.Controllers.PhoneServerByMySqlServices.SetSip_Buddies(user.Tel))
                {
                    case 2:
                        result.Success = true;
                        result.Msg = "系统繁忙_请重试！";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    //return Ok(new StatusCodeRes(StatusCodeType.系统繁忙_请重试));
                    case 0:
                        result.Success = true;
                        result.Msg = "注册失败_请重试！";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    //return Ok(new StatusCodeRes(StatusCodeType.注册失败_请重试));
                }

                if (await _userService.InsertAsync(user))
                {
                    //默认运动目标8000
                    if (await _userService.ModifyUserInfoAndUserShape(user.ID, null, null, null, null, null, 8000))
                    {

                    }
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
        /// 锁定用户
        /// </summary>
        /// <param name="LoginName"></param>
        /// <param name="PassWord"></param>
        /// <param name="TrueName"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Modify_User)]
        public async Task<ActionResult> Lock(Guid? ID)
        {
            JsonAjaxResult result = new JsonAjaxResult();
            if (ID.HasValue)
            {

                UT_Users user = await _userService.GetEntityByIdAsync(ID.Value);
                if (user.Status != 1)
                {
                    user.Status = 1;
                    if (await _userService.UpdateAsync(user))
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
                    result.Msg = "该用户已经是锁定状态！";
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
        /// 解除用户锁定状态
        /// </summary>
        /// <param name="LoginName"></param>
        /// <param name="PassWord"></param>
        /// <param name="TrueName"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Modify_User)]
        public async Task<ActionResult> uLock(Guid? ID)
        {
            JsonAjaxResult result = new JsonAjaxResult();
            if (ID.HasValue)
            {
                UT_Users user = await _userService.GetEntityByIdAsync(ID.Value);
                if (user.Status != 0)
                {
                    user.Status = 0;
                    if (await _userService.UpdateAsync(user))
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
                else
                {
                    result.Success = false;
                    result.Msg = "该用户已经是正常状态！";
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
        /// 充值
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Modify_User)]
        public async Task<ActionResult> recharge(Guid? ID, decimal price)
        {
            JsonAjaxResult result = new JsonAjaxResult();
            if (ID.HasValue)
            {
                if (await _userService.Recharge(ID.Value, price))
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
            else
            {
                result.Success = false;
                result.Msg = "请求失败！";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 充值
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Modify_User)]
        public async Task<ActionResult> setRemark(Guid? ID, string remark)
        {
            JsonAjaxResult result = new JsonAjaxResult();
            if (ID.HasValue)
            {
                UT_Users user = await _userService.GetEntityByIdAsync(ID.Value);
                user.Remark = remark;
                if (await _userService.UpdateAsync(user))
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
            else
            {
                result.Success = false;
                result.Msg = "请求失败！";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
