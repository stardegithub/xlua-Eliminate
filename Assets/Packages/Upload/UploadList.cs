using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using BuildSystem;
using Common;
using Common.UI;
using AssetBundles;
using Download;
using NetWork.Servers;

namespace Upload {

	/// <summary>
	/// 压缩格式.
	/// </summary>
	public enum TCompressFormat {
		None,
		Zip,
		LZ4
	}

	/// <summary>
	/// 检测模式.
	/// </summary>
	public enum TCheckMode {
		/// <summary>
		/// Unity自带模式(hash128).
		/// </summary>
		Unity3d_Hash128,
		/// <summary>
		/// 自定义模式(Md5).
		/// </summary>
		Custom_Md5
	}

	/// <summary>
	/// 上传文件类型.
	/// </summary>
	public enum TUploadFileType {
		/// <summary>
		/// Manifest(Main).
		/// </summary>
		MainManifest,
		/// <summary>
		/// Manifest(Normal).
		/// </summary>
		NormalManifest,
		/// <summary>
		/// Bundle.
		/// </summary>
		Bundle
	}

	/// <summary>
	/// 上传信息.
	/// </summary>
	[System.Serializable]
	public class UploadItem {

		/// <summary>
		/// bundle no.
		/// </summary>
		public int No = 0;

		/// <summary>
		/// ID.
		/// </summary>
		public string ID = null;

		/// <summary>
		/// bundle类型.
		/// </summary>
		public TBundleType BundleType = TBundleType.Normal;

		/// <summary>
		/// 上传文件类型.
		/// </summary>
		public TUploadFileType FileType = TUploadFileType.Bundle;

		/// <summary>
		/// 数据大小.
		/// </summary>
		public string DataSize = null;

		/// <summary>
		/// 验证码（具体什么类型的验证码，与UnloadList指定的验证模式相关）.
		/// </summary>
		public string CheckCode = null;

		/// <summary>
		/// 已上传标志位.
		/// </summary>
		public bool Uploaded = false;

		/// <summary>
		/// 废弃标志位.
		/// </summary>
		/// <value><c>true</c> 废弃; 不废弃, <c>false</c>.</value>
		public bool Scraped = false;

		public string toString() {
			return string.Format ("UploadItem No:{0} ID:{1} BundleType:{2} FileType:{3} DataSize:{4} \n CheckCode:{5} \n Scraped:{6}",
				No, ID, BundleType, FileType, 
				(true == string.IsNullOrEmpty(DataSize)) ? "0" : DataSize,
				(true == string.IsNullOrEmpty(CheckCode)) ? "-" : CheckCode,
				Scraped);
		}

	}

	/// <summary>
	/// 上传列表数据.
	/// </summary>
	[System.Serializable]
	public class UploadListData : JsonDataBase {

		/// <summary>
		/// 文件后缀名.
		/// </summary>
		public string FileSuffix;

		/// <summary>
		/// 检测模式.
		/// </summary>
		public TCheckMode CheckMode;

		/// <summary>
		/// 平台类型.
		/// </summary>
		/// <value>平台类型.</value>
		public string BuildTarget;

		/// <summary>
		/// App版本号.
		/// </summary>
		public string AppVersion;

		/// <summary>
		/// 中心服务器版本.
		/// </summary>
		public string CenterVersion;

		/// <summary>
		/// 压缩格式.
		/// </summary>
		public TCompressFormat CompressFormat;

		/// <summary>
		/// Manifest上传标志位.
		/// </summary>
		public bool ManifestUpload;

		/// <summary>
		/// 目标列表.
		/// </summary>
		public List<UploadItem> Targets = new List<UploadItem> ();

		/// <summary>
		/// 初始化.
		/// </summary>
		public override void Init() {
			FileSuffix = null;
			CheckMode = TCheckMode.Custom_Md5;
			BuildTarget = null;
			AppVersion = null;
			CenterVersion = null;
			CompressFormat = TCompressFormat.None;
			ManifestUpload = true;
			Targets.Clear ();
		}
			
		/// <summary>
		/// 重置.
		/// </summary>
		public override void Reset() {
			Targets.Clear ();
		}

