using System;
using System.Net;
using System.Net.Sockets;

namespace UnitySocket
{
    public class TcpSocketClient
    {
        protected Socket tcpSocket;
        protected SocketAsyncEventArgs connectEventArgs;

        public bool IsConnected { get; protected set; }
        public long SendLatency { get; protected set; }
        public long ReceiveInterval { get { return UserToken.ReceiveStopwatch.ElapsedMilliseconds; } }
        public EndPoint RemoteEndPoint { get; protected set; }
        public TcpSocketUserToken UserToken { get; protected set; }
        public TcpSocketInvokeElement ReceiveInvokeElement { get; set; }

        public TcpSocketClient(int receiveBufferSize)
        {
            UserToken = new TcpSocketUserToken(receiveBufferSize);
            UserToken.SendEventArgs.Completed += IO_Completed;
            UserToken.ReceiveEventArgs.Completed += IO_Completed;

            connectEventArgs = new SocketAsyncEventArgs();
            connectEventArgs.Completed += ConnectEventArg_Completed;
        }

        public void ConnectServer(string hostNameOrAddress, int port, TcpSocketInvokeElement invokeElement)
        {
            EndPoint remoteEndPoint = null;
            IPAddress ipAddress = null;
            if (IPAddress.TryParse(hostNameOrAddress, out ipAddress))
            {
                remoteEndPoint = new IPEndPoint(ipAddress, port);
            }
            else
            {
                IPAddress[] addressList = Dns.GetHostAddresses(hostNameOrAddress);
                if (addressList != null)
                {
                    for (int i = 0; i < addressList.Length; i++)
                    {
                        if (addressList[i].AddressFamily == AddressFamily.InterNetwork
                            || addressList[i].AddressFamily == AddressFamily.InterNetworkV6)
                        {
                            remoteEndPoint = new IPEndPoint(addressList[i], port);
                            break;
                        }
                    }
                }
            }
            ConnectServer(remoteEndPoint, invokeElement);
        }

