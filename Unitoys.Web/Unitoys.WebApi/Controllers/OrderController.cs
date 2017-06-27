using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Unitoys.Core;
using Unitoys.Core.Util;
using Unitoys.ESIM_MVNO;
using Unitoys.ESIM_MVNO.Model;
using Unitoys.IServices;
using Unitoys.Model;
using Unitoys.WebApi.Models;

namespace Unitoys.WebApi.Controllers
{
    public class OrderController : ApiController
    {
        private IOrderService _orderService;
        private IPaymentService _paymentService;
        private IUserService _userService;
        private IPackageService _packageService;
        private IUserDeviceTelService _userDeviceTelService;

        public OrderController(IOrderService orderService, IPaymentService paymentService, IUserService userService, IPackageService packageService, IUserDeviceTelService userDeviceTelService)
        {
            this._orderService = orderService;
            this._paymentService = paymentService;
            this._userService = userService;
            this._packageService = packageService;
            this._userDeviceTelService = userDeviceTelService;
        }

        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> Add([FromBody]AddBindingModel model)
        {
            var currentUser = WebUtil.GetApiUserSession();
            //System.Web.Http.ModelBinding.DefaultActionValueBinder
            if (model.PackageID == Guid.Empty)
            {
                return Ok(new StatusCodeRes(StatusCodeType.参数错误, "套餐ID不能为空"));
            }
            else if (model.Quantity <= 0 || model.Quantity > 30)
            {
                //errorMsg = "包月订单只能购买一个，待后续业务需求是否需要调整！";
                return Ok(new { status = StatusCodeType.数量请在BeginQty至EndQty之间选择, msg = "数量请选中1至30之间选择", data = new { BeginQty = 1, EndQty = 30 } });
            }
            else if (!Enum.IsDefined(typeof(PaymentMethodType), model.PaymentMethod) || model.PaymentMethod == PaymentMethodType.Gift)
            {
                return Ok(new StatusCodeRes(StatusCodeType.参数错误, "无效的支付方式"));
            }
            else
            {
                var result = await _orderService.AddOrder(currentUser.ID, model.PackageID, model.Quantity, model.PaymentMethod, model.MonthPackageFee ?? 0, model.PackageAttributeId, model.BeginDateTime);

                if (result.Value.Key == 1 && result.Value.Value != null)
                {
                    var order = result.Value.Value;
                    var resultModel = new
                    {
                        OrderID = order.ID,
                        OrderNum = order.OrderNum,
                        OrderDate = order.OrderDate.ToString(),
                        PackageId = order.PackageId,
                        PackageName = order.PackageName,
                        Quantity = order.Quantity.ToString(),
                        UnitPrice = order.UnitPrice,
                        TotalPrice = order.TotalPrice,
                        ExpireDays = GetExpireDaysDescr(order),
                        Flow = order.Flow + "",
                        RemainingCallMinutes = order.RemainingCallMinutes + "",
                        PaymentMethod = (int)order.PaymentMethod + "",
                        PackageCategory = order.PackageCategory,
                        PackageIsCategoryFlow = order.PackageIsCategoryFlow,
                        PackageIsSupport4G = order.PackageIsSupport4G,
                        ExpireDaysInt = (order.ExpireDays * order.Quantity).ToString(),
                        CountryName = result.Key,
                        PackageIsApn = order.PackageIsApn,
                        PackageApnName = order.PackageApnName,
                    };
                    return Ok(new { status = 1, msg = "订单创建成功！", data = new { order = resultModel } });
                }
                else
                {
                    switch (result.Value.Key)
                    {
                        case 0:
                            return Ok(new StatusCodeRes(StatusCodeType.失败, "订单创建失败"));
                            break;
                        case 2:
                            return Ok(new StatusCodeRes(StatusCodeType.套餐不可用_请选择其他套餐, "套餐不可用，请选择其他套餐"));
                            break;
                        case 3:
                            return Ok(new StatusCodeRes(StatusCodeType.该套餐不允许购买多个, "该套餐不允许购买多个"));
                            break;
                        case 6:
                            return Ok(new StatusCodeRes(StatusCodeType.无验证通过手机号, "无验证通过手机号"));
                            break;
                        case 8:
                            return Ok(new StatusCodeRes(StatusCodeType.参数错误, "组合ID和套餐不匹配"));
                            break;
                        default:
                            return Ok(new StatusCodeRes(StatusCodeType.失败, "订单创建失败"));
                            break;
                    }

                }
            }
            return Ok(new StatusCodeRes(StatusCodeType.失败, "订单创建失败"));
        }

