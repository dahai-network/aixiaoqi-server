using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Model;

namespace Unitoys.IServices
{
    public interface IOrderUsageService : IBaseService<UT_OrderUsage>
    {
        /// <summary>
        /// 获取当月流量使用明细
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        Task<List<KeyValuePair<string, object>>> GetOrderItemUsageRecordForMonth(Guid userId, int page, int row, int month);
        /// <summary>
        /// 添加流量使用记录
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="usedFlow">已使用流量</param>
        /// <param name="startDate">开始使用时间</param>
        /// <param name="endDate">结束使用时间</param>
        /// <returns></returns>
        Task<bool> AddOrderItemUsageRecordAsync(Guid userId, int usedFlow, DateTime startDate, DateTime endDate);
    }
}
