using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Model;

namespace Unitoys.IServices
{
    public interface IOperationRecordService : IBaseService<UT_OperationRecord>
    {
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="rows">页数</param>
        /// <param name="url">路径</param>
        /// <param name="parameter">参数</param>
        /// <param name="managerLoginName">管理名称</param>
        /// <param name="createStartDate">创建开始时间</param>
        /// <param name="createEndDate">创建结束时间</param>
        /// <param name="status">状态</param>
        /// <returns></returns>
        Task<KeyValuePair<int, List<UT_OperationRecord>>> SearchAsync(int page, int rows, string url, string parameter, string managerLoginName, int? createStartDate, int? createEndDate);
    }
}
