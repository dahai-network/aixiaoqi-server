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
    }
}
