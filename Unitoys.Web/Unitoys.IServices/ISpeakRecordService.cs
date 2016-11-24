using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Model;

namespace Unitoys.IServices
{
    public interface ISpeakRecordService : IBaseService<UT_SpeakRecord>
    {
        /// <summary>
        /// 添加通话记录并且扣除用户通话费用
        /// </summary>
        /// <param name="deviceName">主叫号码</param>
        /// <param name="calledTelNum">被叫号码</param>
        /// <param name="callStartTime">开始拨打时间</param>
        /// <param name="callStopTime">结束通话时间</param>
        /// <param name="callSessionTime">通话时间</param>
        /// <param name="callSourceIp">拨打源IP</param>
        /// <param name="callServerIp">服务器IP</param>
        /// <param name="acctterminatedirection">挂断方</param>
        /// <returns></returns>
        Task<bool> AddRecordAndDeDuction(string deviceName, string calledTelNum, DateTime callStartTime, DateTime callStopTime, int callSessionTime, string callSourceIp, string callServerIp, string acctterminatedirection);

        /// <summary>
        /// 异步搜索
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="rows">页数</param>
        /// <param name="deviceName">主叫号码</param>
        /// <param name="calledTelNum">被叫号码</param>
        /// <param name="callStartBeginTime">开始拨打时间</param>
        /// <param name="callStartEndTime">结束拨打时间</param>
        /// <param name="CallSessionBeginTime">开始拨打时长范围</param>
        /// <param name="CallSessionEndTime">结束拨打时长范围</param>
        /// <param name="isCallConnected">接通情况(是否接通，已接通、未接通）</param>
        /// <returns></returns>
        Task<KeyValuePair<int, List<UT_SpeakRecord>>> SearchAsync(int page, int rows, string deviceName, string calledTelNum, DateTime? callStartBeginTime, DateTime? callStartEndTime, int? CallSessionBeginTime, int? CallSessionEndTime, int? isCallConnected);
    }
}
