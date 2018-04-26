using System;
using System.Collections.Generic;
using UnityEngine;

public class Facade
{
    protected IController m_controller;

    private static Facade instance;

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

    public virtual void RegisterCommand(string messageName, Type commandType)
    {
        m_controller.RegisterCommand(messageName, commandType);
    }

    public void RegisterMultiCommand(Type commandType, params string[] messageNames)
    {
        int count = messageNames.Length;
        for (int i = 0; i < count; i++)
        {
            RegisterCommand(messageNames[i], commandType);
        }
    }

    public void RegisterObserver(IObserver observer)
    {
        Controller.Instance.RegisterObserver(observer, observer.ObserverMessages);
    }

    public void RegisterObserver(IObserver observer, List<string> messageNames)
    {
        if (messageNames == null || messageNames.Count == 0) return;
        Controller.Instance.RegisterObserver(observer, messageNames);
    }

    public virtual void RemoveCommand(string messageName)
    {
        m_controller.RemoveCommand(messageName);
    }

    public void RemoveObserver(IObserver observer)
    {
        Controller.Instance.RemoveObserver(observer, observer.ObserverMessages);
    }

    public void RemoveObserver(IObserver observer, List<string> messageNames)
    {
        if (messageNames == null || messageNames.Count == 0) return;
        Controller.Instance.RemoveObserver(observer, messageNames);
    }

    public virtual bool HasCommand(string messageName)
    {
        return m_controller.HasCommand(messageName);
    }

    public void RemoveMultiCommand(params string[] messageNames)
    {
        int count = messageNames.Length;
        for (int i = 0; i < count; i++)
        {
            RemoveCommand(messageNames[i]);
        }
    }

    public void NotifyMessage(string messageName, object body = null)
    {
        m_controller.NotifyMessage(new Message(messageName, body));
    }

    public void ClearAll()
    {
        m_controller.ClearAll();
    }
}
