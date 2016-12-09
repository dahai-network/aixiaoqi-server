using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Generic;
using Unitoys.ESIM_263mobile;
using Newtonsoft.Json;

namespace UnitTest
{
    [TestClass]
    public class UnitTestMVNOServiceApi
    {
        private MVNOServiceApi _MVNOServiceApi = new MVNOServiceApi();
        private static string userId = "18850161016";
        private static string orderId = "2016120611050976235764045";
        private static string reactiveOrderId = "2016120611073837635765045";
        private static Product product;
        public UnitTestMVNOServiceApi()
        {

        }

        /// <summary>
        /// 查询订单
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestMethodQueryProductList()
        {
            var result = await _MVNOServiceApi.QueryProductList();
            Console.WriteLine(JsonConvert.SerializeObject(result));

            foreach (var item in result.productList)
            {
                if (item.productName.Contains("测试"))
                {
                    product = item;
                };
            }

            Assert.IsNotNull(product);
            Assert.AreEqual("00000", result.returnCode);
        }

        [TestMethod]
        public async Task TestMethodBuyProduct()
        {
            //1000003续订会提示不可续订
            var result = await _MVNOServiceApi.BuyProduct(userId, "1000002", "2016-12-08 00:00:00", 1, 2);
            Console.WriteLine(JsonConvert.SerializeObject(result));

            Assert.AreEqual("00000", result.returnCode);
            orderId = result.orderId;
        }

        [TestMethod]
        public async Task TestMethodGetRemain()
        {
            var result = await _MVNOServiceApi.GetRemain(userId, orderId);
            Console.WriteLine(JsonConvert.SerializeObject(result));

            Assert.AreEqual("00000", result.returnCode);
        }

        /// <summary>
        /// 登网通知
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestMethodNotifyAccess()
        {
            var result = await _MVNOServiceApi.NotifyAccess(userId, orderId);
            Console.WriteLine(JsonConvert.SerializeObject(result));

            Assert.AreEqual("00000", result.returnCode);
        }

        /// <summary>
        /// 退订
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestMethodReturnProduct()
        {

            var result = await _MVNOServiceApi.ReturnProduct(reactiveOrderId);
            Console.WriteLine(JsonConvert.SerializeObject(result));

            Assert.AreEqual("00000", result.returnCode);
        }

        /// <summary>
        /// 续订
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestMethodReactiveProduct()
        {
            var result = await _MVNOServiceApi.ReactiveProduct(userId, orderId, 1);
            Console.WriteLine(JsonConvert.SerializeObject(result));

            //if ("00000" == result.returnCode)
            //{
            reactiveOrderId = result.orderId;
            Assert.AreEqual("00000", result.returnCode);
            //}
            //产品不允许续订
            //else if ("20002" == result.returnCode)
            //{
            //    reactiveOrderId = "";
            //}
            //else
            //{
            //    Assert.Fail("续订失败");
            //}
            //Assert.AreEqual("00000", result.returnCode);

        }

        /// <summary>
        /// 误删重新获取ISMI卡信息接口
        /// IMSI下发
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestMethodGetIMSI()
        {
            var result = await _MVNOServiceApi.GetIMSI(orderId);
            Console.WriteLine(JsonConvert.SerializeObject(result));

            Assert.AreEqual("00000", result.returnCode);
        }

        /// <summary>
        /// 订单查询
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestMethodQueryOrder()
        {
            var result = await _MVNOServiceApi.QueryOrder(userId, orderId);
            Console.WriteLine(JsonConvert.SerializeObject(result));

            Assert.AreEqual("00000", result.returnCode);
        }

        /// <summary>
        /// 消息订阅
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestMethodSubscribeMsg()
        {
            MsgReceiver[] msgReceiverList = new MsgReceiver[] { 
                new MsgReceiver() { 
                    messageId = MessageIdType.登网通知,
                    receiver="http://apitest.unitoys.com/",
                    receiveType=1
                } 
            };
            var result = await _MVNOServiceApi.SubscribeMsg(msgReceiverList);
            Console.WriteLine(JsonConvert.SerializeObject(result));

            Assert.AreEqual("00000", result.returnCode);
        }

        /// <summary>
        /// 消息退订
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestMethodCancelMsg()
        {
            MsgReceiver[] msgReceiverList = new MsgReceiver[] { 
                new MsgReceiver() { 
                    messageId = MessageIdType.登网通知,
                    receiver="http://apitest.unitoys.com/",
                    receiveType=1
                } 
            };
            var result = await _MVNOServiceApi.CancelMsg(msgReceiverList);
            Console.WriteLine(JsonConvert.SerializeObject(result));

            Assert.AreEqual("00000", result.returnCode);
        }

        /// <summary>
        /// 退款
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestMethodRefundOrder()
        {
            var result = await _MVNOServiceApi.RefundOrder(userId, orderId, DateTime.Now.ToString("yyyy-MM-dd"));
            Console.WriteLine(JsonConvert.SerializeObject(result));

            Assert.AreEqual("00000", result.returnCode);
        }
    }
}
