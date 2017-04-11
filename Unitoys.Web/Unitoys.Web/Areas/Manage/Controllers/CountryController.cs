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
    [RequireRolesOrPermissions(UnitoysPermissionStore.Can_View_Country)]
    public class CountryController : BaseController
    {
        private ICountryService _countryService;

        public CountryController() { }

        public CountryController(ICountryService countryService)
        {
            this._countryService = countryService;
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
        public async Task<ActionResult> GetList(int page, int rows, ContinentsType? continents, string countryName, string countryCode, bool? isHot)
        {
            var pageRowsDb = await _countryService.SearchAsync(page, rows, continents, countryName, countryCode, isHot);

            int totalNum = pageRowsDb.Key;

            //过滤掉不必要的字段
            var pageRows = from i in pageRowsDb.Value
                           select new
                           {
                               ID = i.ID,
                               CountryName = i.CountryName,
                               CountryCode = i.CountryCode,
                               Pic = i.Pic.GetCompleteUrl(),
                               LogoPic = i.LogoPic.GetCompleteUrl(),
                               Rate = i.Rate,
                               IsHot = i.IsHot,
                               DisplayOrder=i.DisplayOrder,
                               Continents = i.Continents,
                               Descr = i.Descr,
                           };

            var jsonResult = new { total = totalNum, rows = pageRows };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取所有国家汇率列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public async Task<ActionResult> GetSelectList()
        {
            Expression<Func<UT_Country, bool>> exp = a => 1 == 1;

            var pageRowsDb = await _countryService.GetEntitiesAsync(exp);

            //过滤掉不必要的字段
            var pageRows = from i in pageRowsDb.OrderBy(x=>x.DisplayOrder)
                           select new
                           {
                               ID = i.ID,
                               CountryName = i.CountryName
                           };

            var jsonResult = new { pageRows };

            return Json(pageRows, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Add_Country)]
        public async Task<ActionResult> Add(UT_Country modal)
        {

            JsonAjaxResult result = new JsonAjaxResult();

            if (modal.CountryName.Trim() == "")
            {
                result.Success = false;
                result.Msg = "国家名称不能为空！";
            }
            else if (modal.Rate <= 0)
            {
                result.Success = false;
                result.Msg = "费率标准不能为空！";
            }
            else if (modal.Pic.Trim() == "")
            {
                result.Success = false;
                result.Msg = "图片不能为空！";
            }
            else if (modal.LogoPic.Trim() == "")
            {
                result.Success = false;
                result.Msg = "Logo图片不能为空！";
            }
            else
            {

                UT_Country country = new UT_Country();
                country.CountryName = modal.CountryName;
                country.CountryCode = modal.CountryCode;
                country.IsHot = modal.IsHot;
                country.Continents = modal.Continents;
                country.Rate = modal.Rate;
                country.Pic = modal.Pic;
                country.LogoPic = modal.LogoPic;
                country.CreateDate = DateTime.Now;
                country.DisplayOrder = modal.DisplayOrder;
                country.Descr = modal.Descr;

                if (await _countryService.InsertAsync(country))
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

        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Modify_Country)]
        public async Task<ActionResult> Update(UT_Country model)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (model.CountryName.Trim() == "")
            {
                result.Success = false;
                result.Msg = "国家名称不能为空！";
            }
            else if (model.Rate <= 0)
            {
                result.Success = false;
                result.Msg = "费率标准不能为空！";
            }
            else if (model.Pic.Trim() == "")
            {
                result.Success = false;
                result.Msg = "图片不能为空！";
            }
            else
            {
                UT_Country entity = await _countryService.GetEntityByIdAsync(model.ID);
                entity.CountryName = model.CountryName;
                entity.CountryCode = model.CountryCode;
                entity.IsHot = model.IsHot;
                entity.Continents = model.Continents;
                entity.Rate = model.Rate;
                entity.Pic = StringHelper.TrimStart(model.Pic, UTConfig.SiteConfig.ImageHandleHost);
                entity.LogoPic = StringHelper.TrimStart(model.LogoPic, UTConfig.SiteConfig.ImageHandleHost);
                entity.DisplayOrder = model.DisplayOrder;
                entity.Descr = model.Descr;

                if (await _countryService.UpdateAsync(entity))
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
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Delete_Country)]
        public async Task<ActionResult> Delete(Guid? ID)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (ID.HasValue)
            {
                UT_Country modal = await _countryService.GetEntityByIdAsync(ID.Value);
                await _countryService.DeleteAsync(modal);
                result.Success = true;
                result.Msg = "删除成功！";
            }
            else
            {
                result.Success = false;
                result.Msg = "参数错误！";
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
