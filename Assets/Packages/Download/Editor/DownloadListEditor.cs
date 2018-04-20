using UnityEngine;
using UnityEditor;
using System.Collections;
using Common;
using NetWork.Servers;

namespace Download {

	/// <summary>
	/// Download list editor.
	/// </summary>
	[CustomEditor(typeof(DownloadList))]  
	public class DownloadListEditor 
		: AssetEditorBase<DownloadList, DownloadListData> {

		/// <summary>
		/// 初始化标题信息.
		/// </summary>
		/// <param name="iTarget">目标信息.</param>
		protected override void InitTitleInfo(DownloadList iTarget) {
			base.InitTitleInfo (iTarget);
			EditorGUILayout.LabelField ("CheckMode", iTarget.CheckMode.ToString());
		}

#region Creator

		/// <summary>
		/// 创建下载列表.
		/// </summary>
		[MenuItem("Assets/Create/DownloadList")]	
		static DownloadList Create ()	{	
			return UtilsAsset.CreateAsset<DownloadList> ();	
		}

#endregion

#region File - Json

		/// <summary>
		/// 从JSON文件导入打包配置信息(DownloadList).
		/// </summary>
		[UnityEditor.MenuItem("Assets/DownloadList/File/Json/Import", false, 600)]
		static void Import() {

			DownloadList list = DownloadList.GetInstance ();
			if (list != null) {
				list.ImportFromJsonFile();
			}

			UtilsAsset.AssetsRefresh ();
		}

		/// <summary>
		/// 将服务器信息导出为JSON文件(DownloadList).
		/// </summary>
		[UnityEditor.MenuItem("Assets/DownloadList/File/Json/Export", false, 600)]
		static void Export() {

			DownloadList list = DownloadList.GetInstance ();
			if (list != null) {
				list.ExportToJsonFile();
			}

			UtilsAsset.AssetsRefresh ();
		}

		[UnityEditor.MenuItem("Assets/DownloadList/Clear", false, 600)]
		static void Clear() {

			DownloadList list = DownloadList.GetInstance ();
			if (list != null) {
				ServersConf.GetInstance ();
#if UNITY_EDITOR
				list.Clear ();
				list.ExportToJsonFile ();
#endif
				list.Clear (true, ServersConf.BundlesDir);
				list.ExportToJsonFile (ServersConf.BundlesDir);
			}

			UtilsAsset.AssetsRefresh ();
		}

#endregion

		/// <summary>
		/// 清空.
		/// </summary>
		/// <param name="iForceClear">强力清除标识位（删除Json文件）.</param>
		protected override void Clear(bool iForceClear = false) {
			if (null == this._assetSetting) {
				return;
			}
			ServersConf.GetInstance ();
#if UNITY_EDITOR
			if(true == iForceClear) {
				this._assetSetting.Clear (true);
				this._assetSetting.ExportToJsonFile ();
			} else {
				this._assetSetting.Clear (false);
			}
#endif
			if (true == iForceClear) {
				this._assetSetting.Clear (true, ServersConf.BundlesDir);
				this._assetSetting.ExportToJsonFile (ServersConf.BundlesDir);
			} else {
				this._assetSetting.Clear (false);
			}
		}
	}
}
