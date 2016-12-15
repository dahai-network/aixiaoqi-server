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
    [RequireRolesOrPermissions(UnitoysPermissionStore.Can_View_EjoinDev)]
    public class EjoinDevController : BaseController
    {
        private IEjoinDevService _ejoinDevService;

        public EjoinDevController(IEjoinDevService ejoinDevService)
        {
            this._ejoinDevService = ejoinDevService;
        }
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取EjoinDev列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetList(int page, int rows, string name, int? maxPort, string regIp, RegStatusType? regStatus, ModType? modType)
        {
            var pageRowsDb = await _ejoinDevService.SearchAsync(page, rows, name, maxPort, regIp,regStatus, modType);

            int totalNum = pageRowsDb.Key;

            //过滤掉不必要的字段
            var pageRows = from i in pageRowsDb.Value as List<UT_EjoinDev>
                           select new
                           {
                               ID = i.ID,
                               Name = i.Name.ToString(),
                               Password = i.Password,
                               MaxPort = i.MaxPort,
                               RegStatus = i.RegStatus,
                               RegIp = i.RegIp,
                               Version = i.Version,
                               Mac = i.Mac,
                               ModType = i.ModType
                           };

            var jsonResult = new { total = totalNum, rows = pageRows };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Add_EjoinDev)]
        public async Task<ActionResult> Add(UT_EjoinDev model)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (model.Name.Trim() == "")
            {
                result.Success = false;
                result.Msg = "设备名不能为空！";
            }
            else if (model.Password.Trim() == "")
            {
                result.Success = false;
                result.Msg = "密码不能为空！";
            }
            else if (model.RegIp.Trim() == "")
            {
                result.Success = false;
                result.Msg = "设备注册IP不能为空！";
            }
            else
            {

                UT_EjoinDev entity = new UT_EjoinDev();
                entity.Name = model.Name;
                entity.Password = model.Password;
                entity.MaxPort = model.MaxPort;
                entity.RegStatus = model.RegStatus;
                entity.RegIp = model.RegIp;
                entity.Version = model.Version;
                entity.Mac = model.Mac;
                entity.ModType = model.ModType;

                if (await _ejoinDevService.InsertAsync(entity))
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
        /// 更新EjoinDev
        /// </summary>
        /// <param name="modal"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Modify_EjoinDev)]
        public async Task<ActionResult> Update(UT_EjoinDev model)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (model.Name.Trim() == "")
            {
                result.Success = false;
                result.Msg = "设备名不能为空！";
            }
            else if (model.Password.Trim() == "")
            {
                result.Success = false;
                result.Msg = "密码不能为空！";
            }
            else if (model.RegIp.Trim() == "")
            {
                result.Success = false;
                result.Msg = "设备注册IP不能为空！";
            }
            else
            {
                UT_EjoinDev entity = await _ejoinDevService.GetEntityByIdAsync(model.ID);
                entity.Name = model.Name;
                entity.Password = model.Password;
                entity.MaxPort = model.MaxPort;
                entity.RegStatus = model.RegStatus;
                entity.RegIp = model.RegIp;
                entity.Version = model.Version;
                entity.Mac = model.Mac;
                entity.ModType = model.ModType;

                if (await _ejoinDevService.UpdateAsync(entity))
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
        /// 删除EjoinDev
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Delete_EjoinDev)]
        public async Task<ActionResult> Delete(Guid? ID)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (ID.HasValue)
            {
                bool opResult = await _ejoinDevService.DeleteAsync(await _ejoinDevService.GetEntityByIdAsync(ID.Value));

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
