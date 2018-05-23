using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using AssetBundles;
using Common;
using GameState;
using GameState.Conf;
using EC.Common;
using EC.System;
using EC.UI.Component;

namespace EC.UI
{
	public class UIManager : ManagerMonoBehaviourBase<UIManager>
    {
        public GameObject UIRoot;

        public Camera UICamera;

        private Stack<UIComponentBase> _uiShowStack = new Stack<UIComponentBase>();
        private Dictionary<string, UIComponentBase> _uiMainPool = new Dictionary<string, UIComponentBase>();
        private Dictionary<string, UIComponentData> _uiPrefabPool = new Dictionary<string, UIComponentData>();

        #region Singleton
        protected override void SingletonAwake()
        {
			GameStateConf _stateConf = GameStateConf.GetInstance ();
			if (null == _stateConf) {
				this.Error ("SingletonAwake()::The game state conf is invalid!!!");
				return;
			}

            if (UIRoot == null)
            {
				UIRoot = GameObject.Find(_stateConf.Data.uiRootName);
            }

            if (UIRoot == null)
            {
                CreateUIRoot();
            }

            if (UICamera == null)
            {
				UICamera = GameObject.Find(_stateConf.Data.uiCameraName).GetComponent<Camera>();
            }

            _initialized = true;
        }

        protected override void SingletonDestroy()
        {
        }
        #endregion

        #region Global 
        /// <summary>
        /// 创建UI Root
        /// </summary>
        public void CreateUIRoot()
        {
			GameStateConf _stateConf = GameStateConf.GetInstance ();
			if (null == _stateConf) {
				this.Error ("CreateUIRoot()::The game state conf is invalid!!!");
				return;
			}

			var prefab = DataLoader.Load<GameObject>(_stateConf.Data.uiRootPath);
            if (prefab == null)
            {
				Error("CreateUIRoot(): UIRoot prefab cannot find: {0}", _stateConf.Data.uiRootPath);
                return;
            }

            UIRoot = UnityEngine.Object.Instantiate(prefab);
            UIRoot.name = prefab.name;
			UICamera = GameObject.Find(_stateConf.Data.uiCameraName).GetComponent<Camera>();
            foreach (var uiInfo in _uiMainPool)
            {
                uiInfo.Value.SetParent(UIRoot.transform);
                uiInfo.Value.SetCamera(UICamera);
            }
        }

        /// <summary>
		/// 加载ui预制体.
		/// </summary>
		/// <param name="uiPrefabName">UI预制体名.</param>
		public UIComponentData LoadUIPrefab(string uiPrefabName)
        {
            if (string.IsNullOrEmpty(uiPrefabName))
            {
                Error("PreLoad(): uiPrefabName is Empty");
                return null;
            }

            UIComponentData prefabData;
            if (!_uiPrefabPool.TryGetValue(uiPrefabName, out prefabData))
            {
                prefabData = UIComponentData.LoadData(uiPrefabName);
                if (prefabData != null)
                {
                    _uiPrefabPool.Add(uiPrefabName, prefabData);
                    foreach (var relationalUIName in prefabData.RelationalUINames)
                    {
                        LoadUIPrefab(relationalUIName);
                    }
                }
            }
            return prefabData;
        }

        /// <summary>
        /// UI全部清理.
        /// </summary>
        public void ClearAllUI()
        {
            List<UIComponentBase> uis = new List<UIComponentBase>(_uiShowStack);
            for (int i = 0; i < uis.Count; i++)
            {
                if (!uis[i].IsDontDestroy)
                {
                    uis[i].Close();
                    uis.RemoveAt(i);
                    i--;
                }
            }
            _uiShowStack.Clear();
            foreach (var ui in uis)
            {
                _uiShowStack.Push(ui);
            }
            uis = new List<UIComponentBase>(_uiMainPool.Values);
            foreach (var ui in uis)
            {
                if (!ui.IsDontDestroy)
                {
                    _uiMainPool.Remove(ui.name);
                    Destroy(ui.gameObject);
                }
            }
        }

        /// <summary>
        /// 清理缓存
        /// </summary>
        public void ClearPool()
        {
            foreach (var ui in _uiShowStack)
            {
                ui.Close();
            }
            foreach (var ui in _uiMainPool)
            {
                Destroy(ui.Value);
            }
            _uiShowStack.Clear();
            _uiMainPool.Clear();
        }
        #endregion

        #region UI
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uiPrefabName">UI预制体名</param>
        /// <returns></returns>
        private UIComponentBase CreateOrGetUI(string uiPrefabName)
        {
            UIComponentBase currUI;
            if (!_uiMainPool.TryGetValue(uiPrefabName, out currUI))
            {
                var prefabData = LoadUIPrefab(uiPrefabName);
                if (prefabData != null)
                {
                    currUI = prefabData.CreateUI();
                    if (currUI != null)
                    {
                        currUI.SetParent(UIRoot.transform);
                        currUI.SetCamera(UICamera);
                        _uiMainPool.Add(uiPrefabName, currUI);
                    }
                }
            }
            return currUI;
        }

