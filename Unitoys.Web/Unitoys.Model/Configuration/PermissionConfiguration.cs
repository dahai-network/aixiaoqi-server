using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 权限模型配置
    /// </summary>
    public class PermissionConfiguration : EntityTypeConfiguration<UT_Permission>
    {
        public PermissionConfiguration()
        {
            //角色权限多对多
            this.HasMany(t => t.UT_RolePermission).WithRequired(t => t.UT_Permission).HasForeignKey(t => t.PermissionId);

            this.Property(t => t.Name).HasMaxLength(50).IsRequired();

            this.Property(t => t.Description).HasMaxLength(150).IsOptional();
        }
    }
}
