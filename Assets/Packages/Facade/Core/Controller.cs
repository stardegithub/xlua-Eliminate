using System;
using System.Collections.Generic;

public class Controller : IController
{
    protected IDictionary<string, Type> _commandMap;
    protected IDictionary<IObserver, List<string>> _observerMap;

    protected static volatile IController m_instance;
    protected readonly object m_syncRoot = new object();
    protected static readonly object m_staticSyncRoot = new object();

    protected Controller()
    {
        InitializeController();
    }

    static Controller()
    {
    }

    public static IController Instance
    {
        get
        {
            if (m_instance == null)
            {
                lock (m_staticSyncRoot)
                {
                    if (m_instance == null) m_instance = new Controller();
                }
            }
            return m_instance;
        }
    }

    protected virtual void InitializeController()
    {
        _commandMap = new Dictionary<string, Type>();
        _observerMap = new Dictionary<IObserver, List<string>>();
    }

    public virtual void NotifyMessage(IMessage message)
    {
        Type commandType = null;
        List<IObserver> views = null;
        lock (m_syncRoot)
        {
            if (_commandMap.ContainsKey(message.Name))
            {
                commandType = _commandMap[message.Name];
            }
            else
            {
                views = new List<IObserver>();
                foreach (var de in _observerMap)
                {
                    if (de.Value.Contains(message.Name))
                    {
                        views.Add(de.Key);
                    }
                }
            }
        }
        if (commandType != null)
        {  //Controller
            object commandInstance = Activator.CreateInstance(commandType);
            if (commandInstance is ICommand)
            {
                ((ICommand)commandInstance).Execute(message);
            }
        }
        if (views != null && views.Count > 0)
        {
            for (int i = 0; i < views.Count; i++)
            {
                views[i].OnMessage(message);
            }
            views = null;
        }
    }

    public virtual void RegisterCommand(string messageName, Type commandType)
    {
        lock (m_syncRoot)
        {
            _commandMap[messageName] = commandType;
        }
    }

    public virtual void RegisterObserver(IObserver observer, List<string> messageNames)
    {
        lock (m_syncRoot)
        {
            if (_observerMap.ContainsKey(observer))
            {
                List<string> list = null;
                if (_observerMap.TryGetValue(observer, out list))
                {
                    for (int i = 0; i < messageNames.Count; i++)
                    {
                        if (!list.Contains(messageNames[i])) list.Add(messageNames[i]);
                    }
                }
            }
            else
            {
                _observerMap.Add(observer, messageNames);
            }
        }
    }

    public virtual void RemoveCommand(string messageName)
    {
        lock (m_syncRoot)
        {
            if (_commandMap.ContainsKey(messageName))
            {
                _commandMap.Remove(messageName);
            }
        }
    }

    public virtual void RemoveObserver(IObserver observer, List<string> messageNames)
    {
        lock (m_syncRoot)
        {
            if (_observerMap.ContainsKey(observer))
            {
                List<string> list = null;
                if (_observerMap.TryGetValue(observer, out list))
                {
                    for (int i = 0; i < messageNames.Count; i++)
                    {
                        list.Remove(messageNames[i]);
                    }
                    if (list.Count == 0)
                    {
                        _observerMap.Remove(observer);
                    }
                }
            }
        }
    }

    public virtual bool HasCommand(string messageName)
    {
        lock (m_syncRoot)
        {
            return _commandMap.ContainsKey(messageName);
        }
    }

    public virtual void ClearAll()
    {
        lock (m_syncRoot)
        {
            _commandMap.Clear();
            _observerMap.Clear();
        }
    }
}
