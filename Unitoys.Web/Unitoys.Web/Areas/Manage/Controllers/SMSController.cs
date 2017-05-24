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

namespace Unitoys.Web.Areas.Manage.Controllers
{
    [RequireRolesOrPermissions(UnitoysPermissionStore.Can_View_SMS)]
    public class SMSController : BaseController
    {
        private ISMSService _smsService;

        public SMSController() { }

        public SMSController(ISMSService smsService)
        {
            this._smsService = smsService;
        }
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取所有短信列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public async Task<ActionResult> GetList(int page, int rows, string tel, string to, DateTime? beginSMSTime, DateTime? endSMSTime, SMSStatusType? smsStatus)
        {
            int? beginSMSTimeInt = null;
            int? endSMSTimeInt = null;
            if (beginSMSTime.HasValue)
            {
                beginSMSTimeInt = CommonHelper.ConvertDateTimeInt(beginSMSTime.Value);
            }
            if (endSMSTime.HasValue)
            {
                endSMSTimeInt = CommonHelper.ConvertDateTimeInt(endSMSTime.Value);
            }
            var result = await _smsService.SearchAsync(page, rows, tel, to, beginSMSTimeInt, endSMSTimeInt, smsStatus);

            int totalNum = result.Key;

            //过滤掉不必要的字段
            var pageRows = from i in result.Value as List<UT_SMS>
                           select new
                           {
                               ID = i.ID,
                               Tel = i.UT_Users.Tel,
                               TId = i.TId,
                               Fm = i.Fm,
                               To = i.To,
                               SMSTime = i.SMSTime.ToString(),
                               SMSContent = i.SMSContent,
                               Status = i.Status,
                               IsSend = i.IsSend,
                               ErrorMsg = i.ErrorMsg,
                               //SMSContent = i.SMSContent,
                               //MsgType = i.MsgType,
                               //StatusType = i.StatusType,
                               IsRead = i.IsRead,
                               IccId = i.IccId,
                               DevName = i.DevName,
                               Port = i.Port,
                           };

            var jsonResult = new { total = totalNum, rows = pageRows };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }
    }
}
