using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Unitoys.Core;
using Unitoys.IServices;
using Unitoys.Model;
using Unitoys.WebApi.Models;

namespace Unitoys.WebApi.Controllers
{
    public class OrderByZCSelectionNumberController : ApiController
    {
        private IOrderByZCSelectionNumberService _orderByZCSelectionNumberService;
        public OrderByZCSelectionNumberController(IOrderByZCSelectionNumberService orderByZCSelectionNumberService)
        {
            this._orderByZCSelectionNumberService = orderByZCSelectionNumberService;
        }

        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> Add([FromBody]AddOrderByZCSelectionNumberBindingModel model)
        {
            string errorMsg = "";

            var currentUser = WebUtil.GetApiUserSession();
            //System.Web.Http.ModelBinding.DefaultActionValueBinder
            if (model.OrderByZCId == Guid.Empty)
            {
                errorMsg = "订单ID不能为空！";
            }
            else if (string.IsNullOrEmpty(model.MobileNumber))
            {
                //errorMsg = "包月订单只能购买一个，待后续业务需求是否需要调整！";
                errorMsg = "手机号码不能为空";
            }
            else if (!Enum.IsDefined(typeof(PaymentMethodType), model.PaymentMethod))
            {
                errorMsg = "无效的支付方式！";
            }
            else
            {
                UT_OrderByZCSelectionNumber order = await _orderByZCSelectionNumberService.AddOrder(currentUser.ID, model.OrderByZCId, model.Name, model.IdentityNumber, model.MobileNumber, model.PaymentMethod);
                if (order != null)
                {
                    var resultModel = new
                    {
                        OrderID = order.ID,
                        OrderByZCSelectionNumberNum = order.OrderByZCSelectionNumberNum,
                        OrderDate = order.OrderDate.ToString(),
                        PaymentMethod = (int)order.PaymentMethod + ""
                    };
                    return Ok(new { status = 1, msg = "订单创建成功！", data = new { order = resultModel } });
                }
                else
                {
                    errorMsg = "订单创建失败！";
                }
            }
            return Ok(new { status = 0, msg = errorMsg });
        }

        /// <summary>
        /// 通过用户余额支付众筹选号订单
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> PayOrderByZCSelectionNumberByUserAmount([FromBody]PayOrderByZCSelectionNumberByUserAmountBindingModel model)
        {
            string errorMsg = "";

            var currentUser = WebUtil.GetApiUserSession();

            if (currentUser == null)
            {
                errorMsg = "当前用户不能为空！";
            }
            else
            {
                int resultNum = await _orderByZCSelectionNumberService.PayOrderByUserAmount(currentUser.ID, model.OrderByZCId);

                if (resultNum == 0)
                {
                    return Ok(new { status = 1, msg = "支付成功！" });
                }
                else if (resultNum == -2)
                {
                    errorMsg = "此订单已经支付成功，不能再支付！";
                }
                else if (resultNum == -3)
                {
                    errorMsg = "此订单不属于该用户！";
                }
                else if (resultNum == -4)
                {
                    errorMsg = "用户余额不足！";
                }
                else if (resultNum == -5)
                {
                    errorMsg = "支付方式异常！";
                }
                else if (resultNum == -6)
                {
                    errorMsg = "号码已被选择！";
                }
                else
                {
                    errorMsg = "支付失败！";
                }
            }
            return Ok(new { status = 0, msg = errorMsg });
        }
    }
}
