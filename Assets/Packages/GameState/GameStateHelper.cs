using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using GameState.Conf;
using GameState.Lua;

namespace GameState {

	public class GameStateHelper : ClassExtension
	{

		/// <summary>
		/// 取得游戏状态.
		/// </summary>
		/// <returns>游戏状态.</returns>
		/// <param name="gameStateInfos">游戏状态信息.</param>
		public static Dictionary<string, IGameState> GetGameStates(List<GameStateInfo> iGameStateInfos)
		{
			if (iGameStateInfos == null) return null;
			var _gameStates = new Dictionary<string, IGameState>();

			foreach(GameStateInfo _state in iGameStateInfos) 
			{
				if (null == _state) {
					continue;
				}
				switch (_state.Type) {
				case GameStateType.Inherit:
					{
						Type type = Type.GetType(_state.ClassName);
						_gameStates[_state.Name] = Activator.CreateInstance(type) as IGameState;
					}
					break;
				case GameStateType.CsScript:
					{
						_gameStates[_state.Name] = new ScriptGameState(_state.Name, _state.ClassName);
					}
					break;
				case GameStateType.Lua:
					{
						var ta = AssetBundles.DataLoader.Load<TextAsset>(_state.LuaPath);
						if (ta != null)
						{
							_gameStates[_state.Name] = new LuaGameState(_state.Name, ta.text);
						}
					}
					break;
				default:
					{}
					break;
				}
			}

			return _gameStates;
		}
	}
}