        /// <summary>
        /// 打开UI
        /// </summary>
        /// <param name="uiPrefabName">UI预制体名</param>
        /// <returns></returns>
        public UIComponentBase OpenUI(string uiPrefabName)
        {
            UIComponentBase currUI;
            if (!_uiMainPool.TryGetValue(uiPrefabName, out currUI))
            {
                currUI = CreateOrGetUI(uiPrefabName);
                if (currUI != null)
                {
                    foreach (var relationalUIName in currUI.Data.RelationalUINames)
                    {
                        currUI.AddRelationalUI(CreateOrGetUI(relationalUIName));
                    }
                }
            }

            if (currUI != null)
            {
                // 取得上一层UI信息
                if (0 < this._uiShowStack.Count)
                {
                    UIComponentBase lastUI = null;
                    if (false == currUI.IsOverlapping)
                    {
                        lastUI = this._uiShowStack.Peek();
                        if (null != lastUI)
                        {
                            lastUI.Close();
                        }
                    }
                    // 替换UI的场合，把顶层所有Popup的UI移除后，在关闭上一UI
                    if (true == currUI.IsSwap)
                    {
                        while (true)
                        {
                            lastUI = this._uiShowStack.Peek();
                            if (null == lastUI)
                            {
                                break;
                            }
                            if (false == lastUI.IsOverlapping)
                            {
                                break;
                            }
                            lastUI = this._uiShowStack.Pop();
                            lastUI.Close();
                        }
                        if (null != lastUI)
                        {
                            lastUI.Close();
                        }
                    }
                }

                // 将新加载UI放入UI显示堆
                _uiShowStack.Push(currUI);
                currUI.Open();
            }
            return currUI;
        }

        /// <summary>
        /// 关闭UI
        /// </summary>
        public void CloseUI()
        {
            if (0 >= this._uiShowStack.Count)
            {
                this.Warning("CloseUI():There is no ui left in ui show stack!!");
                return;
            }
            // 推出UI显示栈
            UIComponentBase currUI = this._uiShowStack.Pop();
            // 推出UI显示栈
            if (null != currUI)
            {
                currUI.Close();
                // 替换的场合，重新打开上一个UI
                if (true == currUI.IsSwap)
                {
                    UIComponentBase _lastInfo = this._uiShowStack.Peek();
                    if (null != _lastInfo)
                    {
                        _lastInfo.Open();
                    }
                }
            }
        }

        /// <summary>
        /// 关闭UI（延迟）
        /// </summary>
        /// <param name="iDelayTime">延迟时间（单位：s）.</param>
        public void CloseUI(float iDelayTime)
        {
            StartCoroutine(DelayClose(iDelayTime));
        }

        /// <summary>
        /// 延迟关闭UI.
        /// </summary>
        /// <param name="delayTime">延迟时间（单位：s）.</param>
        private IEnumerator DelayClose(float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
            CloseUI();
        }

        /// <summary>
        /// 关闭UI(包含指定预制体) 
        /// </summary>
        /// <param name="uiPrefabName">UI预制体名.</param>
        public void CloseUI(string uiPrefabName)
        {
            if (string.IsNullOrEmpty(uiPrefabName))
            {
                this.Warning("CloseUI():The prefab name is invalid!!");
                return;
            }
            if (0 >= this._uiShowStack.Count)
            {
                this.Warning("CloseUI():There is no ui left in ui show stack!!");
                return;
            }
            UIComponentBase currUI = null;
            while (true)
            {
                // 推出UI显示栈
                currUI = this._uiShowStack.Peek();
                if (null == currUI)
                {
                    break;
                }
                if (true == uiPrefabName.Equals(currUI.name))
                {
                    currUI = this._uiShowStack.Pop();
                    currUI.Close();

                    currUI = this._uiShowStack.Peek();
                    if (null != currUI)
                    {
                        currUI.Open();
                    }
                    break;
                }
                currUI = this._uiShowStack.Pop();
                currUI.Close();
            }
        }

        /// <summary>
        /// 关闭UI 
        /// </summary>
        /// <param name="uiPrefabName">预制体名.</param>
        /// <param name="iDelayTime">延迟时间（单位：s）.</param>
        public void CloseUI(string uiPrefabName, float iDelayTime)
        {
            StartCoroutine(DelayClose(uiPrefabName, iDelayTime));
        }

        /// <summary>
        /// 延迟关闭UI
        /// </summary>
        /// <param name="uiPrefabName">预制体名.</param>
        /// <param name="delayTime">延迟时间（单位：s）.</param>
        private IEnumerator DelayClose(string uiPrefabName, float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
            CloseUI(uiPrefabName);
        }

