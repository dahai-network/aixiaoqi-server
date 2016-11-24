using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    public class UsersGroupConfiguration : EntityTypeConfiguration<UT_UsersGroup>
    {
        public UsersGroupConfiguration()
        {

            this.HasMany(t => t.UT_Users).WithRequired(t=>t.UT_UsersGroup).HasForeignKey(t=>t.GroupId);

            Property(t => t.GroupName).HasMaxLength(20).IsRequired();

            Property(t => t.Level).IsRequired();

        }
    }
}
