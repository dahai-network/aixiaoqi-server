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
    [RequireRolesOrPermissions(UnitoysPermissionStore.Can_View_OrderByZC)]
    public class OrderByZCController : BaseController
    {
        private IOrderByZCService _orderByZCService;

        public OrderByZCController(IOrderByZCService orderByZCService)
        {
            this._orderByZCService = orderByZCService;
        }
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取OrderByZC列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetList(int page, int rows, string orderNum, string name, string callPhone, string address, PayStatusType? payStatus)
        {
            var pageRowsDb = await _orderByZCService.SearchAsync(page, rows, orderNum, name, callPhone, address, payStatus);

            int totalNum = pageRowsDb.Key;

            //过滤掉不必要的字段
            var pageRows = from i in pageRowsDb.Value as List<UT_OrderByZC>
                           select new
                           {
                               ID = i.ID,
                               OrderDate = i.OrderDate.ToString(),
                               OrderByZCNum = i.OrderByZCNum,
                               Quantity = i.Quantity,
                               UnitPrice = i.UnitPrice,
                               TotalPrice = i.TotalPrice,
                               Name = i.Name,
                               CallPhone = i.CallPhone,
                               Address = i.Address,
                               GiftProperties = i.GiftProperties,
                               Remark = i.Remark
                           };

            var jsonResult = new { total = totalNum, rows = pageRows };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Add_OrderByZC)]
        public async Task<ActionResult> Add(UT_OrderByZC model)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (model.OrderByZCNum.Trim() == "")
            {
                result.Success = false;
                result.Msg = "订单号不能为空！";
            }
            else if (model.UnitPrice == 0)
            {
                result.Success = false;
                result.Msg = "单价不能为空！";
            }
            else if (model.Quantity == 0)
            {
                result.Success = false;
                result.Msg = "数量不能为空！";
            }
            else if (model.TotalPrice == 0)
            {
                result.Success = false;
                result.Msg = "总价不能为空！";
            }
            else if (model.Name.Trim() == "")
            {
                result.Success = false;
                result.Msg = "收货姓名不能为空！";
            }
            else if (model.CallPhone.Trim() == "")
            {
                result.Success = false;
                result.Msg = "联系电话不能为空！";
            }
            else if (model.Address.Trim() == "")
            {
                result.Success = false;
                result.Msg = "地址不能为空！";
            }
            else if (model.GiftProperties.Trim() == "")
            {
                result.Success = false;
                result.Msg = "回报属性不能为空！";
            }
            else if (model.OrderDate == 0)
            {
                result.Success = false;
                result.Msg = "订单日期不能为空！";
            }
            else
            {

                UT_OrderByZC entity = new UT_OrderByZC();
                entity.OrderByZCNum = model.OrderByZCNum;
                entity.UnitPrice = model.UnitPrice;
                entity.Quantity = model.Quantity;
                entity.TotalPrice = model.TotalPrice;
                entity.Name = model.Name;
                entity.CallPhone = model.CallPhone;
                entity.Address = model.Address;
                entity.GiftProperties = model.GiftProperties;
                entity.OrderDate = model.OrderDate;

                if (await _orderByZCService.InsertAsync(entity))
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
        /// 更新OrderByZC
        /// </summary>
        /// <param name="modal"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Modify_OrderByZC)]
        public async Task<ActionResult> Update(UT_OrderByZC model)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (model.OrderByZCNum.Trim() == "")
            {
                result.Success = false;
                result.Msg = "订单号不能为空！";
            }
            else if (model.UnitPrice == 0)
            {
                result.Success = false;
                result.Msg = "单价不能为空！";
            }
            else if (model.Quantity == 0)
            {
                result.Success = false;
                result.Msg = "数量不能为空！";
            }
            else if (model.TotalPrice == 0)
            {
                result.Success = false;
                result.Msg = "总价不能为空！";
            }
            else if (model.Name.Trim() == "")
            {
                result.Success = false;
                result.Msg = "收货姓名不能为空！";
            }
            else if (model.CallPhone.Trim() == "")
            {
                result.Success = false;
                result.Msg = "联系电话不能为空！";
            }
            else if (model.Address.Trim() == "")
            {
                result.Success = false;
                result.Msg = "地址不能为空！";
            }
            else if (model.GiftProperties.Trim() == "")
            {
                result.Success = false;
                result.Msg = "回报属性不能为空！";
            }
            else if (model.OrderDate == 0)
            {
                result.Success = false;
                result.Msg = "订单日期不能为空！";
            }
            else
            {
                UT_OrderByZC entity = await _orderByZCService.GetEntityByIdAsync(model.ID);

                entity.OrderByZCNum = model.OrderByZCNum;
                entity.UnitPrice = model.UnitPrice;
                entity.Quantity = model.Quantity;
                entity.TotalPrice = model.TotalPrice;
                entity.Name = model.Name;
                entity.CallPhone = model.CallPhone;
                entity.Address = model.Address;
                entity.GiftProperties = model.GiftProperties;
                entity.OrderDate = model.OrderDate;

                if (await _orderByZCService.UpdateAsync(entity))
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
        /// 删除OrderByZC
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Delete_OrderByZC)]
        public async Task<ActionResult> Delete(Guid? ID)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (ID.HasValue)
            {
                bool opResult = await _orderByZCService.DeleteAsync(await _orderByZCService.GetEntityByIdAsync(ID.Value));

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
