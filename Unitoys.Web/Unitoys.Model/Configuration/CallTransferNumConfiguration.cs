using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    public class CallTransferNumConfiguration : EntityTypeConfiguration<UT_CallTransferNum>
    {
        public CallTransferNumConfiguration()
        {

            this.Property(t => t.Status).IsRequired();

            this.Property(t => t.TelNum).HasMaxLength(12).IsRequired();

            this.Property(t => t.TelPwd).HasMaxLength(8).IsRequired();

            this.Property(t => t.UserId).IsOptional();
        }
    }
}
