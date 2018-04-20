using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using BuildSystem;
using Common;
using Common.UI;
using Upload;
using AssetBundles;
using NetWork.Servers;

namespace Download {

	/// <summary>
	/// 下载目标信息.
	/// </summary>
	[System.Serializable]
	public class DownloadTargetInfo {
		
		/// <summary>
		/// bundle no.
		/// </summary>
		public int No = 0;

		/// <summary>
		/// Bundle ID.
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
		/// 下载完成标志位.
		/// </summary>
		public bool Downloaded = false;

		/// <summary>
		/// 下载完毕拷贝文件.
		/// </summary>
		public void CopyTargetWhenDownloadCompleted() {
			string copyFrom = null;
			string copyTo = null;

			switch (this.BundleType) {
			case TBundleType.Scene:
				{
					copyFrom = ServersConf.DownloadDirOfScenes;
					copyTo = ServersConf.BundlesDirOfScenes;
				}
				break;
			case TBundleType.Normal:
				{
					copyFrom = ServersConf.DownloadDirOfNormal;
					copyTo = ServersConf.BundlesDirOfNormal;
				}
				break;
			default:
				{
					copyFrom = ServersConf.DownloadDir;
					copyTo = ServersConf.BundlesDir;
				}
				break;
			}
			string fileName = UploadList.GetLocalBundleFileName (this.ID, this.FileType);
			copyFrom = string.Format ("{0}/{1}", copyFrom, fileName);
			copyTo = string.Format ("{0}/{1}", copyTo, fileName);

			if (true == File.Exists (copyFrom)) {
				File.Copy (copyFrom, copyTo, true);
				UtilsLog.Info ("CopyTargetWhenDownloadCompleted", "Copy File:{0} -> {1}",
					copyFrom, copyTo);
			}
			if (false == File.Exists (copyTo)) {
				UtilsLog.Error ("CopyTargetWhenDownloadCompleted", "Failed!! FileName:{0} -> {1}", copyFrom, copyTo);
			} 
			File.Delete (copyFrom);
			UtilsLog.Info ("CopyTargetWhenDownloadCompleted", "Delete File -> {0}",
				copyFrom);
		}

		public string toString() {
			return string.Format ("No:{0} ID:{1} BundleType:{2} FileType:{3} DataSize:{4} Downloaded:{5} \n CheckCode:{6}",
				No, ID, BundleType, FileType, DataSize, Downloaded, CheckCode);
		}
	}

	/// <summary>
	/// 下载列表数据.
	/// </summary>
	[System.Serializable]
	public class DownloadListData : JsonDataBase {
		/// <summary>
		/// 检测模式.
		/// </summary>
		public TCheckMode CheckMode = TCheckMode.Custom_Md5;

		/// <summary>
		/// 下载列表.
		/// </summary>
		public List<DownloadTargetInfo> Targets = new List<DownloadTargetInfo>();

		/// <summary>
		/// 初始化.
		/// </summary>
		public override void Init() {
			CheckMode = TCheckMode.Custom_Md5;
			if (null != Targets) {
				Targets.Clear ();
			}
		}

		/// <summary>
		/// 重置.
		/// </summary>
		public override void Reset() {
			this.Init ();
		}

		/// <summary>
		/// 清空.
		/// </summary>
		public override void Clear() {
			this.Init ();
		}
	}

	/// <summary>
	/// 下载列表.
	/// </summary>
	public class DownloadList : AssetBase<DownloadList, DownloadListData> {

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
		}

		/// <summary>
		/// 下载列表.
		/// </summary>
		public List<DownloadTargetInfo> Targets {
			get { 
				if (null != this.Data) {
					return this.Data.Targets;
				}
				return null;
			}
		}

		private static object _targetUpdateLock = new object();

		/// <summary>
		/// 进度计数器.
		/// </summary>
		private ProgressCounter _progressCounter = new ProgressCounter();

