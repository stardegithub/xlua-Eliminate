using System;
using System.Collections.Generic;

/// <summary>
/// 控制类接口
/// </summary>
public interface IController
{
    /// <summary>
    /// 注册事件命令，收到消息执行对应命令
    /// </summary>
    /// <param name="messageName">消息名字</param>
    /// <param name="commandType">命令类</param>
    void RegisterCommand(string messageName, Type commandType);

    /// <summary>
    /// 注册观察者，观察者监听到消息后作出反应
    /// </summary>
    /// <param name="observer">观察者</param>
    /// <param name="messageNames">监听消息的名字</param>
    void RegisterObserver(IObserver observer, List<string> messageNames);

    /// <summary>
    /// 通知消息，注册了对应消息的事件命令执行，监听对应消息的观察者作出反应
    /// </summary>
    /// <param name="message">消息</param>
    void NotifyMessage(IMessage message);

    /// <summary>
    /// 移除事件命令
    /// </summary>
    /// <param name="messageName">消息名字</param>
    void RemoveCommand(string messageName);

    /// <summary>
    /// 移除观察者
    /// </summary>
    /// <param name="observer">观察者</param>
    void RemoveObserver(IObserver observer, List<string> messageNames);

    /// <summary>
    /// 是否有对应消息的事件命令
    /// </summary>
    /// <param name="messageName"></param>
    /// <returns></returns>
    bool HasCommand(string messageName);

    /// <summary>
    /// 清理
    /// </summary>
    void ClearAll();
}
