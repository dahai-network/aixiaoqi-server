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
    public class MessagePhotoService : BaseService<UT_MessagePhoto>, IMessagePhotoService
    {
        /// <summary>
        /// 获取动态消息的所有图片Path
        /// </summary>
        /// <param name="messageId">动态消息ID</param>
        /// <returns></returns>
        public async Task<List<string>> GetMessagePhotoPath(Guid messageId)
        {
            using(UnitoysEntities db = new UnitoysEntities())
            {
                return await db.UT_MessagePhoto.Where(x => x.MessageId == messageId).Select(x => x.Path).ToListAsync();
            }
        }
    }
}
