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
    }
}