        /// <summary>
        /// 创建免费领取订单
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> AddReceive([FromBody]AddReceiveBindingModel model)
        {
            var currentUser = WebUtil.GetApiUserSession();
            //System.Web.Http.ModelBinding.DefaultActionValueBinder
            if (model.PackageID == Guid.Empty)
            {
                return Ok(new StatusCodeRes(StatusCodeType.参数错误, "套餐ID不能为空"));
            }
            else
            {
                var result = await _orderService.AddReceiveOrder(currentUser.ID, model.PackageID);

                if (result.Value.Key == 1 && result.Value.Value != null)
                {
                    var order = result.Value.Value;
                    var resultModel = new
                    {
                        OrderID = order.ID,
                        OrderNum = order.OrderNum,
                        OrderDate = order.OrderDate.ToString(),
                        PackageId = order.PackageId,
                        PackageName = order.PackageName,
                        Quantity = order.Quantity.ToString(),
                        UnitPrice = order.UnitPrice,
                        TotalPrice = order.TotalPrice,
                        ExpireDays = GetExpireDaysDescr(order),
                        Flow = order.Flow + "",
                        RemainingCallMinutes = order.RemainingCallMinutes + "",
                        PaymentMethod = (int)order.PaymentMethod + "",
                        PackageCategory = order.PackageCategory,
                        PackageIsCategoryFlow = order.PackageIsCategoryFlow,
                        PackageIsSupport4G = order.PackageIsSupport4G,
                        ExpireDaysInt = (order.ExpireDays * order.Quantity).ToString(),
                        CountryName = result.Key,
                        PackageIsApn = order.PackageIsApn,
                        PackageApnName = order.PackageApnName,
                    };
                    return Ok(new { status = 1, msg = "订单创建成功！", data = new { order = resultModel } });
                }
                else
                {
                    switch (result.Value.Key)
                    {
                        case 1:
                            return Ok(new StatusCodeRes(StatusCodeType.失败, "订单创建失败"));
                            break;
                        case 2:
                            return Ok(new StatusCodeRes(StatusCodeType.套餐不可用_请选择其他套餐, "套餐不可用，请选择其他套餐"));
                            break;
                        case 3:
                            return Ok(new StatusCodeRes(StatusCodeType.该套餐不允许购买多个, "该套餐不允许购买多个"));
                            break;
                        case 5:
                            return Ok(new StatusCodeRes(StatusCodeType.该套餐不允许购买多个, "已领取此套餐"));
                            break;
                        case 4:
                            return Ok(new StatusCodeRes(StatusCodeType.失败, "订单创建失败"));
                            break;
                        default:
                            return Ok(new StatusCodeRes(StatusCodeType.失败, "订单创建失败"));
                            break;
                    }
                }
            }
            return Ok(new StatusCodeRes(StatusCodeType.失败, "订单创建失败"));
        }

        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="orderid"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> Cancel([FromBody]PayOrderByUserAmountBindingModel model)
        {
            var currentUser = WebUtil.GetApiUserSession();

            if (model.OrderID == Guid.Empty)
            {
                return Ok(new StatusCodeRes(StatusCodeType.参数错误, "订单ID格式错误"));
            }
            else
            {
                int resultNum = await _orderService.CancelOrder(currentUser.ID, model.OrderID);

                if (resultNum == 0)
                {
                    return Ok(new { status = 1, msg = "取消成功！" });
                }
                else if (resultNum == -2)
                {
                    return Ok(new StatusCodeRes(StatusCodeType.订单已经被取消));
                }
                else if (resultNum == -3)
                {
                    return Ok(new StatusCodeRes(StatusCodeType.参数错误, "此订单不属于该用户"));
                }
                else if (resultNum == -4)
                {
                    return Ok(new StatusCodeRes(StatusCodeType.订单已被使用));
                }
                else if (resultNum == -5)
                {
                    return Ok(new StatusCodeRes(StatusCodeType.订单不允许取消));
                }
                //0.取消订单
                return Ok(new StatusCodeRes(StatusCodeType.失败, "取消失败"));
            }
        }

        /// <summary>
        /// 根据条件查询订单，分页
        /// </summary>
        /// <param name="queryModel">订单查询模型</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> GetUserOrderList([FromUri]GetUserOrderListBindingModel model)
        {
            var currentUser = WebUtil.GetApiUserSession();

            //赋值当前用户ID给queryModel
            model.UserID = currentUser.ID;

            //查询Expression
            //Expression<Func<UT_Order, bool>> exp = GetOrderSearchExpression<UT_Order>(model);

            //如果pageNumber和pageSize为null，则设置默认值。
            model.PageNumber = model.PageNumber ?? 1;
            model.PageSize = model.PageSize ?? 10;

            //如果查询条件不为空，则根据查询条件查询，反则查询所有订单。
            var searchOrders = await _orderService.GetUserOrderList((int)model.PageNumber, (int)model.PageSize, currentUser.ID, PayStatusType.YesPayment, model.OrderStatus, model.PackageCategory, model.PackageIsCategoryFlow, model.PackageIsCategoryCall, model.PackageIsCategoryDualSimStandby, model.PackageIsCategoryKingCard);

            var totalRows = searchOrders.Key;

            var result = from i in searchOrders.Value
                         select new
                         {
                             OrderID = i.ID,
                             OrderNum = i.OrderNum,
                             UserId = i.UserId,
                             PackageId = i.PackageId,
                             PackageName = i.PackageName,
                             PackageCategory = (int)i.PackageCategory + "",
                             PackageIsCategoryFlow = i.PackageIsCategoryFlow,
                             PackageIsCategoryCall = i.PackageIsCategoryCall,
                             PackageIsCategoryDualSimStandby = i.PackageIsCategoryDualSimStandby,
                             PackageIsCategoryKingCard = i.PackageIsCategoryKingCard,
                             Flow = i.Flow + "",
                             Quantity = i.Quantity.ToString(),
                             UnitPrice = i.UnitPrice,
                             TotalPrice = i.TotalPrice,
                             ExpireDays = GetExpireDaysDescr(i),
                             ExpireDaysInt = (i.ExpireDays * i.Quantity).ToString(),
                             OrderDate = i.OrderDate.ToString(),
                             PayDate = i.PayDate.HasValue ? i.PayDate.Value.ToString() : "",
                             PayStatus = (int)i.PayStatus + "",
                             OrderStatus = (int)i.OrderStatus + "",
                             RemainingCallMinutes = i.RemainingCallMinutes + "",
                             //EffectiveDate = i.EffectiveDate.HasValue ? i.EffectiveDate.Value.ToString() : "",
                             ActivationDate = i.ActivationDate.HasValue ? i.ActivationDate.Value.ToString() : "",
                             //LogoPic = i.UT_Package.LogoPic.GetPackageCompleteUrl(),
                             LogoPic = i.UT_Package.UT_Country != null ? i.UT_Package.UT_Country.LogoPic.GetPackageCompleteUrl() : i.UT_Package.LogoPic.GetPackageCompleteUrl(),
                             LastCanActivationDate = GetLastCanActivationDate(i).ToString(),
                             PackageIsApn = i.PackageIsApn,
                             PackageApnName = i.PackageApnName,
                             CountryName = i.UT_Package.UT_Country != null ? i.UT_Package.UT_Country.CountryName : ""
                         };

            return Ok(new { status = 1, data = new { totalRows = totalRows, list = result } });
        }

