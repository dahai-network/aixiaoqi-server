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
    [RequireRolesOrPermissions(UnitoysPermissionStore.Can_View_ContactUS)]
    public class ContactUSController : BaseController
    {
        private IContactUSService _contactUSService;

        public ContactUSController() { }

        public ContactUSController(IContactUSService contactUSService)
        {
            this._contactUSService = contactUSService;
        }
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 获取国家列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public async Task<ActionResult> GetList(int page, int rows, string name, DateTime? createStartDate, DateTime? createEndDate)
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
            var pageRowsDb = await _contactUSService.SearchAsync(page, rows, name, beginTimeInt, endTimeInt);

            int totalNum = pageRowsDb.Key;

            //过滤掉不必要的字段
            var pageRows = from i in pageRowsDb.Value
                           select new
                           {
                               ID = i.ID,
                               Name = i.Name,
                               MailAddress = i.MailAddress,
                               Subject = i.Subject,
                               Content = i.Content,
                               Status = i.Status,
                               CreateDate = i.CreateDate,
                               Remark = i.Remark,
                           };

            var jsonResult = new { total = totalNum, rows = pageRows };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Modify_ContactUS)]
        public async Task<ActionResult> Update(UT_ContactUS model)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            UT_ContactUS entity = await _contactUSService.GetEntityByIdAsync(model.ID);
            entity.Status = model.Status;
            entity.Remark = model.Remark;

            if (await _contactUSService.UpdateAsync(entity))
            {
                result.Success = true;
                result.Msg = "更新成功！";
            }
            else
            {
                result.Success = false;
                result.Msg = "更新失败！";
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Delete_ContactUS)]
        public async Task<ActionResult> Delete(Guid? ID)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (ID.HasValue)
            {
                UT_ContactUS modal = await _contactUSService.GetEntityByIdAsync(ID.Value);
                await _contactUSService.DeleteAsync(modal);
                result.Success = true;
                result.Msg = "删除成功！";
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
