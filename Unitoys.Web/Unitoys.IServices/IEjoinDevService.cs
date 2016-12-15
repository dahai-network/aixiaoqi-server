using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Model;

namespace Unitoys.IServices
{
    /// <summary>
    /// Ejoin设备管理
    /// </summary>
    public interface IEjoinDevService : IBaseService<UT_EjoinDev>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="name">设备名，注册名</param>
        /// <param name="maxPort">最大端口数</param>
        /// <param name="regIp">设备注册IP</param>
        /// <param name="regStatus">注册状态</param>
        /// <param name="modType">设备类型</param>
        /// <returns></returns>
        Task<KeyValuePair<int, List<UT_EjoinDev>>> SearchAsync(int page, int rows, string name, int? maxPort, string regIp, RegStatusType? regStatus, ModType? modType);
    }
}
