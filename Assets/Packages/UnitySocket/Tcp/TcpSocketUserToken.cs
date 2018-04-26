using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;

namespace UnitySocket
{
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

        protected Stopwatch sendStopwatch;
        public Stopwatch SendStopwatch { get { return sendStopwatch; } set { sendStopwatch = value; } }

        protected Stopwatch receiveStopwatch;
        public Stopwatch ReceiveStopwatch { get { return receiveStopwatch; } set { receiveStopwatch = value; } }

        public TcpSocketUserToken(int receiveBufferSize)
        {
            receiveEventArgs = new SocketAsyncEventArgs();
            receiveEventArgs.UserToken = this;
            receiveBuffer = new byte[receiveBufferSize];
            receiveEventArgs.SetBuffer(receiveBuffer, 0, receiveBufferSize);

            sendEventArgs = new SocketAsyncEventArgs();
            sendEventArgs.UserToken = this;

            sendStopwatch = new Stopwatch();
            ReceiveStopwatch = new Stopwatch();
        }

        public bool LockSendState()
        {
            return Interlocked.CompareExchange(ref sendState, 1, 0) == 0;
        }

        public bool UnLockSendState()
        {
            return Interlocked.Exchange(ref sendState, 0) == 1;
        }
    }
}
