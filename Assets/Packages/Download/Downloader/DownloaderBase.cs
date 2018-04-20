using UnityEngine;
using System;
using System.Net;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using BuildSystem;
using Upload;
using Common;
using Common.UI;
using AssetBundles;
using NetWork.Servers;

namespace Download {

	/// <summary>
	/// 目标类型.
	/// </summary>
	public enum TargetType {
		/// <summary>
		/// 无.
		/// </summary>
		None,
		/// <summary>
		/// 资源包.
		/// </summary>
		Bundle,
		/// <summary>
		/// 资源包(Normal).
		/// </summary>
		BundleOfNormal,
		/// <summary>
		/// 资源包(Scenes).
		/// </summary>
		BundleOfScenes,
		/// <summary>
		/// 其他.
		/// </summary>
		Else,
		/// <summary>
		/// 默认.
		/// </summary>
		Default
	}

	/// <summary>
	/// 开始下载.
	/// </summary>
	public delegate void OnStart(string iState);

	/// <summary>
	/// 下载失败回调函数.
	/// </summary>
	/// <param name="iDownloader">下载器.</param>
	/// <param name="iTargetInfo">下载目标信息.</param>
	/// <param name="iIsManifest">Manifest文件标志位.</param>
	/// <param name="iErrors">错误信息.</param>
	public delegate void OnFailed(DownloaderBase iDownloader, DownloadTargetInfo iTargetInfo, bool iIsManifest, List<ErrorDetail> iErrors);

	/// <summary>
	/// 下载失败回调函数.
	/// </summary>
	/// <param name="iDownloader">下载器.</param>
	/// <param name="iDownloadUrl">下载URL.</param>
	/// <param name="iErrors">错误信息.</param>
	public delegate void OnFailedByUrl(DownloaderBase iDownloader, string iDownloadUrl, List<ErrorDetail> iErrors);

	/// <summary>
	/// 下载成功回调函数.
	/// </summary>
	/// <param name="iDownloader">下载器.</param>
	/// <param name="iTargetInfo">下载目标信息.</param>
	/// <param name="iRetries">重试次数.</param>
	public delegate void OnSuccessed(DownloaderBase iDownloader, DownloadTargetInfo iTargetInfo, int iRetries);

	/// <summary>
	/// 下载成功回调函数.
	/// </summary>
	/// <param name="iDownloader">下载器.</param>
	/// <param name="iDownloadUrl">下载URL.</param>
	public delegate void OnSuccessedByUrl(DownloaderBase iDownloader, string iDownloadUrl);

	/// <summary>
	/// 下载器接口.
	/// </summary>
	public abstract class DownloaderBase : IDisposable {

		/// <summary>
		/// 下载缓存大小.
		/// </summary>
		protected const long _downloadBufferSize = 1024;

		/// <summary>
		/// 下载进度.
		/// </summary>
		/// <value>下载进度.</value>
		public float progress { get; protected set; }

		/// <summary>
		/// 下载对象类型.
		/// </summary>
		/// <value>对象类型.</value>
		public TargetType TargetType { get; set; }

		/// <summary>
		/// 下载目录.
		/// </summary>
		/// <value>下载目录.</value>
		protected string DownloadDir {
			get { 
				string dir = null;
				switch (this.TargetType) {
				case TargetType.BundleOfNormal:
					{
						dir = ServersConf.DownloadDirOfNormal;
					}
					break;
				case TargetType.BundleOfScenes:
					{
						dir = ServersConf.DownloadDirOfScenes;
					}
					break;
				case TargetType.None:
				case TargetType.Bundle:
				case TargetType.Else:
				default:
					{
						dir = ServersConf.DownloadDir;
					}
					break;
				}
				return dir;
			}
		}

		/// <summary>
		/// 下载情报.
		/// </summary>
		protected DownloadTargetInfo _target { get; private set; }
			
		/// <summary>
		/// 下载Url.
		/// </summary>
		/// <value>The download URL.</value>
		protected string DownloadBaseUrl { get; private set; }