		/// <summary>
		/// 清空.
		/// </summary>
		public override void Clear() {
			this.Init();
		}
	}

	/// <summary>
	/// 上传列表.
	/// </summary>
	public class UploadList : AssetBase<UploadList, UploadListData> {

		/// <summary>
		/// 文件后缀名.
		/// </summary>
		public string FileSuffix {
			get { 
				if (null != this.Data) {
					return this.Data.FileSuffix;
				}
				return null;
			}
			set {  
				if (null != this.Data) {
					this.Data.FileSuffix = value;
				}
			}
		}

		/// <summary>
		/// 检测模式.
		/// </summary>
		public TCheckMode CheckMode {
			get { 
				if (null != this.Data) {
					return this.Data.CheckMode;
				}
				return TCheckMode.Custom_Md5;
			}
			set {  
				if (null != this.Data) {
					this.Data.CheckMode = value;
				}
			}
		}

		/// <summary>
		/// 平台类型.
		/// </summary>
		/// <value>平台类型.</value>
		public string BuildTarget {
			get { 
				if (null != this.Data) {
					return this.Data.BuildTarget;
				}
				return null;
			}
			set {  
				if (null != this.Data) {
					this.Data.BuildTarget = value;
				}
			}
		}

		/// <summary>
		/// App版本号.
		/// </summary>
		public string AppVersion {
			get { 
				if (null != this.Data) {
					return this.Data.AppVersion;
				}
				return null;
			}
			set {  
				if (null != this.Data) {
					this.Data.AppVersion = value;
				}
			}
		}

		/// <summary>
		/// 中心服务器版本.
		/// </summary>
		public string CenterVersion {
			get { 
				if (null != this.Data) {
					return this.Data.CenterVersion;
				}
				return null;
			}
			set {  
				if (null != this.Data) {
					this.Data.CenterVersion = value;
				}
			}
		}

		/// <summary>
		/// 压缩格式.
		/// </summary>
		public TCompressFormat CompressFormat {
			get { 
				if (null != this.Data) {
					return this.Data.CompressFormat;
				}
				return TCompressFormat.None;
			}
			set {  
				if (null != this.Data) {
					this.Data.CompressFormat = value;
				}
			}
		}

		/// <summary>
		/// Manifest上传标志位.
		/// </summary>
		public bool ManifestUpload {
			get { 
				if (null != this.Data) {
					return this.Data.ManifestUpload;
				}
				return false;
			}
			set {  
				if (null != this.Data) {
					this.Data.ManifestUpload = value;
				}
			}
		}

		/// <summary>
		/// 目标列表.
		/// </summary>
		public List<UploadItem> Targets {
			get { 
				if (null != this.Data) {
					return this.Data.Targets;
				}
				return null;
			}
		}

		/// <summary>
		/// 线程锁.
		/// </summary>
		private static object _targetsThreadLock = new object ();

		/// <summary>
		/// 进度计数器.
		/// </summary>
		private ProgressCounter _progressCounter = new ProgressCounter();

		/// <summary>
		/// AssetBundle打包输出目录.
		/// </summary>
		public string BundlesOutputDir {
			get;
			private set;
		}

		/// <summary>
		/// 一般的Assetbundle的Main Manifest bundle ID(StreamingAssets).
		/// </summary>
		public static string AssetBundleDirNameOfNormal {
			get { 
				return TBundleType.Normal.ToString();
			}	
		}

		/// <summary>
		/// Scene打包成AssetBundle，存放的文件夹名.
		/// </summary>
		public static string AssetBundleDirNameOfScenes {
			get { 
				return TBundleType.Scene.ToString();
			}	
		}

		/// <summary>
		/// 一般的AssetBundle打包输出路径.
		/// </summary>
		public string BundlesOutputDirOfNormal {
			get { 
				string dir = string.Format ("{0}/{1}", this.BundlesOutputDir, AssetBundleDirNameOfNormal);
				if (Directory.Exists (dir) == false) {
					Directory.CreateDirectory (dir);
				}
				return dir;
			}
		}

