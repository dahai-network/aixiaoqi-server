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
    [RequireRolesOrPermissions(UnitoysPermissionStore.Can_View_Package)]
    public class PackageAttributeController : BaseController
    {
        private IPackageAttributeService _packageAttributeService;
        private IOrderService _orderService;
        private ICountryService _countryService;
        public PackageAttributeController() { }

        public PackageAttributeController(IPackageAttributeService packageAttributeService, IOrderService orderService, ICountryService countryService)
        {
            this._packageAttributeService = packageAttributeService;
            this._orderService = orderService;
            this._countryService = countryService;
        }
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取套餐列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetList(int page, int rows, string packageAttributeName, Guid packageId)
        {
            var pageRowsDb = await _packageAttributeService.SearchAsync(page, rows, packageAttributeName, null, null, packageId);

            int totalNum = pageRowsDb.Key;

            //过滤掉不必要的字段
            var pageRows = from i in pageRowsDb.Value as List<UT_PackageAttribute>
                           select new
                           {
                               ID = i.ID,
                               PackageId = i.PackageId,
                               Flow = i.Flow,
                               CallMinutes = i.CallMinutes,
                               ExpireDays = i.ExpireDays,
                               FlowDescr = i.FlowDescr,
                               CallMinutesDescr = i.CallMinutesDescr,
                               ExpireDaysDescr = i.ExpireDaysDescr,
                               Price = i.Price,
                               OriginalPrice = i.OriginalPrice,
                               DisplayOrder = i.DisplayOrder,

                           };

            var jsonResult = new { total = totalNum, rows = pageRows };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //[RequireRolesOrPermissions(UnitoysPermissionStore.Can_Add_Package)]
        //public async Task<ActionResult> Add(UT_PackageAttribute model)
        //{
        //    JsonAjaxResult result = new JsonAjaxResult();

        //    if (model.PackageAttributeName.Trim() == "")
        //    {
        //        result.Success = false;
        //        result.Msg = "套餐标题不能为空！";
        //    }
        //    //else if (model.Price <= 0)
        //    //{
        //    //    result.Success = false;
        //    //    result.Msg = "价格错误！";
        //    //}
        //    //else if (model.Flow < 0)
        //    //{
        //    //    result.Success = false;
        //    //    result.Msg = "套餐流量错误！";
        //    //}
        //    //else if (string.IsNullOrEmpty(model.Pic))
        //    //{
        //    //    result.Success = false;
        //    //    result.Msg = "图片不能为空！";
        //    //}
        //    else if (model.ExpireDays <= 0)
        //    {
        //        result.Success = false;
        //        result.Msg = "套餐有效天数设置错误！";
        //    }
        //    else if (await _packageAttributeService.GetEntitiesCountAsync(x => x.PackageAttributeNum == model.PackageAttributeNum) > 0)
        //    {
        //        result.Success = false;
        //        result.Msg = "该套餐编号已存在！";
        //    }
        //    else
        //    {

        //        UT_PackageAttribute packageAttribute = new UT_PackageAttribute();
        //        packageAttribute.PackageAttributeName = model.PackageAttributeName;
        //        packageAttribute.Price = model.Price;
        //        packageAttribute.Flow = model.Flow;
        //        packageAttribute.CallMinutes = model.CallMinutes;
        //        packageAttribute.Pic = model.Pic;
        //        packageAttribute.LogoPic = model.LogoPic;
        //        packageAttribute.Lock4 = 0;
        //        packageAttribute.ExpireDays = model.ExpireDays;
        //        packageAttribute.CountryId = model.CountryId;
        //        packageAttribute.PackageAttributeNum = model.PackageAttributeNum;
        //        packageAttribute.Operators = model.Operators;
        //        packageAttribute.Features = model.Features;
        //        packageAttribute.Details = model.Details;
        //        packageAttribute.UseDescr = model.UseDescr;
        //        packageAttribute.DisplayOrder = model.DisplayOrder;
        //        packageAttribute.Category = model.Category;
        //        packageAttribute.IsCategoryFlow = model.IsCategoryFlow;
        //        packageAttribute.IsCategoryCall = model.IsCategoryCall;
        //        packageAttribute.IsCategoryDualSimStandby = model.IsCategoryDualSimStandby;
        //        packageAttribute.IsCategoryKingCard = model.IsCategoryKingCard;
        //        packageAttribute.IsCanBuyMultiple = model.IsCanBuyMultiple;
        //        packageAttribute.IsSupport4G = model.IsSupport4G;
        //        packageAttribute.IsApn = model.IsApn;
        //        packageAttribute.ApnName = string.IsNullOrEmpty(model.ApnName) ? "" : model.ApnName;
        //        packageAttribute.OriginalPrice = model.OriginalPrice;
        //        packageAttribute.PicHaveed = model.PicHaveed ?? "";
        //        packageAttribute.DescTitlePic = model.DescTitlePic ?? "";
        //        packageAttribute.DescPic = model.DescPic ?? "";

        //        if (await _packageAttributeService.InsertAsync(packageAttribute))
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
        ///// <summary>
        ///// 更新套餐
        ///// </summary>
        ///// <param name="modal"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[RequireRolesOrPermissions(UnitoysPermissionStore.Can_Modify_Package)]
        //public async Task<ActionResult> Update(UT_PackageAttribute model)
        //{
        //    JsonAjaxResult result = new JsonAjaxResult();

        //    if (model.PackageAttributeName.Trim() == "")
        //    {
        //        result.Success = false;
        //        result.Msg = "套餐标题不能为空！";
        //    }
        //    //else if (model.Price <= 0)
        //    //{
        //    //    result.Success = false;
        //    //    result.Msg = "价格错误！";
        //    //}
        //    //else if (model.Flow < 0)
        //    //{
        //    //    result.Success = false;
        //    //    result.Msg = "套餐流量错误！";
        //    //}
        //    //else if (string.IsNullOrEmpty(model.Pic))
        //    //{
        //    //    result.Success = false;
        //    //    result.Msg = "图片不能为空！";
        //    //}
        //    else if (model.ExpireDays <= 0)
        //    {
        //        result.Success = false;
        //        result.Msg = "套餐有效天数设置错误！";
        //    }
        //    else
        //    {
        //        UT_PackageAttribute packageAttribute = await _packageAttributeService.GetEntityByIdAsync(model.ID);
        //        if (packageAttribute.PackageAttributeNum != model.PackageAttributeNum &&
        //            await _packageAttributeService.GetEntitiesCountAsync(x => x.PackageAttributeNum == model.PackageAttributeNum) > 0)
        //        {
        //            result.Success = false;
        //            result.Msg = "该套餐编号已存在！";
        //        }
        //        if (string.IsNullOrEmpty(result.Msg))
        //        {
        //            packageAttribute.PackageAttributeName = model.PackageAttributeName;
        //            packageAttribute.Price = model.Price;
        //            packageAttribute.Flow = model.Flow;
        //            packageAttribute.Pic = StringHelper.TrimStart(model.Pic, UTConfig.SiteConfig.ImageHandleHost);
        //            packageAttribute.LogoPic = StringHelper.TrimStart(model.LogoPic, UTConfig.SiteConfig.ImageHandleHost);
        //            packageAttribute.ExpireDays = model.ExpireDays;
        //            packageAttribute.CountryId = model.CountryId;
        //            packageAttribute.PackageAttributeNum = model.PackageAttributeNum;
        //            packageAttribute.Operators = model.Operators;
        //            packageAttribute.CallMinutes = model.CallMinutes;
        //            packageAttribute.Features = model.Features;
        //            packageAttribute.Details = model.Details;
        //            packageAttribute.UseDescr = model.UseDescr;
        //            packageAttribute.DisplayOrder = model.DisplayOrder;
        //            packageAttribute.Category = model.Category;
        //            packageAttribute.IsCategoryFlow = model.IsCategoryFlow;
        //            packageAttribute.IsCategoryCall = model.IsCategoryCall;
        //            packageAttribute.IsCategoryDualSimStandby = model.IsCategoryDualSimStandby;
        //            packageAttribute.IsCategoryKingCard = model.IsCategoryKingCard;
        //            packageAttribute.IsCanBuyMultiple = model.IsCanBuyMultiple;
        //            packageAttribute.IsSupport4G = model.IsSupport4G;
        //            packageAttribute.IsApn = model.IsApn;
        //            packageAttribute.ApnName = string.IsNullOrEmpty(model.ApnName) ? "" : model.ApnName;
        //            packageAttribute.OriginalPrice = model.OriginalPrice;
        //            packageAttribute.PicHaveed = model.PicHaveed ?? "";
        //            packageAttribute.DescTitlePic = model.DescTitlePic ?? "";
        //            packageAttribute.DescPic = model.DescPic ?? "";

        //            if (await _packageAttributeService.UpdateAsync(packageAttribute))
        //            {
        //                result.Success = true;
        //                result.Msg = "更新成功！";
        //            }
        //            else
        //            {
        //                result.Success = false;
        //                result.Msg = "更新失败！";
        //            }
        //        }
        //    }
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
        ///// <summary>
        ///// 删除套餐
        ///// </summary>
        ///// <param name="ID"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[RequireRolesOrPermissions(UnitoysPermissionStore.Can_Delete_Package)]
        //public async Task<ActionResult> Delete(Guid? ID)
        //{
        //    JsonAjaxResult result = new JsonAjaxResult();

        //    if (ID.HasValue)
        //    {
        //        bool opResult = false;
        //        //是否有用户购买此套餐，没有则删除，如果有则假删除
        //        if (await _orderService.GetEntitiesCountAsync(c => c.PackageAttributeId.Equals(ID.Value)) > 0)
        //        {
        //            UT_PackageAttribute packageAttribute = await _packageAttributeService.GetEntityByIdAsync(ID.Value);
        //            packageAttribute.IsDeleted = true;
        //            opResult = await _packageAttributeService.UpdateAsync(packageAttribute);
        //        }
        //        else
        //        {
        //            opResult = await _packageAttributeService.DeleteAsync(await _packageAttributeService.GetEntityByIdAsync(ID.Value));
        //        }

        //        if (opResult)
        //        {
        //            result.Success = true;
        //            result.Msg = "删除成功！";
        //        }
        //        else
        //        {
        //            result.Success = false;
        //            result.Msg = "删除失败！";
        //        }
        //    }
        //    else
        //    {
        //        result.Success = false;
        //        result.Msg = "次数错误！";
        //    }

        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
        ///// <summary>
        ///// 下架套餐操作
        ///// </summary>
        ///// <param name="LoginName"></param>
        ///// <param name="PassWord"></param>
        ///// <param name="TrueName"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[RequireRolesOrPermissions(UnitoysPermissionStore.Can_Modify_Package)]
        //public async Task<ActionResult> Lock(Guid? ID)
        //{
        //    JsonAjaxResult result = new JsonAjaxResult();
        //    if (ID.HasValue)
        //    {

        //        UT_PackageAttribute packageAttribute = await _packageAttributeService.GetEntityByIdAsync(ID.Value);
        //        if (packageAttribute.Lock4 != 1)
        //        {
        //            packageAttribute.Lock4 = 1;
        //            if (await _packageAttributeService.UpdateAsync(packageAttribute))
        //            {
        //                result.Success = true;
        //                result.Msg = "操作成功！";
        //            }
        //            else
        //            {
        //                result.Success = false;
        //                result.Msg = "操作失败！";
        //            }
        //        }
        //        else
        //        {
        //            result.Success = false;
        //            result.Msg = "该套餐已经是下架状态！";
        //        }

        //    }
        //    else
        //    {
        //        result.Success = false;
        //        result.Msg = "请求失败！";
        //    }
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
        ///// <summary>
        ///// 上架套餐操作
        ///// </summary>
        ///// <param name="LoginName"></param>
        ///// <param name="PassWord"></param>
        ///// <param name="TrueName"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[RequireRolesOrPermissions(UnitoysPermissionStore.Can_Modify_Package)]
        //public async Task<ActionResult> uLock(Guid? ID)
        //{
        //    JsonAjaxResult result = new JsonAjaxResult();
        //    if (ID.HasValue)
        //    {
        //        UT_PackageAttribute packageAttribute = await _packageAttributeService.GetEntityByIdAsync(ID.Value);
        //        if (packageAttribute.Lock4 != 0)
        //        {
        //            packageAttribute.Lock4 = 0;
        //            if (await _packageAttributeService.UpdateAsync(packageAttribute))
        //            {
        //                result.Success = true;
        //                result.Msg = "更新成功！";
        //            }
        //            else
        //            {
        //                result.Success = false;
        //                result.Msg = "操作失败！";
        //            }
        //        }
        //        else
        //        {
        //            result.Success = false;
        //            result.Msg = "该套餐已经是上架状态！";
        //        }
        //    }
        //    else
        //    {
        //        result.Success = false;
        //        result.Msg = "请求失败！";
        //    }
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
    }
}
