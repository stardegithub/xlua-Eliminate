using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO; 
using BuildSystem;
using AssetBundles;

namespace Common {
	
	/// <summary>
	/// Utility asset.
	/// </summary>
	public class UtilsAsset {	

		/// <summary>
		/// 数据路径.
		/// </summary>
		private readonly static string _dataPath = Application.dataPath;
		/// <summary>
		/// asset文件数据路径.
		/// </summary>
		private const string _assetDataDir = "Resources/Conf";

		/// <summary>
		/// 标记目标物体已改变.
		/// </summary>
		/// <param name="iTarget">已改变目标.</param>
		public static void SetAssetDirty(Object iTarget)
		{
#if UNITY_EDITOR
			if(iTarget != null) {
				EditorUtility.SetDirty (iTarget);
			}
#endif
		}

		/// <summary>
		/// 刷新.
		/// </summary>
		public static void AssetsRefresh() {
#if UNITY_EDITOR
			AssetDatabase.Refresh();	
			AssetDatabase.SaveAssets ();
#endif
		}

		/// <summary>
		/// 开始打包Bundles.
		/// </summary>
		public static void StartBuildBundles() {
		}

		/// <summary>
		/// 结束打包Bundles.
		/// </summary>
		public static void EndBuildBundles() {

			// 刷新Assets
			AssetsRefresh ();
		}

		/// <summary>	
		//	创建Asset.	
		/// </summary>	
		/// <param name="iDirPath">创建目录.</param>
		public static T CreateAsset<T> (string iDirPath = null) where T : UnityEngine.ScriptableObject {	

			T objRet = default(T);

#if UNITY_EDITOR

			string assetFullPath = null;
			try {

				assetFullPath = GetAssetFilePath<T> (iDirPath);
				if(assetFullPath.StartsWith("Resources/") == true) {
					assetFullPath = string.Format("{0}/{1}", _dataPath, assetFullPath);
				}
				if(assetFullPath.EndsWith(".asset") == false) {
					assetFullPath = string.Format("{0}.asset", assetFullPath);
				}
				if(File.Exists(assetFullPath) == true) {
					File.Delete(assetFullPath);
				}
				assetFullPath = assetFullPath.Replace(_dataPath, "Assets");
				assetFullPath = AssetDatabase.GenerateUniqueAssetPath (assetFullPath);

				T asset = ScriptableObject.CreateInstance<T> (); 	
				AssetDatabase.CreateAsset (asset, assetFullPath); 	
				AssetsRefresh();	
				EditorUtility.FocusProjectWindow ();	
				Selection.activeObject = asset;	

				if (File.Exists (assetFullPath) == true) {
					UtilsLog.Info ("UtilityAsset", "CreateAsset Successed!!!(File:{0})",
						assetFullPath);
					// 读取并返回创建对象实例
					objRet = AssetDatabase.LoadAssetAtPath<T> (assetFullPath);
				} else {
					UtilsLog.Error ("UtilityAsset", "CreateAsset Failed!!!(File:{0})",
						assetFullPath);
				}
			}
			catch(UnityException exp) {
				UtilsLog.Exception ("UtilityAsset", "CreateAsset Failed!!! DetailInfo ClassName:{0} \n AssetFile:{1} \n Error:{2}",
					typeof(T).ToString (), 
					(assetFullPath == null) ? "null" : assetFullPath, 
					exp.Message);
			}

#endif

			if (objRet != null) {
				return objRet;
			} else {
				return default(T);
			}
		}

		/// <summary>
		/// 取得Asset文件数据保存路径.
		/// </summary>
		/// <returns>Asset文件数据保存路径.</returns>
		private static string GetAssetDataDir() {
			return _assetDataDir;
		}

