using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Common
{
	/// <summary>
	/// 图片刷新类型.
	/// </summary>
	public enum ImagesRefreshType {
		None = -1,
		/// <summary>
		/// 下载.
		/// </summary>
		Download,
		/// <summary>
		/// 场景加载.
		/// </summary>
		SceneLoad,
		Max
	}

	/// <summary>
	/// 图片自动刷新信息定义.
	/// </summary>
	[System.Serializable]
	public class AutoRefreshImagesInfo : JsonDataBase {
		/// <summary>
		/// 类型.
		/// </summary>
		public ImagesRefreshType Type = ImagesRefreshType.None;

		/// <summary>
		/// 路径列表.
		/// </summary>
		public List<string> Paths = new List<string>();

		/// <summary>
		/// 创建.
		/// </summary>
		/// <param name="iRootDir">根目录.</param>
		/// <param name="iType">类型.</param>
		public static AutoRefreshImagesInfo Create(
			string iRootDir, ImagesRefreshType iType) {
			AutoRefreshImagesInfo objRet = new AutoRefreshImagesInfo ();
			if ((null != objRet) &&
				(false != objRet.Init (iRootDir, iType))) {
				return objRet;
			}
			return null;
		}

		/// <summary>
		/// 初始化.
		/// </summary>
		/// <param name="iRootDir">根目录.</param>
		/// <param name="iType">类型.</param>
		public bool Init(string iRootDir, ImagesRefreshType iType) {
			this.Init ();
			if (true == string.IsNullOrEmpty (iRootDir)) {
				return false;
			}

			if((ImagesRefreshType.None >= iType) ||
				(iType >= ImagesRefreshType.Max)) {
				return false;
			}

			this.Type = iType;

			string _rootDir = string.Format ("{0}{1}", iRootDir, iType.ToString());
			List<FileInfo> files = GetAllFilesFromDir(_rootDir);
			if ((null == files) || (0 >= files.Count)) {
				return false;
			}
			List<string> _files = this.CheckFiles (files);
			if ((null != _files) && (0 < _files.Count)) {
				foreach (string newFile in _files) {
					bool isExist = false;
					foreach (string oldFile in this.Paths) {
						if (false == oldFile.Equals (newFile)) {
							continue;
						}
						isExist = true;
						break;
					}
					if (false == isExist) {
						this.Paths.Add (newFile);
					}
				}
			}
			return true;
		}

		/// <summary>
		/// 刷新.
		/// </summary>
		/// <param name="iRootDir">根目录.</param>
		public void Refresh(string iRootDir) {
			if (false == this.Init (iRootDir, this.Type)) {
				this.Error ("Refresh():Failed!!!(Dir:{0} Type:{1})", 
					iRootDir, this.Type);
				return;
			}
		}

		/// <summary>
		/// 取得所有文件列表.
		/// </summary>
		/// <returns>所有文件列表.</returns>
		/// <param name="iDirPath">文件路径.</param>
		private List<FileInfo> GetAllFilesFromDir(string iDirPath) {


			if(false == Directory.Exists(iDirPath)) {
#if UNITY_EDITOR 

				Directory.CreateDirectory(iDirPath);
#endif
				return null;
			}
				
			List<FileInfo> files = new List<FileInfo> ();
			DirectoryInfo mainDir = new DirectoryInfo (iDirPath);
			if (null == mainDir) {
				return null;
			}
			DirectoryInfo[] subDirs = mainDir.GetDirectories ();
			foreach (DirectoryInfo subDir in subDirs) {
				List<FileInfo> _files = GetAllFilesFromDir (subDir.FullName);
				if (null == _files) {
					continue;
				}
				files.AddRange (_files);
			}
			files.AddRange (mainDir.GetFiles ());
			return files;
		}

		/// <summary>
		/// 取得指定目录文件列表（包含子目录）.
		/// </summary>
		/// <returns>文件列表.</returns>
		/// <param name="iFileInfos">文件信息.</param>
		private List<string> CheckFiles(List<FileInfo> iFileInfos)
		{   
			List<string> filesList = new List<string>();

			foreach (FileInfo fileInfo in iFileInfos) {
				if(string.IsNullOrEmpty(fileInfo.Name)) {
					continue;
				}
				if(fileInfo.Name.EndsWith(".DS_Store") == true) {
					continue;
				}
				if(fileInfo.Name.EndsWith(".meta") == true) {
					continue;
				}
				string _value = this.GetShortFileName(fileInfo.FullName);
				filesList.Add(_value);
			}

			return filesList;
		}

		private string GetShortFileName(string iFullPath) {
			string _temp = iFullPath;
			_temp = _temp.Replace ('\\', '/');

			string _typeName = this.Type.ToString ();

			string strRet = null;
			string[] _temps = _temp.Split ('/');
			bool isStart = false;
			foreach (string loop in _temps) {
				if (false == isStart) {
					if (false == _typeName.Equals (loop)) {
						continue;
					} else {
						isStart = true;
					}
					continue;
				}
				if (true == string.IsNullOrEmpty (strRet)) {
					strRet = loop;
				} else {
					strRet = string.Format ("{0}/{1}", strRet, loop); 
				}
			}

			return strRet;
		}

		/// <summary>
		/// 初始化.
		/// </summary>
		public override void Init() {
			base.Init ();
			Type = ImagesRefreshType.None;
		}

		/// <summary>
		/// 重置.
		/// </summary>
		public override void Reset() {
			base.Reset ();
		}

		/// <summary>
		/// 清空.
		/// </summary>
		public override void Clear() {
			base.Clear ();
			Type = ImagesRefreshType.None;
			Paths.Clear ();
		}
	}

	/// <summary>
	/// 图片自动刷新数据.
	/// </summary>
	[System.Serializable]
	public class AutoRefreshImagesSettingData : JsonDataBase {

		/// <summary>
		/// 目录路径.
		/// </summary>
		public string Root = null;

		/// <summary>
		/// 路径列表.
		/// </summary>
		public List<AutoRefreshImagesInfo> Settings = new List<AutoRefreshImagesInfo>();

		/// <summary>
		/// 初始化.
		/// </summary>
		/// <param name="iRootDir">根目录.</param>
		public bool Init(string iRootDir) {
			this.Init ();
			if(false == Directory.Exists(iRootDir)) {
#if UNITY_EDITOR 
				Directory.CreateDirectory(iRootDir);
#else
				return false;
#endif 
			}
			this.Root = iRootDir;
			return true;
		}

		/// <summary>
		/// 刷新.
		/// </summary>
		/// <param name="iType">类型.</param>
		public void Refresh(ImagesRefreshType iType) {
			AutoRefreshImagesInfo objRet = null;
			foreach (AutoRefreshImagesInfo loop in Settings) {
				if (iType != loop.Type) {
					continue;
				}
				objRet = loop;
				break;
			}
			if (null == objRet) {
				objRet = AutoRefreshImagesInfo.Create (this.Root, iType);
				if (null != objRet) {
					this.Settings.Add (objRet);
				}
			} else {
				objRet.Refresh (this.Root);
			}
		}

		/// <summary>
		/// 初始化.
		/// </summary>
		public override void Init() {
			base.Init ();
			Root = null;
			Settings.Clear ();
		}

		/// <summary>
		/// 重置.
		/// </summary>
		public override void Reset() {
			base.Reset ();
		}

		/// <summary>
		/// 清空.
		/// </summary>
		public override void Clear() {
			base.Clear ();
			Root = null;
			Settings.Clear ();
		}
	}

	/// <summary>
	/// 自动刷新图片设定文件.
	/// </summary>
	[System.Serializable]
	public class AutoRefreshImagesSetting : AssetBase<AutoRefreshImagesSetting, AutoRefreshImagesSettingData> {

		/// <summary>
		/// 根目录.
		/// </summary>
		public static readonly string _rootDir = "Assets/Resources/UI/AutoRefreshImages/";

		/// <summary>
		/// 刷新按钮.
		/// </summary>
		public void Refresh() {
			// 清空
			this.Clear();

			if (false == this.Data.Init (_rootDir)) {
				return;
			}

			// 初始化
			if (true == this.Init ()) {
				this.ExportToJsonFile ();
			}
		}

		/// <summary>
		/// 取得所有自动刷新路片路径列表.
		/// </summary>
		/// <returns>自动刷新路片路径列表.</returns>
		/// <param name="iType">类型.</param>
		public List<string> GetRefreshImagePathsByType(ImagesRefreshType iType) {

			List<string> _tmps = null;
			List<string> _paths = null;
			foreach (AutoRefreshImagesInfo loop in this.Data.Settings) {
				if (iType != loop.Type) {
					continue;
				}
				_tmps = loop.Paths;
				break;
			}
			if ((null == _tmps) || (0 >= _tmps.Count)) {
				return null;
			}
			_paths = new List<string> ();
			foreach (string path in _tmps) {
				string _path = string.Format ("{0}{1}/{2}",
					this.Data.Root, iType.ToString(), path);
				_paths.Add (_path);
			}
			return _paths;
		}
			
#region Implement

		/// <summary>
		/// 初始化数据.
		/// </summary>
		/// <returns><c>true</c>, OK, <c>false</c> NG.</returns>
		public override bool InitAsset () {
			for (int idx = 0; idx < ((int)ImagesRefreshType.Max); ++idx) {
				this.Data.Refresh ((ImagesRefreshType)idx);
			}
			return true;
		}

		/// <summary>
		/// 用用数据.
		/// </summary>
		/// <param name="iData">数据.</param>
		/// <param name="iForceClear">强制清空标志位.</param>
		protected override void ApplyData (AutoRefreshImagesSettingData iData, bool iForceClear) {

			if (null == iData) {
				return;
			}

			// 清空
			if (true == iForceClear) {
				this.Clear ();
			}

			this.Data.Settings = iData.Settings;

			UtilsAsset.SetAssetDirty (this);
		}

#endregion

	}
}
