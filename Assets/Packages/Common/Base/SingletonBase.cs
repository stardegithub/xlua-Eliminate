using UnityEngine;
using System.Collections;

namespace Common
{

    /// <summary>
    /// 单例模版类基类.
    /// </summary>
    public class SingletonBase<T> : ClassExtension where T : class, new()
    {
        protected static T _instance = default(T);
        public static T Instance
        {
            get
            {
                if (null == _instance)
                {
                    _instance = new T();
                }
                if (null != _instance)
                {
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
            this.Info("Init()");
        }

        /// <summary>
        /// 初始化.
        /// </summary>
        public virtual void Reset()
        {
            this.Info("Reset()");
        }
    }

    /// <summary>
    /// 单例脚本模版类基类.
    /// </summary>
    public class SingletonMonoBehaviourBase : MonoBehaviourExtension
    {
        protected virtual void SingletonAwake() { }
        protected virtual void SingletonDestroy() { }
    }

    /// <summary>
    /// 单例脚本模版类基类.
    /// </summary>
    public class SingletonMonoBehaviourBase<T> : MonoBehaviourExtension where T : SingletonMonoBehaviourBase<T>
    {
        private static T _instance;
        public static T Instance { get { return _instance; } }

		/// <summary>
		/// 初始化标志位.
		/// </summary>
		protected bool _initialized = false;

		/// <summary>
		/// 初始化标志位.
		/// </summary>
		public bool Initialized
		{
			get
			{
				return _initialized;
			}
		}

        private void Awake()
        {
            if (_instance != null)
            {
                this.Error("duplicate singleton:{0}, current:{1}, new:{2}, destroy new", 
					typeof(T), _instance.transform.GetInstanceID(), transform.GetInstanceID());
                Destroy(this);
                return;
            }
            _instance = this as T;
            SingletonAwake();
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                SingletonDestroy();
                _instance = null;
            }
        }
			
#region virtual

		protected virtual void SingletonAwake() { 
			this.Info ("SingletonAwake()");
		}
		protected virtual void SingletonDestroy() { 
			this.Info ("SingletonDestroy()");
		}

#endregion

	}
}
