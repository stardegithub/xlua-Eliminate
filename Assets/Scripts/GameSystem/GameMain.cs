using System;
using System.Collections;
using System.Collections.Generic;
using GameManager;
using UnityEngine;

namespace GameSystem
{
    public class GameMain : Singleton<GameMain>
    {
        private bool initialized;
        public bool Initialized
        {
            get
            {
                return initialized;
            }
        }

        private GameObject constManagerLayer;
        private GameObject dynamicManagerLayer;
        private List<IGameManager> constManagers = new List<IGameManager>();
        private List<IGameManager> dynamicManagers = new List<IGameManager>();

        private Action evtOnExceptionPopupContinue;
        private Action evtOnExceptionPopupConfirm;
        private Exception initException = null;
        private int loadLevelExceptionCount = 0;
        public int LoadLevelExceptionCount { get { return loadLevelExceptionCount; } }

        protected override void SingletonAwake()
        {
            base.SingletonAwake();
            gameObject.AddComponent<DontDestroy>();

            Screen.sleepTimeout = GameConfig.Instance.sleepTimeout;
            Application.targetFrameRate = GameConfig.Instance.gameFrameRate;

            evtOnExceptionPopupContinue = delegate ()
            {
                GameStateManager.Instance.SetNextState("GameStateException");
            };
            evtOnExceptionPopupConfirm = delegate ()
            {
                loadLevelExceptionCount--;
            };

            if (GameConfig.Instance == null)
            {
                Debug.LogError("game config asset not found");
                return;
            }

            constManagerLayer = new GameObject("Constant");
            constManagerLayer.transform.parent = transform;
            try
            {
                AddConstantManager(GameConfig.Instance.constManagers);
            }
            catch (Exception e)
            {
                initException = e;
            }
        }

        void Start()
        {
            if (initException == null)
            {
                GameStateManager.Instance.SetNextState("GameStateLogin");
            }
            else
            {
                ShowExceptionPopup(initException, evtOnExceptionPopupContinue, null, "ConstantManager");
            }

            initialized = true;
        }

        private void ShowExceptionPopup(Exception e, Action evtContinue, Action evtBreak, string prefix = null)
        {
            string msg = string.Format("{0} {1}\n{2}", prefix, e.Message, e.StackTrace);
            Debug.LogError(msg);
            // object[] param = new object[] { "Continue", evtContinue, "Break", evtBreak };
            //UIManager.MessageBox("ERROR", msg, UIMessageBox.ButtonType.TwoButton, param);
        }

        private void ShowExceptionPopup(Exception e, Action evtConfirm, string prefix = null)
        {
            loadLevelExceptionCount++;
            //print("exception count " + _loadLevelExceptionCount);
            string msg = string.Format("{0} {1}\n{2}", prefix, e.Message, e.StackTrace);
            Debug.LogError(msg);
            // object[] param = new object[] { "OK", evtConfirm };
            //UIManager.MessageBox("ERROR", msg, UIMessageBox.ButtonType.OneButton, param);
        }

        //public void SimulateMemoryWarningNow()
        //{
        //    OnMemoryWarning();
        //}

        //private void OnMemoryWarning()
        //{
        //    if (_memWwarningHandleTimer > 0) { return; }
        //    _memWwarningHandleTimer = _memWarningHandleInterval;

        //    float mega = 1024f * 1024f;
        //    float curMem = NativeUtils.GetCurrentMemoryBytes() / mega;
        //    float devMem = (float)SystemInfo.systemMemorySize;
        //    float perc = curMem / devMem * 100f;
        //    Debug.LogWarningFormat("Received system low memory warning at: {0:N1}/{1:N1}, {2:N1}%", curMem, devMem, perc);

        //    for (int i = 0; i < _constantMgrs.Count; i++)
        //    {
        //        ICustomManager mgr = _constantMgrs[i];
        //        try
        //        {
        //            mgr.SingletonOnMemoryWarning(curMem, devMem, perc);
        //        }
        //        catch (Exception e)
        //        {
        //            ShowExceptionPopup(e, _evtOnExceptionPopupConfirm, mgr.GetType().Name);
        //        }
        //    }
        //    for (int i = 0; i < _dynamicMgrs.Count; i++)
        //    {
        //        ICustomManager mgr = _dynamicMgrs[i];
        //        try
        //        {
        //            mgr.SingletonOnMemoryWarning(curMem, devMem, perc);
        //        }
        //        catch (Exception e)
        //        {
        //            ShowExceptionPopup(e, _evtOnExceptionPopupConfirm, mgr.GetType().Name);
        //        }
        //    }
        //}

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
            if (dynamicManagerLayer != null)
            {
                if (immediate)
                {
                    DestroyImmediate(dynamicManagerLayer);
                }
                else
                {
                    Destroy(dynamicManagerLayer);
                }
                dynamicManagerLayer = null;
                dynamicManagers.Clear();
            }
        }

        private void CreateDynamicManager()
        {
            if (dynamicManagerLayer == null)
            {
                dynamicManagerLayer = new GameObject("Dynamic");
                DontDestroyOnLoad(dynamicManagerLayer);
                dynamicManagerLayer.transform.parent = transform;
                //dynamicManagers.Clear();
            }
        }

        private void AddManager(string[] typeNames, bool constant)
        {
            GameObject go = constant ? constManagerLayer : dynamicManagerLayer;
            List<IGameManager> managerPool = constant ? constManagers : dynamicManagers;
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
                        Debug.LogErrorFormat("type is not singleton: {0}", name);
                    }
                }
                else
                {
                    // Debug.LogErrorFormat("type not found: {0}", name);
                }
            }
        }

        public void OnBeginStateEnter(string currStateName, string nextStateName)
        {
            loadLevelExceptionCount = 0;

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
                        ShowExceptionPopup(e, evtOnExceptionPopupConfirm, "AddDynamicManager");
                    }
                }
            }

            for (int i = 0; i < constManagers.Count; i++)
            {
                IGameManager manager = constManagers[i];
                try
                {
                    manager.OnBeginStateEnter(currStateName, nextStateName);
                }
                catch (Exception e)
                {
                    ShowExceptionPopup(e, evtOnExceptionPopupConfirm, manager.GetType().Name);
                }
            }

            for (int i = 0; i < dynamicManagers.Count; i++)
            {
                IGameManager mgr = dynamicManagers[i];
                try
                {
                    mgr.OnBeginStateEnter(currStateName, nextStateName);
                }
                catch (Exception e)
                {
                    ShowExceptionPopup(e, evtOnExceptionPopupConfirm, mgr.GetType().Name);
                }
            }
            //PQEventManager.SendCommonMsg(PQEventType.EndLoadLevel, curLevelName, nextLevelName, curState);
        }

        public void OnEndStateExit(string currStateName, string nextStateName)
        {
            loadLevelExceptionCount = 0;
            for (int i = 0; i < constManagers.Count; i++)
            {
                IGameManager mamager = constManagers[i];
                try
                {
                    mamager.OnEndStateExit(currStateName, nextStateName);
                }
                catch (Exception e)
                {
                    ShowExceptionPopup(e, evtOnExceptionPopupConfirm, mamager.GetType().Name);
                }
            }
            DestroyDynamicManager();
            //PQEventManager.SendCommonMsg(PQEventType.BeginLoadLevel, curLevelName, nextLevelName, curState);
            Facade.Instance.ClearAll();
        }
    }
}
