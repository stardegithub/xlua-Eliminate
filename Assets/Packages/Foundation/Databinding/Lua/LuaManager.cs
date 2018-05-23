using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Common;
using XLua;
using GameState;

namespace Foundation.Databinding.Lua
{
    /// <summary>
    /// Lua管理器
    /// </summary>
    /// <typeparam name="LuaManager"></typeparam>
	public class LuaManager : ManagerBehaviourBase<LuaManager>
    {
        /// <summary>
        /// lua gc 间隔
        /// </summary>
        private const float GC_INTERVAL = 1;//1 second 
        private float _lastGCTime = 0;

        private LuaEnv _luaEnv;
        public LuaEnv LuaEnv { get { return _luaEnv; } }

        private Dictionary<string, string> _luaScriptMap;

        private bool _isApplicationQuit;

        #region Singleton
        protected override void SingletonAwake()
        {
            _luaEnv = new LuaEnv();
            _luaScriptMap = new Dictionary<string, string>();
            _initialized = true;
        }

        protected override void SingletonDestroy()
        {
            if (_isApplicationQuit) return;

            _luaEnv.Dispose();
            _luaEnv = null;
        }
        #endregion

        // Update is called once per frame
        void Update()
        {
            if (Time.time - _lastGCTime > GC_INTERVAL)
            {
                _luaEnv.Tick();// lua gc
                _lastGCTime = Time.time;
            }
        }

        void OnApplicationQuit()
        {
            _isApplicationQuit = true;
        }
    }
}