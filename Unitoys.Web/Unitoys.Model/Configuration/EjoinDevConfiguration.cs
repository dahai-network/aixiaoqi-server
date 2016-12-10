using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    public class EjoinDevConfiguration : EntityTypeConfiguration<UT_EjoinDev>
    {
        public EjoinDevConfiguration()
        {
            //一台设备对应多个端口
            this.HasMany(t => t.UT_EjoinDevSlot).WithRequired(t => t.UT_EjoinDev).HasForeignKey(t => t.EjoinDevId);

            this.Property(t => t.Name).HasMaxLength(10).IsRequired();

            this.Property(t => t.Password).HasMaxLength(20).IsRequired();

            this.Property(t => t.MaxPort).IsRequired();

            this.Property(t => t.ModType).IsRequired();

            this.Property(t => t.Mac).IsOptional();

            this.Property(t => t.RegIp).HasMaxLength(15).IsOptional();

            this.Property(t => t.RegStatus).IsRequired();

            this.Property(t => t.Version).HasMaxLength(50).IsOptional();
        }
    }
}
