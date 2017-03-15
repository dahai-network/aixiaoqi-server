using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Core.Config
{
    /// <summary>
    /// 配置策略接口
    /// </summary>
    public partial interface IConfigStrategy
    {
        /// <summary>
        /// 获得配置
        /// </summary>
        SiteConfigInfo GetSiteConfig();
        /// <summary>
        /// 获得配置
        /// </summary>
        DeviceBraceletOTAConfigInfo GetDeviceBraceletOTAConfig();
        /// <summary>
        /// 获得配置
        /// </summary>
        /// <returns></returns>
        DeviceBraceletUniBoxOTAConfigInfo GetDeviceBraceletUniBoxOTAConfig();
    }
}
