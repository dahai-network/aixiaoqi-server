using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    public class FeedbackConfiguration : EntityTypeConfiguration<UT_Feedback>
    {
        public FeedbackConfiguration()
        {
            this.Property(t => t.Info).HasMaxLength(250).IsRequired();
        }
    }
}
