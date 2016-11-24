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
    [RequireRolesOrPermissions(UnitoysPermissionStore.Can_View_Sport)]
    public class SportController : BaseController
    {
        private ISportService _sportService;

        public SportController(ISportService sportService)
        {
            this._sportService = sportService;
        }
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取设备手环列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetList(int page, int rows, string tel, DateTime? createStartDate, DateTime? createEndDate)
        {
            int? beginSMSTimeInt = null;
            int? endSMSTimeInt = null;
            if (createStartDate.HasValue)
            {
                beginSMSTimeInt = CommonHelper.ConvertDateTimeInt(createStartDate.Value);
            }
            if (createEndDate.HasValue)
            {
                endSMSTimeInt = CommonHelper.ConvertDateTimeInt(createEndDate.Value);
            }

            var pageRowsDb = await _sportService.SearchAsync(page, rows, tel, beginSMSTimeInt, endSMSTimeInt);

            int totalNum = pageRowsDb.Key;

            //过滤掉不必要的字段
            var pageRows = from i in pageRowsDb.Value as List<UT_Sport>
                           select new
                           {
                               ID = i.ID,
                               Date = i.Date,
                               StepNum = i.StepNum,
                               CreateDate = i.CreateDate.ToString(),
                               Tel = i.UT_Users == null ? "" : i.UT_Users.Tel,
                           };

            var jsonResult = new { total = totalNum, rows = pageRows };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

    }
}