		/// <summary>
		/// 取得Asset文件目录路径.
		/// </summary>
		/// <returns>取得Asset文件目录路径.</returns>
		/// <param name="iDirPath">Asset存放目录文件（不指定：当前选定对象所在目录）.</param>
		/// <typeparam name="T">指定读取Asset文件绑定类.</typeparam>
		public static string GetAssetFileDir (string iDirPath = null) {
			string dirTmp = iDirPath;
			if(string.IsNullOrEmpty(dirTmp) == true) {
				dirTmp = GetAssetDataDir ();
				dirTmp = string.Format ("{0}/{1}", _dataPath, dirTmp);
			}
			return dirTmp;
		}

		/// <summary>
		/// 取得Asset文件路径.
		/// </summary>
		/// <returns>Asset文件路径.</returns>
		/// <param name="iDirPath">Asset存放目录文件（不指定：当前选定对象所在目录）.</param>
		/// <typeparam name="T">指定读取Asset文件绑定类.</typeparam>
		public static string GetAssetFilePath<T> (string iDirPath = null)  where T : UnityEngine.Object {
		
			// 目录
			string dirTmp = GetAssetFileDir(iDirPath);
				
			// 文件名指定
			string strTmp = typeof(T).ToString();
			// 因为有可能存在空间名（如：common.classA），所以截取最后一个(.)号开始的字符创
			string className = strTmp.Substring (strTmp.LastIndexOf (".") + 1);
			string assetFullPath = string.Format("{0}/{1}.asset", dirTmp, className);

			const string _key = "/Resources/";
			if (assetFullPath.Contains (_key) == true) {

				int startIndex = assetFullPath.IndexOf(_key);
				string pathTmp = assetFullPath.Substring (startIndex + 1);

				int endIndex = pathTmp.LastIndexOf (".");
				pathTmp = pathTmp.Substring (0, endIndex);
				assetFullPath = pathTmp;
			}
			return assetFullPath;
		}

		/// <summary>
		/// 取得导入/导出Asset文件为Json文件用的路径.
		/// </summary>
		/// <returns>Json文件路径.</returns>
		/// <param name="iDirPath">Asset存放目录文件（不指定：当前选定对象所在目录）.</param>
		/// <typeparam name="T">指定读取Asset文件绑定类.</typeparam>
		public static string GetJsonFilePath<T> (string iDirPath = null) {
			string path = iDirPath;
			if(true == string.IsNullOrEmpty(path)) {
				path = GetAssetFileDir(path);
				path = string.Format ("{0}/Json", path);
			}

			// 文件名指定
			string strTmp = typeof(T).ToString();
			// 因为有可能存在空间名（如：common.classA），所以截取最后一个(.)号开始的字符创
			string className = strTmp.Substring (strTmp.LastIndexOf (".") + 1);

			return string.Format("{0}/{1}.json", path, className);
		}

		/// <summary>
		/// 读取打包资源配置信息.
		/// </summary>
		/// <returns>打包资源配置信息.</returns>
		/// <param name="iDirPath">Asset存放目录文件（不指定：当前选定对象所在目录）.</param>
		public static T ReadSetting<T>(string iDirPath = null) where T : UnityEngine.ScriptableObject {

			T objRet = default(T);
			string path = null;

			try {

				path = GetAssetFilePath<T>(iDirPath);
				if(true == string.IsNullOrEmpty(path)) {
					UtilsLog.Error ("UtilityAsset", "GetAssetFilePath():Failed!!!(Dir:{0})", 
						true == string.IsNullOrEmpty(iDirPath) ? "null" : iDirPath); 
					return objRet;
				}
				UtilsLog.Info ("UtilityAsset", "ReadSetting:{0}", path);
				objRet = UtilsAsset.LoadAssetFile<T>(path);

				SetAssetDirty (objRet);

			}
			catch(System.IO.DirectoryNotFoundException exp)
			{
				UtilsLog.Exception ("UtilityAsset", "ReadSetting Failed!!! DetailInfo ClassName:{0} \n AssetFile:{1} \n Error:{2}",
					typeof(T).ToString (), 
					(path == null) ? "null" : path, 
					exp.Message);
			}

			if(default(T) == objRet) {
				objRet = UtilsAsset.CreateAsset<T> (iDirPath);
				AssetsRefresh();
				SetAssetDirty (objRet);
			}

			return objRet;
		}

