using UnityEngine;
using UnityEngine.Events;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Upload;
using AssetBundles;
using Common;
using Common.UI;
using NetWork.Servers;

namespace Download {

	/// <summary>
	/// 下载管理器.
	/// </summary>
	public class DownloadManager : MonoBehaviourExtension {

		/// <summary>
		/// 下载进度条.
		/// </summary>
		[SerializeField]
		public UIProgressBar _downloadBar = null;

		/// <summary>
		/// 完成延迟时间:下载完成后，到加载下一个界面之间的延迟时间（单位：秒）.
		/// </summary>
		[SerializeField]
		public float _completedDelay = 1.0f;

		/// <summary>
		/// 下载事件.
		/// </summary>
		[SerializeField]
		private ProgressEventTrigger _downloadEvents = new ProgressEventTrigger();

		/// <summary>
		/// 下载错误列表.
		/// </summary>
		private List<ErrorDetail> _errors = new List<ErrorDetail> ();
		private static object _downloaderErrorLock = new object ();

		/// <summary>
		/// 最大下载器个数.
		/// </summary>
		private int DownloaderMaxCount { get; set; }

		/// <summary>
		/// 传器个数.
		/// </summary>
		private int DownloaderCount { get; set; }
		private static object _downloaderCountLock = new object ();

		/// <summary>
		/// 下载打断.
		/// </summary>
		/// <value>下载打断.</value>
		public static bool isCanceled { get; set; }

		/// <summary>
		/// 下载状态.
		/// </summary>
		/// <value>下载状态.</value>
		protected string _downloadState { get; private set;}

		/// <summary>
		/// 完成标志位.
		/// </summary>
		private bool _isCompleted = false;

		/// <summary>
		/// 开始标志位.
		/// </summary>
		public bool isStarted  { get; set; }

		/// <summary>
		/// 运行状态.
		/// </summary>
		private TRunState _State = TRunState.OK;

		/// <summary>
		/// 下载队列.
		/// </summary>
		private Queue<DownloaderBase> DownloadQueue = new Queue<DownloaderBase>();

		/// <summary>
		/// Manifest文件上传标志位.
		/// </summary>
		public static bool ManifestUpload = false;

		void Awake() {

			ManifestUpload = UploadList.GetInstance ().ManifestUpload;

			// 设定下载进度条委托
			if (this._downloadBar != null) {
				this._downloadBar.SetDelegate (
					GetDownloadedProgress,
					GetDownloadedState,
					GetDownloadTotalCount,
					GetDownloadedCount);
			}

		}

		/// <summary>
		/// 更新函数.
		/// </summary>
		void Update () {

			// 尚未开始
			if (this.isStarted == false) {
				return;
			}

			// 已经完成
			if (this._isCompleted == true) {
				this.isStarted = false;
				return;
			}

			// 一直等待下载完成
			float progress = this.GetDownloadedProgress ();
			if (progress >= 1.0f) {
				StartCoroutine (DelayToDownloadCompleted(this._completedDelay));
				this._isCompleted = true;
				return;
			}
		}

		#region DownloadBar 

		/// <summary>
		/// 延迟完成下载.
		/// </summary>
		/// <param name="iDelayTime">I delay time.</param>
		private IEnumerator DelayToDownloadCompleted(float iDelayTime) {
			yield return new WaitForSeconds (iDelayTime);
			this.DownloadCompleteNotification (true);
			yield return null;
		}

		private float GetDownloadedProgress() {
			return DownloadList.GetInstance ().GetCompletedProgress ();
		}
		private string GetDownloadedState() {
			return this._downloadState;
		}
		private int GetDownloadTotalCount() {
			return DownloadList.GetInstance ().GetTotalCount ();
		}
		private int GetDownloadedCount() {
			return DownloadList.GetInstance ().GetDownloadedCount ();
		}

		#endregion

		#region Download

		/// <summary>
		/// 下载(Http).
		/// </summary>
		/// <param name="iDownloadPreCheck">下载预检查.</param>
		public void DownloadByHttp(bool iDownloadPreCheck = false) {

			// 打断当前线程并停止所有协同进程
			isCanceled = true;
			this.StopAllCoroutines();

			// 重置开始完成标志位
			this._isCompleted = false;
			this.isStarted = false;

			// 指定下载方式
			ServersConf.GetInstance ().DownloadWay = TDownloadWay.Http;

			// 点击事件
			this.DownloadStart (iDownloadPreCheck);
		}

