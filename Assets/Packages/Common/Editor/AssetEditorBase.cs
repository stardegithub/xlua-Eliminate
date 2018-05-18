using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using BuildSystem;
using BuildSystem.Options;

namespace Common {

	/// <summary>
	/// Asset 文件编辑器基类.
	/// </summary>
	public class AssetEditorBase<T1, T2> : EditorBase 
		where T1 : ScriptableObject, IAssetBase, new()
		where T2 : JsonDataBase, new() {

		protected T1 _assetSetting = default(T1);

		/// <summary>
		/// 导入路径.
		/// </summary>
		protected string _importDir = null;

		/// <summary>
		/// 导出路径.
		/// </summary>
		protected string _exportDir = null;

#region Inspector

		public override void OnInspectorGUI ()  {
			//base.OnInspectorGUI ();
			serializedObject.Update ();

			this._assetSetting = target as T1; 
			if (this._assetSetting != null) {

				// 初始化标题信息
				this.InitTitleInfo(this._assetSetting);

				// 刷新按钮
				EditorGUILayout.BeginHorizontal();
				this.InitTopButtons (this._assetSetting);
				EditorGUILayout.EndHorizontal ();

				// 初始化主面板
				this.InitMainPanel(this._assetSetting);

				// 刷新按钮
				EditorGUILayout.BeginHorizontal();
				this.InitBottomButtons (this._assetSetting);
				EditorGUILayout.EndHorizontal ();
			}

			// 保存变化后的值
			serializedObject.ApplyModifiedProperties();
		}

		/// <summary>
		/// 初始化标题信息.
		/// </summary>
		/// <param name="iTarget">目标信息.</param>
		protected virtual void InitTitleInfo(T1 iTarget) {
			if (true == string.IsNullOrEmpty (this._importDir)) {
				this._importDir = UtilsAsset.CheckMatchPath (this._assetSetting.GetImportPath ());
			}
			EditorGUILayout.LabelField ("Import Dir", this._importDir);
			if (true == string.IsNullOrEmpty (this._exportDir)) {
				this._exportDir = UtilsAsset.CheckMatchPath (this._assetSetting.GetExportPath ());
			}
			EditorGUILayout.LabelField ("Export Dir", this._exportDir);
		}

		/// <summary>
		/// 初始化顶部按钮列表.
		/// </summary>
		/// <param name="iTarget">目标信息.</param>
		protected virtual void InitTopButtons(T1 iTarget) {
			
			// 清空按钮
			if(GUILayout.Button("Clear"))
			{
				this.Clear();
			}

			// 强力清空按钮
			if(GUILayout.Button("ForceClear"))
			{
				this.Clear(true);
			}

			// 导入按钮
			if(GUILayout.Button("Import"))
			{
				this.Import();
			}

			// 导出按钮
			if(GUILayout.Button("Export"))
			{
				this.Export();
			}
		}

		/// <summary>
		/// 初始化主面板.
		/// </summary>
		/// <param name="iTarget">目标信息.</param>
		protected virtual void InitMainPanel(T1 iTarget) {
			if (default(T1) == iTarget) {
				return;
			}
			EditorGUILayout.PropertyField (serializedObject.FindProperty("Data"),true);
		}
			
		/// <summary>
		/// 初始化底部按钮列表.
		/// </summary>
		/// <param name="iTarget">目标信息.</param>
		protected virtual void InitBottomButtons(T1 iTarget) {

			// 清空按钮
			if(GUILayout.Button("Clear"))
			{
				this.Clear(false);
			}

			// 强力清空按钮
			if(GUILayout.Button("ForceClear"))
			{
				this.Clear(true);
			}

			// 导入按钮
			if(GUILayout.Button("Import"))
			{
				this.Import();
			}

			// 导出按钮
			if(GUILayout.Button("Export"))
			{
				this.Export();
			}
		}

		/// <summary>
		/// 清空.
		/// </summary>
		/// <param name="iForceClear">强力清除标识位（删除Json文件）.</param>
		protected virtual void Clear(bool iForceClear = false) {
			if (default(T1) == this._assetSetting) {
				return;
			}
			this._assetSetting.Clear (iForceClear);
		}
			
		/// <summary>
		/// 导入.
		/// </summary>
		protected virtual void Import() {
			if (default(T1) == this._assetSetting) {
				return;
			}
			this._assetSetting.ImportFromJsonFile (true);
		}

		/// <summary>
		/// 导出.
		/// </summary>
		protected virtual void Export() {
			if (default(T1) == this._assetSetting) {
				return;
			}
			this._assetSetting.ExportToJsonFile ();
		}

#endregion

		/// <summary>
		/// 取得当前选中对象所在目录.
		/// </summary>
		/// <returns>当前选中对象所在目录.</returns>
		protected static string GetCurDir()
		{
			UnityEngine.Object[] obj = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets) as UnityEngine.Object[];
			string path = AssetDatabase.GetAssetPath(obj[0]);

			if(path.Contains(".") == false)
			{
				path += "/";
			}

			return path;
		}

#region Creator

