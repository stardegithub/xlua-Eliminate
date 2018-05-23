using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common;
using GameState.Conf;

namespace GameState
{

    /// <summary>
    /// 游戏状态管理器.
    /// </summary>
    public abstract class GameStateManagerBase<T>
        : ManagerBehaviourBase<T> where T : GameStateManagerBase<T>
    {

        /// <summary>
        /// 下一个状态.
        /// </summary>
        protected IGameState _nextState;

        protected IGameState _currState;
        /// <summary>
        /// 当前状态
        /// </summary>
        public IGameState CurrState
        {
            get
            {
                return _currState;
            }
        }

        protected Dictionary<string, IGameState> states;

        #region Singleton

        protected override void SingletonAwake()
        {
            base.SingletonAwake();

            GameStateConf _instance = GameStateConf.GetInstance();
            if (null == _instance)
            {
                this.Error("SingletonAwake()::GameStateConf is not found!!!");
                return;
            }

            this.states = GameStateHelper.GetGameStates(_instance.States);
            if ((null == this.states) || (0 >= this.states.Count))
            {
                this.Error("SingletonAwake()::The game states is empty");
                return;
            }
            _initialized = true;
        }

        protected override void SingletonDestroy()
        {
            base.SingletonDestroy();

            if (null != this._currState)
            {
                this._currState.Exit();
            }

            if (null != states)
            {
                states.Clear();
            }
        }

        #endregion

        void LateUpdate()
        {
            if ((null == _currState) && (null == _nextState))
            {
                return;
            }

            // 状态变化
            if (null == _currState || null == _nextState || false == _currState.IsSame(_nextState))
            {
                string _currStateName = _currState == null ? "" : _currState.Name;
                string _nextStateName = _nextState == null ? "" : _nextState.Name;

                if (null != _currState)
                {
                    // 当前状态退出
                    _currState.Exit();

                    // 退出状态
                    this.OnStateExit(_currStateName, _nextStateName);
                }
				 _currState = _nextState;
                if (null != _nextState)
                {

                    // 开始状态
                    this.OnStateBegin(_currStateName, _nextStateName);

                    _nextState.Enter();
                }
            }
            else
            {
                if (null != _currState)
                {
                    _currState.Update();
                }
            }
        }

        /// <summary>
        /// 设置状态
        /// </summary>
        /// <param name="stateName"></param>
        public void SetNextState(string stateName)
        {
            IGameState _state = GetState(stateName);
            if (null != _state)
            {
                SetNextState(_state);
            }
        }

        /// <summary>
        /// 设置状态
        /// </summary>
        /// <param name="iNextState"></param>
        public void SetNextState(IGameState iNextState)
        {
            _nextState = iNextState;
        }

        /// <summary>
        /// 获得状态
        /// </summary>
        /// <param name="iStateName">状态名字</param>
        /// <returns></returns>
        public IGameState GetState(string iStateName)
        {
            if ((null == this.states) ||
                (0 >= this.states.Count) ||
                (false == Initialized))
            {
                this.Error("GetState()::The game states is empty!!(State:{0})", iStateName);
                return null;
            }
            IGameState _state = null;
            if (false == states.TryGetValue(iStateName, out _state))
            {
                this.Error("GetState()::GameState is not found!!(State:{0})", iStateName);
            }
            return _state;
        }

        #region abstract

        /// <summary>
        /// 开始状态.
        /// </summary>
        /// <param name="iCurStateName">当前状态名.</param>
        /// <param name="iNextStateName">下一个状态名.</param>
        protected abstract void OnStateBegin(string iCurStateName, string iNextStateName);

        /// <summary>
        /// 退出状态.
        /// </summary>
        /// <param name="iCurStateName">当前状态名.</param>
        /// <param name="iNextStateName">下一个状态名.</param>
        protected abstract void OnStateExit(string iCurStateName, string iNextStateName);

        #endregion
    }
}
