using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.IServices;
using Unitoys.Model;
using System.Data.Entity;
using Unitoys.Core;

namespace Unitoys.Services
{
    public class OrderByZCSelectionNumberService : BaseService<UT_OrderByZCSelectionNumber>, IOrderByZCSelectionNumberService
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
        /// <returns></returns>
        public async Task<UT_OrderByZCSelectionNumber> AddOrder(Guid userId, Guid orderByZCId, string Name, string IdentityNumber, string MobileNumber, PaymentMethodType? PaymentMethod)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                UT_ZCSelectionNumber zcSelectionNumber = await db.UT_ZCSelectionNumber.FirstOrDefaultAsync(x => x.MobileNumber == MobileNumber && x.OrderByZCSelectionNumberId == null);
                UT_OrderByZC orderZC = await db.UT_OrderByZC.FirstOrDefaultAsync(x => x.ID == orderByZCId);

                if (zcSelectionNumber != null && orderZC != null && orderZC.UserId == userId)
                {
                    //1. 先添加OrderByZCSelectionNumber实体。
                    UT_OrderByZCSelectionNumber order = new UT_OrderByZCSelectionNumber();
                    order.OrderByZCId = orderByZCId;
                    order.OrderByZCSelectionNumberNum = String.Format("1022{0}", DateTime.Now.ToString("yyyyMMddHHmmssfffffff"));
                    order.Name = Name;
                    order.IdentityNumber = IdentityNumber;
                    order.SelectionNumber = MobileNumber;
                    order.PayStatus = 0; //添加时付款状态默认为0：未付款。
                    order.PaymentMethod = PaymentMethod;
                    order.OrderStatus = 0; //添加时订单状态默认为0：未激活。
                    order.OrderDate = Unitoys.Core.CommonHelper.GetDateTimeInt();
                    order.Quantity = 1;
                    order.UnitPrice = zcSelectionNumber.Price;
                    order.TotalPrice = order.UnitPrice * order.Quantity;

                    //不需要付款的号码则直接可用，绑定对应号码
                    if (zcSelectionNumber.Price <= 0)
                    {
                        SetSubmitAgoAndSelection(db, order, zcSelectionNumber);
                    }

                    db.UT_OrderByZCSelectionNumber.Add(order);

                    return await db.SaveChangesAsync() > 0 ? order : null;

                }
                else
                {
                    return null;
                }
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
                UT_OrderByZCSelectionNumber payOrder = await db.UT_OrderByZCSelectionNumber.Include(x => x.UT_OrderByZC).Where(a => a.ID == orderId).SingleOrDefaultAsync();

