using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Unitoys.Core;
using Unitoys.Core.Security;
using Unitoys.IServices;
using Unitoys.Model;

namespace Unitoys.Web.Areas.Manage.Controllers
{
    [RequireRolesOrPermissions(UnitoysPermissionStore.Can_View_UserBill)]
    public class SpeakRecordController : BaseController
    {
        private readonly ISpeakRecordService _speakRecordService;
        public SpeakRecordController(ISpeakRecordService speakRecordService)
        {
            this._speakRecordService = speakRecordService;
        }

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取通话列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public async Task<ActionResult> GetSpeakRecords(int page, int rows, string deviceName, string calledTelNum, DateTime? callStartBeginTime, DateTime? callStartEndTime, int? CallSessionBeginTime, int? CallSessionEndTime, int? isCallConnected)
        {
            var pageRowsDb = await _speakRecordService.SearchAsync(page, rows, deviceName, calledTelNum, callStartBeginTime, callStartEndTime, CallSessionBeginTime, CallSessionEndTime, isCallConnected);

            int totalNum = pageRowsDb.Key;

            //过滤掉不必要的字段
            var pageRows = from i in pageRowsDb.Value
                           select new
                           {
                               ID = i.ID,
                               DeviceName = i.DeviceName,
                               CalledTelNum = i.CalledTelNum,
                               CallStartTime = i.CallStartTime.ToString(),
                               CallStopTime = i.CallStopTime.ToString(),
                               CallSessionTime = i.CallSessionTime,
                               CallAgoRemainingCallSeconds = i.CallAgoRemainingCallSeconds,
                               CallSourceIp = i.CallSourceIp,
                               CallServerIp = i.CallServerIp,
                               Acctterminatedirection = i.Acctterminatedirection,
                               Status = i.Status
                           };

            var jsonResult = new { total = totalNum, rows = pageRows };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }
    }
}