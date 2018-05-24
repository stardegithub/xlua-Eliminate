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
        [SerializeField]
        protected string luaFilePath;
        protected LuaTable luaTable;
        protected Dictionary<string, Action<object>> messageMethodMap = new Dictionary<string, Action<object>>();
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
            messageMethodMap = null;

            awakeMethod = null;
            startMethod = null;
            onenableMethod = null;
            ondisableMethod = null;
            ondestroyMethod = null;
            updateMethod = null;
            if (luaTable != null)
            {
                luaTable.Dispose();
            }
            if (LuaManager.Instance != null)
            {
                LuaManager.Instance.RemoveTable(luaFilePath);
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
            if (string.IsNullOrEmpty(luaFilePath))
            {
                return null;
            }
            var ta = AssetBundles.DataLoader.Load<TextAsset>(luaFilePath);
            if (ta == null) return null;
            return ta.text;
        }

        protected void BindMethod()
        {
            string luaScript = LoadLuaScript();
            if (string.IsNullOrEmpty(luaScript)) { return; }

            luaTable = LuaManager.Instance.CreateExpandTable(luaFilePath);
            if (luaTable == null) { return; }

            luaTable.Set("gameObject", this.gameObject);
            LuaManager.Instance.DoString(luaScript, luaFilePath, luaTable);

            string[] messages;
            luaTable.Get("observermessages", out messages);
            if (messages != null)
            {
                ObserverMessages = new List<string>(messages);
                foreach (var message in ObserverMessages)
                {
                    messageMethodMap[message] = luaTable.Get<Action<object>>(message);
                }
            }

            luaTable.Get("awake", out awakeMethod);
            luaTable.Get("start", out startMethod);
            luaTable.Get("onenable", out onenableMethod);
            luaTable.Get("ondisable", out ondisableMethod);
            luaTable.Get("ondestroy", out ondestroyMethod);
            luaTable.Get("update", out updateMethod);
        }

        /// <summary>
        /// 处理View消息
        /// </summary>
        /// <param name="message"></param>
        public override void OnMessage(IMessage message)
        {
            var method = messageMethodMap[message.Name];
            if (method != null)
            {
                method(message.Body);
            }
        }
    }
}
