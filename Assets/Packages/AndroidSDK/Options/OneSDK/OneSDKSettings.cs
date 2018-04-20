#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using Common;
using AndroidSDK.Common;
using AndroidSDK.Common.Manifest;

namespace AndroidSDK.OneSDK {

	/// <summary>
	/// 安卓SDK设定信息数据.
	/// </summary>
	[System.Serializable]
	public class OneSDKSettingsData : AndroidSDKSettingsData {

		/// <summary>
		/// 初始化.
		/// </summary>
		public override void Init() {
			base.Init ();

			this.PlatformType = TPlatformType.Huawei;
			this.MinSdkVersion = 19;
			this.MaxSdkVersion = 26;
			this.TargetSdkVersion = 25;
			this.Local = true;
			this.AutoSDKInit = false;
			this.AutoLogin = false;
		}

		/// <summary>
		/// 清空.
		/// </summary>
		public override void Clear() {
			base.Clear ();

			this.PlatformType = TPlatformType.None;
			this.MinSdkVersion = 19;
			this.MaxSdkVersion = 26;
			this.TargetSdkVersion = 25;
			this.Local = true;
			this.AutoSDKInit = false;
			this.AutoLogin = false;
		}
	}

	/// <summary>
	/// 易接SDK设定.
	/// </summary>
	public class OneSDKSettings : AssetBase<OneSDKSettings, OneSDKSettingsData>, IAndroidSDKSettings {

		private static readonly string _assetFileDir = "Assets/Packages/AndroidSDK/OneSDK/Conf";

		/// <summary>
		/// 安卓SDK最小版本.
		/// </summary>
		public TPlatformType PlatformType {
			get { 
				if (null != this.Data) {
					if ((TPlatformType.Huawei != this.Data.PlatformType)) {
						return this.Data.PlatformType = TPlatformType.Huawei;
					}
				}
				return TPlatformType.Huawei;
			}	
		}

		/// <summary>
		/// 安卓SDK最小版本.
		/// </summary>
		public int MinSdkVersion {
			get { 
				if (null != this.Data) {
					return this.Data.MinSdkVersion;
				}
				return -1;
			}	
			set { 
				if (null != this.Data) {
					this.Data.MinSdkVersion = value;
				}
			}
		}

		/// <summary>
		/// 安卓SDK最大版本.
		/// </summary>
		public int MaxSdkVersion {
			get { 
				if (null != this.Data) {
					return this.Data.MaxSdkVersion;
				}
				return -1;
			}	
			set { 
				if (null != this.Data) {
					this.Data.MaxSdkVersion = value;
				}
			}	
		}

		/// <summary>
		/// 安卓SDK目标版本.
		/// </summary>
		public int TargetSdkVersion {
			get { 
				if (null != this.Data) {
					return this.Data.TargetSdkVersion;
				}
				return -1;
			}
			set { 
				if (null != this.Data) {
					this.Data.TargetSdkVersion = value;
				}
			}	
		}

		/// <summary>
		/// App ID.
		/// </summary>
		public string AppID {
			get { 
				if (null != this.Data) {
					return this.Data.AppID;
				}
				return null;
			}
			set { 
				if (null != this.Data) {
					this.Data.AppID = value;
				}
			}	
		}

		/// <summary>
		/// 支付ID.
		/// </summary>
		public string PayID {
			get { 
				if (null != this.Data) {
					return this.Data.PayID;
				}
				return null;
			}		
			set { 
				if (null != this.Data) {
					this.Data.PayID = value;
				}
			}
		}

		/// <summary>
		/// 屏幕方向.
		/// </summary>
		public UIOrientation Orientation {
			get { 
				if (null != this.Data) {
					return this.Data.Orientation;
				}
				return PlayerSettings.defaultInterfaceOrientation;
			}	
			set { 
				if (null != this.Data) {
					this.Data.Orientation = value;
				}
			}
		}

		/// <summary>
		/// 本地存储标志位（false:向服务器拉取相关信息）.
		/// </summary>
		public bool Local {
			get { 
				if (null != this.Data) {
					return this.Data.Local;
				}
				return true;
			}	
			set { 
				if (null != this.Data) {
					this.Data.Local = value;
				}
			}
		}

