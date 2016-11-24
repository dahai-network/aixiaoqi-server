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
    /// 国家费率模型配置
    /// </summary>
    public class CountryConfiguration : EntityTypeConfiguration<UT_Country>
    {
        public CountryConfiguration()
        {
         
            this.HasMany(t => t.UT_Package).WithRequired(t => t.UT_Country).HasForeignKey(t => t.CountryId);

            this.Property(t => t.CountryName).HasMaxLength(50).IsRequired();

            this.Property(t => t.Pic).HasMaxLength(150).IsRequired();

            this.Property(t => t.Rate).HasPrecision(12, 2).IsRequired();

        }
    }
}
