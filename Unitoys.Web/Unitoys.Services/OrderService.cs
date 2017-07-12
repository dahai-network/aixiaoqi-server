using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Core;
using Unitoys.IServices;
using Unitoys.Model;
using System.Data.Entity;
using System.Linq.Expressions;
using Unitoys.ESIM_MVNO;

namespace Unitoys.Services
{
    public class OrderService : BaseService<UT_Order>, IOrderService
    {
        private IUserDeviceTelService _userDeviceTelService;
        public OrderService(IUserDeviceTelService userDeviceTelService)
        {
            this._userDeviceTelService = userDeviceTelService;
        }
        /// <summary>
        /// 添加订单
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="orderName">订单名</param>
        /// <param name="packageId">套餐ID</param>
        /// <param name="quantity">数量</param>
        /// <param name="unitPrice">单价</param>
        /// <param name="orderDate">订单日期</param>
        /// <param name="PaymentMethod">支付方式</param>
        /// <returns>0失败/1成功/2套餐被锁定/3不允许购买多个/4已 免费领取 此套餐/5 设备号码验证/6无有效已验证号码/8组合ID与套餐不匹配</returns>
        public async Task<KeyValuePair<string, KeyValuePair<int, UT_Order>>> AddOrder(Guid userId, Guid packageId, int quantity, PaymentMethodType PaymentMethod, decimal MonthPackageFee, Guid? packageAttributeId, DateTime? BeginDateTime)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                UT_Package package = await db.UT_Package.Include(x => x.UT_Country).SingleOrDefaultAsync(x => x.ID == packageId);
                //套餐被锁定
                if (package.Lock4 == 1)
                {
                    return new KeyValuePair<string, KeyValuePair<int, UT_Order>>("", new KeyValuePair<int, UT_Order>(2, null));
                }
                //该套餐不允许购买多个
                if (!package.IsCanBuyMultiple && quantity != 1)
                {
                    return new KeyValuePair<string, KeyValuePair<int, UT_Order>>("", new KeyValuePair<int, UT_Order>(3, null));
                }
                //免费领取类型的套餐不在此方法处理
                if (package.Category == CategoryType.FreeReceive)
                {
                    return new KeyValuePair<string, KeyValuePair<int, UT_Order>>("", new KeyValuePair<int, UT_Order>(0, null));
                }

                string deviceTel = null;
                //轻松服务类型的套餐不在此方法处理
                if (package.Category == CategoryType.Relaxed)
                {
                    UT_UserDeviceTel userDeviceTel = await _userDeviceTelService.GetFirst(userId);
                    if (userDeviceTel == null)
                    {
                        return new KeyValuePair<string, KeyValuePair<int, UT_Order>>("", new KeyValuePair<int, UT_Order>(6, null));
                    }
                    deviceTel = userDeviceTel.Tel;
                    if (string.IsNullOrEmpty(deviceTel))
                    {
                        return new KeyValuePair<string, KeyValuePair<int, UT_Order>>("", new KeyValuePair<int, UT_Order>(0, null));
                    }
                }

                UT_PackageAttribute packageAttribute = null;
                if (packageAttributeId.HasValue)
                {
                    packageAttribute = await db.UT_PackageAttribute.FindAsync(packageAttributeId);
                    if (packageAttribute.PackageId != packageId)
                    {
                        return new KeyValuePair<string, KeyValuePair<int, UT_Order>>("", new KeyValuePair<int, UT_Order>(8, null));
                    }
                }