		/// <summary>
		/// SDK自动初始化.
		/// </summary>
		public bool AutoSDKInit {
			get { 
				if (null != this.Data) {
					return this.Data.AutoSDKInit;
				}
				return true;
			}	
			set { 
				if (null != this.Data) {
					this.Data.AutoSDKInit = value;
				}
			}
		}

		/// <summary>
		/// 自动登录标志位.
		/// </summary>
		public bool AutoLogin {
			get { 
				if (null != this.Data) {
					return this.Data.AutoLogin;
				}
				return true;
			}	
			set { 
				if (null != this.Data) {
					this.Data.AutoLogin = value;
				}
			}
		}

#region abstract - AssetBase

		/// <summary>
		/// 取得导入路径.
		/// </summary>
		/// <returns>导入路径.</returns>
		public override string GetImportPath () {
			return string.Format("{0}/Json", _assetFileDir);
		}

		/// <summary>
		/// 取得导出路径.
		/// </summary>
		/// <returns>导出路径.</returns>
		public override string GetExportPath () {
			return string.Format("{0}/Json", _assetFileDir);
		}

		/// <summary>
		/// 初始化数据.
		/// </summary>
		/// <returns><c>true</c>, OK, <c>false</c> NG.</returns>
		public override bool InitAsset () { 
			return this.InitSettings(); 
		}

		/// <summary>
		/// 应用数据.
		/// </summary>
		/// <param name="iData">数据.</param>
		/// <param name="iForceClear">强制清空.</param>
		protected override void ApplyData(OneSDKSettingsData iData, bool iForceClear) {

			if (null == iData) {
				return;
			}

			// 清空
			if (true == iForceClear) {
				this.Clear ();
			}

			this.Data.MinSdkVersion = iData.MinSdkVersion;
			this.Data.MaxSdkVersion = iData.MaxSdkVersion;
			this.Data.TargetSdkVersion = iData.TargetSdkVersion;
			this.Data.AppID = iData.AppID;
			this.Data.PayID = iData.PayID;
			this.Data.Orientation = iData.Orientation;
			this.Data.Local = iData.Local;
			this.Data.AutoSDKInit = iData.AutoSDKInit;
			this.Data.AutoLogin = iData.AutoLogin;

			UtilsAsset.SetAssetDirty (this);

		}

#endregion


#region Interface - IAndroidSDKSettings

		/// <summary>
		/// 取得导出华为用的AndroidManifest.xml文件路径.
		/// </summary>
		/// <returns>导出华为用的AndroidManifest.xml文件路径.</returns>
		public string GetAndroidManifestXmlPath() {
			string manifestXmlPath = string.Format ("{0}/AndroidManifest.xml");
			if (false == File.Exists (manifestXmlPath)) {
				return null;
			}
			return manifestXmlPath;
		}

		/// <summary>
		/// 初始化设定信息.
		/// </summary>
		public bool InitSettings() {

			// 路径
			this._path = _assetFileDir;

			return true; 
		}

		/// <summary>
		/// 取得拷贝源文件目录.
		/// </summary>
		/// <returns>取得拷贝源文件目录.</returns>
		public string GetAndroidCopyFromDir() {

			string dir = string.Format("{0}/../AndroidPlatform", Application.dataPath);
			if (false == Directory.Exists (dir)) {
				this.Warning ("GetAndroidCopyFromDir():The directory is not exist!!(Dir:{0})", dir);
				Directory.CreateDirectory (dir);
			}

			dir = string.Format("{0}/{1}", dir, this.PlatformType.ToString());
			if (false == Directory.Exists (dir)) {
				this.Warning ("GetAndroidCopyFromDir():The directory is not exist!!(Dir:{0})", dir);
				Directory.CreateDirectory (dir);
			}

			return dir;
		}

		/// <summary>
		/// 取得拷贝目的文件目录.
		/// </summary>
		/// <returns>取得拷贝目的文件目录.</returns>
		public string GetAndroidCopyToDir() {

			string dir = string.Format("{0}/Plugins/Android", Application.dataPath);
			if (false == Directory.Exists (dir)) {
				this.Warning ("GetAndroidCopyToDir():The directory is not exist!!(Dir:{0})", dir);
				Directory.CreateDirectory (dir);
			}

			return dir;
		}

