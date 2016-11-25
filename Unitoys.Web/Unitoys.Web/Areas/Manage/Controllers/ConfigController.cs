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
    [RequireRolesOrPermissions(UnitoysPermissionStore.Can_View_Config)]
    public class ConfigController : BaseController
    {
        public ConfigController()
        {

        }
        public ActionResult Index()
        {
            var SiteConfigNameList = UTConfig.SiteConfig.GetType().GetProperties()
               .Select(x => x.Name )
               .ToList();

            ViewBag.SiteConfigNameList = SiteConfigNameList;
            return View();
        }

        /// <summary>
        /// 获取设备手环列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetList(int page, int rows)
        {
            var list = (new int[] { 1 })
                .Select(x => new { Id = "SiteConfig", ConfigDescr = "系统配置", ConfigName = "SiteConfig", Config = (Unitoys.Core.Config.IConfigInfo)UTConfig.SiteConfig }).ToList();
            list.Add(new { Id = "DeviceBraceletOTAConfig", ConfigDescr = "手环固件OTA信息配置", ConfigName = "DeviceBraceletOTAConfig", Config = (Unitoys.Core.Config.IConfigInfo)UTConfig.DeviceBraceletOTAConfigInfo });

            var jsonResult = new { total = 2, rows = list };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 更新系统配置
        /// </summary>
        /// <param name="modal"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Modify_Config)]
        public async Task<ActionResult> UpdateSiteConfig(Core.Config.SiteConfigInfo model)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (new Core.Config.ConfigStrategy().SaveSiteConfig(model))
            {
                UTConfig.SetSiteConfig(model);
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

        /// <summary>
        /// 更新手环固件OTA信息配置
        /// </summary>
        /// <param name="modal"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Modify_Config)]
        public async Task<ActionResult> UpdateDeviceBraceletOTAConfig(Core.Config.DeviceBraceletOTAConfigInfo model)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (new Core.Config.ConfigStrategy().SaveDeviceBraceletOTAConfig(model))
            {
                UTConfig.SetDeviceBraceletOTAConfig(model);
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
    }
}
