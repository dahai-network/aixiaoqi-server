using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Model;

namespace Unitoys.WebApi.Models
{
    public class AddAlarmClockBindingModel
    {
        public Guid ID { get; set; }
        /// <summary>
        /// 时间范围
        /// 上午/下午
        /// </summary>
        public AlarmClockTimeRange? TimeRange { get; set; }
        public string Time { get; set; }
        public string Repeat { get; set; }
        /// <summary>
        /// 标签
        /// </summary>
        public string Tag { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public AlarmClockStatus? Status { get; set; }
    }
}
