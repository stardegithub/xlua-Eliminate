using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System;
using XLua;
using GameManager;

namespace GameSystem
{
    public class LuaObserverView : ObserverView
    {
        [SerializeField]
        protected string luaFilePath;
        protected string luaScript;
        protected LuaTable luaTable;
        protected Dictionary<string, Action<object>> messageMethodMap = new Dictionary<string, Action<object>>();
        protected Action awakeMethod, startMethod, onenableMethod, ondisableMethod, ondestroyMethod, updateMethod;

        void Awake()
        {
            LoadLuaScript();
            if (string.IsNullOrEmpty(luaScript))
            {
                return;
            }
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
        }

        void Update()
        {
            if (updateMethod != null)
            {
                updateMethod();
            }
        }

        protected void LoadLuaScript()
        {
            if (string.IsNullOrEmpty(luaFilePath))
            {
                return;
            }
            luaScript = LoadAssetManager.LoadAssetStatic<TextAsset>(luaFilePath).text;
        }

        protected void BindMethod()
        {
            luaTable = LuaManager.Instance.LuaEnv.NewTable();
            LuaTable metaTable = LuaManager.Instance.LuaEnv.NewTable();
            metaTable.Set("__index", LuaManager.Instance.LuaEnv.Global);
            luaTable.SetMetaTable(metaTable);
            metaTable.Dispose();

            luaTable.Set("self", this.gameObject);
            LuaManager.Instance.LuaEnv.DoString(luaScript, "LuaObserverView", luaTable);

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
