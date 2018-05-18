using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common;
using AndroidSDK.Common;
using AndroidSDK.Common.Manifest;
using AndroidSDK.Options;

namespace BuildSystem.Options.OneSDK {

	/// <summary>
	/// 易接SDK 选项数据定义.
	/// </summary>
	[System.Serializable]
	public class OneSDKOptionsData : OptionBaseData {

		/// <summary>
		/// 易接自定义类名：用于闪屏后，跳转到游戏Main Activity名.
		/// </summary>
		public string ZyClassName = null;

		/// <summary>
		/// <meta-data/>节点数据列表.
		/// </summary>
		public List<MetaDataInfo> MetaDatas = new List<MetaDataInfo>();

		/// <summary>
		/// 初始化.
		/// </summary>
		public override void Init() {
			base.Init ();

			this.Option = TSDKOptions.OneSDK;
			this.ZyClassName = null;
			this.MetaDatas.Clear ();
		}

		/// <summary>
		/// 清空.
		/// </summary>
		public override void Clear() {
			base.Clear ();

			this.Option = TSDKOptions.None;
			this.ZyClassName = null;
			this.MetaDatas.Clear ();
		}
	}
}