        /// <summary>
        /// 获取订单有效天数的描述
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private string GetExpireDaysDescr(UT_Order i)
        {
            if (i.OrderStatus == OrderStatusType.Unactivated)
            {
                return "有效天数：" + (i.ExpireDays * i.Quantity).ToString() + (i.PackageCategory == CategoryType.Relaxed ? "月" : "天");
            }
            else if (i.OrderStatus == OrderStatusType.Cancel)
            {
                return "订单已取消";
            }
            else if (i.OrderStatus == OrderStatusType.Used || i.OrderStatus == OrderStatusType.UnactivatError)
            {
                if (!i.EffectiveDateDesc.HasValue)
                {
                    if (i.PackageCategory == CategoryType.Relaxed)
                    {
                        return string.Format("有效期：{0} 到 {1}", CommonHelper.GetTime(i.EffectiveDate.Value.ToString()).ToString("yyyy-MM-dd"), CommonHelper.GetTime(i.EffectiveDate.Value.ToString()).AddMonths(i.ExpireDays * i.Quantity).ToString("yyyy-MM-dd"));
                    }
                    return string.Format("有效期：{0} 到 {1}", CommonHelper.GetTime(i.EffectiveDate.Value.ToString()).ToString("yyyy-MM-dd"), CommonHelper.GetTime(i.EffectiveDate.Value.ToString()).AddDays(i.ExpireDays * i.Quantity).ToString("yyyy-MM-dd"));
                }
                else
                {
                    if (i.PackageCategory == CategoryType.Relaxed)
                    {
                        return string.Format("有效期：{0} 到 {1}", i.EffectiveDateDesc.Value.ToString("yyyy-MM-dd"), i.EffectiveDateDesc.Value.AddMonths(i.ExpireDays * i.Quantity).ToString("yyyy-MM-dd"));
                    }
                    return string.Format("有效期：{0} 到 {1}", i.EffectiveDateDesc.Value.ToString("yyyy-MM-dd"), i.EffectiveDateDesc.Value.AddDays(i.ExpireDays * i.Quantity).ToString("yyyy-MM-dd"));
                }
            }
            else if (i.OrderStatus == OrderStatusType.HasExpired)
            {
                return "订单已过期";
            }

            if (!i.EffectiveDateDesc.HasValue)
            {
                return i.OrderStatus == OrderStatusType.UnactivatError
                                             ? (i.ExpireDays * i.Quantity).ToString()
                                             : CommonHelper.GetTime(i.EffectiveDate.Value.ToString()) + " " + (i.ExpireDays * i.Quantity).ToString();
            }
            else
            {
                return i.OrderStatus == OrderStatusType.UnactivatError
                                           ? (i.ExpireDays * i.Quantity).ToString()
                                           : i.EffectiveDateDesc + " " + (i.ExpireDays * i.Quantity).ToString();
            }
        }

