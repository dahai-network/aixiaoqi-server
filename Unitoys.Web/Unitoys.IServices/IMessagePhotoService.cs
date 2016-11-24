using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Model;

namespace Unitoys.IServices
{
    public interface IMessagePhotoService : IBaseService<UT_MessagePhoto>
    {
        /// <summary>
        /// 获取动态消息的所有图片Path
        /// </summary>
        /// <param name="messageId">动态消息ID</param>
        /// <returns></returns>
        Task<List<string>> GetMessagePhotoPath(Guid messageId);
    }
}
