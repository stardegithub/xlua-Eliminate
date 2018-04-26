using System;
using System.Collections;
using System.Collections.Generic;
using GameSystem;
using UnityEngine;
using UnityEngine.Events;

namespace GameManager
{
    public class UIManager : GameManagerBase<UIManager>
    {
        private GameObject uiRoot;
        public GameObject UIRoot { get { return uiRoot; } set { uiRoot = value; } }

        private Camera uiCamera;
        public Camera UICamera { get { return uiCamera; } set { uiCamera = value; } }

        private List<UIInfo> uiInfoQueue;
        public List<UIInfo> UIInfoQueue { get { return uiInfoQueue; } }

        private Dictionary<string, UIInfo> uiInfoCachePool;
        public Dictionary<string, UIInfo> UIInfoCachePool { get { return uiInfoCachePool; } }

        private Dictionary<string, UIInfo> globalUIInfoPool;
        public Dictionary<string, UIInfo> GlobalUIInfoPool { get { return globalUIInfoPool; } }

        #region Singleton
        protected override void SingletonAwake()
        {
            uiInfoQueue = new List<UIInfo>();
            uiInfoCachePool = new Dictionary<string, UIInfo>();
            globalUIInfoPool = new Dictionary<string, UIInfo>();

            if (uiRoot == null)
            {
                uiRoot = GameObject.Find(GameConfig.Instance.uiRootName);
            }

            if (uiRoot == null)
            {
                CreateUIRoot();
            }

            if (uiCamera == null)
            {
                uiCamera = GameObject.Find(GameConfig.Instance.uiCameraName).GetComponent<Camera>();
            }

            // InitGlobalUIInfoPool();
            initialized = true;
        }

        protected override void SingletonDestroy()
        {
        }
        #endregion

        #region GlobalUI 
        public void CreateUIRoot()
        {
            var prefab = Resources.Load<GameObject>(GameConfig.Instance.uiRootPath);
            uiRoot = UnityEngine.Object.Instantiate(prefab);
            uiCamera = GameObject.Find(GameConfig.Instance.uiCameraName).GetComponent<Camera>();
            foreach (var uiInfo in globalUIInfoPool)
            {
                uiInfo.Value.SetParent(uiRoot.transform);
                uiInfo.Value.SetCamera(uiCamera);
            }

            foreach (var uiInfo in uiInfoCachePool)
            {
                uiInfo.Value.SetParent(uiRoot.transform);
                uiInfo.Value.SetCamera(uiCamera);
            }
        }

        public UIInfo PushGlobalUIInfo(string uiName)
        {
            var go = LoadAssetManager.Instance.LoadAsset<GameObject>(uiName);
            return CreateuGlobalUIInfo(uiName, go);
        }

        public LoadAssetsRequest PushGlobalUIInfoAsync(string uiName, Action<UIInfo> callBack)
        {
            return LoadAssetManager.Instance.LoadAssetAsync(uiName, delegate (LoadAssetInfo loadAssetInfo)
            {
                UIInfo uiInfo = null;
                if (loadAssetInfo != null && loadAssetInfo.assetObject != null)
                {
                    uiInfo = CreateuGlobalUIInfo(loadAssetInfo.assetPath, loadAssetInfo.assetObject as GameObject);
                }
                if (callBack != null) callBack(uiInfo);
            });
        }

        public LoadAssetsRequest PushGlobalUIInfoAsync(string[] uiNames, Action<UIInfo[]> callBack)
        {
            return LoadAssetManager.Instance.LoadAssetsAsync(uiNames, delegate (LoadAssetInfo[] loadAssetInfos)
            {
                UIInfo[] uiInfos = new UIInfo[loadAssetInfos.Length];
                for (int i = 0; i < loadAssetInfos.Length; i++)
                {
                    if (loadAssetInfos[i] != null && loadAssetInfos[i].assetObject != null)
                    {
                        uiInfos[i] = CreateuGlobalUIInfo(loadAssetInfos[i].assetPath, loadAssetInfos[i].assetObject as GameObject);
                    }
                }
                if (callBack != null) callBack(uiInfos);
            });
        }

