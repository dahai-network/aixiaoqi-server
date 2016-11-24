using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Core
{
    public class SktConfig
    {
        public static int iBufferSize = 1024000; //客户端/服务端 接收缓冲区大小(字节)
        public static int iMaxPktSize = 1024 * 1024 * 2;//包的最大大小
        public static int iAliveTime = 1000 * 30; //连接存活时间(毫秒)
        public static int IDPoolSize = 500;         //ID号池的大小
    }
}
