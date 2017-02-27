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
    /// 手环设备TCP连接记录模型配置
    /// </summary>
    public class DeviceBraceletConnectRecordConfiguration : EntityTypeConfiguration<UT_DeviceBraceletConnectRecord>
    {
        public DeviceBraceletConnectRecordConfiguration()
        {

        }
    }
}
