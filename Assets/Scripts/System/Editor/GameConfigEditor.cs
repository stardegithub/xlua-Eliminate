using UnityEngine;
using UnityEditor;
using EC.System;

namespace EC.System
{
    public class GameConfigEditor
    {
        [MenuItem("Assets/Create/CreateGameConfig", false, 700)]
        public static void Create()
        {
            //实例化GameConfig  
            GameConfig config = ScriptableObject.CreateInstance<GameConfig>();

            AssetDatabase.CreateAsset(config, GameConfig.GAME_CONFIG_PATH);
        }
    }
}
