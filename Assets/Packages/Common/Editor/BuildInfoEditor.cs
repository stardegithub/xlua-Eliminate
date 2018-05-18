using UnityEngine;
using UnityEditor;
using System.Collections;
using BuildSystem.Options;

namespace Common { 

	/// <summary>
	/// 打包信息编辑器.
	/// </summary>
	[CustomEditor(typeof(BuildInfo))]  
	public class BuildInfoEditor 
		: AssetOptionsEditorBase<BuildInfo, BuildInfoData, BuildDefaultData, BuildOptionData> {

		/// <summary>
		/// 初始化主面板（选项）.
		/// </summary>
		/// <param name="iTarget">目标信息.</param>
		protected override void InitMainPanelOfOptions(BuildInfo iTarget) {
			if (null == iTarget) {
				return;
			}
			EditorGUILayout.LabelField ("Options");

			string[] _optionNames = System.Enum.GetNames (typeof(TSDKOptions));
			for (int idx = 0; idx < _optionNames.Length; ++idx) {
				string _optionName = _optionNames [idx];
				if (true == string.IsNullOrEmpty (_optionName)) {
					continue;
				}
				TSDKOptions _option = (TSDKOptions)System.Enum.Parse (typeof(TSDKOptions), _optionName);
				if (TSDKOptions.None == _option) {
					continue;
				}

				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField (" ", 
					GUILayout.Width(10.0f), GUILayout.Height(20.0f));
				EditorGUILayout.LabelField (_optionName, 
					GUILayout.Width(100.0f), GUILayout.Height(20.0f));

				bool _isOn = EditorGUILayout.Toggle (iTarget.Data.Options.isOptionValid(_option));

				iTarget.Data.Options.SetOptionOnOrOff (_option, _isOn);
				EditorGUILayout.EndHorizontal ();
			}
		}

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
