using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.IO;
using System.Collections;

#if UNITY_EDITOR

namespace Common {

#region WindowConfInfo

	/// <summary>
	/// 窗口配置信息.
	/// </summary>
	public abstract class WindowConfInfoBase {

		/// <summary>
		/// 构造函数.
		/// </summary>
		public WindowConfInfoBase () {

			// 初始化
			this.Init();

		}

		/// <summary>
		/// 窗口名.
		/// </summary>
		public string WindowName;

		/// <summary>
		/// 标题.
		/// </summary>
		public string Title;

		/// <summary>
		/// 行高.
		/// </summary>
		public float LineHeight;

		/// <summary>
		/// 表示范围.
		/// </summary>
		public Rect DisplayRect;

		/// <summary>
		/// 初始化.
		/// </summary>
		public abstract void Init();

	}

#endregion

#region WindowDataBase

	/// <summary>
	/// 设定信息.
	/// </summary>
	public abstract class WindowDataBase : JsonDataBase {
		
		/// <summary>
		/// 构造函数.
		/// </summary>
		public WindowDataBase () {

			// 初始化
			this.Init();

		}

		/// <summary>
		/// 初始化.
		/// </summary>
		public override void Init() {}

		/// <summary>
		/// 重置.
		/// </summary>
		public override void Reset() {}

		/// <summary>
		/// 清空.
		/// </summary>
		public override void Clear() {}

		/// <summary>
		/// 应用.
		/// </summary>
		public abstract void Apply();

	}

#endregion

	/// <summary>
	/// 窗口基类.
	/// </summary>
	public abstract class WindowBase<T1, T2> : EditorWindow where T1 : WindowDataBase, new() where T2 : WindowConfInfoBase, new() {

		protected static readonly float _DEFAULT_LINE_HEIGHT = 20.0f;

		/// <summary>
		/// 类名.
		/// </summary>
		private string _className = null;
		public string ClassName {
			get {
				if (true == string.IsNullOrEmpty (_className)) {
					_className = this.GetType ().Name;
				}
				return _className;
			}
		}

		/// <summary>
		/// 暂停绘制.
		/// </summary>
		protected bool isPause = false;

		/// <summary>
		/// 数据.
		/// </summary>
		public T1 Data = new T1();

		#region WindowBase - ConfInfo

		/// <summary>
		/// 配置信息.
		/// </summary>
		protected static T2 ConfInfo = new T2();

		/// <summary>
		/// 窗口名.
		/// </summary>
		protected static string WindowName { 
			get { 
				if (null == ConfInfo) {
					return null;
				}
				return ConfInfo.WindowName;
			}
		}

		/// <summary>
		/// 标题.
		/// </summary>
		protected static string Title { 
			get { 
				if (null == ConfInfo) {
					return null;
				}
				return ConfInfo.Title;
			}
		}

		/// <summary>
		/// 行高.
		/// </summary>
		protected static float LineHeight { 
			get { 
				if (null == ConfInfo) {
					return _DEFAULT_LINE_HEIGHT;
				}
				if(0.0f >= ConfInfo.LineHeight)  {
					return _DEFAULT_LINE_HEIGHT;
				}
				return ConfInfo.LineHeight;
			}
		}

		/// <summary>
		/// 表示范围.
		/// </summary>
		protected static Rect DisplayRect { 
			get { 
				if (null == ConfInfo) {
					return new Rect(0.0f, 0.0f, 100.0f, 100.0f);
				}
				return ConfInfo.DisplayRect;
			}
		}

		#endregion

		protected string _jsonPath = null;
		/// <summary>
		/// 路径(Json).
		/// </summary>
		/// <value>路径.</value>
		public string JsonPath {
			get {
				if (true == string.IsNullOrEmpty (this._jsonPath)) {
					return null;
				}
				return _jsonPath;
			}
		}

		/// <summary>
		/// 清空数据.
		/// </summary>
		protected void ClearData() {
			if (default(T1) != this.Data) {
				this.Data.Clear ();
			}
		}

		#region WindowBase - virtual

		/// <summary>
		/// 从JSON文件，导入打包配置信息.
		/// </summary>
		/// <returns><c>true</c>, 导入成功, <c>false</c> 导入失败.</returns>
		/// <param name="iForceClear">强制清空.</param>
		public virtual bool ImportFromJsonFile(bool iForceClear = true) {
			return ImportFromJsonFile (this.JsonPath, iForceClear);
		}

		/// <summary>
		/// 从JSON文件，导入打包配置信息.
		/// </summary>
		/// <returns><c>true</c>, 导入成功, <c>false</c> 导入失败.</returns>
		/// <param name="iImportDir">导入路径.</param>
		/// <param name="iForceClear">强制清空.</param>
		public virtual bool ImportFromJsonFile(string iImportDir, bool iForceClear = true) {
			string importDir = iImportDir;
			if (true == string.IsNullOrEmpty (iImportDir)) {
				importDir = this.JsonPath;
			}
			bool fileExistFlg = false;
			T1 jsonData = UtilsAsset.ImportDataByDir<T1> (out fileExistFlg, importDir);
			if (jsonData != null) {
				this.ApplyImportData (jsonData, iForceClear);
				return true;
			} 
			return false;
		}