		/// <summary>
		/// 删除文件.
		/// </summary>
		/// <param name="iDirPath">Asset存放目录文件（不指定：当前选定对象所在目录）.</param>
		/// <typeparam name="T">指定读取Asset文件绑定类.</typeparam>
		public static void DeleteFile<T> (string iDirPath = null) where T : JsonDataBase {

			string jsonFilePath = GetJsonFilePath<T> (iDirPath);
			if (true == File.Exists (jsonFilePath)) {
				File.Delete (jsonFilePath);
			}
		}

		/// <summary>
		/// 加载(*.asset)文件.
		/// * 加载优先顺序
		/// 1）编辑器模式加载
		/// 2）Resource下的资源
		/// </summary>
		/// <returns>加载文件对象.</returns>
		/// <param name="iPath">路径.</param>
		/// <typeparam name="T">加载对象类型.</typeparam>
		public static T LoadAssetFile<T>(string iPath) where T : Object {

			T objRet = default(T);
			TextAsset textAsset = null;

			// 1）编辑器模式加载
#if UNITY_EDITOR
			objRet = AssetDatabase.LoadAssetAtPath<T>(iPath);
			if(objRet != default(T)) {
				return objRet;
			} else {
				UtilsLog.Warning("UtilityAsset", "LoadAssetFile Failed!!! Path : {0}", iPath);
			}

#endif
			// 2）Resource下的资源
			// 若上述找不到，则在从资源文件夹中加载
			const string _key = "Resources/";
			if ((textAsset == null) && (iPath.Contains (_key) == true)) {
				
				int startIndex = iPath.IndexOf(_key);
				startIndex += _key.Length;
				string pathTmp = iPath.Substring (startIndex);

				int endIndex = pathTmp.LastIndexOf (".");
				if (-1 != endIndex) {
					pathTmp = pathTmp.Substring (0, endIndex);
				}
				Object objTemp = Resources.Load (pathTmp);
				objRet = objTemp as T;
				return objRet;
			}

			return default(T);
		}

		/// <summary>
		/// 从JSON文件，导入打包配置数据.
		/// </summary>
		/// <param name="iIsFileExist">文件是否存在标志位.</param>
		/// <param name="iJsonFileDir">导出Json文件目录.</param>
		/// <typeparam name="T">指定读取Asset文件数据类.</typeparam>
		public static T ImportDataByDir<T>(out bool iIsFileExist, string iJsonFileDir) 
			where T : JsonDataBase, new() {

			iIsFileExist = false;
			string jsonFilePath = GetJsonFilePath<T>(iJsonFileDir);
			if (false == string.IsNullOrEmpty (jsonFilePath)) {
				bool isFileExist = false;
				return ImportDataByPath<T> (out isFileExist, jsonFilePath);
			}
			return default(T);
		}

		/// <summary>
		/// 从JSON文件，导入打包配置数据(文件必须存在).
		/// </summary>
		/// <param name="iJsonFileDir">导出Json文件目录.</param>
		/// <typeparam name="T">指定读取Asset文件数据类.</typeparam>
		public static T ImportDataByPath<T>(out bool iIsFileExist, string iJsonFilePath) 
			where T : JsonDataBase, new() {

			T objRet = default(T);
			string jsonString = null;
			iIsFileExist = false;
			try {
				// 优先加载下载资源包中的信息
				TextAsset _data = DataLoader.LoadData(iJsonFilePath) as TextAsset;
				if(null != _data) {
					// 读取文件
					jsonString = _data.text;
				} else {
					// 若已经有文件不存在
					if(File.Exists(iJsonFilePath) == false) {
						UtilsLog.Warning("UtilityAsset", "ImportDataByPath():File not exist!!![File:{0}]",
							iJsonFilePath);
						return default(T);
					}
					iIsFileExist = true;
					jsonString = File.ReadAllText(iJsonFilePath);
				}
				if(false == string.IsNullOrEmpty(jsonString)) {
					objRet = UtilsJson<T>.ConvertFromJsonString(jsonString); 
					UtilsLog.Info ("UtilityAsset", "ImportDataByPath. <- Path:{0}", iJsonFilePath);
					UtilsLog.Info ("UtilityAsset", "ImportDataByPath. <- Data:{0}", jsonString);
				}
			}
			catch (System.Exception exp) {
				UtilsLog.Exception ("UtilityAsset", "ImportData Failed!!! \n ClassName:{0} \n AssetFile:{1} \n Exception:{2} \n StackTrace:{3}",
					typeof(T).ToString (), 
					(iJsonFilePath == null) ? "null" : iJsonFilePath, 
					exp.Message, exp.StackTrace);
				objRet = default(T);
			}

			return objRet;
		}

