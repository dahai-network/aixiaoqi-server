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
    [RequireRolesOrPermissions(UnitoysPermissionStore.Can_View_AfterSales)]
    public class AfterSalesController : BaseController
    {
        private IAfterSalesService _afterSalesService;

        public AfterSalesController() { }

        public AfterSalesController(IAfterSalesService afterSalesService)
        {
            this._afterSalesService = afterSalesService;
        }
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 获取国家列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public async Task<ActionResult> GetList(int page, int rows, string name, string mobilePhoneAfterSalesNum, DeviceType? productModel, DateTime? createStartDate, DateTime? createEndDate)
        {
            int? beginTimeInt = null;
            int? endTimeInt = null;
            if (createStartDate.HasValue)
            {
                beginTimeInt = CommonHelper.ConvertDateTimeInt(createStartDate.Value);
            }
            if (endTimeInt.HasValue)
            {
                endTimeInt = CommonHelper.ConvertDateTimeInt(createEndDate.Value);
            }
            var pageRowsDb = await _afterSalesService.SearchAsync(page, rows, name, mobilePhoneAfterSalesNum, productModel, beginTimeInt, endTimeInt);

            int totalNum = pageRowsDb.Key;

            //过滤掉不必要的字段
            var pageRows = from i in pageRowsDb.Value
                           select new
                           {
                               ID = i.ID,
                               AfterSalesNum = i.AfterSalesNum,
                               Contact = i.Contact,
                               MobilePhone = i.MobilePhone,
                               Address = i.Address,
                               BuyDate = i.BuyDate.ToString(),
                               ProblemDescr = i.ProblemDescr,
                               Pic1 = i.Pic1.GetCompleteUrl(),
                               Pic2 = i.Pic2.GetCompleteUrl(),
                               Pic3 = i.Pic3.GetCompleteUrl(),
                               Pic4 = i.Pic4.GetCompleteUrl(),
                               Pic5 = i.Pic5.GetCompleteUrl(),
                               Status = i.Status,
                               TrackingNO = i.TrackingNO,
                               ProductModel = i.ProductModel,
                               ExpressCompany = i.ExpressCompany,
                               AuditRemark = i.AuditRemark,
                               CreateDate = CommonHelper.GetTime(i.CreateDate + "").ToString(),
                           };

            var jsonResult = new { total = totalNum, rows = pageRows };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //[RequireRolesOrPermissions(UnitoysPermissionStore.Can_Add_AfterSales)]
        //public async Task<ActionResult> Add(UT_AfterSales modal)
        //{

        //    JsonAjaxResult result = new JsonAjaxResult();

        //    if (modal.AfterSalesName.Trim() == "")
        //    {
        //        result.Success = false;
        //        result.Msg = "国家名称不能为空！";
        //    }
        //    else if (modal.Rate <= 0)
        //    {
        //        result.Success = false;
        //        result.Msg = "费率标准不能为空！";
        //    }
        //    else if (modal.Pic.Trim() == "")
        //    {
        //        result.Success = false;
        //        result.Msg = "图片不能为空！";
        //    }
        //    else if (modal.LogoPic.Trim() == "")
        //    {
        //        result.Success = false;
        //        result.Msg = "Logo图片不能为空！";
        //    }
        //    else
        //    {

        //        UT_AfterSales afterSales = new UT_AfterSales();
        //        afterSales.AfterSalesNum = modal.AfterSalesNum;
        //        afterSales.Name = modal.Name;
        //        afterSales.MobilePhone = modal.IsHot;
        //        afterSales.Continents = modal.Continents;
        //        afterSales.Rate = modal.Rate;
        //        afterSales.Pic = modal.Pic;
        //        afterSales.LogoPic = modal.LogoPic;
        //        afterSales.CreateDate = DateTime.Now;
        //        afterSales.DisplayOrder = modal.DisplayOrder;
        //        afterSales.Descr = modal.Descr;

        //        if (await _afterSalesService.InsertAsync(afterSales))
        //        {
        //            result.Success = true;
        //            result.Msg = "添加成功！";
        //        }
        //        else
        //        {
        //            result.Success = false;
        //            result.Msg = "操作失败！";
        //        }
        //    }

        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Modify_AfterSales)]
        public async Task<ActionResult> Update(UT_AfterSales model)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            UT_AfterSales entity = await _afterSalesService.GetEntityByIdAsync(model.ID);
            entity.Status = model.Status;
            entity.TrackingNO = model.TrackingNO;
            entity.ExpressCompany = model.ExpressCompany;
            entity.AuditRemark = model.AuditRemark;

            if (await _afterSalesService.UpdateAsync(entity))
            {
                result.Success = true;
                result.Msg = "更新成功！";
            }
            else
            {
                result.Success = false;
                result.Msg = "更新失败！";
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //[RequireRolesOrPermissions(UnitoysPermissionStore.Can_Delete_AfterSales)]
        //public async Task<ActionResult> Delete(Guid? ID)
        //{
        //    JsonAjaxResult result = new JsonAjaxResult();

        //    if (ID.HasValue)
        //    {
        //        UT_AfterSales modal = await _afterSalesService.GetEntityByIdAsync(ID.Value);
        //        await _afterSalesService.DeleteAsync(modal);
        //        result.Success = true;
        //        result.Msg = "删除成功！";
        //    }
        //    else
        //    {
        //        result.Success = false;
        //        result.Msg = "参数错误！";
        //    }

        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
    }
}