		/// <summary>
		/// Bundle文件名.
		/// </summary>
		/// <value>Bundle文件名.</value>
		protected string FileName { get; private set; }

		/// <summary>
		/// Bundle文件名(全名).
		/// </summary>
		/// <value>Bundle文件名(全名).</value>
		public string FullFileName { get; protected set; }

		/// <summary>
		/// 重下载次数.
		/// </summary>
		/// <value>重下载次数.</value>
		protected int Retries { get; set; }

		/// <summary>
		/// 超时时间.
		/// </summary>
		/// <value>超时时间.</value>
		protected int TimeOut { get; private set; }

		/// <summary>
		/// 运行状态.
		/// </summary>
		public TRunState State = TRunState.OK;

		/// <summary>
		/// 错误一览.
		/// </summary>
		protected List<ErrorDetail> _errors = new List<ErrorDetail> ();
		protected static object _downloaderErrorLock = new object ();

		/// <summary>
		/// 子线程(Url).
		/// </summary>
		protected Thread threadUrl = null;

		/// <summary>
		/// 子线程.
		/// </summary>
		protected Thread thread = null;

		/// <summary>
		/// 子线程(Manifest).
		/// </summary>
		protected Thread manifestThread = null;

		#region Delegate

		/// <summary>
		/// 开始下载.
		/// </summary>
		/// <value>开始下载.</value>
		public OnStart onStart { get; set; }

		/// <summary>
		/// 下载成功回调函数.
		/// </summary>
		/// <value>The callback.</value>
		public OnSuccessed onSuccessed { get; set; }

		/// <summary>
		/// 下载失败回调函数.
		/// </summary>
		/// <value>失败回调函数.</value>
		public OnFailed onFailed { get; set; }

		/// <summary>
		/// 下载成功回调函数(URL).
		/// </summary>
		/// <value>The callback.</value>
		public OnSuccessedByUrl onSuccessedByUrl { get; set; }

		/// <summary>
		/// 下载失败回调函数(URL).
		/// </summary>
		/// <value>失败回调函数.</value>
		public OnFailedByUrl onFailedByUrl { get; set; }

		#endregion

		/// <summary>
		/// 初始化.
		/// </summary>
		/// <param name="iDownloadUrl">下载Url.</param>
		/// <param name="iOnStart">开始事件委托.</param>
		/// <param name="iOnSuccessed">成功事件委托.</param>
		/// <param name="iOnFailed">失败事件委托.</param>
		/// <param name="iType">下载对象类型.</param>
		public void Init(
			string iDownloadUrl, OnStart iOnStart, 
			OnSuccessedByUrl iOnSuccessed, OnFailedByUrl iOnFailed, TargetType iType = TargetType.Bundle) {

			string urlTmp = iDownloadUrl;
			int lastIndex = urlTmp.LastIndexOf ("/");
			this.DownloadBaseUrl = urlTmp.Substring (0, lastIndex);
			this.FileName = urlTmp.Substring (lastIndex + 1);
			int index = this.FileName.IndexOf ("?");
			if (-1 != index) {
				this.FileName = this.FileName.Substring (0, index);
				this.FullFileName = string.Format ("{0}/{1}", this.DownloadDir, this.FileName);
			}

			this.onStart = iOnStart;
			this.onSuccessedByUrl = iOnSuccessed;
			this.onFailedByUrl = iOnFailed;
			this.Retries = ServersConf.GetInstance().NetRetries;
			this.TimeOut = ServersConf.GetInstance().NetTimeOut * 1000;

			this.TargetType = iType;

			if (Directory.Exists (this.DownloadDir) == false) {
				Directory.CreateDirectory (this.DownloadDir);
			}
		}

