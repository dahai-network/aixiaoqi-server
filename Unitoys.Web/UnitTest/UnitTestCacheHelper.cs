using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unitoys.Core;
using Unitoys.ESIM_MVNO;
using Unitoys.ESIM_MVNO.Model;

namespace UnitTest
{
//FEFEFE5CC04CE0FBCFAB47A62E7632CB9CCEB5F8626687F8878C86D4CBE0495C19C4AE29CA3BCFC0F9C5529D51B67E27430157ECF4B92E44AD3AA0A891C250ED569F7587DF4058135E535397435FA5984F8F885A5AF81BBE772EA4397ECE21B3F840C8FB9D98CBFF1934B5ED686B00FE58845EE63B294D76123839A527DD42A4E21CD35F325EBB4AA2EC5C
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


        [TestMethod]
        public void TestSimDataSTK()
        {
            //SimDataSTK simdata = new SimDataSTK("", new WriteCardSTK()
            //{
            //    iccid = "89860113937500391111",
            //    imsi = "460013833979554",
            //    ki = "F8684A722087E1B76D5E410B994B2222",
            //    //SMSC = "8613800773500F",
            //    //OP = "71CB7A4ABEEC399E06F6FBEA550D3333",
            //    opc = "71CB7A4ABEEC399E06F6FBEA550D3333",
            //    //ISDN = "18012341234F",
            //    //SMSP = "FFFFFFFFFFFFFFFFFFFFFFFFFDFFFFFFFFFFFFFFFFFFFFFFFF0891683110700205F0FFFFFFFFFFFF",
            //    //MSISDN = "FFFFFFFFFFFFFFFFFFFFFFFFFFFF0891688186888888F8FFFFFFFFFF",
            //    //PREFER_NETWORK = "460F01FFFFFFFFFFFFFFFFFFFF",
            //    //EXP_DATE = "1512080000000000",
            //});
            //string writeData = simdata.GetData();

            SimDataSTK simdata = new SimDataSTK("", new WriteCardSTK()
            {
                iccid = "89852031600002515631",
                imsi = "454030220251563",
                ki = "7B173A5B0DABE9564AACF92421A1EE0C",
                //SMSC = "8613800773500F",
                //OP = "71CB7A4ABEEC399E06F6FBEA550D3333",
                opc = "D8314DEF156D27EA9B2ABB4D525991CD",
                PLMN = "268F01214F01214F06204F04230F03255F03208F01262F02250F99232F05232F10202F05272F05272F02234F20206F20242F05270F77219F02246F03226F01238F06222F99228F01240F02286F02244F05244F21260F06",
                //ISDN = "18012341234F",
                //SMSP = "FFFFFFFFFFFFFFFFFFFFFFFFFDFFFFFFFFFFFFFFFFFFFFFFFF0891683110700205F0FFFFFFFFFFFF",
                //MSISDN = "FFFFFFFFFFFFFFFFFFFFFFFFFFFF0891688186888888F8FFFFFFFFFF",
                //PREFER_NETWORK = "460F01FFFFFFFFFFFFFFFFFFFF",
                //EXP_DATE = "1512080000000000",
            });
            string writeData = simdata.GetData();
            Console.WriteLine(writeData);
        }
    }
}
