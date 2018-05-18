using UnityEngine;
using UnityEditor;
using GameSystem;

namespace GameEditor
{
    public class CreateGameConfig
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
