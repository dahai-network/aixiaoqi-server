using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Unitoys.Model;
using Unitoys.Model.Migrations;
using System.Data.Entity.Migrations;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Infrastructure;


namespace Unitoys.Model
{
    public class UnitoysEntities : DbContext
    {
        public UnitoysEntities()
            : base("UnitoysEntities")
        {
            //自动创建表，如果Entity有改到就更新到表结构。（数据迁移）
            Database.SetInitializer<UnitoysEntities>(new MigrateDatabaseToLatestVersion<UnitoysEntities, Configuration>());

            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.AutoDetectChangesEnabled = false;
            Configuration.ValidateOnSaveEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }
        public UnitoysEntities(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }
        public DbSet<UT_ManageUsers> UT_ManageUsers { get; set; }
        public DbSet<UT_Country> UT_Country { get; set; }
        public DbSet<UT_Package> UT_Package { get; set; }
        public DbSet<UT_Payment> UT_Payment { get; set; }
        public DbSet<UT_SMS> UT_SMS { get; set; }
        public DbSet<UT_SMSConfirmation> UT_SMSConfirmation { get; set; }
        public DbSet<UT_SpeakRecord> UT_SpeakRecord { get; set; }
        public DbSet<UT_UserBill> UT_UserBill { get; set; }
        public DbSet<UT_UserLoginRecord> UT_UserLoginInfo { get; set; }
        public DbSet<UT_Users> UT_Users { get; set; }
        public DbSet<UT_UsersGroup> UT_UsersGroup { get; set; }
        public DbSet<UT_DeviceGoip> UT_DeviceGoip { get; set; }
        public DbSet<UT_DeviceBracelet> UT_DeviceBracelet { get; set; }
        public DbSet<UT_EjoinDev> UT_EjoinDev { get; set; }
        public DbSet<UT_EjoinDevSlot> UT_EjoinDevSlot { get; set; }
        public DbSet<UT_Sport> UT_Sport { get; set; }
        public DbSet<UT_SportTimePeriod> UT_SportTimePeriod { get; set; }
        public DbSet<UT_Order> UT_Order { get; set; }
        public DbSet<UT_OrderUsage> UT_OrderUsage { get; set; }
        public DbSet<UT_Message> UT_Message { get; set; }
        public DbSet<UT_MessagePhoto> UT_MessagePhoto { get; set; }
        public DbSet<UT_MessageComment> UT_MessageComment { get; set; }
        public DbSet<UT_MessageLike> UT_MessageLike { get; set; }
        public DbSet<UT_Feedback> UT_Feedback { get; set; }
        public DbSet<UT_Role> UT_Role { get; set; }
        public DbSet<UT_Permission> UT_Permission { get; set; }
        public DbSet<UT_RolePermission> UT_RolePermission { get; set; }
        public DbSet<UT_ManageUsersRole> UT_ManageUsersRole { get; set; }
        public DbSet<UT_PhoneCallback> UT_PhoneCallback { get; set; }
        public DbSet<UT_CallTransferNum> UT_CallTransferNum { get; set; }
        public DbSet<UT_UserShape> UT_UserShape { get; set; }
        public DbSet<UT_PaymentCard> UT_PaymentCard { get; set; }
        public DbSet<UT_UsersWx> UT_UsersWx { get; set; }
        public DbSet<UT_Banner> UT_Banner { get; set; }
        public DbSet<UT_AlarmClock> UT_AlarmClock { get; set; }
        public DbSet<UT_UsersConfig> UT_UsersConfig { get; set; }
        public DbSet<UT_PageShow> UT_PageShow { get; set; }
        public DbSet<UT_OrderByZC> UT_OrderByZC { get; set; }
        public DbSet<UT_OrderByZCConfirmation> UT_OrderByZCConfirmation { get; set; }
        public DbSet<UT_OrderByZCSelectionNumber> UT_OrderByZCSelectionNumber { get; set; }
        public DbSet<UT_ZCSelectionNumber> UT_ZCSelectionNumber { get; set; }
        public DbSet<UT_GiftCard> UT_GiftCard { get; set; }
        public DbSet<UT_OperationRecord> UT_OperationRecord { get; set; }
        public DbSet<UT_DeviceBraceletConnectRecord> UT_DeviceBraceletConnectRecord { get; set; }
        public DbSet<UT_Product> UT_Product { get; set; }
        public DbSet<UT_BlackList> UT_BlackList { get; set; }
        public DbSet<UT_GlobalContent> UT_GlobalContent { get; set; }
        public DbSet<UT_News> UT_News { get; set; }
        public DbSet<UT_OrderDeviceTel> UT_OrderDeviceTel { get; set; }
        public DbSet<UT_UserReceive> UT_UserReceive { get; set; }
        public DbSet<UT_UserDeviceTel> UT_UserDeviceTel { get; set; }
        public DbSet<UT_PushContent> UT_PushContent { get; set; }
        public DbSet<UT_Attribute> UT_Attribute { get; set; }
        public DbSet<UT_AttributeValue> UT_AttributeValue { get; set; }
        public DbSet<UT_ProductAttribute> UT_ProductAttribute { get; set; }
        public DbSet<UT_PackageAttribute> UT_PackageAttribute { get; set; }
        public DbSet<UT_AfterSales> UT_AfterSales { get; set; }
        public DbSet<UT_Agent> UT_Agent { get; set; }
        public DbSet<UT_ContactUS> UT_ContactUS { get; set; }
        public DbSet<UT_UserLog> UT_UserLog { get; set; }
        public DbSet<UT_DeviceBraceletUsageRecord> UT_DeviceBraceletUsageRecord { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //注册配置
            modelBuilder.Configurations.Add(new ManageUsersConfiguration());
            modelBuilder.Configurations.Add(new CountryConfiguration());
            modelBuilder.Configurations.Add(new PaymentConfiguration());
            modelBuilder.Configurations.Add(new PackageConfiguration());
            modelBuilder.Configurations.Add(new UsersConfiguration());
            modelBuilder.Configurations.Add(new UsersGroupConfiguration());
            modelBuilder.Configurations.Add(new SMSConfiguration());
            modelBuilder.Configurations.Add(new SMSConfirmationConfiguration());
            modelBuilder.Configurations.Add(new SpeakRecordConfiguration());
            modelBuilder.Configurations.Add(new UserBillConfiguration());
            modelBuilder.Configurations.Add(new UserLoginRecordConfiguration());
            modelBuilder.Configurations.Add(new DeviceGoipConfiguration());
            modelBuilder.Configurations.Add(new DeviceBraceletConfiguration());
            modelBuilder.Configurations.Add(new EjoinDevConfiguration());
            modelBuilder.Configurations.Add(new EjoinDevSlotConfiguration());
            modelBuilder.Configurations.Add(new OrderConfiguration());
            modelBuilder.Configurations.Add(new OrderUsageConfiguration());
            modelBuilder.Configurations.Add(new MessageConfiguration());
            modelBuilder.Configurations.Add(new MessagePhotoConfiguration());
            modelBuilder.Configurations.Add(new MessageCommentConfiguration());
            modelBuilder.Configurations.Add(new MessageLikeConfiguration());
            modelBuilder.Configurations.Add(new FeedbackConfiguration());
            modelBuilder.Configurations.Add(new RoleConfiguration());
            modelBuilder.Configurations.Add(new PermissionConfiguration());
            modelBuilder.Configurations.Add(new RolePermissionConfiguration());
            modelBuilder.Configurations.Add(new ManageUsersRoleConfiguration());
            modelBuilder.Configurations.Add(new PhoneCallbackConfiguration());
            modelBuilder.Configurations.Add(new CallTransferNumConfiguration());
            modelBuilder.Configurations.Add(new PaymentCardConfiguration());
            modelBuilder.Configurations.Add(new UserShapeConfiguration());
            modelBuilder.Configurations.Add(new UserWxConfiguration());
            modelBuilder.Configurations.Add(new BannerConfiguration());
            modelBuilder.Configurations.Add(new AlarmClockConfiguration());
            modelBuilder.Configurations.Add(new UsersConfigConfiguration());
            modelBuilder.Configurations.Add(new PageShowConfiguration());
            modelBuilder.Configurations.Add(new OrderByZCConfiguration());
            modelBuilder.Configurations.Add(new OrderByZCConfirmationConfiguration());
            modelBuilder.Configurations.Add(new OrderByZCSelectionNumberConfiguration());
            modelBuilder.Configurations.Add(new ZCSelectionNumberConfiguration());
            modelBuilder.Configurations.Add(new GiftCardConfiguration());
            modelBuilder.Configurations.Add(new OperationRecordConfiguration());
            modelBuilder.Configurations.Add(new DeviceBraceletConnectRecordConfiguration());
            modelBuilder.Configurations.Add(new ProductConfiguration());
            modelBuilder.Configurations.Add(new BlackListConfiguration());
            modelBuilder.Configurations.Add(new GlobalContentConfiguration());
            modelBuilder.Configurations.Add(new NewsConfiguration());
            modelBuilder.Configurations.Add(new OrderDeviceTelConfiguration());
            modelBuilder.Configurations.Add(new UserReceiveConfiguration());
            modelBuilder.Configurations.Add(new UserDeviceTelConfiguration());
            modelBuilder.Configurations.Add(new PushContentConfiguration());
            modelBuilder.Configurations.Add(new AttributeConfiguration());
            modelBuilder.Configurations.Add(new AttributeValueConfiguration());
            modelBuilder.Configurations.Add(new ProductAttributeConfiguration());
            modelBuilder.Configurations.Add(new PackageAttributeConfiguration());
            modelBuilder.Configurations.Add(new AfterSalesConfiguration());
            modelBuilder.Configurations.Add(new AgentConfiguration());
            modelBuilder.Configurations.Add(new ContactUSConfiguration());
            modelBuilder.Configurations.Add(new UserLogConfiguration());
            modelBuilder.Configurations.Add(new DeviceBraceletUsageRecordConfiguration());

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();  //表中都统一设置禁用一对多级联删除
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>(); //表中都统一设置禁用多对多级联删除
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>(); //表名为类名，不是上面带s的名字  //移除复数表名的契约

            //已过期的函数modelBuilder.Conventions.Remove<IncludeMetadataConvention>();     //不创建EdmMetadata表  //防止黑幕交易 要不然每次都要访问 EdmMetadata这个表

            base.OnModelCreating(modelBuilder);
        }
        public void FixEfProviderServicesProblem()
        {
            //The Entity Framework provider type 'System.Data.Entity.SqlServer.SqlProviderServices, EntityFramework.SqlServer'  
            //for the 'System.Data.SqlClient' ADO.NET provider could not be loaded.   
            //Make sure the provider assembly is available to the running application.   
            //See http://go.microsoft.com/fwlink/?LinkId=260882 for more information.  
            var instance = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
        }
    }
}
