using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Model;

namespace Unitoys.IServices
{
    public interface IUserDeviceTelService : IBaseService<UT_UserDeviceTel>
    {
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="rows">页数</param>
        /// <param name="tel">用户手机号</param>
        /// <param name="createStartDate">创建开始时间</param>
        /// <param name="createEndDate">创建结束时间</param>
        /// <returns></returns>
        Task<KeyValuePair<int, List<UT_UserDeviceTel>>> SearchAsync(int page, int rows, string tel, int? createStartDate, int? createEndDate);

        Task<UT_UserDeviceTel> GetFirst(Guid userId);
        Task<KeyValuePair<bool, string>> CheckConfirmed(Guid userId, string ICCID);
        /// <summary>
        /// 验证设备内号码
        /// </summary>
        /// <param name="userId">用户</param>
        /// <param name="tel">设备内号码</param>
        /// <param name="ICCID">ICCID</param>
        /// <param name="code">验证码</param>
        /// <returns>
        /// key：0失败/1成功/2无此验证码/3验证码过期/4无此验证手机号
        /// value：设备内号码
        /// </returns>
        Task<KeyValuePair<int, string>> Confirmed(Guid userId, string ICCID, string code);
    }
}
