using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Unitoys.Core
{
    public class DeviceScoketClient
    {
        public static Dictionary<string, string> dicMsg = new Dictionary<string, string>();

        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public static SocketClient sct = null;
        public DeviceScoketClient(){}
        class GetSingleton
        {
            static GetSingleton() { }
            internal static readonly DeviceScoketClient instance = new DeviceScoketClient();
        }
        public static DeviceScoketClient SendInsertUserSocket
        {
            get
            {
                sct = new SocketClient();
                sct.OnConnected += (object sender) =>
                {
                    Object obj = JsonConvert.DeserializeObject("{ \"msgType\":\"device_add\",\"msgId\":101,\"deviceType\":1,\"ip\":\"192.168.1.123\",\"userName\":\"goip.71\",\"userPwd\":\"123456\"}");
                    byte[] Message = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
                    
                    sct.Send(Message);
                };
                sct.OnDataRecv += (object sender, byte[] data) =>
                {
                    if (data.Length > 0)
                    {
                        if (dicMsg.ContainsKey("InsertUser"))
                        {
                            dicMsg.Remove("InsertUser");
                        }
                        dicMsg.Add("InsertUser", Encoding.Default.GetString(data));

                        allDone.Set();
                    }
                };
                return GetSingleton.instance;
            }
        }

        public bool Start(string IP, int port)
        {
            try
            {
                sct.OnDisConnected += sct_OnDisConnected;
                sct.Connect(IP, port);
                return allDone.WaitOne(8000);
            }
            catch
            {
                return false;
            }
        }
        static void sct_OnDisConnected(object sender)
        {

        }
       
    }
    [Serializable]
    public struct Operator
    {
        public string key;

        public string value;

        public Operator(string key, string value) // 初始化
        {
            this.key = key;
            this.value = value;
        }
    }
}
