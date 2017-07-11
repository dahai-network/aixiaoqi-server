using System;
using System.Collections.Generic;
using System.Linq;
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
    [RequireRolesOrPermissions(UnitoysPermissionStore.Can_View_Order)]
    public class OrderController : Controller
    {
        private IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            this._orderService = orderService;
        }

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取订单列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetOrders(int page, int rows, string orderNum, string tel, string packageName, PayStatusType? payStatus, OrderStatusType? orderStatus)
        {
            var result = await _orderService.SearchAsync(page, rows, orderNum, tel, packageName, payStatus, orderStatus);

            int totalNum = result.Key;

            //过滤掉不必要的字段
            var pageRows = from i in result.Value
                           select new
                           {
                               ID = i.ID,
                               UserId = i.UserId,
                               Tel = i.UT_Users.Tel,
                               OrderDate = i.OrderDate.ToString(),
                               OrderNum = i.OrderNum,
                               PackageId = i.PackageId,
                               PackageName = i.PackageName,
                               Flow = i.Flow,
                               CallMinutes = i.UT_Package == null ? "" : i.UT_Package.CallMinutes + "",
                               CountryName = i.UT_Package.UT_Country == null ? "" : i.UT_Package.UT_Country.CountryName,
                               RemainingCallMinutes = i.RemainingCallMinutes,
                               ExpireDays = i.ExpireDays,
                               Quantity = i.Quantity,
                               UnitPrice = i.UnitPrice,
                               TotalPrice = i.TotalPrice,
                               PayDate = i.PayDate.ToString(),
                               PayStatus = i.PayStatus,
                               OrderStatus = i.OrderStatus,
                               EffectiveDate = i.EffectiveDate.ToString(),
                               ActivationDate = i.ActivationDate.ToString(),
                               Remark = i.Remark,
                               PaymentMethod = i.PaymentMethod
                           };

            var jsonResult = new { total = totalNum, rows = pageRows };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 设置已付款操作
        /// </summary>
        /// <param name="LoginName"></param>
        /// <param name="PassWord"></param>
        /// <param name="TrueName"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Modify_Order)]
        public async Task<ActionResult> SetPayStatus(Guid? ID)
        {
            JsonAjaxResult result = new JsonAjaxResult();
            if (ID.HasValue)
            {
                UT_Order order = await _orderService.GetEntityByIdAsync(ID.Value);
                if (order.PayStatus != PayStatusType.YesPayment)
                {
                    string orderOrPayment = order.OrderNum;
                    if (orderOrPayment.StartsWith("8022", StringComparison.OrdinalIgnoreCase))
                    {
                        //处理订单完成。
                        if (await _orderService.OnAfterOrderSuccess(orderOrPayment, order.Quantity * order.UnitPrice) == 0)
                        {
                            result.Success = true;
                            result.Msg = "更新成功！";
                        }
                    }
                }
                else
                {
                    result.Success = false;
                    result.Msg = "该订单已经是已付款状态！";
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
        /// 设置取消订单操作
        /// </summary>
        /// <param name="LoginName"></param>
        /// <param name="PassWord"></param>
        /// <param name="TrueName"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Modify_Order)]
        public async Task<ActionResult> Cancel(Guid? ID)
        {
            JsonAjaxResult result = new JsonAjaxResult();
            if (ID.HasValue)
            {
                UT_Order order = await _orderService.GetEntityByIdAsync(ID.Value);
                int resultNum = await _orderService.CancelOrder(order.UserId, order.ID);

                if (resultNum == 0)
                {
                    result.Success = true;
                    result.Msg = "取消成功！";
                }
                else if (resultNum == -2)
                {
                    result.Success = false;
                    result.Msg = "订单已经被取消！";
                }
                else if (resultNum == -3)
                {
                    result.Success = false;
                    result.Msg = "此订单不属于该用户！";
                }
                else if (resultNum == -4)
                {
                    result.Success = false;
                    result.Msg = "订单已被使用！";
                }
                else if (resultNum == -5)
                {
                    result.Success = false;
                    result.Msg = "订单不允许取消！";

                }
                else
                {
                    result.Success = false;
                    result.Msg = "取消失败！";
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