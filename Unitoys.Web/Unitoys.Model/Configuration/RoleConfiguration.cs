using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 角色模型配置
    /// </summary>
    public class RoleConfiguration : EntityTypeConfiguration<UT_Role>
    {
        public RoleConfiguration()
        {
            //角色权限ForeignKey
            this.HasMany(t => t.UT_RolePermission).WithRequired(t => t.UT_Role).HasForeignKey(t => t.RoleId);
            //用户角色ForeignKey
            this.HasMany(t => t.UT_ManageUsersRole).WithRequired(t => t.UT_Role).HasForeignKey(t => t.RoleId);

            this.Property(t => t.Name).HasMaxLength(50).IsRequired();

            this.Property(t => t.Description).HasMaxLength(150).IsOptional();
        }
    }
}
