using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using UnitySocket;

namespace GameManager
{
    public class TcpSocketManager : GameManagerBase<TcpSocketManager>
    {
        public const int RECEIVE_BUFFER_MAX_SIZE = 8192;

        private TcpSocketClient tcpClient;
        private long tcpSocketState;
        // public int TcpSocketState { get { return tcpSocketState; } }
        private int reconnectCount;
        // public int ReconnectCount { get { return reconnectCount; } }

        private TcpSendMsg tcpSendMsg;
        private List<TcpSendMsg> sendMsgPool;
        private int sendMsgPoolState;//0空闲，1Enqueue，2Dequeue，3Peek，4Clear
        public int SendMsgCount { get { return sendMsgPool == null ? 0 : sendMsgPool.Count; } }

        private TcpReceiveMsg[] tcpReceiveMsgs;
        private ReceiveInvokeElement receiveIvokeElement;
        private Thread heartbeatTread;
        private TcpSendMsg heartbeatMsg;
        private int heartbeatInterval;
        private int maxReceiveInterval;

        public bool IsConnected { get { return tcpSocketState > 0; } }

        #region Singleton
        protected override void SingletonAwake()
        {
            sendMsgPool = new List<TcpSendMsg>();
            tcpClient = new TcpSocketClient(RECEIVE_BUFFER_MAX_SIZE);
            initialized = true;
        }

        protected override void SingletonDestroy()
        {
            if (heartbeatTread != null) heartbeatTread.Abort();
            tcpClient.CloseSocket();
        }
        #endregion

        private void LateUpdate()
        {
            if (tcpSocketState == 1)
            {
                if (receiveIvokeElement.ReleaseReceiveMsgPool(ref tcpReceiveMsgs))
                {
                    for (int i = 0; i < tcpReceiveMsgs.Length; i++)
                    {
                        tcpReceiveMsgs[i].ProcessAction();
                    }
                }

                if (!tcpClient.UserToken.TcpSocket.Connected /*|| tcpClient.ReceiveInterval > maxReceiveInterval*/)
                {
                    Interlocked.Exchange(ref tcpSocketState, 0);
                    tcpClient.UserToken.TcpSocket.Close();
                    receiveIvokeElement.SocketErrorInvoke(tcpClient.UserToken, SocketError.Shutdown);
                    UnityEngine.Debug.LogWarning("Receive TimeOut, Close TcpClient");
                }
            }
        }

        public void Init(ReceiveInvokeElement receiveInvokeElement, TcpSendMsg heartbeatMsg, int heartbeatInterval)
        {
            this.receiveIvokeElement = receiveInvokeElement;
            tcpClient.ReceiveInvokeElement = receiveInvokeElement;

            if (heartbeatMsg != null)
            {
                heartbeatMsg.Encode();
                this.heartbeatMsg = heartbeatMsg;
            }

            this.heartbeatInterval = heartbeatInterval;
            this.maxReceiveInterval = heartbeatInterval * 2;
        }

        public void ConnectServer(string hostNameOrAddress, int port, Action onConnectCompleted, Action<SocketError> onSocketError)
        {
            var invokeElement = new TcpSocketInvokeElement();
            invokeElement.OnSocketCompleted = ConnectCompleted;
            invokeElement.OnSocketCompleted += onConnectCompleted;
            invokeElement.OnSocketError = onSocketError;
            ConnectServer(hostNameOrAddress, port, invokeElement);
        }

        public void ConnectServer(string hostNameOrAddress, int port, TcpSocketInvokeElement invokeElement)
        {
#if DEBUG_NETIO
            UnityEngine.Debug.Log(string.Format("Connect :{0} {1}", hostNameOrAddress, port));
#endif
            Interlocked.Exchange(ref tcpSocketState, 0);
            tcpClient.ConnectServer(hostNameOrAddress, port, invokeElement);
        }

        public void ConnectCompleted()
        {
            UnityEngine.Debug.Log("ConnectCompleted");
            Interlocked.Exchange(ref tcpSocketState, 1);
        }

        public void StartSendHeartbeat()
        {
            if (heartbeatMsg != null)
            {
                if (heartbeatTread != null) heartbeatTread.Abort();
                heartbeatTread = new Thread(SendHeartbeat);
                heartbeatTread.Start();
            }
        }

        private void SendHeartbeat()
        {
            SocketError errorCode;
            while (true)
            {
                Thread.Sleep(heartbeatInterval);
                tcpClient.Send(heartbeatMsg.Packet, out errorCode);

                if (errorCode != SocketError.Success)
                {
                    if (Interlocked.Exchange(ref tcpSocketState, -1) != -1)
                    {
                        tcpClient.ReconnectSocket(heartbeatMsg);
                    }
                    break;
                }
            }
        }

