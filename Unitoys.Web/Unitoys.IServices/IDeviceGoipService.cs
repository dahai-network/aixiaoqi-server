using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Model;

namespace Unitoys.IServices
{
    /// <summary>
    /// goip端口管理
    /// </summary>
    public interface IDeviceGoipService : IBaseService<UT_DeviceGoip>
    {
        /// <summary>
        /// 分配一个空闲的Goip端口
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        Task<UT_DeviceGoip> GetNotUsedPortAsync(Guid UserId);
        /// <summary>
        /// 取消一个正在使用的Goip端口
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns>0失败/1成功/2不存在正在使用的端口</returns>
        Task<int> CancelUsedPortAsync(Guid UserId);
        /// <summary>
        /// 设备使用的设备
        /// </summary>
        /// <param name="UserId">用户</param>
        /// <param name="IccId">卡标识</param>
        /// <returns></returns>
        Task<bool> SetUsedAsync(Guid UserId, string IccId);
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="rows">页数</param>
        /// <param name="tel">用户手机号</param>
        /// <param name="createStartDate">创建开始时间</param>
        /// <param name="createEndDate">创建结束时间</param>
        /// <returns></returns>
        Task<KeyValuePair<int, List<UT_DeviceGoip>>> SearchAsync(int page, int rows, string tel, int? createStartDate, int? createEndDate, DeviceGoipStatus? status);
        /// <summary>
        /// 获取使用中的Goip和Goip中的用户信息
        /// </summary>
        /// <returns></returns>
        Task<List<UT_DeviceGoip>> GetUsedEntitysAndUserByIdAsync();

        /// <summary>
        /// 获取使用中的Goip和Goip中的用户信息
        /// </summary>
        /// <param name="IccId"></param>
        /// <returns></returns>
        Task<UT_DeviceGoip> GetUsedEntityAndUserAsync(string IccId);
        /// <summary>
        /// 验证用户Goip并返回数据
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<UT_DeviceGoip> CheckUserGoipAsync(Guid userId);
        /// <summary>
        /// 判断该IccId是否已存在其他用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<bool> CheckIccIdExistsByNotUserAsync(Guid userId, string IccId);
    }
}
