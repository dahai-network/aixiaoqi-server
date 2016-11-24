using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.WebApi.Models
{
    public class AddFeedbackBindingModel
    {
        public string Version { get; set; }
        public string Model { get; set; }
        public string Info { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Mail { get; set; }
        /// <summary>
        /// 终端
        /// </summary>
        public string Terminal { get; set; }
    }
}
