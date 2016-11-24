using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.WebApi.Models
{
    public class ConfigResponseModel
    {
        /// <summary>
        /// 名称（英文）
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述（中文描述）
        /// </summary>
        //public string Descr { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }
    }

}
