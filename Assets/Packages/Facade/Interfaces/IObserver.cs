using System;
using System.Collections.Generic;

/// <summary>
/// 观察者类借口
/// </summary>
public interface IObserver
{
    /// <summary>
    /// 默认监听消息
    /// </summary>
    /// <returns></returns>
    List<string> ObserverMessages { get; }

    /// <summary>
    /// 收到监听的消息，作出反应
    /// </summary>
    /// <param name="message">消息</param>
    void OnMessage(IMessage message);
}
