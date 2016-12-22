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
    public class PackageController : BaseController
    {
        private IPackageService _packageService;
        private IOrderService _orderService;
        private ICountryService _countryService;
        public PackageController() { }

        public PackageController(IPackageService packageService, IOrderService orderService, ICountryService countryService)
        {
            this._packageService = packageService;
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
        public async Task<ActionResult> GetList(int page, int rows, string packageName, string countryId, string operators, CategoryType? category)
        {
            var pageRowsDb = await _packageService.SearchAsync(page, rows, packageName, countryId, operators, category);

            int totalNum = pageRowsDb.Key;

            //过滤掉不必要的字段
            var pageRows = from i in pageRowsDb.Value as List<UT_Package>
                           select new
                           {
                               ID = i.ID,
                               PackageName = i.PackageName,
                               PackageNum = i.PackageNum,
                               Price = i.Price,
                               Flow = i.Flow,
                               CallMinutes = i.CallMinutes,
                               Pic = i.Pic.GetCompleteUrl(),
                               ExpireDays = i.ExpireDays,
                               CountryId = i.CountryId,
                               CountryName = i.UT_Country == null ? "" : i.UT_Country.CountryName,
                               Category = i.Category,
                               CategoryDescr = i.Category.GetDescription(),
                               Lock4 = i.Lock4,
                               Operators = i.Operators,
                               Features = i.Features,
                               Details = i.Details,
                               DisplayOrder = i.DisplayOrder,
                               //Category=i.Category
                           };

            var jsonResult = new { total = totalNum, rows = pageRows };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Add_Package)]
        public async Task<ActionResult> Add(UT_Package model)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (model.PackageName.Trim() == "")
            {
                result.Success = false;
                result.Msg = "套餐标题不能为空！";
            }
            else if (model.Price <= 0)
            {
                result.Success = false;
                result.Msg = "价格错误！";
            }
            else if (model.Flow < 0)
            {
                result.Success = false;
                result.Msg = "套餐流量错误！";
            }
            else if (model.Pic.Trim() == "")
            {
                result.Success = false;
                result.Msg = "图片不能为空！";
            }
            else if (model.ExpireDays <= 0)
            {
                result.Success = false;
                result.Msg = "套餐有效天数设置错误！";
            }
            else if (await _packageService.GetEntitiesCountAsync(x => x.PackageNum == model.PackageNum) > 0)
            {
                result.Success = false;
                result.Msg = "该套餐编号已存在！";
            }
            else
            {

                UT_Package package = new UT_Package();
                package.PackageName = model.PackageName;
                package.Price = model.Price;
                package.Flow = model.Flow;
                package.CallMinutes = model.CallMinutes;
                package.Pic = model.Pic;
                package.Lock4 = 0;
                package.ExpireDays = model.ExpireDays;
                package.CountryId = model.CountryId;
                package.PackageNum = model.PackageNum;
                package.Operators = model.Operators;
                package.Features = model.Features;
                package.Details = model.Details;
                package.DisplayOrder = model.DisplayOrder;
                package.Category = model.Category;

                if (await _packageService.InsertAsync(package))
                {
                    result.Success = true;
                    result.Msg = "添加成功！";
                }
                else
                {
                    result.Success = false;
                    result.Msg = "操作失败！";
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 更新套餐
        /// </summary>
        /// <param name="modal"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Modify_Package)]
        public async Task<ActionResult> Update(UT_Package model)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (model.PackageName.Trim() == "")
            {
                result.Success = false;
                result.Msg = "套餐标题不能为空！";
            }
            else if (model.Price <= 0)
            {
                result.Success = false;
                result.Msg = "价格错误！";
            }
            else if (model.Flow < 0)
            {
                result.Success = false;
                result.Msg = "套餐流量错误！";
            }
            else if (model.Pic.Trim() == "")
            {
                result.Success = false;
                result.Msg = "图片不能为空！";
            }
            else if (model.ExpireDays <= 0)
            {
                result.Success = false;
                result.Msg = "套餐有效天数设置错误！";
            }
            else
            {
                UT_Package package = await _packageService.GetEntityByIdAsync(model.ID);
                if (package.PackageNum != model.PackageNum &&
                    await _packageService.GetEntitiesCountAsync(x => x.PackageNum == model.PackageNum) > 0)
                {
                    result.Success = false;
                    result.Msg = "该套餐编号已存在！";
                }
                if (string.IsNullOrEmpty(result.Msg))
                {
                    package.PackageName = model.PackageName;
                    package.Price = model.Price;
                    package.Flow = model.Flow;
                    package.Pic = StringHelper.TrimStart(model.Pic, UTConfig.SiteConfig.ImageHandleHost);
                    package.ExpireDays = model.ExpireDays;
                    package.CountryId = model.CountryId;
                    package.PackageNum = model.PackageNum;
                    package.Operators = model.Operators;
                    package.CallMinutes = model.CallMinutes;
                    package.Features = model.Features;
                    package.Details = model.Details;
                    package.DisplayOrder = model.DisplayOrder;
                    package.Category = model.Category;

                    if (await _packageService.UpdateAsync(package))
                    {
                        result.Success = true;
                        result.Msg = "更新成功！";
                    }
                    else
                    {
                        result.Success = false;
                        result.Msg = "更新失败！";
                    }
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 删除套餐
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Delete_Package)]
        public async Task<ActionResult> Delete(Guid? ID)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (ID.HasValue)
            {
                bool opResult = false;
                //是否有用户购买此套餐，没有则删除，如果有则假删除
                if (await _orderService.GetEntitiesCountAsync(c => c.PackageId.Equals(ID.Value)) > 0)
                {
                    UT_Package package = await _packageService.GetEntityByIdAsync(ID.Value);
                    package.IsDeleted = true;
                    opResult = await _packageService.UpdateAsync(package);
                }
                else
                {
                    opResult = await _packageService.DeleteAsync(await _packageService.GetEntityByIdAsync(ID.Value));
                }

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
                result.Msg = "次数错误！";
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 下架套餐操作
        /// </summary>
        /// <param name="LoginName"></param>
        /// <param name="PassWord"></param>
        /// <param name="TrueName"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Modify_Package)]
        public async Task<ActionResult> Lock(Guid? ID)
        {
            JsonAjaxResult result = new JsonAjaxResult();
            if (ID.HasValue)
            {

                UT_Package package = await _packageService.GetEntityByIdAsync(ID.Value);
                if (package.Lock4 != 1)
                {
                    package.Lock4 = 1;
                    if (await _packageService.UpdateAsync(package))
                    {
                        result.Success = true;
                        result.Msg = "操作成功！";
                    }
                    else
                    {
                        result.Success = false;
                        result.Msg = "操作失败！";
                    }
                }
                else
                {
                    result.Success = false;
                    result.Msg = "该套餐已经是下架状态！";
                }

            }
            else
            {
                result.Success = false;
                result.Msg = "请求失败！";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 上架套餐操作
        /// </summary>
        /// <param name="LoginName"></param>
        /// <param name="PassWord"></param>
        /// <param name="TrueName"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Modify_Package)]
        public async Task<ActionResult> uLock(Guid? ID)
        {
            JsonAjaxResult result = new JsonAjaxResult();
            if (ID.HasValue)
            {
                UT_Package package = await _packageService.GetEntityByIdAsync(ID.Value);
                if (package.Lock4 != 0)
                {
                    package.Lock4 = 0;
                    if (await _packageService.UpdateAsync(package))
                    {
                        result.Success = true;
                        result.Msg = "更新成功！";
                    }
                    else
                    {
                        result.Success = false;
                        result.Msg = "操作失败！";
                    }
                }
                else
                {
                    result.Success = false;
                    result.Msg = "该套餐已经是上架状态！";
                }
            }
            else
            {
                result.Success = false;
                result.Msg = "请求失败！";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
