using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    public class UserShapeConfiguration : EntityTypeConfiguration<UT_UserShape>
    {
        public UserShapeConfiguration()
        {
            //this.Property(t => t.UserId).IsRequired();
        }
    }
}
