using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Core.Config
{
    /// <summary>
    /// 基于文件的配置策略
    /// </summary>
    public class ConfigStrategy : IConfigStrategy
    {
        private readonly string _siteConfigFilePath = "/App_Data/site.config";//关系数据库配置信息文件路径
        private readonly string _deviceBraceletOTAConfigFilePath = "/App_Data/deviceBraceletOTA.config";//手环设备空中升级固件

        /// <summary>
        /// 从文件中加载配置信息
        /// </summary>
        /// <param name="configInfoType">配置信息类型</param>
        /// <param name="configInfoFile">配置信息文件路径</param>
        /// <returns>配置信息</returns>
        private IConfigInfo LoadConfigInfo(Type configInfoType, string configInfoFile)
        {
            return (IConfigInfo)IOHelper.DeserializeFromXML(configInfoType, configInfoFile);
        }
        /// <summary>
        /// 将配置信息保存到文件中
        /// </summary>
        /// <param name="configInfo">配置信息</param>
        /// <param name="configInfoFile">保存路径</param>
        /// <returns>是否保存成功</returns>
        private bool SaveConfigInfo(IConfigInfo configInfo, string configInfoFile)
        {
            return IOHelper.SerializeToXml(configInfo, configInfoFile);
        }

        /// <summary>
        /// 获得配置
        /// </summary>
        public SiteConfigInfo GetSiteConfig()
        {
            return (SiteConfigInfo)LoadConfigInfo(typeof(SiteConfigInfo), IOHelper.GetMapPath(_siteConfigFilePath));
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="configInfo">短信配置信息</param>
        /// <returns>是否保存结果</returns>
        public bool SaveSiteConfig(SiteConfigInfo configInfo)
        {
            return SaveConfigInfo(configInfo, IOHelper.GetMapPath(_siteConfigFilePath));
        }

        /// <summary>
        /// 获得配置
        /// </summary>
        public DeviceBraceletOTAConfigInfo GetDeviceBraceletOTAConfig()
        {
            return (DeviceBraceletOTAConfigInfo)LoadConfigInfo(typeof(DeviceBraceletOTAConfigInfo), IOHelper.GetMapPath(_deviceBraceletOTAConfigFilePath));
        }
    }
}
