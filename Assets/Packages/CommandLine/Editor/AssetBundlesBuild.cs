//#define BUNDLE_UPLOAD
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using AssetBundles;
using Upload;
using NetWork.Servers;
using BuildSystem;
using Common;

namespace CommandLine {

	/// <summary>
	/// 资源打包.
	/// </summary>
	public class AssetBundlesBuild : Editor {

		#region Build Bundles

		/// <summary>
		/// IOS资源打包.
		/// </summary>
		[UnityEditor.MenuItem("Tools/AssetBundles/Build/IOS")]
		static void BuildForIOS() 
		{
			const string funcBlock = "AssetBundlesBuild.BuildForIOS()";
			BuildLogger.OpenBlock(funcBlock);

			// 开始打包Bundles
			UtilsAsset.StartBuildBundles ();

			BuildAssetBundle(BuildTarget.iOS, true);

			// 生成上传用的Shell
			CreateUploadShell();

			// 开始打包Bundles
			UtilsAsset.EndBuildBundles ();

			BuildLogger.CloseBlock();
		}

		/// <summary>
		/// 安卓资源打包.
		/// </summary>
		[@MenuItem("Tools/AssetBundles/Build/Android")]
		static void BuildForAndroid()
		{
			const string funcBlock = "AssetBundlesBuild.BuildForAndroid()";
			BuildLogger.OpenBlock(funcBlock);

			// 开始打包Bundles
			UtilsAsset.StartBuildBundles ();

			BuildAssetBundle(BuildTarget.Android, true);

			// 生成上传用的Shell
			CreateUploadShell();

			// 开始打包Bundles
			UtilsAsset.EndBuildBundles ();

			BuildLogger.CloseBlock();
		}

