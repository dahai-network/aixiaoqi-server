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
    public class UserBillController : BaseController
    {
        private readonly IUserBillService _userBillService;
        public UserBillController(IUserBillService userBillService)
        {
            this._userBillService = userBillService;
        }

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取用户账单列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public async Task<ActionResult> GetUserBills(int page, int rows, string loginName, DateTime? createStartDate, DateTime? createEndDate, int? billType)
        {
            int? createStartDateInt = null;
            int? createEndDateInt = null;
            if (createStartDate.HasValue)
            {
                createStartDateInt = CommonHelper.ConvertDateTimeInt(createStartDate.Value);
            }
            if (createEndDate.HasValue)
            {
                createEndDateInt = CommonHelper.ConvertDateTimeInt(createEndDate.Value);
            }

            var pageRowsDb = await _userBillService.SearchAsync(page, rows, loginName, createStartDateInt, createEndDateInt, billType);

            int totalNum = pageRowsDb.Key;

            //过滤掉不必要的字段
            var pageRows = from i in pageRowsDb.Value
                           select new
                           {
                               ID = i.ID,
                               LoginName = i.LoginName,
                               Amount = i.Amount,
                               UserAmount = i.UserAmount,
                               CreateDate = i.CreateDate.ToString(),
                               BillType = i.BillType,
                               PayType = i.PayType,
                               Descr = i.Descr
                           };

            var jsonResult = new { total = totalNum, rows = pageRows };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }
    }
}