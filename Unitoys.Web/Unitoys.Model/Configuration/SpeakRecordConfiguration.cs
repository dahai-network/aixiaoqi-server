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
    /// 通话记录模型配置
    /// </summary>
    public class SpeakRecordConfiguration : EntityTypeConfiguration<UT_SpeakRecord>
    {
        public SpeakRecordConfiguration()
        {
            this.Property(t => t.DeviceName).HasMaxLength(15).IsRequired();

            this.Property(t => t.CalledTelNum).HasMaxLength(30).IsRequired();

            this.Property(t => t.CallSourceIp).HasMaxLength(30).IsRequired();

            this.Property(t => t.CallServerIp).HasMaxLength(30).IsRequired();

            this.Property(t => t.TotalAmount).HasPrecision(13, 2).IsRequired();

            this.Property(t => t.Acctterminatedirection).HasMaxLength(10).IsRequired();
        }
    }
}
