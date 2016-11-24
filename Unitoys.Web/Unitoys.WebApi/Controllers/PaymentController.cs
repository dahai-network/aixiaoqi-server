using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Unitoys.Core;
using Unitoys.IServices;
using Unitoys.Model;
using Unitoys.WebApi.Models;
using XKSocket;

namespace Unitoys.WebApi.Controllers
{
    public class PaymentController : ApiController
    {
        private IPaymentService _paymentService;
        public PaymentController(IPaymentService paymentService)
        {
            this._paymentService = paymentService;
        }

        /// <summary>
        /// 创建充值余额的付款信息
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> Add([FromBody]AddPaymentBindingModel model)
        {
            string errorMsg = "";

            var currentUser = WebUtil.GetApiUserSession();

            if (currentUser == null)
            {
                errorMsg = "当前用户不能为空！";
            }
            else if (model.Amount <= 0)
            {
                errorMsg = "充值金额需要大于0元！";
            }
            else if (model.PaymentMethod != PaymentMethodType.AliPay && model.PaymentMethod != PaymentMethodType.WxPay)
            {
                errorMsg = "无效的支付方式！";
            }
            else
            {
                var result = await _paymentService.Add(currentUser.ID,model.Amount,model.PaymentMethod);

                if (result != null)
                {
                    return Ok(new
                    {
                        status = 1,
                        msg = "充值订单添加成功！",
                        data = new
                        {
                            payment = new
                            {
                                PaymentNum = result.PaymentNum,
                                Amount = result.Amount,
                                OrderDate = CommonHelper.ConvertDateTimeInt(result.CreateDate).ToString()
                            }
                        }
                    });
                }
                else
                {
                    errorMsg = "充值订单添加失败！";
                }
            }

            return Ok(new { status = 0, msg = errorMsg });
        }


    }
}
