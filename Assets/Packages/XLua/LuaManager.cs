using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Common;

namespace XLua
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
        private Dictionary<string, LuaTable> _luaTableMap;
        private LuaTable tableMap;

        private bool _isApplicationQuit;

        #region Singleton
        protected override void SingletonAwake()
        {
            _luaEnv = new LuaEnv();
            AddLoaders();

            _luaTableMap = new Dictionary<string, LuaTable>();
            tableMap = _luaEnv.NewTable();
            _luaEnv.Global.Set("tablemap", tableMap);
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

        /// <summary>
        /// 添加require的加载文件方法
        /// </summary>
        private void AddLoaders()
        {
            var _conf = GameState.Conf.GameStateConf.GetInstance();
            foreach (string luaFilePrefix in _conf.Data.LuaFilePrefixs)
            {
                foreach (string luaFileSuffix in _conf.Data.LuaFileSuffixs)
                {
                    _luaEnv.AddLoader((ref string filename) =>
                    {
                        string filePath = string.Format("{0}{1}{2}", luaFilePrefix, filename, luaFileSuffix);
                        var luaText = AssetBundles.DataLoader.Load<TextAsset>(filePath);
                        if (luaText != null)
                        {
                            return System.Text.Encoding.UTF8.GetBytes(luaText.text);
                        }
                        else
                        {
                            return null;
                        }
                    });
                }
            }
        }

        /// <summary>
        /// 创建LuaTable
        /// </summary>
        /// <param name="tableName">做key</param>
        /// <returns></returns>
        public LuaTable CreateTable(string tableName)
        {
            if (_luaTableMap.ContainsKey(tableName))
            {
                Error("CreateTable():the table has existed :{0}", tableName);
                return null;
            }
            LuaTable luaTable = _luaEnv.NewTable();
            AddTable(tableName, luaTable);
            return luaTable;
        }

        /// <summary>
        /// 创建拓展的LuaTable
        /// </summary>
        /// <param name="tableName">做key</param>
        /// <returns></returns>
        public LuaTable CreateExpandTable(string tableName)
        {
            if (_luaTableMap.ContainsKey(tableName))
            {
                Error("CreateExpandTable():the table has existed :{0}", tableName);
                return null;
            }
            LuaTable luaTable = _luaEnv.NewTable();
            LuaTable metaTable = _luaEnv.NewTable();
            metaTable.Set("__index", _luaEnv.Global);
            luaTable.SetMetaTable(metaTable);
            metaTable.Dispose();
            AddTable(tableName, luaTable);
            return luaTable;
        }

        /// <summary>
        /// 执行代码块
        /// </summary>
        /// <param name="chunk"></param>
        /// <param name="chunkName"></param>
        /// <param name="luaTable"></param>
        public void DoString(string chunk, string chunkName = "chunk", LuaTable luaTable = null)
        {
            _luaEnv.DoString(chunk, chunkName, luaTable);
        }

        /// <summary>
        /// 执行代码块
        /// </summary>
        /// <param name="chunk"></param>
        /// <param name="chunkName"></param>
        /// <param name="tableName"></param>
        public void DoString(string chunk, string chunkName = "chunk", string tableName = null)
        {
            if (tableName == null)
            {
                _luaEnv.DoString(chunk, chunkName, null);
            }
            else
            {
                LuaTable luaTable = GetTable(tableName);
                _luaEnv.DoString(chunk, chunkName, luaTable);
            }
        }

        /// <summary>
        /// 添加LuaTable
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="luaTable"></param>
        public void AddTable(string tableName, LuaTable luaTable)
        {
            _luaTableMap.Add(tableName, luaTable);
            tableMap.Set(tableName, luaTable);
        }

        /// <summary>
        /// 获取LuaTable
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public LuaTable GetTable(string tableName)
        {
            if (_luaTableMap.ContainsKey(tableName))
            {
                return _luaTableMap[tableName];
            }
            else
            {
                Error("GetTable():the table has existed :{0}", tableName);
                return null;
            }
        }

        /// <summary>
        /// 移除LuaTable
        /// </summary>
        /// <param name="tableName"></param>
        public void RemoveTable(string tableName)
        {
            if (_luaTableMap.ContainsKey(tableName))
            {
                _luaTableMap[tableName].Dispose();
                _luaTableMap.Remove(tableName);
                tableMap.Set(tableName, default(LuaTable));
            }
            else
            {
                Error("RemoveTable():the table has existed :{0}", tableName);
            }
        }
    }
}