		/// <summary>
		/// 打包资源文件
		/// </summary>
		/// <param name="buildTarget">Build target.</param>
		/// <param name="needCompress">If set to <c>true</c> need compress.</param>
		private static void BuildAssetBundle(BuildTarget buildTarget, bool needCompress = false)
		{
			const string funcBlock = "AssetBundlesBuild.BuildAssetBundle()";
			BuildLogger.OpenBlock(funcBlock);

			// 设置上传的打包类型
			UploadList.GetInstance().BuildTarget = buildTarget.ToString();

			BundlesConfig bcConfig = BundlesConfig.GetInstance();
			if((null == bcConfig) || (0 >= bcConfig.Resources.Count)) {
				BuildLogger.LogError ("BuildAssetBundle::BundlesConfig is invalid!!!");
				return;
			}

			// 清空依赖关系列表
			BundlesMap bundlesMap = BundlesMap.GetInstance ();
			if(null == bundlesMap) {
				BuildLogger.LogError ("BuildAssetBundle::bundlesMap is invalid!!!");
				return;
			}
			bundlesMap.Clear ();

			List<BundleResource> allConfig = bcConfig.Resources;

			// make bundle config
			foreach(BundleResource bc in allConfig)
			{
				// filter file
				if(bc.Mode == BundleMode.OneDir)
				{
					string bundleId = BundlesMap.GetBundleID (bc.Path);
					BundleMap bm = bundlesMap.GetOrCreateBundlesMap(bundleId);

					bm.ID = bundleId;
					bm.Path = bc.Path;

					// 取得当前目录的文件列表
					List<string> files = GetAllFiles(bc.Path);

					// 遍历文件列表
					foreach(string file in files)
					{
						// .DS_Store文件
						if(file.EndsWith(".DS_Store") == true) {
							continue;
						}
						// *.meta文件
						if(file.EndsWith(".meta") == true) {
							continue;
						}

						// 若为忽略文件，则跳过
						if (bcConfig.isIgnoreFile (bc, file) == true) {
							bm.RemoveIgnorFile (file);
							continue;
						}
						bm.AddFile(file);
					}

					bundlesMap.Maps.Add(bm);
				}
				else if(bc.Mode == BundleMode.SceneOneToOne)
				{
					// 取得当前目录的文件列表
					List<string> files = GetAllFiles(bc.Path);

					foreach(string file in files)
					{
						// .DS_Store文件
						if(file.EndsWith(".DS_Store") == true) {
							continue;
						}
						// *.meta文件
						if(file.EndsWith(".meta") == true) {
							continue;
						}
						// 若非场景文件，则跳过
						if (file.EndsWith (".unity") == false) {
							continue;
						}

						// 若为忽略文件，则跳过
						string bundleId = BundlesMap.GetBundleID(file);
						BundleMap bm = bundlesMap.GetOrCreateBundlesMap(bundleId);
						if (bcConfig.isIgnoreFile (bc, file) == true) {
							bm.RemoveIgnorFile (file);
							continue;
						}

						bm.ID = bundleId;
						bm.Path = bc.Path;
						bm.Type = TBundleType.Scene;
						bm.AddFile(file);

						bundlesMap.Maps.Add(bm);
					}
				}
				else if(bc.Mode == BundleMode.FileOneToOne)
				{
					// 取得当前目录的文件列表
					List<string> files = GetAllFiles(bc.Path);

					foreach(string file in files)
					{
						// .DS_Store文件
						if(file.EndsWith(".DS_Store") == true) {
							continue;
						}
						// *.meta文件
						if(file.EndsWith(".meta") == true) {
							continue;
						}

						// 若为忽略文件，则跳过
						string bundleId = BundlesMap.GetBundleID(file);
						BundleMap bm = bundlesMap.GetOrCreateBundlesMap(bundleId);
						if (bcConfig.isIgnoreFile (bc, file) == true) {
							bm.RemoveIgnorFile (file);
							continue;
						}

						bm.ID = bundleId;
						bm.Path = bc.Path;
						bm.AddFile(file);

						bundlesMap.Maps.Add(bm);

					}
				}
				else if(bc.Mode == BundleMode.TopDirOneToOne)
				{
					// 取得目录列表
					string[] directories = Directory.GetDirectories (bc.Path);
					if ((directories == null) || (directories.Length <= 0)) {
						BuildLogger.LogWarning ("The no subfolder in this path!!!(dir:{0})", 
							bc.Path);
						continue;
					}

					foreach(string dir in directories)
					{
						// 取得当前目录的文件列表
						List<string> files = GetAllFiles(dir);

						string bundleId = BundlesMap.GetBundleID(dir);
						bundleId = BundlesMap.GetBundleID(dir);
						if (string.IsNullOrEmpty (bundleId) == true) {
							continue;
						}
						BundleMap bm = bundlesMap.GetOrCreateBundlesMap(bundleId);
						bm.ID = bundleId;
						bm.Path = bc.Path;

						foreach(string file in files)
						{
							// .DS_Store文件
							if(file.EndsWith(".DS_Store") == true) {
								continue;
							}
							// *.meta文件
							if(file.EndsWith(".meta") == true) {
								continue;
							}

							// 若为忽略文件，则跳过
							if (bcConfig.isIgnoreFile (bc, file) == true) {
								bm.RemoveIgnorFile (file);
								continue;
							}

							bm.AddFile(file);
						}

						bundlesMap.Maps.Add(bm);
					}
				}
			}

			// 目录检测
			string checkDir = UploadList.GetInstance().BundlesOutputDir;
			if(Directory.Exists(checkDir) == false) {
				Directory.CreateDirectory (checkDir);
			}
			checkDir = UploadList.GetInstance().BundlesOutputDirOfNormal;
			if(Directory.Exists(checkDir) == false) {
				Directory.CreateDirectory (checkDir);
			}
			checkDir = UploadList.GetInstance().BundlesOutputDirOfScene;
			if(Directory.Exists(checkDir) == false) {
				Directory.CreateDirectory (checkDir);
			}

			bool successed = false;
			AssetBundleManifest result = null;
			string[] allAssets = null;
			AssetBundleBuild[] targets = null;

			// 一般Bundles
			try {

				targets = bundlesMap.GetAllNormalBundleTargets();
				BuildAssetBundleOptions options = BuildAssetBundleOptions.UncompressedAssetBundle;
				result = BuildPipeline.BuildAssetBundles(
					UploadList.GetInstance().BundlesOutputDirOfNormal, 
					targets, 
					options, 
					buildTarget);
				BuildLogger.LogMessage(" -> BuildPipeline.BuildAssetBundles");
				if(result != null) {
					allAssets = result.GetAllAssetBundles();
					if((allAssets != null) && (targets.Length == allAssets.Length)) {
						successed =  true;
					}
				}
			} catch (Exception exp) {
				BuildLogger.LogException("BuildAssetBundles Detail : {0}", exp.Message);
				successed = false;
			}

			// 更新导出标志位
			if (successed == true) {
				BuildLogger.LogMessage (" -> BundlesConfig.UpdateBundleStateWhenCompleted");

				Dictionary<string, string> hashCodes = new Dictionary<string, string>();
				foreach(string asset in allAssets) {
					Hash128 hashCode = result.GetAssetBundleHash(asset);
					if(string.IsNullOrEmpty(hashCode.ToString()) == true) {
						continue;
					}
					string fileSuffix = UploadList.GetInstance ().FileSuffix;
					string key = asset;
					if(string.IsNullOrEmpty(fileSuffix) == false) {
						fileSuffix = fileSuffix.ToLower();
						fileSuffix = string.Format(".{0}", fileSuffix);
						key = key.Replace(fileSuffix, "");
					}
					hashCodes[key] = hashCode.ToString();
				}
				// 初始化检测信息（Hash Code）
				bundlesMap.UpdateUploadList (TBundleType.Normal, hashCodes);
				BuildLogger.LogMessage(" -> BundlesMap.UpdateUploadList Normal");
			}

			// Scene Bundles
			List<SceneBundleInfo> targetScenes = bundlesMap.GetAllSceneBundleTargets();
			if ((targetScenes != null) && (targetScenes.Count > 0)) {
				foreach (SceneBundleInfo scene in targetScenes) {
					if ((scene == null) || 
						(scene.GetAllTargets () == null) || 
						(scene.GetAllTargets ().Length <= 0)) {
						continue;
					}
					try {

						BuildOptions options = BuildOptions.BuildAdditionalStreamedScenes;
						if (TBuildMode.Debug == BuildInfo.GetInstance ().BuildMode) {
							options |= BuildOptions.Development;
						}
						string sceneState = BuildPipeline.BuildPlayer (
							scene.GetAllTargets (),
							UploadList.GetLocalSceneBundleFilePath (scene.BundleId),
							buildTarget,
							options);
						BuildLogger.LogMessage (" -> BuildPipeline.BuildStreamedSceneAssetBundle(State:{0})", sceneState);
					} catch (Exception exp) {
						BuildLogger.LogException ("BuildStreamedSceneAssetBundle Detail:{0}", exp.Message);
						successed = false;
					}
				}
			}

			// 更新导出标志位
			if (successed == true) {
				BuildLogger.LogMessage (" -> BundlesConfig.UpdateBundleStateWhenCompleted");

				// 初始化检测信息（Hash Code）
				bundlesMap.UpdateUploadList (TBundleType.Scene);
				BuildLogger.LogMessage(" -> BundlesMap.UpdateUploadList Scene");
			}

			BuildInfo.GetInstance().ExportToJsonFile();
			BuildLogger.LogMessage(" -> BuildInfo.ExportToJsonFile");

			BuildLogger.CloseBlock();
		}


