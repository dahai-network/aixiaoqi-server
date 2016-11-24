using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.WebApi.Models
{
    public class BannerBindingModels
    {
        /// <summary>
        /// 链接地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 图片
        /// </summary>
        public string Image { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
    }
}
