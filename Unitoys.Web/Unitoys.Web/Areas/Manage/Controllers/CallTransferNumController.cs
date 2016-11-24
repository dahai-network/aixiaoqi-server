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
    [RequireRolesOrPermissions(UnitoysPermissionStore.Can_View_CallTransferNum)]
    public class CallTransferNumController : BaseController
    {
        private ICallTransferNumService _callTransferNumService;

        public CallTransferNumController(ICallTransferNumService callTransferNumService)
        {
            this._callTransferNumService = callTransferNumService;
        }
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取大号列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetList(int page, int rows, string telNum, DateTime? createStartDate, DateTime? createEndDate, StatusType? status)
        {
            var pageRowsDb = await _callTransferNumService.SearchAsync(page, rows, telNum, createStartDate, createEndDate, status);

            int totalNum = pageRowsDb.Key;

            //过滤掉不必要的字段
            var pageRows = from i in pageRowsDb.Value as List<UT_CallTransferNum>
                           select new
                           {
                               ID = i.ID,
                               TelNum = i.TelNum,
                               TelPwd = i.TelPwd,
                               CreateDate = i.CreateDate.ToString(),
                               //UserId = i.UserId,
                               Tel = i.UT_Users == null ? "" : i.UT_Users.Tel
                           };

            var jsonResult = new { total = totalNum, rows = pageRows };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Add_CallTransferNum)]
        public async Task<ActionResult> Add(UT_CallTransferNum model, string TelNums)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            var TelNumsArray = TelNums.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            if (TelNums.Trim() == "" || TelNums == null)
            {
                result.Success = false;
                result.Msg = "大号不能为空！";
            }
            else if (TelNumsArray.Any(x => x.IndexOf(' ') == -1))
            {
                result.Success = false;
                result.Msg = "输入大号与密码格式错误！";
            }
            else if (TelNumsArray.Any(x => x.Split(' ')[0].Length > 12 || x.Split(' ')[1].Length > 8))
            {
                result.Success = false;
                result.Msg = "输入大号或密码长度错误！";
            }
            //else if (model.UserId == Guid.Empty)
            //{
            //    result.Success = false;
            //    result.Msg = "用户不能为空！";
            //}

            else
            {
                List<UT_CallTransferNum> list = new List<UT_CallTransferNum>();
                foreach (var info in TelNumsArray)
                {
                    string TelNum = info.Split(' ')[0];
                    string TelPwd = info.Split(' ')[1];

                    UT_CallTransferNum entity = new UT_CallTransferNum();
                    entity.TelNum = TelNum;
                    entity.TelPwd = TelPwd;
                    entity.Status = StatusType.Enable;
                    entity.CreateDate = DateTime.Now;
                    //entity.UserId = model.UserId;
                    list.Add(entity);
                }

                if (await _callTransferNumService.InsertAsync(list))
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
        /// 更新大号
        /// </summary>
        /// <param name="modal"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Modify_CallTransferNum)]
        public async Task<ActionResult> Update(UT_CallTransferNum model)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (model.TelNum.Trim() == "")
            {
                result.Success = false;
                result.Msg = "大号不能为空！";
            }
            else if (model.UserId == Guid.Empty)
            {
                result.Success = false;
                result.Msg = "用户不能为空！";
            }
            else
            {
                UT_CallTransferNum entity = await _callTransferNumService.GetEntityByIdAsync(model.ID);
                entity.TelNum = model.TelNum;
                entity.TelPwd = model.TelPwd;
                //entity.UserId = model.UserId;
                if (await _callTransferNumService.UpdateAsync(entity))
                {
                    result.Success = true;
                    result.Msg = "更新成功！";
                }
                else
                {
                    result.Success = false;
                    result.Msg = "更新失败！";
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 删除大号
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Delete_CallTransferNum)]
        public async Task<ActionResult> Delete(Guid? ID)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (ID.HasValue)
            {
                bool opResult = await _callTransferNumService.DeleteAsync(await _callTransferNumService.GetEntityByIdAsync(ID.Value));

                if (opResult)
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
    }
}
