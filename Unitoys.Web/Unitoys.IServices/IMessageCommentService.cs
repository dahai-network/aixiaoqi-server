using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Model;

namespace Unitoys.IServices
{
    public interface IMessageCommentService : IBaseService<UT_MessageComment>
    {
        /// <summary>
        /// 发表评论
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="messageId">用户动态消息ID</param>
        /// <param name="content">评论内容</param>
        /// <returns></returns>
        Task<UT_MessageComment> AddMessageComment(Guid userId, Guid messageId, string content);
    }
}
