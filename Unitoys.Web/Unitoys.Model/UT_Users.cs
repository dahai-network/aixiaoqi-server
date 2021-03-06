//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Unitoys.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class UT_Users
    {
        public UT_Users()
        {
            this.UT_SMS = new HashSet<UT_SMS>();
            this.UT_SpeakRecord = new HashSet<UT_SpeakRecord>();
            this.UT_UserLoginInfo = new HashSet<UT_UserLoginInfo>();
            this.UT_UserPackage = new HashSet<UT_UserPackage>();
            this.UT_PayRecord = new HashSet<UT_PayRecord>();
        }
    
        public int ID { get; set; }
        public string LoginName { get; set; }
        public string PassWord { get; set; }
        public string TrueName { get; set; }
        public string Email { get; set; }
        public string Tel { get; set; }
        public string QQ { get; set; }
        public Nullable<System.DateTime> AddTime { get; set; }
        public Nullable<int> GroupId { get; set; }
        public Nullable<int> Lock4 { get; set; }
        public Nullable<int> Score { get; set; }
        public Nullable<int> Sex { get; set; }
        public string Amount { get; set; }
    
        public virtual ICollection<UT_SMS> UT_SMS { get; set; }
        public virtual ICollection<UT_SpeakRecord> UT_SpeakRecord { get; set; }
        public virtual ICollection<UT_UserLoginInfo> UT_UserLoginInfo { get; set; }
        public virtual UT_UsersGroup UT_UsersGroup { get; set; }
        public virtual ICollection<UT_UserPackage> UT_UserPackage { get; set; }
        public virtual ICollection<UT_PayRecord> UT_PayRecord { get; set; }
    }
}
