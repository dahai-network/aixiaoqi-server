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
    public class AfterSalesConfiguration : EntityTypeConfiguration<UT_AfterSales>
    {
        public AfterSalesConfiguration()
        {

        }
    }
}