		/// <summary>
		/// Scenes的AssetBundle打包输出路径
		/// </summary>
		public string BundlesOutputDirOfScene {
			get { 
				string dir = string.Format ("{0}/{1}", this.BundlesOutputDir, AssetBundleDirNameOfScenes);
				if (Directory.Exists (dir) == false) {
					Directory.CreateDirectory (dir);
				}
				return dir;
			}
		}

		/// <summary>
		/// Md5对象.
		/// </summary>
		private static MD5CryptoServiceProvider _md5 = null;

		/// <summary>
		/// 取得BundleNo.
		/// </summary>
		/// <returns>BundleNo.</returns>
		private int GetBundleNo() {
			if ((this.Targets == null) || (this.Targets.Count <= 0)) {
				return 1;
			}
			return (this.Targets.Count + 1);
		}

		/// <summary>
		/// 创建UploadItem.
		/// </summary>
		/// <returns>UploadItem.</returns>
		/// <param name="iTargetId">目标ID.</param>
		/// <param name="iBundleType">Bundle类型.</param>
		/// <param name="iFileType">文件类型.</param>
		private UploadItem CreateUploadItem(
			string iTargetId, TBundleType iBundleType, 
			TUploadFileType iFileType) {
			UploadItem objRet = new UploadItem (); 
			if (objRet != null) {
				objRet.No = this.GetBundleNo ();
				objRet.ID = iTargetId;
				objRet.BundleType = iBundleType;
				objRet.FileType = iFileType;
				objRet.Uploaded = false;
				this.Targets.Add (objRet);
			}
			return objRet;
		}

		/// <summary>
		/// 添加MainManifest对象.
		/// </summary>
		public void AddMainManifestAssetsTarget() {

			string manifestBundleId = AssetBundleDirNameOfNormal;
			if (string.IsNullOrEmpty (manifestBundleId) == true) {
				return;
			}

			string path = GetLocalBundleFilePath(
				manifestBundleId, TUploadFileType.MainManifest, false);
			if (File.Exists (path) == false) {
				return;
			}

			BundleMap bm = new BundleMap ();
			bm.ID = manifestBundleId;
			bm.Type = TBundleType.Normal;
			int index = path.IndexOf (manifestBundleId);
			bm.Path = path.Substring (0, index);

			// 添加对象
			this.AddTarget (bm, TUploadFileType.MainManifest);

		}

		/// <summary>
		/// 添加对象.
		/// </summary>
		/// <param name="iTarget">I target.</param>
		private void AddTarget(UploadItem iTarget) {
			if (iTarget == null) {
				return;
			}
			this.Targets.Add (iTarget);
		}

		/// <summary>
		/// 添加对象.
		/// </summary>
		/// <param name="iTarget">对象.</param>
		/// <param name="iFileType">上传文件类型.</param>
		/// <param name="iHashCode">HashCode(Unity3d打包生成).</param>
		public void AddTarget(
			BundleMap iTarget, TUploadFileType iFileType, string iHashCode = null) {
			if (iTarget == null) {
				return;
			}
			UploadItem _item = null;
			string filePath = GetLocalBundleFilePath (
				iTarget.ID, iFileType, (TBundleType.Scene == iTarget.Type));
			string checkCode = null;

			string dataSize = null;
			if ((false == string.IsNullOrEmpty(filePath)) && 
				(true == File.Exists (filePath))) {
				if (TCheckMode.Unity3d_Hash128 == this.CheckMode) {
					checkCode = iHashCode;
				} else {
					checkCode = GetFileMD5 (filePath);
				}
				FileInfo fileInfo = new FileInfo (filePath);
				dataSize = fileInfo.Length.ToString();

			} else {
				this.Warning ("AddTarget()::Target File is not exist!!!(target:{0})", filePath);
			}

			bool _exist = this.isTargetExist (iTarget.ID, iFileType, out _item);
			if (false == _exist) {
				_item = this.CreateUploadItem (iTarget.ID, iTarget.Type, iFileType);
				_item.CheckCode = checkCode;
				_item.DataSize = dataSize;
			} else {
				if ((false == string.IsNullOrEmpty(checkCode)) && 
					(false == checkCode.Equals (_item.CheckCode))) {
					_item.CheckCode = checkCode;
					_item.DataSize = dataSize;
					_item.Uploaded = false;
				}
			}
			UtilsAsset.SetAssetDirty (this);
		}

