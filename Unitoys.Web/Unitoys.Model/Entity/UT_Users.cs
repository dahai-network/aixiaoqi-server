using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 用户
    /// </summary>
    public class UT_Users : UT_Entity
    {
        public UT_Users()
        {
            this.UT_Payment = new HashSet<UT_Payment>();
            this.UT_SpeakRecord = new HashSet<UT_SpeakRecord>();
            this.UT_UserLoginInfo = new HashSet<UT_UserLoginRecord>();
            this.UT_SMS = new HashSet<UT_SMS>();
            this.UT_Order = new HashSet<UT_Order>();
            this.UT_DeviceBracelet = new HashSet<UT_DeviceBracelet>();
            this.UT_Message = new HashSet<UT_Message>();
            this.UT_MessageComment = new HashSet<UT_MessageComment>();
            this.UT_MessageLike = new HashSet<UT_MessageLike>();
            this.UT_Feedback = new HashSet<UT_Feedback>();
            this.UT_PhoneCallback = new HashSet<UT_PhoneCallback>();
            this.UT_CallTransferNum = new HashSet<UT_CallTransferNum>();
            this.UT_PaymentCard = new HashSet<UT_PaymentCard>();
            this.UT_UserShape = new HashSet<UT_UserShape>();
            this.UT_Sport = new HashSet<UT_Sport>();
            this.UT_SportTimePeriod = new HashSet<UT_SportTimePeriod>();
            //this.UT_UsersWx = new UT_UsersWx();
            this.UT_UsersConfig = new HashSet<UT_UsersConfig>();
            this.UT_OrderByZCConfirmation = new HashSet<UT_OrderByZCConfirmation>();
            this.UT_OrderByZCSelectionNumber = new HashSet<UT_OrderByZCSelectionNumber>();
            
        }
        /// <summary>
        /// 手机号码（需要短信验证）
        /// </summary>
        public string Tel { get; set; }
        /// <summary>
        /// 电子邮箱（需邮箱验证）
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 登录密码
        /// </summary>
        public string PassWord { get; set; }
        /// <summary>
        /// 账户余额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 用户头像
        /// </summary>
        public string UserHead { get; set; }
        /// <summary>
        /// 真实姓名
        /// </summary>
        public string TrueName { get; set; }

        /// <summary>
        /// QQ
        /// </summary>
        public string QQ { get; set; }
        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public int Sex { get; set; }
        /// <summary>
        /// 年龄
        /// </summary>
        public int Age { get; set; }
        /// <summary>
        /// 积分
        /// </summary>
        public int Score { get; set; }
        /// <summary>
        /// 所属组
        /// </summary>
        public Guid GroupId { get; set; }
        /// <summary>
        /// 状态，0：正常，1：锁定
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 出生日期
        /// </summary>
        public int Birthday { get; set; }
        /// <summary>
        /// 乐观并发
        /// </summary>
        [Timestamp]
        public Byte[] RowVersion { get; set; }
        public virtual ICollection<UT_Payment> UT_Payment { get; set; }
        public virtual ICollection<UT_SpeakRecord> UT_SpeakRecord { get; set; }
        public virtual ICollection<UT_UserLoginRecord> UT_UserLoginInfo { get; set; }
        public virtual ICollection<UT_SMS> UT_SMS { get; set; }
        public virtual ICollection<UT_Order> UT_Order { get; set; }
        public virtual ICollection<UT_DeviceBracelet> UT_DeviceBracelet { get; set; }
        public virtual ICollection<UT_AlarmClock> UT_AlarmClock { get; set; }
        public virtual ICollection<UT_UsersConfig> UT_UsersConfig { get; set; }
        public virtual ICollection<UT_Sport> UT_Sport { get; set; }
        public virtual ICollection<UT_SportTimePeriod> UT_SportTimePeriod { get; set; }
        public virtual ICollection<UT_Message> UT_Message { get; set; }
        public virtual ICollection<UT_MessageComment> UT_MessageComment { get; set; }
        public virtual ICollection<UT_MessageLike> UT_MessageLike { get; set; }
        public virtual ICollection<UT_Feedback> UT_Feedback { get; set; }
        public virtual ICollection<UT_PhoneCallback> UT_PhoneCallback { get; set; }
        public virtual UT_UsersGroup UT_UsersGroup { get; set; }
        public virtual ICollection<UT_CallTransferNum> UT_CallTransferNum { get; set; }
        public virtual ICollection<UT_UserShape> UT_UserShape { get; set; }
        public virtual ICollection<UT_PaymentCard> UT_PaymentCard { get; set; }
        public virtual ICollection<UT_DeviceGoip> UT_DeviceGoip { get; set; }
        //public virtual ICollection<UT_OrderByZC> UT_OrderByZC { get; set; }
        public virtual ICollection<UT_OrderByZCConfirmation> UT_OrderByZCConfirmation { get; set; }
        public virtual ICollection<UT_OrderByZCSelectionNumber> UT_OrderByZCSelectionNumber { get; set; }
        public virtual UT_UsersWx UT_UsersWx { get; set; }
        public virtual ICollection<UT_GiftCard> UT_GiftCard { get; set; }
        public virtual ICollection<UT_EjoinDevSlot> UT_EjoinDevSlot { get; set; }

    }
}
