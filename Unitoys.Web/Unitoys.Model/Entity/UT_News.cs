using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 新闻
    /// </summary>
    public class UT_News : UT_Entity
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 图片
        /// </summary>
        public string Image { get; set; }
        /// <summary>
        /// 新闻时间
        /// </summary>
        public DateTime NewsDate { get; set; }
        /// <summary>
        /// 发布人
        /// </summary>
        public string Publisher { get; set; }
        /// <summary>
        /// 新闻详情内容（图文）
        /// </summary>
        public string Content { get; set; }
        public NewsType NewsType { get; set; }
        public int CreateDate { get; set; }
        /// <summary>
        /// 显示排序
        /// </summary>
        //public int DisplayOrder { get; set; }
    }

    /// <summary>
    /// 新闻类型
    /// </summary>
    public enum NewsType
    {
        /// <summary>
        /// 行业
        /// </summary>
        Industry,
        /// <summary>
        /// 公司
        /// </summary>
        Company
    }
}
