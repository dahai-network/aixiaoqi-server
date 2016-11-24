using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 在线支付记录模型配置
    /// </summary>
    public class PaymentConfiguration : EntityTypeConfiguration<UT_Payment>
    {
        public PaymentConfiguration()
        {

            this.Property(t => t.PaymentMethod).IsRequired();

            this.Property(t => t.PaymentPurpose).HasMaxLength(20).IsRequired();

            this.Property(t => t.Amount).HasPrecision(12, 2).IsRequired();

            this.Property(t => t.Discount).HasPrecision(12, 2).IsOptional();

        }
    }
}
