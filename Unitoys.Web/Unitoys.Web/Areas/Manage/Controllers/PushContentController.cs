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
    [RequireRolesOrPermissions(UnitoysPermissionStore.Can_View_PushContent)]
    public class PushContentController : BaseController
    {
        private IPushContentService _pushContentService;

        public PushContentController(IPushContentService pushContentService)
        {
            this._pushContentService = pushContentService;
        }
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取PushContent列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetList(int page, int rows, string title, DateTime? createStartDate, DateTime? createEndDate)
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
            var pageRowsDb = await _pushContentService.SearchAsync(page, rows, title, beginTimeInt, endTimeInt);

            int totalNum = pageRowsDb.Key;

            //过滤掉不必要的字段
            var pageRows = from i in pageRowsDb.Value as List<UT_PushContent>
                           select new
                           {
                               ID = i.ID,
                               CreateDate = i.CreateDate.ToString(),
                               Title = i.Title,
                               Pic = i.Image
                           };

            var jsonResult = new { total = totalNum, rows = pageRows };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Add_PushContent)]
        public async Task<ActionResult> Add(UT_PushContent model)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (model.Title.Trim() == "")
            {
                result.Success = false;
                result.Msg = "Title不能为空！";
            }
            else
            {

                UT_PushContent entity = new UT_PushContent();
                entity.Title = model.Title;
                entity.Image = model.Image ?? "";
                entity.CreateDate = CommonHelper.GetDateTimeInt();

                if (await _pushContentService.InsertAsync(entity))
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
        /// 更新PushContent
        /// </summary>
        /// <param name="modal"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Modify_PushContent)]
        public async Task<ActionResult> Update(UT_PushContent model)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (model.Title.Trim() == "")
            {
                result.Success = false;
                result.Msg = "Title不能为空！";
            }
            else
            {
                UT_PushContent entity = await _pushContentService.GetEntityByIdAsync(model.ID);

                entity.Title = model.Title;
                entity.Image = model.Image ?? "";

                if (await _pushContentService.UpdateAsync(entity))
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
        /// 删除PushContent
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Delete_PushContent)]
        public async Task<ActionResult> Delete(Guid? ID)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (ID.HasValue)
            {
                bool opResult = await _pushContentService.DeleteAsync(await _pushContentService.GetEntityByIdAsync(ID.Value));

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
