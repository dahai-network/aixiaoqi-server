using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 用户设备号码
    /// </summary>
    public class UT_UserDeviceTel : UT_Entity
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid? UserId { get; set; }
        public string Tel { get; set; }
        public string ICCID { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public int CreateDate { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public int UpdateDate { get; set; }
        public virtual UT_Users UT_Users { get; set; }
        /// <summary>
        /// 是否已经验证过
        /// </summary>
        public bool IsConfirmed { get; set; }
        public DateTime? ConfirmDate { get; set; }
        /// <summary>
        /// 乐观并发
        /// </summary>
        [Timestamp]
        public Byte[] RowVersion { get; set; }
    }
}
