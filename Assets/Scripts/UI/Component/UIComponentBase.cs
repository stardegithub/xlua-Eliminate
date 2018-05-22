using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using AssetBundles;
using Common;
using EC.System;

namespace EC.UI.Component
{
    /// <summary>
	/// UI类型.
	/// </summary>
	public enum UIType
    {
        None,
        /// <summary>
        /// 一般.
        /// </summary>
        Normal,
        /// <summary>
        /// 弹窗.
        /// </summary>
        Popup,
        /// <summary>
        /// 提示.
        /// </summary>
        Tips
    }

    /// <summary>
    /// UI选项.
    /// </summary>
    public enum UIOptions
    {
        None = 0x00000000,
        /// <summary>
        /// 是否可返回（目前仅Android下有效。用以判断当前UI是否对，Android下返回键是否有效）.
        /// </summary>
        BackAble = 0x00000001,
        /// <summary>
        /// 堆叠（按UI加载顺序，一次从先往后依次堆叠。允许多层UI同时显示）.
        /// </summary>
        Overlapping = 0x00000002,
        /// <summary>
        /// 替换（加载当前UI的同时，关闭前一UI，关闭当前UI，则重新显示上一UI）.
        /// </summary>
        Swap = 0x00000004,
        /// <summary>
        /// 不释放(加载前后，无论是否场景切换，当前对象都不释放。但是可以激活(SetActive(true/false))).
        /// </summary>
        DontDestroy = 0x00010000
    }

    [Serializable]
    public class UIComponentData : JsonDataBase
    {
        /// <summary>
        /// 名字，做key
        /// </summary>
        public string Name;
        /// <summary>
        /// 预制体
        /// </summary>
        public UIComponentBase Prefab;
        /// <summary>
        /// 类型.
        /// </summary>
        public UIType Type = UIType.None;
        /// <summary>
        /// 选项.
        /// </summary>
        public int Options = (int)UIOptions.None;
        /// <summary>
        /// 关联UI
        /// </summary>
        public List<string> RelationalUINames = new List<string>();

        /// <summary>
        /// 是否可返回.
        /// </summary>
        public virtual bool IsBackAble
        {
            get
            {
                return ((this.Options & ((int)UIOptions.BackAble)) ==
                    ((int)UIOptions.BackAble));
            }
        }

        /// <summary>
        /// 是否可堆叠.
        /// </summary>
        public virtual bool IsOverlapping
        {
            get
            {
                return ((this.Options & ((int)UIOptions.Overlapping)) ==
                    ((int)UIOptions.Overlapping));
            }
        }

        /// <summary>
        /// 是否可替换.
        /// </summary>
        public virtual bool IsSwap
        {
            get
            {
                return ((this.Options & ((int)UIOptions.Swap)) ==
                    ((int)UIOptions.Swap));
            }
        }

        /// <summary>
        /// 是否不用释放.
        /// </summary>
        public virtual bool IsDontDestroy
        {
            get
            {
                return ((this.Options & ((int)UIOptions.DontDestroy)) ==
                    ((int)UIOptions.DontDestroy));
            }
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <returns></returns>
        public virtual UIComponentBase CreateUI()
        {
            if (Prefab == null)
            {
                this.Error("CreateUI():Prefab do not exist", Name);
                return null;
            }
			var ui = UnityEngine.Object.Instantiate(Prefab);
            ui.name = this.Name;
            return ui;
        }


        /// 初始化.
        /// </summary>
        public override void Init()
        {
            base.Init();
        }

        /// <summary>
        /// 清空.
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            Name = null;
            Type = UIType.None;
            Options = (int)UIOptions.None;
            RelationalUINames.Clear();
        }