		/// <summary>
		/// 添加对象.
		/// </summary>
		/// <param name="iTarget">上传目标.</param>
		public void AddTarget(UploadItem iTarget) {

			DownloadTargetInfo target = null;
			if (false == isTargetExist (iTarget.ID, iTarget.FileType, out target)) {
				target = new DownloadTargetInfo ();
				this.Targets.Add (target);
				target.No = iTarget.No;
				target.ID = iTarget.ID;
				target.BundleType = iTarget.BundleType;
				target.FileType = iTarget.FileType;
				target.DataSize = iTarget.DataSize;
				target.CheckCode = iTarget.CheckCode;
				target.Downloaded = false;
				UtilsLog.Info ("AddTarget", "New:{0}", iTarget.toString ());
			} else {
				// 变更的场合
				if (false == iTarget.CheckCode.Equals (target.CheckCode)) {
					target.DataSize = iTarget.DataSize;
					target.CheckCode = iTarget.CheckCode;
					target.Downloaded = false;
					UtilsLog.Info ("AddTarget", "Changed:{0}", iTarget.toString ());
				}
			}
		}

		/// <summary>
		/// 取得下载对象列表
		/// </summary>
		/// <returns>下载对象列表.</returns>
		public DownloadTargetInfo[] GetDownloadTargets() {
			if ((this.Targets == null) ||
				(this.Targets.Count <= 0)) {
				return null;
			}
			DownloadTargetInfo[] targets = this.Targets
				.OrderBy(o => o.No)
				.ToArray ();
			if ((targets != null) && (targets.Length > 0)) {
				return targets;
			} else {
				return null;
			}
		}
		/// <summary>
		/// 判断目标存不存在.
		/// </summary>
		/// <returns><c>true</c>, 存在, <c>false</c> 不存在.</returns>
		/// <param name="iBundleId">BundleID.</param>
		/// <param name="iTarget">下载目标信息.</param>
		public bool isTargetExist(string iBundleId, out DownloadTargetInfo iTarget) {
			return isTargetExist (iBundleId, TUploadFileType.Bundle, out iTarget);
		}

		/// <summary>
		/// 判断目标存不存在.
		/// </summary>
		/// <returns><c>true</c>, 存在, <c>false</c> 不存在.</returns>
		/// <param name="iBundleId">BundleID.</param>
		/// <param name="iFileType">文件类型.</param>
		/// <param name="iTarget">下载目标信息.</param>
		public bool isTargetExist(string iBundleId, 
			TUploadFileType iFileType, out DownloadTargetInfo iTarget) {
			iTarget = null;
			if (string.IsNullOrEmpty (iBundleId) == true) {
				return false;
			}

			DownloadTargetInfo[] targets = this.Targets
				.Where (o => ( 
					(iBundleId.Equals (o.ID) == true) &&
					(iFileType == o.FileType)))
				.OrderByDescending (o => o.No)
				.ToArray ();
			if ((targets == null) || (targets.Length <= 0)) {
				return false;
			}
			if (1 != targets.Length) {
				UtilsLog.Warning ("isTargetExist", "There is duplicate id exist in download list!!!(Bundle ID:{0} FileType:{1})", 
					iBundleId, iFileType);
			}
			iTarget = targets [0];
			return true;
		}

		/// <summary>
		/// 取得Bundle全路径名.
		/// </summary>
		/// <returns>Bundle全路径名.</returns>
		/// <param name="iBundleId">BundleId.</param>
		/// <param name="iFileType">文件类型.</param>
		public string GetBundleFullPath(
			string iBundleId, TUploadFileType iFileType = TUploadFileType.Bundle) {
			DownloadTargetInfo targetInfo = null;
			if (isTargetExist (iBundleId, iFileType, out targetInfo) == false) {
				UtilsLog.Error ("GetBundleFullPath", "This bundles is not exist!!!({BundleId:{0} FileType:{1})", 
					iBundleId, iFileType);
				return null;
			}
			if (targetInfo == null) {
				return null;
			}
			string fileName = UploadList.GetLocalBundleFileName(iBundleId, targetInfo.FileType);
			if (string.IsNullOrEmpty (fileName) == true) {
				return null;
			}
				
			string fileFullPath = null;
			switch (targetInfo.BundleType) {
			case TBundleType.Normal:
				{
					fileFullPath = string.Format ("{0}/{1}",
						ServersConf.BundlesDirOfNormal,
						fileName);
				}
				break;
			case TBundleType.Scene:
				{
					fileFullPath = string.Format ("{0}/{1}",
						ServersConf.BundlesDirOfScenes,
						fileName);
				}
				break;
			default:
				{
					fileFullPath = string.Format ("{0}/{1}",
						ServersConf.BundlesDir,
						fileName);
				}
				break;
			}
			return fileFullPath;
		}

