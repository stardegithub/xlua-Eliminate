using UnitySocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Net.Sockets;

namespace GameManager
{
    public class ReceiveInvokeElement : TcpSocketInvokeElement
    {
        protected MemoryStream receiveStream;
        protected BinaryReader receiveReader;
        protected int receiveMsgPoolState;//0空闲，1Enqueue，2Release
        protected Queue<TcpReceiveMsg> receiveMsgPool;
        protected ITcpReceiveMsgFactory tcpReceiveMsgFactory;

        public ReceiveInvokeElement(ITcpReceiveMsgFactory tcpReceiveMsgFactory)
        {
            receiveStream = new MemoryStream();
            receiveReader = new BinaryReader(receiveStream);
            receiveMsgPool = new Queue<TcpReceiveMsg>();
            this.tcpReceiveMsgFactory = tcpReceiveMsgFactory;
        }

        public override void SocketCompletedInvoke(TcpSocketUserToken userToken)
        {
            ProcessPacket(userToken);
            base.SocketCompletedInvoke(userToken);
            // ThreadPoolManager.Instance.QueueOnU3DThread(null, delegate (object obj)
            // {
            //     UIManager.Instance.CloseNetworkWaitMask();
            // });
        }

        protected virtual void ProcessPacket(TcpSocketUserToken userToken)
        {
            var packet = userToken.ReceiveBuffer;
            receiveStream.Seek(0, SeekOrigin.End);
            receiveStream.Write(packet, 0, userToken.ReceiveEventArgs.BytesTransferred);
            //Reset to beginning
            receiveStream.Seek(0, SeekOrigin.Begin);
            while (RemainingBytes() > 2)
            {
                byte[] msgLenByte = receiveReader.ReadBytes(2);
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(msgLenByte);
                }
                ushort len = BitConverter.ToUInt16(msgLenByte, 0);

                if (RemainingBytes() >= len)
                {
                    byte[] bytes = receiveReader.ReadBytes(len);
                    BuildReceiveMsg(bytes);
                }
                else
                {
                    //Back up the position two bytes
                    receiveStream.Position = receiveStream.Position - 2;
                    break;
                }
            }
            //Create a new stream with any leftover bytes
            byte[] leftover = receiveReader.ReadBytes((int)RemainingBytes());
            receiveStream.SetLength(0);     //Clear
            receiveStream.Write(leftover, 0, leftover.Length);
        }

        /// <summary>
        /// 剩余的字节
        /// </summary>
        protected virtual long RemainingBytes()
        {
            return receiveStream.Length - receiveStream.Position;
        }

        protected virtual void BuildReceiveMsg(byte[] bytes)
        {
            TcpReceiveMsg receiveMsg = tcpReceiveMsgFactory.Build(bytes);
            if (receiveMsg != null)
            {
                EnqueueReceiveMsgPool(receiveMsg);
            }
        }

        protected virtual void EnqueueReceiveMsgPool(TcpReceiveMsg tcpReceiveMsg)
        {
            while (true)
            {
                if (Interlocked.CompareExchange(ref receiveMsgPoolState, 1, 0) == 0)
                {
                    receiveMsgPool.Enqueue(tcpReceiveMsg);
                    receiveMsgPoolState = 0;
                    return;
                }
            }
        }

        public virtual bool ReleaseReceiveMsgPool(ref TcpReceiveMsg[] tcpReceiveMsgs)
        {
            if (receiveMsgPool.Count > 0) //只有主线程调用，判断一次
            {
                while (true)
                {
                    if (Interlocked.CompareExchange(ref receiveMsgPoolState, 2, 0) == 0)
                    {
                        tcpReceiveMsgs = receiveMsgPool.ToArray();
                        receiveMsgPool.Clear();
                        receiveMsgPoolState = 0;
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
