using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Model;

namespace Unitoys.IServices
{
    public interface ISportService : IBaseService<UT_Sport>
    {
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="rows">页数</param>
        /// <param name="tel">用户手机号</param>
        /// <param name="createStartDate">创建开始时间</param>
        /// <param name="createEndDate">创建结束时间</param>
        /// <returns></returns>
        Task<KeyValuePair<int, List<UT_Sport>>> SearchAsync(int page, int rows, string tel, int? createStartDate, int? createEndDate);
        /// <summary>
        /// 新增总运动数和时间段运动数
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="stepNum">步数</param>
        /// <param name="stepTime">时间</param>
        /// <returns></returns>
        Task<bool> AddSportAndTimePeriodAsync(Guid userId, int stepNum, int stepTime);

        /// <summary>
        /// 添加运动历史记录(内部进行去重与相关处理)
        /// </summary>
        /// <param name="userId">用户</param>
        /// <param name="ToDays">今天</param>
        /// <param name="YesterDays">昨天</param>
        /// <param name="BeforeYesterDays">前天</param>
        /// <param name="HistoryDays">历史</param>
        /// <returns></returns>
        Task<bool> AddSportHistoryAsync(Guid userId, List<int> ToDays, List<int> YesterDays, List<int> BeforeYesterDays, List<int> HistoryDays);
    }
}
