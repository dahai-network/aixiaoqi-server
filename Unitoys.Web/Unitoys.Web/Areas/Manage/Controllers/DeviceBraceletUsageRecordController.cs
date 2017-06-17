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
    [RequireRolesOrPermissions(UnitoysPermissionStore.Can_View_DeviceBracelet)]
    public class DeviceBraceletUsageRecordController : BaseController
    {
        private IDeviceBraceletUsageRecordService _deviceBraceletUsageRecordService;

        public DeviceBraceletUsageRecordController(IDeviceBraceletUsageRecordService deviceBraceletUsageRecordService)
        {
            this._deviceBraceletUsageRecordService = deviceBraceletUsageRecordService;
        }
        //public ActionResult Index()
        //{
        //    return View();
        //}

        /// <summary>
        /// 获取DeviceBraceletUsageRecord列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetList(string IMEI)
        {
            int page = 1, rows = 600;
            var pageRowsDb = await _deviceBraceletUsageRecordService.SearchAsync(page, rows, IMEI);

            int totalNum = pageRowsDb.Key;

            //过滤掉不必要的字段
            var pageRows = from i in pageRowsDb.Value as List<UT_DeviceBraceletUsageRecord>
                           select new
                           {
                               ID = i.ID,
                               IMEI = i.IMEI,
                               Version = i.Version,
                               Power = i.Power,
                               Tel = i.UT_Users != null ? i.UT_Users.Tel : "",
                               CreateDate = i.CreateDate,
                           };

            var jsonResult = new { total = totalNum, rows = pageRows };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }
    }
}
