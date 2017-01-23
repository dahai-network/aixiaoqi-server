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
    [RequireRolesOrPermissions(UnitoysPermissionStore.Can_View_OperationRecord)]
    public class OperationRecordController : BaseController
    {
        private IOperationRecordService _OperationRecordService;

        public OperationRecordController(IOperationRecordService OperationRecordService)
        {
            this._OperationRecordService = OperationRecordService;
        }
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取操作记录列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetList(int page, int rows, string url, string managerLoginName, DateTime? createStartDate, DateTime? createEndDate)
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
            var pageRowsDb = await _OperationRecordService.SearchAsync(page, rows, url, managerLoginName, createStartDateInt, createEndDateInt);

            int totalNum = pageRowsDb.Key;

            //过滤掉不必要的字段
            var pageRows = from i in pageRowsDb.Value as List<UT_OperationRecord>
                           select new
                           {
                               ID = i.ID,
                               Url = i.Url,
                               Parameter = i.Parameter,
                               Data = i.Data,
                               Response = i.Response,
                               CreateDate = i.CreateDate.ToString(),
                               CreateManageUsersName = i.UT_ManageUsers.LoginName,
                               Remark = i.Remark,
                           };

            var jsonResult = new { total = totalNum, rows = pageRows };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }
    }
}