		/// <summary>
		/// 初始化.
		/// </summary>
		/// <param name="iTarget">下载目标.</param>
		/// <param name="iOnStart">开始事件委托.</param>
		/// <param name="iOnSuccessed">成功事件委托.</param>
		/// <param name="iOnFailed">失败事件委托.</param>
		/// <param name="iLocalSave">本地保存标志位.</param>
		/// <param name="iType">下载对象类型.</param>
		public void Init(
			DownloadTargetInfo iTarget, OnStart iOnStart,
			OnSuccessed iOnSuccessed, OnFailed iOnFailed) {

			this._target = iTarget;
			this.DownloadBaseUrl = ServersConf.GetBundleDownloadBaseURL (iTarget);
			this.FileName = UploadList.GetLocalBundleFileName(iTarget.ID, iTarget.FileType);
			this.FullFileName = string.Format ("{0}/{1}", this.DownloadDir, this.FileName);

			this.onStart = iOnStart;
			this.onSuccessed = iOnSuccessed;
			this.onFailed = iOnFailed;
			this.Retries = ServersConf.GetInstance().NetRetries;
			this.TimeOut = ServersConf.GetInstance ().NetTimeOut * 1000;

			if (TBundleType.Scene != iTarget.BundleType) {
				this.TargetType = TargetType.BundleOfNormal;
			} else {
				this.TargetType = TargetType.BundleOfScenes;
			}

			// 检查目录
			if (Directory.Exists (this.DownloadDir) == false) {
				Directory.CreateDirectory (this.DownloadDir);
			}
		}

		/// <summary>
		/// 重置.
		/// </summary>
		public void Reset() {
			this.Retries = ServersConf.GetInstance().NetRetries;
		}

		#region virtual

		/// <summary>
		/// 获取下载文件的总大小
		/// </summary>
		/// <returns>下载文件的总大小.</returns>
		/// <param name="iDownloadUrl">下载地址.</param>
		/// <param name="iFileSize">文件大小.</param>
		protected TRunState GetFileDataSize(string iDownloadUrl, ref long iFileSize) {

			TRunState ret = TRunState.OK;
			if (null != this._target) {
				iFileSize = Convert.ToInt64(this._target.DataSize);
			} else {
				HttpWebResponse response = null;
				HttpWebRequest request = null;
				try {

					Uri _url = new Uri(iDownloadUrl);
					request = WebRequest.Create (_url) as HttpWebRequest;
					if(null == request) {
						UtilsLog.Error ("DownloaderBase", "GetFileDataSize:HttpWebRequest Create Failed!!!");
					}
					if(TRunState.OK == ret) {
						request.Timeout = this.TimeOut;
						request.ReadWriteTimeout = this.TimeOut;
						request.Method = "HEAD";
						request.ContentType = "application/octet-stream";
						request.KeepAlive = false;

						response = request.GetResponse () as HttpWebResponse;
						if (response.StatusCode != HttpStatusCode.OK) {

							ErrorDetail error = new ErrorDetail ();
							error.Type = TErrorType.HttpError;
							error.State = TRunState.GetFileSizeFromServerFailed;
							error.Detail = string.Format ("GetFileDataSize Failed!!(File:{0} StatusCode:{1})",
								iDownloadUrl, response.StatusCode.ToString ());
							error.Retries = this.Retries;
							iFileSize = -1;

							response.Close();
							response = null;
							return TRunState.GetFileSizeFromServerFailed;

						} else {
							iFileSize = response.ContentLength;
						}
					}
				} catch (Exception exp) {

					UtilsLog.Exception ("DownloaderBase", "GetFileDataSize Type:{0} Message:{1} StackTrace:{2}", 
						exp.GetType().ToString(), exp.Message, exp.StackTrace);

					ErrorDetail error = new ErrorDetail ();
					error.Type = TErrorType.HttpException;
					error.State = TRunState.Exception;
					error.Detail = string.Format ("GetFileDataSize Failed!!(File:{0} exp:{1})",
						iDownloadUrl, exp.Message);
					error.Retries = this.Retries;
					iFileSize = -1;

					ret = TRunState.Exception;

					if (null != request) {
						request.Abort ();
					}

				} finally {
					if (null != response) {
						try
						{
							response.Close();
							response = null;
						}
						catch {
							request.Abort();
						}
					}
				}

				if (0 < iFileSize) {
					UtilsLog.Info ("DownloaderBase", "GetFileDataSize Size:{0} ({1})", 
						iFileSize.ToString (), iDownloadUrl);
				}
			}
			
			return ret;
		}
			
