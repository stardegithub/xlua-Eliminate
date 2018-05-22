using UnityEngine;
using System;

namespace EC.System
{
    public class GameConfig : ScriptableObject
    {
        public const string GAME_CONFIG_PATH = "Assets/Resources/Conf/GameConfig.asset";

        private static GameConfig instance;
        public static GameConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = AssetBundles.DataLoader.Load<GameConfig>(GAME_CONFIG_PATH);
                }
                return instance;
            }
        }

        public enum GameStateType
        {
            Inherit,
            Script,
            Lua,
        }

        [Serializable]
        public class GameStateInfo
        {
            public GameStateType type;
            public string name;
            public string context;
            public string[] dynamicManagers;
        }

        public int sleepTimeout = SleepTimeout.NeverSleep;
        public int gameFrameRate = 60;
        public int timerInterval = 1;

        public string firstGameState = "GameStateLogin";
        public string exceptionGameState = "GameStateException";

        public string uiRootName = "UIRoot";
        public string uiRootPath = "UI/Global/UIRoot";
        public string uiCameraName = "UICamera";

        public string[] constManagers;

        public GameStateInfo[] gameStateInfos;
    }
}