        /// <summary>
        /// 根据ID查询用户订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IHttpActionResult> GetByID(Guid id)
        {
            var currentUser = WebUtil.GetApiUserSession();
            var orderResult = await _orderService.GetEntityAndPackageByIdAsync(id);

            if (orderResult == null)
            {
                //return Ok(new { status = 0, msg = "信息异常！" });
                return Ok(new StatusCodeRes(StatusCodeType.参数错误, "信息异常"));
            }
            else if (orderResult.UserId != currentUser.ID)
            {
                return Ok(new StatusCodeRes(StatusCodeType.参数错误, "订单不属于此用户"));
            }

            var data = new
            {
                OrderID = orderResult.ID,
                OrderNum = orderResult.OrderNum,
                UserId = orderResult.UserId,
                PackageId = orderResult.PackageId,
                PackageName = orderResult.PackageName,
                PackageCategory = (int)orderResult.PackageCategory + "",
                PackageIsCategoryFlow = orderResult.PackageIsCategoryFlow,
                PackageIsCategoryCall = orderResult.PackageIsCategoryCall,
                PackageIsCategoryDualSimStandby = orderResult.PackageIsCategoryDualSimStandby,
                PackageIsCategoryKingCard = orderResult.PackageIsCategoryKingCard,
                PackageFeatures = orderResult.PackageFeatures,
                PackageDetails = orderResult.PackageDetails,
                PackageIsSupport4G = orderResult.PackageIsSupport4G,
                PackageIsApn = orderResult.PackageIsApn,
                PackageApnName = orderResult.PackageApnName ?? "",
                Flow = orderResult.Flow,
                Quantity = orderResult.Quantity.ToString(),
                UnitPrice = orderResult.UnitPrice,
                TotalPrice = orderResult.TotalPrice,
                ExpireDays = GetExpireDaysDescr(orderResult).Replace("有效天数：", "").Replace("有效期：", ""),
                ExpireDaysInt = (orderResult.ExpireDays * orderResult.Quantity).ToString(),
                OrderDate = orderResult.OrderDate.ToString(),
                PayDate = orderResult.PayDate.HasValue ? orderResult.PayDate.Value.ToString() : "",
                PayStatus = (int)orderResult.PayStatus + "",
                OrderStatus = (int)orderResult.OrderStatus + "",
                RemainingCallMinutes = orderResult.RemainingCallMinutes.ToString(),
                //EffectiveDate = orderResult.EffectiveDate.HasValue ? orderResult.EffectiveDate.Value.ToString().ToString() : "",
                ActivationDate = orderResult.ActivationDate.HasValue ? orderResult.ActivationDate.Value.ToString() : "",
                LogoPic = orderResult.UT_Package.LogoPic.GetPackageCompleteUrl(), //orderResult.UT_Package.UT_Country != null ? orderResult.UT_Package.UT_Country.LogoPic.GetPackageCompleteUrl() : orderResult.UT_Package.Pic.GetPackageCompleteUrl(),
                PaymentMethod = (int)orderResult.PaymentMethod + "",
                LastCanActivationDate = GetLastCanActivationDate(orderResult).ToString(),
                CountryName = orderResult.UT_Package.UT_Country != null ? orderResult.UT_Package.UT_Country.CountryName : ""
            };
            return Ok(new { status = 1, data = new { list = data } });
        }

        /// <summary>
        /// 套餐激活
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> Activation([FromBody]ActivationBindingModel model)
        {
            if ((!model.BeginTime.HasValue || model.BeginTime == 0) && !model.BeginDateTime.HasValue)
            {
                return Ok(new StatusCodeRes(StatusCodeType.参数错误, "激活失败，时间格式错误"));
            }
            if (model.OrderID == Guid.Empty)
            {
                return Ok(new StatusCodeRes(StatusCodeType.参数错误, "订单ID格式错误"));
            }

            var currentUser = WebUtil.GetApiUserSession();

            var result = await _orderService.Activation(currentUser.ID, model.OrderID, model.BeginTime, model.BeginDateTime);
            switch (result)
            {
                case -12:
                    return Ok(new StatusCodeRes(StatusCodeType.激活失败_超过最晚激活日期));
                case -13:
                    return Ok(new StatusCodeRes(StatusCodeType.激活失败_激活类型异常));
                case -14:
                    return Ok(new StatusCodeRes(StatusCodeType.激活套餐失败_可能套餐已过期, "暂时无法激活,请联系客服"));
                case -15:
                    return Ok(new StatusCodeRes(StatusCodeType.激活套餐失败_可能套餐已过期, "激活出现问题,暂时无法激活,请联系客服"));
                case 10:
                    return Ok(new { status = 1, msg = "订单待激活", data = new { OrderID = model.OrderID } });// Data = order.PackageOrderData 
                case -11:
                    return Ok(new StatusCodeRes(StatusCodeType.失败, "激活失败，可能订单不存在或未支付"));
                case -16:
                    return Ok(new StatusCodeRes(StatusCodeType.内部错误, "更新订单失败"));
                default:
                    return Ok(new StatusCodeRes(StatusCodeType.内部错误, "套餐激活出现问题"));
            }

        }

        /// <summary>
        /// 获取最晚激活时间
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        private static int GetLastCanActivationDate(UT_Order order)
        {
            if (order == null)
            {
                return 0;
            }
            if (order.PackageCategory == CategoryType.KingCard)
            {
                return CommonHelper.ConvertDateTimeInt(CommonHelper.GetTime(order.OrderDate.ToString()).AddMonths(3));
            }
            else
            {
                return CommonHelper.ConvertDateTimeInt(CommonHelper.GetTime(order.OrderDate.ToString()).AddMonths(6));
            }
        }

