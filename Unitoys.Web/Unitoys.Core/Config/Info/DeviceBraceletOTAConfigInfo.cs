using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Core.Config
{
    /// <summary>
    /// 手环设备空中升级配置类
    /// </summary>
    [Serializable]
    public class DeviceBraceletOTAConfigInfo : IConfigInfo
    {
        /// <summary>
        /// 版本号
        /// 示例：20
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// 版本名
        /// 示例：1.0.1
        /// </summary>
        public string VersionName { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Descr { get; set; }
        /// <summary>
        /// 下载URL
        /// </summary>
        public string Url { get; set; }
    }
}