		/// <summary>
		/// 下载(WWW).
		/// </summary>
		/// <param name="iDownloadPreCheck">下载预检查.</param>
		public void DownloadByWWW(bool iDownloadPreCheck = false) {

			// 打断当前线程并停止所有协同进程
			isCanceled = true;
			this.StopAllCoroutines();

			// 重置开始完成标志位
			this._isCompleted = false;
			this.isStarted = false;

			// 指定下载方式
			ServersConf.GetInstance ().DownloadWay = TDownloadWay.WWW;

			// 点击事件
			this.DownloadStart (iDownloadPreCheck);
		}

		/// <summary>
		/// 取消下载.
		/// </summary>
		public void DownloadCancel() {

			// 重置开始完成标志位
			this._isCompleted = false;
			this.isStarted = false;

			// 打断当前线程并停止所有协同进程
			this.StopAllCoroutines();

			// 确认取消下载
			this.DownloadCancelNotification ();

		}

		/// <summary>
		/// 下载重置.
		/// </summary>
		public void DownloadReset() {

			// 打断当前线程并停止所有协同进程
			isCanceled = true;
			this.StopAllCoroutines();

			// 清空错误列表
			if(this._errors != null) {
				this._errors.Clear ();
			}
			this._State = TRunState.OK;

			this._isCompleted = false;
			this.isStarted = false;

			// 清空上传列表
			UploadList.GetInstance().Clear();

			// 下载列表清空
			DownloadList.GetInstance ().Reset ();

			// 进度条重置
			if (this._downloadBar != null) {
				this._downloadBar.Reset ();
			}

		}

		/// <summary>
		/// 下载开始.
		/// </summary>
		/// <param name="iDownloadPreCheck">下载预检查.</param>
		public void DownloadStart(bool iDownloadPreCheck) {

			// 下载已经完成
			if (true == this._isCompleted) {
				this.isStarted = false;
				return;
			}

			// 下载已经开始
			if (true == this.isStarted) {
				return;
			}

			this.isStarted = true;
			isCanceled = false;
			// 检测&开始下载
			StartCoroutine (DownloadCheck(iDownloadPreCheck));

		}

		#endregion

		/// <summary>
		/// 下载检测.
		/// </summary>
		/// <returns>The check.</returns>
		/// <param name="iDownloadPreCheck">下载预检查.</param>
		private IEnumerator DownloadCheck(bool iDownloadPreCheck) {

			// 下载开始
			this.isStarted = true;

			if (false == iDownloadPreCheck) {
				// 检测下载文件夹
				// 下载根目录
				this.CheckDownloadDirs (ServersConf.DownloadRootDir);
				yield return new WaitForEndOfFrame ();

				// 下载目录
				this.CheckDownloadDirs (ServersConf.DownloadDir);
				yield return new WaitForEndOfFrame ();

				// 下载目录(Normal)
				this.CheckDownloadDirs (ServersConf.DownloadDirOfNormal);
				yield return new WaitForEndOfFrame ();

				// 下载目录(Scenes)
				this.CheckDownloadDirs (ServersConf.DownloadDirOfScenes);
				yield return new WaitForEndOfFrame ();

				// Bundles目录
				this.CheckDownloadDirs (ServersConf.BundlesDir);
				yield return new WaitForEndOfFrame ();

				// Bundles目录(Normal)
				this.CheckDownloadDirs (ServersConf.BundlesDirOfNormal);
				yield return new WaitForEndOfFrame ();

				// Bundles目录(Scene)
				this.CheckDownloadDirs (ServersConf.BundlesDirOfScenes);
				yield return new WaitForEndOfFrame ();

				// 解压缩目录
				this.CheckDownloadDirs (ServersConf.DecompressedDir);
				yield return new WaitForEndOfFrame ();

				// 解压缩目录(Normal)
				this.CheckDownloadDirs (ServersConf.DecompressedDirOfNormal);
				yield return new WaitForEndOfFrame ();

				// 解压缩目录(Scenes)
				this.CheckDownloadDirs (ServersConf.DecompressedDirOfScenes);
				yield return new WaitForEndOfFrame ();
			}

			// 下载Bundle包依赖文件
			if (false == iDownloadPreCheck) {
				yield return DownloadBundlesMap ();
				yield return new WaitForEndOfFrame ();
			}

			if (TRunState.OK == this._State) {
				// 检测下载用上传列表
				if (false == iDownloadPreCheck) {
					yield return DownloadUploadlist ();
					yield return new WaitForEndOfFrame ();
				}
			} else {
				UtilsLog.Error ("DownloadCheck", "Failed:State:{0}", this._State);
			}

			if ((false == iDownloadPreCheck) && (TRunState.OK == this._State)) { 

				// 更新下载文件
				UploadList.GetInstance ().UpdateFromUploadlistFile ();
				yield return new WaitForEndOfFrame ();

				// 更新下载列表
				UploadList.GetInstance ().UpdateLocalInfoForDownload ();
				yield return new WaitForEndOfFrame ();

			}

			if (TRunState.OK == this._State) {

				// 需要下载时
				if (DownloadList.GetInstance ().isDownloadNecessary == true) {
					// 下载前确认提示
					this.DownloadPreConfirmNotification (
						DownloadList.GetInstance ().GetTotalCount (),
						DownloadList.GetInstance ().GetTotalDatasize ());
				} else {
					this.isStarted = false;
					this._isCompleted = false;

					UtilsLog.Info ("DownloadCheck", "There is no download target or all already be downloaded!");
					this.DownloadCompleteNotification (false);
				}
			
			}

			yield return null;

		}

