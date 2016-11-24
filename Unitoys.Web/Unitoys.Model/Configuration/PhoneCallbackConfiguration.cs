using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    public class PhoneCallbackConfiguration : EntityTypeConfiguration<UT_PhoneCallback>
    {
        public PhoneCallbackConfiguration()
        {
            this.Property(x => x.To).HasMaxLength(15).IsRequired();

            this.Property(x => x.From).HasMaxLength(15).IsRequired();
        }
    }
}
