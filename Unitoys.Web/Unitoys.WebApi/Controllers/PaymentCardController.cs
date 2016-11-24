using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Unitoys.Core;
using Unitoys.IServices;
using Unitoys.Model;
using Unitoys.WebApi.Models;

namespace Unitoys.WebApi.Controllers
{
    /// <summary>
    /// 充值卡API
    /// </summary>
    public class PaymentCardController : ApiController
    {
        private IPaymentCardService _PaymentCardService;
        private IOrderService _orderService;
        public PaymentCardController(IPaymentCardService PaymentCardService, IOrderService orderService)
        {
            this._PaymentCardService = PaymentCardService;
            this._orderService = orderService;
        }



        /// <summary>
        /// 充值
        /// </summary>
        /// <param name="authQuery"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> Recharge([FromBody]PaymentCardBindingModels model)
        {
            var currentUser = WebUtil.GetApiUserSession();

            UT_PaymentCard outModel = new UT_PaymentCard();
            if (string.IsNullOrEmpty(model.CardPwd) || model.CardPwd.Length < 16)
            {
                return Ok(new { status = 0, msg = "请输入16位完整卡密码！" });
            }
            var result = await _PaymentCardService.Recharge(currentUser.ID, model.CardPwd, outModel);
            switch (result)
            {
                case 0:
                    return Ok(new { status = 0, msg = "充值失败！" });
                case 1:
                    return Ok(new { status = 1, msg = "充值成功！", data = new { CardNum = outModel.CardNum, Pirce = outModel.Price.ToString() } });
                case 2:
                    return Ok(new { status = 0, msg = "充值卡已被使用或失效！" });
                case 3:
                    return Ok(new { status = 0, msg = "充值卡已超过最晚可用时间！" });
                default:
                    return Ok(new { status = 0, msg = "充值异常，请重试！" });
            }

        }

    }
}
