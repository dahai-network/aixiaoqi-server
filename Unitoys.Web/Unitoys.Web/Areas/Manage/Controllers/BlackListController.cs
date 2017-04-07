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
    [RequireRolesOrPermissions(UnitoysPermissionStore.Can_View_BlackList)]
    public class BlackListController : BaseController
    {
        private IBlackListService _blackListService;

        public BlackListController(IBlackListService blackListService)
        {
            this._blackListService = blackListService;
        }
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取BlackList列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetList(int page, int rows, string blackNum, string tel, DateTime? createStartDate, DateTime? createEndDate)
        {
            int? beginTimeInt = null;
            int? endTimeInt = null;
            if (createStartDate.HasValue)
            {
                beginTimeInt = CommonHelper.ConvertDateTimeInt(createStartDate.Value);
            }
            if (endTimeInt.HasValue)
            {
                endTimeInt = CommonHelper.ConvertDateTimeInt(createEndDate.Value);
            }
            var pageRowsDb = await _blackListService.SearchAsync(page, rows, blackNum, tel, beginTimeInt, endTimeInt);

            int totalNum = pageRowsDb.Key;

            //过滤掉不必要的字段
            var pageRows = from i in pageRowsDb.Value as List<UT_BlackList>
                           select new
                           {
                               ID = i.ID,
                               CreateDate = i.CreateDate.ToString(),
                               BlackNum = i.BlackNum,
                               Tel = i.UT_Users == null ? "" : i.UT_Users.Tel,
                           };

            var jsonResult = new { total = totalNum, rows = pageRows };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Add_BlackList)]
        public async Task<ActionResult> Add(UT_BlackList model)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (model.BlackNum.Trim() == "")
            {
                result.Success = false;
                result.Msg = "BlackNum不能为空！";
            }
            else
            {

                UT_BlackList entity = new UT_BlackList();
                entity.BlackNum = model.BlackNum;
                entity.CreateDate = CommonHelper.GetDateTimeInt();

                if (await _blackListService.InsertAsync(entity))
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
        /// 更新BlackList
        /// </summary>
        /// <param name="modal"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Modify_BlackList)]
        public async Task<ActionResult> Update(UT_BlackList model)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (model.BlackNum.Trim() == "")
            {
                result.Success = false;
                result.Msg = "BlackNum不能为空！";
            }
            else
            {
                UT_BlackList entity = await _blackListService.GetEntityByIdAsync(model.ID);
                entity.BlackNum = model.BlackNum;

                if (await _blackListService.UpdateAsync(entity))
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
        /// 删除BlackList
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Delete_BlackList)]
        public async Task<ActionResult> Delete(Guid? ID)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (ID.HasValue)
            {
                bool opResult = await _blackListService.DeleteAsync(await _blackListService.GetEntityByIdAsync(ID.Value));

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
                result.Msg = "参数错误！";
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}
