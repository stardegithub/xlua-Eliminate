using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Common {

	/// <summary>
	/// 图片自动刷新设定编辑器.
	/// </summary>
	[CustomEditor(typeof(AutoRefreshImagesSetting))]  
	public class AutoRefreshImagesSettingEditor 
		: AssetEditorBase<AutoRefreshImagesSetting, AutoRefreshImagesSettingData> {

		#region Inspector

		/// <summary>
		/// 初始化顶部按钮列表.
		/// </summary>
		/// <param name="iTarget">目标信息.</param>
		protected override void InitTopButtons(AutoRefreshImagesSetting iTarget) {
			
			base.InitTopButtons (iTarget);

			// 刷新按钮
			if(GUILayout.Button("Refresh"))
			{
				iTarget.Refresh ();
			}
		}

		/// <summary>
		/// 初始化底部按钮列表.
		/// </summary>
		/// <param name="iTarget">目标信息.</param>
		protected override void InitBottomButtons(AutoRefreshImagesSetting iTarget) {

			base.InitBottomButtons (iTarget);

			// 刷新按钮
			if(GUILayout.Button("Refresh"))
			{
				iTarget.Refresh ();
			}
		}

		#endregion

		#region Create
	
		/// <summary>
		/// 创建图片自动刷新配置文件.
		/// </summary>
		[MenuItem("Assets/Create/AutoRefreshImagesSetting")]	
		static AutoRefreshImagesSetting Create ()	{	
			return UtilsAsset.CreateAsset<AutoRefreshImagesSetting> ();
		}

		#endregion

		#region File - Json

		[UnityEditor.MenuItem("Assets/AutoRefreshImages/Clear", false, 600)]
		static void Clear() {
			AutoRefreshImagesSetting setting = AutoRefreshImagesSetting.GetInstance();
			if (setting != null) {
				setting.Clear ();
			}

			UtilsAsset.AssetsRefresh ();
		}

		[UnityEditor.MenuItem("Assets/AutoRefreshImages/Json/Import", false, 600)]
		static void Import() {
			AutoRefreshImagesSetting setting = AutoRefreshImagesSetting.GetInstance();
			if (setting != null) {
				setting.ImportFromJsonFile ();
			}

			UtilsAsset.AssetsRefresh ();
		}

		[UnityEditor.MenuItem("Assets/AutoRefreshImages/Json/Export", false, 600)]
		static void Export() {

			AutoRefreshImagesSetting setting = AutoRefreshImagesSetting.GetInstance();
			if (setting != null) {
				setting.ExportToJsonFile ();
			}

			UtilsAsset.AssetsRefresh ();
		}

		#endregion

	}
}
