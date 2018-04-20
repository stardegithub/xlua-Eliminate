using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using BuildSystem;
using Common;

namespace AndroidSDK.OneSDK {

	/// <summary>
	/// 易接SDK接入设定文件编辑器.
	/// </summary>
	[CustomEditor(typeof(OneSDKSettings))]  
	public class OneSDKSettingsEditor 
		: AssetEditorBase<OneSDKSettings, OneSDKSettingsData> {

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
		[MenuItem("Assets/Create/AndroidSDK/OneSDKSettings")]	
		static OneSDKSettings CreateSettings () {
			const string funcBlock = "OneSDKSettingsEditor.CreateSettings()";
			BuildLogger.OpenBlock (funcBlock);

			string curDir = GetCurDir ();
			if (Directory.Exists (curDir) == false) {
				return null;
			}
			OneSDKSettings settings = UtilsAsset.CreateAsset<OneSDKSettings> (curDir);
			if (settings != null) {
				// 初始化
				settings.Init();
			}
			BuildLogger.CloseBlock ();
			return settings;
		}

#endregion

#region MenuItem

		[UnityEditor.MenuItem("Assets/AndroidSDK/OneSDK/Clear", false, 600)]
		static void Clear() {

			const string funcBlock = "OneSDKSettingsEditor.Clear()";
			BuildLogger.OpenBlock (funcBlock);

			OneSDKSettings settings = OneSDKSettings.GetInstance();
			if (settings != null) {
				settings.Clear ();
			}   

			BuildLogger.CloseBlock ();
		}

		[UnityEditor.MenuItem("Assets/AndroidSDK/OneSDK/Json/Import", false, 600)]
		static void Import() {

			const string funcBlock = "OneSDKSettingsEditor.Import()";
			BuildLogger.OpenBlock (funcBlock);

			OneSDKSettings settings = OneSDKSettings.GetInstance();
			if (settings != null) {
				settings.ImportFromJsonFile ();
			}

			BuildLogger.CloseBlock ();
		}

		[UnityEditor.MenuItem("Assets/AndroidSDK/OneSDK/Json/Export", false, 600)]
		static void Export() {

			const string funcBlock = "OneSDKSettingsEditor.Export()";
			BuildLogger.OpenBlock (funcBlock);

			OneSDKSettings settings = OneSDKSettings.GetInstance();
			if (settings != null) {
				settings.ExportToJsonFile ();
			}

			BuildLogger.CloseBlock ();
		}

#endregion

	}
}
