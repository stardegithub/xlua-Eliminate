using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Foundation.Databinding;
using XLua;
using Common;

namespace Foundation.Databinding.Lua
{
    public class LuaObservableBehaviour : MonoBehaviourExtension, IObservableModel
    {
        const string BaseChunk = @"
            function set(key, value)
                _ENV[key] = value
                notifyproperty(key, value)
            end";

        [HideInInspector]
        public TextAsset LuaText;
        [HideInInspector]
        public string LuaTextPath;
        protected string _tableName;
        protected LuaTable _luaTable;

        protected Action awakeMethod, startMethod, onenableMethod, ondisableMethod, ondestroyMethod, updateMethod;



        protected Action<ObservableMessage> _onBindingEvent = delegate { };
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

        private LuaBinder _binder;

        private bool _isDisposed;

        private ObservableMessage _bindingMessage;

        private LuaBinder Binder
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
            if (_luaTable != null)
            {
                _luaTable.Dispose();
            }
            if (LuaManager.Instance != null)
            {
                LuaManager.Instance.RemoveTable(_tableName);
            }

            Dispose();
        }

        void Update()
        {
            if (updateMethod != null)
            {
                updateMethod();
            }
        }

        public string LoadLuaScript()
        {
            if (LuaText == null)
            {
                if (string.IsNullOrEmpty(LuaTextPath))
                {
                    return null;
                }
                LuaText = AssetBundles.DataLoader.Load<TextAsset>(LuaTextPath);
            }

            if (LuaText == null) return null;
            _tableName = LuaText.name;
            return LuaText.text;
        }

        protected void InitBinder()
        {
            if (_binder != null) { return; }

            string luaScript = LoadLuaScript();
            if (string.IsNullOrEmpty(luaScript)) { return; }

            _luaTable = LuaManager.Instance.CreateExpandTable(_tableName);
            if (_luaTable == null) { return; }

            _luaTable.Set("notifyproperty", new Action<string, object>(NotifyProperty));
            _luaTable.Set("self", this.gameObject);
            LuaManager.Instance.DoString(BaseChunk, "basechunk", _luaTable);
            LuaManager.Instance.DoString(luaScript, _tableName, _luaTable);

            _luaTable.Get("awake", out awakeMethod);
            _luaTable.Get("start", out startMethod);
            _luaTable.Get("onenable", out onenableMethod);
            _luaTable.Get("ondisable", out ondisableMethod);
            _luaTable.Get("ondestroy", out ondestroyMethod);
            _luaTable.Get("update", out updateMethod);
            _binder = new LuaBinder(this, _luaTable);
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