        /// <summary>
        /// 激活大王卡套餐
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> ActivationKindCard([FromBody]ActivationKindCardBindingModel model)
        {
            if (!ValidateHelper.IsMobile(model.Tel))
            {
                return Ok(new StatusCodeRes(StatusCodeType.手机号码格式不正确));
            }
            else if (model.Tel.Substring(0, 3) != "176" && model.Tel.Substring(0, 3) != "185")
            {
                return Ok(new StatusCodeRes(StatusCodeType.失败, "当前号段不支持，如确认号段无误，请联系客服人员"));
            }
            if (model.OrderID != Guid.Empty)
            {
                var currentUser = WebUtil.GetApiUserSession();

                UT_Order order = await _orderService.GetEntityByIdAsync(model.OrderID);
                if (order != null && currentUser.ID == order.UserId && order.PayDate != null && order.PayStatus == PayStatusType.YesPayment)
                {
                    var LastCanActivationDate = GetLastCanActivationDate(order);

                    if (CommonHelper.GetDateTimeInt() > LastCanActivationDate)
                    {
                        return Ok(new StatusCodeRes(StatusCodeType.激活失败_超过最晚激活日期));
                    }
                    if (order.PackageCategory != CategoryType.KingCard)
                    {
                        return Ok(new StatusCodeRes(StatusCodeType.激活失败_激活类型异常));
                    }
                    if (order.OrderStatus == 0)
                    {
                        //1.购买产品
                        UT_Package package = await _packageService.GetEntityByIdAsync(order.PackageId);
                        UT_Users user = await _userService.GetEntityByIdAsync(order.UserId);

                        try
                        {
                            var result = true;
                            //3.保存订单卡数据
                            //order.PackageOrderId = result.orderid;
                            //order.PackageOrderData = result.data;
                            order.EffectiveDate = CommonHelper.GetDateTimeInt();
                            order.OrderStatus = OrderStatusType.Used;//充值后为已使用
                            order.ActivationDate = CommonHelper.GetDateTimeInt();
                            SetExpireDate(order);
                            order.Remark = string.IsNullOrEmpty(order.Remark) ? "充值号码：" + model.Tel : order.Remark + " " + "充值号码：" + model.Tel;
                            if (!await _orderService.UpdateAsync(order))
                            {
                                LoggerHelper.Error("大王卡激活失败,更新数据库失败");
                                return Ok(new StatusCodeRes(StatusCodeType.内部错误, "更新订单失败"));
                            }
                            else
                            {
                                //TODO 为号码充值话费
                                //2.购买订单卡
                                //var result = await ESIMUtil.BuyProduct(package.PackageNum, user.Tel, order.OrderNum, model.BeginTime, order.Quantity * package.ExpireDays);
                            }
                        }
                        catch (Exception ex)
                        {
                            LoggerHelper.Error(ex.Message, ex);
                            throw;
                        }
                    }
                    //else if (order.OrderStatus != 0)
                    //{
                    //    return Ok(new { status = 1, msg = "激活处理中，请勿重复激活！", data = new { OrderID = order.ID } });// Data = order.PackageOrderData 
                    //}

                    //4.返回订单卡数据
                    return Ok(new { status = 1, msg = "激活成功！", data = new { OrderID = order.ID } });// Data = order.PackageOrderData 
                }

                return Ok(new StatusCodeRes(StatusCodeType.失败, "激活失败，可能订单不存在或未支付"));
            }
            return Ok(new StatusCodeRes(StatusCodeType.参数错误, "订单ID格式错误"));
        }

        private Byte[] hexToBytes(String s)
        {
            Byte[] bytes;
            bytes = new Byte[s.Length / 2];

            for (int i = 0; i < bytes.Length; i++)
            {
                String temp_sub = s.Substring(2 * i, 2);
                bytes[i] = (Byte)Convert.ToInt32(temp_sub, 16);
            }
            return bytes;
        }

        /// <summary>
        /// 查询订单卡数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> QueryOrderData([FromBody]ActivationBindingModel model)
        {
            if (model.OrderID != Guid.Empty)
            {
                var currentUser = WebUtil.GetApiUserSession();

                UT_Order order = await _orderService.GetEntityByIdAsync(model.OrderID);
                if (order != null && currentUser.ID == order.UserId && order.PayDate != null && order.PayStatus == PayStatusType.YesPayment)
                {
                    if (order.OrderStatus == OrderStatusType.UnactivatError || order.OrderStatus == OrderStatusType.Used)
                    {
                        //Byte[] resultData = hexToBytes(order.PackageOrderData);
                        //string dataB64 = Convert.ToBase64String(resultData);
                        //3.返回订单卡数据
                        //return Ok(new { status = 1, data = new { OrderID = order.ID, Data = 0000000000000(Convert.ToBase64String(hexToBytes(order.PackageOrderData)), Encoding.UTF8).ToUpper() } });
                        var result = await new Unitoys.ESIM_MVNO.MVNOServiceApi().QueryOrder(order.PackageOrderId);

                        if (result.status != "1")
                        {
                            LoggerHelper.Error("订单ID:" + order.ID + ",查询订单套餐信息失败：" + result.msg);
                            return Ok(new StatusCodeRes(StatusCodeType.失败, "查询订单套餐信息失败！"));
                        }

                        //3.生成卡数据
                        string writeData = "";
                        if (string.IsNullOrEmpty(model.EmptyCardSerialNumber))
                        {
                            SimDataSTK simdata = new SimDataSTK(model.EmptyCardSerialNumber, new WriteCardSTK()
                            {
                                iccid = result.data.esimResource.iccid,
                                imsi = result.data.esimResource.imsi,
                                ki = result.data.esimResource.ki,
                                opc = result.data.esimResource.opc,
                                PLMN = result.data.esimResource.plmn,
                            });
                            writeData = simdata.GetData();
                        }
                        else
                        {
                            SimData simdata = new SimData(model.EmptyCardSerialNumber, new WriteCard()
                            {
                                iccid = result.data.esimResource.iccid,
                                imsi = result.data.esimResource.imsi,
                                ki = result.data.esimResource.ki,
                                opc = result.data.esimResource.opc,
                                PLMN = result.data.esimResource.plmn,
                            });
                            writeData = simdata.GetData();
                        }

                        return Ok(new { status = 1, data = new { OrderID = order.ID, Data = writeData } });
                    }
                }

                return Ok(new StatusCodeRes(StatusCodeType.失败, "无有效订单卡数据！"));
            }
            return Ok(new StatusCodeRes(StatusCodeType.参数错误, "订单ID格式错误！"));
        }