		/// <summary>
		/// 导出成JSON文件.
		/// </summary>
		/// <returns>导出路径.</returns>
		/// <param name="iInstance">欲导出实例对象.</param>
		/// <param name="iJsonFileDir">导出Json文件目录.</param>
		/// <typeparam name="T">指定读取Asset文件绑定类.</typeparam>
		public static string ExportData<T>(T iInstance, string iJsonFileDir = null) 
			where T : JsonDataBase, new() {

			string jsonFilePath = null;
			try {

				jsonFilePath = GetJsonFilePath<T>(iJsonFileDir);
				// 若已经有文件存在，则强制删除
				if (File.Exists (jsonFilePath) == true) {
					File.Delete (jsonFilePath);
				}
				// 导出JSON文件
				string jsonString = UtilsJson<T>.ConvertToJsonString(iInstance);
				File.WriteAllText (jsonFilePath, jsonString);

				UtilsLog.Info ("UtilityAsset", "ExportData. -> Path:{0}", jsonFilePath);
				UtilsLog.Info ("UtilityAsset", "ExportData. -> Data:{0}", jsonString);
			}
			catch (System.Exception exp) {
				UtilsLog.Exception ("UtilityAsset", "ExportData Failed!!! ClassName:{0} AssetFile:{1} Exception:{2} StackTrace:{3}",
					typeof(T).ToString (), 
					(jsonFilePath == null) ? "null" : jsonFilePath, 
					exp.Message, exp.StackTrace);
			}

//			AssetsRefresh();

			if (true == File.Exists (jsonFilePath)) {
				UtilsLog.Info ("UtilityAsset", "ExportData. -> {0}", jsonFilePath);
			}
			return jsonFilePath;
		}

		/// <summary>
		/// 清空目录.
		/// </summary>
		/// <param name="iDir">目录.</param>
		public static void ClearDirectory(string iDir) {

			// 清空文件
			string[] files = Directory.GetFiles (iDir);
			if ((null != files) && (1 <= files.Length)) {
				foreach (string file in files) {
					File.Delete (file);
					UtilsLog.Info ("UtilityAsset", "ClearDirectory Delete File:{0}", file);
				}
			}

			// 清空子目录
			string[] dirs = Directory.GetDirectories (iDir);
			if ((null != dirs) && (1 <= dirs.Length)) {
				foreach (string dir in dirs) {
					ClearDirectory (dir);
				}
			} else {
				Directory.Delete (iDir);
				UtilsLog.Info ("UtilityAsset", "ClearDirectory Delete Directory:{0}", iDir);
			}
		}

