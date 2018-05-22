using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Common;
using GameState.Conf;

namespace GameState.Conf.Editor {


	[CustomEditor(typeof(GameStateConf))]  
	public class GameStateConfEditor 
		: AssetEditorBase<GameStateConf, GameStateConfData> {

#region Creator

		/// <summary>
		/// 创建游戏状态配置信息.
		/// </summary>
		[MenuItem("Assets/Create/GameStateConf")]	
		static GameStateConf Create ()	{	
			return UtilsAsset.CreateAsset<GameStateConf> ();	
		}

#endregion

#region File - Json

		/// <summary>
		/// 从JSON文件导入打包配置信息(BuildInfo).
		/// </summary>
		[UnityEditor.MenuItem("Assets/GameStateConf/File/Json/Import", false, 600)]
		static void Import() {

			GameStateConf _info = GameStateConf.GetInstance ();
			if (null != _info) {
				_info.ImportFromJsonFile();
			}

			UtilsAsset.AssetsRefresh ();
		}



		/// <summary>
		/// 将打包配置信息导出为JSON文件(BuildInfo).
		/// </summary>
		[UnityEditor.MenuItem("Assets/GameStateConf/File/Json/Export", false, 600)]
		static void Export() {

			GameStateConf _info = GameStateConf.GetInstance ();
			if (null != _info) {
				_info.ExportToJsonFile();
			}

			UtilsAsset.AssetsRefresh ();
		}

		/// <summary>
		/// 清空 bundles map.
		/// </summary>
		[UnityEditor.MenuItem("Assets/GameStateConf/Clear", false, 600)]
		static void Clear() {
			GameStateConf _info = GameStateConf.GetInstance ();
			if (null != _info) {
				_info.Clear ();
			}
			UtilsAsset.AssetsRefresh ();
		}

#endregion

	}
}
