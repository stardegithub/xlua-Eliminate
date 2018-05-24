using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System;
using XLua;
using Common;
using Facade.Core;
using Foundation.Databinding.Lua;

namespace Facade.Lua
{
    public class FacadeLuaObserver : ObserverBehaviour
    {
        [HideInInspector]
        public TextAsset LuaText;
        [HideInInspector]
        public string LuaTextPath;
        protected string _tableName;
        protected LuaTable _luaTable;
        protected Dictionary<string, Action<object>> _messageMethodMap = new Dictionary<string, Action<object>>();
        protected Action awakeMethod, startMethod, onenableMethod, ondisableMethod, ondestroyMethod, updateMethod;

        void Awake()
        {
            BindMethod();
            Facade.Instance.RemoveObserver(this, ObserverMessages);
            Facade.Instance.RegisterObserver(this, ObserverMessages);

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

        void OnDestroy()
        {
            if (ondestroyMethod != null)
            {
                ondestroyMethod();
            }

            Facade.Instance.RemoveObserver(this, ObserverMessages);
            _messageMethodMap = null;

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
        }

        void Update()
        {
            if (updateMethod != null)
            {
                updateMethod();
            }
        }

        protected string LoadLuaScript()
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


        protected void BindMethod()
        {
            string luaScript = LoadLuaScript();
            if (string.IsNullOrEmpty(luaScript)) { return; }

            _luaTable = LuaManager.Instance.CreateExpandTable(_tableName);
            if (_luaTable == null) { return; }

            _luaTable.Set("gameObject", this.gameObject);
            LuaManager.Instance.DoString(luaScript, _tableName, _luaTable);

            string[] messages;
            _luaTable.Get("observermessages", out messages);
            if (messages != null)
            {
                ObserverMessages = new List<string>(messages);
                foreach (var message in ObserverMessages)
                {
                    _messageMethodMap[message] = _luaTable.Get<Action<object>>(message);
                }
            }

            _luaTable.Get("awake", out awakeMethod);
            _luaTable.Get("start", out startMethod);
            _luaTable.Get("onenable", out onenableMethod);
            _luaTable.Get("ondisable", out ondisableMethod);
            _luaTable.Get("ondestroy", out ondestroyMethod);
            _luaTable.Get("update", out updateMethod);
        }

        /// <summary>
        /// 处理View消息
        /// </summary>
        /// <param name="message"></param>
        public override void OnMessage(IMessage message)
        {
            var method = _messageMethodMap[message.Name];
            if (method != null)
            {
                method(message.Body);
            }
        }
    }
}
