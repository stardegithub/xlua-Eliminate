using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using BuildSystem;
using AssetBundles;

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
	public class BuildInfoData : JsonDataBase {
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
	/// 打包信息设定类
	/// </summary>
	[System.Serializable]
	public class BuildInfo : AssetBase<BuildInfo, BuildInfoData> {

		/// <summary>
		/// BuildMode.
		/// </summary>
		public TBuildMode BuildMode {
			get { 
				if (null != this.Data) {
					return this.Data.BuildMode;
				}
				return TBuildMode.Debug;
			}
			set { 
				if (null != this.Data) {
					this.Data.BuildMode = value;
				}
			}
		}

		/// <summary>
		/// 打包名.
		/// </summary>
		public string BuildName {
			get { 
				if (null != this.Data) {
					return this.Data.BuildName;
				}
				return null;
			}
			set { 
				if (null != this.Data) {
					this.Data.BuildName = value;
				}
			}
		}

		/// <summary>
		/// ID.
		/// </summary>
		public string BuildID {
			get { 
				if (null != this.Data) {
					return this.Data.BuildID;
				}
				return null;
			}
			set {  
				if (null != this.Data) {
					this.Data.BuildID = value;
				}
			}
		}

		/// <summary>
		/// 版本号.
		/// </summary>
		public string BuildVersion {
			get { 
				if (null != this.Data) {
					return this.Data.BuildVersion;
				}
				return null;
			}
			set { 
				if (null != this.Data) {
					this.Data.BuildVersion = value;
				}
			}
		}

		/// <summary>
		/// 版本号(Short).
		/// </summary>
		public string BuildShortVersion {
			get { 
				if (null != this.Data) {
					return this.Data.BuildShortVersion;
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
					return this.Data.BuildVersionCode;
				}
				return -1;
			}
			set { 
				if (null != this.Data) {
					this.Data.BuildVersionCode = value;
				}
			}
		}

		/// <summary>
		/// 中心服务器版本.
		/// </summary>
		public string CenterVersion {
			get { 
				if (null != this.Data) {
					return this.Data.CenterVersion;
				}
				return null;
			}
			set { 
				if (null != this.Data) {
					this.Data.CenterVersion = value;
				}
			}
		}

		/// <summary>
		/// 平台类型.
		/// </summary>
		public TPlatformType PlatformType {
			get { 
				if (null != this.Data) {
					return this.Data.PlatformType;
				}
				return TPlatformType.None;
			}
			set { 
				if (null != this.Data) {
					this.Data.PlatformType = value;
				}
			}
		}

		/// <summary>
		/// 打包号（Teamcity等CI上生成的打包号）.
		/// </summary>
		public int BuildNumber {
			get { 
				if ((null != this.Data) && (null != this.Data.TempInfo)) {
					return this.Data.TempInfo.BuildNumber;
				}
				return -1;
			} 
			set { 
				if ((null != this.Data) && (null != this.Data.TempInfo)) {
					this.Data.TempInfo.BuildNumber = value;
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
			if(string.IsNullOrEmpty(this.Data.BuildID) == false) {
#if UNITY_EDITOR
#if UNITY_5_5_OR_NEWER
                PlayerSettings.applicationIdentifier = this.Data.BuildID;
#else
				PlayerSettings.bundleIdentifier = this.Data.BuildID;
#endif
#endif
			}

			// 版本号
			if(string.IsNullOrEmpty(this.Data.BuildVersion) == false) {
#if UNITY_EDITOR && UNITY_EDITOR
				PlayerSettings.bundleVersion = this.Data.BuildVersion;
#endif
			}

			// 版本号
			if(-1 != this.Data.BuildVersionCode) {

#if UNITY_IOS && UNITY_EDITOR
				PlayerSettings.iOS.buildNumber = this.BuildVersionCode.ToString();
#endif
#if UNITY_ANDROID && UNITY_EDITOR
				PlayerSettings.Android.bundleVersionCode = this.Data.BuildVersionCode;
#endif

			}

			UtilsLog.Info ("BuildInfo", "BuildName : {0}", (this.Data.BuildName == null) ? "null" : this.Data.BuildName);
			UtilsLog.Info ("BuildInfo", "BuildID : {0}", (this.Data.BuildID == null) ? "null" : this.Data.BuildID);
			UtilsLog.Info ("BuildInfo", "BuildVersion : {0}", (this.Data.BuildVersion == null) ? "null" : this.Data.BuildVersion);
			UtilsLog.Info ("BuildInfo", "BuildShortVersion : {0}", (this.Data.BuildShortVersion == null) ? "null" : this.Data.BuildShortVersion);
			UtilsLog.Info ("BuildInfo", "BuildVersionCode : {0}", this.Data.BuildVersionCode);
			UtilsLog.Info ("BuildInfo", "CenterVersion : {0}", (this.Data.CenterVersion == null) ? "null" : this.Data.CenterVersion);

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

			this.Data.BuildName = iData.BuildName;
			this.Data.BuildID = iData.BuildID;
			this.Data.BuildVersion = iData.BuildVersion;
			this.Data.BuildShortVersion = iData.BuildShortVersion;
			this.Data.BuildVersionCode = iData.BuildVersionCode;
			this.Data.CenterVersion = iData.CenterVersion;

			UtilsAsset.SetAssetDirty (this);
		}

		#endregion
	}
}