        private static string StringToHexString(string s, Encoding encode)
        {
            byte[] b = encode.GetBytes(s);//按照指定编码将string编程字节数组
            string result = string.Empty;
            for (int i = 0; i < b.Length; i++)//逐字节变为16进制字符
            {
                result += Convert.ToString(b[i], 16);
            }
            return result;
        }

        /// <summary>
        /// 订单套餐激活本地完成
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> ActivationLocalCompleted([FromBody]ActivationBindingModel model)
        {
            if (model.OrderID != Guid.Empty)
            {
                var currentUser = WebUtil.GetApiUserSession();

                UT_Order order = await _orderService.GetEntityByIdAsync(model.OrderID);
                if (order != null
                    && currentUser.ID == order.UserId
                    && order.PayDate != null
                    && order.PayStatus == PayStatusType.YesPayment
                    && (order.OrderStatus == OrderStatusType.UnactivatError || order.OrderStatus == OrderStatusType.Used))
                {
                    //3.保存订单卡数据
                    order.OrderStatus = OrderStatusType.Used;
                    if (!await _orderService.UpdateAsync(order))
                    {
                        return Ok(new StatusCodeRes(StatusCodeType.内部错误, "更新订单失败"));
                    }

                    //4.返回订单卡数据
                    return Ok(new { status = 1, data = new { OrderID = order.ID } });// Data = order.PackageOrderData 
                }

                return Ok(new StatusCodeRes(StatusCodeType.失败, "激活失败，可能订单不存在或未支付"));
            }
            return Ok(new StatusCodeRes(StatusCodeType.参数错误, "订单ID格式错误"));
        }

        /// <summary>
        /// 通过用户余额支付套餐订单
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> PayOrderByUserAmount([FromBody]PayOrderByUserAmountBindingModel model)
        {
            string errorMsg = "";

            var currentUser = WebUtil.GetApiUserSession();

            int resultNum = await _orderService.PayOrderByUserAmount(currentUser.ID, model.OrderID);

            if (resultNum == 0 || resultNum == 10 || resultNum <= -11)
            {
                switch (resultNum)
                {
                    case 10://激活成功
                    case 0:
                        return Ok(new { status = 1, msg = "支付成功！" });
                    case -12:
                        return Ok(new StatusCodeRes(StatusCodeType.激活失败_超过最晚激活日期, "支付成功,激活失败,超过最晚激活日期"));
                    case -13:
                        return Ok(new StatusCodeRes(StatusCodeType.激活失败_激活类型异常, "支付成功,激活失败,激活类型异常"));
                    case -14:
                        return Ok(new StatusCodeRes(StatusCodeType.激活套餐失败_可能套餐已过期, "支付成功,暂时无法激活,请联系客服"));
                    case -15:
                        return Ok(new StatusCodeRes(StatusCodeType.激活套餐失败_可能套餐已过期, "支付成功,激活出现问题,暂时无法激活,请联系客服"));
                    //case 10:
                    //    return Ok(new { status = 1, msg = "订单待激活", data = new { OrderID = model.OrderID } });// Data = order.PackageOrderData 
                    case -11:
                        return Ok(new StatusCodeRes(StatusCodeType.失败, "支付成功,激活失败，可能订单不存在或未支付"));
                    case -16:
                        return Ok(new StatusCodeRes(StatusCodeType.内部错误, "支付成功,套餐激活后,更新订单失败"));
                    default:
                        return Ok(new StatusCodeRes(StatusCodeType.内部错误, "支付成功,套餐激活出现问题"));
                }
            }
            else if (resultNum == -2)
            {
                return Ok(new StatusCodeRes(StatusCodeType.此订单已经支付成功_不能再支付));
            }
            else if (resultNum == -3)
            {
                //errorMsg = "此订单不属于该用户！";
                return Ok(new StatusCodeRes(StatusCodeType.参数错误, "此订单不属于该用户"));
            }
            else if (resultNum == -4)
            {
                errorMsg = "用户余额不足！";
                return Ok(new StatusCodeRes(StatusCodeType.用户余额不足));
            }
            else if (resultNum == -5)
            {
                errorMsg = "支付方式异常！";
                return Ok(new StatusCodeRes(StatusCodeType.支付方式异常));
            }
            else
            {
                return Ok(new StatusCodeRes(StatusCodeType.失败, "支付失败"));
            }

            return Ok(new { status = 0, msg = errorMsg });
        }

