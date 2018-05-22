using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using Facade;
using GameState;
using GameState.Conf;
using EC.Common;
using EC.GameState;

namespace EC.System
{
    public class GameMain : SingletonMonoBehaviourBase<GameMain>
    {
        private GameObject _constManagerLayer;
        private GameObject _dynamicManagerLayer;
		private List<IManagerBase> _constManagers;
		private List<IManagerBase> _dynamicManagers;
        private Action _evtOnExceptionPopupContinue;
        private Action _evtOnExceptionPopupConfirm;
        private Exception _initException = null;
        private int _loadLevelExceptionCount = 0;
        public int LoadLevelExceptionCount { get { return _loadLevelExceptionCount; } }

        protected override void SingletonAwake()
        {
			base.SingletonAwake();

			_constManagers = new List<IManagerBase>();
			_dynamicManagers = new List<IManagerBase>();

			// 异常场景委托
            _evtOnExceptionPopupContinue = delegate ()
            {
				GameStateManager.Instance.SetNextState(GameStateConf.GetInstance().ExceptionGameState);
            };
            _evtOnExceptionPopupConfirm = delegate ()
            {
				--_loadLevelExceptionCount;
            };

			// 挂接不释放脚本
			DontDestroy _dontDestroy = this.GetComponent<DontDestroy>();
			if (null == _dontDestroy) {
				this.gameObject.AddComponent<DontDestroy> ();
			}

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
			GameStateConf _conf = GameStateConf.GetInstance ();
			if (null ==  _conf)
            {
                this.Error("the Game config asset not found!!");
                return;
            }

			Screen.sleepTimeout = _conf.SleepTimeout;
			Application.targetFrameRate = _conf.FPS;

            _constManagerLayer = new GameObject("Constant");
            _constManagerLayer.transform.parent = transform;

            try
            {
				AddConstantManager(_conf.Managers.ToArray());
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
					GameStateManager.Instance.SetNextState(_conf.FirstGameState);
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
			List<IManagerBase> managerPool = constant ? _constManagers : _dynamicManagers;
			Type managerBaseType = typeof(IManagerBase);
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
							managerPool.Add(node.AddComponent(type) as IManagerBase);
                        }
                        else
                        {
							IManagerBase mgr = GameManageHelper.GetCustomManage(type);
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

			GameStateConf _stateConf = GameStateConf.GetInstance ();
			if (null == _stateConf) {
				this.Error ("OnBeginStateEnter()::The game state conf is invalid!!!");
				return;
			}

			GameStateInfo _nextStateInfo = Array.Find(
				_stateConf.States.ToArray(), c => c.Name == nextStateName);
			
			if (null != _nextStateInfo)
            {
				List<string> _managerNames = _nextStateInfo.Managers;
				if ((null != _managerNames) && (0 < _managerNames.Count))
                {
                    try
                    {
						AddDynamicManager(_managerNames.ToArray());
                    }
                    catch (Exception e)
                    {
                        ShowExceptionPopup(e, _evtOnExceptionPopupConfirm, "AddDynamicManager");
                    }
                }
            }

            for (int i = 0; i < _constManagers.Count; i++)
            {
				IManagerBase manager = _constManagers[i];
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
				IManagerBase mgr = _dynamicManagers[i];
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
				IManagerBase mamager = _constManagers[i];
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
			Facade.Facade.Instance.ClearAll ();
        }
    }
}
