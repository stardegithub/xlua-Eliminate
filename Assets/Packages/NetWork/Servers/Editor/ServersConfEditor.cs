using UnityEngine;
using UnityEditor; 
using System.Collections;
using Common;
using AssetBundles;

namespace NetWork.Servers {

	/// <summary>
	/// 上传编辑器.
	/// </summary>
	[CustomEditor(typeof(ServersConf))]  
	public class ServersConfEditor 
		: AssetEditorBase<ServersConf, ServersConfData> {

#region Creator

		/// <summary>
		/// 创建服务器配置信息.
		/// </summary>
		[MenuItem("Assets/Create/NetWork/ServerConf")]	
		static ServersConf CreateServersConf ()	{	
			return UtilsAsset.CreateAsset<ServersConf> ();	
		}

#endregion

#region File - Json

		/// <summary>
		/// 从JSON文件导入打包配置信息(ServersConf).
		/// </summary>
		[UnityEditor.MenuItem("Assets/NetWork/ServersConf/File/Json/Import", false, 600)]
		static void Import() {

			ServersConf conf = ServersConf.GetInstance ();
			if (conf != null) {
				conf.ImportFromJsonFile();
			}

			UtilsAsset.AssetsRefresh ();
		}

		/// <summary>
		/// 将服务器信息导出为JSON文件(ServersConf).
		/// </summary>
		[UnityEditor.MenuItem("Assets/NetWork/ServersConf/File/Json/Export", false, 600)]
		static void Export() {

			ServersConf conf = ServersConf.GetInstance ();
			if (conf != null) {
				conf.ExportToJsonFile();
			}

			UtilsAsset.AssetsRefresh ();
		}

		[UnityEditor.MenuItem("Assets/NetWork/ServersConf/Clear", false, 600)]
		static void Clear() {
			ServersConf conf = ServersConf.GetInstance ();
			if (conf != null) {
				conf.Clear ();
			}

			UtilsAsset.AssetsRefresh ();
		}

		#endregion
	}

}
