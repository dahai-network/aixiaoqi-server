using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 短信验证
    /// </summary>
    public class UT_SMSConfirmation : UT_Entity
    {
        /// <summary>
        /// 电话号码
        /// </summary>
        public string Tel { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime ExpireDate { get; set; }
        /// <summary>
        /// 验证时间
        /// </summary>
        public DateTime? ConfirmDate { get; set; }
        /// <summary>
        /// 验证类型, 1:注册 2:忘记密码 3:众筹订单绑定增加号码
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 是否已经验证过
        /// </summary>
        public bool IsConfirmed { get; set; }
    }
    public enum EnumSMSConfirmationType
    {
        Register = 1,
        ForgotPassword = 2,
    }
}
