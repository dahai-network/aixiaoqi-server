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
    [RequireRolesOrPermissions(UnitoysPermissionStore.Can_View_PageShow)]
    public class PageShowController : BaseController
    {
        private IPageShowService _pageShowService;

        public PageShowController(IPageShowService pageShowService)
        {
            this._pageShowService = pageShowService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<ActionResult> P(string id)
        {
            UT_PageShow model = null;

            Guid ID;
            if (Guid.TryParse(id, out ID))
            {
                model = await _pageShowService.GetEntityByIdAsync(ID);
            }
            else
            {
                model = await _pageShowService.GetEntityAsync(x => x.EntryName == id);
            }
            if (model != null)
            {
                ViewBag.Content = Server.HtmlDecode(model.Content);
                return View("Show");
            }
            else
            {
                return Content("内容不存在");
            }
        }

        /// <summary>
        /// 获取设备手环列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetList(int page, int rows, string name, string entryName, DateTime? createStartDate, DateTime? createEndDate)
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
            var pageRowsDb = await _pageShowService.SearchAsync(page, rows, name, entryName, beginTimeInt, endTimeInt);

            int totalNum = pageRowsDb.Key;

            //过滤掉不必要的字段
            var pageRows = from i in pageRowsDb.Value as List<UT_PageShow>
                           select new
                           {
                               ID = i.ID,
                               CreateDate = i.CreateDate.ToString(),
                               Name = i.Name,
                               Content = Server.HtmlDecode(i.Content),
                               EntryName = i.EntryName,
                               Url = Url.Action("P", "PageShow", new { id = string.IsNullOrEmpty(i.EntryName) ? i.ID.ToString() : i.EntryName }),
                               //Content = i.Content,
                               //Title = i.Title,
                               //DisplayOrder = i.DisplayOrder
                           };

            var jsonResult = new { total = totalNum, rows = pageRows };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Add_PageShow)]
        public async Task<ActionResult> Add(UT_PageShow model)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            Guid EntryNameTryID;
            if (!string.IsNullOrEmpty(model.EntryName)
                && Guid.TryParse(model.EntryName, out EntryNameTryID))
            {
                result.Success = false;
                result.Msg = "友好地址名不允许使用此格式！";
            }
            else if (string.IsNullOrEmpty(model.Name))
            {
                result.Success = false;
                result.Msg = "Name不能为空！";
            }
            else if (!string.IsNullOrEmpty(model.EntryName)
                && await _pageShowService.GetEntitiesCountAsync(x => x.EntryName == model.EntryName) > 0)
            {
                result.Success = false;
                result.Msg = "该友好地址名已存在！";
            }
            else
            {

                UT_PageShow entity = new UT_PageShow();
                entity.Name = model.Name;
                entity.EntryName = model.EntryName;
                entity.Content = model.Content;
                entity.CreateDate = CommonHelper.GetDateTimeInt();

                if (await _pageShowService.InsertAsync(entity))
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
        /// 更新数据
        /// </summary>
        /// <param name="modal"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Modify_PageShow)]
        public async Task<ActionResult> Update(UT_PageShow model)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            Guid EntryNameTryID;
            if (!string.IsNullOrEmpty(model.EntryName)
                && Guid.TryParse(model.EntryName, out EntryNameTryID))
            {
                result.Success = false;
                result.Msg = "友好地址名不允许使用此格式！";
            }
            else if (model.Name.Trim() == "")
            {
                result.Success = false;
                result.Msg = "Name不能为空！";
            }
            else
            {
                UT_PageShow entity = await _pageShowService.GetEntityByIdAsync(model.ID);

                if (!string.IsNullOrEmpty(model.EntryName)
                    && entity.EntryName != model.EntryName
                    && await _pageShowService.GetEntitiesCountAsync(x => x.EntryName == model.EntryName) > 0)
                {
                    result.Success = false;
                    result.Msg = "该友好地址名已存在！";
                }

                entity.Name = model.Name;
                entity.EntryName = model.EntryName;
                entity.Content = model.Content;

                if (await _pageShowService.UpdateAsync(entity))
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
        /// 删除GOIP
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Delete_PageShow)]
        public async Task<ActionResult> Delete(Guid? ID)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (ID.HasValue)
            {
                bool opResult = await _pageShowService.DeleteAsync(await _pageShowService.GetEntityByIdAsync(ID.Value));

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


        /// <summary>
        /// 上传图片
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> UploadImgAsync()
        {
            HttpPostedFileBase image = Request.Files["upload"];
            var CKEditorFuncNum = System.Web.HttpContext.Current.Request["CKEditorFuncNum"];

            string fileName = image.FileName;
            string extension = System.IO.Path.GetExtension(fileName);

            string filePath = String.Format("/Unitoys/{0}/{1}/{2}","PageShow", CommonHelper.GetYear(), CommonHelper.GetMonth());

            string newFileName = string.Format("{0}", DateTime.Now.ToString("yyMMddHHmmssfffffff"));

            var constructorInfo = typeof(HttpPostedFile).GetConstructors(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)[0];
            var obj = (HttpPostedFile)constructorInfo
                      .Invoke(new object[] { image.FileName, image.ContentType, image.InputStream });


            var resultAsync = await WebUtil.UploadImgAsync(obj, filePath, newFileName);

            //return Content(resultAsync);

            return Content("<script>window.parent.CKEDITOR.tools.callFunction(" + CKEditorFuncNum + ", \"" + resultAsync.GetCompleteUrl() + "\");</script>");

        }
    }
}
