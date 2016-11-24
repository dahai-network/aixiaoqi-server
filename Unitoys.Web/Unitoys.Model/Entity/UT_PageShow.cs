using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 页面展示管理
    /// </summary>
    public class UT_PageShow : UT_Entity
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 友好地址名
        /// </summary>
        public string EntryName { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public int CreateDate { get; set; }
    }
}
