using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    public class OperationRecordConfiguration : EntityTypeConfiguration<UT_OperationRecord>
    {
        public OperationRecordConfiguration()
        {
            this.Property(t => t.CreateDate).IsRequired();
            this.Property(t => t.ManageUserId).IsRequired();
        }
    }
}
