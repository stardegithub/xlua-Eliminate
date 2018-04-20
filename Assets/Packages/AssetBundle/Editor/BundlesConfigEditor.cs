using UnityEngine;
using UnityEditor;
using System.Collections;
using Common;
using Upload;

namespace AssetBundles {

	[CustomEditor(typeof(BundlesConfig))]  
	public class BundlesConfigEditor 
		: AssetEditorBase<BundlesConfig, BundlesConfigData> {

#region Create 

		/// <summary>
		/// 创建资源打包配置文件（用于设定当前打包对象）.
		/// </summary>
		[MenuItem("Assets/Create/Bundles/Config")]	
		static BundlesConfig CreateBundlesConfig ()	{	
			return UtilsAsset.CreateAsset<BundlesConfig> ();
		}

#endregion

#region File - Json

		[UnityEditor.MenuItem("Assets/Bundles/Clear", false, 600)]
		static void TotalClear() {
			BundlesConfig bcConfig = BundlesConfig.GetInstance();
			if (bcConfig != null) {
				bcConfig.Clear ();
			}

			BundlesMap map = BundlesMap.GetInstance ();
			if (map != null) {
				map.Clear ();
			}

			UploadList info = UploadList.GetInstance ();
			if (info != null) {
				info.Clear ();
			}

			UtilsAsset.AssetsRefresh ();
		}
			
		[UnityEditor.MenuItem("Assets/Bundles/Config/Clear", false, 600)]
		static void Clear() {
			BundlesConfig bcConfig = BundlesConfig.GetInstance();
			if (bcConfig != null) {
				bcConfig.Clear ();
			}

			UtilsAsset.AssetsRefresh ();
		}
			
		/// <summary>
		/// 从JSON文件导入打包配置信息.
		/// </summary>
		[UnityEditor.MenuItem("Assets/Bundles/Config/File/Json/Import", false, 600)]
		static void Import() {

			BundlesConfig bcConfig = BundlesConfig.GetInstance();
			if (bcConfig != null) {
				bcConfig.ImportFromJsonFile ();
			}

			UtilsAsset.AssetsRefresh ();
		}

		/// <summary>
		/// 将打包配置信息导出为JSON文件.
		/// </summary>
		[UnityEditor.MenuItem("Assets/Bundles/Config/File/Json/Export", false, 600)]
		static void Export() {

			BundlesConfig bcConfig = BundlesConfig.GetInstance();
			if (bcConfig != null) {
				bcConfig.ExportToJsonFile ();
			}

			UtilsAsset.AssetsRefresh ();
		}

#endregion

#region AssetBundles - Resource

		[UnityEditor.MenuItem("Assets/Bundles/Config/Resources/OneDir", false, 600)]
		static void BundleOneDir()
		{
			string path = GetCurDir();

			BundlesConfig bcConfig = BundlesConfig.GetInstance();
			if (bcConfig != null) {
				bcConfig.AddResource (BundleMode.OneDir, path);
			}

			UtilsAsset.AssetsRefresh ();
		}

		[UnityEditor.MenuItem("Assets/Bundles/Config/Resources/FileOneToOne", false, 600)]
		static void BundleFileOneToOne()
		{
			string path = GetCurDir();

			BundlesConfig bcConfig = BundlesConfig.GetInstance();
			if (bcConfig != null) {
				bcConfig.AddResource (BundleMode.FileOneToOne, path);
			}
			UtilsAsset.AssetsRefresh ();
		}

		[UnityEditor.MenuItem("Assets/Bundles/Config/Resources/TopDirOneToOne", false, 600)]
		static void BundleTopDirOneToOne()
		{
			string path = GetCurDir();

			BundlesConfig bcConfig = BundlesConfig.GetInstance();
			if (bcConfig != null) {
				bcConfig.AddResource (BundleMode.TopDirOneToOne, path);
			}
			UtilsAsset.AssetsRefresh ();
		}

		[UnityEditor.MenuItem("Assets/Bundles/Config/Resources/SceneOneToOne", false, 600)]
		static void BundleSceneOneToOne()
		{
			string path = GetCurDir();

			BundlesConfig bcConfig = BundlesConfig.GetInstance();
			if (bcConfig != null) {
				bcConfig.AddResource (BundleMode.SceneOneToOne, path);
			}
			UtilsAsset.AssetsRefresh ();
		}

		/// <summary>
		/// 从打包配置信息中移除当前指定对象.
		/// </summary>
		[UnityEditor.MenuItem("Assets/Bundles/Config/Resources/Remove", false, 600)]
		static void RemoveSource()
		{
			string curPath = GetCurDir();
			int lastIndex = curPath.LastIndexOf ("/");
			lastIndex = (lastIndex == (curPath.Length - 1)) ? curPath.Length : (lastIndex + 1);
			string resourcePath = curPath.Substring (0, lastIndex);

			BundlesConfig bcConfig = BundlesConfig.GetInstance();
			if (bcConfig != null) {
				bcConfig.RemoveResource (curPath);
			}
			UtilsAsset.AssetsRefresh ();
		}

		/// <summary>
		/// 清空打包资源列表.
		/// </summary>
		[UnityEditor.MenuItem("Assets/Bundles/Config/Resources/Clear", false, 600)]
		static void ClearAll() {
			BundlesConfig bcConfig = BundlesConfig.GetInstance();
			if (bcConfig != null) {
				bcConfig.ClearResources ();
			}
			UtilsAsset.AssetsRefresh ();
		}

		/// <summary>
		/// 将当前目录或者文件，添加到菜单.
		/// </summary>
		[UnityEditor.MenuItem("Assets/Bundles/Config/Resources/Ignore/Ignore", false, 600)]
		static void IgnoreCurrentTarget()
		{
			string curPath = GetCurDir();
			int lastIndex = curPath.LastIndexOf ("/");
			lastIndex = (lastIndex == (curPath.Length - 1)) ? curPath.Length : (lastIndex + 1);
			string resourcePath = curPath.Substring (0, lastIndex);

			// 设定忽略列表
			BundlesConfig bcConfig = BundlesConfig.GetInstance();
			if (bcConfig != null) {
				bcConfig.AddIgnoreTarget (resourcePath, curPath);
			}
			UtilsAsset.AssetsRefresh ();
		}

		/// <summary>
		/// 将当前目录或者文件，添加到菜单.
		/// </summary>
		[UnityEditor.MenuItem("Assets/Bundles/Config/Resources/Ignore/Remove", false, 600)]
		static void RemoveIgnoreCurrentTarget()
		{
			string curPath = GetCurDir();
			int lastIndex = curPath.LastIndexOf ("/");
			lastIndex = (lastIndex == (curPath.Length - 1)) ? curPath.Length : (lastIndex + 1);
			string resourcePath = curPath.Substring (0, lastIndex);

			// 设定忽略列表
			BundlesConfig bcConfig = BundlesConfig.GetInstance();
			if (bcConfig != null) {
				bcConfig.RemoveIgnoreInfo (resourcePath, curPath);
			}
			UtilsAsset.AssetsRefresh ();
		}

		/// <summary>
		/// 清空打包资源列表.
		/// </summary>
		[UnityEditor.MenuItem("Assets/Bundles/Config/Resources/Ignore/Clear", false, 600)]
		static void ClearAllIgnoreInfo() {

			string curPath = GetCurDir();
			int lastIndex = curPath.LastIndexOf ("/");
			lastIndex = (lastIndex == (curPath.Length - 1)) ? curPath.Length : (lastIndex + 1);
			string resourcePath = curPath.Substring (0, lastIndex);

			BundlesConfig bcConfig = BundlesConfig.GetInstance();
			if (bcConfig != null) {
				bcConfig.ClearAllIgnoreInfo (resourcePath);
			}
			UtilsAsset.AssetsRefresh ();
		}

#endregion

		#region AssetBundles - UnResource

		/// <summary>
		/// 设定当前对象为非资源对象，并添加到打包资源配置信息.
		/// </summary>
		[UnityEditor.MenuItem("Assets/Bundles/Config/UnResources/Add", false, 600)]
		static void AddUnResource()
		{
			string path = GetCurDir();

			BundlesConfig bcConfig = BundlesConfig.GetInstance();
			if (bcConfig != null) {
				bcConfig.AddUnResource (path);
			}
			UtilsAsset.AssetsRefresh ();
		}

		/// <summary>
		/// 从当前打包资源配置信息的非资源列表中移除当前对象.
		/// </summary>
		[UnityEditor.MenuItem("Assets/Bundles/Config/UnResources/Remove", false, 600)]
		static void RemoveUnResource()
		{
			string curPath = GetCurDir();
			int lastIndex = curPath.LastIndexOf ("/");
			lastIndex = (lastIndex == (curPath.Length - 1)) ? curPath.Length : (lastIndex + 1);
			string resourcePath = curPath.Substring (0, lastIndex);

			BundlesConfig bcConfig = BundlesConfig.GetInstance();
			if (bcConfig != null) {
				bcConfig.RemoveUnResource (resourcePath);
			}
			UtilsAsset.AssetsRefresh ();
		}

		/// <summary>
		/// 清空非资源列表.
		/// </summary>
		[UnityEditor.MenuItem("Assets/Bundles/Config/UnResources/Clear", false, 600)]
		static void ClearAllUnResources() {
			BundlesConfig bcConfig = BundlesConfig.GetInstance();
			if (bcConfig != null) {
				bcConfig.ClearUnResources ();
			}
			UtilsAsset.AssetsRefresh ();
		}

#endregion

	}

}
