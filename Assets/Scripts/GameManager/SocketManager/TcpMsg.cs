using UnityEngine;
using System.Collections;
using UnitySocket;
using System.Net.Sockets;

namespace GameManager
{
    public interface ITcpReceiveMsgFactory
    {
        TcpReceiveMsg Build(byte[] bytes);
    }

    public abstract class TcpReceiveMsg
    {
        public virtual bool Decode(byte[] bytes)
        {
            string jsonStr = System.Text.Encoding.UTF8.GetString(bytes);
            JsonUtility.FromJsonOverwrite(jsonStr, this);
            return true;
        }

        public virtual bool ProcessAction()
        {
            Debug.LogError(GetType() + " does not implement");
            return true;
        }
    }

    public abstract class TcpSendMsg : TcpSocketInvokeElement
    {
        public int SendCount { get; set; }

        public byte[] Packet { get; set; }

        public bool SendImmediate { get; set; }


        public virtual bool Encode()
        {
            string jsonData = JsonUtility.ToJson(this);
            Packet = System.Text.Encoding.UTF8.GetBytes(jsonData);
            return true;
        }

        public virtual bool SendPacket(TcpSocketClient tcpClient)
        {
            bool result = tcpClient.SendAsync(Packet, this);
            if (result) SendCount++;
            return result;
        }

        public override void SocketCompletedInvoke(TcpSocketUserToken userToken)
        {
            TcpSocketManager.Instance.SendMsgCompleted(this);
            base.SocketCompletedInvoke(userToken);
        }
    }
}