		/// <summary>
		/// 取得AndroidManifest对象.
		/// </summary>
		/// <returns>AndroidManifest对象.</returns>
		/// <param name="iGameName">游戏名.</param>
		public ManifestBase GetAndroidManifest (string iGameName) {
			ManifestBase manifest = null;
			string _dir = GetAndroidCopyFromDir ();
			if (true == Directory.Exists (_dir)) {
//				manifest = OneSDKSettings.GetInstance (_dir, iGameName);
			}
			return manifest;
		}


		/// <summary>
		/// 应用设定信息到AndroidManifest.xml.
		/// </summary>
		/// <param name="iManifest">AndroidManifest对象.</param>
		/// <param name="iPackageName">游戏包名.</param>
		public bool AppSettingsToAndroidManifestFile (
			ManifestBase iManifest, string iPackageName) {

			if (null == iManifest) {
				return false;
			}

			if (false == string.IsNullOrEmpty (iPackageName)) {
				iManifest.ApplyPackageName (iPackageName);  
			}

			return true;
		}

		/// <summary>
		/// 打包Android（apk文件）之前，提前应用设定.
		/// </summary>
		/// <param name="iGameName">游戏名.</param>
		/// <param name="iPackageName">游戏包名.</param>
		public void PreApplyAndroidBuild (string iGameName, string iPackageName) {

			// 合并AndroidManifest.xml文件
			if(true == this.MergeManifestFile(iGameName, iPackageName)) {
				// 拷贝库资源文件
				this.CopyResources();
			}
		}

		/// <summary>
		/// 合并AndroidManifest.xml文件.
		/// </summary>
		/// <returns><c>true</c>, OK, <c>false</c> NG.</returns>
		/// <param name="iGameName">游戏名.</param>
		/// <param name="iPackageName">游戏包名.</param>
		public bool MergeManifestFile(string iGameName, string iPackageName) {

			ManifestBase manifest = this.GetAndroidManifest(iGameName);
			if (null == manifest) {
				return false;
			}

			// 保存路径
			string copyToManifestFile = GetAndroidCopyToDir();
			string savePath = string.Format ("{0}/AndroidManifest.xml", copyToManifestFile);

			// 保存AndroidManifest.xml文件
			return this.SaveAndroidManifestFile (manifest, savePath, iPackageName);

		}

		/// <summary>
		/// 保存AndroidManifest.xml文件.
		/// </summary>
		/// <param name="iManifest">AndroidManifest对象.</param>
		/// <param name="iSavePath">保存路径.</param>
		/// <param name="iPackageName">游戏包名.</param>
		public bool SaveAndroidManifestFile (ManifestBase iManifest, string iSavePath, string iPackageName) {
			if (null == iManifest) {
				return false;
			}

			// 应用设定信息到AndroidManifest.xml
			bool bolRet = this.AppSettingsToAndroidManifestFile (iManifest, iPackageName);
			if (true == bolRet) {
				// 保存
				iManifest.Save (iSavePath);
			}
			return bolRet;
		}

		/// <summary>
		/// 拷贝库资源文件.
		/// </summary>
		public void CopyResources () {
			string CopyFromDir = GetAndroidCopyFromDir ();
			string CopyToDir = GetAndroidCopyToDir ();

			// Libs
			string[] files = Directory.GetFiles (CopyFromDir);
			foreach (string file in files) {
				if (true == file.EndsWith ("AndroidManifest.xml")) {
					continue;
				}
				if (true == file.EndsWith (".meta")) {
					continue;
				}

				int lastIndex = file.LastIndexOf ("/");
				string fileName = file.Substring (lastIndex + 1);
				if (true == string.IsNullOrEmpty (fileName)) {
					continue;
				}

				string copyToFile = string.Format ("{0}/{1}", CopyToDir, fileName);
				if (true == File.Exists (copyToFile)) {
					File.Delete (copyToFile);
				}
				UtilsLog.Info ("CopyResources", "Copy Libs : {0} -> {1}",
					file, copyToFile);

				File.Copy (file, copyToFile);
			}

			// res
			string CopyRes = string.Format ("{0}/res", CopyFromDir);
			if (true == Directory.Exists (CopyRes)) {
				UtilsAsset.CopyDirectory (CopyRes, CopyToDir);
			}
		}

#endregion
	}
}

#endif
