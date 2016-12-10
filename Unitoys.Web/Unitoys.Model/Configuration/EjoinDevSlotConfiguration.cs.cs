using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    public class EjoinDevSlotConfiguration : EntityTypeConfiguration<UT_EjoinDevSlot>
    {
        public EjoinDevSlotConfiguration()
        {


            this.Property(t => t.ICCID).HasMaxLength(20).IsOptional();

            this.Property(t => t.IMEI).HasMaxLength(15).IsOptional();

            this.Property(t => t.PortNum).IsRequired();

            this.Property(t => t.SimNum).HasMaxLength(17).IsOptional();

            this.Property(t => t.Status).IsRequired();

            this.Property(t => t.UserId).IsOptional();
        }
    }
}
