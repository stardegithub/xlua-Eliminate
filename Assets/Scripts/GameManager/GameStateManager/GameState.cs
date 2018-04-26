using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Events;
using System.Reflection;
using System.Collections.Generic;
using XLua;
using GameSystem;

namespace GameManager
{
    public interface IGameState
    {
        string StateName { get; }
        void Enter();
        void Update();
        void Exit();
    }

    public class GameStateBase : IGameState
    {
        protected string stateName;

        public virtual string StateName
        {
            get { return stateName; }
        }

        public virtual void Enter()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void Exit()
        {
        }
    }

    public class ScriptGameState : GameStateBase
    {
        protected string stateType;
        protected object scriptInstance;
        protected MethodInfo enterMethod;
        protected MethodInfo updateMethod;
        protected MethodInfo exitMethod;

        public ScriptGameState(string stateName, string stateType)
        {
            this.stateName = stateName;
            this.stateType = stateType;
        }

        protected void ReflectMethod()
        {
            Type type = Type.GetType(stateType);
            if (type == null)
            {
                return;
            }

            scriptInstance = Activator.CreateInstance(type);

            MethodInfo mi = type.GetMethod("Enter", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
            if (mi != null && mi.GetParameters().Length == 0)
            {
                enterMethod = mi;
            }

            mi = type.GetMethod("Update", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
            if (mi != null && mi.GetParameters().Length == 0)
            {
                updateMethod = mi;
            }

            mi = type.GetMethod("Exit", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
            if (mi != null && mi.GetParameters().Length == 0)
            {
                exitMethod = mi;
            }
        }

        public override void Enter()
        {
            ReflectMethod();

            if (enterMethod != null)
            {
                enterMethod.Invoke(scriptInstance, null);
            }
        }

        public override void Update()
        {
            if (updateMethod != null)
            {
                updateMethod.Invoke(scriptInstance, null);
            }
        }

        public override void Exit()
        {
            if (exitMethod != null)
            {
                exitMethod.Invoke(scriptInstance, null);
            }

            scriptInstance = null;
            enterMethod = null;
            updateMethod = null;
            exitMethod = null;
        }
    }

    public class LuaGameState : GameStateBase
    {
        protected string luaScript;
        protected LuaTable luaTable;

        protected Action enterMethod;
        protected Action updateMethod;
        protected Action exitMethod;

        public LuaGameState(string stateName, string luaScript)
        {
            this.stateName = stateName;
            this.luaScript = luaScript;
        }

        protected void BindMethod()
        {
            luaTable = LuaManager.Instance.LuaEnv.NewTable();
            LuaTable metaTable = LuaManager.Instance.LuaEnv.NewTable();
            metaTable.Set("__index", LuaManager.Instance.LuaEnv.Global);
            luaTable.SetMetaTable(metaTable);
            metaTable.Dispose();
            LuaManager.Instance.LuaEnv.DoString(luaScript, "LuaGameState", luaTable);

            luaTable.Get("enter", out enterMethod);
            luaTable.Get("update", out updateMethod);
            luaTable.Get("exit", out exitMethod);
        }

        void LuaTableForEach<TKey, TValue>(TKey k, TValue v)
        {

        }

        public override void Enter()
        {
            BindMethod();

            if (enterMethod != null)
            {
                enterMethod();
            }
        }

        public override void Update()
        {
            if (updateMethod != null)
            {
                updateMethod();
            }
        }

        public override void Exit()
        {
            if (exitMethod != null)
            {
                exitMethod();
            }

            enterMethod = null;
            updateMethod = null;
            exitMethod = null;
            luaTable.Dispose();
        }
    }

    public class GameStateHelper
    {
        public static Dictionary<string, IGameState> GetGameStates(GameConfig.GameStateInfo[] gameStateInfos)
        {
            if (gameStateInfos == null) return null;
            var gameStates = new Dictionary<string, IGameState>();

            for (int i = 0; i < gameStateInfos.Length; i++)
            {
                if (gameStateInfos[i].type == GameConfig.GameStateType.Inherit)
                {
                    Type type = Type.GetType(gameStateInfos[i].context);
                    gameStates[gameStateInfos[i].name] = Activator.CreateInstance(type) as IGameState;
                }
                else if (gameStateInfos[i].type == GameConfig.GameStateType.Script)
                {
                    gameStates[gameStateInfos[i].name] = new ScriptGameState(gameStateInfos[i].name, gameStateInfos[i].context);
                }
                else if (gameStateInfos[i].type == GameConfig.GameStateType.Lua)
                {
                    string luaScript = LoadAssetManager.LoadAssetStatic<TextAsset>(gameStateInfos[i].context).text;
                    gameStates[gameStateInfos[i].name] = new LuaGameState(gameStateInfos[i].name, luaScript);
                }
            }

            return gameStates;
        }
    }
}
