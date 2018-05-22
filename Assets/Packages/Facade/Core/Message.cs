using System;

namespace Facade.Core {
	public class Message : IMessage
	{
		public Message(string name) : this(name, null)
		{
		}

		public Message(string name, object body)
		{
			_name = name;
			_body = body;
		}

		/// <summary>
		/// 消息转字符串
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string msg = "Notification Name: " + Name;
			msg += "\nBody:" + ((Body == null) ? "null" : Body.ToString());
			return msg;
		}

		/// <summary>
		/// 消息名字
		/// </summary>
		/// <returns></returns>
		public virtual string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// 消息体
		/// </summary>
		/// <returns></returns>
		public virtual object Body
		{
			get
			{
				// Setting and getting of reference types is atomic, no need to lock here
				return _body;
			}
			set
			{
				// Setting and getting of reference types is atomic, no need to lock here
				_body = value;
			}
		}

		/// <summary>
		/// The name of the notification instance
		/// </summary>
		private string _name;

		/// <summary>
		/// The body of the notification instance
		/// </summary>
		private object _body;
	}

}