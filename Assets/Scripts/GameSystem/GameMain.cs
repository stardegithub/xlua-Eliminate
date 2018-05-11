using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using GameManager;
using UnityEngine;

namespace GameSystem
{
    public class GameMain : SingletonMonoBehaviourBase<GameMain>
    {
        private bool _initialized;
        public bool Initialized { get { return _initialized; } }

        private GameObject _constManagerLayer;
        private GameObject _dynamicManagerLayer;
        private List<IGameManager> _constManagers;
        private List<IGameManager> _dynamicManagers;
        private Action _evtOnExceptionPopupContinue;
        private Action _evtOnExceptionPopupConfirm;
        private Exception _initException = null;
        private int _loadLevelExceptionCount = 0;
        public int LoadLevelExceptionCount { get { return _loadLevelExceptionCount; } }

        protected override void SingletonAwake()
        {
            _constManagers = new List<IGameManager>();
            _dynamicManagers = new List<IGameManager>();
            _evtOnExceptionPopupContinue = delegate ()
            {
                GameStateManager.Instance.SetNextState("GameStateException");
            };
            _evtOnExceptionPopupConfirm = delegate ()
            {
                _loadLevelExceptionCount--;
            };

            base.SingletonAwake();
            gameObject.AddComponent<DontDestroy>();

            DownLoad(DownLoadSuccess, DownLoadFail);
        }

        /// <summary>
        /// 下载游戏资源
        /// </summary>
        /// <param name="onDownLoadSuccess">下载成功回调</param>
        /// <param name="onDownLoadFail">下载失败回调</param>
        void DownLoad(Action onDownLoadSuccess, Action onDownLoadFail)
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            //todo
            onDownLoadSuccess();
        }

        /// <summary>
        /// 下载成功处理
        /// </summary>
        void DownLoadSuccess()
        {
            InitGame();
        }

        /// <summary>
        /// 下载失败处理
        /// </summary>
        void DownLoadFail()
        {
            //todo
        }

        /// <summary>
        /// 初始化游戏
        /// </summary>
        void InitGame()
        {
            if (GameConfig.Instance == null)
            {
                Error("game config asset not found");
                return;
            }

            Screen.sleepTimeout = GameConfig.Instance.sleepTimeout;
            Application.targetFrameRate = GameConfig.Instance.gameFrameRate;

            _constManagerLayer = new GameObject("Constant");
            _constManagerLayer.transform.parent = transform;

            try
            {
                AddConstantManager(GameConfig.Instance.constManagers);
            }
            catch (Exception e)
            {
                _initException = e;
            }
            finally
            {
                _initialized = true;
                if (_initException == null)
                {
                    GameStateManager.Instance.SetNextState("GameStateLogin");
                }
                else
                {
                    ShowExceptionPopup(_initException, _evtOnExceptionPopupContinue, "ConstantManager");
                }
            }
        }

        /// <summary>
        /// 异常信息弹窗
        /// </summary>
        /// <param name="e">异常</param>
        /// <param name="evtConfirm">回调函数</param>
        /// <param name="prefix">弹窗标题</param>
        private void ShowExceptionPopup(Exception e, Action evtConfirm, string prefix = null)
        {
            _loadLevelExceptionCount++;
            //print("exception count " + _loadLevelExceptionCount);
            string msg = string.Format("{0} {1}\n{2}", prefix, e.Message, e.StackTrace);
            Error(msg);
            // object[] param = new object[] { "OK", evtConfirm };
            //UIManager.MessageBox("ERROR", msg, UIMessageBox.ButtonType.OneButton, param);
        }

        private void AddConstantManager(string[] typeNames)
        {
            AddManager(typeNames, true);
        }

        private void AddDynamicManager(string[] typeNames)
        {
            AddManager(typeNames, false);
        }

        private void DestroyDynamicManager(bool immediate = false)
        {
            if (_dynamicManagerLayer != null)
            {
                if (immediate)
                {
                    DestroyImmediate(_dynamicManagerLayer);
                }
                else
                {
                    Destroy(_dynamicManagerLayer);
                }
                _dynamicManagerLayer = null;
                _dynamicManagers.Clear();
            }
        }

