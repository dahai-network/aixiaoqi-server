using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Core.Config;

namespace Unitoys.Core
{
    /// <summary>
    /// 配置管理类
    /// </summary>
    public class UTConfig
    {
        private static object _locker = new object();//锁对象

        private static IConfigStrategy _iconfigstrategy = null;//配置策略

        private static SiteConfigInfo _siteconfiginfo = null;//基础配置信息
        private static DeviceBraceletOTAConfigInfo _deviceBraceletOTAconfiginfo = null;//手环设备OTA配置信息

        static UTConfig()
        {
            _iconfigstrategy = new ConfigStrategy();
        }
        /// <summary>
        /// 邮件配置信息
        /// </summary>
        public static SiteConfigInfo SiteConfig
        {
            get
            {
                if (_siteconfiginfo == null)
                {
                    lock (_locker)
                    {
                        if (_siteconfiginfo == null)
                        {
                            _siteconfiginfo = _iconfigstrategy.GetSiteConfig();
                        }
                    }
                }

                return _siteconfiginfo;
            }
        }

        /// <summary>
        /// 手环固件OTA信息
        /// </summary>
        public static DeviceBraceletOTAConfigInfo DeviceBraceletOTAConfigInfo
        {
            get
            {
                if (_deviceBraceletOTAconfiginfo == null)
                {
                    lock (_locker)
                    {
                        if (_deviceBraceletOTAconfiginfo == null)
                        {
                            _deviceBraceletOTAconfiginfo = _iconfigstrategy.GetDeviceBraceletOTAConfig();
                        }
                    }
                }

                return _deviceBraceletOTAconfiginfo;
            }
        }
    }
}
