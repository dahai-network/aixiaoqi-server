using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    public class PackageAttributeConfiguration : EntityTypeConfiguration<UT_PackageAttribute>
    {
        public PackageAttributeConfiguration()
        {
            this.HasRequired(b => b.UT_Package).WithMany().HasForeignKey(b => b.PackageId);
        }
    }
}
