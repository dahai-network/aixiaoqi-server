using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Model;

namespace Unitoys.IServices
{
    public interface ISMSService : IBaseService<UT_SMS>
    {
        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="ID">发送者ID</param>
        /// <param name="ToNum">接收者号码</param>
        /// <returns></returns>
        bool SendSMS(Guid ID, string ToNum);

        /// <summary>
        /// 批量异步插入
        /// </summary>
        /// <param name="entityList"></param>
        /// <returns></returns>
        Task<bool> InsertAsync(List<UT_SMS> entityList);

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="row">页数</param>
        /// <param name="tel">用户手机号</param>
        /// <param name="to">接收号码</param>
        /// <param name="beginSMSTime">时间开始</param>
        /// <param name="endSMSTime">时间结束</param>
        /// <param name="smsStatus">短信状态</param>
        /// <returns></returns>
        Task<KeyValuePair<int, List<UT_SMS>>> SearchAsync(int page, int row, string tel, string to, int? beginSMSTime, int? endSMSTime, SMSStatusType? smsStatus);

        /// <summary>
        /// 获取用户联系手机号的最后一条往来信息
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="row">页数</param>
        /// <param name="UserId">用户ID</param>
        /// <param name="Tel">手机号</param>
        /// <returns>多个联系手机号的最后一条往来信息</returns>
        Task<IEnumerable<UT_SMS>> GetLastSMSByUserContactTelAsync(int page, int row, Guid UserId, string Tel);

        /// <summary>
        /// 根据用户和来往手机号获取信息
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="row">页数</param>
        /// <param name="UserId">用户ID</param>
        /// <param name="Tel">用户手机号</param>
        /// <param name="ContactTel">联系手机号</param>
        /// <returns>用户和来往手机号短信</returns>
        Task<IEnumerable<UT_SMS>> GetByUserAndTelAsync(int page, int row, Guid UserId, string Tel, string ContactTel);

        /// <summary>
        /// 根据用户获取最大的收件时间
        /// </summary>
        /// <param name="UserId">用户ID</param>
        /// <returns>最大的收件时间</returns>
        Task<int> GetMaxNotSendTimeByUserAsync(Guid UserId);

        /// <summary>
        /// 批量删除多条
        /// </summary>
        /// <param name="userId">用户</param>
        /// <param name="ids">多个id</param>
        /// <returns></returns>
        Task<bool> DeletesAsync(Guid userId, Guid[] ids);

        /// <summary>
        /// 批量删除联系人短信
        /// </summary>
        /// <param name="UserId">用户</param>
        /// <param name="Tel">用户手机号码</param>
        /// <param name="ContactTel">联系人电话</param>
        /// <returns></returns>
        Task<bool> DeletesByTelAsync(Guid UserId, string Tel, string ContactTel);
    }
}
