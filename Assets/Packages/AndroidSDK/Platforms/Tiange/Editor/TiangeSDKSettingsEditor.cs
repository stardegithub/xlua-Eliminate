#if UNITY_ANDROID

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using Common;
using BuildSystem;

namespace AndroidSDK.Platforms.Tiange {

	/// <summary>
	/// 天鸽SDK设定编辑器.
	/// </summary>
	[CustomEditor(typeof(TiangeSDKSettings))]  
	public class TiangeSDKSettingsEditor  
		: AssetEditorBase<TiangeSDKSettings, TiangeSDKSettingsData> {


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