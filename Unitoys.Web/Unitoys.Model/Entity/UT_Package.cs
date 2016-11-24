using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 套餐表
    /// </summary>
    public class UT_Package : UT_Entity
    {
        public UT_Package()
        {
            this.UT_Order = new HashSet<UT_Order>();
        }
        /// <summary>
        /// 套餐名称
        /// </summary>
        public string PackageName { get; set; }
        /// <summary>
        /// 套餐编号
        /// 保存与ESim对应的产品编号
        /// </summary>
        public string PackageNum { get; set; }
        /// <summary>
        /// 套餐价格
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 套餐包含的总流量
        /// </summary>
        public int Flow { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Desction { get; set; }
        /// <summary>
        /// 特色
        /// </summary>
        public string Features { get; set; }
        /// <summary>
        /// 详情
        /// </summary>
        public string Details { get; set; }
        /// <summary>
        /// 图片
        /// </summary>
        public string Pic { get; set; }
        /// <summary>
        /// 有效天数
        /// </summary>
        public int ExpireDays { get; set; }
        /// <summary>
        /// 所属国家
        /// </summary>
        public Guid CountryId { get; set; }
        /// <summary>
        /// 运营商
        /// </summary>
        public string Operators { get; set; }
        /// <summary>
        /// 是否锁定 0:正常  1：锁定下架
        /// </summary>
        public int Lock4 { get; set; }
        /// <summary>
        /// 是否逻辑删除
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// 通话分钟数
        /// </summary>
        public int CallMinutes { get; set; }
        /// <summary>
        /// 显示顺序
        /// </summary>
        public int DisplayOrder { get; set; }
        public virtual ICollection<UT_Order> UT_Order { get; set; }
        public virtual UT_Country UT_Country { get; set; }
    }
}
