using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Model;

namespace Unitoys.IServices
{
    public interface IGiftCardService : IBaseService<UT_GiftCard>
    {
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="rows">页数</param>
        /// <param name="cardNum">卡号</param>
        /// <param name="cardPwd">卡密</param>
        /// <param name="createStartDate">创建开始时间</param>
        /// <param name="createEndDate">创建结束时间</param>
        /// <param name="status">状态</param>
        /// <param name="tel">用户</param>
        /// <returns></returns>
        Task<KeyValuePair<int, List<UT_GiftCard>>> SearchAsync(int page, int rows, string cardNum, string cardPwd, DateTime? createStartDate, DateTime? createEndDate, GiftCardStatusType? status, string tel);

        /// <summary>
        /// 新增生成卡
        /// </summary>
        /// <param name="ManageUserId"></param>
        /// <param name="Qty"></param>
        /// <param name="Price"></param>
        /// <returns></returns>
        Task<bool> AddGenerateCard(Guid ManageUserId, int Qty);

        /// <summary>
        /// 批量异步插入Entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> InsertAsync(List<UT_GiftCard> entityList);

        /// <summary>
        /// 绑定
        /// </summary>
        /// <param name="userId">绑定用户</param>
        /// <param name="cardPwd">卡密</param>
        /// <returns>0失败/1成功/2状态不等于未使用/3已超过最晚有效时间</returns>
        Task<int> Bind(Guid userId, string cardPwd, UT_GiftCard model);
    }
}
