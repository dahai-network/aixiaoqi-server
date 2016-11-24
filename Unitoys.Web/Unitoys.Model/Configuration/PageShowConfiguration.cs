using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    public class PageShowConfiguration : EntityTypeConfiguration<UT_PageShow>
    {
        public PageShowConfiguration()
        {
            //this.Property(t => t.IMEI).HasMaxLength(50).IsRequired();
        }
    }
}
