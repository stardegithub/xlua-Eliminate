using UnityEngine;
using System.Collections;

namespace Common {

	/// <summary>
	/// 单例模版类基类.
	/// </summary>
	public class SingletonBase<T> : ClassExtension where T : class, new()
	{
		protected static T _instance = default(T);
		public static T Instance { 
			get { 
				if (null == _instance) {
					_instance = new T ();
				}
				if (null != _instance) {
					return _instance;
				}
				return default(T); 
			} 
		}

		/// <summary>
		/// 构造函数.
		/// </summary>
		protected SingletonBase()
		{
			_instance = this as T;
			Init();
		}

		/// <summary>
		/// 初始化.
		/// </summary>
		protected virtual void Init()
		{
			this.Info ("Init()");
		}

		/// <summary>
		/// 初始化.
		/// </summary>
		public virtual void Reset()
		{
			this.Info ("Reset()");
		}
	}

	/// <summary>
	/// 单例脚本模版类基类.
	/// </summary>
	public class SingletonMonoBehaviourBase : MonoBehaviourExtension
	{
		protected virtual void SingletonAwake() {}
		protected virtual void SingletonDestroy() {}
	}
}
