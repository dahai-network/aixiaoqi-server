using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 后台管理员
    /// </summary>
    public class UT_ManageUsers : UT_Entity
    {

        public UT_ManageUsers()
        {
            this.UT_ManageUsersRole = new HashSet<UT_ManageUsersRole>();
            this.UT_PaymentCard = new HashSet<UT_PaymentCard>();
        }

        /// <summary>
        /// 登录名
        /// </summary>
        public string LoginName { get; set; }
        /// <summary>
        /// 登录密码
        /// </summary>
        public string PassWord { get; set; }
        /// <summary>
        /// 真实姓名
        /// </summary>
        public string TrueName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 是否锁定禁用
        /// </summary>
        public int Lock4 { get; set; }

        public virtual ICollection<UT_ManageUsersRole> UT_ManageUsersRole { get; set; }
        public virtual ICollection<UT_PaymentCard> UT_PaymentCard { get; set; }
        public virtual ICollection<UT_GiftCard> UT_GiftCard { get; set; }
    }
}
