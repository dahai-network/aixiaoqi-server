using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    public class UserPackageConfiguration : EntityTypeConfiguration<UT_UserPackage>
    {
        public UserPackageConfiguration()
        {

            this.Property(t => t.LoginName).HasMaxLength(20).IsOptional();

            this.Property(t => t.PackageName).HasMaxLength(100).IsRequired();

            this.Property(t => t.TotalFlow).IsRequired();

            this.Property(t => t.UsedFlow).IsRequired();

        }
    }
}
