using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Common {

	/// <summary>
	/// 打包信息编辑器.
	/// </summary>
	[CustomEditor(typeof(BuildInfo))]  
	public class BuildInfoEditor 
		: AssetEditorBase<BuildInfo, BuildInfoData> {

#region Create

		/// <summary>
		/// 创建打包信息配置文件.
		/// </summary>
		[MenuItem("Assets/Create/BuildInfo")]	
		static BuildInfo Create ()	{	
			return CreateAsset();  
		}

#endregion

#region File - Json

		/// <summary>
		/// 从JSON文件导入打包配置信息(BuildInfo).
		/// </summary>
		[UnityEditor.MenuItem("Assets/BuildInfo/File/Json/Import", false, 600)]
		static void Import() {

			BuildInfo _info = BuildInfo.GetInstance ();
			if (null != _info) {
				_info.ImportFromJsonFile();
			}

			UtilsAsset.AssetsRefresh ();
		}



		/// <summary>
		/// 将打包配置信息导出为JSON文件(BuildInfo).
		/// </summary>
		[UnityEditor.MenuItem("Assets/BuildInfo/File/Json/Export", false, 600)]
		static void Export() {

			BuildInfo _info = BuildInfo.GetInstance ();
			if (null != _info) {
				_info.ExportToJsonFile();
			}

			UtilsAsset.AssetsRefresh ();
		}

		/// <summary>
		/// 清空 bundles map.
		/// </summary>
		[UnityEditor.MenuItem("Assets/BuildInfo/Clear", false, 600)]
		static void Clear() {
			BuildInfo _info = BuildInfo.GetInstance ();
			if (null != _info) {
				_info.Clear ();
			}
			UtilsAsset.AssetsRefresh ();
		}

#endregion

	}
}
