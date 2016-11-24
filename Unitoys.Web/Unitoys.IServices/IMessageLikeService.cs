using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Model;

namespace Unitoys.IServices
{
    public interface IMessageLikeService : IBaseService<UT_MessageLike>
    {
        /// <summary>
        /// 点赞或者取消赞
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="messageId">动态消息ID</param>
        /// <returns>返回最新的动态消息点赞总数</returns>
        Task<int> LikeOrUnlike(Guid userId, Guid messageId);
    }
}
