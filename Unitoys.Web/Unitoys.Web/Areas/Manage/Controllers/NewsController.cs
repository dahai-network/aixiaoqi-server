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
    [RequireRolesOrPermissions(UnitoysPermissionStore.Can_View_News)]
    public class NewsController : BaseController
    {
        private INewsService _newsService;

        public NewsController(INewsService newsService)
        {
            this._newsService = newsService;
        }
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取News列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetList(int page, int rows, string title, string publisher, DateTime? createStartDate, DateTime? createEndDate)
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
            var pageRowsDb = await _newsService.SearchAsync(page, rows, title, publisher, beginTimeInt, endTimeInt);

            int totalNum = pageRowsDb.Key;

            //过滤掉不必要的字段
            var pageRows = from i in pageRowsDb.Value as List<UT_News>
                           select new
                           {
                               ID = i.ID,
                               CreateDate = i.CreateDate.ToString(),
                               Title = i.Title,
                               Image = i.Image,
                               NewsDate = i.NewsDate.ToString(),
                               Publisher = i.Publisher,
                               Content = Server.HtmlDecode(i.Content),
                               NewsType = i.NewsType,
                               IsTop = i.IsTop
                           };

            var jsonResult = new { total = totalNum, rows = pageRows };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Add_News)]
        public async Task<ActionResult> Add(UT_News model)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (model.Title.Trim() == "")
            {
                result.Success = false;
                result.Msg = "Title不能为空！";
            }
            else
            {

                UT_News entity = new UT_News();
                entity.Title = model.Title;
                entity.Image = model.Image;
                entity.NewsDate = model.NewsDate;
                entity.Publisher = model.Publisher;
                entity.Content = model.Content;
                entity.NewsType = model.NewsType;
                entity.IsTop = model.IsTop;
                entity.CreateDate = CommonHelper.GetDateTimeInt();


                if (await _newsService.InsertAsync(entity))
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
        /// 更新News
        /// </summary>
        /// <param name="modal"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Modify_News)]
        public async Task<ActionResult> Update(UT_News model)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (model.Title.Trim() == "")
            {
                result.Success = false;
                result.Msg = "Title不能为空！";
            }
            else
            {
                UT_News entity = await _newsService.GetEntityByIdAsync(model.ID);

                entity.Title = model.Title;
                entity.Image = model.Image;
                entity.NewsDate = model.NewsDate;
                entity.Publisher = model.Publisher;
                entity.Content = model.Content;
                entity.NewsType = model.NewsType;
                entity.IsTop = model.IsTop;

                if (await _newsService.UpdateAsync(entity))
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
        /// 删除News
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Delete_News)]
        public async Task<ActionResult> Delete(Guid? ID)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (ID.HasValue)
            {
                bool opResult = await _newsService.DeleteAsync(await _newsService.GetEntityByIdAsync(ID.Value));

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
