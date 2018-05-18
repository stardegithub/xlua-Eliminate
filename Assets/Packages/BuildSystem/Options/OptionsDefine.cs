using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common;
using BuildSystem.Options.OneSDK;

namespace BuildSystem.Options {

	/// <summary>
	/// Android SDK 追加选项.
	/// </summary>
	public enum TSDKOptions {
		/// <summary>
		/// 无.
		/// </summary>
		None = 0x00000000,
		/// <summary>
		/// 易接SDK.
		/// </summary>
		OneSDK = 0x00000001
	}

	/// <summary>
	/// 选项数据(Base).
	/// </summary>
	[System.Serializable]
	public class OptionBaseData : JsonDataBase {
		/// <summary>
		/// 选项.
		/// </summary>
		public TSDKOptions Option = TSDKOptions.None;
	}

	/// <summary>
	/// 选项定义.
	/// </summary>
	[System.Serializable]
	public class OptionsBaseData : JsonDataBase {
		/// <summary>
		/// 指定选项.
		/// </summary>
		public int OptionsSettings = 0;

		/// <summary>
		/// 初始化.
		/// </summary>
		public override void Init() {
			base.Init ();
			this.OptionsSettings = (int)TSDKOptions.None;
		}

		/// <summary>
		/// 清空.
		/// </summary>
		public override void Clear() {
			base.Clear ();

			this.OptionsSettings = (int)TSDKOptions.None;
		}
			
		/// <summary>
		/// 设定选项.
		/// </summary>
		/// <param name="iOption">选项.</param>
		/// <param name="iIsOn">true:On; false:Off.</param>
		public void SetOptionOnOrOff(TSDKOptions iOption, bool iIsOn) {
			if (true == iIsOn) {
				this.OptionsSettings |= (int)iOption;
			} else {
				this.OptionsSettings &= (~(int)iOption);
			}
		}

		/// <summary>
		/// 判断选项是否有效.
		/// </summary>
		/// <returns><c>true</c>, 有效, <c>false</c> 无效.</returns>
		/// <param name="iOption">选项.</param>
		public bool isOptionValid(TSDKOptions iOption) {
			return ((this.OptionsSettings & (int)iOption) == ((int)iOption));
		}
	}

	/// <summary>
	/// 选项定义.
	/// </summary>
	[System.Serializable]
	public class BuildSettingOptionsData : OptionsBaseData {

		/// <summary>
		/// 易接SDK选项.
		/// </summary>
		public OneSDKOptionsData OneSDK = new OneSDKOptionsData();

		/// <summary>
		/// 初始化.
		/// </summary>
		public override void Init() {
			base.Init ();

			this.OneSDK.Clear ();
		}
			
		/// <summary>
		/// 清空.
		/// </summary>
		public override void Clear() {
			base.Clear ();

			this.OneSDK.Clear ();
		}
	}

}