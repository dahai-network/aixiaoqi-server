﻿using System;
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
    [RequireRolesOrPermissions(UnitoysPermissionStore.Can_View_Banner)]
    public class BannerController : BaseController
    {
        private IBannerService _bannerService;

        public BannerController(IBannerService bannerService)
        {
            this._bannerService = bannerService;
        }
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取Banner列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetList(int page, int rows, string title, string url, DateTime? createStartDate, DateTime? createEndDate)
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
            var pageRowsDb = await _bannerService.SearchAsync(page, rows, title, url, beginTimeInt, endTimeInt);

            int totalNum = pageRowsDb.Key;

            //过滤掉不必要的字段
            var pageRows = from i in pageRowsDb.Value as List<UT_Banner>
                           select new
                           {
                               ID = i.ID,
                               CreateDate = i.CreateDate.ToString(),
                               Url = i.Url,
                               Image = i.Image,
                               Title = i.Title,
                               DisplayOrder = i.DisplayOrder
                           };

            var jsonResult = new { total = totalNum, rows = pageRows };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Add_Banner)]
        public async Task<ActionResult> Add(UT_Banner model)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (model.Title.Trim() == "")
            {
                result.Success = false;
                result.Msg = "Title不能为空！";
            }
            else
            {

                UT_Banner entity = new UT_Banner();
                entity.Url = model.Url;
                entity.Image = model.Image;
                entity.Title = model.Title;
                entity.DisplayOrder = model.DisplayOrder;
                entity.CreateDate = CommonHelper.GetDateTimeInt();

                if (await _bannerService.InsertAsync(entity))
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
        /// 更新Banner
        /// </summary>
        /// <param name="modal"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Modify_Banner)]
        public async Task<ActionResult> Update(UT_Banner model)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (model.Title.Trim() == "")
            {
                result.Success = false;
                result.Msg = "Title不能为空！";
            }
            else
            {
                UT_Banner entity = await _bannerService.GetEntityByIdAsync(model.ID);

                entity.Url = model.Url;
                entity.Image = model.Image;
                entity.Title = model.Title;
                entity.DisplayOrder = model.DisplayOrder;

                if (await _bannerService.UpdateAsync(entity))
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
        /// 删除Banner
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Delete_Banner)]
        public async Task<ActionResult> Delete(Guid? ID)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (ID.HasValue)
            {
                bool opResult = await _bannerService.DeleteAsync(await _bannerService.GetEntityByIdAsync(ID.Value));

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
