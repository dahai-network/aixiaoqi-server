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
    [RequireRolesOrPermissions(UnitoysPermissionStore.Can_View_Feedback)]
    public class FeedbackController : BaseController
    {
        private IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            this._feedbackService = feedbackService;
        }
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取反馈列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetList(int page, int rows, string tel, DateTime? createStartDate, DateTime? createEndDate)
        {
            var pageRowsDb = await _feedbackService.SearchAsync(page, rows, tel, createStartDate, createEndDate);

            int totalNum = pageRowsDb.Key;

            //过滤掉不必要的字段
            var pageRows = from i in pageRowsDb.Value as List<UT_Feedback>
                           select new
                           {
                               ID = i.ID,
                               Info = i.Info,
                               Version = i.Version,
                               Model = i.Model,
                               Tel = i.UT_Users == null ? "" : i.UT_Users.Tel,
                               Mail = i.Mail,
                               CreateDate = i.CreateDate.ToString(),
                               Entrance = i.Entrance,
                               //UserId = i.UserId,
                           };

            var jsonResult = new { total = totalNum, rows = pageRows };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除反馈
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Delete_Feedback)]
        public async Task<ActionResult> Delete(Guid? ID)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (ID.HasValue)
            {
                bool opResult = await _feedbackService.DeleteAsync(await _feedbackService.GetEntityByIdAsync(ID.Value));

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
