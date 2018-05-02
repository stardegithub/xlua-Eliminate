using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;

namespace UnitySocket
{
    /// <summary>
    /// 异步请求操作对象(完成端口)
    /// </summary>
    public class TcpSocketUserToken
    {
        protected Socket tcpSocket;
        public Socket TcpSocket
        {
            get
            {
                return tcpSocket;
            }
            set
            {
                tcpSocket = value;
                sendEventArgs.AcceptSocket = tcpSocket;
                receiveEventArgs.AcceptSocket = tcpSocket;
            }
        }

        protected byte[] receiveBuffer;

        public byte[] ReceiveBuffer { get { return receiveBuffer; } }

        protected SocketAsyncEventArgs receiveEventArgs;
        public SocketAsyncEventArgs ReceiveEventArgs { get { return receiveEventArgs; } set { receiveEventArgs = value; } }

        protected int sendState;

        protected SocketAsyncEventArgs sendEventArgs;
        public SocketAsyncEventArgs SendEventArgs { get { return sendEventArgs; } set { sendEventArgs = value; } }

        protected TcpSocketInvokeElement sendInvokeElement;
        public TcpSocketInvokeElement SendInvokeElement { get { return sendInvokeElement; } set { sendInvokeElement = value; } }

        protected DateTime connectDateTime;
        public DateTime ConnectDateTime { get { return connectDateTime; } set { connectDateTime = value; } }

        public TcpSocketUserToken(int receiveBufferSize)
        {
            receiveEventArgs = new SocketAsyncEventArgs();
            receiveEventArgs.UserToken = this;
            receiveBuffer = new byte[receiveBufferSize];
            receiveEventArgs.SetBuffer(receiveBuffer, 0, receiveBufferSize);

            sendEventArgs = new SocketAsyncEventArgs();
            sendEventArgs.UserToken = this;
        }

        /// <summary>
        /// 用原子锁修改发送状态
        /// </summary>
        /// <returns></returns>
        public bool LockSendState()
        {
            return Interlocked.CompareExchange(ref sendState, 1, 0) == 0;
        }

        /// <summary>
        /// 用原子锁解锁发送状态
        /// </summary>
        /// <returns></returns>
        public bool UnLockSendState()
        {
            return Interlocked.Exchange(ref sendState, 0) == 1;
        }
    }
}
