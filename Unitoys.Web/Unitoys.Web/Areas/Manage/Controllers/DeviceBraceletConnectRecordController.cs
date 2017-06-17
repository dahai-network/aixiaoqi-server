using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
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
    [RequireRolesOrPermissions(UnitoysPermissionStore.Can_View_DeviceBraceletConnectRecord)]
    public class DeviceBraceletConnectRecordController : BaseController
    {
        private IDeviceBraceletConnectRecordService _deviceBraceletConnectRecordService;

        public DeviceBraceletConnectRecordController() { }

        public DeviceBraceletConnectRecordController(IDeviceBraceletConnectRecordService deviceBraceletConnectRecordService)
        {
            this._deviceBraceletConnectRecordService = deviceBraceletConnectRecordService;
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
        public async Task<ActionResult> GetList(int page, int rows, string sort, string order, string iMEI, string tel, DateTime? createStartDate, DateTime? createEndDate, bool? isOnLine)
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
            var pageRowsDb = await _deviceBraceletConnectRecordService.SearchAsync(page, rows, sort, order, iMEI, tel, beginSMSTimeInt, endSMSTimeInt, isOnLine);

            int totalNum = pageRowsDb.Key;

            //过滤掉不必要的字段
            var pageRows = from i in pageRowsDb.Value as List<UT_DeviceBraceletConnectRecord>
                           select new
                           {
                               ID = i.ID,
                               SessionId = i.SessionId,
                               IMEI = i.IMEI,
                               CreateDate = i.CreateDate,
                               ConnectDate = i.ConnectDate,
                               DisconnectDate = i.DisconnectDate,
                               DisconnectStatus = i.DisconnectStatus.ToString(),
                               ConnectDuration = i.DisconnectDate.HasValue ? GetHumanTime(i.DisconnectDate.Value - i.ConnectDate) : GetHumanTime(CommonHelper.GetDateTimeInt() - i.ConnectDate),
                               EjoinDevNameAndPort = i.EjoinDevNameAndPort,
                               RegSuccessDate = i.RegSuccessDate,
                               Remark = i.Remark,
                               Tel = i.UT_Users == null ? "" : i.UT_Users.Tel,
                               ClientType = i.ClientType,
                           };

            var jsonResult = new { total = totalNum, rows = pageRows };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 人类可识别的时间大小
        /// </summary>
        /// <param name="seconds">总秒数</param>
        /// <returns></returns>
        private string GetHumanTime(int seconds)
        {
            TimeSpan ts = new TimeSpan(0, 0, seconds);

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (ts.Days > 0)
            {
                sb.Append((int)ts.TotalDays + "天");
            }
            if (ts.Hours > 0)
            {
                sb.Append(ts.Hours + "小时");
            }
            if (ts.Minutes > 0)
            {
                sb.Append(ts.Minutes + "分");
            }
            if (ts.Seconds > 0)
            {
                sb.Append(ts.Seconds + "秒");
            }
            return sb.ToString();
        }


    }
}