                return await Add(db, userId, package, packageAttribute, quantity, PaymentMethod, deviceTel, MonthPackageFee, BeginDateTime);
            }
        }

        /// <summary>
        /// 添加免费领取订单
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="orderName">订单名</param>
        /// <param name="packageId">套餐ID</param>
        /// <param name="quantity">数量</param>
        /// <param name="unitPrice">单价</param>
        /// <param name="orderDate">订单日期</param>
        /// <param name="PaymentMethod">支付方式</param>
        /// <returns>0失败/1成功/2套餐被锁定/3不允许购买多个/4不是免费套餐/5已 免费领取 此套餐</returns>
        public async Task<KeyValuePair<string, KeyValuePair<int, UT_Order>>> AddReceiveOrder(Guid userId, Guid packageId)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                UT_Package package = await db.UT_Package.Include(x => x.UT_Country).SingleOrDefaultAsync(x => x.ID == packageId);
                //套餐被锁定
                if (package.Lock4 == 1)
                {
                    return new KeyValuePair<string, KeyValuePair<int, UT_Order>>("", new KeyValuePair<int, UT_Order>(2, null));
                }
                //如果不是免费套餐
                if (package.Category != CategoryType.FreeReceive)
                {
                    return new KeyValuePair<string, KeyValuePair<int, UT_Order>>("", new KeyValuePair<int, UT_Order>(4, null));
                }
                //已 免费领取 此套餐
                if (UserReceiveService.CheckHaveed(db, userId, packageId))
                {
                    return new KeyValuePair<string, KeyValuePair<int, UT_Order>>("", new KeyValuePair<int, UT_Order>(5, null));
                }
                return await Add(db, userId, package, null, 1, PaymentMethodType.Gift, null, 0, null);
            }
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="package"></param>
        /// <param name="quantity"></param>
        /// <param name="PaymentMethod"></param>
        /// <param name="deviceTel"></param>
        /// <param name="MonthPackageFee"></param>
        /// <returns></returns>
        private static async Task<KeyValuePair<string, KeyValuePair<int, UT_Order>>> Add(UnitoysEntities db, Guid userId, UT_Package package, UT_PackageAttribute packageAttribute, int quantity, PaymentMethodType PaymentMethod, string deviceTel, decimal MonthPackageFee, DateTime? BeginDateTime)
        {
            if (package != null)
            {
                //1. 先添加Order实体。
                UT_Order order = new UT_Order();
                order.UserId = userId;
                order.OrderNum = String.Format("8022{0}", DateTime.Now.ToString("yyMMddHHmmssffff"));
                order.PackageId = package.ID;
                order.PackageName = package.PackageName;
                order.PackageCategory = package.Category;
                order.PackageIsCategoryFlow = package.IsCategoryFlow;
                order.PackageIsCategoryCall = package.IsCategoryCall;
                order.PackageIsCategoryDualSimStandby = package.IsCategoryDualSimStandby;
                order.PackageIsCategoryKingCard = package.IsCategoryKingCard;
                order.OrderDate = CommonHelper.GetDateTimeInt();
                order.PayStatus = 0; //添加时付款状态默认为0：未付款。
                order.Quantity = quantity;
                order.UnitPrice = package.Price;
                order.TotalPrice = package.Price * quantity;
                order.Flow = package.Flow * quantity;
                order.OrderStatus = 0; //添加时订单状态默认为0：未激活。
                order.ExpireDays = package.ExpireDays;
                order.RemainingCallMinutes = package.CallMinutes;
                order.PackageFeatures = package.Features;
                order.PackageDetails = package.Details;
                order.PackageIsSupport4G = package.IsSupport4G;
                order.PackageIsApn = package.IsApn;
                order.PackageApnName = package.ApnName;

                if (BeginDateTime.HasValue)
                {
                    order.EffectiveDate = CommonHelper.ConvertDateTimeInt(BeginDateTime.Value);
                    order.EffectiveDateDesc = BeginDateTime;
                }

                //order.PayUserAmount = PayUserAmount;
                //order.IsPayUserAmount = IsPayUserAmount;
                order.PaymentMethod = PaymentMethod;

                if (packageAttribute != null)
                {
                    order.PackageAttributeId = packageAttribute.ID;
                    order.RemainingCallMinutes = packageAttribute.CallMinutes ?? -1;
                    order.ExpireDays = packageAttribute.ExpireDays ?? -1;
                    order.UnitPrice = packageAttribute.Price;
                    order.TotalPrice = packageAttribute.Price * quantity;
                    order.Flow = packageAttribute.Flow.HasValue ? packageAttribute.Flow.Value * quantity : -1;
                }

                //免费领取
                if (package.Category == CategoryType.FreeReceive)
                {
                    UT_UserReceive userReceice = new UT_UserReceive()
                    {
                        UserId = userId,
                        OrderId = order.ID,
                        PackageId = package.ID,
                        CreateDate = CommonHelper.GetDateTimeInt()
                    };
                    db.UT_UserReceive.Add(userReceice);
                    order.ActivationDate = CommonHelper.GetDateTimeInt();
                    order.OrderStatus = OrderStatusType.Used;
                    order.PayStatus = PayStatusType.YesPayment;
                    order.EffectiveDate = CommonHelper.ConvertDateTimeInt(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));
                    SetExpireDate(order);
                    //order.EffectiveDateDesc = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                }

                //轻松服务
                if (package.Category == CategoryType.Relaxed)
                {
                    UT_OrderDeviceTel orderDeviceTel = new UT_OrderDeviceTel()
                    {
                        MonthPackageFee = MonthPackageFee,
                        Tel = deviceTel,
                        OrderId = order.ID,
                    };
                    order.TotalPrice += MonthPackageFee * order.ExpireDays;
                    db.UT_OrderDeviceTel.Add(orderDeviceTel);
                }
                db.UT_Order.Add(order);

                if (await db.SaveChangesAsync() > 0)
                {
                    //返回区域名称
                    if (package.UT_Country != null)
                    {
                        return new KeyValuePair<string, KeyValuePair<int, UT_Order>>(package.UT_Country.CountryName, new KeyValuePair<int, UT_Order>(1, order));
                    }
                    else
                    {
                        return new KeyValuePair<string, KeyValuePair<int, UT_Order>>("", new KeyValuePair<int, UT_Order>(1, order));
                    }
                }
            }
            return new KeyValuePair<string, KeyValuePair<int, UT_Order>>("", new KeyValuePair<int, UT_Order>(0, null));
        }

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
        /// 通过用户余额支付套餐订单
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="orderId">订单ID</param>
        /// <returns></returns>
        public async Task<int> PayOrderByUserAmount(Guid userId, Guid orderId)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                //1. 根据用户ID和订单ID获取相应的Entity。
                UT_Users payUser = await db.UT_Users.FindAsync(userId);
                UT_Order payOrder = await db.UT_Order.Include(x => x.UT_Package).Where(a => a.ID == orderId).SingleOrDefaultAsync();

                if (payUser != null && payOrder != null)
                {
                    //判断此订单是否为余额支付，如果不是则返回-5
                    if (payOrder.PaymentMethod != PaymentMethodType.Balance)
                    {
                        return -5;
                    }
                    //判断此订单是否已经付款，如果已付款则返回-2。
                    if (payOrder.PayDate != null && payOrder.PayStatus == PayStatusType.YesPayment)
                    {
                        return -2;
                    }
                    //判断此订单的用户ID，如果不属于该用户的订单则返回-3。
                    else if (payOrder.UserId != payUser.ID)
                    {
                        return -3;
                    }

                    //2. 首先计算出订单的金额是否大于用户的余额
                    decimal orderTotalAmount = payOrder.TotalPrice;

                    if (payUser.Amount < orderTotalAmount)
                    {
                        //如果用户余额小于订单需要支付金额，则返回-4。
                        return -4;
                    }
                    else
                    {
                        //3. 如果用户余额大于等于订单需要支付金额，则扣除用户余额，将订单状态改为完成支付。

                        //用户余额扣除订单的总金额。
                        payUser.Amount -= orderTotalAmount;

                        //订单支付日期赋值为当前，并状态设置为已付款。
                        payOrder.PayDate = CommonHelper.GetDateTimeInt();
                        payOrder.PayStatus = PayStatusType.YesPayment;

                        //如果属于通话套餐或双卡双待套餐则默认激活
                        //if ((payOrder.PackageIsCategoryDualSimStandby || payOrder.PackageIsCategoryCall) && payOrder.PackageIsCategoryFlow == false)
                        //{
                        //    payOrder.OrderStatus = OrderStatusType.Used;
                        //    payOrder.EffectiveDate = CommonHelper.GetDateTimeInt();
                        //    payOrder.ActivationDate = CommonHelper.GetDateTimeInt();

                        //    var remark = "add：PackageCategory" + payOrder.PackageIsCategoryFlow.ToString() + payOrder.PackageIsCategoryCall.ToString() + payOrder.PackageIsCategoryDualSimStandby.ToString() + payOrder.PackageIsCategoryKingCard.ToString();
                        //    payOrder.Remark = string.IsNullOrEmpty(payOrder.Remark) ? remark : payOrder.Remark + remark;
                        //}

                        //如果属于通话套餐或双卡双待套餐则默认激活
                        if (payOrder.PackageCategory == CategoryType.DualSimStandby || payOrder.PackageCategory == CategoryType.Call || payOrder.PackageCategory == CategoryType.Relaxed)
                        {
                            payOrder.OrderStatus = OrderStatusType.Used;
                            payOrder.EffectiveDate = CommonHelper.GetDateTimeInt();
                            payOrder.ActivationDate = CommonHelper.GetDateTimeInt();
                            SetExpireDate(payOrder);
                            payOrder.Remark = string.IsNullOrEmpty(payOrder.Remark) ? payOrder.PackageCategory.ToString() : payOrder.Remark + payOrder.PackageCategory.ToString();
                        }

                        db.UT_Users.Attach(payUser);
                        db.UT_Order.Attach(payOrder);
                        db.Entry<UT_Users>(payUser).State = EntityState.Modified;
                        db.Entry<UT_Order>(payOrder).State = EntityState.Modified;

                        //建立UserBill记录账单。
                        UT_UserBill userBill = new UT_UserBill();
                        userBill.UserId = payUser.ID;
                        userBill.LoginName = payUser.Tel;
                        userBill.Amount = orderTotalAmount;
                        userBill.UserAmount = payUser.Amount;
                        userBill.CreateDate = CommonHelper.GetDateTimeInt();
                        userBill.BillType = 0; //支出
                        userBill.PayType = 2; //余额支付
                        userBill.Descr = "购买套餐:" + payOrder.PackageName;

                        db.UT_UserBill.Add(userBill);

                        var payResult = await db.SaveChangesAsync() > 0 ? 0 : -1;

                        //付款成功后激活订单
                        if (payResult != 0)
                        {
                            return payResult;
                        }
                        //流量套餐-激活订单
                        if (payOrder.PackageCategory == CategoryType.Flow && (payOrder.EffectiveDate.HasValue || payOrder.EffectiveDateDesc.HasValue))
                        {
                            var activationResult = await Activation(payUser.ID, payOrder, payOrder.UT_Package, payUser, payOrder.EffectiveDate, payOrder.EffectiveDateDesc, db);
                            if (activationResult == 10)
                            {
                                if (await db.SaveChangesAsync() <= 0)
                                {
                                    LoggerHelper.Error("套餐激活成功,更新数据库失败");
                                    //return Ok(new StatusCodeRes(StatusCodeType.内部错误, "更新订单失败"));
                                    return -16;
                                }
                            }
                            return activationResult;
                        }
                        return 0;
                    }
                }
                return -1;
            }
        }

        /// <summary>
        /// 订单支付成功后调用
        /// </summary>
        /// <param name="orderNum">订单编号</param>
        /// <param name="payAmount">支付金额</param>
        /// <returns></returns>
        public async Task<int> OnAfterOrderSuccess(string orderNum, decimal payAmount)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                //根据orderId获取Order对象。
                UT_Order order = await db.UT_Order.Include(x => x.UT_Package).Where(a => a.OrderNum == orderNum).SingleOrDefaultAsync();

                if (order != null)
                {
                    //根据order的UserId获取User。
                    UT_Users user = await db.UT_Users.FindAsync(order.UserId);
                    //订单应在线支付金额
                    decimal orderOnlineAmount = order.TotalPrice;

                    //如果支付的金额小于订单应付金额
                    if (payAmount < orderOnlineAmount)
                    {
                        order.PayDate = CommonHelper.GetDateTimeInt();
                        order.PayStatus = PayStatusType.AbnormalPayment;
                        order.Remark = "实际支付金额：" + payAmount;
                        db.UT_Order.Attach(order);
                        db.Entry<UT_Order>(order).State = EntityState.Modified;
                        if (await db.SaveChangesAsync() <= 0)
                        {
                            LoggerHelper.Error("订单错误支付更新失败", new Exception("订单错误支付更新失败"));
                        }
                        return -1;
                    }
                    if (payAmount > orderOnlineAmount)
                    {
                        order.Remark = "实际支付金额：" + payAmount;
                    }

                    //如果是已付款状态，直接返回true表示已经处理好了。
                    if (order.PayStatus == PayStatusType.YesPayment) return 0;

                    //设置为已付款，付款日期设置为当前，并保存。
                    order.PayDate = CommonHelper.GetDateTimeInt();
                    order.PayStatus = PayStatusType.YesPayment;

                    ////如果属于通话套餐或双卡双待套餐则默认激活
                    //if ((order.PackageIsCategoryDualSimStandby || order.PackageIsCategoryCall) && order.PackageIsCategoryFlow == false)
                    //{
                    //    order.OrderStatus = OrderStatusType.Used;
                    //    order.EffectiveDate = CommonHelper.GetDateTimeInt();
                    //    order.ActivationDate = CommonHelper.GetDateTimeInt();

                    //    var remark = "add：PackageCategory" + order.PackageIsCategoryFlow.ToString() + order.PackageIsCategoryCall.ToString() + order.PackageIsCategoryDualSimStandby.ToString() + order.PackageIsCategoryKingCard.ToString();
                    //    order.Remark = string.IsNullOrEmpty(order.Remark) ? remark : order.Remark + remark;
                    //}

                    //如果属于通话套餐或双卡双待套餐则默认激活
                    if (order.PackageCategory == CategoryType.DualSimStandby || order.PackageCategory == CategoryType.Call || order.PackageCategory == CategoryType.Relaxed)
                    {
                        order.OrderStatus = OrderStatusType.Used;
                        order.EffectiveDate = CommonHelper.GetDateTimeInt();
                        order.ActivationDate = CommonHelper.GetDateTimeInt();
                        SetExpireDate(order);
                        order.Remark = string.IsNullOrEmpty(order.Remark) ? order.PackageCategory.ToString() : order.Remark + order.PackageCategory.ToString();
                    }

                    db.UT_Order.Attach(order);
                    db.Entry<UT_Order>(order).State = EntityState.Modified;

                    //建立UserBill记录账单。
                    UT_UserBill userBill = new UT_UserBill();
                    userBill.UserId = order.UserId;
                    userBill.LoginName = user.Tel;
                    userBill.Amount = orderOnlineAmount;
                    userBill.UserAmount = user.Amount;
                    userBill.CreateDate = CommonHelper.GetDateTimeInt();
                    userBill.BillType = 0; //支出
                    userBill.PayType = 1; //在线支付
                    userBill.Descr = "购买套餐:" + order.PackageName;

                    db.UT_UserBill.Add(userBill);

                    //TODO 合并用户余额支付部分，待删除
                    //如果是包含余额支付的订单，扣除用户余额,记录账单
                    //if (order.IsPayUserAmount == 1 || order.PayUserAmount > 0)
                    //{
                    //    //不足以扣除的话，将在线支付的金额退回用户账户余额，取消订单
                    //    if (user.Amount >= order.PayUserAmount)
                    //    {
                    //        user.Amount -= order.PayUserAmount;
                    //        //建立UserBill记录账单。
                    //        UT_UserBill userOrderBill = new UT_UserBill();
                    //        userOrderBill.UserId = order.UserId;
                    //        userOrderBill.LoginName = user.Tel;
                    //        userOrderBill.Amount = order.PayUserAmount;
                    //        userOrderBill.UserAmount = user.Amount;
                    //        userOrderBill.CreateDate = CommonHelper.GetDateTimeInt();
                    //        userOrderBill.BillType = 0; //支出
                    //        userOrderBill.PayType = 6; //订单余额支付
                    //        userOrderBill.Descr = "购买套餐：" + order.UT_Package.PackageName;

                    //        db.UT_UserBill.Add(userOrderBill);
                    //    }
                    //    else
                    //    {
                    //        user.Amount += orderOnlineAmount;
                    //        //建立UserBill记录账单。
                    //        UT_UserBill userOrderBill = new UT_UserBill();
                    //        userOrderBill.UserId = order.UserId;
                    //        userOrderBill.LoginName = user.Tel;
                    //        userOrderBill.Amount = orderOnlineAmount;
                    //        userOrderBill.UserAmount = user.Amount;
                    //        userOrderBill.CreateDate = CommonHelper.GetDateTimeInt();
                    //        userOrderBill.BillType = 1; //收入
                    //        userOrderBill.PayType = 7; //订单退款

                    //        //取消订单
                    //        order.OrderStatus = OrderStatusType.Cancel;

                    //        db.UT_UserBill.Add(userOrderBill);
                    //    }

                    //    db.UT_Users.Attach(user);
                    //    db.Entry<UT_Users>(user).State = EntityState.Modified;
                    //}

                    var payResult = await db.SaveChangesAsync() > 0 ? 0 : -1;

                    //付款成功后激活订单
                    if (payResult != 0)
                    {
                        return payResult;
                    }
                    //流量套餐-激活订单
                    if (order.PackageCategory == CategoryType.Flow && (order.EffectiveDate.HasValue || order.EffectiveDateDesc.HasValue))
                    {
                        var activationResult = await Activation(order.ID, order, order.UT_Package, user, order.EffectiveDate, order.EffectiveDateDesc, db);
                        if (activationResult == 10)
                        {
                            if (await db.SaveChangesAsync() <= 0)
                            {
                                LoggerHelper.Error("套餐激活成功,更新数据库失败");
                                //return Ok(new StatusCodeRes(StatusCodeType.内部错误, "更新订单失败"));
                                return -16;
                            }
                        }
                        return activationResult;
                    }
                    return 0;
                }
                return -1;
            }
        }

        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="orderId">订单ID</param>
        /// <param name="descr">描述前缀</param>
        /// <returns></returns>
        public async Task<int> CancelOrder(Guid userId, Guid orderId, string descr = "")
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                //根据orderId获取Order对象。
                UT_Order order = await db.UT_Order.FindAsync(orderId);
                //根据Payment的UserId获取User，添加充值金额到用户上，并保存。
                UT_Users user = await db.UT_Users.FindAsync(order.UserId);

                //订单为未激活，并且不是已取消
                if (order != null && user != null)
                {
                    //判断此订单的用户ID，如果不属于该用户的订单则返回-3。
                    if (order.UserId != user.ID)
                    {
                        return -3;
                    }
                    //判断此订单是否已经取消，如果已取消则返回-2。
                    else if (order.OrderStatus == OrderStatusType.Cancel)
                    {
                        return -2;
                    }
                    //判断此订单是否不是未激活，如果不是未激活则返回-4。
                    if (order.OrderStatus != OrderStatusType.Unactivated)
                    {
                        return -4;
                    }
                    if (order.PaymentMethod == PaymentMethodType.Gift)
                    {
                        return -5;
                    }

                    //设置状态为已取消。
                    order.OrderStatus = OrderStatusType.Cancel;

                    db.UT_Order.Attach(order);
                    db.Entry<UT_Order>(order).State = EntityState.Modified;

                    //如果支付状态为已支付，添加订单金额，返现到用户上，并保存。;
                    if (order.PayStatus == PayStatusType.YesPayment)
                    {
                        user.Amount += order.Quantity * order.UnitPrice;
                        db.UT_Users.Attach(user);
                        db.Entry<UT_Users>(user).State = EntityState.Modified;


                        //建立UserBill记录账单。
                        UT_UserBill userBill = new UT_UserBill();
                        userBill.UserId = order.UserId;
                        userBill.LoginName = user.Tel;
                        userBill.Amount = order.Quantity * order.UnitPrice;
                        userBill.UserAmount = user.Amount;
                        userBill.CreateDate = CommonHelper.GetDateTimeInt();
                        userBill.BillType = 1; //收入
                        userBill.PayType = 5; //取消订单
                        userBill.Descr = descr + "取消订单:" + order.PackageName;

                        db.UT_UserBill.Add(userBill);
                    }

                    return await db.SaveChangesAsync() > 0 ? 0 : -1;
                }
                return -1;
            }
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="row">页数</param>
        /// <param name="orderNum">订单号</param>
        /// <param name="tel">用户手机号</param>
        /// <param name="packageName">套餐名</param>
        /// <param name="payStatus">付款状态</param>
        /// <param name="orderStatus">订单状态</param>
        /// <returns></returns>
        public async Task<KeyValuePair<int, List<UT_Order>>> SearchAsync(int page, int row, string orderNum, string tel, string packageName, PayStatusType? payStatus, OrderStatusType? orderStatus)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = db.UT_Order.Include(x => x.UT_Users).Include(x => x.UT_Package).Include(x => x.UT_Package.UT_Country);

                if (!string.IsNullOrEmpty(orderNum))
                {
                    query = query.Where(x => x.OrderNum.Contains(orderNum));
                }

                if (!string.IsNullOrEmpty(tel))
                {
                    query = query.Where(x => x.UT_Users.Tel.Contains(tel));
                }

                if (!string.IsNullOrEmpty(packageName))
                {
                    query = query.Where(x => x.PackageName.Contains(packageName));
                }

                if (payStatus != null)
                {
                    query = query.Where(x => x.PayStatus == payStatus);
                }

                if (orderStatus != null)
                {
                    query = query.Where(x => x.OrderStatus == orderStatus);
                }

                var result = await query.OrderByDescending(x => x.OrderDate).Skip((page - 1) * row).Take(row).ToListAsync();

                var count = await query.CountAsync();

                return new KeyValuePair<int, List<UT_Order>>(count, result);
            }
        }

        /// <summary>
        /// 查询用户订单
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="row">页数</param>
        /// <param name="userId">用户</param>
        /// <param name="payStatus">支付状态</param>
        /// <returns></returns>
        public async Task<KeyValuePair<int, List<UT_Order>>> GetUserOrderList(int page, int row, Guid userId, PayStatusType? payStatus, OrderStatusType? orderStatus, CategoryType? PackageCategory, bool? packageIsCategoryFlow, bool? packageIsCategoryCall, bool? packageIsCategoryDualSimStandby, bool? packageIsCategoryKingCard)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = db.UT_Order.Include(x => x.UT_Users).Include(x => x.UT_Package).Include(x => x.UT_Package.UT_Country).Where(x => true);

                if (payStatus != null)
                {
                    query = query.Where(x => x.PayStatus == payStatus);
                }
                if (orderStatus.HasValue)
                {
                    if (orderStatus == OrderStatusType.Unactivated)
                        query = query.Where(x => x.OrderStatus == orderStatus || x.OrderStatus == OrderStatusType.UnactivatError);
                    else
                        query = query.Where(x => x.OrderStatus == orderStatus);
                }
                query = query.Where(x => x.OrderStatus != OrderStatusType.Cancel);
                query = query.Where(x => x.UserId == userId);

                if (PackageCategory.HasValue)
                {
                    query = query.Where(x => x.PackageCategory == PackageCategory.Value);
                }
                if (packageIsCategoryFlow.HasValue)
                {
                    query = query.Where(x => x.PackageIsCategoryFlow == packageIsCategoryFlow.Value);
                }
                if (packageIsCategoryCall.HasValue)
                {
                    query = query.Where(x => x.PackageIsCategoryCall == packageIsCategoryCall.Value);
                }
                if (packageIsCategoryDualSimStandby.HasValue)
                {
                    query = query.Where(x => x.PackageIsCategoryDualSimStandby == packageIsCategoryDualSimStandby.Value);
                }
                if (packageIsCategoryKingCard.HasValue)
                {
                    query = query.Where(x => x.PackageIsCategoryKingCard == packageIsCategoryKingCard.Value);
                }

                var result = await query.OrderByDescending(x => x.OrderStatus != OrderStatusType.HasExpired).ThenByDescending(x => x.OrderDate).Skip((page - 1) * row).Take(row).ToListAsync();

                var count = await query.CountAsync();

                return new KeyValuePair<int, List<UT_Order>>(count, result);
            }
        }

        /// <summary>
        /// 判断当前用户是否有正在使用的套餐
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsStatusUsed(Guid ID)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                return await db.UT_Order.AnyAsync(a => a.OrderStatus == OrderStatusType.Used && a.UserId == ID);
            }
        }

        /// <summary>
        /// 根据ID获取订单和订单中的套餐
        /// </summary>
        /// <returns></returns>
        public async Task<UT_Order> GetEntityAndPackageByIdAsync(Guid ID)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                return await db.UT_Order.Include(x => x.UT_Package.UT_Country).FirstOrDefaultAsync(x => x.ID == ID);
            }
        }

        /// <summary>
        /// 根据套餐类型判断当前用户是否有正在使用的套餐
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsStatusUsed(Guid userId, CategoryType? PackageCategory, bool? packageIsCategoryFlow, bool? packageIsCategoryCall, bool? packageIsCategoryDualSimStandby, bool? packageIsCategoryKingCard)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = db.UT_Order.Where(a => a.OrderStatus == OrderStatusType.Used && a.UserId == userId);

                if (PackageCategory.HasValue)
                {
                    query = query.Where(x => x.PackageCategory == PackageCategory.Value);
                }
                if (packageIsCategoryFlow.HasValue)
                {
                    query = query.Where(x => x.PackageIsCategoryFlow == packageIsCategoryFlow.Value);
                }
                if (packageIsCategoryCall.HasValue)
                {
                    query = query.Where(x => x.PackageIsCategoryCall == packageIsCategoryCall.Value);
                }
                if (packageIsCategoryDualSimStandby.HasValue)
                {
                    query = query.Where(x => x.PackageIsCategoryDualSimStandby == packageIsCategoryDualSimStandby.Value);
                }
                if (packageIsCategoryKingCard.HasValue)
                {
                    query = query.Where(x => x.PackageIsCategoryKingCard == packageIsCategoryKingCard.Value);
                }
                return await query.AnyAsync();
            }
        }

        /// <summary>
        /// 激活订单（MVNO购卡）
        /// </summary>
        /// <param name="UserID">用户ID</param>
        /// <param name="OrderID">订单ID</param>
        /// <param name="BeginTime">开始生效时间戳</param>
        /// <param name="BeginDateTime">开始生效日期格式</param>
        /// <returns>
        /// -12：激活失败_超过最晚激活日期
        /// -13：激活失败_激活类型异常（非流量套餐）
        /// -14：激活套餐失败_可能套餐已过期 "暂时无法激活,请联系客服"
        /// -15：卡激活过程异常（购卡异常）
        /// -11：失败
        /// 10：成功
        /// -16：套餐激活成功,更新数据库失败
        /// </returns>
        public async Task<int> Activation(Guid UserID, Guid OrderID, int? BeginTime, DateTime? BeginDateTime)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var order = await db.UT_Order.FindAsync(OrderID);
                var result = await Activation(UserID, order, null, null, BeginTime, BeginDateTime, db);
                if (result == 10)
                {
                    if (await db.SaveChangesAsync() <= 0)
                    {
                        LoggerHelper.Error("套餐激活成功,更新数据库失败");
                        //return Ok(new StatusCodeRes(StatusCodeType.内部错误, "更新订单失败"));
                        return -16;
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// 激活订单（MVNO购卡）
        /// </summary>
        /// <param name="UserID">用户ID</param>
        /// <param name="order">订单实体</param>
        /// <param name="package">套餐实体，减少多余的访问，如果原来已获取到，便传入，不传入则根据order的套餐来获取</param>
        /// <param name="user">用户实体，减少多余的访问，如果原来已获取到，便传入，不传入则根据order的用户来获取</param>
        /// <param name="BeginTime">开始生效时间戳</param>
        /// <param name="BeginDateTime">开始生效日期格式</param>
        /// <param name="db">数据库上下午，用于复用与事务</param>
        /// <returns>
        /// -12：激活失败_超过最晚激活日期
        /// -13：激活失败_激活类型异常（非流量套餐）
        /// -14：激活套餐失败_可能套餐已过期 "暂时无法激活,请联系客服"
        /// -15：卡激活过程异常（购卡异常）
        /// -11：失败
        /// 10：成功
        /// </returns>
        private async Task<int> Activation(Guid UserID, UT_Order order, UT_Package package, UT_Users user, int? BeginTime, DateTime? BeginDateTime, UnitoysEntities db)
        {
            //为什么用10和-11等数字，只是作用于区分此错误属于激活部分
            //UT_Order order = await db.UT_Order.FindAsync(OrderID);
            if (order != null && UserID == order.UserId && order.PayDate != null && order.PayStatus == PayStatusType.YesPayment)
            {
                var LastCanActivationDate = GetLastCanActivationDate(order);

                if (CommonHelper.GetDateTimeInt() > LastCanActivationDate)
                {
                    return -12;
                    //return Ok(new StatusCodeRes(StatusCodeType.激活失败_超过最晚激活日期));
                }
                if (order.PackageCategory != CategoryType.Flow)
                {
                    //
                    return -13;
                    //return Ok(new StatusCodeRes(StatusCodeType.激活失败_激活类型异常));
                }
                if (order.OrderStatus == 0)
                {
                    //1.购买产品
                    if (package == null)
                        package = await db.UT_Package.FindAsync(order.PackageId);
                    if (user == null)
                        user = await db.UT_Users.FindAsync(order.UserId);
                    try
                    {
                        //2.购买订单卡
                        //var result = await ESIMUtil.BuyProduct(package.PackageNum, user.Tel, order.OrderNum, model.BeginTime, order.Quantity * package.ExpireDays);

                        ResponseModel<BuyProduct> result = null;

                        //服务生效时间，激活时间。兼容时间戳版本
                        string beginTime = BeginDateTime.HasValue
                            ? BeginDateTime.Value.ToString("yyyy-MM-dd HH:mm:ss")
                            : CommonHelper.GetTime(BeginTime + "").ToString("yyyy-MM-dd HH:mm:ss");

                        //如果是不能购买多个的套餐则认为有效天数字段只是用于描述
                        if (package.IsCanBuyMultiple)
                        {
                            result = await new Unitoys.ESIM_MVNO.MVNOServiceApi().BuyProduct(user.Tel, package.PackageNum, beginTime, order.Quantity * package.ExpireDays);
                        }
                        else
                        {
                            result = await new Unitoys.ESIM_MVNO.MVNOServiceApi().BuyProduct(user.Tel, package.PackageNum, beginTime, order.Quantity);
                        }

                        if (result.status != "1")
                        {
                            LoggerHelper.Error("订单ID:" + order.ID + ",购买产品失败,返回msg：" + result.msg);
                            return -14;
                            //return Ok(new StatusCodeRes(StatusCodeType.激活套餐失败_可能套餐已过期, "暂时无法激活,请联系客服"));
                            //return Ok(new StatusCodeRes(StatusCodeType.激活套餐失败_可能套餐已过期));
                        }

                        //3.保存订单Id
                        order.PackageOrderId = result.data.orderId;
                        //order.PackageOrderData = result.data.data;

                        order.EffectiveDate = BeginDateTime.HasValue ? CommonHelper.ConvertDateTimeInt(BeginDateTime.Value) : BeginTime;
                        order.EffectiveDateDesc = BeginDateTime;

                        SetExpireDate(order);

                        order.OrderStatus = OrderStatusType.UnactivatError;//默认是激活失败
                        order.ActivationDate = CommonHelper.GetDateTimeInt();

                        db.UT_Order.Attach(order);
                        db.Entry<UT_Order>(order).State = EntityState.Modified;

                        //if (!await _orderService.UpdateAsync(order))
                        //{
                        //    LoggerHelper.Error("套餐激活成功,更新数据库失败");
                        //    return Ok(new StatusCodeRes(StatusCodeType.内部错误, "更新订单失败"));
                        //}
                    }
                    catch (Exception ex)
                    {
                        //卡激活过程异常
                        LoggerHelper.Error(ex.Message, ex);
                        return -15;
                        //throw;
                    }
                }
                return 10;
                //4.返回订单卡数据
                //return Ok(new { status = 1, msg = "订单待激活", data = new { OrderID = order.ID } });// Data = order.PackageOrderData 
            }
            //return Ok(new StatusCodeRes(StatusCodeType.失败, "激活失败，可能订单不存在或未支付"));

            return -11;
        }
    }
}
