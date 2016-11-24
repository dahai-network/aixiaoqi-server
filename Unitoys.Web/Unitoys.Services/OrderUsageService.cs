using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.IServices;
using Unitoys.Model;
using System.Data.Entity;

namespace Unitoys.Services
{
    public class OrderUsageService : BaseService<UT_OrderUsage>, IOrderUsageService
    {
        /// <summary>
        /// 获取当月流量使用明细
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public async Task<List<KeyValuePair<string, object>>> GetOrderItemUsageRecordForMonth(Guid userId, int page, int row, int month)
        {
            using(UnitoysEntities db = new UnitoysEntities())
            {
                //1. 首先获取用户已经成功付款的订单。
                var alreadyPayOrders = await db.UT_Order.Where(x => x.UserId == userId && x.PayDate != null && x.PayStatus == PayStatusType.YesPayment).ToListAsync();

                if (alreadyPayOrders.Count > 0)
                {
                    List<KeyValuePair<string, object>> resultList = new List<KeyValuePair<string, object>>();

                    DateTime now = DateTime.Now;
                    //月份的1号
                    DateTime queryStartDate = new DateTime(now.Year, month == 0 ? now.Month : month, 1);
                    //下个月份的1号
                    DateTime queryEndDate = new DateTime(now.Year, month == 0 ? now.Month + 1 : month + 1, 1);

                    //2. 获取用户购买的套餐流量明细。
                    foreach (var order in alreadyPayOrders)
                    {
                        var currentMonthUsage = order.UT_OrderUsage.Where(x => x.StartDate >= queryStartDate && x.EndDate < queryEndDate);

                        foreach (var orderUsage in currentMonthUsage)
                        {
                            //3. 对OrderItemUsage进行字段过滤。
                            var data = new
                            {
                                UsedFlow = orderUsage.UsedFlow,
                                StartDate = orderUsage.StartDate,
                                EndDate = orderUsage.EndDate
                            };

                            resultList.Add(new KeyValuePair<string, object>(order.PackageName, data));
                        }                        
                    }

                    if(resultList.Count > 0)
                    {
                        resultList = resultList.Skip((page - 1) * row).Take(row).ToList();
                    }
                    return resultList;
                }
                return null;
            }
        }

        /// <summary>
        /// 添加流量使用记录
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="usedFlow">已使用流量</param>
        /// <param name="startDate">开始使用时间</param>
        /// <param name="endDate">结束使用时间</param>
        /// <returns></returns>
        public async Task<bool> AddOrderItemUsageRecordAsync(Guid userId, int usedFlow, DateTime startDate, DateTime endDate)
        {
            using(UnitoysEntities db = new UnitoysEntities())
            {
                //1. 首先获取用户已经成功付款的订单并正在使用中的套餐。
                var alreadyPayOrdersAndIsActive = await db.UT_Order.Where(x => x.UserId == userId && x.PayDate != null && x.PayStatus == PayStatusType.YesPayment && x.OrderStatus == OrderStatusType.Used).SingleOrDefaultAsync();
                
                if(alreadyPayOrdersAndIsActive != null)
                {
                    //2. 添加流量使用明细记录。
                    UT_OrderUsage orderItemUsage = new UT_OrderUsage()
                    {
                        OrderId = alreadyPayOrdersAndIsActive.ID,
                        UsedFlow = usedFlow,
                        StartDate = startDate,
                        EndDate = endDate
                    };
                    db.UT_OrderUsage.Add(orderItemUsage);

                    //3. 判断使用流量是否超出套餐的流量，如果超出则改变Order的状态。
                    int totalUsedFlow = alreadyPayOrdersAndIsActive.UT_OrderUsage.Sum(x => x.UsedFlow);

                    if (totalUsedFlow > alreadyPayOrdersAndIsActive.Flow)
                    {
                        //将订单的OrderStatus赋值为已用完状态。
                        alreadyPayOrdersAndIsActive.OrderStatus = OrderStatusType.HasExpired;

                        db.UT_Order.Attach(alreadyPayOrdersAndIsActive);
                        db.Entry<UT_Order>(alreadyPayOrdersAndIsActive).State = System.Data.Entity.EntityState.Modified;
                    }

                    return await db.SaveChangesAsync() > 0;
                }   
                return false;
            }
        }
    }
}
