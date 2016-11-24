using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    public class UserLoginRecordConfiguration : EntityTypeConfiguration<UT_UserLoginRecord>
    {
        public UserLoginRecordConfiguration()
        {
            this.Property(t => t.LoginName).HasMaxLength(20).IsOptional();

            this.Property(t => t.Entrance).HasMaxLength(100);

            this.Property(t => t.LoginIp).HasMaxLength(20);

        }
    }
}