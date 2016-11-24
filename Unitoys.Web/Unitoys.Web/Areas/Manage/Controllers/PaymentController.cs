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
    [RequireRolesOrPermissions(UnitoysPermissionStore.Can_View_Payment)]
    public class PaymentController : BaseController
    {
        private IPaymentService _paymentService;

        public PaymentController() { }

        public PaymentController(IPaymentService paymentService)
        {
            this._paymentService = paymentService;
        }
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取在线支付订单信息
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetList(int page, int rows, string paymentNum, string tel, DateTime? createStartDate, DateTime? createEndDate)
        {
            var pageRowsDb = await _paymentService.GetPaymentsIncludeUser(page, rows, paymentNum, tel, createStartDate, createEndDate);

            int totalNum = pageRowsDb.Key;

            //过滤掉不必要的字段
            var pageRows = from i in pageRowsDb.Value as List<UT_Payment>
                           select new
                           {
                               ID = i.ID,
                               UserId = i.UserId,
                               Tel = i.UT_Users.Tel,
                               PaymentMethod = i.PaymentMethod,
                               PaymentPurpose = i.PaymentPurpose,
                               PayOrReceive = i.PayOrReceive,
                               Amount = i.Amount,
                               CreateDate = i.CreateDate.ToString(),
                               PaymentComfirmDate = i.PaymentConfirmDate.ToString(),
                               Status = i.Status,
                               PaymentNum = i.PaymentNum,
                               Remark = i.Remark,
                           };

            var jsonResult = new { total = totalNum, rows = pageRows };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改充值记录状态为已付款
        /// </summary>
        /// <param name="paymentId">充值记录ID</param>
        /// <returns></returns>          
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Modify_Payment)]
        public async Task<ActionResult> ChangeStatus(Guid paymentId)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (paymentId == Guid.Empty)
            {
                result.Success = false;
                result.Msg = "ID不能为空！";
            }
            else
            {
                UT_Payment payment = await _paymentService.GetEntityByIdAsync(paymentId);

                if (payment == null)
                {
                    result.Success = false;
                    result.Msg = "该充值记录不存在！";
                }
                else
                {
                    if (payment.Status == 1)
                    {
                        result.Success = false;
                        result.Msg = "该充值记录状态已经是已付款！";
                    }
                    else
                    {
                        payment.Status = 1;
                    }

                    if (await _paymentService.UpdateAsync(payment))
                    {
                        result.Success = true;
                        result.Msg = "修改成功！";
                    }
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
