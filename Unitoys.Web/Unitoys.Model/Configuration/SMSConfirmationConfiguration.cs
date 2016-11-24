using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// SMS短信模型配置
    /// </summary>
    public class SMSConfirmationConfiguration : EntityTypeConfiguration<UT_SMSConfirmation>
    {
        public SMSConfirmationConfiguration()
        {
            this.Property(t => t.Tel).HasMaxLength(15).IsRequired();

            this.Property(t => t.Code).HasMaxLength(20).IsRequired();
        }
    }
}
