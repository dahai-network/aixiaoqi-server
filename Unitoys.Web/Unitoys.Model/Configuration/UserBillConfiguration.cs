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
    public class UserBillConfiguration : EntityTypeConfiguration<UT_UserBill>
    {
        public UserBillConfiguration()
        {

            this.Property(t => t.Amount).HasPrecision(13,2).IsRequired();

            this.Property(t => t.UserAmount).HasPrecision(13, 2).IsRequired();

        }
    }
}
