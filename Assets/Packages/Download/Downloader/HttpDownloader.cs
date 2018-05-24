using UnityEngine;
using System;
using System.Net;
using System.IO;
using System.Collections;
using System.Threading;
using BuildSystem;
using Common;
using Common.UI;
using AssetBundles;

namespace Download {

	/// <summary>
	/// 下载器(HttpDownloader:可设置超时，允许断点续载).
	/// </summary>
	public sealed class HttpDownloader : DownloaderBase {

		#region Creator

		/// <summary>
		/// 创建Downloader（Http）.
		/// </summary>
		/// <param name="iDownloadUrl">下载Url.</param>
		/// <param name="iOnStart">开始事件委托.</param>
		/// <param name="iOnSuccessed">成功事件委托.</param>
		/// <param name="iOnFailed">失败事件委托.</param>
		/// <param name="iType">下载对象类型.</param>
		public static HttpDownloader Create(
			string iDownloadUrl, OnStart iOnStart,
			OnSuccessedByUrl iOnSuccessed, OnFailedByUrl iOnFailed,
			TargetType iType = TargetType.Bundle) {
			HttpDownloader downloader = new HttpDownloader ();
			if (downloader != null) {
				downloader.Init (iDownloadUrl, iOnStart, iOnSuccessed, iOnFailed, iType);
				return downloader;
			} else {
				UtilsLog.Error ("HttpDownloader", "Create()::Downloader Create failed!!");
				return null;
			}
		}

		/// <summary>
		/// 创建Downloader.
		/// </summary>
		/// <param name="iTargetInfo">下载目标.</param>
		/// <param name="iOnStart">开始委托回调.</param>
		/// <param name="iOnSuccessed">成功委托回调.</param>
		/// <param name="iOnFailed">失败委托回调.</param>
		/// <param name="iRetries">重下载次数.</param>
		/// <param name="iTimeOut">超时时间（单位：秒）.</param>
		public static HttpDownloader Create(
			DownloadTargetInfo iTargetInfo, OnStart iOnStart,
			OnSuccessed iOnSuccessed, OnFailed iOnFailed) {
			HttpDownloader downloader = new HttpDownloader ();
			if (downloader != null) {
				// 初始化
				downloader.Init (iTargetInfo, iOnStart, iOnSuccessed, iOnFailed);
				return downloader;
			} else {
				UtilsLog.Error ("HttpDownloader", "Create()::Downloader Create failed!!");
				return null;
			}
		}

		#endregion

		#region Implement

		/// <summary>
		/// 异步下载(Url)
		/// </summary>
		/// <param name="iIsFileClean">文件清空标志位.</param>
		public override IEnumerator AsynDownLoadByUrl (bool iIsFileClean = false) {

			this.State = TRunState.OK;

			if ((string.IsNullOrEmpty (this.DownloadBaseUrl) == false) && 
				(string.IsNullOrEmpty (this.FileName) == false)) {

				if (iIsFileClean == true) {
					string fileFullPath = string.Format ("{0}/{1}", this.DownloadDir, this.FileName);
					if (File.Exists (fileFullPath) == true) {
						File.Delete (fileFullPath);
					}
				}

				// 下载开始
				if (this.onStart != null) {
					this.onStart (this.FileName);
				}

				while (this.Retries >= 0) {
					
					// 下载被打断
					if (DownloadManager.isCanceled == true) {
						break;
					}
					// 下载文件
					this.State = this.DownloadFileByHttp (this.DownloadBaseUrl, this.FileName);
					if (TRunState.OK != this.State) {
						--this.Retries;
					} else {
					
						if (this.onSuccessedByUrl != null) {
							this.onSuccessedByUrl (this, 
								string.Format ("{0}/{1}", this.DownloadBaseUrl, this.FileName));
						}
						this.State = TRunState.Completed;
						break;
					}
					yield return new WaitForEndOfFrame();
				}
					
				if ((TRunState.OK != this.State) &&
					(TRunState.Completed != this.State) &&
					(TRunState.Canceled != this.State)) {
					if (this.onFailedByUrl != null) {

						ErrorDetail _detail = new ErrorDetail();
						_detail.State = this.State;
						_detail.Type =  TErrorType.DownloadFailed;
						_detail.Retries = this.Retries;
						_detail.Detail = "AsynDownLoadByUrl Failed";
						this._errors.Add(_detail);

						this.onFailedByUrl (
							this, string.Format ("{0}/{1}", this.DownloadBaseUrl, this.FileName), this._errors);
					}
				}
			}

			yield return null;
		}
			
