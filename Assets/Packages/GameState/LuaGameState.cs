using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using GameState;
using Foundation.Databinding.Lua;
using XLua;

namespace GameState.Lua
{

    /// <summary>
    /// 游戏状态(lua)
    /// </summary>
    public class LuaGameState : GameStateBase
    {
        protected string luaScript;
        protected LuaTable luaTable;

        protected Action enterMethod;
        protected Action updateMethod;
        protected Action exitMethod;

        public LuaGameState(string stateName, string luaScript)
        {
            this.Name = stateName;
            this.luaScript = luaScript;
        }

        protected void BindMethod()
        {
            luaTable = LuaManager.Instance.CreateExpandTable(Name);
            if (luaTable == null) { return; }
            LuaManager.Instance.DoString(luaScript, Name, luaTable);

            luaTable.Get("enter", out enterMethod);
            luaTable.Get("update", out updateMethod);
            luaTable.Get("exit", out exitMethod);
        }

        void LuaTableForEach<TKey, TValue>(TKey k, TValue v)
        {

        }

        /// <summary>
        /// 开始
        /// </summary>
        public override void Enter()
        {
            BindMethod();

            if (enterMethod != null)
            {
                enterMethod();
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        public override void Update()
        {
            if (updateMethod != null)
            {
                updateMethod();
            }
        }

        /// <summary>
        /// 退出
        /// </summary>
        public override void Exit()
        {
            if (exitMethod != null)
            {
                exitMethod();
            }

            enterMethod = null;
            updateMethod = null;
            exitMethod = null;
            if (luaTable != null)
            {
                luaTable.Dispose();
            }
            if (LuaManager.Instance != null)
            {
                LuaManager.Instance.RemoveTable(Name);
            }
        }
    }
}
