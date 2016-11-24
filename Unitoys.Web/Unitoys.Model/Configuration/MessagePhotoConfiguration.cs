using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    public class MessagePhotoConfiguration : EntityTypeConfiguration<UT_MessagePhoto>
    {
        public MessagePhotoConfiguration()
        {
            this.Property(t => t.Path).HasMaxLength(50).IsRequired();
        }
    }
}