		/// <summary>
		/// 创建设定文件（在当前目录）.
		/// </summary>
		protected static T1 CreateAssetAtCurDir ()	{	

			string curDir = GetCurDir ();
			if (Directory.Exists (curDir) == false) {
				return null;
			}
			return UtilsAsset.CreateAsset<T1> (curDir);	
		}

		/// <summary>
		/// 创建设定文件（在当前目录）.
		/// </summary>
		protected static T1 CreateAsset ()	{	
			return UtilsAsset.CreateAsset<T1> ();	
		}

#endregion

	}

	/// <summary>
	/// Asset 文件编辑器基类(Options).
	/// </summary>
	public class AssetOptionsEditorBase<T1, T2, T3, T4> : EditorBase 
		where T1 : ScriptableObject, IAssetBase, new()
		where T2 : OptionsDataBase<T3, T4>, new() 
		where T3 : JsonDataBase, new() 
		where T4 : OptionsBaseData, new() {

		protected T1 _assetSetting = default(T1);

		/// <summary>
		/// 导入路径.
		/// </summary>
		protected string _importDir = null;

		/// <summary>
		/// 导出路径.
		/// </summary>
		protected string _exportDir = null;

		#region Inspector

		public override void OnInspectorGUI ()  {
			//base.OnInspectorGUI ();
			serializedObject.Update ();

			this._assetSetting = target as T1; 
			if (this._assetSetting != null) {

				// 初始化标题信息
				this.InitTitleInfo(this._assetSetting);

				// 刷新按钮
				EditorGUILayout.BeginHorizontal();
				this.InitTopButtons (this._assetSetting);
				EditorGUILayout.EndHorizontal ();

				// 初始化主面板
				this.InitMainPanelOfDefault(this._assetSetting);

				// 初始化选项面板
				this.InitMainPanelOfOptions(this._assetSetting);

				// 刷新按钮
				EditorGUILayout.BeginHorizontal();
				this.InitBottomButtons (this._assetSetting);
				EditorGUILayout.EndHorizontal ();
			}

			// 保存变化后的值
			serializedObject.ApplyModifiedProperties();
		}

		/// <summary>
		/// 初始化标题信息.
		/// </summary>
		/// <param name="iTarget">目标信息.</param>
		protected virtual void InitTitleInfo(T1 iTarget) {
			if (true == string.IsNullOrEmpty (this._importDir)) {
				this._importDir = UtilsAsset.CheckMatchPath (this._assetSetting.GetImportPath ());
			}
			EditorGUILayout.LabelField ("Import Dir", this._importDir);
			if (true == string.IsNullOrEmpty (this._exportDir)) {
				this._exportDir = UtilsAsset.CheckMatchPath (this._assetSetting.GetExportPath ());
			}
			EditorGUILayout.LabelField ("Export Dir", this._exportDir);
		}

		/// <summary>
		/// 初始化顶部按钮列表.
		/// </summary>
		/// <param name="iTarget">目标信息.</param>
		protected virtual void InitTopButtons(T1 iTarget) {

			// 清空按钮
			if(GUILayout.Button("Clear"))
			{
				this.Clear();
			}

			// 强力清空按钮
			if(GUILayout.Button("ForceClear"))
			{
				this.Clear(true);
			}

			// 导入按钮
			if(GUILayout.Button("Import"))
			{
				this.Import();
			}

			// 导出按钮
			if(GUILayout.Button("Export"))
			{
				this.Export();
			}
		}

		/// <summary>
		/// 初始化主面板（默认）.
		/// </summary>
		/// <param name="iTarget">目标信息.</param>
		protected virtual void InitMainPanelOfDefault(T1 iTarget) {
			if (default(T1) == iTarget) {
				return;
			}
			SerializedProperty _default = serializedObject.FindProperty ("Data.Default");
			if (null == _default) {
				this.Error ("InitMainPanelOfDefault():The Data.Default is null!!!");
				return;
			}
			EditorGUILayout.PropertyField (_default, true);
		}

		/// <summary>
		/// 初始化主面板（选项）.
		/// </summary>
		/// <param name="iTarget">目标信息.</param>
		protected virtual void InitMainPanelOfOptions(T1 iTarget) {
			if (default(T1) == iTarget) {
				return;
			}
			SerializedProperty _options = serializedObject.FindProperty ("Data.Options");
			if (null == _options) {
				this.Error ("InitMainPanelOfOptions():The Data.Options is null!!!");
				return;
			}
			EditorGUILayout.PropertyField (_options, true);
		}

		/// <summary>
		/// 初始化底部按钮列表.
		/// </summary>
		/// <param name="iTarget">目标信息.</param>
		protected virtual void InitBottomButtons(T1 iTarget) {

			// 清空按钮
			if(GUILayout.Button("Clear"))
			{
				this.Clear(false);
			}

			// 强力清空按钮
			if(GUILayout.Button("ForceClear"))
			{
				this.Clear(true);
			}

			// 导入按钮
			if(GUILayout.Button("Import"))
			{
				this.Import();
			}

			// 导出按钮
			if(GUILayout.Button("Export"))
			{
				this.Export();
			}
		}

		/// <summary>
		/// 清空.
		/// </summary>
		/// <param name="iForceClear">强力清除标识位（删除Json文件）.</param>
		protected virtual void Clear(bool iForceClear = false) {
			if (default(T1) == this._assetSetting) {
				return;
			}
			this._assetSetting.Clear (iForceClear);
		}

		/// <summary>
		/// 导入.
		/// </summary>
		protected virtual void Import() {
			if (default(T1) == this._assetSetting) {
				return;
			}
			this._assetSetting.ImportFromJsonFile (true);
		}

		/// <summary>
		/// 导出.
		/// </summary>
		protected virtual void Export() {
			if (default(T1) == this._assetSetting) {
				return;
			}
			this._assetSetting.ExportToJsonFile ();
		}

		#endregion

		/// <summary>
		/// 取得当前选中对象所在目录.
		/// </summary>
		/// <returns>当前选中对象所在目录.</returns>
		protected static string GetCurDir()
		{
			UnityEngine.Object[] obj = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets) as UnityEngine.Object[];
			string path = AssetDatabase.GetAssetPath(obj[0]);

			if(path.Contains(".") == false)
			{
				path += "/";
			}

			return path;
		}

		#region Creator

		/// <summary>
		/// 创建设定文件（在当前目录）.
		/// </summary>
		protected static T1 CreateAssetAtCurDir ()	{	

			string curDir = GetCurDir ();
			if (Directory.Exists (curDir) == false) {
				return null;
			}
			return UtilsAsset.CreateAsset<T1> (curDir);	
		}

		/// <summary>
		/// 创建设定文件（在当前目录）.
		/// </summary>
		protected static T1 CreateAsset ()	{	
			return UtilsAsset.CreateAsset<T1> ();	
		}

		#endregion

	}
}