		/// <summary>
		/// 拷贝目录.
		/// </summary>
		/// <param name="iFromDir">源目录.</param>
		/// <param name="iToDir">目标目录.</param>
		public static void CopyDirectory(string iFromDir, string iToDir) 
		{ 
			DirectoryInfo dirInfo = new DirectoryInfo (iFromDir);
			string _toDir = string.Format ("{0}/{1}", iToDir, dirInfo.Name);
			if (false == Directory.Exists (_toDir)) {
				Directory.CreateDirectory (_toDir);
			}

			// 拷贝文件
			FileInfo[] allFiles = dirInfo.GetFiles();
			foreach (FileInfo file in allFiles) {
				if (true == file.Name.EndsWith (".meta")) {
					continue;
				}

				// 拷贝文件
				string copyToFile = string.Format ("{0}/{1}", _toDir, file.Name);
				UtilsLog.Info ("UtilityAsset", "CopyDirectory File: {0} -> {1}",
					file.FullName, copyToFile);

				File.Copy (file.FullName, copyToFile, true);
			}

			// 检索子文件夹
			DirectoryInfo[] subDirs = dirInfo.GetDirectories();
			foreach (DirectoryInfo subDir in subDirs) {
				string _fromDir = string.Format ("{0}/{1}", iFromDir, subDir.Name);
				CopyDirectory (_fromDir, _toDir);
			}
		} 

		/// <summary>
		/// 检测&创建目录.
		/// </summary>
		/// <returns><c>true</c>, OK, <c>false</c> NG.</returns>
		/// <param name="iDir">目录.</param>
		/// <param name="iSplitMark">路径分割符号.</param>
		public static bool CheckAndCreateDirByFullPath(string iDir, char iSplitMark = '/') {

			string[] subDirs = iDir.Split (iSplitMark);
			if ((null == subDirs) || (0 >= subDirs.Length)) {
				return true;
			}

			bool isOK = true;
			string checkDir = null;
			for (int idx = 0; idx < subDirs.Length; ++idx) {
				if (null == checkDir) {
					checkDir = subDirs [idx];
				} else {
					checkDir = string.Format ("{0}{1}{2}", checkDir, iSplitMark, subDirs [idx]);
				}
				if (true == string.IsNullOrEmpty (checkDir)) {
					continue;
				}
				isOK = CheckAndCreateDir (checkDir);
				if (false == isOK) {
					break;
				}
			}
			return isOK;
		}

		public static bool CheckAndCreateDir(string iDir) {
			if (true == string.IsNullOrEmpty (iDir)) {
				return true;
			}
			if (false == Directory.Exists (iDir)) {
				UtilsLog.Info ("UtilityAsset", "CheckAndCreateDir():Dir:{0}", iDir);
				Directory.CreateDirectory (iDir);
				if (false == Directory.Exists (iDir)) {
					UtilsLog.Error ("UtilityAsset", "CheckAndCreateDir():Dir Failed!!!(Dir:{0})", iDir);
					return false;
				}
			}
			return true;
		}
			
		/// <summary>
		/// 检测匹配路径（始终控制在Application.dataPath之下）.
		/// </summary>
		/// <returns>检测后的路径.</returns>
		/// <param name="iTargetPath">检测并取得匹配路径.</param>
		public static string CheckMatchPath(string iTargetPath) {
			string _targetPath = iTargetPath;
			string _rootPath = Application.dataPath;
			DirectoryInfo _targetDir = new DirectoryInfo (_targetPath);
			DirectoryInfo _rootDir = new DirectoryInfo (_rootPath);

			DirectoryInfo _targetParent = _targetDir.Parent;
			string _checkPath = string.Format ("{0}", _targetDir.Name);
			while (null != _targetParent) {

				string _nameTmp = _targetParent.Name;
				_nameTmp = _nameTmp.Replace("\\", "/");

				if (true == _nameTmp.EndsWith (":/")) {
					_checkPath = string.Format ("{0}{1}", _nameTmp, _checkPath);	
				} else if(true == _nameTmp.Equals ("/")) {
					_checkPath = string.Format ("{0}{1}", _nameTmp, _checkPath);	
				} else {
					_checkPath = string.Format ("{0}/{1}", _nameTmp, _checkPath);	
				}
				if (true == _targetParent.Name.Equals (_rootDir.Name)) {
					_targetParent = null;
				} else {
					_targetParent = _targetParent.Parent;
				}
			}
			UtilsLog.Info ("UtilityAsset", "CheckMatchPath():{0} \n -> {1}", iTargetPath, _checkPath);
			return _checkPath;
		}
	}
}
