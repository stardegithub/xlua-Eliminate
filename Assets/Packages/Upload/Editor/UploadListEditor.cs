using UnityEngine;
using UnityEditor;
using System.Collections;
using Common;

namespace Upload {

	/// <summary>
	/// Upload list editor.
	/// </summary>
	[CustomEditor(typeof(UploadList))]  
	public class UploadListEditor 
		: AssetEditorBase<UploadList, UploadListData> {

		/// <summary>
		/// 初始化主面板.
		/// </summary>
		/// <param name="iTarget">目标信息.</param>
		protected override void InitMainPanel(UploadList iTarget) {
			// 文件后缀
			iTarget.FileSuffix = EditorGUILayout.TextField ("FileSuffix", iTarget.FileSuffix);
			// 检测模式
			iTarget.CheckMode = (Upload.TCheckMode)EditorGUILayout.EnumPopup ("CheckMode", iTarget.CheckMode);
			// BuildTarget
			EditorGUILayout.LabelField ("BuildTarget", iTarget.BuildTarget);
			// AppVersion
			EditorGUILayout.LabelField ("AppVersion", iTarget.AppVersion);
			// CenterVersion
			EditorGUILayout.LabelField ("CenterVersion", iTarget.CenterVersion);
			// 检测模式
			iTarget.CompressFormat = (TCompressFormat)EditorGUILayout.EnumPopup ("CompressFormat", iTarget.CompressFormat);
			// 上传Manifest
			iTarget.ManifestUpload = EditorGUILayout.Toggle("上传Manifest", iTarget.ManifestUpload);

			// Targets
			EditorGUILayout.PropertyField (serializedObject.FindProperty("Data.Targets"),true);
		}

		/// <summary>
		/// 初始化顶部按钮列表.
		/// </summary>
		/// <param name="iTarget">目标信息.</param>
		protected override void InitTopButtons(UploadList iTarget) {

			base.InitTopButtons (iTarget);

			// 清空按钮
			if(GUILayout.Button("ClearList"))
			{
				iTarget.Data.Targets.Clear();
				iTarget.ExportToJsonFile ();
			}
		}

		/// <summary>
		/// 初始化底部按钮列表.
		/// </summary>
		/// <param name="iTarget">目标信息.</param>
		protected override void InitBottomButtons(UploadList iTarget) {

			base.InitBottomButtons (iTarget);

			// 清空按钮
			if(GUILayout.Button("ClearList"))
			{
				iTarget.Data.Targets.Clear();
				iTarget.ExportToJsonFile ();
			}
		}

#region Creator

		/// <summary>
		/// 创建资源打包地图（用于打包）.
		/// </summary>
		[MenuItem("Assets/Create/UploadList")]	
		static UploadList Create ()	{	
			return UtilsAsset.CreateAsset<UploadList> ();	
		}

#endregion

#region File - Json

		/// <summary>
		/// 从JSON文件导入打包配置信息(Info).
		/// </summary>
		[UnityEditor.MenuItem("Assets/UploadList/File/Json/Import", false, 600)]
		static void Import() {

			UploadList info = UploadList.GetInstance ();
			if (info != null) {
				info.ImportFromJsonFile();
			}

			UtilsAsset.AssetsRefresh ();
		}

		/// <summary>
		/// 将打包配置信息导出为JSON文件(Info).
		/// </summary>
		[UnityEditor.MenuItem("Assets/UploadList/File/Json/Export", false, 600)]
		static void Export() {

			UploadList info = UploadList.GetInstance ();
			if (info != null) {
				info.ExportToJsonFile();
			}

			UtilsAsset.AssetsRefresh ();
		}

		/// <summary>
		/// 清空 bundles信息.
		/// </summary>
		[UnityEditor.MenuItem("Assets/UploadList/Clear", false, 600)]
		static void Clear() {
			UploadList info = UploadList.GetInstance ();
			if (info != null) {
				info.Clear ();
			}

			UtilsAsset.AssetsRefresh ();
		}

		#endregion
	}

}