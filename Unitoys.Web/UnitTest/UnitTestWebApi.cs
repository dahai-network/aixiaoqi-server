using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Generic;

namespace UnitTest
{
    [TestClass]
    public class UnitTestWebApi:IDisposable
    {
        private HttpClient _httClient;

        public UnitTestWebApi()
        {
            _httClient = new HttpClient();
            _httClient.BaseAddress = new Uri("http://localhost:12457");
        }
        public void Dispose()
        {
            _httClient.Dispose();
        }
        [TestMethod]
        public async Task TestMethodClientApiPutDevice()
        {
            var paramlist = new List<KeyValuePair<string, string>>();
            paramlist.Add(new KeyValuePair<string, string>("msgType", "reg"));
            paramlist.Add(new KeyValuePair<string, string>("msgId", "100"));
            paramlist.Add(new KeyValuePair<string, string>("deviceStatus", "1"));
            paramlist.Add(new KeyValuePair<string, string>("deviceType", "goip"));
            paramlist.Add(new KeyValuePair<string, string>("mac", "00-30-F1-00-01-C1"));
            paramlist.Add(new KeyValuePair<string, string>("did", "1"));
            paramlist.Add(new KeyValuePair<string, string>("version", "516-467-808-041-100-000"));
            paramlist.Add(new KeyValuePair<string, string>("status", "0000000010000100"));
            paramlist.Add(new KeyValuePair<string, string>("maxPorts", "16"));
            paramlist.Add(new KeyValuePair<string, string>("ip", "192.168.1.41"));
            paramlist.Add(new KeyValuePair<string, string>("userName", "goip16.terry"));

            var response = await _httClient.PutAsync("/api/clientapi/putdevice", new FormUrlEncodedContent(paramlist));
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
