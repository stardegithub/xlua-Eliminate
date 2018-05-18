#if UNITY_ANDROID

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using Common;
using BuildSystem;
using BuildSystem.Options;

namespace AndroidSDK.Platforms.Tiange {

	/// <summary>
	/// 天鸽SDK设定编辑器.
	/// </summary>
	[CustomEditor(typeof(TiangeSDKSettings))]  
	public class TiangeSDKSettingsEditor  
		: AssetOptionsEditorBase<TiangeSDKSettings, TiangeSDKData, TiangeSDKSettingsData, BuildSettingOptionsData> {
	
		/// <summary>
		/// 取得当前选中对象所在目录.
		/// </summary>
		/// <returns>当前选中对象所在目录.</returns>
		private static string GetCurDir()
		{
			UnityEngine.Object[] obj = Selection.GetFiltered(typeof(UnityEngine.Object), 
				SelectionMode.Assets) as UnityEngine.Object[];
			string path = AssetDatabase.GetAssetPath(obj[0]);

			if(path.Contains(".") == false)
			{
				path += "/";
			}

			return path;
		}

		/// <summary>
		/// 初始化主面板（选项）.
		/// </summary>
		/// <param name="iTarget">目标信息.</param>
		protected override void InitMainPanelOfOptions(TiangeSDKSettings iTarget) {
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
				if (true == _isOn) {
					EditorGUILayout.PropertyField (
						serializedObject.FindProperty(string.Format("Data.Options.{0}", _optionName)),true);
				}
			}
		}

#region Creator

		/// <summary>
		/// 创建下载用Bar.
		/// </summary>
		[MenuItem("Assets/Create/AndroidSDK/TiangeSDKSettings")]	
		static TiangeSDKSettings CreateSettings () {
			const string funcBlock = "TiangeSDKSettingsEditor.CreateSettings()";
			BuildLogger.OpenBlock (funcBlock);

			string curDir = GetCurDir ();
			if (Directory.Exists (curDir) == false) {
				return null;
			}
			TiangeSDKSettings settings = UtilsAsset.CreateAsset<TiangeSDKSettings> (curDir);
			if (settings != null) {
				// 初始化
				settings.Init();
			}
			BuildLogger.CloseBlock ();
			return settings;
		}

#endregion

#region MenuItem

		[UnityEditor.MenuItem("Assets/AndroidSDK/Tiange/Clear", false, 600)]
		static void Clear() {

			const string funcBlock = "TiangeSDKSettingsEditor.Clear()";
			BuildLogger.OpenBlock (funcBlock);

			TiangeSDKSettings settings = TiangeSDKSettings.GetInstance();
			if (settings != null) {
				settings.Clear ();
			}   

			BuildLogger.CloseBlock ();
		}

		[UnityEditor.MenuItem("Assets/AndroidSDK/Tiange/Json/Import", false, 600)]
		static void Import() {

			const string funcBlock = "TiangeSDKSettingsEditor.Import()";
			BuildLogger.OpenBlock (funcBlock);

			TiangeSDKSettings settings = TiangeSDKSettings.GetInstance();
			if (settings != null) {
				settings.ImportFromJsonFile ();
			}

			BuildLogger.CloseBlock ();
		}

		[UnityEditor.MenuItem("Assets/AndroidSDK/Tiange/Json/Export", false, 600)]
		static void Export() {

			const string funcBlock = "TiangeSDKSettingsEditor.Export()";
			BuildLogger.OpenBlock (funcBlock);

			TiangeSDKSettings settings = TiangeSDKSettings.GetInstance();
			if (settings != null) {
				settings.ExportToJsonFile ();
			}

			BuildLogger.CloseBlock ();
		}

#endregion

	}
}

#endif