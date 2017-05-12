using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    public class OrderDeviceTelConfiguration : EntityTypeConfiguration<UT_OrderDeviceTel>
    {
        public OrderDeviceTelConfiguration()
        {
            this.HasRequired(b => b.UT_Order).WithMany().HasForeignKey(b => b.OrderId);
        }
    }
}
