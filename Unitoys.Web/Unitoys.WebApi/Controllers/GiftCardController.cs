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
    public class GiftCardController : ApiController
    {
        private IGiftCardService _giftCardService;

        public GiftCardController(IGiftCardService giftCardService)
        {
            this._giftCardService = giftCardService;
        }

        /// <summary>
        /// 绑定
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> Bind([FromBody]GiftCardBindingModels model)
        {
            var currentUser = WebUtil.GetApiUserSession();

            UT_GiftCard outModel = new UT_GiftCard();
            if (string.IsNullOrEmpty(model.CardPwd) || model.CardPwd.Length < 16)
            {
                return Ok(new StatusCodeRes(StatusCodeType.请输入16位礼包卡密码));
            }
            var result = await _giftCardService.Bind(currentUser.ID, model.CardPwd, outModel);
            switch (result)
            {
                case 0:
                    return Ok(new StatusCodeRes(StatusCodeType.失败, "绑定失败"));
                case 1:
                    return Ok(new { status = 1, msg = "绑定成功！", data = new { CardNum = outModel.CardNum } });
                case 2:
                    return Ok(new StatusCodeRes(StatusCodeType.礼包卡已被使用或失效));
                case 3:
                    return Ok(new StatusCodeRes(StatusCodeType.礼包卡已超过最晚可用时间));
                case 4:
                    return Ok(new StatusCodeRes(StatusCodeType.内部错误, "套餐不存在"));
                case 5:
                    return Ok(new StatusCodeRes(StatusCodeType.已绑定礼包卡));
                default:
                    return Ok(new StatusCodeRes(StatusCodeType.绑定异常_请重试));
            }
        }
    }
}
