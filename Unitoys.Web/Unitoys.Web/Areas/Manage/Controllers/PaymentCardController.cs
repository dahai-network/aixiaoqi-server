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
    [RequireRolesOrPermissions(UnitoysPermissionStore.Can_View_PaymentCard)]
    public class PaymentCardController : BaseController
    {
        private IPaymentCardService _paymentCardService;

        public PaymentCardController(IPaymentCardService paymentCardService)
        {
            this._paymentCardService = paymentCardService;
        }
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取充值卡列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetList(int page, int rows, string cardNum, DateTime? createStartDate, DateTime? createEndDate, PaymentCardStatusType? status)
        {
            var pageRowsDb = await _paymentCardService.SearchAsync(page, rows, cardNum, createStartDate, createEndDate, status);

            int totalNum = pageRowsDb.Key;

            //过滤掉不必要的字段
            var pageRows = from i in pageRowsDb.Value as List<UT_PaymentCard>
                           select new
                           {
                               ID = i.ID,
                               CardNum = i.CardNum,
                               CardPwd = i.CardPwd,
                               Price = i.Price,
                               CreateDate = i.CreateDate.ToString(),
                               CreateManageUsersName = i.UT_ManageUsers.LoginName,
                               PaymentDate = i.PaymentDate,
                               LastEffectiveDate = i.LastEffectiveDate,
                               Tel = i.UT_Users == null ? "" : i.UT_Users.Tel,
                               Status = i.Status
                           };

            var jsonResult = new { total = totalNum, rows = pageRows };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Add_PaymentCard)]
        public async Task<ActionResult> Add(int qty, int price)
        {
            JsonAjaxResult result = new JsonAjaxResult();
            if (qty <= 0)
            {
                result.Success = false;
                result.Msg = "数量错误！";
            }
            else if (price <= 0)
            {
                result.Success = false;
                result.Msg = "金额错误！";
            }
            else
            {

                if (await _paymentCardService.AddGenerateCard(WebUtil.GetManageUserSession().ID, qty, price))
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
        /// 更新充值卡
        /// </summary>
        /// <param name="modal"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Modify_PaymentCard)]
        public async Task<ActionResult> Update(UT_PaymentCard model)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (model.CardNum.Trim() == "")
            {
                result.Success = false;
                result.Msg = "充值卡不能为空！";
            }
            else if (model.UserId == Guid.Empty)
            {
                result.Success = false;
                result.Msg = "用户不能为空！";
            }
            else
            {
                UT_PaymentCard entity = await _paymentCardService.GetEntityByIdAsync(model.ID);
                entity.CardNum = model.CardNum;
                entity.CardPwd = model.CardPwd;
                //entity.UserId = model.UserId;
                if (await _paymentCardService.UpdateAsync(entity))
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
        /// 删除充值卡
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Delete_PaymentCard)]
        public async Task<ActionResult> Delete(Guid? ID)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (ID.HasValue)
            {
                bool opResult = await _paymentCardService.DeleteAsync(await _paymentCardService.GetEntityByIdAsync(ID.Value));

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
        /// 失效充值卡操作
        /// </summary>
        /// <param name="LoginName"></param>
        /// <param name="PassWord"></param>
        /// <param name="TrueName"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Modify_PaymentCard)]
        public async Task<ActionResult> Invalid(Guid? ID)
        {
            JsonAjaxResult result = new JsonAjaxResult();
            if (ID.HasValue)
            {

                UT_PaymentCard paymentCard = await _paymentCardService.GetEntityByIdAsync(ID.Value);
                if (paymentCard.Status == PaymentCardStatusType.Disabled)
                {
                    result.Success = false;
                    result.Msg = "该充值卡目前是已使用状态！";
                }
                else if (paymentCard.Status != PaymentCardStatusType.Invalid)
                {
                    paymentCard.Status = PaymentCardStatusType.Invalid;
                    if (await _paymentCardService.UpdateAsync(paymentCard))
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
                    result.Msg = "该充值卡已经是失效状态！";
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
