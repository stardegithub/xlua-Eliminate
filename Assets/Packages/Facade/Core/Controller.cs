using System;
using System.Collections.Generic;

namespace Facade.Core {
	/// <summary>
	/// 控制器，核心类
	/// </summary>
	public class Controller : IController
	{
		protected IDictionary<string, Type> _commandMap;
		protected IDictionary<IObserver, List<string>> _observerMap;

		protected static volatile IController _instance;
		protected readonly object _syncRoot = new object();
		protected static readonly object _staticSyncRoot = new object();

		protected Controller()
		{
			InitializeController();
		}

		static Controller()
		{
		}

		/// <summary>
		/// 单例
		/// </summary>
		/// <returns></returns>
		public static IController Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (_staticSyncRoot)
					{
						if (_instance == null) _instance = new Controller();
					}
				}
				return _instance;
			}
		}

		/// <summary>
		/// 初始化
		/// </summary>
		protected virtual void InitializeController()
		{
			_commandMap = new Dictionary<string, Type>();
			_observerMap = new Dictionary<IObserver, List<string>>();
		}

		/// <summary>
		/// 通知消息，注册了对应消息的事件命令执行，监听对应消息的观察者作出反应
		/// </summary>
		/// <param name="message">消息</param>
		public virtual void NotifyMessage(IMessage message)
		{
			Type commandType = null;
			List<IObserver> views = null;
			lock (_syncRoot)
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

		/// <summary>
		/// 注册事件命令，收到消息执行对应命令
		/// </summary>
		/// <param name="messageName">消息名字</param>
		/// <param name="commandType">命令类</param>
		public virtual void RegisterCommand(string messageName, Type commandType)
		{
			lock (_syncRoot)
			{
				_commandMap[messageName] = commandType;
			}
		}

		/// <summary>
		/// 注册观察者，观察者监听到消息后作出反应
		/// </summary>
		/// <param name="observer">观察者</param>
		/// <param name="messageNames">监听消息的名字</param>
		public virtual void RegisterObserver(IObserver observer, List<string> messageNames)
		{
			lock (_syncRoot)
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

		/// <summary>
		/// 移除事件命令
		/// </summary>
		/// <param name="messageName">消息名字</param>
		public virtual void RemoveCommand(string messageName)
		{
			lock (_syncRoot)
			{
				if (_commandMap.ContainsKey(messageName))
				{
					_commandMap.Remove(messageName);
				}
			}
		}

		/// <summary>
		/// 移除观察者
		/// </summary>
		/// <param name="observer">观察者</param>
		public virtual void RemoveObserver(IObserver observer, List<string> messageNames)
		{
			lock (_syncRoot)
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

		/// <summary>
		/// 是否有对应消息的事件命令
		/// </summary>
		/// <param name="messageName"></param>
		/// <returns></returns>
		public virtual bool HasCommand(string messageName)
		{
			lock (_syncRoot)
			{
				return _commandMap.ContainsKey(messageName);
			}
		}

		/// <summary>
		/// 清理事件命令池，观察者池
		/// </summary>
		public virtual void ClearAll()
		{
			lock (_syncRoot)
			{
				_commandMap.Clear();
				_observerMap.Clear();
			}
		}
	}
}
