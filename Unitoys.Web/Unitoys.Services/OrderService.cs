﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Core;
using Unitoys.IServices;
using Unitoys.Model;
using System.Data.Entity;
using System.Linq.Expressions;

namespace Unitoys.Services
{
    public class OrderService : BaseService<UT_Order>, IOrderService
    {
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
        /// <returns>0失败/1成功/2套餐被锁定</returns>
        public async Task<KeyValuePair<int, UT_Order>> AddOrder(Guid userId, Guid packageId, int quantity, PaymentMethodType PaymentMethod)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                UT_Package package = await db.UT_Package.FindAsync(packageId);
                //套餐被锁定
                if (package.Lock4 == 1)
                {
                    return new KeyValuePair<int, UT_Order>(2, null);
                }
                //该套餐不允许购买多个
                if (!package.IsCanBuyMultiple && quantity != 1)
                {
                    return new KeyValuePair<int, UT_Order>(3, null);
                }
                if (package != null)
                {
                    //1. 先添加Order实体。
                    UT_Order order = new UT_Order();
                    order.UserId = userId;
                    order.OrderNum = String.Format("8022{0}", DateTime.Now.ToString("yyMMddHHmmssffff"));
                    order.PackageId = packageId;
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
                    //order.PayUserAmount = PayUserAmount;
                    //order.IsPayUserAmount = IsPayUserAmount;
                    order.PaymentMethod = PaymentMethod;
                    db.UT_Order.Add(order);

                    if (await db.SaveChangesAsync() > 0)
                    {
                        return new KeyValuePair<int, UT_Order>(1, order);
                    }
                }
                return new KeyValuePair<int, UT_Order>(0, null);
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
                    decimal orderTotalAmount = payOrder.UnitPrice * payOrder.Quantity;

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
                        if (payOrder.PackageCategory == CategoryType.DualSimStandby || payOrder.PackageCategory == CategoryType.Call)
                        {
                            payOrder.OrderStatus = OrderStatusType.Used;
                            payOrder.EffectiveDate = CommonHelper.GetDateTimeInt();
                            payOrder.ActivationDate = CommonHelper.GetDateTimeInt();
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

                        return await db.SaveChangesAsync() > 0 ? 0 : -1;
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
        public async Task<bool> OnAfterOrderSuccess(string orderNum, decimal payAmount)
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
                    decimal orderOnlineAmount = order.Quantity * order.UnitPrice;

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
                        return false;
                    }
                    if (payAmount > orderOnlineAmount)
                    {
                        order.Remark = "实际支付金额：" + payAmount;
                    }

                    //如果是已付款状态，直接返回true表示已经处理好了。
                    if (order.PayStatus == PayStatusType.YesPayment) return true;

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
                    if (order.PackageCategory == CategoryType.DualSimStandby || order.PackageCategory == CategoryType.Call)
                    {
                        order.OrderStatus = OrderStatusType.Used;
                        order.EffectiveDate = CommonHelper.GetDateTimeInt();
                        order.ActivationDate = CommonHelper.GetDateTimeInt();
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

                    return await db.SaveChangesAsync() > 0;
                }
                return false;
            }
        }

        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="orderNum">订单编号</param>
        /// <returns></returns>
        public async Task<int> CancelOrder(Guid userId, Guid orderId)
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
                        userBill.Descr = "取消订单:" + order.PackageName;

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
    }
}
