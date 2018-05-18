using UnityEngine;
using System.IO;
using System.Collections;
using BuildSystem.Options;

namespace Common {

	/// <summary>
	/// Asset设定接口.
	/// </summary>
	public interface IAssetBase {

		/// <summary>
		/// 初始化.
		/// </summary>
		/// <param name="iAssetFilePath">asset文件路径.</param>
		bool Init(string iAssetFilePath = null);

		/// <summary>
		/// 刷新.
		/// </summary>
		void Refresh();

		/// <summary>
		/// 清空.
		/// </summary>
		/// <param name="iIsFileDelete">删除数据文件标志位.</param>
		/// <param name="iDirPath">Asset存放目录文件（不指定：当前选定对象所在目录）.</param>
		void Clear(bool iIsFileDelete = false, string iDirPath = null);

		/// <summary>
		/// 取得导入路径.
		/// </summary>
		/// <returns>导入路径.</returns>
		string GetImportPath ();

		/// <summary>
		/// 从JSON文件，导入打包配置信息.
		/// </summary>
		/// <returns><c>true</c>, 导入成功, <c>false</c> 导入失败.</returns>
		/// <param name="iForceClear">强制清空.</param>
		bool ImportFromJsonFile(bool iForceClear = true);

		/// <summary>
		/// 从JSON文件，导入打包配置信息.
		/// </summary>
		/// <returns><c>true</c>, 导入成功, <c>false</c> 导入失败.</returns>
		/// <param name="iImportDir">导入路径.</param>
		/// <param name="iForceClear">强制清空.</param>
		bool ImportFromJsonFile(string iImportDir, bool iForceClear = true);

		/// <summary>
		/// 取得导出路径.
		/// </summary>
		/// <returns>导出路径.</returns>
		string GetExportPath ();

		/// <summary>
		/// 导出成JSON文件.
		/// </summary>
		/// <returns>导出文件(Json格式)路径.</returns>
		string ExportToJsonFile();

		/// <summary>
		/// 导出成JSON文件.
		/// </summary>
		/// <returns>导出文件(Json格式)路径.</returns>
		/// <param name="iExportDir">导出目录路径.</param>
		string ExportToJsonFile(string iExportDir);
	}

	/// <summary>
	/// Asset设定类.
	/// </summary>
	public abstract class AssetBase<T1, T2> : ScriptableObject, IAssetBase, ILogExtension
		where T1 : ScriptableObject, IAssetBase, new()
		where T2 : JsonDataBase, new() {
	
		/// <summary>
		/// 实例.
		/// </summary>
		private static T1 _instance = default(T1);    

		/// <summary>
		/// 取得实例.
		/// </summary>
		/// <returns>实例.</returns>
		/// <param name="iPath">读取路径.</param>
		public static T1 GetInstance(string iPath = null) {
			if (_instance == null) {
				_instance = UtilsAsset.ReadSetting<T1>(iPath);
				string _name = typeof(T1).Name;
				if ((null != _instance) && (true == _instance.Init ())) {
					UtilsLog.Info (_name, "GetInstance Successed!!!");
				} else {
					UtilsLog.Error (_name, "GetInstance Failed!!!");
					return null;
				}
			}
			return _instance;
		}

#if BUILD_DEBUG

		/// <summary>
		/// 类名.
		/// </summary>
		private string _className = null;
		public string ClassName {
			get { 
				if(false == string.IsNullOrEmpty(_className)) {
					return _className;
				}
				_className = GetType().Name;
				return _className;
			}
		}

#endif

		/// <summary>
		/// 数据.
		/// </summary>
		public T2 Data = new T2();

		protected string _path = null;
		/// <summary>
		/// 路径.
		/// </summary>
		/// <value>路径.</value>
		public string Path {
			get { 
				if (true == string.IsNullOrEmpty (this._path)) {
					_path = UtilsAsset.GetAssetFileDir ();
					_path = UtilsAsset.CheckMatchPath (_path);
				}
				return _path;
			}
		}

		private string _importJsonPath = null;
		/// <summary>
		/// 导入路径(Json).
		/// </summary>
		/// <value>导入路径(Json).</value>
		public string ImportJsonPath {
			get {
				if (true == string.IsNullOrEmpty (this._importJsonPath)) {
					if (false == string.IsNullOrEmpty (this.Path)) {
						this._importJsonPath = string.Format ("{0}/Json", this.Path);
						if (false == Directory.Exists (this._importJsonPath)) {
							Directory.CreateDirectory (this._importJsonPath);
						}
					}
				}
				return this._importJsonPath;
			}
			protected set { 
				this._importJsonPath = value;
			}
		}

		private string _exportJsonPath = null;
		/// <summary>
		/// 导出路径(Json).
		/// </summary>
		/// <value>导出路径(Json).</value>
		public string ExportJsonPath {
			get {
				if (true == string.IsNullOrEmpty (this._exportJsonPath)) {
					if (false == string.IsNullOrEmpty (this.Path)) {
						this._exportJsonPath = string.Format ("{0}/Json", this.Path);
						if (false == Directory.Exists (this._exportJsonPath)) {
							Directory.CreateDirectory (this._exportJsonPath);
						}
					}
				}
				return this._exportJsonPath;
			}
			protected set { 
				this._exportJsonPath = value;
			}
		}

		/// <summary>
		/// 清空数据.
		/// </summary>
		protected virtual void ClearData() {
			if (default(T2) != this.Data) {
				this.Data.Clear ();
			}
		}

#region virtual

		/// <summary>
		/// 取得导入路径.
		/// </summary>
		/// <returns>导入路径.</returns>
		public virtual string GetImportPath () {
			return this.ImportJsonPath;
		}

		/// <summary>
		/// 从JSON文件，导入打包配置信息.
		/// </summary>
		/// <returns><c>true</c>, 导入成功, <c>false</c> 导入失败.</returns>
		/// <param name="iForceClear">强制清空.</param>
		public virtual bool ImportFromJsonFile(bool iForceClear = true) {
			return ImportFromJsonFile (this.GetImportPath(), iForceClear);
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
				importDir = this.GetImportPath ();
			} else {
				this.ImportJsonPath = iImportDir;
			}
			bool fileExistFlg = false;
			T2 jsonData = UtilsAsset.ImportDataByDir<T2> (out fileExistFlg, importDir);
			if (jsonData != null) {
				this.ApplyData (jsonData, iForceClear);
				return true;
			} 
			// 文件不存在则，视为导入成功
			if (false == fileExistFlg) {  
				this.Init ();
				return true;
			}
			return false;
		}