		/// <summary>
		/// 继续下载检测.
		/// </summary>
		/// <returns>The check.</returns>
		private IEnumerator DownloadContinue() {

			// 初始化下载进度条
			this.initDownloadBar();
			yield return new WaitForEndOfFrame();

			// 初始化下载队列
			yield return this.initDownloadQueue();
			yield return new WaitForEndOfFrame();
			if (TRunState.OK == this._State) {
				// 开始下载
				yield return DownloadFiles ();

				if (TRunState.OK != this._State) {
					this.DownloadFailedNotification ();
				}
			} else {

				this.isStarted = false;
				this._isCompleted = false;
				this._errors.Clear ();
			}

			yield return null;
		}

		/// <summary>
		/// 开始下载.
		/// </summary>
		private IEnumerator DownloadFiles() {

			TDownloadWay downloadWay = ServersConf.GetInstance ().DownloadWay;
			while (this.DownloadQueue.Count > 0) {

				// 下载器个数控制
				if (this.DownloaderCount >= this.DownloaderMaxCount) {
					yield return new WaitForSeconds (1.0f);
				}
					
				// 下载出错则停止
				if (TRunState.OK != this._State) {
					UtilsLog.Error ("DownloadFiles", "Download Failed!!! State:{0}", this._State.ToString());
					// 取消现有下载线程
					isCanceled = true;
					yield break;
				}

				DownloaderBase downloader = this.DownloadQueue.Dequeue ();
				if (downloader == null) {
					continue;
				}
				// Bundle文件
				if (TDownloadWay.WWW == downloadWay) {
					yield return downloader.AsynDownLoadTarget ();
				} else {
					downloader.ThreadDownLoadTarget ();
				}
				yield return new WaitForEndOfFrame();

				// 下载出错则停止
				if (TRunState.OK != this._State) {
					UtilsLog.Error ("DownloadFiles", "Download Failed!!! State:{0}", this._State.ToString());
					// 取消现有下载线程
					isCanceled = true;
					yield break;
				}
			}
				
		}

		/// <summary>
		/// 初始化下载队列.
		/// </summary>
		private IEnumerator initDownloadQueue() {
		
			// 初始化清空
			this.DownloadQueue.Clear();

			List<DownloadTargetInfo> targets = DownloadList.GetInstance ().Targets;
			DownloadTargetInfo[] downloadTargets = targets
				.Where (o => (false == o.Downloaded))
				.ToArray ();
			if ((downloadTargets == null) || (downloadTargets.Length <= 0)) {
				this._State = TRunState.NoDownloadTarget;
				UtilsLog.Warning ("initDownloadQueue", "There is no target to download!!!");
			}
			yield return new WaitForEndOfFrame ();

			if (TRunState.OK == this._State) {

				this.DownloaderCount = 0;
				int targetsCount = downloadTargets.Length;
				int maxCount = ServersConf.GetInstance().ThreadMaxCount;
				this.DownloaderMaxCount = (targetsCount > maxCount) ? maxCount : targetsCount;
				// 遍历下载列表，并压进下载队列
				foreach (DownloadTargetInfo loop in downloadTargets) {

					if(loop == null) {
						continue;
					}

					DownloaderBase downloader = CreateDownloader (loop);
					if (downloader != null) {
						this.DownloadQueue.Enqueue (downloader);
					}
				}
			}
		}

