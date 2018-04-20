using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using Common;

namespace Device {

	/// <summary>
	/// 设备信息.
	/// </summary>
	public class DeviceInfo : SingletonBase<DeviceInfo> {

		/// <summary>
		/// 硬件.
		/// </summary>
		private IDevice _device = null;

		/// <summary>
		/// 初始化.
		/// </summary>
		protected override void Init()
		{
			base.Init();

#if UNITY_IOS || UNITY_IPHONE
			_device = new iOSDevice();
#endif

#if UNITY_ANDROID
#endif
		}

		/// <summary>
		/// 取得边距余白（全面凭手机，因为有刘海，圆角等情况，需要调整显示范围。其他的安全区域即屏幕区域）.
		/// </summary>
		/// <returns>安全区域.</returns>
		/// <param name="iIsDebugTest">Debug测试标志位.</param>
		public DisplaySafePadding GetSafePadding (bool iIsDebugTest = false) {
			if (null == _device) {
				return DisplaySafePadding.Zero;
			}
			return _device.GetSafePadding (iIsDebugTest);
		}

		/// <summary>
		/// 判断是否为Retina屏幕.
		/// </summary>
		/// <returns><c>true</c> 是Retina; 非Retina, <c>false</c>.</returns>
		public bool IsRetina () {
			if (null == _device) {
				return false;
			}
			return _device.IsRetina ();
		}

		/// <summary>
		/// 取得Retina屏幕的缩放率.
		/// </summary>
		/// <returns>Retina屏幕的缩放率.</returns>
		public double GetRetinaScale () {
			if (null == _device) {
				return 1.0f;
			}
			return _device.GetRetinaScale ();
		}
	
		/// <summary>
		/// 取得设备类型.
		/// </summary>
		/// <returns>设备类型.</returns>
		public DeviceType GetDeviceType () {
			if (null == _device) {
				return DeviceType.Unknow;
			}
			return _device.GetDeviceType ();
		}
	}
}