                if (payUser != null && payOrder != null)
                {
                    UT_ZCSelectionNumber zcSelectionNumber = await db.UT_ZCSelectionNumber.FirstOrDefaultAsync(x => x.MobileNumber == payOrder.SelectionNumber);

                    //判断此号码是否已被选取
                    if (zcSelectionNumber.OrderByZCSelectionNumberId != null && zcSelectionNumber.OrderByZCSelectionNumberId != payOrder.ID)
                    {
                        return -6;
                    }
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
                    else if (payOrder.UT_OrderByZC.UserId != payUser.ID)
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

                        //绑定号码
                        SetSubmitAgoAndSelection(db, payOrder, zcSelectionNumber);

                        db.UT_Users.Attach(payUser);
                        db.UT_OrderByZCSelectionNumber.Attach(payOrder);
                        db.Entry<UT_Users>(payUser).State = EntityState.Modified;
                        db.Entry<UT_OrderByZCSelectionNumber>(payOrder).State = EntityState.Modified;

                        //建立UserBill记录账单。
                        UT_UserBill userBill = new UT_UserBill();
                        userBill.UserId = payUser.ID;
                        userBill.LoginName = payUser.Tel;
                        userBill.Amount = orderTotalAmount;
                        userBill.UserAmount = payUser.Amount;
                        userBill.CreateDate = CommonHelper.GetDateTimeInt();
                        userBill.BillType = 0; //支出
                        userBill.PayType = 2; //余额支付
                        userBill.Descr = "购买众筹订单选号：" + payOrder.SelectionNumber;

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
                UT_OrderByZCSelectionNumber order = await db.UT_OrderByZCSelectionNumber.Include(x => x.UT_OrderByZC).Where(a => a.OrderByZCSelectionNumberNum == orderNum).SingleOrDefaultAsync();

                if (order != null)
                {
                    //根据order的UserId获取User。
                    UT_Users user = await db.UT_Users.FindAsync(order.UT_OrderByZC.UserId);
                    UT_ZCSelectionNumber zcSelectionNumber = await db.UT_ZCSelectionNumber.FirstOrDefaultAsync(x => x.MobileNumber == order.SelectionNumber);

                    //订单应在线支付金额
                    decimal orderOnlineAmount = order.Quantity * order.UnitPrice;

                    //如果支付的金额小于订单应付金额
                    if (payAmount < orderOnlineAmount)
                    {
                        order.PayDate = CommonHelper.GetDateTimeInt();
                        order.PayStatus = PayStatusType.AbnormalPayment;
                        order.Remark = "实际支付金额：" + payAmount;
                        db.UT_OrderByZCSelectionNumber.Attach(order);
                        db.Entry<UT_OrderByZCSelectionNumber>(order).State = EntityState.Modified;
                        if (await db.SaveChangesAsync() <= 0)
                        {
                            LoggerHelper.Error("订单错误支付更新失败", new Exception("订单错误支付更新失败"));
                        }
                        return false;
                    }

                    //判断此号码是否已被选取
                    //已被选择则退回余额
                    //TODO考虑是否推送消息
                    if (zcSelectionNumber.OrderByZCSelectionNumberId != null && zcSelectionNumber.OrderByZCSelectionNumberId != order.ID)
                    {
                        user.Amount += payAmount;
                        //建立UserBill记录账单。
                        UT_UserBill userOrderBill = new UT_UserBill();
                        userOrderBill.UserId = user.ID;
                        userOrderBill.LoginName = user.Tel;
                        userOrderBill.Amount = orderOnlineAmount;
                        userOrderBill.UserAmount = user.Amount;
                        userOrderBill.CreateDate = CommonHelper.GetDateTimeInt();
                        userOrderBill.BillType = 1; //收入
                        userOrderBill.PayType = 7; //订单退款
                        userOrderBill.Descr = "退款众筹订单选号(已被选)：" + order.SelectionNumber;

                        //取消订单
                        order.OrderStatus = OrderByZCSelectionNumberStatusType.Cancel;
                        order.Remark = "实际支付金额：" + payAmount;

                        db.UT_UserBill.Add(userOrderBill);
                        if (await db.SaveChangesAsync() <= 0)
                        {
                            LoggerHelper.Error("退款众筹订单选号(已被选)更新失败", new Exception("退款众筹订单选号(已被选)更新失败"));
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

                    //绑定号码
                    SetSubmitAgoAndSelection(db, order, zcSelectionNumber);

                    db.UT_OrderByZCSelectionNumber.Attach(order);
                    db.Entry<UT_OrderByZCSelectionNumber>(order).State = EntityState.Modified;

                    //建立UserBill记录账单。
                    UT_UserBill userBill = new UT_UserBill();
                    userBill.UserId = user.ID;
                    userBill.LoginName = user.Tel;
                    userBill.Amount = orderOnlineAmount;
                    userBill.UserAmount = user.Amount;
                    userBill.CreateDate = CommonHelper.GetDateTimeInt();
                    userBill.BillType = 0; //支出
                    userBill.PayType = 1; //在线支付
                    userBill.Descr = "购买众筹订单选号：" + order.SelectionNumber;

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
        /// 设为待提交,并且绑定对应号码
        /// </summary>
        /// <param name="db"></param>
        /// <param name="payOrder"></param>
        /// <param name="zcSelectionNumber"></param>
        private static void SetSubmitAgoAndSelection(UnitoysEntities db, UT_OrderByZCSelectionNumber payOrder, UT_ZCSelectionNumber zcSelectionNumber)
        {
            payOrder.ZCSelectionNumberId = zcSelectionNumber.ID;
            payOrder.OrderStatus = OrderByZCSelectionNumberStatusType.SubmitAgo;

            zcSelectionNumber.OrderByZCSelectionNumberId = payOrder.ID;

            db.UT_ZCSelectionNumber.Attach(zcSelectionNumber);
            db.Entry<UT_ZCSelectionNumber>(zcSelectionNumber).State = EntityState.Modified;
        }


        /// <summary>
        /// 根据条件获取订单选号和订单
        /// </summary>
        /// <returns></returns>
        public async Task<UT_OrderByZCSelectionNumber> GetEntityAndOrderByZCAsync(System.Linq.Expressions.Expression<Func<UT_OrderByZCSelectionNumber, bool>> exp)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                return await db.UT_OrderByZCSelectionNumber.Include(x => x.UT_OrderByZC).FirstOrDefaultAsync(exp);
            }
        }
    }
}
