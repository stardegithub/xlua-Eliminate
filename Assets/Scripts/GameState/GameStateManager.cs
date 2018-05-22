using UnityEngine;
using System.Collections.Generic;
using GameState;
using GameState.Conf;
using EC.Common;
using EC.System;

namespace EC.GameState
{
    public class GameStateManager : GameManagerBase<GameStateManager>
    {
        private IGameState _nextState;
        private IGameState _currState;
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

        private Dictionary<string, IGameState> _gameStates;

        #region Singleton
        protected override void SingletonAwake()
        {
			GameStateConf _instance = GameStateConf.GetInstance ();
			if (null == _instance)
            {
				this.Error("GameStateConf is not found!!!");
				return;
            }

			_gameStates = GameStateHelper.GetGameStates(_instance.States);
            if (_gameStates == null || _gameStates.Count == 0)
            {
				this.Error("The game states conf is empty");
				return;
            }
            _initialized = true;
        }
        #endregion

        void LateUpdate()
        {
            if (_currState != _nextState)
            {
                string currStateName = _currState == null ? "" : _currState.StateName;
                string nextStateName = _nextState == null ? "" : _nextState.StateName;

                if (_currState != null)
                {
                    _currState.Exit();
                    GameMain.Instance.OnEndStateExit(currStateName, nextStateName);
                }

                _currState = _nextState;
                if (_nextState != null)
                {

                    GameMain.Instance.OnBeginStateEnter(currStateName, nextStateName);
                    _nextState.Enter();
                }
            }
            else if (_currState != null)
            {
                _currState.Update();
            }
        }
        
        /// <summary>
        /// 设置状态
        /// </summary>
        /// <param name="stateName"></param>
        public void SetNextState(string stateName)
        {
            var gameState = GetState(stateName);
            if (gameState != null)
            {
                SetNextState(gameState);
            }
        }

        /// <summary>
        /// 设置状态
        /// </summary>
        /// <param name="gameState"></param>
        public void SetNextState(IGameState gameState)
        {
            _nextState = gameState;
        }

        /// <summary>
        /// 获得状态
        /// </summary>
        /// <param name="stateName">状态名字</param>
        /// <returns></returns>
        public IGameState GetState(string stateName)
        {
            if (_gameStates == null || !Initialized) return null;
            IGameState gameState;
            if (!_gameStates.TryGetValue(stateName, out gameState))
            {
                string msg = string.Format("GameState is not found: {0}", stateName);
                Error(msg);
            }
            return gameState;
        }

        /// <summary>
        /// 刷新状态
        /// </summary>
        public void RefreshState()
        {
            if (_currState != null)
            {
                _currState.Exit();
            }

            if (_currState != null)
            {
                _currState.Enter();
            }
        }
    }
}
