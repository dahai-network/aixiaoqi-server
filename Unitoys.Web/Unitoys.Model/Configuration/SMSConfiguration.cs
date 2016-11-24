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
    /// SMS短信模型配置
    /// </summary>
    public class SMSConfiguration : EntityTypeConfiguration<UT_SMS>
    {
        public SMSConfiguration()
        {

            this.Property(t => t.SMSContent).HasColumnType("text").IsRequired();

            //this.Property(t => t.LoginName).HasMaxLength(20).IsOptional();

            //this.Property(t => t.FromNum).HasMaxLength(13).IsRequired();

            //this.Property(t => t.ToNum).HasMaxLength(13).IsRequired();

        }
    }
}
