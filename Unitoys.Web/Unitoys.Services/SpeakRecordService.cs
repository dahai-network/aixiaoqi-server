using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Core;
using Unitoys.IServices;
using Unitoys.Model;
using System.Data.Entity;

namespace Unitoys.Services
{
    public class SpeakRecordService : BaseService<UT_SpeakRecord>, ISpeakRecordService
    {
        private IUserService _userService;
        public SpeakRecordService(IUserService userService)
        {
            this._userService = userService;
        }
        //直拨每秒的费用
        //private const decimal CallDirectPricePerSeconds = 0.01m;
        //回拨每秒的费用
        //private const decimal CallBackPricePerSeconds = 0.02m;

        /// <summary>
        /// 添加通话记录并且扣除用户通话费用
        /// </summary>
        /// <param name="deviceName">主叫号码</param>
        /// <param name="calledTelNum">被叫号码</param>
        /// <param name="callStartTime">开始拨打时间</param>
        /// <param name="callStopTime">结束通话时间</param>
        /// <param name="callSessionTime">通话时间</param>
        /// <param name="callSourceIp">拨打源IP</param>
        /// <param name="callServerIp">服务器IP</param>
        /// <param name="acctterminatedirection">挂断方</param>
        /// <returns></returns>
        public async Task<bool> AddRecordAndDeDuction(string deviceName, string calledTelNum, DateTime callStartTime, DateTime callStopTime, int callSessionTime, string callSourceIp, string callServerIp, string acctterminatedirection)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                //1. 根据主叫号码获取用户。
                UT_Users user = await db.UT_Users.Where(x => x.Tel == deviceName).SingleOrDefaultAsync();

                if (user != null)
                {
                    //2. 判断被叫号码前缀是否为972，973则为回拨记录，若前缀为0或者1则为直拨。
                    int callType = 0;

                    if (calledTelNum.StartsWith("972") || calledTelNum.StartsWith("973"))
                    {
                        if (ValidateHelper.IsMobile(calledTelNum.Substring(3)))
                        {
                            callType = 1;
                        }
                    }
                    else if (calledTelNum.StartsWith("981"))
                    {
                        //会有座机的情况
                        //if (ValidateHelper.IsMobile(calledTelNum.Substring(1)))
                        //{
                        callType = 2;
                        //}
                    }
                    else
                    {
                        return false;
                    }

                    //3. 计算通话记录的通话费用。
                    //      扣除订单剩余套餐剩余通话时间，
                    //      通话时间不足一分钟算一分钟

                    //获取当前用户有效订单-已激活+已付款+剩余通话时间大于0
                    UT_Order order = await db.UT_Order.Where(x => x.UserId == user.ID
                            && x.OrderStatus == OrderStatusType.Used
                            && x.PayStatus == PayStatusType.YesPayment
                            && (x.PackageIsCategoryCall || x.PackageIsCategoryDualSimStandby)
                            && x.RemainingCallMinutes > 0).OrderByDescending(x => x.ActivationDate).FirstOrDefaultAsync();

                    int callMinutes = 0;
                    //用户需支付的通话分钟费用
                    int userAmountCallSessionTimeMinutes = Convert.ToInt32(Math.Ceiling(callSessionTime / 60m));
                    //订单所用通话分钟数
                    int orderUsedCallMinutes = 0;
                    if (order != null)
                    {
                        callMinutes = Convert.ToInt32(Math.Ceiling(callSessionTime / 60m));

                        // 判断订单剩余通话时间是否够扣除此次通话时间,不够的话则计算用户账户所需支付金额
                        if (order.RemainingCallMinutes >= callMinutes)
                        {
                            orderUsedCallMinutes = callMinutes;

                            order.RemainingCallMinutes = order.RemainingCallMinutes - callMinutes;

                            userAmountCallSessionTimeMinutes = 0;


                        }
                        else
                        {
                            orderUsedCallMinutes = order.RemainingCallMinutes;

                            order.RemainingCallMinutes = 0;
                            order.OrderStatus = OrderStatusType.HasExpired;

                            userAmountCallSessionTimeMinutes = callMinutes - order.RemainingCallMinutes;


                        }

                        db.UT_Order.Attach(order);
                        db.Entry<UT_Order>(order).State = EntityState.Modified;

                        //添加话费使用明细记录
                        UT_OrderUsage orderItemUsage = new UT_OrderUsage()
                        {
                            OrderId = order.ID,
                            UsedCallMinutes = orderUsedCallMinutes,
                            StartDate = callStartTime,
                            EndDate = callStopTime
                        };
                        db.UT_OrderUsage.Add(orderItemUsage);
                    }

                    decimal userTotalAmount = 0m;

                    if (userAmountCallSessionTimeMinutes > 0)
                    {
                        if (callType == 1)
                        {
                            //3.1 计算回拨的费用。
                            userTotalAmount = userAmountCallSessionTimeMinutes * UTConfig.SiteConfig.CallBackPricePerMinutes;
                        }
                        else if (callType == 2)
                        {
                            //3.2 计算直拨的费用。
                            userTotalAmount = userAmountCallSessionTimeMinutes * UTConfig.SiteConfig.CallDirectPricePerMinutes;
                        }
                    }

                    int CallAgoRemainingCallSeconds = 0;
                    calledTelNum = calledTelNum.Substring(3);
                    if (calledTelNum.IndexOf('#') > -1)
                    {
                        if (Int32.TryParse(calledTelNum.Substring(calledTelNum.IndexOf('#') + 1), out CallAgoRemainingCallSeconds))
                        {
                            calledTelNum = calledTelNum.Substring(0, calledTelNum.IndexOf('#'));
                        };
                    }

                    //不依赖与网络电话端传递过来的剩余秒数
                    CallAgoRemainingCallSeconds = await _userService.GetAmountAndOrderMaximumPhoneCallTime(user.ID);

                    //4. 添加通话记录。
                    UT_SpeakRecord speakRecord = new UT_SpeakRecord()
                    {
                        UserId = user.ID,
                        DeviceName = deviceName,
                        CalledTelNum = calledTelNum,
                        CallAgoRemainingCallSeconds = CallAgoRemainingCallSeconds,
                        CallStartTime = callStartTime,
                        CallStopTime = callStopTime,
                        CallSessionTime = callSessionTime,
                        TotalAmount = userTotalAmount,//除去订单剩余通话分钟数后，实际支付金额
                        CallSourceIp = callSourceIp,
                        CallServerIp = callServerIp,
                        Acctterminatedirection = acctterminatedirection,
                        CallType = callType,
                        Status = SpeakRecordStatus.Missing
                    };
                    db.UT_SpeakRecord.Add(speakRecord);

                    //5. 扣除用户费用。
                    if (userTotalAmount > 0)
                    {
                        user.Amount = user.Amount - userTotalAmount;

                        db.UT_Users.Attach(user);
                        db.Entry<UT_Users>(user).State = EntityState.Modified;

                        //建立UserBill记录账单。
                        UT_UserBill userBill = new UT_UserBill();
                        userBill.UserId = user.ID;
                        userBill.LoginName = user.Tel;
                        userBill.Amount = userTotalAmount;
                        userBill.UserAmount = user.Amount;
                        userBill.CreateDate = CommonHelper.GetDateTimeInt();
                        userBill.BillType = 0; //支出
                        userBill.PayType = 4; //扣话费
                        userBill.Descr = string.Format("拨打:{0}（{1}分钟）", calledTelNum, userAmountCallSessionTimeMinutes);

                        db.UT_UserBill.Add(userBill);
                    }

                    return await db.SaveChangesAsync() > 0;
                }

