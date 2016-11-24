using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    public class MessageCommentConfiguration : EntityTypeConfiguration<UT_MessageComment>
    {
        public MessageCommentConfiguration()
        {
            this.Property(t => t.Content).HasMaxLength(150).IsRequired();
        }
    }
}
