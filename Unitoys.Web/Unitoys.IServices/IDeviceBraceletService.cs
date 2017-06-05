using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Model;

namespace Unitoys.IServices
{
    public interface IDeviceBraceletService : IBaseService<UT_DeviceBracelet>
    {
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="rows">页数</param>
        /// <param name="iMEI">设备号</param>
        /// <param name="tel">用户手机号</param>
        /// <param name="createStartDate">创建开始时间</param>
        /// <param name="createEndDate">创建结束时间</param>
        /// <returns></returns>
        Task<KeyValuePair<int, List<UT_DeviceBracelet>>> SearchAsync(int page, int rows, string iMEI, string tel, int? createStartDate, int? createEndDate);

        /// <summary>
        /// 检查设备号是否已经存在
        /// </summary>
        /// <param name="UserId">用户</param>
        /// <returns>true：存在，false：不存在</returns>
        Task<bool> CheckUserIdExist(Guid UserId);

        /// <summary>
        /// 检查此设备是否被其他用户绑定
        /// </summary>
        /// <param name="UserId">用户</param>
        /// <param name="IMEI">设备名</param>
        /// <returns></returns>
        Task<bool> CheckIMEIByNotUserExist(Guid UserId, string IMEI);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="IMEIs"></param>
        /// <returns></returns>
        Task<List<string>> GetBindsIMEI(string[] IMEIs);
    }
}