                return false;
            }
        }

        /// <summary>
        /// 添加漏接通话记录
        /// </summary>
        /// <param name="deviceName">主叫号码</param>
        /// <param name="calledTelNum">被叫号码</param>
        /// <param name="callStartTime">开始拨打时间</param>
        /// <param name="callStopTime">结束通话时间</param>
        /// <param name="callSessionTime">通话时间</param>
        /// <param name="callSourceIp">拨打源IP</param>
        /// <param name="callServerIp">服务器IP</param>
        /// <param name="acctterminatedirection">挂断方</param>
        /// <returns></returns>
        public async Task<bool> AddRecordMissing(string deviceName, string calledTelNum, DateTime callStartTime, DateTime callStopTime, int callSessionTime, string callSourceIp, string callServerIp, string acctterminatedirection)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                //1. 根据主叫号码获取用户。
                UT_Users user = await db.UT_Users.Where(x => x.Tel == calledTelNum).SingleOrDefaultAsync();

                if (user != null)
                {
                    //2. 判断被叫号码前缀是否为972，973则为回拨记录，若前缀为0或者1则为直拨。
                    int callType = 0;

                    //if (calledTelNum.StartsWith("972") || calledTelNum.StartsWith("973"))
                    //{
                    //    if (ValidateHelper.IsMobile(calledTelNum.Substring(3)))
                    //    {
                    //        callType = 1;
                    //    }
                    //}
                    //else if (calledTelNum.StartsWith("981"))
                    //{
                    //    //会有座机的情况
                    //    //if (ValidateHelper.IsMobile(calledTelNum.Substring(1)))
                    //    //{
                    //    callType = 2;
                    //    //}
                    //}
                    //else
                    //{
                    //    return false;
                    //}

                    //用户需支付的通话分钟费用
                    int userAmountCallSessionTimeMinutes = Convert.ToInt32(Math.Ceiling(callSessionTime / 60m));

                    decimal userTotalAmount = 0m;

                    if (userAmountCallSessionTimeMinutes > 0)
                    {
                        if (callType == 1)
                        {
                            //3.1 计算回拨的费用。
                            userTotalAmount = userAmountCallSessionTimeMinutes * UTConfig.SiteConfig.CallBackPricePerMinutes;
                        }
                        else if (callType == 2)
                        {
                            //3.2 计算直拨的费用。
                            userTotalAmount = userAmountCallSessionTimeMinutes * UTConfig.SiteConfig.CallDirectPricePerMinutes;
                        }
                    }

                    int CallAgoRemainingCallSeconds = 0;
                    //calledTelNum = calledTelNum.Substring(3);
                    //if (calledTelNum.IndexOf('#') > -1)
                    //{
                    //    if (Int32.TryParse(calledTelNum.Substring(calledTelNum.IndexOf('#') + 1), out CallAgoRemainingCallSeconds))
                    //    {
                    //        calledTelNum = calledTelNum.Substring(0, calledTelNum.IndexOf('#'));
                    //    };
                    //}

                    //不依赖与网络电话端传递过来的剩余秒数
                    CallAgoRemainingCallSeconds = await _userService.GetAmountAndOrderMaximumPhoneCallTime(user.ID);

                    //4. 添加通话记录。
                    UT_SpeakRecord speakRecord = new UT_SpeakRecord()
                    {
                        UserId = user.ID,
                        DeviceName = deviceName,
                        CalledTelNum = calledTelNum,
                        CallAgoRemainingCallSeconds = CallAgoRemainingCallSeconds,
                        CallStartTime = callStartTime,
                        CallStopTime = callStopTime,
                        CallSessionTime = callSessionTime,
                        TotalAmount = userTotalAmount,//除去订单剩余通话分钟数后，实际支付金额
                        CallSourceIp = callSourceIp,
                        CallServerIp = callServerIp,
                        Acctterminatedirection = acctterminatedirection,
                        CallType = callType,
                        Status = SpeakRecordStatus.Missing
                    };
                    db.UT_SpeakRecord.Add(speakRecord);

                    return await db.SaveChangesAsync() > 0;
                }

                return false;
            }
        }

        /// <summary>
        /// 异步搜索
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="rows">页数</param>
        /// <param name="deviceName">主叫号码</param>
        /// <param name="calledTelNum">被叫号码</param>
        /// <param name="callStartBeginTime">开始拨打时间</param>
        /// <param name="callStartEndTime">结束拨打时间</param>
        /// <param name="CallSessionBeginTime">开始拨打时长范围</param>
        /// <param name="CallSessionEndTime">结束拨打时长范围</param>
        /// <param name="isCallConnected">接通情况(是否接通，已接通、未接通）</param>
        /// <returns></returns>
        public async Task<KeyValuePair<int, List<UT_SpeakRecord>>> SearchAsync(int page, int rows, string deviceName, string calledTelNum, DateTime? callStartBeginTime, DateTime? callStartEndTime, int? CallSessionBeginTime, int? CallSessionEndTime, int? isCallConnected)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = db.UT_SpeakRecord.Where(x => true);

                if (!string.IsNullOrEmpty(deviceName))
                {
                    query = query.Where(x => x.DeviceName.Contains(deviceName));
                }
                if (!string.IsNullOrEmpty(calledTelNum))
                {
                    query = query.Where(x => x.CalledTelNum.Contains(calledTelNum));
                }
                if (callStartBeginTime.HasValue)
                {
                    query = query.Where(x => x.CallStartTime >= callStartBeginTime);
                }
                if (callStartEndTime.HasValue)
                {
                    query = query.Where(x => x.CallStartTime <= callStartEndTime);
                }
                if (CallSessionBeginTime.HasValue)
                {
                    query = query.Where(x => x.CallSessionTime >= CallSessionBeginTime);
                }
                if (CallSessionEndTime.HasValue)
                {
                    query = query.Where(x => x.CallSessionTime <= CallSessionEndTime);
                }

                //会话状态，已接通/未接通
                if (isCallConnected.HasValue)
                {
                    if (isCallConnected == 1)
                    {
                        query = query.Where(x => x.CallSessionTime > 0);
                    }
                    else
                    {
                        query = query.Where(x => x.CallSessionTime == 0);
                    }
                }

                var result = await query.OrderByDescending(x => x.CallStartTime).Skip((page - 1) * rows).Take(rows).ToListAsync();

                var count = await query.CountAsync();

                return new KeyValuePair<int, List<UT_SpeakRecord>>(count, result);
            }
        }
    }
}