		/// <summary>
		/// 下载文件（Http）.
		/// </summary>
		/// <returns>运行状态.</returns>
		/// <param name="iParentUrl">下载父Url.</param>
		/// <param name="iFileName">下载文件名.</param>
		protected IEnumerator DownloadFileByWWW(string iParentUrl, string iFileName) {

			// 下载被打断
			if (DownloadManager.isCanceled == true) {
				this.State = TRunState.Canceled;
			}

			WWW www = null;
			if (TRunState.OK == this.State) {
				string downloadUrl = string.Format ("{0}/{1}", iParentUrl, iFileName);

				www = new WWW (downloadUrl);
				// 设置下载权限
				www.threadPriority = UnityEngine.ThreadPriority.Normal;

				yield return www;
				if (www.error != null) {

					this.State = TRunState.Error;

					ErrorDetail error = new ErrorDetail ();
					error.Type = TErrorType.WWWError;
					error.State = this.State;
					error.Detail = www.error;
					error.Retries = this.Retries;
				}
			}
			yield return new WaitForEndOfFrame ();

			// 下载被打断
			if (DownloadManager.isCanceled == true) {
				this.State = TRunState.Canceled;
			}

			if ((www != null) && (TRunState.OK == this.State)) {

				string filePath = null;
				try {
					
					// 字节
					byte[] bytes = www.bytes;

					string fileName = Path.GetFileNameWithoutExtension (www.url.Replace ("%20", " "));
					string fileExtension = Path.GetExtension (www.url);
					string[] strUrl = fileExtension.Split ('?');
					filePath = string.Format ("{0}/{1}{2}", 
						this.DownloadDir, fileName, strUrl [0]);
					filePath = filePath.Replace ("%20", " ");
					if (File.Exists (filePath) == true) {
						File.Delete (filePath);
					}

					// 生成并写入文件
					File.WriteAllBytes (filePath, bytes);
#if UNITY_IOS
					UnityEngine.iOS.Device.SetNoBackupFlag(filePath);
#endif
				}  catch (Exception exp) {

					this.State = TRunState.Exception;
					ErrorDetail error = new ErrorDetail();
					error.Type = TErrorType.SysException;
					error.State = this.State;
					error.Detail = string.Format ("System Exception Type:{0} Detail:{0}", 
						exp.GetType ().ToString (), exp.Message);
					error.Retries = this.Retries;
					this._errors.Add(error);
				} finally {
				
					if ((TRunState.OK == this.State) && 
						(this._target != null)) {
						// 检测文件
						if (false == this.CheckFileByCheckMode (this._target, filePath)) {

							// 删除文件
							if (File.Exists (filePath) == true) {
								File.Delete (filePath);
							}

							this.State = TRunState.Error;
							ErrorDetail error = new ErrorDetail ();
							error.Type = TErrorType.FileCheckFailed;
							error.State = this.State;
							error.Detail = string.Format ("File Check Failed!!!");
							error.Retries = this.Retries;
							this._errors.Add (error);

							UtilsLog.Error ("DownloadFileByWWW", "File Check -> NG (File:{0} Retries:{1})", iFileName, this.Retries);
						} else {
							UtilsLog.Info ("DownloadFileByWWW", "File Check -> OK (File:{0} Retries:{1})", iFileName, this.Retries);
						}
					}

				}

			}
			yield return new WaitForEndOfFrame ();
		}