		/// <summary>
		/// 取得导出路径.
		/// </summary>
		/// <returns>导出路径.</returns>
		public virtual string GetExportPath () {
			return this.ExportJsonPath;
		}

		/// <summary>
		/// 导出成JSON文件.
		/// </summary>
		/// <returns>导出文件(Json格式).</returns>
		public virtual string ExportToJsonFile() {
			return ExportToJsonFile (this.GetExportPath());
		}

		/// <summary>
		/// 导出成JSON文件.
		/// </summary>
		/// <returns>导出文件(Json格式).</returns>
		/// <param name="iExportDir">导出路径.</param>
		public virtual string ExportToJsonFile(string iExportDir) {
			string exportDir = iExportDir;
			if (true == string.IsNullOrEmpty (exportDir)) {
				exportDir = this.GetExportPath ();
			} else {
				this.ExportJsonPath = iExportDir;
			}
			return UtilsAsset.ExportData<T2> (this.Data, exportDir);
		}

		/// <summary>
		/// 初始化.
		/// </summary>
		/// <param name="iAssetFilePath">Asset文件路径.</param>
		public virtual bool Init(string iAssetFilePath = null) {
			this._path = iAssetFilePath;
			return this.InitAsset ();
		}
			
		/// <summary>
		/// 刷新.
		/// </summary>
		public virtual void Refresh() { }

		/// <summary>
		/// 清空.
		/// </summary>
		/// <param name="iIsFileDelete">删除数据文件标志位.</param>
		/// <param name="iDirPath">Asset存放目录文件（不指定：当前选定对象所在目录）.</param>
		public virtual void Clear(bool iIsFileDelete = false, string iDirPath = null) {

			// 清空数据
			this.ClearData();

			// 删除数据文件
			if (true == iIsFileDelete) {
				UtilsAsset.DeleteFile<T2> (iDirPath);
			}

			UtilsAsset.SetAssetDirty (this);
		}

		/// <summary>
		/// 初始化数据.
		/// </summary>
		/// <returns><c>true</c>, OK, <c>false</c> NG.</returns>
		public virtual bool InitAsset () { return true; }

#endregion

#region abstract

		/// <summary>
		/// 用用数据.
		/// </summary>
		/// <param name="iData">数据.</param>
		/// <param name="iForceClear">强制清空标志位.</param>
		protected abstract void ApplyData (T2 iData, bool iForceClear);

#endregion

		/// <summary>
		/// 日志输出：消息.
		/// </summary>
		/// <param name="iScript">脚本.</param>
		/// <param name="iFormat">格式.</param>
		/// <param name="iArgs">参数.</param>
		public void Info(string iFormat, params object[] iArgs) {
#if BUILD_DEBUG
			UtilsLog.Info (this.ClassName, iFormat, iArgs);
#endif
		}

