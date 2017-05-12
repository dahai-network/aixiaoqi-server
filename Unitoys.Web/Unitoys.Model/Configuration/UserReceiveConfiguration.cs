using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    public class UserReceiveConfiguration : EntityTypeConfiguration<UT_UserReceive>
    {
        public UserReceiveConfiguration()
        {
            this.HasRequired(b => b.UT_Order).WithMany().HasForeignKey(b => b.OrderId);
        }
    }
}