        public void ConnectServer(EndPoint remoteEndPoint, TcpSocketInvokeElement invokeElement)
        {
            if (remoteEndPoint == null)
            {
                return;
            }

            this.RemoteEndPoint = remoteEndPoint;
            tcpSocket = new Socket(remoteEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            tcpSocket.NoDelay = true;
            UserToken.TcpSocket = tcpSocket;
            connectEventArgs.UserToken = invokeElement;
            connectEventArgs.RemoteEndPoint = remoteEndPoint;

            ResetTimer();
            bool willRaiseEvent = tcpSocket.ConnectAsync(connectEventArgs);
            if (!willRaiseEvent)
            {
                ProcessConnect(connectEventArgs);
            }
        }

        protected void ConnectEventArg_Completed(object sender, SocketAsyncEventArgs connectEventArgs)
        {
            ProcessConnect(connectEventArgs);
        }

        protected void ProcessConnect(SocketAsyncEventArgs connectEventArgs)
        {
            if (connectEventArgs.SocketError == SocketError.Success || connectEventArgs.SocketError == SocketError.IsConnected)
            {
                UserToken.ConnectDateTime = DateTime.Now;

                if (connectEventArgs.UserToken != null)
                {
                    var invokeElement = connectEventArgs.UserToken as TcpSocketInvokeElement;
                    invokeElement.SocketCompletedInvoke(UserToken);
                }

                bool willRaiseEvent = UserToken.TcpSocket.ReceiveAsync(UserToken.ReceiveEventArgs); //投递接收请求
                if (!willRaiseEvent)
                {
                    lock (UserToken)
                    {
                        ProcessReceive(UserToken.ReceiveEventArgs);
                    }
                }
            }
            else
            {
                if (connectEventArgs.UserToken != null)
                {
                    var invokeElement = connectEventArgs.UserToken as TcpSocketInvokeElement;
                    invokeElement.SocketErrorInvoke(UserToken, connectEventArgs.SocketError);
                }
            }
        }

        protected void IO_Completed(object sender, SocketAsyncEventArgs asyncEventArgs)
        {
            var userToken = asyncEventArgs.UserToken as TcpSocketUserToken;
            lock (userToken)
            {
                if (asyncEventArgs.LastOperation == SocketAsyncOperation.Receive)
                {
                    ProcessReceive(asyncEventArgs);
                }
                else if (asyncEventArgs.LastOperation == SocketAsyncOperation.Send)
                {
                    ProcessSend(asyncEventArgs);
                }
            }
        }

        protected void ProcessReceive(SocketAsyncEventArgs receiveEventArgs)
        {
            var userToken = receiveEventArgs.UserToken as TcpSocketUserToken;

            if (userToken.ReceiveEventArgs.SocketError == SocketError.Success)
            {
                // UnityEngine.Debug.Log("receive packet:" + userToken.ReceiveEventArgs.BytesTransferred);

                if (ReceiveInvokeElement != null && userToken.ReceiveEventArgs.BytesTransferred > 0)
                {
                    userToken.ReceiveStopwatch.Reset();
                    userToken.ReceiveStopwatch.Start();

                    try
                    {
                        ReceiveInvokeElement.SocketCompletedInvoke(userToken);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError("Process Receive Exception:" + e.ToString());
                    }
                }
                bool willRaiseEvent = userToken.TcpSocket.ReceiveAsync(userToken.ReceiveEventArgs); //投递接收请求
                if (!willRaiseEvent)
                {
                    ProcessReceive(userToken.ReceiveEventArgs);
                }
            }
            else
            {
                if (ReceiveInvokeElement != null)
                {
                    ReceiveInvokeElement.SocketErrorInvoke(userToken, userToken.ReceiveEventArgs.SocketError);
                }
            }
        }

        protected void ProcessSend(SocketAsyncEventArgs sendEventArgs)
        {
            var userToken = sendEventArgs.UserToken as TcpSocketUserToken;
            userToken.UnLockSendState();
            if (sendEventArgs.SocketError == SocketError.Success)
            {
                if (userToken.SendInvokeElement != null)
                {
                    userToken.SendInvokeElement.SocketCompletedInvoke(userToken);
                }
                userToken.SendStopwatch.Stop();
                SendLatency = userToken.SendStopwatch.ElapsedMilliseconds;
            }
            else
            {
                if (userToken.SendInvokeElement != null)
                {
                    userToken.SendInvokeElement.SocketErrorInvoke(userToken, userToken.ReceiveEventArgs.SocketError);
                }
            }
        }

        public bool SendAsync(byte[] buffer, TcpSocketInvokeElement invokeElement)
        {
            if (UserToken.TcpSocket == null || !UserToken.LockSendState()) return false;
            if (!UserToken.TcpSocket.Connected)
            {
                invokeElement.SocketErrorInvoke(UserToken, SocketError.Shutdown);
                return false;
            }
            UserToken.SendInvokeElement = invokeElement;
            UserToken.SendStopwatch.Reset();
            UserToken.SendStopwatch.Start();

            UserToken.SendEventArgs.SetBuffer(buffer, 0, buffer.Length);
            bool willRaiseEvent = UserToken.TcpSocket.SendAsync(UserToken.SendEventArgs);
            if (!willRaiseEvent)
            {
                ProcessSend(UserToken.SendEventArgs);
            }
            return true;
        }

        public void BeginSend(byte[] buffer, TcpSocketInvokeElement invokeElement)
        {
            if (UserToken.TcpSocket == null) return;
            if (!UserToken.TcpSocket.Connected)
            {
                invokeElement.SocketErrorInvoke(UserToken, SocketError.Shutdown);
                return;
            }
            TcpSocketSendState sendState = new TcpSocketSendState(UserToken.TcpSocket);
            sendState.SendInvokeElement = invokeElement;
            sendState.SendStopwatch.Start();
            sendState.Buffer = buffer;
            UserToken.TcpSocket.BeginSend(sendState.Buffer, sendState.CurrentLength, sendState.TargetLength, SocketFlags.None, out sendState.errorCode, new AsyncCallback(EndSend), sendState);
        }

        private void EndSend(IAsyncResult ar)
        {
            TcpSocketSendState sendState = ar.AsyncState as TcpSocketSendState;
            int length = UserToken.TcpSocket.EndSend(ar);
            sendState.CurrentLength += length;

            if (sendState.errorCode == SocketError.Success)
            {
                if (sendState.CurrentLength < sendState.TargetLength)
                {
                    UserToken.TcpSocket.BeginSend(sendState.Buffer, sendState.CurrentLength, sendState.TargetLength, SocketFlags.None, out sendState.errorCode, new AsyncCallback(EndSend), sendState);
                }
                else
                {
                    if (sendState.SendInvokeElement != null)
                    {
                        sendState.SendInvokeElement.SocketCompletedInvoke(UserToken);
                        sendState.SendStopwatch.Stop();
                        SendLatency = sendState.SendStopwatch.ElapsedMilliseconds;
                    }
                }
            }
            else
            {
                if (sendState.SendInvokeElement != null)
                {
                    sendState.SendInvokeElement.SocketErrorInvoke(UserToken, sendState.errorCode);
                }
            }
        }

        public void Send(byte[] buffer, out SocketError errorCode)
        {
            if (UserToken.TcpSocket == null)
            {
                errorCode = SocketError.NotConnected;
                return;
            }
            UserToken.TcpSocket.Send(buffer, 0, buffer.Length, SocketFlags.None, out errorCode);
        }

        public void ReconnectSocket(TcpSocketInvokeElement invokeElement)
        {
            UserToken.TcpSocket.Close();
            ConnectServer(RemoteEndPoint, invokeElement);
        }

        public void ResetTimer()
        {
            UserToken.SendStopwatch.Reset();
            UserToken.ReceiveStopwatch.Reset();
            UserToken.ReceiveStopwatch.Start();
        }

        public void CloseSocket()
        {
            if (UserToken.TcpSocket == null) return;
            //userToken.TcpSocket.Shutdown(SocketShutdown.Both);
            UserToken.TcpSocket.Close();
            UserToken.TcpSocket = null; //释放引用，并清理缓存，包括释放协议对象等资源
            UserToken.ReceiveEventArgs.Dispose();
            UserToken.ReceiveEventArgs = null;
            UserToken.SendEventArgs.Dispose();
            UserToken.SendEventArgs = null;
            UserToken.SendStopwatch.Reset();
            UserToken.ReceiveStopwatch.Reset();
        }
    }
}
