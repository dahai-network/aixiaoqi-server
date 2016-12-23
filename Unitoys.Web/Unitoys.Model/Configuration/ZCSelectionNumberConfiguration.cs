using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 流量记录模型配置
    /// </summary>
    public class ZCSelectionNumberConfiguration : EntityTypeConfiguration<UT_ZCSelectionNumber>
    {
        public ZCSelectionNumberConfiguration()
        {
            //众筹订单选号绑定选号1对1
            //this.HasOptional(t => t.UT_OrderByZCSelectionNumber).WithMany().HasForeignKey(t => t.OrderByZCSelectionNumberId);
            this.Property(p => p.RowVersion).IsRowVersion();
        }
    }
}
