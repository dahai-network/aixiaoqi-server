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
    [RequireRolesOrPermissions(UnitoysPermissionStore.Can_View_DeviceGoip)]
    public class DeviceGoipController : BaseController
    {
        private IDeviceGoipService _deviceGoipService;

        public DeviceGoipController(IDeviceGoipService deviceGoipService)
        {
            this._deviceGoipService = deviceGoipService;
        }
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取设备手环列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetList(int page, int rows, string tel, DateTime? createStartDate, DateTime? createEndDate, DeviceGoipStatus? status)
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
            var pageRowsDb = await _deviceGoipService.SearchAsync(page, rows, tel, beginTimeInt, endTimeInt, status);

            int totalNum = pageRowsDb.Key;

            //过滤掉不必要的字段
            var pageRows = from i in pageRowsDb.Value as List<UT_DeviceGoip>
                           select new
                           {
                               ID = i.ID,
                               Mac = i.Mac,
                               DeviceName = i.DeviceName,
                               Status = i.Status,
                               Port = i.Port,
                               CreateDate = i.CreateDate.ToString(),
                               UpdateDate = i.UpdateDate.ToString(),
                               UserId = i.UserId,
                               IccId = i.IccId,
                               Tel = i.UT_Users == null ? "" : i.UT_Users.Tel,
                           };

            var jsonResult = new { total = totalNum, rows = pageRows };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Add_DeviceGoip)]
        public async Task<ActionResult> Add(UT_DeviceGoip model)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (model.Mac.Trim() == "")
            {
                result.Success = false;
                result.Msg = "Mac不能为空！";
            }
            else
            {

                UT_DeviceGoip entity = new UT_DeviceGoip();
                entity.Mac = model.Mac;
                entity.DeviceName = model.DeviceName;
                entity.Status = model.Status;
                entity.Port = model.Port;
                entity.IccId = model.IccId;
                entity.CreateDate = CommonHelper.GetDateTimeInt();
                entity.UpdateDate = CommonHelper.GetDateTimeInt();

                if (await _deviceGoipService.InsertAsync(entity))
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
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Modify_DeviceGoip)]
        public async Task<ActionResult> Update(UT_DeviceGoip model)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (model.Mac.Trim() == "")
            {
                result.Success = false;
                result.Msg = "Mac不能为空！";
            }
            else
            {
                UT_DeviceGoip entity = await _deviceGoipService.GetEntityByIdAsync(model.ID);

                entity.Mac = model.Mac;
                entity.DeviceName = model.DeviceName;
                entity.Status = model.Status;
                entity.Port = model.Port;
                entity.IccId = model.IccId;
                entity.UpdateDate = CommonHelper.GetDateTimeInt();

                if (await _deviceGoipService.UpdateAsync(entity))
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
        /// 删除GOIP
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Delete_DeviceBracelet)]
        public async Task<ActionResult> Delete(Guid? ID)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (ID.HasValue)
            {
                bool opResult = await _deviceGoipService.DeleteAsync(await _deviceGoipService.GetEntityByIdAsync(ID.Value));

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