        private UIInfo CreateuGlobalUIInfo(string globalUIInfoName, GameObject go)
        {
            if (go == null) return null;

            var uiInfo = new UIInfo(globalUIInfoName, go, uiCamera);
            uiInfo.SetParent(uiRoot.transform);
            var uiInfoPoolValues = new List<UIInfo>(globalUIInfoPool.Values);
            for (int i = 0; i < uiInfoPoolValues.Count; i++)
            {
                uiInfoPoolValues[i].TryAddRelationalUI(uiInfo);
            }
            uiInfo.TryAddRelationalUI(uiInfoPoolValues);
            globalUIInfoPool[globalUIInfoName] = uiInfo;

            return uiInfo;
        }
        #endregion

        #region UI 

        public UIInfo PushUIInfoCache(string uiName)
        {
            var go = LoadAssetManager.Instance.LoadAsset<GameObject>(uiName);
            return CreateuUIInfo(uiName, go);
        }

        public LoadAssetsRequest PushUIInfoCacheAsync(string uiName, Action<UIInfo> callBack)
        {
            return LoadAssetManager.Instance.LoadAssetAsync(uiName, delegate (LoadAssetInfo loadAssetInfo)
            {
                UIInfo uiInfo = null;
                if (loadAssetInfo != null && loadAssetInfo.assetObject != null)
                {
                    uiInfo = CreateuUIInfo(loadAssetInfo.assetPath, loadAssetInfo.assetObject as GameObject);
                }
                if (callBack != null) callBack(uiInfo);
            });
        }

        public LoadAssetsRequest PushUIInfoCacheAsync(string[] uiNames, Action<UIInfo[]> callBack)
        {
            return LoadAssetManager.Instance.LoadAssetsAsync(uiNames, delegate (LoadAssetInfo[] loadAssetInfos)
            {
                UIInfo[] uiInfos = new UIInfo[loadAssetInfos.Length];
                for (int i = 0; i < loadAssetInfos.Length; i++)
                {
                    if (loadAssetInfos[i] != null && loadAssetInfos[i].assetObject != null)
                    {
                        uiInfos[i] = CreateuUIInfo(loadAssetInfos[i].assetPath, loadAssetInfos[i].assetObject as GameObject);
                    }
                }
                if (callBack != null) callBack(uiInfos);
            });
        }

        private UIInfo CreateuUIInfo(string uiInfoName, GameObject go)
        {
            if (go == null) return null;

            var uiInfo = new UIInfo(uiInfoName, go, uiCamera);
            uiInfo.SetParent(uiRoot.transform);
            var uiInfoPoolValues = new List<UIInfo>(uiInfoCachePool.Values);
            for (int i = 0; i < uiInfoPoolValues.Count; i++)
            {
                uiInfoPoolValues[i].TryAddRelationalUI(uiInfo);
            }
            uiInfo.TryAddRelationalUI(uiInfoPoolValues);
            uiInfoCachePool[uiInfoName] = uiInfo;

            return uiInfo;
        }
        #endregion

        #region UIQueue
        public UIInfo GetCurrentUI()
        {
            if (uiInfoQueue.Count > 0)
            {
                return uiInfoQueue[uiInfoQueue.Count - 1];
            }
            else
            {
                return null;
            }
        }

        public UIInfo GetUI(string uiName)
        {
            return uiInfoQueue.Find(c => c.Name == uiName);
        }

        public bool HasOpen(string uiName)
        {
            var uiInfo = GetUI(uiName);
            return uiInfo != null && uiInfo.GameObject && uiInfo.GameObject.activeSelf;
        }

        public List<UIInfo> GetCurrentAllRelationalUIs()
        {
            UIInfo uiInfo = GetCurrentUI();
            if (uiInfo == null) return null;
            List<UIInfo> uiInfos = new List<UIInfo>();
            uiInfos.Add(uiInfo);
            GetAllRelationalUIs(uiInfo, ref uiInfos);
            return uiInfos;
        }

