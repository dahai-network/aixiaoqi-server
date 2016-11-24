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
    [RequireRolesOrPermissions(UnitoysPermissionStore.Can_View_User)]
    public class UserLoginRecordController : BaseController
    {
        private IUserLoginRecordService _userLoginRecordService;

        public UserLoginRecordController() { }

        public UserLoginRecordController(IUserLoginRecordService userLoginRecordService)
        {
            this._userLoginRecordService = userLoginRecordService;
        }
        
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 获取用户登录记录列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetList(int page, int rows, string loginName = "")
        {
            string CommandText = "1=1";
            if (loginName != "")
            {
                CommandText = "loginname like '%" + loginName+"%'";
            }
            var pageRowsDb = await _userLoginRecordService.GetEntitiesForPagingAsync("UT_UserLoginRecord", page, rows, "LoginDate", "desc", CommandText);

            int totalNum = pageRowsDb.Key;

            //过滤掉不必要的字段
            var pageRows = from i in pageRowsDb.Value as List<UT_UserLoginRecord>
                           select new
                           {
                               ID = i.ID,
                               LoginName=i.LoginName,
                               UserId = i.UserId,
                               LoginIp = i.LoginIp + "(" + IPScanner.IPLocation(i.LoginIp) + ")",
                               LoginDate = i.LoginDate.ToString(),
                               Entrance = i.Entrance
                           };

            var jsonResult = new { total = totalNum, rows = pageRows };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }
    }
}
