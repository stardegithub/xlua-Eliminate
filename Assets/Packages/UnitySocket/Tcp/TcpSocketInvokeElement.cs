using System;
using System.Net.Sockets;

namespace UnitySocket
{
    /// <summary>
    /// tcp 异步请求完成回调元素
    /// </summary>
    public class TcpSocketInvokeElement
    {
        public virtual Action OnSocketCompleted { get; set; }

        public virtual Action<SocketError> OnSocketError { get; set; }

        /// <summary>
        /// 请求完成回调
        /// </summary>
        /// <param name="userToken"></param>
        public virtual void SocketCompletedInvoke(TcpSocketUserToken userToken)
        {
            if (OnSocketCompleted != null)
            {
                OnSocketCompleted();
            }
        }

        /// <summary>
        /// 请求错误回调
        /// </summary>
        /// <param name="userToken"></param>
        /// <param name="socketError"></param>
        public virtual void SocketErrorInvoke(TcpSocketUserToken userToken, SocketError socketError)
        {
            if (OnSocketError != null)
            {
                OnSocketError(socketError);
            }
        }
    }
}
