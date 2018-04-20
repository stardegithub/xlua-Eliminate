using UnityEngine;
using System;
using System.Collections;

#if UNITY_IOS || UNITY_IPHONE
using System.Runtime.InteropServices;
#endif

using Common;

namespace Device {

#if UNITY_IOS || UNITY_IPHONE

	/// <summary>
	/// 设备(iOS).
	/// </summary>
	public class iOSDevice : ClassExtension, IDevice {

#region DllImport

		[DllImport("__Internal")]
		private extern static void _GetSafePaddingImpl(
			out float ioLeft, out float ioRight, out float ioTop, out float ioBottom);

		[DllImport("__Internal")]
		private extern static double _GetRetinaScale();

		[DllImport("__Internal")]
		private extern static bool _IsRetina();

		[DllImport("__Internal")]
		private extern static double _ScreenWidth();

		[DllImport("__Internal")]
		private extern static double _ScreenHeight();

#endregion

		/// <summary>
		/// 取得边距余白（全面凭手机，因为有刘海，圆角等情况，需要调整显示范围。其他的安全区域即屏幕区域）.
		/// </summary>
		/// <returns>安全区域.</returns>
		/// <param name="iIsDebugTest">Debug测试标志位.</param>
		public DisplaySafePadding GetSafePadding (bool iIsDebugTest = false) {

			DisplaySafePadding _safePadding = DisplaySafePadding.Create (0, 0, 0, 0);
			if (null == _safePadding) {
				this.Error ("GetSafePadding():Failed!!!");
				return null;
			}

			if(true == Application.isMobilePlatform) {
				// Retina
				_safePadding.isRetina = this.IsRetina();
				_safePadding.RetinaScale = this.GetRetinaScale ();

				// 取得安全区域
				float _left = 0.0f;
				float _right = 0.0f;
				float _top = 0.0f;
				float _bottom = 0.0f;
				_GetSafePaddingImpl(
					out _left, out _right, 
					out _top, out _bottom);
				_safePadding.Padding.left = (int)_left;
				_safePadding.Padding.right = (int)_right;
				_safePadding.Padding.top = (int)_top;
				_safePadding.Padding.bottom = (int)_bottom;
			}

#if BUILD_DEBUG
			if((true == iIsDebugTest) && 
			   (false == Application.isMobilePlatform)) {
				// 测试iPhoneX的尺寸数据（去掉刘海和圆角的数据）
				_safePadding.Padding.left = 44;
				_safePadding.Padding.right = 44;
				_safePadding.Padding.top = 0;
				_safePadding.Padding.bottom = 21;
				_safePadding.isRetina = true;
				_safePadding.RetinaScale = 3.0f;
			}
#endif
			Rect a;
			this.Info ("GetSafePadding():{0}", _safePadding.ToString());
			return _safePadding;
		}

		/// <summary>
		/// 判断是否为Retina屏幕.
		/// </summary>
		/// <returns><c>true</c> 是Retina; 非Retina, <c>false</c>.</returns>
		public bool IsRetina () {
			bool _isRetina = false;
			if (true == Application.isMobilePlatform) {
				_isRetina = _IsRetina ();
			}
			this.Info ("IsRetina():{0}", _isRetina);
			return _isRetina;
		}

		/// <summary>
		/// 取得Retina屏幕的缩放率.
		/// </summary>
		/// <returns>Retina屏幕的缩放率.</returns>
		public double GetRetinaScale () {
			double _retinaScale = 1.0f;
			if (true == Application.isMobilePlatform) {
				_retinaScale = _GetRetinaScale ();
			}
			this.Info ("GetRetinaScale():{0}", _retinaScale);
			return _retinaScale;
		}

		/// <summary>
		/// 取得设备类型.
		/// </summary>
		/// <returns>设备类型.</returns>
		public DeviceType GetDeviceType () {
			return DeviceType.None;
		}
			
		/// <summary>
		/// 取得屏幕大小：宽度.
		/// </summary>
		/// <returns>屏幕大小：宽度.</returns>
		public double ScreenWidth () {
			if (true == Application.isMobilePlatform) {
				return _ScreenWidth ();
			}
			return Screen.width;
		}

		/// <summary>
		/// 取得屏幕大小：高度.
		/// </summary>
		/// <returns>屏幕大小：高度.</returns>
		public double ScreenHeight () {
			if (true == Application.isMobilePlatform) {
				return _ScreenHeight ();
			}
			return Screen.height;
		}
	}

#endif
}
