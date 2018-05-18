using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using BuildSystem;
using AssetBundles;
using BuildSystem.Options;

namespace Common {

	/// <summary>
	/// BuildMode.
	/// </summary>
	public enum TBuildMode {
		/// <summary>
		/// 无.
		/// </summary>
		None,

		/// <summary>
		/// Debug模式.
		/// </summary>
		Debug,

		/// <summary>
		/// Release模式.
		/// </summary>
		Release,

		/// <summary>
		/// Store模式.
		/// </summary>
		Store,
		Max
	}

	/// <summary>
	/// 临时情报.
	/// </summary>
	[System.Serializable]
	public class TempInfo {
		public int BuildNumber = -1;

		/// <summary>
		/// 清空.
		/// </summary>
		public void Clear() {
			BuildNumber = -1;
		}
	}

	/// <summary>
	/// 打包数据.
	/// </summary>
	[System.Serializable]
	public class BuildDefaultData : JsonDataBase {
		/// <summary>
		/// BuildMode.
		/// </summary>
		public TBuildMode BuildMode;

		/// <summary>
		/// 打包名.
		/// </summary>
		public string BuildName;

		/// <summary>
		/// ID.
		/// </summary>
		public string BuildID;

		/// <summary>
		/// 版本号.
		/// </summary>
		public string BuildVersion;

		/// <summary>
		/// 版本号(Short).
		/// </summary>
		public string BuildShortVersion;

		/// <summary>
		/// 版本Code
		/// </summary>
		public int BuildVersionCode;

		/// <summary>
		/// 中心服务器版本.
		/// </summary>
		public string CenterVersion;

		/// <summary>
		/// Android SDK(只有打包Android的时候 才有效).
		/// </summary>
		public TPlatformType PlatformType;

		/// <summary>
		/// 临时信息.
		/// </summary>
		public TempInfo TempInfo = new TempInfo();

		/// <summary>
		/// 清空.
		/// </summary>
		public override void Clear() {
			base.Clear ();

			this.BuildMode = TBuildMode.Debug;
			this.BuildName = null;
			this.BuildID = null;
			this.BuildVersion = null;
			this.BuildShortVersion = null;
			this.BuildVersionCode = -1;
			this.CenterVersion = null;
			this.PlatformType = TPlatformType.None;
			this.TempInfo.Clear ();
		}
	}

	/// <summary>
	/// 选项定义.
	/// </summary>
	[System.Serializable]
	public class BuildOptionData : OptionsBaseData {}

	/// <summary>
	/// 打包信息.
	/// </summary>
	[System.Serializable]
	public class BuildInfoData : OptionsDataBase<BuildDefaultData, BuildOptionData> {}

	/// <summary>
	/// 打包信息设定类
	/// </summary>
	[System.Serializable]
	public class BuildInfo : AssetOptionsBase<BuildInfo, BuildInfoData, BuildDefaultData, BuildOptionData> {

		/// <summary>
		/// BuildMode.
		/// </summary>
		public TBuildMode BuildMode {
			get { 
				if (null != this.Data) {
					return this.Data.Default.BuildMode;
				}
				return TBuildMode.Debug;
			}
			set { 
				if (null != this.Data) {
					this.Data.Default.BuildMode = value;
				}
			}
		}

		/// <summary>
		/// 打包名.
		/// </summary>
		public string BuildName {
			get { 
				if (null != this.Data) {
					return this.Data.Default.BuildName;
				}
				return null;
			}
			set { 
				if (null != this.Data) {
					this.Data.Default.BuildName = value;
				}
			}
		}

		/// <summary>
		/// ID.
		/// </summary>
		public string BuildID {
			get { 
				if (null != this.Data) {
					return this.Data.Default.BuildID;
				}
				return null;
			}
			set {  
				if (null != this.Data) {
					this.Data.Default.BuildID = value;
				}
			}
		}

		/// <summary>
		/// 版本号.
		/// </summary>
		public string BuildVersion {
			get { 
				if (null != this.Data) {
					return this.Data.Default.BuildVersion;
				}
				return null;
			}
			set { 
				if (null != this.Data) {
					this.Data.Default.BuildVersion = value;
				}
			}
		}

