﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class UnitoysEntities : DbContext
    {
        public UnitoysEntities()
            : base("name=UnitoysEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<UT_Country> UT_Country { get; set; }
        public virtual DbSet<UT_ManageUsers> UT_ManageUsers { get; set; }
        public virtual DbSet<UT_SMS> UT_SMS { get; set; }
        public virtual DbSet<UT_SpeakRecord> UT_SpeakRecord { get; set; }
        public virtual DbSet<UT_UserLoginInfo> UT_UserLoginInfo { get; set; }
        public virtual DbSet<UT_Users> UT_Users { get; set; }
        public virtual DbSet<UT_UsersGroup> UT_UsersGroup { get; set; }
        public virtual DbSet<UT_Package> UT_Package { get; set; }
        public virtual DbSet<UT_UserPackage> UT_UserPackage { get; set; }
        public virtual DbSet<UT_PayRecord> UT_PayRecord { get; set; }
        public virtual DbSet<UT_UserBill> UT_UserBill { get; set; }
    }
}
