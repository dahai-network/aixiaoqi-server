using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    public class PackageConfiguration : EntityTypeConfiguration<UT_Package>
    {
        public PackageConfiguration()
        {
            //订单项1对多
            this.HasMany(t => t.UT_Order).WithRequired(t => t.UT_Package).HasForeignKey(t => t.PackageId);

            this.Property(t => t.PackageName).HasMaxLength(100).IsRequired();

            this.Property(t => t.Price).HasPrecision(12, 2);

            //this.Property(t => t.Desction).HasMaxLength(200);

            this.Property(t => t.Pic).HasMaxLength(100);

            
        }
    }
}