		/// <summary>
		/// 下载文件（Http）.
		/// </summary>
		/// <returns>运行状态.</returns>
		/// <param name="iParentUrl">下载父Url.</param>
		/// <param name="iFileName">下载文件名.</param>
		protected TRunState DownloadFileByHttp(string iParentUrl, string iFileName) {
			HttpWebRequest request = null;
			HttpWebResponse response = null;
			FileStream fs = null;
			Stream stream = null;
			string filePath = null;
			TRunState stateTmp = TRunState.OK;
			try {

				// 取得下载文件名
				string downloadUrl = string.Format("{0}/{1}", iParentUrl, iFileName);
				filePath = string.Format ("{0}/{1}", this.DownloadDir, iFileName);

				// 使用流操作文件
				fs = new FileStream (filePath, FileMode.OpenOrCreate, FileAccess.Write);
				// 获取文件现在的长度
				long fileLength = fs.Length;
				// 获取下载文件的总长度
				long totalLength = -1;

				stateTmp = this.GetFileDataSize (downloadUrl, ref totalLength);
				if (TRunState.OK == stateTmp) {
					// 下载被打断或者取消
					if (DownloadManager.isCanceled == true) {
						stateTmp = TRunState.Canceled;
					}
				}

				// 断点续传
				if (TRunState.OK == stateTmp) {
					// 如果没下载完
					if (fileLength < totalLength) {

						// 断点续传核心，设置本地文件流的起始位置
						fs.Seek (fileLength, SeekOrigin.Begin);

						Uri _url = new Uri(downloadUrl);
						request = WebRequest.Create (_url) as HttpWebRequest;
						if(null == request) {
							UtilsLog.Error ("DownloaderBase", "DownloadFileByHttp:HttpWebRequest Create Failed!!!");
						}
						request.Timeout = this.TimeOut;
						request.ReadWriteTimeout = this.TimeOut;
						request.ContentType = "application/octet-stream";
						request.KeepAlive = false;

						// 断点续传核心，设置远程访问文件流的起始位置
						request.AddRange ((int)fileLength);
						response = request.GetResponse () as HttpWebResponse;
						if (response == null) {
							ErrorDetail error = new ErrorDetail();
							error.Type = TErrorType.HttpError;
							error.State = this.State;
							error.Detail = string.Format ("The HttpWebRequest is null or created failed(Url:{0})", 
								downloadUrl);
							error.Retries = this.Retries;
							this._errors.Add(error);

							stateTmp = TRunState.Error;
						} else {
							if ((HttpStatusCode.OK != response.StatusCode) &&
								(HttpStatusCode.PartialContent != response.StatusCode)) {

								ErrorDetail error = new ErrorDetail();
								error.Type = TErrorType.HttpError;
								error.State = TRunState.Error;
								error.Detail = string.Format ("HttpStatusCode:{0} Detail:{1}",
									response.StatusCode, response.StatusDescription);
								error.Retries = this.Retries;
								this._errors.Add(error);

								stateTmp = TRunState.Error;
								response.Close();
								response = null;
							}
						}

						// 下载被打断或者取消
						if (TRunState.OK == stateTmp) {
							// 下载被打断或者取消
							if (DownloadManager.isCanceled == true) {
								stateTmp = TRunState.Canceled;
							}
						}
		
						if ((TRunState.OK == stateTmp) && (null != response)) {
							
							stream = response.GetResponseStream ();

							// 初始化下载缓存大小
							byte[] buffer = new byte[_downloadBufferSize];

							// 使用流读取内容到buffer中
							// 注意方法返回值代表读取的实际长度
							int length = stream.Read (buffer, 0, buffer.Length);
							while (length > 0) {

								// 下载被打断
								if (DownloadManager.isCanceled == true) {
									stateTmp = TRunState.Canceled;
									break;
								}

								// 将缓存内容再写入本地文件中
								fs.Write (buffer, 0, length);
								fs.Flush();

								// 计算进度
								fileLength += length;
								progress = (float)fileLength / (float)totalLength;
								progress = (progress >= 1.0f) ? 1.0f : progress;

								// 继续读取，直至读取完毕
								length = stream.Read (buffer, 0, buffer.Length);
							}

						} else {
							progress = 1.0f;
						}
					}
				}

			} catch (Exception exp) {
				
				ErrorDetail error = new ErrorDetail();
				error.Type = TErrorType.SysException;
				error.State = TRunState.Exception;
				error.Detail = string.Format ("Type:{0} Msg:{1} StackTrace:{2}", 
					exp.GetType ().ToString (), exp.Message, exp.StackTrace);
				error.Retries = this.Retries;
				this._errors.Add(error);

				stateTmp = TRunState.Exception;

				if (null != request) {
					request.Abort ();
				}

			} finally {

				if (null != response) {
					try
					{
						response.Close();
						response = null;
					}
					catch {
						request.Abort();
					}
				}

				// 关闭&释放文件对象
				if(fs != null) {
					fs.Close ();
					fs.Dispose ();
					fs = null;

					UtilsLog.Info ("DownloaderBase", "DownloadFileByHttp:File Close -> File:{0}", iFileName);
				}
				if(stream != null) {
					stream.Close ();
					stream.Dispose ();
					stream = null;

					UtilsLog.Info ("DownloaderBase", "DownloadFileByHttp:Stream Close -> File:{0}", iFileName);
				}

				if ((TRunState.OK == this.State) && 
				(this._target != null)) {
					// 检测文件
					if(false == this.CheckFileByCheckMode(this._target, filePath)) {

						// 删除文件
						if(File.Exists(filePath) == true) {
							File.Delete (filePath);
						}
							
						stateTmp = TRunState.Error;

						ErrorDetail error = new ErrorDetail();
						error.Type = TErrorType.FileCheckFailed;
						error.State = stateTmp;
						error.Detail = string.Format ("File Check Failed!!!");
						error.Retries = this.Retries;
						this._errors.Add(error);

						UtilsLog.Error ("DownloaderBase", "DownloadFileByHttp:File Check -> NG (File:{0} Retries:{1})", iFileName, this.Retries);
					} else {
						UtilsLog.Info ("DownloaderBase", "DownloadFileByHttp:File Check -> OK (File:{0} Retries:{1})", iFileName, this.Retries);
					}
				}
			}
			return stateTmp;
		}

