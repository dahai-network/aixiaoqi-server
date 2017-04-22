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
    [RequireRolesOrPermissions(UnitoysPermissionStore.Can_View_GlobalContent)]
    public class GlobalContentController : BaseController
    {
        private IGlobalContentService _globalContentService;

        public GlobalContentController(IGlobalContentService globalContentService)
        {
            this._globalContentService = globalContentService;
        }
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取GlobalContent列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetList(int page, int rows, string name, GlobalContentType? globalContentType, DateTime? createStartDate, DateTime? createEndDate)
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
            var pageRowsDb = await _globalContentService.SearchAsync(page, rows, name, globalContentType, beginTimeInt, endTimeInt);

            int totalNum = pageRowsDb.Key;

            //过滤掉不必要的字段
            var pageRows = from i in pageRowsDb.Value as List<UT_GlobalContent>
                           select new
                           {
                               ID = i.ID,
                               CreateDate = i.CreateDate.ToString(),
                               Name = i.Name,
                               Content = Server.HtmlDecode(i.Content),
                               GlobalContentType = i.GlobalContentType
                           };

            var jsonResult = new { total = totalNum, rows = pageRows };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Add_GlobalContent)]
        public async Task<ActionResult> Add(UT_GlobalContent model)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (model.Name.Trim() == "")
            {
                result.Success = false;
                result.Msg = "Title不能为空！";
            }
            else
            {

                UT_GlobalContent entity = new UT_GlobalContent();
                entity.Name = model.Name;
                entity.Content = model.Content;
                entity.GlobalContentType = model.GlobalContentType;
                entity.CreateDate = CommonHelper.GetDateTimeInt();

                if (await _globalContentService.InsertAsync(entity))
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
        /// 更新GlobalContent
        /// </summary>
        /// <param name="modal"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Modify_GlobalContent)]
        public async Task<ActionResult> Update(UT_GlobalContent model)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (model.Name.Trim() == "")
            {
                result.Success = false;
                result.Msg = "Title不能为空！";
            }
            else
            {
                UT_GlobalContent entity = await _globalContentService.GetEntityByIdAsync(model.ID);

                entity.Name = model.Name;
                entity.Content = model.Content;
                entity.GlobalContentType = model.GlobalContentType;

                if (await _globalContentService.UpdateAsync(entity))
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
        /// 删除GlobalContent
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Delete_GlobalContent)]
        public async Task<ActionResult> Delete(Guid? ID)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (ID.HasValue)
            {
                bool opResult = await _globalContentService.DeleteAsync(await _globalContentService.GetEntityByIdAsync(ID.Value));

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