        public List<UIInfo> GetAllRelationalUIs(string uiName)
        {
            UIInfo uiInfo = GetUI(uiName);
            if (uiInfo == null) return null;
            List<UIInfo> uiInfos = new List<UIInfo>();
            uiInfos.Add(uiInfo);
            GetAllRelationalUIs(uiInfo, ref uiInfos);
            return uiInfos;
        }

        private void GetAllRelationalUIs(UIInfo uiInfo, ref List<UIInfo> container)
        {
            if (uiInfo.RelationalUIList != null && uiInfo.RelationalUIList.Count > 0)
            {
                foreach (var element in uiInfo.RelationalUIList)
                {
                    GetAllRelationalUIs(element, ref container);
                }
            }
        }

        public bool OpenUI(string uiName, bool isGlobalUI = false)
        {
            UIInfo uiInfo = null;
            if (!uiInfoCachePool.TryGetValue(uiName, out uiInfo))
            {
                globalUIInfoPool.TryGetValue(uiName, out uiInfo);
            }

            if (uiInfo == null)
            {
                if (isGlobalUI)
                {
                    uiInfo = PushGlobalUIInfo(uiName);
                }
                else
                {
                    uiInfo = PushUIInfoCache(uiName);
                }
            }

            if (uiInfo == null || (uiInfoQueue.Count > 0 && uiInfoQueue[uiInfoQueue.Count - 1].Name == uiName))
            {
                return false;
            }

            if (uiInfo.Open())
            {
                uiInfoQueue.Add(uiInfo);
                return true;
            }
            return false;
        }

        public void OpenUI(string uiName, IEnumerator routine, bool isGlobalUI = false)
        {
            StartCoroutine(OpenUI_Routine(uiName, routine, isGlobalUI));
        }

        private IEnumerator OpenUI_Routine(string uiName, IEnumerator routine, bool isGlobalUI = false)
        {
            if (OpenUI(uiName, isGlobalUI))
            {
                StartCoroutine(routine);
            }
            yield return null;
        }

        public bool CloseUI(bool destroy)
        {
            var uiInfo = GetCurrentUI();
            if (uiInfo != null && uiInfo.Close(destroy))
            {
                uiInfoQueue.Remove(uiInfo);
                return true;
            }
            return false;
        }

        public bool CloseUI(string uiName, bool destroy)
        {
            var uiInfo = GetUI(uiName);
            if (uiInfo != null && uiInfo.Close(destroy))
            {
                uiInfoQueue.Remove(uiInfo);
                return true;
            }
            return false;
        }

        public void CloseUI(bool destroy, IEnumerator routine)
        {
            StartCoroutine(CloseUI_Routine(destroy, routine));
        }

        private IEnumerator CloseUI_Routine(bool destroy, IEnumerator routine)
        {
            yield return StartCoroutine(routine);
            CloseUI(destroy);
        }

        public void CloseUI(string uiName, bool destroy, IEnumerator routine)
        {
            StartCoroutine(CloseUI_Routine(uiName, destroy, routine));
        }

        private IEnumerator CloseUI_Routine(string uiName, bool destroy, IEnumerator routine)
        {
            yield return StartCoroutine(routine);
            CloseUI(uiName, destroy);
        }

        public void ClearCachePool()
        {
            UnityEngine.Object.DestroyImmediate(uiRoot);

            foreach (var element in uiInfoCachePool)
            {
                element.Value.Close(true, true);
            }

            foreach (var element in globalUIInfoPool)
            {
                element.Value.Close(true, false);
            }
            uiInfoCachePool.Clear();
            uiInfoQueue.Clear();

            CreateUIRoot();
            uiCamera = GameObject.Find(GameConfig.Instance.uiCameraName).GetComponent<Camera>();
        }
        #endregion
    }
}
