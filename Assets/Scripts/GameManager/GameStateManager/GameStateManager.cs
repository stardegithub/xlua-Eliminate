using System.Collections.Generic;
using GameSystem;
using UnityEngine;

namespace GameManager
{
    public class GameStateManager : GameManagerBase<GameStateManager>
    {
        private IGameState nextState;
        private IGameState currState;
        public IGameState CurrState
        {
            get
            {
                return currState;
            }
        }

        private Dictionary<string, IGameState> gameStates;

        #region Singleton
        protected override void SingletonAwake()
        {
            if (GameConfig.Instance == null)
            {
                Debug.LogErrorFormat("GameConfig is not found {0}", GameConfig.GAME_CONFIG_PATH);
            }

            gameStates = GameStateHelper.GetGameStates(GameConfig.Instance.gameStateInfos);
            if (gameStates == null || gameStates.Count == 0)
            {
                Error("gameStates is empty");
            }
            initialized = true;
        }
        #endregion

        void LateUpdate()
        {
            if (currState != nextState)
            {
                string currStateName = currState == null ? "" : currState.StateName;
                string nextStateName = nextState == null ? "" : nextState.StateName;

                if (currState != null)
                {
                    currState.Exit();
                    GameMain.Instance.OnEndStateExit(currStateName, nextStateName);
                }

                currState = nextState;
                if (nextState != null)
                {

                    GameMain.Instance.OnBeginStateEnter(currStateName, nextStateName);
                    nextState.Enter();
                }
            }
            else if (currState != null)
            {
                currState.Update();
            }
        }
        public void SetNextState(string stateName)
        {
            var gameState = GetState(stateName);
            if (gameState != null)
            {
                SetNextState(gameState);
            }
        }

        public void SetNextState(IGameState gameState)
        {
            nextState = gameState;
        }

        public IGameState GetState(string stateName)
        {
            if (gameStates == null || !Initialized) return null;
            IGameState gameState;
            if (!gameStates.TryGetValue(stateName, out gameState))
            {
                string msg = string.Format("GameState is not found: {0}", stateName);
                Error(msg);
            }
            return gameState;
        }

        public void RefreshState()
        {
            if (currState != null)
            {
                currState.Exit();
            }

            if (currState != null)
            {
                currState.Enter();
            }
        }
    }
}