        public void SendMessage(TcpSendMsg msg, bool immediate)
        {
            msg.Encode();
            msg.SendImmediate = immediate;

            if (Interlocked.Read(ref tcpSocketState) == 1)
            {
                if (immediate)
                {
                    tcpClient.BeginSend(msg.Packet, msg);
                }
                else
                {
                    EnqueueSendMsgPool(msg);
                    SendMsgPacket();
                }
            }
        }

        public void SendMsgCompleted(TcpSendMsg tcpSendMsg)
        {
            if (!tcpSendMsg.SendImmediate)
            {
                DequeueSendMsgPool(tcpSendMsg);
            }
            SendMsgPacket();
        }

        private void SendMsgPacket()
        {
            if (PeekSendMsgPool(ref tcpSendMsg))
            {
                // ThreadPoolManager.Instance.QueueOnU3DThread(null, delegate (object obj)
                // {
                //     UIManager.Instance.OpenNetworkWaitMask();
                // });

                tcpSendMsg.SendPacket(tcpClient);
            }
            // else
            // {
            //     ThreadPoolManager.Instance.QueueOnU3DThread(null, delegate (object obj)
            //     {
            //         UIManager.Instance.CloseNetworkWaitMask();
            //     });
            // }
        }

        private void EnqueueSendMsgPool(TcpSendMsg tcpSendMsg)
        {
            while (true)
            {
                if (Interlocked.CompareExchange(ref sendMsgPoolState, 1, 0) == 0)
                {
                    sendMsgPool.Add(tcpSendMsg);
                    sendMsgPoolState = 0;
                    return;
                }
            }
        }

        private void DequeueSendMsgPool(TcpSendMsg tcpSendMsg)
        {
            if (sendMsgPool.Count > 0)
            {
                while (true)
                {
                    if (Interlocked.CompareExchange(ref sendMsgPoolState, 2, 0) == 0)
                    {
                        sendMsgPool.Remove(tcpSendMsg);
                        sendMsgPoolState = 0;
                        return;
                    }
                }
            }
        }

        private bool PeekSendMsgPool(ref TcpSendMsg tcpSendMsg)
        {
            if (sendMsgPool.Count > 0)
            {
                while (true)
                {
                    if (Interlocked.CompareExchange(ref sendMsgPoolState, 3, 0) == 0)
                    {
                        if (sendMsgPool.Count > 0)
                        {
                            tcpSendMsg = sendMsgPool[0];
                            sendMsgPoolState = 0;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            return false;
        }

        public void ClearSendMsgPool()
        {
            while (true)
            {
                if (Interlocked.CompareExchange(ref sendMsgPoolState, 4, 0) == 0)
                {
                    sendMsgPool.Clear();
                    return;
                }
            }
        }

        private bool Reconnect(TcpSocketUserToken userToken, TcpSocketInvokeElement invokeElement, int maxCount)
        {
            if (userToken.TcpSocket != null)
            {
                if (Interlocked.Exchange(ref tcpSocketState, -1) != -1)
                {
                    if (maxCount > 0 && reconnectCount >= maxCount)
                    {
                        return false;
                    }

                    if (!(!userToken.TcpSocket.Connected || (userToken.TcpSocket.Poll(1000, SelectMode.SelectRead) && (userToken.TcpSocket.Available == 0))))
                    {
                        UnityEngine.Debug.LogWarning("Reconnect --> TcpSocket is Connected");

                        ReconnectCompleted();
                        // invokeElement.SocketCompletedInvoke(userToken);
                        SendMsgPacket();
                    }
                    else
                    {
                        UnityEngine.Debug.LogWarning("Reconnect --> TcpSocket is not Connected");

                        // invokeElement.ClearSendMsgPool();
                        if (heartbeatTread != null) heartbeatTread.Abort();
                        reconnectCount++;
                        tcpClient.ReconnectSocket(invokeElement);
                    }
                }
                return true;
            }
            return false;
        }

        public bool TryReconnect(TcpSocketUserToken userToken, TcpSocketInvokeElement invokeElement, int maxCount = 1)
        {
            return Reconnect(userToken, invokeElement, maxCount);
        }

        public bool RetryReconnect(TcpSocketInvokeElement invokeElement)
        {
            Interlocked.Exchange(ref tcpSocketState, -2);
            return Reconnect(tcpClient.UserToken, invokeElement, 0);
        }

        public void ReconnectCompleted()
        {
            ConnectCompleted();
            reconnectCount = 0;
            // StartSendHeartbeat();
            // SendMsgPacket();
        }
    }
}