        /// <summary>
        /// 根据类型检查是否存在使用中的类型套餐
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> CheckUsedExistByPageCategory([FromBody]IsStatusUsedByCategoryBindingModel model)
        {
            string errorMsg = "";

            var currentUser = WebUtil.GetApiUserSession();

            if (!model.PackageCategory.HasValue)
            {
                return Ok(new StatusCodeRes(StatusCodeType.必填参数为空, "套餐类型不能为空"));
            }
            if (model == null || (!model.PackageCategory.HasValue && !model.PackageIsCategoryFlow.HasValue && !model.PackageIsCategoryCall.HasValue && !model.PackageIsCategoryDualSimStandby.HasValue && !model.PackageIsCategoryKingCard.HasValue))
            {
                return Ok(new StatusCodeRes(StatusCodeType.必填参数为空, "套餐类型不能为空"));
            }
            else
            {
                if (model.PackageCategory.HasValue && model.PackageCategory == CategoryType.Call)
                {
                    return Ok(new { status = 1, data = new { Used = "1" } });
                    //model.PackageCategory = CategoryType.DualSimStandby;
                    //model.PackageIsCategoryDualSimStandby = true;
                    //model.PackageCategory = null;
                }
                if (model.PackageCategory.HasValue && model.PackageCategory == CategoryType.DualSimStandby)
                {
                    return Ok(new { status = 1, data = new { Used = "1" } });
                    //model.PackageIsCategoryDualSimStandby = true;
                    //model.PackageCategory = null;
                }
                if (model.PackageIsCategoryCall.HasValue && model.PackageIsCategoryCall == true)
                {
                    model.PackageIsCategoryDualSimStandby = true;
                }
                bool result = await _orderService.IsStatusUsed(currentUser.ID, model.PackageCategory, model.PackageIsCategoryFlow, model.PackageIsCategoryCall, model.PackageIsCategoryDualSimStandby, model.PackageIsCategoryKingCard);
                return Ok(new { status = 1, data = new { Used = result ? "1" : "0" } });
            }
        }

        /// <summary>
        /// app调用支付成功返回的接口
        /// </summary>
        /// <param name="token"></param>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        [NoAuthenticate]
        public async Task<IHttpActionResult> PayNotifyAsync([FromBody]PayNotifyAsyncBindingModel model)
        {

            var token = System.Web.HttpContext.Current.Request.Headers["token"];
            //todo 由于测试api中header发送有问题，暂时使用此种方式进行测试
            if (System.Web.HttpContext.Current.Request.QueryString["expires"] == "1471316792")
            {
                token = System.Web.HttpContext.Current.Request.QueryString["token"];
            }

            string reckonKey = SecureHelper.MD5(String.Format("{0}{1}{2}{3}{4}", token, model.Amount, model.OrderNum, model.PayDate, "f378qh87"));

            //判断请求是否合法
            if (reckonKey.Equals(model.Key))
            {

                var orderOrPayment = model.OrderNum;

                if (orderOrPayment.StartsWith("8022", StringComparison.OrdinalIgnoreCase))
                {
                    //处理订单完成。
                    //if (await _orderService.OnAfterOrderSuccess(orderOrPayment))
                    //{
                    //    return Ok(new { status = 1, msg = "支付成功！" });
                    //}
                }
                else if (orderOrPayment.StartsWith("9022", StringComparison.OrdinalIgnoreCase))
                {
                    //处理付款完成。
                    //if (await _paymentService.OnAfterPaymentSuccess(orderOrPayment))
                    //{
                    //    return Ok(new { status = 1, msg = "充值成功！" });
                    //}
                }
                return Ok(new { status = 0, msg = "订单处理失败！" });
            }
            else
            {
                return Ok(new { status = 0, msg = "非法请求！" });
            }


        }

