using UnityEngine;
using UnityEditor;
using System.Collections;
using Common;

namespace AssetBundles {

	[CustomEditor(typeof(BundlesMap))]  
	public class BundlesMapEditor 
		: AssetEditorBase<BundlesMap, BundlesMapData> {

#region Create 

		/// <summary>
		/// 创建资源打包地图（用于打包）.
		/// </summary>
		[MenuItem("Assets/Create/Bundles/Map")]	
		static BundlesMap CreateBundlesMap ()	{	
			return UtilsAsset.CreateAsset<BundlesMap> ();	
		}

#endregion

#region File - Json

		/// <summary>
		/// 从JSON文件导入打包配置信息(Map).
		/// </summary>
		[UnityEditor.MenuItem("Assets/Bundles/Map/File/Json/Import", false, 600)]
		static void ImportFromMapJsonFile() {

			BundlesMap map = BundlesMap.GetInstance ();
			if (map != null) {
				map.ImportFromJsonFile();
			}

			UtilsAsset.AssetsRefresh ();
		}

		/// <summary>
		/// 将打包配置信息导出为JSON文件(Map).
		/// </summary>
		[UnityEditor.MenuItem("Assets/Bundles/Map/File/Json/Export", false, 600)]
		static void ExportToMapJsonFile() {

			BundlesMap map = BundlesMap.GetInstance ();
			if (map != null) {
				map.ExportToJsonFile();
			}

			UtilsAsset.AssetsRefresh ();
		}

		/// <summary>
		/// 清空 bundles map.
		/// </summary>
		[UnityEditor.MenuItem("Assets/Bundles/Map/Clear", false, 600)]
		static void ClearBundlesMap() {
			BundlesMap map = BundlesMap.GetInstance ();
			if (map != null) {
				map.Clear ();
			}

			UtilsAsset.AssetsRefresh ();
		}

#endregion

	}
}