		/// <summary>
		/// 版本号(Short).
		/// </summary>
		public string BuildShortVersion {
			get { 
				if (null != this.Data) {
					return this.Data.Default.BuildShortVersion;
				}
				return null;
			}
		}
			
		/// <summary>
		/// 版本Code
		/// </summary>
		public int BuildVersionCode {
			get { 
				if (null != this.Data) {
					return this.Data.Default.BuildVersionCode;
				}
				return -1;
			}
			set { 
				if (null != this.Data) {
					this.Data.Default.BuildVersionCode = value;
				}
			}
		}

		/// <summary>
		/// 中心服务器版本.
		/// </summary>
		public string CenterVersion {
			get { 
				if (null != this.Data) {
					return this.Data.Default.CenterVersion;
				}
				return null;
			}
			set { 
				if (null != this.Data) {
					this.Data.Default.CenterVersion = value;
				}
			}
		}

		/// <summary>
		/// 平台类型.
		/// </summary>
		public TPlatformType PlatformType {
			get { 
				if (null != this.Data) {
					return this.Data.Default.PlatformType;
				}
				return TPlatformType.None;
			}
			set { 
				if (null != this.Data) {
					this.Data.Default.PlatformType = value;
				}
			}
		}

		/// <summary>
		/// 打包号（Teamcity等CI上生成的打包号）.
		/// </summary>
		public int BuildNumber {
			get { 
				if ((null != this.Data) && (null != this.Data.Default.TempInfo)) {
					return this.Data.Default.TempInfo.BuildNumber;
				}
				return -1;
			} 
			set { 
				if ((null != this.Data) && (null != this.Data.Default.TempInfo)) {
					this.Data.Default.TempInfo.BuildNumber = value;
				}
			}
		}

		#region Implement

		/// <summary>
		/// 初始化数据.
		/// </summary>
		/// <returns><c>true</c>, OK, <c>false</c> NG.</returns>
		public override bool InitAsset () {

			// 打包ID
			if(string.IsNullOrEmpty(this.Data.Default.BuildID) == false) {
#if UNITY_EDITOR
#if UNITY_5_5_OR_NEWER
				PlayerSettings.applicationIdentifier = this.Data.Default.BuildID;
#else
				PlayerSettings.bundleIdentifier = this.Data.Default.BuildID;
#endif
#endif
			}

			// 版本号
			if(string.IsNullOrEmpty(this.Data.Default.BuildVersion) == false) {
#if UNITY_EDITOR && UNITY_EDITOR
				PlayerSettings.bundleVersion = this.Data.Default.BuildVersion;
#endif
			}

			// 版本号
			if(-1 != this.Data.Default.BuildVersionCode) {

#if UNITY_IOS && UNITY_EDITOR
				PlayerSettings.iOS.buildNumber = this.BuildVersionCode.ToString();
#endif
#if UNITY_ANDROID && UNITY_EDITOR
				PlayerSettings.Android.bundleVersionCode = this.Data.Default.BuildVersionCode;
#endif

			}

			this.Info ("Data : {0}", this.Data.ToString());

			return true;
		}

		/// <summary>
		/// 用用数据.
		/// </summary>
		/// <param name="iData">数据.</param>
		/// <param name="iForceClear">强制清空标志位.</param>
		protected override void ApplyData (BuildInfoData iData, bool iForceClear) {
			
			if (null == iData) {
				return;
			}

			// 清空
			if (true == iForceClear) {
				this.Clear ();
			}

			// 默认数据
			this.Data.Default.BuildName = iData.Default.BuildName;
			this.Data.Default.BuildID = iData.Default.BuildID;
			this.Data.Default.BuildVersion = iData.Default.BuildVersion;
			this.Data.Default.BuildShortVersion = iData.Default.BuildShortVersion;
			this.Data.Default.BuildVersionCode = iData.Default.BuildVersionCode;
			this.Data.Default.CenterVersion = iData.Default.CenterVersion;

			// 选项数据
			this.Data.Options.OptionsSettings = iData.Options.OptionsSettings;

			UtilsAsset.SetAssetDirty (this);
		}

		#endregion
	}
}