		/// <summary>
		/// 取得本地上传用的输入文件名.
		/// </summary>
		/// <returns>上传用的输入文件名.</returns>
		/// <param name="iBundleId">Bundle ID.</param>
		/// <param name="iFileType">文件类型.</param>
		public static string GetLocalBundleFileName(
			string iBundleId, TUploadFileType iFileType) {

			string fileName = iBundleId;
			switch (iFileType) {
			case TUploadFileType.Bundle:
				{
					string fileSuffix = UploadList.GetInstance ().FileSuffix;
					if (string.IsNullOrEmpty (fileSuffix) == false) {
						fileName = string.Format ("{0}.{1}", fileName, fileSuffix);
					}
				}
				break;
			case TUploadFileType.NormalManifest:
				{
					string fileSuffix = UploadList.GetInstance ().FileSuffix;
					if (string.IsNullOrEmpty (fileSuffix) == false) {
						fileName = string.Format ("{0}.{1}", fileName, fileSuffix);
					}
					fileName = string.Format ("{0}.manifest", fileName);
				}
				break;
			case TUploadFileType.MainManifest:
			default:
				{
					
				}
				break;
			}
			return fileName;
		}

		/// <summary>
		/// Gets the local scene bundle file path.
		/// </summary>
		/// <returns>The local scene bundle file path.</returns>
		/// <param name="iBundleId">I bundle identifier.</param>
		public static string GetLocalSceneBundleFilePath(string iBundleId) {
			string fileName = GetLocalBundleFileName(iBundleId, TUploadFileType.Bundle);
			return string.Format ("{0}/{1}", 
				UploadList.GetInstance ().BundlesOutputDirOfScene, fileName);
		}

		/// <summary>
		/// 取得本地上传用的输入文件地址.
		/// </summary>
		/// <returns>上传用的输入文件地址.</returns>
		/// <param name="iBundleId">Bundle ID.</param>
		/// <param name="iFileType">文件类型.</param>
		/// <param name="iIsScene">场景标志位.</param>
		public static string GetLocalBundleFilePath(
			string iBundleId, TUploadFileType iFileType, bool iIsScene) {
		
			string fileName = GetLocalBundleFileName(iBundleId, iFileType);
			if (iIsScene == true) {
				return string.Format ("{0}/{1}", 
					UploadList.GetInstance ().BundlesOutputDirOfScene, fileName);
			} else {
				return string.Format ("{0}/{1}", 
					UploadList.GetInstance ().BundlesOutputDirOfNormal, fileName);
			}

		}

		/// <summary>
		/// 取得Bundle的相对路径（下载/上传用）.
		/// 路径：bundles/<BuildTarget:(iOS/Android)><BuildMode:(Debug/Release/Store)><UploadDatetime>
		/// </summary>
		/// <returns>Bundle的相对路径.</returns>
		/// <param name="iIsScene">场景标志位.</param>
		public static string GetBundleRelativePath(bool iIsScene) {
			if (iIsScene == true) {
				return string.Format ("bundles/{0}/{1}/{2}", 
					UploadList.GetInstance ().BuildTarget,
					UploadList.GetInstance ().AppVersion,
					UploadList.AssetBundleDirNameOfScenes);
			} else {
				return string.Format ("bundles/{0}/{1}/{2}", 
					UploadList.GetInstance ().BuildTarget,
					UploadList.GetInstance ().AppVersion,
					UploadList.AssetBundleDirNameOfNormal);
			}
		}

