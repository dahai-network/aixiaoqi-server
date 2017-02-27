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
    public class DeviceBraceletController : BaseController
    {
        private IDeviceBraceletService _deviceBraceletService;

        public DeviceBraceletController(IDeviceBraceletService deviceBraceletService)
        {
            this._deviceBraceletService = deviceBraceletService;
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
        public async Task<ActionResult> GetList(int page, int rows, string iMEI, string tel, DateTime? createStartDate, DateTime? createEndDate)
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
            var pageRowsDb = await _deviceBraceletService.SearchAsync(page, rows, iMEI, tel, beginSMSTimeInt, endSMSTimeInt);

            int totalNum = pageRowsDb.Key;

            //过滤掉不必要的字段
            var pageRows = from i in pageRowsDb.Value as List<UT_DeviceBracelet>
                           select new
                           {
                               ID = i.ID,
                               IMEI = i.IMEI,
                               CreateDate = i.CreateDate.ToString(),
                               UpdateDate = i.UpdateDate.ToString(),
                               Version = i.Version,
                               Power = i.Power,
                               ConnectDate = i.ConnectDate,
                               DeviceType = i.DeviceType,
                               Tel = i.UT_Users == null ? "" : i.UT_Users.Tel,
                           };

            var jsonResult = new { total = totalNum, rows = pageRows };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除手环
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Delete_DeviceBracelet)]
        public async Task<ActionResult> Delete(Guid? ID)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (ID.HasValue)
            {
                bool opResult = await _deviceBraceletService.DeleteAsync(await _deviceBraceletService.GetEntityByIdAsync(ID.Value));

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
