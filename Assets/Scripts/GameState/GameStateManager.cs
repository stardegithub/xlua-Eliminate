using UnityEngine;
using System.Collections.Generic;
using GameState;
using GameState.Conf;
using Common;
using EC.System;

namespace EC.GameState
{

	/// <summary>
	/// 游戏状态管理器.
	/// </summary>
	public class GameStateManager 
		: GameStateManagerBase<GameStateManager>
	{

		/// <summary>
		/// 开始状态.
		/// </summary>
		/// <param name="iCurStateName">当前状态名.</param>
		/// <param name="iNextStateName">下一个状态名.</param>
		protected override void OnStateBegin (string iCurStateName, string iNextStateName) {
			if (null == GameMain.Instance) {
				return;
			}
			GameMain.Instance.OnBeginStateEnter (iCurStateName, iNextStateName);
		}

		/// <summary>
		/// 退出状态.
		/// </summary>
		/// <param name="iCurStateName">当前状态名.</param>
		/// <param name="iNextStateName">下一个状态名.</param>
		protected override void OnStateExit (string iCurStateName, string iNextStateName) {
			if (null == GameMain.Instance) {
				return;
			}
			GameMain.Instance.OnEndStateExit (iCurStateName, iNextStateName);
		}

        /// <summary>
        /// 刷新状态
        /// </summary>
        public void RefreshState()
        {
			if (null != _currState)
            {
				_currState.Exit();
				_currState.Enter();
            }
        }
    }
}