		#endregion

		#region abstract

		/// <summary>
		/// 异步下载(Url)
		/// </summary>
		/// <param name="iIsFileClean">文件清空标志位.</param>
		public abstract IEnumerator AsynDownLoadByUrl (bool iIsFileClean = false);

		/// <summary>
		/// 异步下载目标
		/// </summary>
		public abstract IEnumerator AsynDownLoadTarget ();

		/// <summary>
		/// 子线程下载(Url)
		/// </summary>
		public abstract void ThreadDownLoadByUrl();

		/// <summary>
		/// 子线程下载目标
		/// </summary>
		public abstract void ThreadDownLoadTarget();

		#endregion

		#region Implement

		/// <summary>
		/// 释放函数.
		/// </summary>
		public void Dispose() {

		}

		#endregion

		#region FileCheck

		/// <summary>
		/// 根据检测模式检测已下载的文件.
		/// </summary>
		/// <returns><c>true</c>, OK, <c>false</c> NG.</returns>
		/// <param name="iTarget">目标信息.</param>
		/// <param name="iDownloadFileFullPath">已经下载到本地的路径.</param>
		private bool CheckFileByCheckMode(DownloadTargetInfo iTarget, string iDownloadFileFullPath) {
		
			if (File.Exists (iDownloadFileFullPath) == false) {
				return false;
			}

			bool isCheckOK = true;
			switch (UploadList.GetInstance ().CheckMode) {
			case TCheckMode.Unity3d_Hash128:
				{
					UtilsLog.Info ("CheckFileByCheckMode", "The Unity3d_Hash128 of check mode has not been supported yet!!!");
					isCheckOK = false;
				}
				break;
			case TCheckMode.Custom_Md5:
				{
					string md5 = UploadList.GetFileMD5 (iDownloadFileFullPath);
					if ((true == string.IsNullOrEmpty (md5)) ||
						(false == md5.Equals (iTarget.CheckCode))) {
						isCheckOK = false;
					}
				}
				break;
			default:
				break;
			}

			return isCheckOK;
		}

		#endregion
	}

}
