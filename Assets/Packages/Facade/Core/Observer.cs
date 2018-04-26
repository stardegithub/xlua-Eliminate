using System.Collections.Generic;
using System;
using UnityEngine;

public class ControllerObserver : IObserver
{
    protected Dictionary<string, Action<object>> messageMethodMap;

    public ControllerObserver()
    {
        messageMethodMap = new Dictionary<string, Action<object>>();
        ObserverMessages = new List<string>(messageMethodMap.Keys);
    }

    public void AddMethod(string messageName, Action<object> method)
    {
        messageMethodMap[messageName] = method;
    }

    public void Register()
    {
        Facade.Instance.RegisterObserver(this);
    }

    public void Remove()
    {
        Remove(new List<string>(messageMethodMap.Keys));
    }

    public void Remove(List<string> messageNames)
    {
        Facade.Instance.RemoveObserver(this, messageNames);
    }

    public virtual List<string> ObserverMessages { get; protected set; }

    public virtual void OnMessage(IMessage message)
    {
        var method = messageMethodMap[message.Name];
        if (method != null)
        {
            method(message.Body);
        }
    }
}

public class ObserverView : MonoBehaviour, IObserver
{
    public virtual List<string> ObserverMessages { get; protected set; }

    public virtual void OnMessage(IMessage message)
    {
    }
}
