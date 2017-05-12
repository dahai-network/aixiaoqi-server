using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Model;

namespace Unitoys.IServices
{
    public interface IPushContentService : IBaseService<UT_PushContent>
    {
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="rows">页数</param>
        /// <param name="title">标题</param>
        /// <param name="url">链接地址</param>
        /// <param name="createStartDate">创建开始时间</param>
        /// <param name="createEndDate">创建结束时间</param>
        /// <returns></returns>
        Task<KeyValuePair<int, List<UT_PushContent>>> SearchAsync(int page, int rows, string title, int? createStartDate, int? createEndDate);
    }
}
