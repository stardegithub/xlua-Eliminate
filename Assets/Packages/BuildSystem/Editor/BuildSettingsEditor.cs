using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using Common;
using BuildSystem;
using AssetBundles;

namespace BuildSystem {

	/// <summary>
	/// 工程设置器.
	/// </summary>
	[CustomEditor(typeof(BuildSettings))]  
	public class BuildSettingsEditor 
		: AssetEditorBase<BuildSettings, BuildSettingsData> {


#region Creator

		/// <summary>
		/// 创建工程设置器.
		/// </summary>
		[MenuItem("Assets/Create/BuildSettings")]	
		static BuildSettings Create ()	{	
			return CreateAssetAtCurDir();	
		}
			
#endregion

#region XCode - MenuItem

		[UnityEditor.MenuItem("Assets/BuildSettings/XCode/Clear", false, 600)]
		static void XCodeClear() {

			BuildSettings settings = BuildSettings.GetInstance(BuildSettings.AssetFileDir);
			if (settings != null) {
				settings.XCodeClear ();
			}
		}

		[UnityEditor.MenuItem("Assets/BuildSettings/XCode/Reset", false, 600)]
		static void XCodeReset() {

			BuildSettings settings = BuildSettings.GetInstance(BuildSettings.AssetFileDir);
			if (settings != null) {
				settings.XCodeReset ();
			}
		}

		[UnityEditor.MenuItem("Assets/BuildSettings/JSon/Import", false, 600)]
		static void Import() {

			BuildSettings settings = BuildSettings.GetInstance(BuildSettings.AssetFileDir);
			if (settings != null) {
				settings.ImportFromJsonFile ();
			}
		}

		[UnityEditor.MenuItem("Assets/BuildSettings/JSon/Export", false, 600)]
		static void Export() {

			BuildSettings settings = BuildSettings.GetInstance(BuildSettings.AssetFileDir);
			if (settings != null) {
				settings.ExportToJsonFile ();
			}
		}

#endregion

	}
}
