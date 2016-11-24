using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Model;

namespace Unitoys.IServices
{
    public interface ICallTransferNumService : IBaseService<UT_CallTransferNum>
    {
        /// <summary>
        /// 获取可用的大号
        /// </summary>
        /// <returns></returns>
        Task<UT_CallTransferNum> GetUsableModel(Guid ID);

        /// <summary>
        /// 取消用户空闲大号资源
        /// </summary>
        /// <returns></returns>
        Task<bool> ModifyToUsable(Guid ID);

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="rows">页数</param>
        /// <param name="telNum">大号</param>
        /// <param name="createStartDate">创建开始时间</param>
        /// <param name="createEndDate">创建结束时间</param>
        /// <param name="status">状态</param>
        /// <returns></returns>
        Task<KeyValuePair<int, List<UT_CallTransferNum>>> SearchAsync(int page, int rows, string telNum, DateTime? createStartDate, DateTime? createEndDate, StatusType? status);


        /// <summary>
        /// 批量异步插入Entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> InsertAsync(List<UT_CallTransferNum> entityList);

        /// <summary>
        /// 获取正在使用的大号
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        Task<UT_CallTransferNum> GetDisabledModel(Guid userID);
    }
}
