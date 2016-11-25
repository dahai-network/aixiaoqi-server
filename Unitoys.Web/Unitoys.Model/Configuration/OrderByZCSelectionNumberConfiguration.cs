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
    public class OrderByZCSelectionNumberConfiguration : EntityTypeConfiguration<UT_OrderByZCSelectionNumber>
    {
        public OrderByZCSelectionNumberConfiguration()
        {
            //众筹订单选号绑定选号1对1
            this.HasRequired(t => t.UT_ZCSelectionNumber).WithMany().HasForeignKey(t => t.ZCSelectionNumberId);
        }
    }
}
