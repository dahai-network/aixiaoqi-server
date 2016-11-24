using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Core
{
    //委托
    public delegate void CtSocketEventHandler(object sender);
    public delegate void CtDataRecvHandler(object sender, byte[] data);
    public delegate void CtErrorEventHandler(object sender, SocketException error);
    public class SocketClient
    {
        public event CtSocketEventHandler OnConnected = null;
        public event CtSocketEventHandler OnDisConnected = null;
        public event CtDataRecvHandler OnDataRecv = null;
        public event CtErrorEventHandler OnError = null;

        private Socket _Skt = null;
        private byte[] _DataBuffer;

        public SocketClient()
        {
            this._DataBuffer = new byte[SktConfig.iBufferSize];
        }
        /// <summary>
        /// 建立socket连接
        /// </summary>
        /// <param name="ip">IP</param>
        /// <param name="port">端口</param>
        public void Connect(string ip, int port)
        {
            this._Skt = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse(ip), port);
            this._Skt.BeginConnect(iep, new AsyncCallback(Connected), this._Skt);
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="buffer">数据内容</param>
        public void Send(byte[] buffer)
        {
            List<byte> ListBytes = new List<byte>();

            #region # tcp包头 #
            //包的长度
            ListBytes.AddRange(BitConverter.GetBytes((UInt32)(IPAddress.HostToNetworkOrder(buffer.Length + 10))));
            //版本号
            ListBytes.AddRange(BitConverter.GetBytes((ushort)(1)));
            //预留
            ListBytes.AddRange(BitConverter.GetBytes((UInt32)(IPAddress.HostToNetworkOrder(0))));
            #endregion
            
            //包体
            ListBytes.AddRange(buffer);
            try
            {
                _Skt.BeginSend(ListBytes.ToArray(),
                    0,
                    ListBytes.ToArray().Length,
                    SocketFlags.None,
                    new AsyncCallback(HandleSendData),
                    _Skt);
            }
            catch (ObjectDisposedException)
            {
                NotifyDisConnectedEvent();
            }
            catch (SocketException x)
            {
                NotifyErrorEvent(x);
            }
        }
        /// <summary>
        /// 关闭socket连接
        /// </summary>
        public void Close()
        {
            try
            {
                if (_Skt != null)
                {
                    _Skt.Close();
                    NotifyDisConnectedEvent();
                }
            }
            catch 
            {
               
            }
        }

        private void Connected(IAsyncResult iar)
        {
            Socket socket = (Socket)iar.AsyncState;
            try
            {
                socket.EndConnect(iar);
            }
            catch (SocketException x)
            {
                this.NotifyErrorEvent(x);
                return;
            }
            if (socket.Connected)
            {
                //触发连接建立事件
                this.NotifyConnectedEvent();
                //开始接收数据
                this.BeginRecvData(socket);
            }
        }
        private void BeginRecvData(Socket skt)
        {
            skt.BeginReceive(_DataBuffer,
                0,
                SktConfig.iBufferSize,
                SocketFlags.None,
                new AsyncCallback(HandleRecvData),
                skt);
        }
        private void HandleRecvData(IAsyncResult parameter)
        {
            try
            {
                Socket remote = (Socket)parameter.AsyncState;
                int read = remote.EndReceive(parameter);
                if (read == 0)
                {
                    NotifyDisConnectedEvent();//表示正常的断开
                }
                else
                {
                    byte[] packLen = new byte[4];
                    Array.Copy(_DataBuffer, 0, packLen, 0, 4);
                    int len = (int)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(packLen, 0));

                    //效验是否是一个完整的包
                    if (read == len)
                    {
                        //包头效验通过，发送包体到回调函数
                        byte[] btmp = new byte[read];
                        Array.Copy(_DataBuffer, 10, btmp, 0, read);
                        this.NofifyDataRecvEvent(btmp);
                    }
                    else
                    {
                        //否者回传空数组。
                        this.NofifyDataRecvEvent(new byte[0]);
                    }
                    
                }
            }
            catch (ObjectDisposedException)
            {
                NotifyDisConnectedEvent();
            }
            catch (SocketException x)
            {
                if (x.ErrorCode == 10054)
                {
                    NotifyDisConnectedEvent();
                }
                else
                {
                    NotifyErrorEvent(x);
                }
            }
        }
        private void HandleSendData(IAsyncResult parameter)
        {
            Socket socket = (Socket)parameter.AsyncState;
            socket.EndSend(parameter);
        }
        private void NotifyConnectedEvent()
        {
            if (OnConnected != null) { OnConnected(this); }
        }
        private void NofifyDataRecvEvent(byte[] data)
        {
            if (OnDataRecv != null) { OnDataRecv(this, data); }
        }
        private void NotifyDisConnectedEvent()
        {
            if (OnDisConnected != null) { OnDisConnected(this); }
        }
        private void NotifyErrorEvent(SocketException error)
        {
            if (OnError != null) { OnError(this, error); }
        }
    }
}
