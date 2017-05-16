using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    public class AttributeValueConfiguration : EntityTypeConfiguration<UT_AttributeValue>
    {
        public AttributeValueConfiguration()
        {
            this.HasRequired(b => b.UT_Attribute).WithMany().HasForeignKey(b => b.AttributeId);
        }
    }
}
