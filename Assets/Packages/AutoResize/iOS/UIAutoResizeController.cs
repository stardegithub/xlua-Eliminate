using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using Common;
using Device;

namespace AutoResize {

	/// <summary>
	/// 自适应控制器.
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	[AddComponentMenu("Common/UI/UIAutoResizeController")]
	public class UIAutoResizeController : MonoBehaviourExtension {

#if BUILD_DEBUG

		/// <summary>
		/// Debug 测试标志位.
		/// </summary>
		public bool IsDebugTest = false;
		private bool _lastIsDebugTest = false;

#endif

		/// <summary>
		/// 边距.
		/// </summary>
		public RectOffset Padding = new RectOffset (0, 0, 0, 0);

		/// <summary>
		/// 位置偏移.
		/// </summary>
		public Vector2 PosOffset = new Vector2 (0.0f, 0.0f);

		private RectTransform _uiPanel = null;

		/// <summary>
		/// 上一个安全边距.
		/// </summary>
		private DisplaySafePadding _defaultPadding = DisplaySafePadding.Create(0, 0, 0, 0);
		private bool _applyed = false;

		void Awake () {
						
			_uiPanel = this.GetComponent<RectTransform> ();
			if (null == _uiPanel) {
				this.Error ("Awake():There is no component which is RectTransform!!!");
			}
		}

		void Start() {
			_applyed = false;
		}

		/// <summary>
		/// 应用安全区域.
		/// </summary>
		/// <param name="iSafePadding">安全边距.</param>
		private void ApplySafePadding(DisplaySafePadding iSafePadding)
		{
			this.Info ("ApplySafePadding():{0}", iSafePadding.ToString());
				
			if (true == DisplaySafePadding.Zero.Equal (this._defaultPadding)) {
				this._defaultPadding.Padding.left = (int)this._uiPanel.offsetMin.x;
				this._defaultPadding.Padding.right = (int)this._uiPanel.offsetMax.x;
				this._defaultPadding.Padding.top = (int)this._uiPanel.offsetMax.y;
				this._defaultPadding.Padding.bottom = (int)this._uiPanel.offsetMin.y;
				this._defaultPadding.isRetina = iSafePadding.isRetina;
				this._defaultPadding.RetinaScale = iSafePadding.RetinaScale;
			}

			// 设定左右边距
			if (null != this._uiPanel) {
				this._uiPanel.pivot = new Vector2 (0.5f, 0.5f);
				// 上边距
				{
					float _temp = this._defaultPadding.Padding.top;
					if (true == iSafePadding.IsFullScreen ()) {
						_temp += iSafePadding.Padding.top;
						_temp += this.Padding.top;
					}
					this._uiPanel.offsetMax = new Vector2 (this._uiPanel.offsetMax.x, -_temp);
				}
				// 右边距
				{
					float _temp = this._defaultPadding.Padding.right;
					if (true == iSafePadding.IsFullScreen ()) {
						_temp += iSafePadding.Padding.right;
						_temp += this.Padding.right;
					}
					this._uiPanel.offsetMax = new Vector2 (-_temp, this._uiPanel.offsetMax.y);
				}

				// 下边距
				{
					float _temp = this._defaultPadding.Padding.bottom;
					if (true == iSafePadding.IsFullScreen ()) {
						_temp += iSafePadding.Padding.bottom;
						_temp += this.Padding.bottom;
						_temp += this.PosOffset.y;
					}
					this._uiPanel.offsetMin = new Vector2 (this._uiPanel.offsetMin.x, iSafePadding.Padding.bottom);
				}
				// 左边距
				{
					float _temp = this._defaultPadding.Padding.left;
					if (true == iSafePadding.IsFullScreen ()) {
						_temp += iSafePadding.Padding.left;
						_temp += this.Padding.left;
						_temp += this.PosOffset.x;
					}
					this._uiPanel.offsetMin = new Vector2 (_temp, this._uiPanel.offsetMin.y);
				}
			}
		}
			
		/// <summary>
		/// 更新.
		/// </summary>
		void Update () 
		{
			if (null == this._uiPanel) {
				return;
			}

			// 判断安全范围是否改变了
			if(true == this._applyed) {
				return;
			}

			// 取得安全区域
			DisplaySafePadding _safeArea = null;
#if BUILD_DEBUG
			_safeArea = DeviceInfo.Instance.GetSafePadding(this.IsDebugTest);
#else
			_safeArea = DeviceInfo.Instance.GetSafePadding();
#endif
			if ((null != _safeArea) && 
				(false == this._defaultPadding.Equal(_safeArea))) {
				this._applyed = false;
			}

			// 应用安全区域
			if (false == this._applyed) {
				ApplySafePadding (_safeArea);
				this._applyed = true;
			}
		}


#if BUILD_DEBUG

		void LateUpdate() {

			if (this._lastIsDebugTest == this.IsDebugTest) {
				return;
			}
			this._lastIsDebugTest = this.IsDebugTest;

			// 显示／隐藏 安全区域
			this.ShowOrHiddenSafeArea();
		}

		/// <summary>
		/// 显示／隐藏 安全区域.
		/// </summary>
		private void ShowOrHiddenSafeArea() {
			
			this.Info ("ShowOrHiddenSafeArea():{0} -> {1}", 
				this.IsDebugTest, !this.IsDebugTest);

			if (true == this.IsDebugTest) {

				Image _debugArea = this.gameObject.AddComponent<Image> ();
				if (null != _debugArea) {
					_debugArea.color = new Color (0.0f, 1.0f, 0.0f, 0.1f);
				}
			} else {
				Image _debugArea = this.GetComponent<Image> ();
				if (null != _debugArea) {
					DestroyObject (_debugArea);
				}
			}
		}

#endif

#if UNITY_EDITOR

		/// <summary>
		/// 应用设定.
		/// </summary>
		public void Apply() {
			this._applyed = false;
		}

#endif

	}
}
