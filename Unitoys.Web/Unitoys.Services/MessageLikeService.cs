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
    public class MessageLikeService : BaseService<UT_MessageLike>, IMessageLikeService
    {
        /// <summary>
        /// 点赞或者取消赞
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="messageId">动态消息ID</param>
        /// <returns>返回最新的动态消息点赞总数</returns>
        public async Task<int> LikeOrUnlike(Guid userId, Guid messageId)
        {
            using(UnitoysEntities db = new UnitoysEntities())
            {
                //1. 首先判断当前用户是否已经点赞了此消息，如果没有，则新增一条记录，反则删除。
                var messageLike = await db.UT_MessageLike.Where(x => x.UserId == userId && x.MessageId == messageId).SingleOrDefaultAsync();

                //2. 如果已经赞了，再按则为取消赞，这里做删除操作。
                if(messageLike != null)
                {
                    db.UT_MessageLike.Attach(messageLike);
                    db.Entry<UT_MessageLike>(messageLike).State = System.Data.Entity.EntityState.Deleted;
                }
                else
                {
                    UT_MessageLike newLike = new UT_MessageLike()
                    {
                        UserId = userId,
                        MessageId = messageId,
                        CreateDate = DateTime.Now
                    };
                    db.UT_MessageLike.Add(newLike);
                }

                await db.SaveChangesAsync();

                //3. 获取最新的动态消息点赞数并返回。
                var likeCount = await db.UT_MessageLike.CountAsync(x => x.MessageId == messageId);

                return likeCount;
            }
        }
    }
}
