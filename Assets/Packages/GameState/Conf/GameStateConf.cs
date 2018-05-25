using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Common;

namespace GameState.Conf
{

    /// <summary>
    /// 游戏状态类型.
    /// </summary>
    public enum GameStateType
    {
        /// <summary>
        /// 无.
        /// </summary>
        None,
        /// <summary>
        /// 集成.
        /// </summary>
        Inherit,
        /// <summary>
        /// 脚本.
        /// </summary>
        CsScript,
        /// <summary>
        /// Lua.
        /// </summary>
        Lua,
        /// <summary>
        /// 未知.
        /// </summary>
        Unkonwn
    }

    [Serializable]
    public class GameStateInfo : JsonDataBase
    {
        /// <summary>
        /// 游戏状态类型.
        /// </summary>
        public GameStateType Type;

        /// <summary>
        /// 名称.
        /// </summary>
        public string Name;

        /// <summary>
        /// 类名，用于继承类型和c#脚本类型
        /// </summary>
        public string ClassName;

        /// <summary>
        /// lua脚本地址，只用于lua类型
        /// </summary>
        public string LuaPath;

        /// <summary>
        /// 管理器（各个状态动态的）.
        /// </summary>
        public List<string> Managers;

        /// <summary>
        /// 清空.
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            Type = GameStateType.None;
            Name = null;
            ClassName = null;
            LuaPath = null;
            Managers.Clear();
        }
    }

    /// <summary>
    /// 游戏状态配置数据.
    /// </summary>
    [Serializable]
    public class GameStateConfData : JsonDataBase
    {

        /// <summary>
        /// 睡眠时间.
        /// </summary>
        public int SleepTimeout = UnityEngine.SleepTimeout.NeverSleep;

        /// <summary>
        /// 每秒传输帧数(单位：帧／秒).
        /// </summary>
        public int FPS = 60;

        /// <summary>
        /// 中断时间(单位：秒).
        /// </summary>
        public int TimerInterval = 1;

        /// <summary>
        /// 第一个游戏状态.
        /// </summary>
        public string FirstGameState = "GameStateLogin";

        /// <summary>
        /// 异常游戏状态.
        /// </summary>
        public string ExceptionGameState = "GameStateException";

        /// <summary>
        /// .
        /// </summary>
        public string uiRootName = "UIRoot";
        public string uiRootPath = "Prefab/UIRoot";
        public string uiCameraName = "UICamera";

        /// <summary>
        /// lua文件前缀
        /// </summary
        public string[] LuaFilePrefixs = new string[]{ "Lua/", "Lua/Data/" };
        /// <summary>
        /// lua文件后缀
        /// </summary>
        public string[] LuaFileSuffixs = new string[]{ ".lua", "" };

        /// <summary>
        /// 管理器（固定）.
        /// </summary>
        public List<string> Managers;

        /// <summary>
        /// 状态一览.
        /// </summary>
        public List<GameStateInfo> States;

        /// <summary>
        /// 清空.
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            SleepTimeout = UnityEngine.SleepTimeout.NeverSleep;
            FPS = 60;
            TimerInterval = 1;
            FirstGameState = "GameStateLogin";
            ExceptionGameState = "GameStateException";
            uiRootName = "UIRoot";
            uiRootPath = "UI/Global/UIRoot";
            uiCameraName = "UICamera";
            Managers.Clear();
            States.Clear();
        }
    }

    /// <summary>
    /// 游戏状态配置.
    /// </summary>
    public class GameStateConf : AssetBase<GameStateConf, GameStateConfData>
    {

        /// <summary>
        /// 睡眠时间.
        /// </summary>
        public int SleepTimeout
        {
            get
            {
                if (null == this.Data)
                {
                    return -1;
                }
                return this.Data.SleepTimeout;
            }
        }

        /// <summary>
        /// 每秒传输帧数(单位：帧／秒).
        /// </summary>
        public int FPS
        {
            get
            {
                if (null == this.Data)
                {
                    return 60;
                }
                return this.Data.FPS;
            }
        }

        /// <summary>
        /// 第一个游戏状态.
        /// </summary>
        public string FirstGameState
        {
            get
            {
                if (null == this.Data)
                {
                    return null;
                }
                return this.Data.FirstGameState;
            }
        }

        /// <summary>
        /// 异常游戏状态.
        /// </summary>
        public string ExceptionGameState
        {
            get
            {
                if (null == this.Data)
                {
                    return null;
                }
                return this.Data.ExceptionGameState;
            }
        }

        /// <summary>
        /// 管理器（固定）.
        /// </summary>
        public List<string> Managers
        {
            get
            {
                if (null == this.Data)
                {
                    return null;
                }
                return this.Data.Managers;
            }
        }

        /// <summary>
        /// 状态一览.
        /// </summary>
        public List<GameStateInfo> States
        {
            get
            {
                if (null == this.Data)
                {
                    return null;
                }
                return this.Data.States;
            }
        }

        /// <summary>
        /// 用用数据.
        /// </summary>
        /// <param name="iData">数据.</param>
        /// <param name="iForceClear">强制清空标志位.</param>
        protected override void ApplyData(GameStateConfData iData, bool iForceClear)
        {

            if (null == iData)
            {
                return;
            }

            // 清空
            if (true == iForceClear)
            {
                this.Clear();
            }

            this.Data.SleepTimeout = iData.SleepTimeout;
            this.Data.FPS = iData.FPS;
            this.Data.TimerInterval = iData.TimerInterval;
            this.Data.FirstGameState = iData.FirstGameState;
            this.Data.ExceptionGameState = iData.ExceptionGameState;
            this.Data.uiRootName = iData.uiRootName;
            this.Data.uiRootPath = iData.uiRootPath;
            this.Data.uiCameraName = iData.uiCameraName;
            this.Data.LuaFilePrefixs = iData.LuaFilePrefixs;
            this.Data.LuaFileSuffixs = iData.LuaFileSuffixs;
            this.Data.Managers.AddRange(iData.Managers);
            this.Data.States.AddRange(iData.States);

            UtilsAsset.SetAssetDirty(this);
        }
    }
}
