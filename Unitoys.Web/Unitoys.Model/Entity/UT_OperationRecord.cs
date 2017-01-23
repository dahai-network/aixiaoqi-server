using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 用户充值卡
    /// </summary>
    public class UT_OperationRecord : UT_Entity
    {
        /// <summary>
        /// 操作路径
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 操作
        /// </summary>
        public string Parameter { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public string Data { get; set; }
        /// <summary>
        /// 返回
        /// </summary>
        public string Response { get; set; }
        /// <summary>
        /// 创建管理
        /// </summary>
        public Guid ManageUserId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public int CreateDate { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        public virtual UT_ManageUsers UT_ManageUsers { get; set; }
    }
}
