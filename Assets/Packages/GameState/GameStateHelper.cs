using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using GameState.Lua;
using EC.System;

namespace GameState {

	public class GameStateHelper : ClassExtension
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
					var ta = AssetBundles.DataLoader.Load<TextAsset>(gameStateInfos[i].context);
					if (ta != null)
					{
						gameStates[gameStateInfos[i].name] = new LuaGameState(gameStateInfos[i].name, ta.text);
					}
				}
			}

			return gameStates;
		}
	}
}
