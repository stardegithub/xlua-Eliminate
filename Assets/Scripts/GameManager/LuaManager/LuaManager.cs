using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using XLua;

namespace GameManager
{
    public class LuaManager : GameManagerBase<LuaManager>
    {
        private const float GCInterval = 1;//1 second 
        private float lastGCTime = 0;

        private LuaEnv luaEnv;

        public LuaEnv LuaEnv { get { return luaEnv; } }

        private Dictionary<string, string> luaScriptMap;

        #region Singleton
        protected override void SingletonAwake()
        {
            luaEnv = new LuaEnv();
            luaScriptMap = new Dictionary<string, string>();
            initialized = true;
        }

        protected override void SingletonDestroy()
        {
            // luaEnv.Dispose();
            // luaEnv = null;
        }
        #endregion

        // Update is called once per frame
        void Update()
        {
            if (Time.time - lastGCTime > GCInterval)
            {
                luaEnv.Tick();
                lastGCTime = Time.time;
            }
        }
    }
}