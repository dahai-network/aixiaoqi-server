using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unitoys.Eims;
using System.Collections.Generic;

namespace UnitTest
{
    [TestClass]
    public class UnitTestEimsApi
    {
        [TestMethod]
        public void TestSMSSend()
        {
            TestAsync();
            //Assert.AreEqual((string)Unitoys.Core.CacheHelper.Get("key2"), null);
        }

        private static async void TestAsync()
        {
            EimsApi api = new EimsApi();
            List<SMSTaskModel> list = new List<SMSTaskModel>()
            {
                new  SMSTaskModel()
                {
                tid=1,
                devname="shebei1",
                port="1",
                iccid="",
                to="13800138000,13800138001",
                sms="Hello 你好",
                sdr=null,//是否开启短信成功状态报告
                },
                new  SMSTaskModel()
                {
                tid=2,
                devname="shebei1",
                port="2",
                iccid="",
                to="13800138000,13800138001",
                sms="Hello 你不好"
                }
            };
            //发送短信
            //可能出错地方，传递了参数值为null
            var result = await api.SMSSend(list);
            Assert.AreNotEqual(result, null);
            Assert.AreEqual(result.code, 200);
            Assert.AreEqual(result.reason, "OK");
            Assert.AreEqual(result.type, "task-status");
            Assert.AreNotEqual(result.status.Length, 0);

            //查询短信状态
            var result2 = await api.SMSReportQuery("11", "", "", "");
            Assert.AreNotEqual(result, null);
            Assert.AreEqual(result.code, 200);
            Assert.AreEqual(result.reason, "OK");
            Assert.AreEqual(result.type, "status-report");
            Assert.AreNotEqual(result.status.Length, 0);

        }
    }
}
