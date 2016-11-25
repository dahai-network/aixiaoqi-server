using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 订单模型配置
    /// </summary>
    public class OrderByZCConfiguration : EntityTypeConfiguration<UT_OrderByZC>
    {
        public OrderByZCConfiguration()
        {
            //订单项1对多
            this.HasMany(t => t.UT_OrderByZCSelectionNumber).WithRequired(t => t.UT_OrderByZC).HasForeignKey(t => t.OrderByZCId);

            this.Property(t => t.UnitPrice).HasPrecision(12, 2).IsRequired();

            this.Property(t => t.TotalPrice).HasPrecision(12, 2).IsRequired();

            this.Property(t => t.OrderByZCNum).HasMaxLength(30).IsRequired();
        }
    }
}
