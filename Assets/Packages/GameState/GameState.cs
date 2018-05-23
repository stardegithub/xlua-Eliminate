using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using Common;
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
        string Name { get; }

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

		/// <summary>
		/// 判断是否为同一状态.
		/// </summary>
		/// <returns><c>true</c> 同一状态; 非同一状态, <c>false</c>.</returns>
		/// <param name="iState">状态.</param>
		bool IsSame (IGameState iState);
    }

    /// <summary>
    /// 游戏状态基类
    /// </summary>
	public abstract class GameStateBase : ClassExtension, IGameState
    {
        protected string name;

        /// <summary>
        /// 状态名
        /// </summary>
        public string Name
        {
			get { return name; }
			protected set { name = value; }
        }
			
		/// <summary>
		/// 判断是否为同一状态.
		/// </summary>
		/// <returns><c>true</c> 同一状态; 非同一状态, <c>false</c>.</returns>
		/// <param name="iState">状态.</param>
		public bool IsSame (IGameState iState) {
			if ((null == iState) || 
				(false == string.IsNullOrEmpty(iState.Name))) {
				return false;
			}
			if (true == string.IsNullOrEmpty (this.Name)) {
				return false;
			}
			return this.Name.Equals (iState.Name);
		}

        /// <summary>
        /// 开始
        /// </summary>
		public abstract void Enter();

        /// <summary>
        /// 更新
        /// </summary>
		public abstract void Update();

        /// <summary>
        /// 退出
        /// </summary>
		public abstract void Exit();
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
            this.Name = stateName;
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
