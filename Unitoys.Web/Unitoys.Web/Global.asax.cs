using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Unitoys.Ioc;
using Unitoys.Core.Initializer;
using Unitoys.Web.Models;
using Senparc.Weixin.Cache.Redis;
using Senparc.Weixin.Cache;
using Senparc.Weixin.Threads;
using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.MP.TenPayLib;
using Senparc.Weixin.MP.TenPayLibV3;

namespace Unitoys.Web
{
   
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //注册 log4net
            log4net.Config.XmlConfigurator.Configure(
               new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + "\\App_Data\\log4net.config")
           );
            AreaRegistration.RegisterAllAreas();

            GlobalConfiguration.Configure(WebApiConfig.Register);

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            //为ASP.NET MVC注册IOC容器
            NinjectRegister.RegisterFovMvc();
            //为WebApi注册IOC容器
            NinjectRegister.RegisterFovWebApi(GlobalConfiguration.Configuration);
            

            //初始化数据库测试数据
            DatabaseInitializer.Initialize(false);

            System.Net.ServicePointManager.DefaultConnectionLimit = 512;

            MvcHandler.DisableMvcResponseHeader = true;

            RegisterWeixinCache();//注册分布式缓存
            RegisterWeixinThreads();//激活微信缓存（必须）
            RegisterSenparcWeixin();//注册Demo所用微信公众号的账号信息

            Senparc.Weixin.Config.IsDebug = true;//这里设为Debug状态时，/App_Data/目录下会生成日志文件记录所有的API请求日志，正式发布版本建议关闭
        }
        /// <summary>
        /// 自定义缓存策略
        /// </summary>
        private void RegisterWeixinCache()
        {
          

            var redisConfiguration = System.Configuration.ConfigurationManager.AppSettings["Cache_Redis_Configuration"];
            var redisPassWord = System.Configuration.ConfigurationManager.AppSettings["Cache_Redis_PassWord"];
            RedisManager.ConfigurationOption = redisConfiguration;
            RedisManager.ConfigurationPassWord = redisPassWord;

            //如果不执行下面的注册过程，则默认使用本地缓存

            if (redisConfiguration != "Redis配置")
            {
                CacheStrategyFactory.RegisterContainerCacheStrategy(() => RedisContainerCacheStrategy.Instance);//Redis
            }
        }
        /// <summary>
        /// 激活微信缓存
        /// </summary>
        private void RegisterWeixinThreads()
        {
            ThreadUtility.Register();
        }
        /// <summary>
        /// 注册Demo所用微信公众号的账号信息
        /// </summary>
        private void RegisterSenparcWeixin()
        {
            AccessTokenContainer.Register(
                System.Configuration.ConfigurationManager.AppSettings["WeixinAppId"],
                System.Configuration.ConfigurationManager.AppSettings["WeixinAppSecret"],
                "【爱小器】公众号");
        }
     
    }
}