using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Foundation.Databinding;
using GameManager;
using UnityEngine;
using XLua;

namespace GameSystem
{
    public class LuaObservableBehaviour : MonoBehaviour, IObservableModel
    {
        [HideInInspector]
        public TextAsset _luaText;
        [HideInInspector]
        public string _luaTextPath;

        private Action awakeMethod, startMethod, onenableMethod, ondisableMethod, ondestroyMethod, updateMethod;

        

        private Action<ObservableMessage> _onBindingEvent = delegate { };
        public event Action<ObservableMessage> OnBindingUpdate
        {
            add
            {
                _onBindingEvent = (Action<ObservableMessage>)Delegate.Combine(_onBindingEvent, value);
            }
            remove
            {
                _onBindingEvent = (Action<ObservableMessage>)Delegate.Remove(_onBindingEvent, value);
            }
        }

        private LuaModelBinder _binder;

        private bool _isDisposed;

        private ObservableMessage _bindingMessage;

        private LuaModelBinder Binder
        {
            get
            {
                if (_binder == null && !_isDisposed)
                    InitBinder();
                return _binder;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool IsApplicationQuit { get; private set; }

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            if (_bindingMessage == null)
                _bindingMessage = new ObservableMessage { Sender = this };
            InitBinder();

            if (awakeMethod != null)
            {
                awakeMethod();
            }
        }

        void Start()
        {
            if (startMethod != null)
            {
                startMethod();
            }
        }

        void OnEnable()
        {
            if (onenableMethod != null)
            {
                onenableMethod();
            }
        }

        void OnDisable()
        {
            if (ondisableMethod != null)
            {
                ondisableMethod();
            }
        }

        protected virtual void OnDestroy()
        {
            if (IsApplicationQuit)
                return;

            if (ondestroyMethod != null)
            {
                ondestroyMethod();
            }

            awakeMethod = null;
            startMethod = null;
            onenableMethod = null;
            ondisableMethod = null;
            ondestroyMethod = null;
            updateMethod = null;

            Dispose();
        }

        void Update()
        {
            if (updateMethod != null)
            {
                updateMethod();
            }
        }

        protected void InitBinder()
        {
            if (_binder != null)
            {
                return;
            }

            string luaScript = GetLuaScript();
            if (string.IsNullOrEmpty(luaScript))
            {
                return;
            }

            LuaTable luaTable = LuaManager.Instance.LuaEnv.NewTable();
            LuaTable metaTable = LuaManager.Instance.LuaEnv.NewTable();
            metaTable.Set("__index", LuaManager.Instance.LuaEnv.Global);
            luaTable.SetMetaTable(metaTable);
            metaTable.Dispose();

            Action<string, object> action = NotifyProperty;
            luaTable.Set("notifyproperty", action);
            luaTable.Set("self", this.gameObject);
            LuaManager.Instance.LuaEnv.DoString(@"
            function set(key, value)
                _ENV[key] = value
                notifyproperty(key, value)
            end", GetType().Name, luaTable);
            LuaManager.Instance.LuaEnv.DoString(luaScript, GetType().Name, luaTable);

            luaTable.Get("awake", out awakeMethod);
            luaTable.Get("start", out startMethod);
            luaTable.Get("onenable", out onenableMethod);
            luaTable.Get("ondisable", out ondisableMethod);
            luaTable.Get("ondestroy", out ondestroyMethod);
            luaTable.Get("update", out updateMethod);
            _binder = new LuaModelBinder(this, luaTable);
        }

        public string GetLuaScript()
        {
            if (_luaText == null)
            {
                if (string.IsNullOrEmpty(_luaTextPath))
                {
                    return null;
                }
                _luaText = AssetBundles.DataLoader.Load<TextAsset>(_luaTextPath);
            }

            if (_luaText == null) return null;
            return _luaText.text;
        }

        [HideInInspector]
        public virtual void Dispose()
        {
            _isDisposed = true;

            if (_binder != null)
            {
                _binder.Dispose();
            }

            if (_bindingMessage != null)
            {
                _bindingMessage.Dispose();
            }

            _bindingMessage = null;
            _binder = null;
        }

        public void RaiseBindingUpdate(string memberName, object paramater)
        {
            if (_bindingMessage == null)
                _bindingMessage = new ObservableMessage { Sender = this };

            Binder.RaiseBindingUpdate(memberName, paramater);

            if (_onBindingEvent != null)
            {
                _bindingMessage.Name = memberName;
                _bindingMessage.Value = paramater;
                _onBindingEvent(_bindingMessage);
            }
        }

        [HideInInspector]
        public void Command(string memberName)
        {
            _binder.Command(memberName);
        }

        public void Command(string memberName, object paramater)
        {
            _binder.Command(memberName, paramater);
        }

        [HideInInspector]
        public virtual object GetValue(string memberName)
        {
            return Binder.GetValue(memberName);
        }

        public virtual object GetValue(string memberName, object paramater)
        {
            return Binder.GetValue(memberName, paramater);
        }

        public virtual void NotifyProperty(string memberName, object paramater)
        {
            RaiseBindingUpdate(memberName, paramater);
        }

        protected virtual void OnApplicationQuit()
        {
            IsApplicationQuit = true;
        }

#if !UNITY_WSA
        /// <summary>
        /// Mvvm light set method
        /// </summary>
        /// <remarks>
        /// https://github.com/NVentimiglia/Unity3d-Databinding-Mvvm-Mvc/issues/3
        /// https://github.com/negue
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueHolder"></param>
        /// <param name="value"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        protected bool Set<T>(ref T valueHolder, T value, string propName = null)
        {
            var same = EqualityComparer<T>.Default.Equals(valueHolder, value);

            if (!same)
            {
                if (string.IsNullOrEmpty(propName))
                {
                    // get call stack
                    var stackTrace = new StackTrace();
                    // get method calls (frames)
                    var stackFrames = stackTrace.GetFrames().ToList();

                    if (propName == null && stackFrames.Count > 1)
                    {
                        propName = stackFrames[1].GetMethod().Name.Replace("set_", "");
                    }
                }

                valueHolder = value;

                NotifyProperty(propName, value);

                return true;
            }

            return false;
        }
#else
        /// <summary>
        /// Mvvm light set method
        /// </summary>
        /// <remarks>
        /// https://github.com/NVentimiglia/Unity3d-Databinding-Mvvm-Mvc/issues/3
        /// https://github.com/negue
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueHolder"></param>
        /// <param name="value"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        protected bool Set<T>(ref T valueHolder, T value, [CallerMemberName] string propName = null)
        {
            var same = EqualityComparer<T>.Default.Equals(valueHolder, value);

            if (!same)
            {
                NotifyProperty(propName, value);
                valueHolder = value;
                return true;
            }

            return false;
        }
#endif
    }
}
