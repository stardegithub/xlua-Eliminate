using System;
using System.Diagnostics;
using System.Net.Sockets;

namespace UnitySocket
{
    /// <summary>
    /// 异步发送
    /// </summary>
    public class TcpSocketSendState
    {
        protected Socket tcpSocket;
        public Socket TcpSocket { get; protected set; }

        protected byte[] buffer;
        public byte[] Buffer
        {
            get
            {
                return buffer;
            }
            set
            {
                buffer = value;
                TargetLength = value.Length;
            }
        }

        public SocketError errorCode;

        public int TargetLength { get; protected set; }
        public int CurrentLength { get; set; }
        public TcpSocketInvokeElement SendInvokeElement { get; set; }


        public TcpSocketSendState(Socket socket)
        {
            tcpSocket = socket;
            CurrentLength = 0;
        }
    }
}
