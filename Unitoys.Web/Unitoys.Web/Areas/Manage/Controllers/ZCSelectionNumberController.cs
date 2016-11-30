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
    [RequireRolesOrPermissions(UnitoysPermissionStore.Can_View_ZCSelectionNumber)]
    public class ZCSelectionNumberController : BaseController
    {
        private IZCSelectionNumberService _zCSelectionNumberService;

        public ZCSelectionNumberController(IZCSelectionNumberService zCSelectionNumberService)
        {
            this._zCSelectionNumberService = zCSelectionNumberService;
        }
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取ZCSelectionNumber列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetList(int page, int rows, string provinceName, string cityName, string mobileNumber, string userTel)
        {
            var pageRowsDb = await _zCSelectionNumberService.SearchAsync(page, rows, provinceName, cityName, mobileNumber, userTel);

            int totalNum = pageRowsDb.Key;

            //过滤掉不必要的字段
            var pageRows = from i in pageRowsDb.Value as List<UT_ZCSelectionNumber>
                           select new
                           {
                               ID = i.ID,
                               CreateDate = i.CreateDate.ToString(),
                               ProvinceName = i.ProvinceName,
                               CityName = i.CityName,
                               MobileNumber = i.MobileNumber,
                               Price = i.Price,
                               Tel = i.OrderByZCSelectionNumberId != null ? i.UT_OrderByZCSelectionNumber.UT_Users.Tel : ""
                           };

            var jsonResult = new { total = totalNum, rows = pageRows };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Add_ZCSelectionNumber)]
        public async Task<ActionResult> Add(UT_ZCSelectionNumber model)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (model.ProvinceName.Trim() == "")
            {
                result.Success = false;
                result.Msg = "ProvinceName不能为空！";
            }
            else if (model.CityName.Trim() == "")
            {
                result.Success = false;
                result.Msg = "CityName不能为空！";
            }
            else if (model.MobileNumber.Trim() == "")
            {
                result.Success = false;
                result.Msg = "MobileNumber不能为空！";
            }
            else
            {

                UT_ZCSelectionNumber entity = new UT_ZCSelectionNumber();
                entity.ProvinceName = model.ProvinceName;
                entity.CityName = model.CityName;
                entity.MobileNumber = model.MobileNumber;
                entity.Price = model.Price;
                entity.CreateDate = CommonHelper.GetDateTimeInt();

                if (await _zCSelectionNumberService.InsertAsync(entity))
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
        /// 更新ZCSelectionNumber
        /// </summary>
        /// <param name="modal"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Modify_ZCSelectionNumber)]
        public async Task<ActionResult> Update(UT_ZCSelectionNumber model)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (model.ProvinceName.Trim() == "")
            {
                result.Success = false;
                result.Msg = "ProvinceName不能为空！";
            }
            else if (model.CityName.Trim() == "")
            {
                result.Success = false;
                result.Msg = "CityName不能为空！";
            }
            else if (model.MobileNumber.Trim() == "")
            {
                result.Success = false;
                result.Msg = "MobileNumber不能为空！";
            }
            else
            {
                UT_ZCSelectionNumber entity = await _zCSelectionNumberService.GetEntityByIdAsync(model.ID);
                entity.ProvinceName = model.ProvinceName;
                entity.CityName = model.CityName;
                entity.MobileNumber = model.MobileNumber;
                entity.Price = model.Price;

                if (await _zCSelectionNumberService.UpdateAsync(entity))
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

        /// <summary>
        /// 删除ZCSelectionNumber
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Delete_ZCSelectionNumber)]
        public async Task<ActionResult> Delete(Guid? ID)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (ID.HasValue)
            {
                bool opResult = await _zCSelectionNumberService.DeleteAsync(await _zCSelectionNumberService.GetEntityByIdAsync(ID.Value));

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
                result.Msg = "参数错误！";
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}
