using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 联系我们
    /// </summary>
    public class UT_ContactUS : UT_Entity
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string MailAddress { get; set; }
        /// <summary>
        /// 主题
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// 信息
        /// </summary>
        public string Content { get; set; }
        public int CreateDate { get; set; }
        /// <summary>
        /// 显示排序
        /// </summary>
        //public int DisplayOrder { get; set; }
    }
}