		/// <summary>
		/// 重置.
		/// </summary>
		public void Reset() {

			// 初始化进度计数器
			this.InitProgressCounter ();

			foreach (DownloadTargetInfo loop in this.Targets) {
				loop.Downloaded = false;
			}
			// 重新导出文件，为了保持一致
			this.ExportToJsonFile (ServersConf.BundlesDir);

			// 清空列表
			UtilsAsset.SetAssetDirty (this);

		}

		#region Implement

		/// <summary>
		/// 应用数据.
		/// </summary>
		/// <param name="iData">数据.</param>
		/// <param name="iForceClear">强制清空.</param>
		protected override void ApplyData(DownloadListData iData, bool iForceClear) {
			if (null == iData) {
				return;
			}
			// 清空
			if (true == iForceClear) {
				this.Clear ();
			}

			// 添加以后信息
			foreach (DownloadTargetInfo loop in iData.Targets) {
				this.Targets.Add (loop);
			}

			UtilsAsset.SetAssetDirty (this);

		}

		/// <summary>
		/// 清空.
		/// </summary>
		/// <param name="iIsFileDelete">删除数据文件标志位.</param>
		/// <param name="iDirPath">Asset存放目录文件（不指定：当前选定对象所在目录）.</param>
		public override void Clear(bool iIsFileDelete = false, string iDirPath = null) {

			this.Info ("Clear():IsFileDelete::{0} DirPath::{1}",
				iIsFileDelete, (true == string.IsNullOrEmpty(iDirPath)) ? "null" : iDirPath);
			if (this._progressCounter != null) {
				this._progressCounter.Clear();
			}

			base.Clear (iIsFileDelete);
			if (false == string.IsNullOrEmpty (iDirPath)) {
				base.Clear (iIsFileDelete, iDirPath);
			}

			// 清空列表
			UtilsAsset.SetAssetDirty (this);

		}

		#endregion

		#region ProgressCounter

		/// <summary>
		/// 是否需要下载.
		/// </summary>
		/// <value><c>true</c> 需要下载; 无需下载, <c>false</c>.</value>
		public bool isDownloadNecessary {
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
		/// 已下载文件总数.
		/// </summary>
		public int GetDownloadedCount() { 
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
			foreach (DownloadTargetInfo loop in this.Targets) {
				// 已经下载
				if (true == loop.Downloaded) {
					continue;
				}

				totalDataSize += ((string.IsNullOrEmpty(loop.DataSize) == true) ? 0 : Convert.ToInt64(loop.DataSize));
				++totalCount;
			}
			this._progressCounter.Init (totalCount, totalDataSize);
		}

		/// <summary>
		/// 下载完成.
		/// </summary>
		/// <param name="iTargetID">目标ID.</param>
		/// <param name="iFileType">文件类型.</param>
		public void DownloadCompleted(string iTargetID, TUploadFileType iFileType) {

			lock (_targetUpdateLock) {
				DownloadTargetInfo downloadInfo = null;
				if (this.isTargetExist (iTargetID, iFileType, out downloadInfo) == true) {
					if (downloadInfo == null) {
						return;
					}
					downloadInfo.Downloaded = true;

					if (this._progressCounter != null) {
						this._progressCounter.UpdateCompletedCount ();

						long dataSize = (string.IsNullOrEmpty(downloadInfo.DataSize) == true) ? 0 : Convert.ToInt64(downloadInfo.DataSize);
						this._progressCounter.UpdateCompletedDataSize (dataSize);

						UtilsLog.Info ("DownloadCompleted", "Count({0}/{1}) DataSize({2}/{3})",
							this._progressCounter.DidCount, this._progressCounter.TotalCount, 
							this._progressCounter.DidDatasize, this._progressCounter.TotalDatasize);
					}
				}
			}
		}

		#endregion
	}
}