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
    public class OrderConfiguration : EntityTypeConfiguration<UT_Order>
    {
        public OrderConfiguration()
        {
            //订单项1对多
            this.HasMany(t => t.UT_OrderUsage).WithRequired(t => t.UT_Order).HasForeignKey(t => t.OrderId);

            this.Property(t => t.PackageName).HasMaxLength(100).IsRequired();

            this.Property(t => t.UnitPrice).HasPrecision(12, 2).IsRequired();

            this.Property(t => t.TotalPrice).HasPrecision(12, 2).IsRequired();

            this.Property(t => t.ExpireDays).IsRequired();

            this.Property(t => t.OrderNum).HasMaxLength(30).IsRequired();

            this.Property(p => p.RowVersion).IsRowVersion();
        }
    }
}
