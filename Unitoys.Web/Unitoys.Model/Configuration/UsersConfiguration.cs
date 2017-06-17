using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 用户表模型配置
    /// </summary>
    public class UsersConfiguration : EntityTypeConfiguration<UT_Users>
    {
        public UsersConfiguration()
        {
            //在线充值记录表1对多
            this.HasMany(t => t.UT_Payment).WithRequired(t => t.UT_Users).HasForeignKey(t => t.UserId);
            //通话记录表1对多
            this.HasMany(t => t.UT_SpeakRecord).WithRequired(t => t.UT_Users).HasForeignKey(t => t.UserId);
            //用户登录记录表1对多
            this.HasMany(t => t.UT_UserLoginInfo).WithRequired(t => t.UT_Users).HasForeignKey(t => t.UserId);
            //短信记录表1对多
            this.HasMany(t => t.UT_SMS).WithRequired(t => t.UT_Users).HasForeignKey(t => t.UserId);
            //订单表1对多
            this.HasMany(t => t.UT_Order).WithRequired(t => t.UT_Users).HasForeignKey(t => t.UserId);
            //用户Wifi设备1对多
            this.HasMany(t => t.UT_DeviceBracelet).WithRequired(t => t.UT_Users).HasForeignKey(t => t.UserId);
            //动态消息表1对多
            this.HasMany(t => t.UT_Message).WithRequired(t => t.UT_Users).HasForeignKey(t => t.UserId);
            //评论1对多
            this.HasMany(t => t.UT_MessageComment).WithRequired(t => t.UT_Users).HasForeignKey(t => t.UserId);
            //点赞1对多
            this.HasMany(t => t.UT_MessageLike).WithRequired(t => t.UT_Users).HasForeignKey(t => t.UserId);
            //反馈1对多
            this.HasMany(t => t.UT_Feedback).WithOptional(t => t.UT_Users).HasForeignKey(t => t.UserId);
            //电话回拨记录1对多
            this.HasMany(t => t.UT_PhoneCallback).WithRequired(t => t.UT_Users).HasForeignKey(t => t.UserId);

            //大号资源表1对多
            this.HasMany(t => t.UT_CallTransferNum).WithOptional(t => t.UT_Users).HasForeignKey(t => t.UserId);
            //充值卡1对多
            this.HasMany(t => t.UT_PaymentCard).WithOptional(t => t.UT_Users).HasForeignKey(t => t.UserId);
            //用户体形表1对多
            this.HasMany(t => t.UT_UserShape).WithRequired(t => t.UT_Users).HasForeignKey(t => t.UserId);
            //用户Wifi设备1对多
            this.HasMany(t => t.UT_DeviceGoip).WithOptional(t => t.UT_Users).HasForeignKey(t => t.UserId);
            //用户运动表1对多
            this.HasMany(t => t.UT_Sport).WithRequired(t => t.UT_Users).HasForeignKey(t => t.UserId);
            //微信绑定用户表1对1
            // this.HasRequired(t => t.UT_UsersWx).WithRequiredPrincipal(t => t.UT_User);
            //闹钟1对多
            this.HasMany(t => t.UT_AlarmClock).WithRequired(t => t.UT_Users).HasForeignKey(t => t.UserId);
            //用户配置1对多
            this.HasMany(t => t.UT_UsersConfig).WithRequired(t => t.UT_Users).HasForeignKey(t => t.UserId);
            //众筹订单表1对多
            //this.HasMany(t => t.UT_OrderByZC).WithOptional(t => t.UT_Users).HasForeignKey(t => t.UserId);
            //众筹订单号码确认表1对多
            this.HasMany(t => t.UT_OrderByZCSelectionNumber).WithRequired(t => t.UT_Users).HasForeignKey(t => t.UserId);
            //礼品卡1对多
            this.HasMany(t => t.UT_GiftCard).WithOptional(t => t.UT_Users).HasForeignKey(t => t.UserId);
            //一正设备端口一对多
            this.HasMany(t => t.UT_EjoinDevSlot).WithOptional(t => t.UT_Users).HasForeignKey(t => t.UserId);
            //手环设备连接记录表1对多
            this.HasMany(t => t.UT_DeviceBraceletConnectRecord).WithRequired(t => t.UT_Users).HasForeignKey(t => t.UserId);
            //黑名单1对多
            this.HasMany(t => t.UT_BlackList).WithRequired(t => t.UT_Users).HasForeignKey(t => t.UserId);
            //用户领取1对多
            this.HasMany(t => t.UT_UserReceive).WithRequired(t => t.UT_Users).HasForeignKey(t => t.UserId);
            //用户设备号码1对多
            this.HasMany(t => t.UT_UserDeviceTel).WithRequired(t => t.UT_Users).HasForeignKey(t => t.UserId);
            //用户日志1对多
            this.HasMany(t => t.UT_UserLog).WithRequired(t => t.UT_Users).HasForeignKey(t => t.UserId);
            //用户设备使用记录
            this.HasMany(t => t.UT_DeviceBraceletUsageRecord).WithRequired(t => t.UT_Users).HasForeignKey(t => t.UserId);

            this.Property(t => t.PassWord).HasMaxLength(32).IsRequired();

            this.Property(t => t.NickName).HasMaxLength(20).IsOptional();

            this.Property(t => t.UserHead).HasMaxLength(200).IsRequired();

            this.Property(t => t.TrueName).HasMaxLength(10).IsOptional();

            this.Property(t => t.Amount).HasPrecision(13, 2).IsRequired();

            this.Property(t => t.Email).HasMaxLength(50).IsOptional();

            this.Property(t => t.Tel).HasMaxLength(15).IsRequired();

            this.Property(t => t.QQ).HasMaxLength(12).IsOptional();

            this.Property(t => t.Sex).IsOptional();

            this.Property(t => t.Status).IsRequired();

            this.Property(p => p.RowVersion).IsRowVersion();

        }
    }
}