		/// <summary>
		/// 日志输出：警告.
		/// </summary>
		/// <param name="iScript">脚本.</param>
		/// <param name="iFormat">格式.</param>
		/// <param name="iArgs">参数.</param>
		public void Warning(string iFormat, params object[] iArgs) {
#if BUILD_DEBUG
			UtilsLog.Warning (this.ClassName, iFormat, iArgs);
#endif
		}

		/// <summary>
		/// 日志输出：错误.
		/// </summary>
		/// <param name="iScript">脚本.</param>
		/// <param name="iFormat">格式.</param>
		/// <param name="iArgs">参数.</param>
		public void Error(string iFormat, params object[] iArgs) {
#if BUILD_DEBUG
			UtilsLog.Error (this.ClassName, iFormat, iArgs);
#endif
		}

		/// <summary>
		/// 日志输出：异常.
		/// </summary>
		/// <param name="iScript">脚本.</param>
		/// <param name="iFormat">格式.</param>
		/// <param name="iArgs">参数.</param>
		public void Exception(string iFormat, params object[] iArgs) {
#if BUILD_DEBUG
			UtilsLog.Exception (this.ClassName, iFormat, iArgs);
#endif
		}
	}
		
	/// <summary>
	/// Asset设定类(带Options).
	/// </summary>
	public abstract class AssetOptionsBase<T1, T2, T3, T4> : ScriptableObject, IAssetBase, ILogExtension
		where T1 : ScriptableObject, IAssetBase, new()
		where T2 : OptionsDataBase<T3, T4>, new()
		where T3 : JsonDataBase, new()
		where T4 : OptionsBaseData, new() {

		/// <summary>
		/// 实例.
		/// </summary>
		private static T1 _instance = default(T1);    

		/// <summary>
		/// 取得实例.
		/// </summary>
		/// <returns>实例.</returns>
		/// <param name="iPath">读取路径.</param>
		public static T1 GetInstance(string iPath = null) {
			if (_instance == null) {
				_instance = UtilsAsset.ReadSetting<T1>(iPath);
				string _name = typeof(T1).Name;
				if ((null != _instance) && (true == _instance.Init ())) {
					UtilsLog.Info (_name, "GetInstance Successed!!!");
				} else {
					UtilsLog.Error (_name, "GetInstance Failed!!!");
					return null;
				}
			}
			return _instance;
		}

#if BUILD_DEBUG

		/// <summary>
		/// 类名.
		/// </summary>
		private string _className = null;
		public string ClassName {
			get { 
				if(false == string.IsNullOrEmpty(_className)) {
					return _className;
				}
				_className = GetType().Name;
				return _className;
			}
		}

#endif

		/// <summary>
		/// 数据.
		/// </summary>
		public T2 Data = new T2();

		protected string _path = null;
		/// <summary>
		/// 路径.
		/// </summary>
		/// <value>路径.</value>
		public string Path {
			get { 
				if (true == string.IsNullOrEmpty (this._path)) {
					_path = UtilsAsset.GetAssetFileDir ();
					_path = UtilsAsset.CheckMatchPath (_path);
				}
				return _path;
			}
		}

		private string _importJsonPath = null;
		/// <summary>
		/// 导入路径(Json).
		/// </summary>
		/// <value>导入路径(Json).</value>
		public string ImportJsonPath {
			get {
				if (true == string.IsNullOrEmpty (this._importJsonPath)) {
					if (false == string.IsNullOrEmpty (this.Path)) {
						this._importJsonPath = string.Format ("{0}/Json", this.Path);
						if (false == Directory.Exists (this._importJsonPath)) {
							Directory.CreateDirectory (this._importJsonPath);
						}
					}
				}
				return this._importJsonPath;
			}
			protected set { 
				this._importJsonPath = value;
			}
		}

		private string _exportJsonPath = null;
		/// <summary>
		/// 导出路径(Json).
		/// </summary>
		/// <value>导出路径(Json).</value>
		public string ExportJsonPath {
			get {
				if (true == string.IsNullOrEmpty (this._exportJsonPath)) {
					if (false == string.IsNullOrEmpty (this.Path)) {
						this._exportJsonPath = string.Format ("{0}/Json", this.Path);
						if (false == Directory.Exists (this._exportJsonPath)) {
							Directory.CreateDirectory (this._exportJsonPath);
						}
					}
				}
				return this._exportJsonPath;
			}
			protected set { 
				this._exportJsonPath = value;
			}
		}

		/// <summary>
		/// 清空数据.
		/// </summary>
		protected void ClearData() {
			if (default(T2) != this.Data) {
				this.Data.Clear ();
			}
		}

		#region virtual

		/// <summary>
		/// 取得导入路径.
		/// </summary>
		/// <returns>导入路径.</returns>
		public virtual string GetImportPath () {
			return this.ImportJsonPath;
		}

		/// <summary>
		/// 从JSON文件，导入打包配置信息.
		/// </summary>
		/// <returns><c>true</c>, 导入成功, <c>false</c> 导入失败.</returns>
		/// <param name="iForceClear">强制清空.</param>
		public virtual bool ImportFromJsonFile(bool iForceClear = true) {
			return ImportFromJsonFile (this.GetImportPath(), iForceClear);
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
				importDir = this.GetImportPath ();
			} else {
				this.ImportJsonPath = iImportDir;
			}
			bool fileExistFlg = false;
			T2 jsonData = UtilsAsset.ImportDataByDir<T2> (out fileExistFlg, importDir);
			if (jsonData != null) {
				this.ApplyData (jsonData, iForceClear);
				return true;
			} 
			// 文件不存在则，视为导入成功
			if (false == fileExistFlg) {  
				this.Init ();
				return true;
			}
			return false;
		}

		/// <summary>
		/// 取得导出路径.
		/// </summary>
		/// <returns>导出路径.</returns>
		public virtual string GetExportPath () {
			return this.ExportJsonPath;
		}

		/// <summary>
		/// 导出成JSON文件.
		/// </summary>
		/// <returns>导出文件(Json格式).</returns>
		public virtual string ExportToJsonFile() {
			return ExportToJsonFile (this.GetExportPath());
		}

		/// <summary>
		/// 导出成JSON文件.
		/// </summary>
		/// <returns>导出文件(Json格式).</returns>
		/// <param name="iExportDir">导出路径.</param>
		public virtual string ExportToJsonFile(string iExportDir) {
			string exportDir = iExportDir;
			if (true == string.IsNullOrEmpty (exportDir)) {
				exportDir = this.GetExportPath ();
			} else {
				this.ExportJsonPath = iExportDir;
			}
			return UtilsAsset.ExportData<T2> (this.Data, exportDir);
		}

		/// <summary>
		/// 初始化.
		/// </summary>
		/// <param name="iAssetFilePath">Asset文件路径.</param>
		public virtual bool Init(string iAssetFilePath = null) {
			this._path = iAssetFilePath;
			return this.InitAsset ();
		}

		/// <summary>
		/// 刷新.
		/// </summary>
		public virtual void Refresh() { }

		/// <summary>
		/// 清空.
		/// </summary>
		/// <param name="iIsFileDelete">删除数据文件标志位.</param>
		/// <param name="iDirPath">Asset存放目录文件（不指定：当前选定对象所在目录）.</param>
		public virtual void Clear(bool iIsFileDelete = false, string iDirPath = null) {

			// 清空数据
			this.ClearData();

			// 删除数据文件
			if (true == iIsFileDelete) {
				UtilsAsset.DeleteFile<T2> (iDirPath);
			}

			UtilsAsset.SetAssetDirty (this);
		}

		/// <summary>
		/// 初始化数据.
		/// </summary>
		/// <returns><c>true</c>, OK, <c>false</c> NG.</returns>
		public virtual bool InitAsset () { return true; }

		#endregion

		#region abstract

		/// <summary>
		/// 用用数据.
		/// </summary>
		/// <param name="iData">数据.</param>
		/// <param name="iForceClear">强制清空标志位.</param>
		protected abstract void ApplyData (T2 iData, bool iForceClear);

		#endregion

		/// <summary>
		/// 日志输出：消息.
		/// </summary>
		/// <param name="iScript">脚本.</param>
		/// <param name="iFormat">格式.</param>
		/// <param name="iArgs">参数.</param>
		public void Info(string iFormat, params object[] iArgs) {
			#if BUILD_DEBUG
			UtilsLog.Info (this.ClassName, iFormat, iArgs);
			#endif
		}

		/// <summary>
		/// 日志输出：警告.
		/// </summary>
		/// <param name="iScript">脚本.</param>
		/// <param name="iFormat">格式.</param>
		/// <param name="iArgs">参数.</param>
		public void Warning(string iFormat, params object[] iArgs) {
			#if BUILD_DEBUG
			UtilsLog.Warning (this.ClassName, iFormat, iArgs);
			#endif
		}

		/// <summary>
		/// 日志输出：错误.
		/// </summary>
		/// <param name="iScript">脚本.</param>
		/// <param name="iFormat">格式.</param>
		/// <param name="iArgs">参数.</param>
		public void Error(string iFormat, params object[] iArgs) {
			#if BUILD_DEBUG
			UtilsLog.Error (this.ClassName, iFormat, iArgs);
			#endif
		}

		/// <summary>
		/// 日志输出：异常.
		/// </summary>
		/// <param name="iScript">脚本.</param>
		/// <param name="iFormat">格式.</param>
		/// <param name="iArgs">参数.</param>
		public void Exception(string iFormat, params object[] iArgs) {
			#if BUILD_DEBUG
			UtilsLog.Exception (this.ClassName, iFormat, iArgs);
			#endif
		}
	}
}