		/// <summary>
		/// 取得文件的Md5码.
		/// </summary>
		/// <returns>文件的Md5码.</returns>
		/// <param name="iFilePath">文件路径.</param>
		public static string GetFileMD5(string iFilePath)
		{
			if(_md5 == null)
			{
				_md5 = new MD5CryptoServiceProvider();
			}

			if(false == File.Exists(iFilePath))
			{
				UtilsLog.Error("UploadList", "GetFileMD5()::The file is not exist!!!(File:{0})", iFilePath);
				return null;
			}

			FileStream fs = new FileStream(iFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
			byte[] hash = _md5.ComputeHash(fs);
			string strMD5 = System.BitConverter.ToString(hash);
			if (null != fs) {
				fs.Close ();
				fs.Dispose ();
			}

			strMD5 = strMD5.ToLower();
			strMD5 = strMD5.Replace("-", "");

			return strMD5;
		}

		/// <summary>
		/// 更新上传信息.
		/// </summary>
		/// <param name="iDir">指定目录.</param>
		public void UpdateFromUploadlistFile() {

			// 取得导入目录
			string ImportDir = ServersConf.DownloadDir;

			// 导入文件
			this.ImportFromJsonFile(ImportDir, true);
		}

		/// <summary>
		/// 更新本地信息，为下一步下载做准备.
		/// </summary>
		public void UpdateLocalInfoForDownload() {

			// bundles目录
			string bundlesDir = ServersConf.BundlesDir;

			// 导入Bundle包依赖关系
			BundlesMap.GetInstance().ImportFromJsonFile(bundlesDir, true);

			// 导入已经有下载信息
			DownloadList.GetInstance().ImportFromJsonFile(bundlesDir, true);

			// 将已经下载的最新依赖列表文件更新本地依赖列表
			string downloadDir = ServersConf.DownloadDir;
			// 不清空本地
			BundlesMap.GetInstance().ImportFromJsonFile(downloadDir, false);

			// 从最新的上传列表更新下载信息
			// 抽出条件
			// 1）上传完毕
			// 2）没有被废弃
			UploadItem[] targets = this.Targets
				.Where (o => (
					(false == o.Scraped)))
				.OrderBy (o => o.No)
				.ToArray ();

			if ((targets == null) || (targets.Length <= 0)) {
				this.Info("UpdateLocalInfoForDownload()::There is no target to download！！！");
				return;
			} 
			this.Info("UpdateLocalInfoForDownload()::Targets Count:{0}", targets.Length.ToString());

			foreach (UploadItem Loop in targets) {
				DownloadList.GetInstance ().AddTarget (Loop);
			}
			// 初始化进度计数器
			DownloadList.GetInstance ().InitProgressCounter ();
		}

		#region Implement

		/// <summary>
		/// 初始化Asset.
		/// </summary>
		public override bool InitAsset () {
			this.BundlesOutputDir = Application.streamingAssetsPath;

			return base.InitAsset ();
		}

		/// <summary>
		/// 应用数据.
		/// </summary>
		/// <param name="iData">数据.</param>
		/// <param name="iForceClear">强制清空.</param>
		protected override void ApplyData(UploadListData iData, bool iForceClear) {
			if (null == iData) {
				return;
			}

			// 清空
			if (true == iForceClear) {
				this.Clear ();
			}

			this.Data.FileSuffix = iData.FileSuffix;
			this.Data.CheckMode = iData.CheckMode;
			this.Data.AppVersion = iData.AppVersion;
			this.Data.CenterVersion = iData.CenterVersion;
			this.Data.BuildTarget = iData.BuildTarget;
			this.Data.ManifestUpload = iData.ManifestUpload;
			this.Data.CompressFormat = iData.CompressFormat;

			// 添加资源信息
			foreach(UploadItem loop in iData.Targets) {
				this.AddTarget (loop);
			}
			UtilsAsset.SetAssetDirty (this);

		}

		/// <summary>
		/// 清空.
		/// </summary>
		/// <param name="iIsFileDelete">删除数据文件标志位.</param>
		/// <param name="iDirPath">Asset存放目录文件（不指定：当前选定对象所在目录）.</param>
		public override void Clear(bool iIsFileDelete = false, string iDirPath = null) {

			if (this._progressCounter != null) {
				this._progressCounter.Clear ();
			}

			base.Clear (iIsFileDelete, iDirPath);

			// 清空列表
			UtilsAsset.SetAssetDirty (this);
		}

		#endregion


		/// <summary>
		/// 判断目标是否存在.
		/// </summary>
		/// <returns><c>true</c>,存在, <c>false</c> 不存在.</returns>
		/// <param name="iTargetID">目标ID.</param>
		/// <param name="iFileType">文件类型.</param>
		/// <param name="iTarget">目标信息.</param>
		private bool isTargetExist(string iTargetID, TUploadFileType iFileType, out UploadItem iTarget) {

			iTarget = null;

			UploadItem[] targets = this.Targets
				.Where (o => (
					(true == iTargetID.Equals(o.ID)) && 
					(iFileType == o.FileType)))
				.OrderBy (o => o.No)
				.ToArray ();
			if ((targets == null) || (targets.Length <= 0)) {
				return false;
			}
			if (1 != targets.Length) {
				this.Warning("isTargetExist()::There is duplicate id exist in upload list!!!(Bundle ID:{0})", iTargetID);
			}
			iTarget = targets [0];
			return true;
		}


		#region ProgressCounter

		/// <summary>
		/// 是否需要上传.
		/// </summary>
		/// <value><c>true</c> 需要下载; 无需下载, <c>false</c>.</value>
		public bool isUploadNecessary {
			get { 
				if (this._progressCounter == null) {
					return false;
				}
				return (this._progressCounter.TotalDatasize > 0); 
			}
		}

		/// <summary>
		/// 取得完成进度（0.0f~1.0f）.
		/// </summary>
		/// <returns>完成进度（0.0f~1.0f）.</returns>
		public float GetCompletedProgress() {
			if (this._progressCounter == null) {
				return 1.0f;
			}
			return this._progressCounter.Progress;
		}

		/// <summary>
		/// 文件下载总数.
		/// </summary>
		/// <returns>文件下载总数.</returns>
		public int GetTotalCount() { 
			return (_progressCounter == null) ? 0 : _progressCounter.TotalCount; 
		}

		/// <summary>
		/// 数据总大小（单位：byte）.
		/// </summary>
		/// <returns>数据总大小（单位：byte）.</returns>
		public long GetTotalDatasize() { 
			return (_progressCounter == null) ? 0 : _progressCounter.TotalDatasize;
		}

		/// <summary>
		/// 已上传文件总数.
		/// </summary>
		public int GetUploadedCount() { 
			return (_progressCounter == null) ? 0 : _progressCounter.DidCount;
		}



		/// <summary>
		/// 初始化进度计数器.
		/// </summary>
		public void InitProgressCounter() {
			if (this._progressCounter == null) {
				return;
			}
			int totalCount = 0;
			long totalDataSize = 0;
			foreach (UploadItem loop in this.Targets) {
				// 已经废弃
				if(true == loop.Scraped) {
					continue;	
				}
				// 上传完毕
				if(true == loop.Uploaded) {
					continue;	
				}
					
				totalDataSize += ((string.IsNullOrEmpty(loop.DataSize) == true) ? 0 : Convert.ToInt64(loop.DataSize));
				++totalCount;
			}
			this._progressCounter.Init (totalCount, totalDataSize);
		}

		/// <summary>
		/// 上传完成.
		/// </summary>
		/// <param name="iTargetID">目标ID.</param>
		/// <param name="iFileType">文件类型.</param>
		public void UploadCompleted(string iTargetID, TUploadFileType iFileType) {

			lock (_targetsThreadLock) {
				UploadItem uploadItem = null;
				if (true == this.isTargetExist (iTargetID, iFileType, out uploadItem)) {
					if (uploadItem == null) {
						return;
					}
					uploadItem.Uploaded = true;
					if (this._progressCounter != null) {
						this._progressCounter.UpdateCompletedCount ();

						long dataSize = (string.IsNullOrEmpty(uploadItem.DataSize) == true) ? 0 : Convert.ToInt64(uploadItem.DataSize);
						this._progressCounter.UpdateCompletedDataSize (dataSize);
					}
				}
			}
		}

		#endregion
		
	}
}