        public override string ToString()
        {
            string _str = base.ToString();
            _str = string.Format("{0} Name:{1} Type:{2} BackAble:{3} Overlapping:{4} Swap:{5} DontDestroy:{6} RelationalUIs Count:{7}",
                _str, this.Name, this.Type,
                this.IsBackAble.ToString(), this.IsOverlapping.ToString(), this.IsSwap.ToString(), this.IsDontDestroy.ToString(),
                this.RelationalUINames.Count);
            return _str;
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        /// <returns></returns>
        public static UIComponentData LoadData(string uiPrefabName)
        {
            var prefab = DataLoader.Load<GameObject>(uiPrefabName);
            if (prefab == null)
            {
                UtilsLog.Error("UIComponentData.LoadData():Prefab Load Failed!!!(Path:{0})", uiPrefabName);
                return null;
            }
            var prefabComponent = prefab.GetComponent<UIComponentBase>();
            if (prefabComponent == null)
            {
                UtilsLog.Error("UIComponentData.LoadData():Prefab do not exist UIComponentBase !!!(Path:{0})", uiPrefabName);
                return null;
            }
            if (prefabComponent.Data.Prefab == null)
            {
                prefabComponent.Data.Prefab = prefabComponent;
            }
            return prefabComponent.Data;
        }
    }

    public class UIComponentBase : MonoBehaviourExtension
    {
        public UIComponentData Data;

        /// <summary>
        /// 父物体
        /// </summary>
        protected Transform _parent;
        /// <summary>
        /// 渲染相机
        /// </summary>
        protected Camera _renderCamera;
        /// <summary>
        /// 关联计数器（<=0时，关闭UI）.
        /// </summary>
        protected int ReferenceCount = 0;
        protected List<UIComponentBase> RelationalUIs = new List<UIComponentBase>();

        /// <summary>
        /// 是否可返回.
        /// </summary>
        public virtual bool IsBackAble
        {
            get
            {
                return Data.IsBackAble;
            }
        }

        /// <summary>
        /// 是否可堆叠.
        /// </summary>
        public virtual bool IsOverlapping
        {
            get
            {
                return Data.IsOverlapping;
            }
        }

        /// <summary>
        /// 是否可替换.
        /// </summary>
        public virtual bool IsSwap
        {
            get
            {
                return Data.IsSwap;
            }
        }

        /// <summary>
        /// 是否不用释放.
        /// </summary>
        public virtual bool IsDontDestroy
        {
            get
            {
                return Data.IsDontDestroy;
            }
        }

        public virtual bool IsOpen
        {
            get
            {
                return ReferenceCount > 0;
            }
        }

        protected virtual void Awake()
        {
            if (IsDontDestroy && GetComponent<DontDestroy>() == null)
            {
                gameObject.AddComponent<DontDestroy>();
            }
        }

        /// <summary>
        /// 设定父物体
        /// </summary>
        /// <param name="parent"></param>
        public void SetParent(Transform parent)
        {
            this._parent = parent;
            if (gameObject)
            {
                transform.SetParent(parent, false);
            }
        }

        /// <summary>
        /// 设定渲染相机
        /// </summary>
        /// <param name="camera"></param>
        public void SetCamera(Camera camera)
        {
            this._renderCamera = camera;
            if (gameObject)
            {
                var canvas = GetComponent<Canvas>();
                if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                {
                    if (_renderCamera != null && _renderCamera.gameObject.activeSelf)
                    {
                        canvas.worldCamera = _renderCamera;
                    }
                    else
                    {
                        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
                            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    }
                }
            }
        }

        /// <summary>
        /// 添加关联UI
        /// </summary>
        /// <param name="UIComponentBase"></param>
        public void AddRelationalUI(UIComponentBase ui)
        {
            RelationalUIs.Add(ui);
        }

        /// <summary>
        /// 打开
        /// </summary>
        public void Open()
        {
            ++ReferenceCount;
            if (!gameObject)
            {
                Data.CreateUI();
            }
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
            foreach (var relationalUI in RelationalUIs)
            {
                relationalUI.Open();
            }
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        {
            --ReferenceCount;
            ReferenceCount = (0 >= ReferenceCount) ? 0 : ReferenceCount;

            // 被关闭
            if ((null != this) && (0 >= ReferenceCount))
            {
                this.gameObject.SetActive(false);
                this.Info("Close(): <- {0}({1})", this.Data.Name, this.ReferenceCount);
            }

            foreach (var relationalUI in RelationalUIs)
            {
                relationalUI.Close();
            }
        }
    }
}