using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Unitoys.Core;
using Unitoys.Core.Util;
using Unitoys.IServices;
using Unitoys.Model;
using Unitoys.WebApi.Models;

namespace Unitoys.WebApi.Controllers
{
    public class OrderByZCController : ApiController
    {
        private IOrderByZCService _orderByZCService;
        private IPaymentService _paymentService;
        private IUserService _userService;
        private ISMSConfirmationService _smsConfirmationService;

        public OrderByZCController(IOrderByZCService orderByZCService, IPaymentService paymentService, IUserService userService, ISMSConfirmationService smsConfirmationService)
        {
            this._orderByZCService = orderByZCService;
            this._paymentService = paymentService;
            this._userService = userService;
            this._smsConfirmationService = smsConfirmationService;
        }

        /// <summary>
        /// 绑定众筹订单
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> Bind([FromBody]OrderZCBindingModels model)
        {
            string errorMsg = "";

            var currentUser = WebUtil.GetApiUserSession();

            if (!ValidateHelper.IsMobile(model.tel))
            {
                errorMsg = "手机号码格式不正确！";
            }
            else if (!ValidateHelper.IsNumeric(model.smsVerCode))
            {
                errorMsg = "验证码无效！";
            }
            else
            {
                //判断手机验证码是否正确。
                UT_SMSConfirmation smsConfirmation = await _smsConfirmationService.GetEntityAsync(x => x.Tel == model.tel && x.Code == model.smsVerCode && x.Type == 3 && !x.IsConfirmed);

                //判断当前时间是否到达验证码过期时间。
                if (smsConfirmation != null)
                {
                    if (DateTime.Now > smsConfirmation.ExpireDate)
                    {
                        errorMsg = "此验证码已经过期，请重新发送验证码。";
                    }
                    else
                    {

                        switch (_orderByZCService.BindOrder(currentUser.ID, model.tel).ToString())
                        {

                            case "0":
                                errorMsg = "订单绑定失败";
                                break;
                            case "1":
                                return Ok(new { status = 1, msg = "订单绑定成功！" });
                            case "2":
                                errorMsg = "无有效订单";
                                break;
                            case "3":
                                errorMsg = "订单已被绑定";
                                break;
                            default:
                                errorMsg = "订单信息错误";
                                break;
                        }

                    }
                }
                else
                {
                    errorMsg = "验证码错误！";
                }
            }
            return Ok(new { status = 0, msg = errorMsg });
        }

        /// <summary>
        /// 根据条件查询众筹订单，分页
        /// </summary>
        /// <param name="queryModel">订单查询模型</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> GetUserOrderByZCList(GetUserOrderZCListBindingModel model)
        {
            var currentUser = WebUtil.GetApiUserSession();

            //如果pageNumber和pageSize为null，则设置默认值。
            model.PageNumber = model.PageNumber ?? 1;
            model.PageSize = model.PageSize ?? 10;

            var searchOrderByZCs = await _orderByZCService.GetUserOrderByZCList((int)model.PageNumber, (int)model.PageSize, currentUser.ID);

            var totalRows = searchOrderByZCs.Key;

            var result = from i in searchOrderByZCs.Value
                         select new
                         {
                             OrderByZCID = i.ID,
                             OrderByZCNum = i.OrderByZCNum,
                             Quantity = i.Quantity.ToString(),
                             UnitPrice = i.UnitPrice,
                             TotalPrice = i.TotalPrice,
                             OrderDate = i.OrderDate.ToString(),
                             SelectionedNumberList = i.UT_OrderByZCSelectionNumber
                             .Select(x => new
                             {
                                 ProvinceName = x.UT_ZCSelectionNumber.ProvinceName,
                                 CityName = x.UT_ZCSelectionNumber.CityName,
                                 MobileNumber = x.UT_ZCSelectionNumber.MobileNumber,
                             })
                         };

            return Ok(new { status = 1, data = new { totalRows = totalRows, list = result } });
        }

        /// <summary>
        /// 根据ID查询用户订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IHttpActionResult> GetByID(Guid id)
        {
            var currentUser = WebUtil.GetApiUserSession();
            var modelResult = await _orderByZCService.GetEntityAndOrderByZCSelectionNumberByIdAsync(id);

            if (modelResult == null)
            {
                return Ok(new { status = 0, msg = "信息异常！" });
            }
            else if (modelResult.UserId != currentUser.ID)
            {
                return Ok(new { status = 0, msg = "订单不属于此用户！" });
            }

            var data = new
            {
                OrderByZCID = modelResult.ID,
                OrderByZCNum = modelResult.OrderByZCNum,
                Quantity = modelResult.Quantity.ToString(),
                UnitPrice = modelResult.UnitPrice,
                TotalPrice = modelResult.TotalPrice,
                OrderDate = modelResult.OrderDate.ToString(),
                SelectionedNumberList = modelResult.UT_OrderByZCSelectionNumber
                .Select(x => new
                {
                    Name = x.Name,
                    IdentityNumber = x.IdentityNumber,
                    ProvinceName = x.UT_ZCSelectionNumber.ProvinceName,
                    CityName = x.UT_ZCSelectionNumber.CityName,
                    MobileNumber = x.UT_ZCSelectionNumber.MobileNumber,
                    Location = x.UT_ZCSelectionNumber.ProvinceName + x.UT_ZCSelectionNumber.CityName
                })
            };
            return Ok(new { status = 1, data = new { list = data } });
        }

    }
}
