using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.WebApi.Models
{
    public class AddSportAndTimePeriodBindingModel
    {
        /// <summary>
        /// 步数
        /// </summary>
        public int StepNum { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public int StepTime { get; set; }
    }

    public class AddSportAndTimePeriodForOneHoursBindingModel
    {
        /// <summary>
        /// 日期
        /// </summary>
        public int Date { get; set; }
        /// <summary>
        /// 步数
        /// </summary>
        public int StepNum { get; set; }
        /// <summary>
        /// 开始记录时间
        /// </summary>
        public int StartStepTime { get; set; }
    }


}
