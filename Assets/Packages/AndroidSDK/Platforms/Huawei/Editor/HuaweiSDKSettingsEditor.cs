using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using BuildSystem;
using Common;

namespace AndroidSDK.Platforms.Huawei {
	/// <summary>
	/// 华为SDK设定信息编辑器扩展.
	/// </summary>
	[CustomEditor(typeof(HuaweiSDKSettings))]  
	public class HuaweiSDKSettingsEditor 
		: AssetEditorBase<HuaweiSDKSettings, HuaweiSDKSettingsData> {
	
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
		[MenuItem("Assets/Create/AndroidSDK/HuaweiSDKSettings")]	
		static HuaweiSDKSettings CreateSettings () {
			const string funcBlock = "HuaweiSDKEditor.CreateSettings()";
			BuildLogger.OpenBlock (funcBlock);

			string curDir = GetCurDir ();
			if (Directory.Exists (curDir) == false) {
				return null;
			}
			HuaweiSDKSettings settings = UtilsAsset.CreateAsset<HuaweiSDKSettings> (curDir);
			if (settings != null) {
				// 初始化
				settings.Init();
			}
			BuildLogger.CloseBlock ();
			return settings;
		}

#endregion

#region MenuItem

		[UnityEditor.MenuItem("Assets/AndroidSDK/Huawei/Clear", false, 600)]
		static void Clear() {

			const string funcBlock = "HuaweiSDKEditor.Clear()";
			BuildLogger.OpenBlock (funcBlock);

			HuaweiSDKSettings settings = HuaweiSDKSettings.GetInstance();
			if (settings != null) {
				settings.Clear ();
			}   

			BuildLogger.CloseBlock ();
		}

		[UnityEditor.MenuItem("Assets/AndroidSDK/Huawei/Json/Import", false, 600)]
		static void Import() {

			const string funcBlock = "HuaweiSDKEditor.Import()";
			BuildLogger.OpenBlock (funcBlock);

			HuaweiSDKSettings settings = HuaweiSDKSettings.GetInstance();
			if (settings != null) {
				settings.ImportFromJsonFile ();
			}

			BuildLogger.CloseBlock ();
		}

		[UnityEditor.MenuItem("Assets/AndroidSDK/Huawei/Json/Export", false, 600)]
		static void Export() {

			const string funcBlock = "HuaweiSDKEditor.Export()";
			BuildLogger.OpenBlock (funcBlock);

			HuaweiSDKSettings settings = HuaweiSDKSettings.GetInstance();
			if (settings != null) {
				settings.ExportToJsonFile ();
			}

			BuildLogger.CloseBlock ();
		}

#endregion

	}
}
