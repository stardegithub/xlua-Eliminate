using System;
using System.Net.Sockets;

namespace UnitySocket
{
    public class TcpSocketInvokeElement
    {
        public virtual Action OnSocketCompleted { get; set; }

        public virtual Action<SocketError> OnSocketError { get; set; }

        public virtual void SocketCompletedInvoke(TcpSocketUserToken userToken)
        {
            if (OnSocketCompleted != null)
            {
                OnSocketCompleted();
            }
        }

        public virtual void SocketErrorInvoke(TcpSocketUserToken userToken, SocketError socketError)
        {
            if (OnSocketError != null)
            {
                OnSocketError(socketError);
            }
        }
    }
}