		/// <summary>
		/// 创建下载器.
		/// </summary>
		/// <returns>下载器.</returns>
		/// <param name="iTarget">目标信息.</param>
		private DownloaderBase CreateDownloader(string iTargetUrl) {

			DownloaderBase downloader = null;
			TDownloadWay downloadWay = ServersConf.GetInstance ().DownloadWay;
			switch (downloadWay) {

			case TDownloadWay.WWW:
				{
					downloader = WWWDownloader.Create (
						iTargetUrl, OnStartDownload, OnDownloadSuccessedByUrl, OnDownloadFailedByUrl );
				}
				break;
			case TDownloadWay.None:
			case TDownloadWay.Http:
			default:
				{
					downloader = HttpDownloader.Create (
						iTargetUrl, OnStartDownload, OnDownloadSuccessedByUrl, OnDownloadFailedByUrl );
				}
				break;
			}
			return downloader;
		}

		/// <summary>
		/// 创建下载器（Url）.
		/// </summary>
		/// <returns>The downloader by URL.</returns>
		/// <param name="iDownloadUrl">下载Url.</param>
		/// <param name="iOnStart">开始事件委托.</param>
		/// <param name="iOnSuccessed">成功事件委托.</param>
		/// <param name="iOnFailed">失败事件委托.</param>
		/// <param name="iType">下载对象类型.</param>
		public static DownloaderBase CreateDownloaderByUrl(string iUrl, 
			OnStart iOnStart, OnSuccessedByUrl iOnSuccessed, OnFailedByUrl iOnFailed,
			TargetType iType) {
			DownloaderBase downloader = null;
			TDownloadWay downloadWay = ServersConf.GetInstance ().DownloadWay;
			switch (downloadWay) {

			case TDownloadWay.WWW:
				{
					downloader = WWWDownloader.Create (
						iUrl, iOnStart, iOnSuccessed, iOnFailed, iType);
				}
				break;
			case TDownloadWay.None:
			case TDownloadWay.Http:
			default:
				{
					downloader = HttpDownloader.Create (
						iUrl, iOnStart, iOnSuccessed, iOnFailed, iType);
				}
				break;
			}
			return downloader;
		
		}

		/// <summary>
		/// 创建下载器.
		/// </summary>
		/// <returns>下载器.</returns>
		/// <param name="iTarget">目标信息.</param>
		private DownloaderBase CreateDownloader(DownloadTargetInfo iTarget) {
			DownloaderBase downloader = null;
			TDownloadWay downloadWay = ServersConf.GetInstance ().DownloadWay;
			switch (downloadWay) {

			case TDownloadWay.WWW:
				{
					downloader = WWWDownloader.Create (
						iTarget, OnStartDownload, OnDownloadSuccessed, OnDownloadFail );
				}
				break;
			case TDownloadWay.None:
			case TDownloadWay.Http:
			default:
				{
					downloader = HttpDownloader.Create (
						iTarget, OnStartDownload, OnDownloadSuccessed, OnDownloadFail );
				}
				break;
			}
			return downloader;
		}

		/// <summary>
		/// 初始化下载进度条.
		/// </summary>
		private void initDownloadBar() {
			if (this._downloadBar == null) {
				return;
			}
			this._downloadBar.Init ();
		}

		/// <summary>
		/// 检测下载用的目录.
		/// </summary>
		/// <param name="iDir">检测目录.</param>
		private void CheckDownloadDirs(string iDir) {
			if (string.IsNullOrEmpty (iDir) == true) {
				return;
			}
			if (false == Directory.Exists (iDir)) {
				Directory.CreateDirectory (iDir);
				UtilsLog.Info ("CheckDownloadDirs", " Create Dir:{0}", iDir);
				if (false == Directory.Exists (iDir)) {
					UtilsLog.Error ("CheckDownloadDirs", " Create Dir Failed!! Dir:{0}", iDir);
				}
			}
		}

		/// <summary>
		/// 下载Bundle包依赖文件.
		/// </summary>
		private IEnumerator DownloadBundlesMap() {

			string downloadUrl = ServersConf.GetInstance ().GetDownloadUrlOfBundlesMap ();
			DownloaderBase downloader = this.CreateDownloader(downloadUrl);
			if (downloader != null) {
				downloader.ThreadDownLoadByUrl ();
				yield return new WaitUntil (() => (TRunState.Completed == downloader.State));
				downloader.Dispose ();
			}
			yield return null;
		}

