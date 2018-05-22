using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Events;
using System.Reflection;
using System.Collections.Generic;
using XLua;
using Foundation.Databinding.Lua;
using EC.System;

namespace GameState
{
    /// <summary>
    /// 游戏状态接口
    /// </summary>
    public interface IGameState
    {
        /// <summary>
        /// 名字
        /// </summary>
        string StateName { get; }

        /// <summary>
        /// 开始
        /// </summary>
        void Enter();

        /// <summary>
        /// 更新
        /// </summary>
        void Update();

        /// <summary>
        /// 退出
        /// </summary>
        void Exit();
    }

    /// <summary>
    /// 游戏状态基类
    /// </summary>
    public abstract class GameStateBase : IGameState
    {
        protected string stateName;

        /// <summary>
        /// 名字
        /// </summary>
        public virtual string StateName
        {
            get { return stateName; }
        }

        /// <summary>
        /// 开始
        /// </summary>
        public virtual void Enter()
        {
        }

        /// <summary>
        /// 更新
        /// </summary>
        public virtual void Update()
        {
        }

        /// <summary>
        /// 退出
        /// </summary>
        public virtual void Exit()
        {
        }
    }

    /// <summary>
    /// 游戏状态(反射类)
    /// </summary>
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

        /// <summary>
        /// 反射函数
        /// </summary>
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

        /// <summary>
        /// 开始
        /// </summary>
        public override void Enter()
        {
            ReflectMethod();

            if (enterMethod != null)
            {
                enterMethod.Invoke(scriptInstance, null);
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        public override void Update()
        {
            if (updateMethod != null)
            {
                updateMethod.Invoke(scriptInstance, null);
            }
        }

        /// <summary>
        /// 退出
        /// </summary>
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
}
