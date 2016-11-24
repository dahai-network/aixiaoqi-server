using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    public class DeviceGoipConfiguration : EntityTypeConfiguration<UT_DeviceGoip>
    {
        public DeviceGoipConfiguration()
        {


            this.Property(t => t.UpdateDate).IsRequired();

            this.Property(t => t.CreateDate).IsRequired();

            this.Property(t => t.DeviceName).HasMaxLength(30).IsRequired();

            this.Property(t => t.Mac).HasMaxLength(17).IsRequired();

            this.Property(t => t.Status).IsRequired();

            this.Property(t => t.Port).IsRequired();

            this.Property(t => t.UserId).IsOptional();
        }
    }
}
