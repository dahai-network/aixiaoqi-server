using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Model;

namespace Unitoys.IServices
{
    /// <summary>
    /// Ejoin设备端口管理
    /// </summary>
    public interface IEjoinDevSlotService : IBaseService<UT_EjoinDevSlot>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="EjoinDevId">一正设备</param>
        /// <returns></returns>
        Task<KeyValuePair<int, List<UT_EjoinDevSlot>>> SearchAsync(int page, int rows, Guid? EjoinDevId);

        /// <summary>
        /// 获取使用中的端口和端口中的用户信息
        /// </summary>
        /// <param name="DevName">设备名</param>
        /// <param name="Port">端口</param>
        /// <returns></returns>
        Task<UT_EjoinDevSlot> GetUsedEntityAndUserAsync(string DevName, int Port);

        /// <summary>
        /// 获取使用的端口和设备
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<UT_EjoinDevSlot> GetUsedAAndEjoinDevsync(Guid userId);
    }
}
