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
    /// 管理员模型配置
    /// </summary>
    public class ManageUsersConfiguration : EntityTypeConfiguration<UT_ManageUsers>
    {
        public ManageUsersConfiguration()
        {
            //用户角色1对多
            this.HasMany(t => t.UT_ManageUsersRole).WithRequired(t => t.UT_ManageUsers).HasForeignKey(t => t.ManageUserId);

            //充值卡1对多
            this.HasMany(t => t.UT_PaymentCard).WithRequired(t => t.UT_ManageUsers).HasForeignKey(t => t.ManageUserId);

            Property(t => t.LoginName).HasMaxLength(20).IsRequired();

            Property(t => t.PassWord).HasMaxLength(32).IsRequired();

            Property(t => t.TrueName).HasMaxLength(25);

        }
    }
}
