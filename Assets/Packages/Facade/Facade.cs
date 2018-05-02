using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 外壳类Facade
/// </summary>
public class Facade
{
    protected IController m_controller;

    private static Facade instance;

    /// <summary>
    /// 单例
    /// </summary>
    /// <returns></returns>
    public static Facade Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Facade();
            }
            return instance;
        }
    }

    protected Facade()
    {
        if (m_controller != null) return;
        m_controller = Controller.Instance;
    }

    /// <summary>
    /// 注册事件命令，收到消息执行对应命令
    /// </summary>
    /// <param name="messageName">消息名字</param>
    /// <param name="commandType">命令类</param>
    public virtual void RegisterCommand(string messageName, Type commandType)
    {
        m_controller.RegisterCommand(messageName, commandType);
    }

    /// <summary>
    /// 注册多个事件命令，收到消息执行对应命令
    /// </summary>
    /// <param name="commandType">命令类集合</param>
    /// <param name="messageNames">消息名字</param>
    public void RegisterMultiCommand(Type commandType, params string[] messageNames)
    {
        int count = messageNames.Length;
        for (int i = 0; i < count; i++)
        {
            RegisterCommand(messageNames[i], commandType);
        }
    }

    /// <summary>
    /// 注册观察者，监听默认消息，观察者监听到消息后作出反应
    /// </summary>
    /// <param name="observer">观察者</param>
    public void RegisterObserver(IObserver observer)
    {
        Controller.Instance.RegisterObserver(observer, observer.ObserverMessages);
    }

    /// <summary>
    /// 注册观察者，指定监听消息，观察者监听到消息后作出反应
    /// </summary>
    /// <param name="observer">观察者</param>
    /// <param name="messageNames">监听消息的名字</param>
    public void RegisterObserver(IObserver observer, List<string> messageNames)
    {
        if (messageNames == null || messageNames.Count == 0) return;
        Controller.Instance.RegisterObserver(observer, messageNames);
    }

    /// <summary>
    /// 移除事件命令
    /// </summary>
    /// <param name="messageName">消息名字</param>
    public virtual void RemoveCommand(string messageName)
    {
        m_controller.RemoveCommand(messageName);
    }

    /// <summary>
    /// 移除观察者
    /// </summary>
    /// <param name="observer">观察者</param>
    public void RemoveObserver(IObserver observer)
    {
        Controller.Instance.RemoveObserver(observer, observer.ObserverMessages);
    }

    /// <summary>
    /// 移除观察者的监听消息
    /// </summary>
    /// <param name="observer">观察者</param>
    /// <param name="messageNames">监听消息的名字</param>
    public void RemoveObserver(IObserver observer, List<string> messageNames)
    {
        if (messageNames == null || messageNames.Count == 0) return;
        Controller.Instance.RemoveObserver(observer, messageNames);
    }

    /// <summary>
    /// 是否有对应消息的事件命令
    /// </summary>
    /// <param name="messageName"></param>
    /// <returns></returns>
    public virtual bool HasCommand(string messageName)
    {
        return m_controller.HasCommand(messageName);
    }

    /// <summary>
    /// 移除多个事件命令
    /// </summary>
    /// <param name="messageName">消息名字</param>
    public void RemoveMultiCommand(params string[] messageNames)
    {
        int count = messageNames.Length;
        for (int i = 0; i < count; i++)
        {
            RemoveCommand(messageNames[i]);
        }
    }

    /// <summary>
    /// 通知消息，注册了对应消息的事件命令执行，监听对应消息的观察者作出反应
    /// </summary>
    /// <param name="messageName">消息名字</param>
    /// <param name="body">消息体</param>
    public void NotifyMessage(string messageName, object body = null)
    {
        m_controller.NotifyMessage(new Message(messageName, body));
    }

    /// <summary>
    /// 清理事件命令池，观察者池
    /// </summary>
    public void ClearAll()
    {
        m_controller.ClearAll();
    }
}
