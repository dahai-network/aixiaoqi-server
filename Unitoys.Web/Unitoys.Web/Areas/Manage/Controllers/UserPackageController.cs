using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Unitoys.IServices;
using Unitoys.Model;

namespace Unitoys.Web.Areas.Manage.Controllers
{
    public class UserPackageController : BaseController
    {

        //private IUserPackageService _userPackageService;

        //public UserPackageController() { }
        //public UserPackageController(IUserPackageService userPackageService)
        //{
        //    this._userPackageService = userPackageService;
        //}
        //public ActionResult Index()
        //{
        //    return View();
        //}
        ///// <summary>
        ///// 获取订单列表
        ///// </summary>
        ///// <param name="page"></param>
        ///// <param name="rows"></param>
        ///// <returns></returns>
        //public async Task<ActionResult> GetList(int page, int rows)
        //{

        //    var pageRowsDb = await _userPackageService.GetEntitiesForPagingAsync("UT_UserPackage", page, rows, "CreateDate", "desc", "1=1");

        //    int totalNum = pageRowsDb.Key;

        //    //过滤掉不必要的字段
        //    var pageRows = from i in pageRowsDb.Value as List<UT_UserPackage>
        //                   select new
        //                   {
        //                       ID = i.ID,
        //                       UserId = i.UserId,
        //                       LoginName = i.LoginName,
        //                       PackageName = i.PackageName,
        //                       TotalFlow = i.TotalFlow,
        //                       UsedFlow = i.UsedFlow,
        //                       CreateDate = i.CreateDate.ToString(),
        //                       StartDate = i.StartDate.ToString(),
        //                       ExpireDays = i.ExpireDays,
        //                       Status = i.Status
        //                   };

        //    var jsonResult = new { total = totalNum, rows = pageRows };

        //    return Json(jsonResult, JsonRequestBehavior.AllowGet);
        //}
    }
}