        /// <summary>
        /// 关闭UI(不包含指定预制体) 
        /// </summary>
        /// <param name="uiPrefabName">UI预制体名.</param>
        public void CloseUITo(string uiPrefabName)
        {
            if (string.IsNullOrEmpty(uiPrefabName))
            {
                this.Warning("CloseUI():The prefab name is invalid!!");
                return;
            }

            if (0 >= this._uiShowStack.Count)
            {
                this.Warning("CloseUI():There is no ui left in ui show stack!!");
                return;
            }
            UIComponentBase currUI = null;
            while (true)
            {
                // 推出UI显示栈
                currUI = this._uiShowStack.Peek();
                if (null == currUI)
                {
                    break;
                }
                if (true == uiPrefabName.Equals(currUI.name))
                {
                    currUI.Open();
                    break;
                }
                currUI = this._uiShowStack.Pop();
                currUI.Close();
            }
        }

        /// <summary>
        /// 关闭UI(不包含指定预制体)
        /// </summary>
        /// <param name="uiPrefabName">UI预制体名.</param>
        /// <param name="delayTime">延迟时间（单位：s）.</param>
        public void CloseUITo(string uiPrefabName, float delayTime)
        {
            StartCoroutine(DelayCloseTo(uiPrefabName, delayTime));
        }

        /// <summary>
        /// 延迟关闭UI(不包含指定预制体).
        /// </summary>
        /// <param name="uiPrefabName">UI预制体名.</param>
        /// <param name="delayTime">延迟时间（单位：s）.</param>
        private IEnumerator DelayCloseTo(string uiPrefabName, float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
            CloseUITo(uiPrefabName);
        }

        /// <summary>
		/// 取得最上层的UI
		/// </summary>
		/// <returns>最上层的UI.</returns>
		public UIComponentBase GetAboveUI()
        {
            if (0 >= this._uiShowStack.Count)
            {
                return null;
            }
            UIComponentBase currUI = this._uiShowStack.Peek();
            if (null == currUI)
            {
                return null;
            }
            return currUI;
        }

        /// <summary>
        /// 取得指定的UI
        /// </summary>
        /// <param name="uiPrefabName">UI预制体名.</param>
        /// <returns>指定的UI.</returns>
        public UIComponentBase GetUI(string uiPrefabName)
        {
            UIComponentBase uiComponent;
            _uiMainPool.TryGetValue(uiPrefabName, out uiComponent);
            return uiComponent;
        }

        /// <summary>
        /// UI是否已经打开
        /// </summary>
        /// <param name="uiPrefabName">UI预制体名.</param>
        /// <returns></returns>
        public bool HasOpenUI(string uiPrefabName)
        {
            var uiComponent = GetUI(uiPrefabName);

            return uiComponent.IsOpen;
        }
        #endregion

#if UNITY_ANDROID
        /// <summary>
        /// Android模拟器CD计数器.
        /// </summary>
        private float lastPressTime = 0;

        protected void LateUpdate()
        {
            if (Time.time - lastPressTime < 0.1f)
            {
                return;
            }

            // Android下返回键
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                this.Info("LateUpdate(): Enter Escape(Back Key)");

                // 返回键按下
                this.OnBackKeyPressed();
                lastPressTime = Time.time;
            }

#if ANDROID_SIMULATOR
            // 左Shift + R 或者右Shift + R
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.R) ||
                Input.GetKey(KeyCode.RightShift) && Input.GetKey(KeyCode.R))
            {
                this.Info("LateUpdate(): Enter Escape(Back Key - Android Simulator)");
                // 返回键按下
                this.OnBackKeyPressed();
                lastPressTime = Time.time;
            }
#endif
        }

        /// <summary>
        /// 返回键按下事件.
        /// </summary>
        private void OnBackKeyPressed()
        {
            this.Info("OnBackKeyPressed()");

            if (false == this.InnerBackUI())
            {
                Application.Quit();
            }
        }

        /// <summary>
        /// 返回.
        /// </summary>
        /// <returns><c>true</c>, 返回成功, <c>false</c> 无上一步可返回／无需返回.</returns>
        private bool InnerBackUI()
        {
            if (0 < this._uiShowStack.Count)
            {

                UIComponentBase currUI = this._uiShowStack.Peek();
                if (null == currUI)
                {
                    return false;
                }
                if (false == currUI.IsBackAble)
                {
                    return false;
                }
                currUI = this._uiShowStack.Pop();
                if (null == currUI)
                {
                    return false;
                }
                currUI.Close();

                // 替换的场合，回复上一个UI
                if (true == currUI.IsSwap)
                {
                    currUI = this._uiShowStack.Peek();
                    currUI.Open();
                }

                return true;
            }

            return false;
        }
#endif
    }
}
