using UnityEngine;
using UnityEditor;
using System.Collections;
using BuildSystem;
using Common;

namespace CommandLine {
	/// <summary>
	/// 宏设定.
	/// </summary>
	public class DefinesSetting {

		/// <summary>
		/// 追加宏(iOS).
		/// </summary>
		static void AddIOSDefines() {

			const string funcBlock = "DefinesSetting.AddIOSDefines()";
			BuildLogger.OpenBlock(funcBlock);

			string[] _defines = BuildParameters.Defines;
			SetDefines (_defines, BuildTargetGroup.iOS);

			BuildLogger.CloseBlock();
		}

		/// <summary>
		/// 追加宏(iOS).
		/// </summary>
		static void AddAndroidDefines() {

			const string funcBlock = "DefinesSetting.AddAndroidDefines()";
			BuildLogger.OpenBlock(funcBlock);

			string[] _defines = BuildParameters.Defines;
			SetDefines (_defines, BuildTargetGroup.Android);

			BuildLogger.CloseBlock();
		}
			
		/// <summary>
		/// 设定宏.
		/// </summary>
		/// <param name="iDefines">宏.</param>
		/// <param name="iTargetGroup">目标组.</param>
		private static void SetDefines(string[] iDefines, BuildTargetGroup iTargetGroup) {

			if ((null == iDefines) || (0 >= iDefines.Length)) {
				return;
			}

			bool fileExistFlg = false;
			DefinesData _definesData = UtilsAsset.ImportDataByDir<DefinesData> (
				out fileExistFlg, DefinesWindow._jsonFileDir);
			if (null == _definesData) {
				return;
			}
			 
			// 追加
			_definesData.AddDefines (iDefines, 
				(BuildTargetGroup.Android == iTargetGroup), 
				(BuildTargetGroup.iOS == iTargetGroup));

			// 重新导出
			UtilsAsset.ExportData<DefinesData>(_definesData, DefinesWindow._jsonFileDir);

			// 应用设定信息
			_definesData.Apply();

		}
			
		/// <summary>
		/// 设定宏.
		/// </summary>
		/// <param name="iDefines">宏.</param>
		/// <param name="iTargetGroup">目标组.</param>
		public static void SetDefines(DefineInfo[] iDefines, BuildTargetGroup iTargetGroup) {

			if ((null == iDefines) || (0 >= iDefines.Length)) {
				return;
			}

			string _defines = null;
			foreach (DefineInfo define in iDefines) {
				if (true == string.IsNullOrEmpty (define.Name)) {
					continue;
				}
				if (true == string.IsNullOrEmpty (_defines)) {
					_defines = define.Name;
				} else {
					_defines = string.Format ("{0};{1}", _defines, define.Name);
				}
			}
			if (false == string.IsNullOrEmpty (_defines)) {
				PlayerSettings.SetScriptingDefineSymbolsForGroup (iTargetGroup, _defines); 
				UtilsLog.Info ("DefinesSetting", "SetDefines -> Target:{0} Defines:{1}", iTargetGroup, _defines);
				BuildLogger.LogMessage ("DefinesSetting()::Defines({0}):{1}",
					iTargetGroup.ToString(), _defines);
			}
		}
	}
}