        private void CreateDynamicManager()
        {
            if (_dynamicManagerLayer == null)
            {
                _dynamicManagerLayer = new GameObject("Dynamic");
                DontDestroyOnLoad(_dynamicManagerLayer);
                _dynamicManagerLayer.transform.parent = transform;
                //dynamicManagers.Clear();
            }
        }

        private void AddManager(string[] typeNames, bool constant)
        {
            GameObject go = constant ? _constManagerLayer : _dynamicManagerLayer;
            List<IGameManager> managerPool = constant ? _constManagers : _dynamicManagers;
            Type managerBaseType = typeof(IGameManager);
            for (int i = 0; i < typeNames.Length; i++)
            {
                string name = typeNames[i];
                Type type = Type.GetType(name);

                if (type != null)
                {
                    if (managerBaseType.IsAssignableFrom(type))
                    {
                        if (type.IsSubclassOf(typeof(MonoBehaviour)))
                        {
                            GameObject node = new GameObject(type.Name);
                            node.transform.parent = go.transform;
                            managerPool.Add(node.AddComponent(type) as IGameManager);
                        }
                        else
                        {
                            IGameManager mgr = GameManageHelper.GetCustomManage(type);
                            if (mgr != null)
                            {
                                managerPool.Add(mgr);
                            }
                        }
                    }
                    else
                    {
                        string msg = string.Format("type is not inherit form IGameManager: {0}", name);
                        Error(msg);
                        throw new Exception(msg);
                    }
                }
                else
                {
                    string msg = string.Format("type is not found: {0}", name);
                    Error(msg);
                    throw new Exception(msg);
                }
            }
        }

        /// <summary>
        /// 状态开始
        /// </summary>
        /// <param name="currStateName"></param>
        /// <param name="nextStateName"></param>
        public void OnBeginStateEnter(string currStateName, string nextStateName)
        {
            _loadLevelExceptionCount = 0;
            CreateDynamicManager();
            var nextStateInfo = Array.Find(GameConfig.Instance.gameStateInfos, c => c.name == nextStateName);
            if (nextStateInfo != null)
            {
                string[] managerNames = nextStateInfo.dynamicManagers;
                if (managerNames != null)
                {
                    try
                    {
                        AddDynamicManager(managerNames);
                    }
                    catch (Exception e)
                    {
                        ShowExceptionPopup(e, _evtOnExceptionPopupConfirm, "AddDynamicManager");
                    }
                }
            }

            for (int i = 0; i < _constManagers.Count; i++)
            {
                IGameManager manager = _constManagers[i];
                try
                {
                    manager.OnBeginStateEnter(currStateName, nextStateName);
                }
                catch (Exception e)
                {
                    ShowExceptionPopup(e, _evtOnExceptionPopupConfirm, manager.GetType().Name);
                }
            }

            for (int i = 0; i < _dynamicManagers.Count; i++)
            {
                IGameManager mgr = _dynamicManagers[i];
                try
                {
                    mgr.OnBeginStateEnter(currStateName, nextStateName);
                }
                catch (Exception e)
                {
                    ShowExceptionPopup(e, _evtOnExceptionPopupConfirm, mgr.GetType().Name);
                }
            }
        }


        /// <summary>
        /// 状态结束
        /// </summary>
        /// <param name="currStateName"></param>
        /// <param name="nextStateName"></param>
        public void OnEndStateExit(string currStateName, string nextStateName)
        {
            _loadLevelExceptionCount = 0;
            for (int i = 0; i < _constManagers.Count; i++)
            {
                IGameManager mamager = _constManagers[i];
                try
                {
                    mamager.OnEndStateExit(currStateName, nextStateName);
                }
                catch (Exception e)
                {
                    ShowExceptionPopup(e, _evtOnExceptionPopupConfirm, mamager.GetType().Name);
                }
            }
            DestroyDynamicManager();
            Facade.Instance.ClearAll();
        }
    }
}