		/// <summary>
		/// 下载上传列表.
		/// </summary>
		private IEnumerator DownloadUploadlist() {

			string downloadUrl = ServersConf.GetInstance ().GetDownloadUrlOfUploadList ();
			DownloaderBase downloader = this.CreateDownloader(downloadUrl);
			if (downloader != null) {
				
				downloader.ThreadDownLoadByUrl ();
				yield return new WaitUntil (() => (TRunState.Completed == downloader.State));
				downloader.Dispose ();
			}
			yield return null;
		}

		#region DownloadDelegate

		/// <summary>
		/// 下载前通知事件委托（下载前提示用户的信息）.
		/// </summary>
		private void DownloadPreConfirmNotification(int iTotalCount, long iTotalDataSize) {
			if ((this._downloadEvents != null) && (this._downloadEvents.OnPreConfirmNotification != null)) {
				this._downloadEvents.OnPreConfirmNotification.Invoke (iTotalCount, iTotalDataSize, DownloadPreConfirm);
			}
		}

		/// <summary>
		/// 下载事先确认委托.
		/// </summary>
		/// <param name="iYesOrNo">是否确认下载（true:下载;false:不下载;）.</param>
		private void DownloadPreConfirm(bool iYesOrNo) {
			if (iYesOrNo == false) {
				return;
			}
			// 继续下载
			StartCoroutine (DownloadContinue ());
		}

		/// <summary>
		/// 下载取消确认.
		/// </summary>
		private void DownloadCancelNotification() { 

			if ((this._downloadEvents != null) && (this._downloadEvents.OnCancelNotification != null)) {
				this._downloadEvents.OnCancelNotification.Invoke (DownloadCancelConfirm);
			}

		}

		/// <summary>
		/// 下载取消确认.
		/// </summary>
		/// <param name="iYesOrNo">是否取消下载（true:取消下载;false:不取消下载;）.</param>
		private void DownloadCancelConfirm(bool iYesOrNo) {
			if (iYesOrNo == true) {
				// 清空错误列表
				if(this._errors != null) {
					this._errors.Clear ();
				}
				return;
			}

			// 打断标志位
			isCanceled = true;

			// 导出已下载完成信息
			// 取得导出目录
			string ExportDir = ServersConf.BundlesDir;
			// 导出最新的下载列表信息
			DownloadList.GetInstance().ExportToJsonFile(ExportDir);

			// 继续下载
			StartCoroutine (DownloadContinue ());
		}

		/// <summary>
		/// 下载完成确认.
		/// </summary>
		/// <param name="iIsDidDownloaded">是否下载过.</param>
		private void DownloadCompleteNotification(bool iIsDidDownloaded) {

			if ((this._downloadEvents != null) && (this._downloadEvents.OnCompletedNotification != null)) {
				this._downloadEvents.OnCompletedNotification.Invoke ();
			}
			this._isCompleted = true;

			if (iIsDidDownloaded == true) {

				// 取得导出目录
				string ExportDir = ServersConf.BundlesDir;
				// 导出最新的下载列表信息
				DownloadList.GetInstance ().ExportToJsonFile (ExportDir);

				// 导入Bundle包依赖关系
				BundlesMap.GetInstance ().ExportToJsonFile (ExportDir);

				// 清空下载文件夹
				this.ClearDownloadDir();
			}
		}

		/// <summary>
		/// 下载失败确认.
		/// </summary>
		private void DownloadFailedNotification() {
			if ((this._downloadEvents != null) && (this._downloadEvents.OnErrorsNotification != null)) {
				this._downloadEvents.OnErrorsNotification.Invoke (this._errors);
			}
		}

		#endregion

		/// <summary>
		/// 清空下载文件夹.
		/// </summary>
		private void ClearDownloadDir() {
			string dir = ServersConf.DownloadDir;
			if (false == Directory.Exists (dir)) {
				return;
			}
			string[] files = Directory.GetFiles (dir, "*.*", SearchOption.AllDirectories);
			foreach (string file in files) {
				File.Delete (file);
			}
		}

		#region Download Callback

