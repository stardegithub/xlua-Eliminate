using System;
using System.Net;
using System.Net.Sockets;
using Common;

namespace UnitySocket
{
    /// <summary>
    /// tcp socket客服端类, 基于C#的Socket实现
    /// </summary>
    public class TcpSocketClient : ClassExtension
    {
        /// <summary>
        /// tcp socket
        /// </summary>
        protected Socket tcpSocket;
        /// <summary>
        /// 异步连接操作事件
        /// </summary>
        protected SocketAsyncEventArgs connectEventArgs;

        /// <summary>
        /// 请求连接的地址
        /// </summary>
        /// <returns></returns>
        public EndPoint RemoteEndPoint { get; protected set; }
        /// <summary>
        /// 异步请求操作对象
        /// </summary>
        /// <returns></returns>
        public TcpSocketUserToken UserToken { get; protected set; }
        /// <summary>
        /// tcp接收回调元素
        /// </summary>
        /// <returns></returns>
        public TcpSocketInvokeElement ReceiveInvokeElement { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="receiveBufferSize">最大接收缓存大小</param>
        public TcpSocketClient(int receiveBufferSize)
        {
            UserToken = new TcpSocketUserToken(receiveBufferSize);
            UserToken.SendEventArgs.Completed += IO_Completed;
            UserToken.ReceiveEventArgs.Completed += IO_Completed;

            connectEventArgs = new SocketAsyncEventArgs();
            connectEventArgs.Completed += ConnectEventArg_Completed;
        }

        /// <summary>
        /// 异步连接服务器
        /// </summary>
        /// <param name="hostNameOrAddress">地址(支持ipv6)</param>
        /// <param name="port">端口</param>
        /// <param name="invokeElement">连接完成回调元素</param>
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

        /// <summary>
        /// 异步连接服务器
        /// </summary>
        /// <param name="remoteEndPoint">请求连接的地址</param>
        /// <param name="invokeElement">连接完成回调元素</param>
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

            Info("start connect server : {0}", remoteEndPoint);

            bool willRaiseEvent = tcpSocket.ConnectAsync(connectEventArgs);
            if (!willRaiseEvent)
            {
                ProcessConnect(connectEventArgs);
            }
        }

        /// <summary>
        /// 异步连接完成回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="connectEventArgs"></param>
        protected void ConnectEventArg_Completed(object sender, SocketAsyncEventArgs connectEventArgs)
        {
            Info("connect server completed: {0}", connectEventArgs.SocketError);
            ProcessConnect(connectEventArgs);
        }

        /// <summary>
        /// 连接完成处理
        /// </summary>
        /// <param name="connectEventArgs"></param>
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

        /// <summary>
        /// 异步请求（接收，发送）完成回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="asyncEventArgs"></param>
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

        /// <summary>
        /// 接收完成处理
        /// </summary>
        /// <param name="receiveEventArgs"></param>
        protected void ProcessReceive(SocketAsyncEventArgs receiveEventArgs)
        {
            var userToken = receiveEventArgs.UserToken as TcpSocketUserToken;

            if (userToken.ReceiveEventArgs.SocketError == SocketError.Success)
            {
                // Info("receive packet:" + userToken.ReceiveEventArgs.BytesTransferred);

                if (ReceiveInvokeElement != null && userToken.ReceiveEventArgs.BytesTransferred > 0)
                {
                    try
                    {
                        ReceiveInvokeElement.SocketCompletedInvoke(userToken);
                    }
                    catch (Exception e)
                    {
                        Error("Process Receive Exception: {0}", e.ToString());
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

        /// <summary>
        /// 发送完成处理
        /// </summary>
        /// <param name="sendEventArgs"></param>
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
            }
            else
            {
                Error("Process Send Error: ", sendEventArgs.SocketError);
                if (userToken.SendInvokeElement != null)
                {
                    userToken.SendInvokeElement.SocketErrorInvoke(userToken, userToken.ReceiveEventArgs.SocketError);
                }
            }
        }

        /// <summary>
        /// 完成端口的异步发送，前一步次发送没完成前，后一次不能发送，建议发送用这种方法
        /// </summary>
        /// <param name="buffer">发送数据</param>
        /// <param name="invokeElement">发送完成回调元素</param>
        /// <returns></returns>
        public bool SendAsync(byte[] buffer, TcpSocketInvokeElement invokeElement)
        {
            if (UserToken.TcpSocket == null || !UserToken.LockSendState()) return false;
            if (!UserToken.TcpSocket.Connected)
            {
                invokeElement.SocketErrorInvoke(UserToken, SocketError.Shutdown);
                return false;
            }
            UserToken.SendInvokeElement = invokeElement;

            UserToken.SendEventArgs.SetBuffer(buffer, 0, buffer.Length);
            bool willRaiseEvent = UserToken.TcpSocket.SendAsync(UserToken.SendEventArgs);
            if (!willRaiseEvent)
            {
                ProcessSend(UserToken.SendEventArgs);
            }
            return true;
        }

        /// <summary>
        /// 异步发送，前一步次发送没完成，不影响后一次发送，强连接使用，比如帧同步战斗数据发送
        /// </summary>
        /// <param name="buffer">发送数据</param>
        /// <param name="invokeElement">发送完成回调元素</param>
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
            sendState.Buffer = buffer;
            UserToken.TcpSocket.BeginSend(sendState.Buffer, sendState.CurrentLength, sendState.TargetLength, SocketFlags.None, out sendState.errorCode, new AsyncCallback(EndSend), sendState);
        }

        /// <summary>
        /// 异步发送接收结束回调
        /// </summary>
        /// <param name="ar"></param>
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
                    }
                }
            }
            else
            {
                Error("End Send Error: ", sendState.errorCode);
                if (sendState.SendInvokeElement != null)
                {
                    sendState.SendInvokeElement.SocketErrorInvoke(UserToken, sendState.errorCode);
                }
            }
        }

        /// <summary>
        /// 同步发送
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="errorCode"></param>
        public void Send(byte[] buffer, out SocketError errorCode)
        {
            if (UserToken.TcpSocket == null)
            {
                errorCode = SocketError.NotConnected;
                return;
            }
            UserToken.TcpSocket.Send(buffer, 0, buffer.Length, SocketFlags.None, out errorCode);
        }

        /// <summary>
        /// 重连
        /// </summary>
        /// <param name="invokeElement">重连完成回调元素</param>
        public void ReconnectSocket(TcpSocketInvokeElement invokeElement)
        {
            UserToken.TcpSocket.Close();
            ConnectServer(RemoteEndPoint, invokeElement);
        }

        /// <summary>
        /// 关闭
        /// </summary>
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
        }
    }
}