		/// <summary>
		/// 导出成JSON文件.
		/// </summary>
		/// <returns>导出文件(Json格式).</returns>
		public virtual string ExportToJsonFile() {
			return ExportToJsonFile (this.JsonPath);
		}

		/// <summary>
		/// 导出成JSON文件.
		/// </summary>
		/// <returns>导出文件(Json格式).</returns>
		/// <param name="iExportDir">导出路径.</param>
		public virtual string ExportToJsonFile(string iExportDir) {
			string exportDir = iExportDir;
			if (true == string.IsNullOrEmpty (exportDir)) {
				exportDir = this.JsonPath;
			}
			if (false == UtilsAsset.CheckAndCreateDirByFullPath (this.JsonPath)) {
				UtilsLog.Error (this.ClassName, "ExportToJsonFile -> CheckAndCreateDirByFullPath Failed!!! \n (Path:{0})",
					this.JsonPath);
				return null;
			}
			return UtilsAsset.ExportData<T1> (this.Data, exportDir);
		}

		/// <summary>
		/// 初始化.
		/// </summary>
		/// <param name="iJsonFilePath">Json文件路径.</param>
		public virtual bool Init(string iJsonFilePath = null) {
			// Json保存文件路径
			this._jsonPath = iJsonFilePath;
			// 初始化窗口尺寸大小
			this.InitWindowSizeInfo (DisplayRect);

			// 初始化时导入最新
			return this.ImportFromJsonFile(true);
		}

		/// <summary>
		/// 清空.
		/// </summary>
		/// <param name="iIsFileDelete">删除数据文件标志位.</param>
		public virtual void Clear(bool iIsFileDelete = false) {

			// 清空数据
			this.ClearData();

			// 删除数据文件
			if (true == iIsFileDelete) {
				UtilsAsset.DeleteFile<T1> ();
			}

			UtilsAsset.SetAssetDirty (this);
		}

		#endregion

		#region WindowBase - GUI

		void OnGUI () {

			if (true == isPause) {
				return;
			}
			Rect _DIYRect = ConfInfo.DisplayRect;

			EditorGUILayout.LabelField (Title);
			_DIYRect.y += LineHeight;
			_DIYRect.height -= LineHeight;
			_DIYRect.height -= LineHeight * 1.5f;

			// 绘制Window
			this.OnWindowGUI ();

			EditorGUILayout.BeginHorizontal ();
			float buttonWidth = _DIYRect.width - 6.0f * 4;
			buttonWidth /= 4.0f;

			if(GUILayout.Button("Clear",GUILayout.Width(buttonWidth)))
			{
				this.OnClearClick();
			}
			if(GUILayout.Button("Import",GUILayout.Width(buttonWidth)))
			{
				this.OnImportClick();
			}
			if(GUILayout.Button("Export",GUILayout.Width(buttonWidth)))
			{
				this.OnExportClick();
			}
			if(GUILayout.Button("Apply",GUILayout.Width(buttonWidth)))
			{
				this.OnApplyClick();
			}
			EditorGUILayout.EndHorizontal ();
		}

		#endregion

		#region WindowBase - virtual

		/// <summary>
		/// 清空按钮点击事件.
		/// </summary>
		protected virtual void OnClearClick() {
			UtilsLog.Info (this.ClassName, "OnClearClick");
			// 清空自定义宏一览
			this.Clear(true);
		}
			
		/// <summary>
		/// 导入按钮点击事件.
		/// </summary>
		protected virtual void OnImportClick() {
			UtilsLog.Info (this.ClassName, "OnImportClick");
			this.ImportFromJsonFile (true);
		}

		/// <summary>
		/// 导出按钮点击事件.
		/// </summary>
		protected virtual void OnExportClick() {
			UtilsLog.Info (this.ClassName, "OnExportClick");
			this.ExportToJsonFile ();
		}

		/// <summary>
		/// 应用按钮点击事件.
		/// </summary>
		protected virtual void OnApplyClick() {
			UtilsLog.Info (this.ClassName, "OnApplyClick");
			// 先导出保存
			this.ExportToJsonFile ();

			// 应用
			this.Apply();
		}

		/// <summary>
		/// 初始化窗口尺寸信息.
		/// </summary>
		/// <param name="iDisplayRect">表示范围.</param>
		protected virtual void InitWindowSizeInfo(Rect iDisplayRect) {
			UtilsLog.Info (this.ClassName, "InitWindowSizeInfo Rect(X:{0} Y:{1} Width:{2} Height:{3})",
				iDisplayRect.x, iDisplayRect.y, iDisplayRect.width, iDisplayRect.height);
		}
			
		/// <summary>
		/// 应用.
		/// </summary>
		public virtual void Apply () {
			if (null == this.Data) {
				return;
			}
			this.Data.Apply ();
		}
	
		#endregion

		#region WindowBase - abstract

		/// <summary>
		/// 绘制WindowGUI.
		/// </summary>
		protected abstract void OnWindowGUI ();

		/// <summary>
		/// 应用导入数据数据.
		/// </summary>
		/// <param name="iData">数据.</param>
		/// <param name="iForceClear">强制清空标志位.</param>
		protected abstract void ApplyImportData (T1 iData, bool iForceClear);

		#endregion
	}

}

#endif