		/// <summary>
		/// 开始下载.
		/// </summary>
		/// <param name="iState">状态.</param>
		public void OnStartDownload(string iState) {
			if (this._downloadBar == null) {
				return;
			}
			lock (_downloaderCountLock) {
				// 上传信息
				this._downloadState = iState;

				++this.DownloaderCount;
			}
		}

		/// <summary>
		/// 下载失败回调函数.
		/// </summary>
		/// <param name="iDownloadUrl">下载URL.</param>
		/// <param name="iErrors">错误信息.</param>
		public void OnDownloadFailedByUrl(DownloaderBase iDownloader, string iDownloadUrl, List<ErrorDetail> iErrors) {
			
			// 线程安全：添加错误信息至列表
			lock (_downloaderErrorLock) {

				string errsStr = null;
				foreach (ErrorDetail error in iErrors) {
					if (string.IsNullOrEmpty (errsStr) == true) {
						errsStr = string.Format ("Type:{0} State:{1} Detail:{2} Retries:{3}", 
							error.Type.ToString(), error.State.ToString(), error.Detail, error.Retries.ToString());
					} else {
						errsStr = string.Format ("{0} \n Type:{1} State:{2} Detail:{3} Retries:{4}", 
							errsStr, error.Type.ToString(), error.State.ToString(), error.Detail, error.Retries.ToString());
					}
				}

				UtilsLog.Error ("OnDownloadFailedByUrl", "Download Failed!!! DownloadUrl:{0} Detail:{1}", iDownloadUrl, errsStr);

				this._errors.AddRange (iErrors);
				this._State = TRunState.Error;

				if (iDownloader != null) {
					iDownloader.Dispose ();
					GC.Collect ();
				}
			}
		}

		/// <summary>
		/// 下载成功回调函数.
		/// </summary>
		/// <param name="iDownloadUrl">下载URL.</param>
		public void OnDownloadSuccessedByUrl(DownloaderBase iDownloader, string iDownloadUrl) {
			UtilsLog.Info ("OnDownloadSuccessedByUrl", "Download Successed. DownloadUrl:{0}", iDownloadUrl);
		}

		/// <summary>
		/// 下载失败回调函数.
		/// </summary>
		/// <param name="iDownloadInfo">下载信息.</param>
		/// <param name="iIsManifest">Manifest文件标志位.</param>
		/// <param name="iErrors">错误信息.</param>
		public void OnDownloadFail(DownloaderBase iDownloader, DownloadTargetInfo iDownloadInfo, bool iIsManifest, List<ErrorDetail> iErrors) {

			// 线程安全：添加错误信息至列表
			lock (_downloaderErrorLock) {
				string errsStr = null;
				foreach (ErrorDetail error in iErrors) {
					if (string.IsNullOrEmpty (errsStr) == true) {
						errsStr = string.Format ("Type:{0} State:{1} Detail:{2} Retries:{3}", 
							error.Type.ToString(), error.State.ToString(), error.Detail, error.Retries.ToString());
					} else {
						errsStr = string.Format ("{0} \n Type:{1} State:{2} Detail:{3} Retries:{4}", 
							errsStr, error.Type.ToString(), error.State.ToString(), error.Detail, error.Retries.ToString());
					}
				}

				UtilsLog.Error ("OnDownloadFail", "Download Failed!!! {0} \n Detail:{1}",
					iDownloadInfo.toString(), errsStr);

				this._errors.AddRange (iErrors);
				this._State = TRunState.Error;

				if (iDownloader != null) {
					iDownloader.Dispose ();
					GC.Collect ();
				}
			}
		}
		/// <summary>
		/// 下载成功回调函数.
		/// </summary>
		/// <param name="iDownloadInfo">下载信息.</param>
		/// <param name="iRetries">重试次数.</param>
		public void OnDownloadSuccessed(DownloaderBase iDownloader, DownloadTargetInfo iDownloadInfo, int iRetries) {
			UtilsLog.Info ("OnDownloadSuccessed", "Download Successed. {0} Retries:{1}",
				iDownloadInfo.toString(), iRetries);

			DownloadList.GetInstance ().DownloadCompleted (iDownloadInfo.ID, iDownloadInfo.FileType);

			if (true == iDownloadInfo.Downloaded) {

				// 文件拷贝
				iDownloadInfo.CopyTargetWhenDownloadCompleted();

				if (iDownloader != null) {
					iDownloader.Dispose ();
					GC.Collect ();
				}

				lock (_downloaderCountLock) {
					--this.DownloaderCount;
				}
			}
		}

		#endregion
	}
}
