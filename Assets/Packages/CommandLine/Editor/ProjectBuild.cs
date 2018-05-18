using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Collections;
using Common;
using AssetBundles;
using Download;
using AndroidSDK;
using AndroidSDK.Common;
using AndroidSDK.OneSDK;
using AndroidSDK.Platforms.Huawei;
#if UNITY_ANDROID
using AndroidSDK.Platforms.Tiange;
#endif
using BuildSystem;
using Common;
using NetWork.Servers;
using Upload;

namespace CommandLine {
	/// <summary>
	/// 工程打包类.
	/// </summary>
	class ProjectBuild : EditorBase {

		/// <summary>
		/// 输出用根目录(默认).
		/// </summary>
		private static string _defaultOutputRootDir = string.Format("{0}/../Output", Application.dataPath);

		/// <summary>
		/// 打包场景列表.
		/// </summary>
		/// <returns>打包场景列表.</returns>
		static string[] GetBuildScenes()
		{
			List<string> scenes = new List<string>();

			foreach(EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
			{
				if (scene == null) {
					continue;
				}
				if (scene.enabled) {
					scenes.Add (scene.path);
					BuildLogger.LogMessage ("[BuildScene]: -> {0}", scene.path);
				}
			}
			return scenes.ToArray();
		}

#region Build Android

		/// <summary>
		/// 安卓打包.
		/// </summary>
		[UnityEditor.MenuItem("Tools/PerformBuild/AndroidBuild")]
		static void BuildForAndroid()
		{
			const string funcBlock = "ProjectBuild.BuildForAndroid()";
			BuildLogger.OpenBlock(funcBlock);

			// 设定打包信息
			SetBuildInfoFromParameters();

#if UNITY_ANDROID

			TBuildMode buildMode = BuildInfo.GetInstance().BuildMode;
			BuildLogger.LogMessage ("BuildMode:{0}", buildMode.ToString());

			// 输出格式（{ProjectName}_v{ProjectVersion}_{buildNumber}_YYYYMMDDHHMMSS.apk）
			string buildTime = BuildParameters.BuildTime;

			// 输出路径
			// 游戏输出目录（Android）:{OutputDir}/Android/{ProjectName}
			string outputDir = GetOutputDir (BuildTarget.Android, BuildParameters.OutputDir);
			if (outputDir == null) {
				BuildLogger.LogException("Create Dir Failed.!!(Dir:{0})", 
					BuildParameters.OutputDir);
				throw new ApplicationException();
			}

			// 打包选项
			BuildOptions buildOptionTmp = BuildOptions.None;
			if (TBuildMode.Debug == buildMode) {
				buildOptionTmp |= BuildOptions.Development;
				buildOptionTmp |= BuildOptions.AllowDebugging;
				buildOptionTmp |= BuildOptions.ConnectWithProfiler;
			} else {
				bool isCheatMode = BuildParameters.IsCheatMode;
				if (true == isCheatMode) {
					buildOptionTmp |= BuildOptions.Development;
				}
			}
			BuildLogger.LogMessage ("BuildOption:{0}", ((int)buildOptionTmp).ToString());

			// 版本号
			string buildVersion = BuildInfo.GetInstance().BuildVersion;
			if (string.IsNullOrEmpty (buildVersion) == false) {
				PlayerSettings.bundleVersion = buildVersion;
			}
			BuildLogger.LogMessage ("BuildVersion:{0}", buildVersion);

			// buildVersionCode
			int buildVersionCode = BuildInfo.GetInstance().BuildVersionCode;
			PlayerSettings.Android.bundleVersionCode = buildVersionCode;
			BuildLogger.LogMessage ("BundleVersionCode:{0}", buildVersionCode.ToString());

			// 中心服务器版本号
			string centerVersion = BuildInfo.GetInstance ().CenterVersion;
			BuildLogger.LogMessage ("CenterVersion:{0}", centerVersion);

			// 工程名
			string projectName = BuildInfo.GetInstance().BuildName;
			BuildLogger.LogMessage ("ProjectName:{0}", projectName);

			// 游戏名字
			string gameName = BuildParameters.GameName;
			if (true == string.IsNullOrEmpty (gameName)) {
				gameName = projectName;
			}
			PlayerSettings.productName = gameName;
			BuildLogger.LogMessage ("GameName:{0}", gameName);

			// BuildID
			string buildID = BuildInfo.GetInstance().BuildID;
			if (false == string.IsNullOrEmpty (buildID)) {
#if UNITY_5_5_OR_NEWER
                PlayerSettings.applicationIdentifier = buildID;
#else
				PlayerSettings.bundleIdentifier = buildID;
#endif
			}
			BuildLogger.LogMessage ("BuildID:{0}", buildID);

			// 初始化
			InitForAndroidBuild();
			BuildLogger.LogMessage (" --> InitForAndroidBuild()");
		
			// Apk输出路径
			int buildNumber = BuildInfo.GetInstance().BuildNumber;
			BuildLogger.LogMessage ("BuildNumber:{0}", buildNumber.ToString());

			string apkPath = string.Format ("{0}/{1}_{2}_v{3}_-_{4}_{5}.apk", 
				outputDir, 
				projectName, 
				buildMode,
				buildVersion, 
				buildTime, buildID);
			if (0 < buildNumber) {
				apkPath = string.Format ("{0}/{1}_{2}_v{3}_{4}_{5}_{6}.apk", 
					outputDir, 
					projectName, 
					buildMode,
					buildVersion, 
					buildNumber, 
					buildTime, buildID);
			}
			BuildLogger.LogMessage("Apk File Path:{0}", apkPath);
				
			// 输出打包信息
			OutputBuildInfo(buildVersion, projectName);

			// 开发者模式
			if (BuildOptions.Development == buildOptionTmp) {
				// 打包之前，移除非资源对象
				AssetBundles.Common.MoveUnResources();
			}

			// Android下IL2CPP不支持，所以设置回Mono
			PlayerSettings.SetPropertyInt("ScriptingBackend", (int)ScriptingImplementation.Mono2x, BuildTarget.Android);

			string error = BuildPipeline.BuildPlayer(GetBuildScenes(), apkPath, BuildTarget.Android, buildOptionTmp);

			// 开发者模式
			if (BuildOptions.Development == buildOptionTmp) {
				// 打包之后，恢复非资源对象
				AssetBundles.Common.MoveBackUnResources();
			}

			if (error != null && !error.Equals ("") && !(error.Length == 0)) {
				BuildLogger.LogException("Android Build Failed!!!(error:{0})", error);
				BuildLogger.CloseBlock();
				throw new ApplicationException ();
			} else {
				BuildLogger.LogMessage("Android Build Successed.");
			}

#else

			BuildLogger.LogError("The platform for build is not android.");

#endif

			BuildLogger.CloseBlock();
		}
			

		#if UNITY_ANDROID

		static void InitForAndroidBuild() {

			// 清空下载目录
			DownloadList _instance = DownloadList.GetInstance();
			if (_instance != null) {
				_instance.Clear (true);
			}
			BuildSettings.GetInstance (BuildSettings.AssetFileDir);

			// 清空Plugins/Android目录
			ClearPluginsAndroid();

			// 重置Plugins/Android
			ResetPluginsAndroid();

			// 设置相关AndroidSDK相关设定
			IAndroidSDKSettings androidSDK = GetCurAndroidSDKSetting ();
			if (androidSDK != null) {
				string gameName = BuildInfo.GetInstance ().BuildName;
				string packageName = BuildInfo.GetInstance ().BuildID;
				// 打包Android（apk文件）之前，提前应用设定
				androidSDK.PreApplyAndroidBuild (gameName, packageName);
			} else {
				BuildLogger.LogWarning("Android SDK invalid!!");
			}

			// 刷新
			UtilsAsset.AssetsRefresh ();
		}

		/// <summary>
		/// 清空Plugins/Android目录.
		/// </summary>
		static void ClearPluginsAndroid() {
		
			string _dir = string.Format ("{0}/Plugins/Android", 
				Application.dataPath);
			if (false == UtilsAsset.CheckAndCreateDir (_dir)) {
				return;
			}

			// 清空目录
			UtilsAsset.ClearDirectory(_dir);
		}

		/// <summary>
		/// 重置Plugins/Android.
		/// </summary>
		static void ResetPluginsAndroid() {
			string _fromDir = string.Format ("{0}/../AndroidPlatform/Default", 
				Application.dataPath);
			if (false == UtilsAsset.CheckAndCreateDir (_fromDir)) {
				return;
			}

			string _toDir = string.Format ("{0}/Plugins/Android", 
				Application.dataPath);
			if (false == UtilsAsset.CheckAndCreateDir (_toDir)) {
				return;
			}
				
			// 拷贝文件
			DirectoryInfo dirInfo = new DirectoryInfo(_fromDir);
			FileInfo[] allFiles = dirInfo.GetFiles();
			if ((null != allFiles) && (1 <= allFiles.Length)) {
				foreach (FileInfo file in allFiles) {
					if (true == file.Name.EndsWith (".meta")) {
						continue;
					}

					// 拷贝文件
					string copyToFile = string.Format ("{0}/{1}", _toDir, file.Name);
					BuildLogger.LogMessage ("Copy File : {0} -> {1}",
						file.FullName, copyToFile);

					File.Copy (file.FullName, copyToFile, true);
				}
			}

			// 检索子文件夹
			DirectoryInfo[] subDirs = dirInfo.GetDirectories();
			if ((null != subDirs) && (1 <= subDirs.Length)) {
				foreach (DirectoryInfo subDir in subDirs) {
					string subfromDir = string.Format ("{0}/{1}", _fromDir, subDir.Name);

					// 拷贝
					UtilsAsset.CopyDirectory (subfromDir, _toDir);
				}
			}
		}

		/// <summary>
		/// 取得当前AndroidSDK设定信息.
		/// </summary>
		/// <returns>当前AndroidSDK设定信息.</returns>
		static IAndroidSDKSettings GetCurAndroidSDKSetting() {
			IAndroidSDKSettings settings = null;

			// 平台类型
			TPlatformType platformType = BuildInfo.GetInstance ().PlatformType;
			BuildLogger.LogMessage ("PlatformType:{0}.", platformType.ToString ());

			switch (platformType) {
				// 华为
			case TPlatformType.Huawei:
				{
					settings = HuaweiSDKSettings.GetInstance ();
				}
				break;

				// 天鸽
			case TPlatformType.Tiange:
				{
					settings = TiangeSDKSettings.GetInstance ();
				}
				break;
			case TPlatformType.None:
			case TPlatformType.iOS:
			default:
				{
					// 清空/Plugins/Android下的文件
				}
				break;
			}

			return settings;
		}

#endif

		#endregion

		#region Export Xcode Project

		/// <summary>
		/// 导出XCodeProject工程.
		/// </summary>
		[UnityEditor.MenuItem("Tools/PerformBuild/ExportXcodeProject")]
		static void ExportXcodeProject()
		{ 
			const string funcBlock = "ProjectBuild.ExportXcodeProject()";
			BuildLogger.OpenBlock(funcBlock);

			// 设定打包信息
			SetBuildInfoFromParameters();
			// 平台类型
			BuildInfo.GetInstance().PlatformType = TPlatformType.iOS;

			TBuildMode buildMode = BuildInfo.GetInstance ().BuildMode;
			BuildLogger.LogMessage ("BuildMode:{0}", buildMode.ToString());

			// 初始化
			InitForExportXcodeProject();
			BuildLogger.LogMessage (" --> InitForExportXcodeProject()");

			// 预定义宏
			//PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, "GAMELINK");

			// 游戏输出目录（iOS）:{OutputDir}/iOS/{ProjectName}
			string outputDir = GetOutputDir (BuildTarget.iOS, null);
			if (outputDir == null) {
				BuildLogger.LogException("Create Dir Failed.!!(Dir:{0})", 
					BuildParameters.OutputDir);
				throw new ApplicationException();
			}

			// 打包选项
			BuildOptions buildOptionTmp = BuildOptions.None;
			if (TBuildMode.Debug == buildMode) {
				buildOptionTmp |= BuildOptions.Development;
				buildOptionTmp |= BuildOptions.AllowDebugging;
				buildOptionTmp |= BuildOptions.ConnectWithProfiler;
			} else {
				bool isCheatMode = BuildParameters.IsCheatMode;
				if (true == isCheatMode) {
					buildOptionTmp |= BuildOptions.Development;
				}
			}
			BuildLogger.LogMessage ("BuildOption:{0}", ((int)buildOptionTmp).ToString());

			// 工程名
			string projectName = BuildInfo.GetInstance().BuildName;
			BuildLogger.LogMessage ("ProjectName:{0}", projectName);
				
			// 游戏名字
			string gameName = BuildParameters.GameName;
			if (true == string.IsNullOrEmpty (gameName)) {
				gameName = projectName;
			}
			PlayerSettings.iOS.applicationDisplayName = gameName;
			BuildLogger.LogMessage ("GameName:{0}", gameName);

			// BuildID
			string buildID = BuildInfo.GetInstance().BuildID;
			if (false == string.IsNullOrEmpty (buildID)) {
#if UNITY_5_5_OR_NEWER
                PlayerSettings.applicationIdentifier = buildID;
#else
				PlayerSettings.bundleIdentifier = buildID;
#endif
			}
			BuildLogger.LogMessage ("BuildID:{0}", buildID);

			// 版本号
			string buildVersion = BuildInfo.GetInstance ().BuildVersion;
			PlayerSettings.bundleVersion = buildVersion;
			BuildLogger.LogMessage ("BuildVersion:{0}", buildVersion);

			// 中心服务器版本号
			string centerVersion = BuildInfo.GetInstance ().CenterVersion;
			BuildLogger.LogMessage ("CenterVersion:{0}", centerVersion);

			// XCode工程目录
			string XcodeProject = string.Format("{0}/XcodeProject", outputDir);

			// 输出打包信息
			OutputBuildInfo(buildVersion, projectName);

			// 开发者模式
			if (BuildOptions.Development == buildOptionTmp) {
				// 打包之前，将非资源对象，临时移动到临时文件夹
				AssetBundles.Common.MoveUnResources();
			}

			// 打包成XCode工程目录
			#if UNITY_5_OR_NEWER
			string error = BuildPipeline.BuildPlayer(
				GetBuildScenes(), 
				XcodeProject, 
				BuildTarget.iOS, buildOptionTmp);
			#else
			string error = BuildPipeline.BuildPlayer(
				GetBuildScenes(), 
				XcodeProject,  
				BuildTarget.iOS, buildOptionTmp);
			#endif

			// 开发者模式
			if (BuildOptions.Development == buildOptionTmp) {
				// 恢复非资源性文件
				AssetBundles.Common.MoveBackUnResources();
			}

			// 存在错误则，打包编译失败
			if (error != null && !error.Equals ("") && !(error.Length == 0)) {
				BuildLogger.LogException("iOS Build Failed!!!(error:{0})", error);
				BuildLogger.CloseBlock();
				throw new ApplicationException ();
			} else {
				BuildLogger.LogMessage("iOS Build Successed.");
			}
			BuildLogger.CloseBlock();
		}

		#endregion

		#region 其他处理

		/// <summary>
		/// 取得输出目录.
		/// </summary>
		/// <returns>The output dir.</returns>
		/// <param name="iTarget">打包目标类型.</param>
		/// <param name="iOutputDir">输出目录（未指定：默认输出根目录）.</param>
		private static string GetOutputDir(BuildTarget iTarget, string iOutputDir = null) {
			string outputRootDir = iOutputDir;
			if (string.IsNullOrEmpty (outputRootDir) == true) {
				outputRootDir = _defaultOutputRootDir;
			}

			if (Directory.Exists (outputRootDir) == false) {
				BuildLogger.LogWarning ("The directory is not exist, so to create.(dir:{0})",
					outputRootDir);
				Directory.CreateDirectory (outputRootDir);
			}
			if (Directory.Exists (outputRootDir) == false) {
				BuildLogger.LogError ("[Directory Create Failed] -> Dir:{0}", outputRootDir);
				return null;
			} else {
				BuildLogger.LogMessage ("[Directory Create Successed] -> Dir:{0}", outputRootDir);
			}
			string outputDir = string.Format ("{0}/{1}", outputRootDir, iTarget.ToString());
			if (Directory.Exists (outputDir) == false) {
				BuildLogger.LogWarning ("The directory is not exist, so to create.(dir:{0})",
					outputDir);
				Directory.CreateDirectory (outputDir);
			}
			if (Directory.Exists (outputDir) == false) {
				BuildLogger.LogError ("[Directory Create Failed] -> Dir:{0}", outputDir);
				return null;
			} else {
				BuildLogger.LogMessage ("[Directory Create Successed] -> Dir:{0}", outputDir);
			}
			return outputDir;
		}

		/// <summary>
		/// 输出打包信息(导出的XCode工程 打包ipa文件时使用).
		/// </summary>
		/// <param name="iProjectName">工程名.</param>
		/// <param name="iProjectVersion">工程版本.</param>
		private static void OutputBuildInfo(
			string iProjectName, string iProjectVersion) {
			
			const string funcBlock = "ProjectBuild.OutputBuildInfo()";
			BuildLogger.OpenBlock(funcBlock);

			string filePath = string.Format("{0}/../Shell/BuildInfo", Application.dataPath);
			if (File.Exists (filePath) == true) {
				File.Delete (filePath);
			}
			FileStream fStrm = new FileStream (filePath, FileMode.OpenOrCreate, FileAccess.Write);


			string buildInfo = string.Format("{0}:{1}:{2}", 
				BuildParameters.BuildTime, iProjectName, iProjectVersion);

			BuildLogger.LogMessage ("BuildInfo:{0}", buildInfo);

			// 获得字节数组
			byte[] data = System.Text.Encoding.Default.GetBytes(buildInfo); 
			// 写入
			fStrm.Write (data, 0, data.Length);
			// 清空缓冲区、关闭流
			if (null != fStrm) {
				fStrm.Flush ();
				fStrm.Close ();
				fStrm.Dispose ();
			}

			BuildLogger.CloseBlock();
		}

		#endregion

		private static void InitForExportXcodeProject() {
			// 清空下载目录
			DownloadList _instance = DownloadList.GetInstance();
			if (_instance != null) {
				_instance.Clear (true);
			}

			BuildSettings.GetInstance (BuildSettings.AssetFileDir); 
		}

		/// <summary>
		/// 设定打包信息
		/// </summary>
		private static void SetBuildInfoFromParameters() {

			// 平台类型
			TPlatformType platformType = BuildParameters.PlatformType;
			if (TPlatformType.None != platformType) {
				BuildInfo.GetInstance ().PlatformType = platformType;
			} 

			// 工程名
			string projectName = BuildParameters.ProjectName;
			if (false == string.IsNullOrEmpty (projectName)) {
				BuildInfo.GetInstance().BuildName = projectName;
			}

			// 打包ID
			string buildID = BuildParameters.BuildId;
			if (false == string.IsNullOrEmpty (buildID)) {
				BuildInfo.GetInstance().BuildID = buildID;
			}

			// 打包模式
			TBuildMode buildMode = BuildParameters.BuildMode;
			if (TBuildMode.None != buildMode) {
				BuildInfo.GetInstance ().BuildMode = buildMode;
			} 

			// 版本号
			string buildVersion = BuildParameters.BuildVersion;
			if(false == string.IsNullOrEmpty(buildVersion)) {
				BuildInfo.GetInstance().BuildVersion = buildVersion;
			}

			// VersionCode
			int buildVersionCode = BuildParameters.BuildVersionCode;
			if (-1 != buildVersionCode) {
				BuildInfo.GetInstance ().BuildVersionCode = buildVersionCode;
			}

			// 中心服务器版本号
			string centerVersion = BuildParameters.CenterVersion;
			if (false == string.IsNullOrEmpty (centerVersion)) {
				BuildInfo.GetInstance ().CenterVersion = centerVersion;
			}

			// 打包号
			int buildNumber = BuildParameters.BuildNumber;
			if (-1 < buildNumber) {
				BuildInfo.GetInstance ().BuildNumber = buildNumber;
			}

			// 是否跳过下载
			bool isSkipDownload = BuildParameters.IsSkipDownload;
			ServersConf.GetInstance ().SkipDownload = isSkipDownload;
		}
	}
}
