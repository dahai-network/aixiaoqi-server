using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    public class ProductAttributeConfiguration : EntityTypeConfiguration<UT_ProductAttribute>
    {
        public ProductAttributeConfiguration()
        {
            this.HasRequired(b => b.UT_Attribute).WithMany().HasForeignKey(b => b.AttributeId);
            this.HasRequired(b => b.UT_AttributeValue).WithMany().HasForeignKey(b => b.AttributeValueId);
            this.HasRequired(b => b.UT_Package).WithMany().HasForeignKey(b => b.PackageId);
        }
    }
}
