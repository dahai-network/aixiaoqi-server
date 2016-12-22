using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    public class GiftCardConfiguration : EntityTypeConfiguration<UT_GiftCard>
    {
        public GiftCardConfiguration()
        {
            this.Property(t => t.Status).IsRequired();

            this.Property(t => t.CreateDate).IsRequired();

            this.Property(t => t.CardNum).HasMaxLength(12).IsRequired();

            this.Property(t => t.CardPwd).HasMaxLength(16).IsRequired();

            this.Property(t => t.UserId).IsOptional();

            this.Property(p => p.RowVersion).IsRowVersion();
        }
    }
}
