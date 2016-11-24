using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unitoys.Core;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestCacheHelperSet()
        {
            Unitoys.Core.CacheHelper.Set("key1", "111");
            //未操作3秒
            Unitoys.Core.CacheHelper.Set("key2", "222", new TimeSpan(0, 0, 3));
            //5秒后过期
            Unitoys.Core.CacheHelper.Set("key3", "333", DateTime.Now.AddSeconds(5));
            Unitoys.Core.CacheHelper.Set("key4", "444", null, new string[] { "key3" }, testRemovedCallback);
            Unitoys.Core.CacheHelper.Set("key5", "555", null, new string[] { "key2" }, RunOptionsUpdateCallback);

            var aaa = Unitoys.Core.CacheHelper.Get("key1");
            var bbb = Unitoys.Core.CacheHelper.Get("key2");
            var ccc = Unitoys.Core.CacheHelper.Get("key3");
            Assert.AreEqual(aaa, "111");
            Assert.AreEqual(bbb, "222");

            //三秒后过期
            System.Threading.Thread.Sleep(3000);
            //测试过期是否有效
            Assert.AreEqual((string)Unitoys.Core.CacheHelper.Get("key2"), null);

            //测试依赖项更新是否有效
            string key5s = (string)Unitoys.Core.CacheHelper.Get("key5");
            Assert.AreEqual(key5s, "key5testUpdate555");
            
        }

        private static void testRemovedCallback(string key, object value, System.Web.Caching.CacheItemRemovedReason reason)
        {
            if (reason == System.Web.Caching.CacheItemRemovedReason.Removed)
                return;        // 忽略后续调用HttpRuntime.Cache.Insert()所触发的操作

            // 能运行到这里，就表示是肯定是缓存过期了。
            // 换句话说就是：用户3秒钟再也没操作过了。

            // 从参数value取回操作信息
            string aaax = (string)value;
            // 这里可以对info做其它的处理。
        }

        public static void RunOptionsUpdateCallback(
    string key, System.Web.Caching.CacheItemUpdateReason reason,
    out object expensiveObject,
    out System.Web.Caching.CacheDependency dependency,
    out DateTime absoluteExpiration,
    out TimeSpan slidingExpiration)
        {
            // 注意哦：在这个方法中，不要出现【未处理异常】，否则缓存对象将被移除。

            // 说明：这里我并不关心参数reason，因为我根本就没有使用过期时间
            //        所以，只有一种原因：依赖的文件发生了改变。
            //        参数key我也不关心，因为这个方法是【专用】的。

            // 重新加载配置参数

            expensiveObject = "key5testUpdate555";
            //Unitoys.Core.CacheHelper.Set("key2", "222", new TimeSpan(0, 0, 10));
            //依赖缓存项为空的情况下，无法更新缓存项内容，缓存项内容为null
            dependency = new System.Web.Caching.CacheDependency(null, new string[] { "key1" });
            absoluteExpiration = System.Web.Caching.Cache.NoAbsoluteExpiration;
            slidingExpiration = System.Web.Caching.Cache.NoSlidingExpiration;
            //Unitoys.Core.CacheHelper.Set("key5", "444", new string[] { "key2" }, RunOptionsUpdateCallback);
        }
    }
}
