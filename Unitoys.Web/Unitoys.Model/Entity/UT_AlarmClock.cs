using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 终端手环设备管理
    /// </summary>
    public class UT_AlarmClock : UT_Entity
    {

        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 时间范围
        /// 上午/下午
        /// </summary>
        public AlarmClockTimeRange TimeRange { get; set; }
        /// <summary>
        /// 闹钟时间
        /// </summary>
        public string Time { get; set; }
        /// <summary>
        /// 重复  
        /// Sun,Mon,Tues,Wed,Thur,Fri,Sat
        /// </summary>
        public string Repeat { get; set; }
        /// <summary>
        /// 标签
        /// </summary>
        public string Tag { get; set; }
        /// <summary>
        /// 状态
        /// 禁用/启用
        /// </summary>
        public AlarmClockStatus Status { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public int CreateDate { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public int UpdateDate { get; set; }

        public virtual UT_Users UT_Users { get; set; }
    }
    public enum AlarmClockStatus
    {
        /// <summary>
        /// 禁用
        /// </summary>
        Disabled = 0,
        /// <summary>
        /// 启用
        /// </summary>
        Enable = 1
    }
    public enum AlarmClockTimeRange
    {
        /// <summary>
        /// 上午
        /// </summary>
        AM = 0,
        /// <summary>
        /// 下午
        /// </summary>
        PM = 1
    }
}