        /// <summary>
        /// 根据条件查询订单使用余量
        /// </summary>
        /// <param name="queryModel">订单查询模型</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> GetUserOrderUsageRemaining()
        {
            var currentUser = WebUtil.GetApiUserSession();

            //如果查询条件不为空，则根据查询条件查询，反则查询所有订单。
            var searchOrders = await _orderService.GetEntitiesAsync(x =>
                x.UserId == currentUser.ID
                && x.OrderStatus == OrderStatusType.Used
                && x.PayStatus == PayStatusType.YesPayment
                && (x.PackageCategory == CategoryType.Call || x.PackageCategory == CategoryType.Flow));
            int TotalNumFlow = searchOrders.Count(x => x.PackageCategory == CategoryType.Flow);

            var searchOrder = (await _orderService.GetEntitiesAsync(x =>
                x.UserId == currentUser.ID
                && x.OrderStatus == OrderStatusType.Used
                && x.PayStatus == PayStatusType.YesPayment
                && (x.PackageCategory == CategoryType.FreeReceive || x.PackageCategory == CategoryType.Relaxed))).OrderByDescending(x => x.PackageCategory).FirstOrDefault();

            var unCount = await _orderService.GetEntitiesCountAsync(x =>
                x.UserId == currentUser.ID
                && (x.OrderStatus == OrderStatusType.Unactivated || x.OrderStatus == OrderStatusType.UnactivatError)
                && x.PackageCategory == CategoryType.Flow
                && x.PayStatus == PayStatusType.YesPayment);

            //如果只有一个已激活流量套餐，那么显示名称
            if (TotalNumFlow == 1)
            {
                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        Used = new
                        {
                            ServiceName = searchOrder == null ? "" : searchOrder.PackageName,
                            ServiceOrderId = searchOrder == null ? "" : searchOrder.ID.ToString(),
                            ServicePackageCategory = searchOrder == null ? "" : ((int)searchOrder.PackageCategory).ToString(),

                            TotalNum = searchOrders.Count().ToString(),//总数量
                            TotalNumFlow = TotalNumFlow.ToString(),//流量套餐总数量
                            FlowPackageName = searchOrders.FirstOrDefault(x => x.PackageCategory == CategoryType.Flow).PackageName,//
                            TotalRemainingCallMinutes = searchOrders.Sum(x => x.RemainingCallMinutes).ToString(),//总剩余通话分钟数
                        },
                        Unactivated = new
                        {
                            TotalNumFlow = unCount.ToString(),//总未激活流量套餐书
                        }
                    }
                });
            }
            else
            {
                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        Used = new
                        {
                            ServiceName = searchOrder == null ? "" : searchOrder.PackageName,
                            OrderId = searchOrder == null ? "" : searchOrder.ID.ToString(),
                            PackageCategory = searchOrder == null ? "" : ((int)searchOrder.PackageCategory).ToString(),

                            TotalNum = searchOrders.Count().ToString(),
                            TotalNumFlow = TotalNumFlow.ToString(),
                            TotalRemainingCallMinutes = searchOrders.Sum(x => x.RemainingCallMinutes).ToString(),
                        },
                        Unactivated = new
                        {
                            TotalNumFlow = unCount.ToString(),
                        }
                    }
                });
            }
        }

        #region 获取查询的Expression表达式
        /// <summary>
        /// 获取查询表达式
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        private Expression<Func<TEntity, bool>> GetOrderSearchExpression<TEntity>(GetUserOrderListBindingModel model)
        {
            Expression expression = null;
            ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "entity");

            try
            {
                Expression searchExpression = null;
                Expression inExpression;
                MemberExpression left = null;
                ConstantExpression right;

                if (model.UserID != Guid.Empty)
                {
                    left = Expression.Property(parameter, "UserId");
                    right = Expression.Constant(model.UserID, typeof(Guid));
                    inExpression = Expression.Equal(left, right);
                    searchExpression = searchExpression == null ? inExpression : Expression.And(inExpression, searchExpression);
                }
                if (model.PackageName != null)
                {
                    left = Expression.Property(parameter, "PackageName");
                    right = Expression.Constant(model.PackageName, typeof(string));
                    inExpression = Expression.Call(left, typeof(string).GetMethod("Contains"), right);
                    searchExpression = searchExpression == null ? inExpression : Expression.And(inExpression, searchExpression);
                }
                if (model.OrderStartDate != DateTime.MinValue)
                {
                    left = Expression.Property(parameter, "OrderDate");
                    right = Expression.Constant(model.OrderStartDate, typeof(DateTime));
                    inExpression = Expression.GreaterThanOrEqual(left, right);
                    searchExpression = searchExpression == null ? inExpression : Expression.And(inExpression, searchExpression);
                }
                if (model.OrderEndDate != DateTime.MinValue)
                {
                    left = Expression.Property(parameter, "OrderDate");
                    right = Expression.Constant(model.OrderEndDate, typeof(DateTime));
                    inExpression = Expression.LessThanOrEqual(left, right);
                    searchExpression = searchExpression == null ? inExpression : Expression.And(inExpression, searchExpression);
                }
                if (model.PayStartDate != DateTime.MinValue)
                {
                    left = Expression.Property(parameter, "PayDate");
                    right = Expression.Constant(model.PayStartDate, typeof(DateTime?));
                    inExpression = Expression.GreaterThanOrEqual(left, right);
                    searchExpression = searchExpression == null ? inExpression : Expression.And(inExpression, searchExpression);
                }
                if (model.PayEndDate != DateTime.MinValue)
                {
                    left = Expression.Property(parameter, "PayDate");
                    right = Expression.Constant(model.PayEndDate, typeof(DateTime?));
                    inExpression = Expression.LessThanOrEqual(left, right);
                    searchExpression = searchExpression == null ? inExpression : Expression.And(inExpression, searchExpression);
                }
                if (model.PayStatus != null)
                {
                    left = Expression.Property(parameter, "PayStatus");
                    right = Expression.Constant(model.PayStatus, typeof(int));
                    inExpression = Expression.Equal(left, right);
                    searchExpression = searchExpression == null ? inExpression : Expression.And(inExpression, searchExpression);
                }
                if (model.OrderStatus != null)
                {
                    left = Expression.Property(parameter, "OrderStatus");
                    right = Expression.Constant(model.OrderStatus, typeof(int));
                    inExpression = Expression.Equal(left, right);
                    searchExpression = searchExpression == null ? inExpression : Expression.And(inExpression, searchExpression);
                }

                expression = expression == null ? searchExpression : Expression.And(expression, searchExpression);
            }
            catch (Exception)
            {
                return null;
            }

            return expression == null ? x => true : Expression.Lambda<Func<TEntity, bool>>(expression, parameter);
        }
        #endregion
        /// <summary>
        /// 设置到期日期
        /// </summary>
        /// <param name="order"></param>
        private static void SetExpireDate(UT_Order order)
        {
            if (order.PackageCategory != CategoryType.Relaxed)
            {
                order.ExpireDate = order.EffectiveDate + (order.ExpireDays * 86400 * order.Quantity);
            }
            else
            {
                order.ExpireDate = CommonHelper.ConvertDateTimeInt(CommonHelper.GetTime(order.EffectiveDate.Value.ToString()).AddMonths(order.ExpireDays * order.Quantity));
            }
        }
    }
}