		/// <summary>
		/// 取得指定目录文件列表（包含子目录）.
		/// </summary>
		/// <returns>文件列表.</returns>
		/// <param name="iDirection">文件目录.</param>
		static List<string> GetAllFiles(string iDirection)
		{   
			List<string> filesList = new List<string>();

			try   
			{  
				bool isDir = false;
				if((false == string.IsNullOrEmpty(iDirection)) && 
					(true == iDirection.EndsWith("/"))){
					isDir = true;
				}
				if(true == isDir) {
					string[] files = Directory.GetFiles(iDirection, "*.*", SearchOption.AllDirectories);

					foreach(string strVal in files)
					{
						if(string.IsNullOrEmpty(strVal)) {
							continue;
						}
						if(strVal.EndsWith(".ds_store") == true) {
							continue;
						}
						filesList.Add(strVal);
					}
				} else {
					filesList.Add(iDirection);
				}

			}  
			catch (System.IO.DirectoryNotFoundException exp)   
			{  
				BuildLogger.LogException ("The Directory is not exist!!!(dir:{0} detail:{1})", 
					iDirection, exp.Message);
			} 

			return filesList;
		}

		#endregion

		#region Create Upload Shell

		static void CreateUploadShell() {
			const string funcBlock = "AssetBundlesBuild.CreateUploadShell()";
			BuildLogger.OpenBlock(funcBlock);

			string filePath = string.Format("{0}/../Shell/Upload.sh", Application.dataPath);
			if (File.Exists (filePath) == true) {
				File.Delete (filePath);
			}
			FileStream fs = new FileStream (filePath, FileMode.OpenOrCreate, FileAccess.Write);
			if (null == fs) {
				return;
			}
			StreamWriter sw = new StreamWriter(fs);
			if (null == sw) {
				if (null != fs) {
					fs.Flush ();
					fs.Close ();
					fs.Dispose ();
				}
				return;
			}

			// 写入文件头
			sw.WriteLine("#!/bin/bash");
			sw.Flush ();

			sw.WriteLine("");
			sw.Flush ();
		
			// 设定变量
			sw.WriteLine("# 上传根目录");
			sw.WriteLine("ROOT_DIR=bundles");
			sw.Flush ();
			sw.WriteLine("# 本地上传路径");
			sw.WriteLine(string.Format("UPLOAD_FROM_ROOT_DIR={0}/StreamingAssets", Application.dataPath));
			sw.WriteLine("");
			sw.Flush ();
			sw.WriteLine("# 上传目标平台");
			sw.WriteLine(string.Format("BUILD_TARGET={0}", UploadList.GetInstance().BuildTarget));
			sw.Flush ();
			sw.WriteLine("# App Version");
			sw.WriteLine(string.Format("APP_VERSION={0}", UploadList.GetInstance().AppVersion));
			sw.Flush ();
			sw.WriteLine("# Center Version");
			sw.WriteLine(string.Format("CENTER_VERSION={0}", UploadList.GetInstance().CenterVersion));
			sw.WriteLine("");
			sw.Flush ();

			UploadServerInfo uploadServer = ServersConf.GetInstance ().UploadServer;
			sw.WriteLine("# 检测上传目录");
			sw.WriteLine("# $1 上传目录");
			sw.WriteLine("checkUploadDir()");
			sw.WriteLine("{");
			sw.WriteLine("ftp -n<<!");
			sw.WriteLine(string.Format("open {0} {1}", uploadServer.IpAddresss, uploadServer.PortNo));
			sw.WriteLine(string.Format("user {0} {1}", uploadServer.AccountId, uploadServer.Pwd));
			sw.WriteLine("binary");
			sw.WriteLine("pwd");
			sw.WriteLine("if [ ! -d \"$1\" ]; then");
			sw.WriteLine("  mkdir \"$1\"");
			sw.WriteLine("fi");
			sw.WriteLine("prompt");
//			sw.WriteLine("ls -l");
			sw.WriteLine("close");
			sw.WriteLine("bye");
			sw.WriteLine("!");
			sw.WriteLine("}");
			sw.WriteLine("");
			sw.Flush ();

			sw.WriteLine("# 文件上传函数");
			sw.WriteLine("# $1 本地上传目录");
			sw.WriteLine("# $2 上传目标目录");
			sw.WriteLine("# $3 上传目标文件");
			sw.WriteLine("upload()");
			sw.WriteLine("{");
			sw.WriteLine("ftp -n<<!");
			sw.WriteLine(string.Format("open {0} {1}", uploadServer.IpAddresss, uploadServer.PortNo));
			sw.WriteLine(string.Format("user {0} {1}", uploadServer.AccountId, uploadServer.Pwd));
			sw.WriteLine("binary");
			sw.WriteLine("cd \"$2\"");
			sw.WriteLine("lcd \"$1\"");
			sw.WriteLine("pwd");
			sw.WriteLine("prompt");
			sw.WriteLine("put \"$3\"");
//			sw.WriteLine("ls -l");
			sw.WriteLine("close");
			sw.WriteLine("bye");
			sw.WriteLine("!");
			sw.WriteLine("}");
			sw.WriteLine("");
			sw.Flush ();


			sw.WriteLine("# 检测目录");
			sw.WriteLine("checkUploadDir $ROOT_DIR");
			sw.WriteLine("checkUploadDir $ROOT_DIR/$BUILD_TARGET");
			sw.WriteLine("checkUploadDir $ROOT_DIR/$BUILD_TARGET/$APP_VERSION");
			sw.WriteLine("");
			sw.Flush ();

			sw.WriteLine("# 上传资源文件");
			List<UploadItem> Targets = UploadList.GetInstance ().Targets;
			UploadItem[] _normals = Targets
				.Where(o => (TBundleType.Normal == o.BundleType))
				.OrderBy(o => o.No)
				.ToArray ();
			if (0 < _normals.Length) {
				sw.WriteLine ("# 检测一般文件目录");
				sw.WriteLine (string.Format ("checkUploadDir $ROOT_DIR/$BUILD_TARGET/$APP_VERSION/{0}", TBundleType.Normal.ToString ()));

				sw.WriteLine ("# 一般文件");
				foreach (UploadItem loop in _normals) {
					string fileName = UploadList.GetLocalBundleFileName (loop.ID, loop.FileType);
					sw.WriteLine (string.Format ("upload $UPLOAD_FROM_ROOT_DIR/{0} $ROOT_DIR/$BUILD_TARGET/$APP_VERSION/{0} {1}",
						TBundleType.Normal.ToString (), fileName));
				}
				sw.WriteLine ("");
				sw.Flush ();
			}

			UploadItem[] _scenes = Targets
				.Where(o => (TBundleType.Scene == o.BundleType))
				.OrderBy(o => o.No)
				.ToArray ();
			if (0 < _scenes.Length) {
				sw.WriteLine ("# 检测场景文件目录");
				sw.WriteLine (string.Format ("checkUploadDir $ROOT_DIR/$BUILD_TARGET/$APP_VERSION/{0}", TBundleType.Scene.ToString ()));
				sw.WriteLine ("# 场景文件");
				foreach (UploadItem loop in _scenes) {
					string fileName = UploadList.GetLocalBundleFileName (loop.ID, loop.FileType);
					sw.WriteLine (string.Format ("upload $UPLOAD_FROM_ROOT_DIR/{0} $ROOT_DIR/$BUILD_TARGET/$APP_VERSION/{0} {1}",
						TBundleType.Scene.ToString (), fileName));
				}
				sw.WriteLine ("");
				sw.Flush ();
			}

			// 导出依赖文件
			BundlesMap.GetInstance().ExportToJsonFile(string.Format("{0}/StreamingAssets", Application.dataPath));
			sw.WriteLine ("# 上传依赖文件");
			sw.WriteLine ("upload $UPLOAD_FROM_ROOT_DIR $ROOT_DIR/$BUILD_TARGET/$APP_VERSION BundlesMapData.json");

			// 导出上传列表文件
			UploadList.GetInstance().ExportToJsonFile(string.Format("{0}/StreamingAssets", Application.dataPath));
			sw.WriteLine ("# 上传上传列表文件");
			sw.WriteLine ("upload $UPLOAD_FROM_ROOT_DIR $ROOT_DIR/$BUILD_TARGET/$APP_VERSION UploadListData.json");

			if (0 < Targets.Count) {
				sw.WriteLine ("# 清空上传文件");
				sw.WriteLine (string.Format ("rm -rfv $UPLOAD_FROM_ROOT_DIR", TBundleType.Normal.ToString ()));
				sw.WriteLine ("");
				sw.Flush ();
			}

			sw.WriteLine("");
			sw.Flush ();

			if (null != fs) {
				fs.Flush ();
				fs.Close ();
				fs.Dispose ();
			}			

			if (null != sw) {
				sw.Flush ();
				sw.Close ();
				sw.Dispose ();
			}

			BuildLogger.CloseBlock ();
		}

		#endregion

	}
}