		/// <summary>
		/// 异步下载目标
		/// </summary>
		public override IEnumerator AsynDownLoadTarget () {
			yield return null;
		}

		/// <summary>
		/// 子线程下载(Url)(断点续传)
		/// </summary>
		public override void ThreadDownLoadByUrl() {

			this.State = TRunState.OK;

			if (threadUrl != null) {
				threadUrl.Abort ();
			}

			if ((string.IsNullOrEmpty (this.DownloadBaseUrl) == false) && 
				(string.IsNullOrEmpty (this.FileName) == false)) {

				// 下载开始
				if (this.onStart != null) {
					this.onStart (this.FileName);
				}

				// 开启子线程下载,使用匿名方法
				threadUrl = new Thread (delegate() {
					while (this.Retries >= 0) {

						// 下载被打断
						if (DownloadManager.isCanceled == true) {
							break;
						}

						// 下载文件
						this.State = this.DownloadFileByHttp (this.DownloadBaseUrl, this.FileName);
						if (TRunState.OK != this.State) {
							--this.Retries;
						} else {

							if (this.onSuccessedByUrl != null) {
								this.onSuccessedByUrl (this, 
									string.Format ("{0}/{1}", this.DownloadBaseUrl, this.FileName));
							}
							// 完成
							this.State = TRunState.Completed;
							break;
						}

					}

					if ((TRunState.OK != this.State) &&
						(TRunState.Completed != this.State) &&
						(TRunState.Canceled != this.State)) {
						if (this.onFailedByUrl != null) {
							
							ErrorDetail _detail = new ErrorDetail();
							_detail.State = this.State;
							_detail.Type =  TErrorType.DownloadFailed;
							_detail.Retries = this.Retries;
							_detail.Detail = "ThreadDownLoadByUrl Failed";
							this._errors.Add(_detail);

							this.onFailedByUrl (this, string.Format ("{0}/{1}", this.DownloadBaseUrl, this.FileName), this._errors);
						}
					}

				});
				
				//开启子线程
				threadUrl.IsBackground = true;
				threadUrl.Start ();
			}
		}

		/// <summary>
		/// 子线程下载目标(断点续传)
		/// </summary>
		public override void ThreadDownLoadTarget() {

			if (this._target == null) {
				return;
			}

			if (thread != null) {
				thread.Abort ();
			}

			if ((string.IsNullOrEmpty (this.DownloadBaseUrl) == false) && 
				(string.IsNullOrEmpty (this.FileName) == false)) {

				// 下载开始
				if (this.onStart != null) {
					this.onStart (this.FileName);
				}

				//开启子线程下载,使用匿名方法
				thread = new Thread (delegate() {

					while (this.Retries >= 0) {

						// 下载被打断
						if (DownloadManager.isCanceled == true) {
							break;
						}

						// 下载文件
						this.State = this.DownloadFileByHttp (this.DownloadBaseUrl, this.FileName);
						if (TRunState.OK != this.State) {
							--this.Retries;
						} else {

							if (this.onSuccessed != null) {
								this.onSuccessed (this, this._target, this.Retries);
							}
							this.State = TRunState.Completed;
							break;
						}

					}

					if ((TRunState.OK != this.State) &&
						(TRunState.Completed != this.State) &&
						(TRunState.Canceled != this.State)) {
						if (this.onFailed != null) {
							
							ErrorDetail _detail = new ErrorDetail();
							_detail.State = this.State;
							_detail.Type =  TErrorType.DownloadFailed;
							_detail.Retries = this.Retries;
							_detail.Detail = "ThreadDownLoadTarget Failed";
							this._errors.Add(_detail);

							this.onFailed (this, this._target, false, this._errors);
						}
					} 
				});


				//开启子线程
				thread.IsBackground = true;
				thread.Start ();
			}
		}

		#endregion
	}
}
