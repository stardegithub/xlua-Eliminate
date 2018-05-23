using UnityEngine;
using System.Collections;
using Common;

namespace Device {

	/// <summary>
	/// 设备类型.
	/// </summary>
	public enum DeviceType {
		None,
		Unknow = None,

#region IOS

		/// <summary>
		/// iPhone4 或者更低版本.
		/// </summary>
		iPhone4Less,
		/// <summary>
		/// iPhone5.
		/// </summary>
		iPhone5,
		/// <summary>
		/// iPhone6.
		/// </summary>
		iPhone6,
		/// <summary>
		/// iPhone6 Plus.
		/// </summary>
		iPhone6Plus,

#endregion

#region IOS
		
#endregion

	}

	/// <summary>
	/// 显示安全范围信息.
	/// </summary>
	[System.Serializable]
	public sealed class DisplaySafePadding : JsonDataBase {

		/// <summary>
		/// 创建
		/// </summary>
		/// <param name="iLeft">边距：.</param>
		/// <param name="iRight">边距：.</param>
		/// <param name="iTop">边距：.</param>
		/// <param name="iBottom">边距：.</param>
		/// <param name="isRetina">是否为Retina.</param>
		/// <param name="iRetinaScale">Retina缩放率.</param>
		public static DisplaySafePadding Create(
			int iLeft, int iRight, int iTop, int iBottom, 
			bool iIsRetina = false, float iRetinaScale = 1.0f) {
			DisplaySafePadding _padding = new DisplaySafePadding ();
			if (null != _padding) {
				// 初始化
				_padding.Init ();
				_padding.Padding.left = iLeft;
				_padding.Padding.right = iRight;
				_padding.Padding.top = iTop;
				_padding.Padding.bottom = iBottom;
				_padding.isRetina = iIsRetina;
				_padding.RetinaScale = iRetinaScale;
			} else {
				_padding = null;
			}
			return _padding;
		}

		/// <summary>
		/// 构造函数（禁止外部New）.
		/// </summary>
		protected DisplaySafePadding() {
			
		}

		/// <summary>
		/// 边距.
		/// </summary>
		public RectOffset Padding = null;

		/// <summary>
		/// 是否为Retina屏幕.
		/// </summary>
		public bool isRetina = false;

		/// <summary>
		/// Retina屏幕缩放率.
		/// </summary>
		public double RetinaScale = 1.0f;

		/// <summary>
		/// 初始化.
		/// </summary>
		public override void Init() {
			base.Init ();

			Padding = new RectOffset ();
			Padding.left = 0;
			Padding.right = 0;
			Padding.top = 0;
			Padding.right = 0;

			isRetina = false;
			RetinaScale = 1.0f;
		}
			
		/// <summary>
		/// 清空.
		/// </summary>
		public override void Clear() {
			base.Clear ();

			Padding = null;
			isRetina = false;
			RetinaScale = 1.0f;
		}

		/// <summary>
		/// 边距是否都为0.
		/// </summary>
		/// <returns><c>true</c> 是; 否, <c>false</c>.</returns>
		public bool IsZero() {
			return ((0 == this.Padding.left) &&
				(0 == this.Padding.right) &&
				(0 == this.Padding.top) &&
				(0 == this.Padding.bottom));
		}

		/// <summary>
		/// 判断是否为全面屏.
		/// </summary>
		/// <returns><c>true</c> 是全面屏; 非全面屏, <c>false</c>.</returns>
		public bool IsFullScreen() {
			
			if ((0.0f == Padding.left) &&
				(0.0f == Padding.right) &&
				(0.0f == Padding.top) &&
				(0.0f == Padding.bottom)) {
				return false;
			}
			return true;
		}
			
		/// <summary>
		/// 比较.
		/// </summary>
		/// <param name="x">比较对象X.</param>
		/// <returns><c>true</c> 相等; 不相等, <c>false</c>.</returns>
		public bool Equal(DisplaySafePadding x) {
			if (null == x) {
				return false;
			}
			return ((this.Padding.left == x.Padding.left) && 
				(this.Padding.right == x.Padding.right) && 
				(this.Padding.top == x.Padding.top) && 
				(this.Padding.right == x.Padding.right) && 
				(this.isRetina == x.isRetina) && 
				(this.RetinaScale == x.RetinaScale));
		}

		public override string ToString ()
		{
			string _str = base.ToString ();
			return string.Format ("{0} isRetina:{1}({2}) Padding(L:{3} R:{4} T:{5} B:{6})",
				_str, isRetina, RetinaScale, Padding.left, Padding.right, Padding.top, Padding.bottom);
		}
	}

	/// <summary>
	/// 设备接口.
	/// </summary>
	public interface IDevice  {

		/// <summary>
		/// 取得边距余白（全面凭手机，因为有刘海，圆角等情况，需要调整显示范围。其他的安全区域即屏幕区域）.
		/// </summary>
		/// <returns>安全区域.</returns>
		/// <param name="iIsDebugTest">Debug测试标志位.</param>
		DisplaySafePadding GetSafePadding (bool iIsDebugTest = false);

		/// <summary>
		/// 判断是否为Retina屏幕.
		/// </summary>
		/// <returns><c>true</c> 是Retina; 非Retina, <c>false</c>.</returns>
		bool IsRetina ();

		/// <summary>
		/// 取得Retina屏幕的缩放率.
		/// </summary>
		/// <returns>Retina屏幕的缩放率.</returns>
		double GetRetinaScale ();

		/// <summary>
		/// 取得设备类型.
		/// </summary>
		/// <returns>设备类型.</returns>
		DeviceType GetDeviceType ();

		/// <summary>
		/// 取得屏幕大小：宽度.
		/// </summary>
		/// <returns>屏幕大小：宽度.</returns>
		double ScreenWidth ();

		/// <summary>
		/// 取得屏幕大小：高度.
		/// </summary>
		/// <returns>屏幕大小：高度.</returns>
		double ScreenHeight ();
	}
}
