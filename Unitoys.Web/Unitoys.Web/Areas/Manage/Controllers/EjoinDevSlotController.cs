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
    [RequireRolesOrPermissions(UnitoysPermissionStore.Can_View_EjoinDevSlot)]
    public class EjoinDevSlotController : BaseController
    {
        private IEjoinDevSlotService _ejoinDevSlotService;

        public EjoinDevSlotController(IEjoinDevSlotService ejoinDevSlotService)
        {
            this._ejoinDevSlotService = ejoinDevSlotService;
        }
        //public ActionResult Index()
        //{
        //    return View();
        //}

        /// <summary>
        /// 获取EjoinDevSlot列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetList(Guid? EjoinDevId)
        {
            int page = 1, rows = 600;
            var pageRowsDb = await _ejoinDevSlotService.SearchAsync(page, rows, EjoinDevId);

            int totalNum = pageRowsDb.Key;

            //过滤掉不必要的字段
            var pageRows = from i in pageRowsDb.Value as List<UT_EjoinDevSlot>
                           select new
                           {
                               ID = i.ID,
                               IMEI = i.IMEI,
                               PortNum = i.PortNum,
                               ICCID = i.ICCID,
                               SimNum = i.SimNum,
                               Status = i.Status,
                               EjoinDevId = i.EjoinDevId,
                               Tel = i.UT_Users != null ? i.UT_Users.Tel : "",
                           };

            var jsonResult = new { total = totalNum, rows = pageRows };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }
    }
}
