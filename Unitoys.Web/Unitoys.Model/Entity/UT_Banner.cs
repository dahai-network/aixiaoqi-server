using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// Banner图管理
    /// </summary>
    public class UT_Banner : UT_Entity
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
        /// <summary>
        /// 显示顺序
        /// </summary>
        public int DisplayOrder { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public int CreateDate { get; set; }
    }
}
