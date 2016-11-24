using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 用户登录记录
    /// </summary>
    public class UT_UserLoginRecord : UT_Entity
    {
       
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 用户登录名
        /// </summary>
        public string LoginName { get; set; }

        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime LoginDate { get; set; }

        /// <summary>
        /// 登录ip
        /// </summary>
        public string LoginIp { get; set; }

        /// <summary>
        /// 登录入口
        /// </summary>
        public string Entrance { get; set; }

        public virtual UT_Users UT_Users { get; set; }
    }
}
