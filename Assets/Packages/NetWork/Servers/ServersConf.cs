using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using BuildSystem;
using Common;
using AssetBundles;
using Download;
using Upload;

namespace NetWork.Servers {

	/// <summary>
	/// 服务器ID.
	/// </summary>
	public enum TServerID {
		Invalid = -1,
		/// <summary>
		/// 默认ID.
		/// </summary>
		Default,
		Max
	}

	/// <summary>
	/// 下载方式.
	/// </summary>
	public enum TDownloadWay {
		/// <summary>
		/// 无.
		/// </summary>
		None,
		/// <summary>
		/// WWW.
		/// </summary>
		WWW,
		/// <summary>
		/// Http.
		/// </summary>
		Http
	}

	/// <summary>
	/// ProgressTips.
	/// </summary>
	[System.Serializable]
	public class ProgressTips : JsonDataBase {
		
		/// <summary>
		/// 语言包标志位.
		/// </summary>
		public bool LanguagePakage = false;

		/// <summary>
		/// 更新间隔时间(单位：秒).
		/// </summary>
		public int Interval = 3;

		/// <summary>
		/// 下载中提示信息包.
		/// </summary>
		public List<string> Tips = new List<string> ();

		/// <summary>
		/// 初始化.
		/// </summary>
		public override void Init() {
			LanguagePakage = false;
			Interval = -1;
			if (Tips != null) {
				Tips.Clear ();
			}
		}
			
		/// <summary>
		/// 重置.
		/// </summary>
		public override void Reset() {
			LanguagePakage = false;
			Interval = 3;
			if (Tips != null) {
				Tips.Clear ();
			}
		}

		/// <summary>
		/// 清空.
		/// </summary>
		public override void Clear() {
			this.Init ();
		}
	}

	/// <summary>
	/// 上传服务器信息.
	/// </summary>
	[System.Serializable]
	public class UploadServerInfo : JsonDataBase
	{

		/// <summary>
		/// 服务器ID.
		/// </summary>
		public TServerID ID;

		/// <summary>
		/// 服务器登陆用账户ID/用户名.
		/// </summary>
		public string AccountId;

		/// <summary>
		/// 服务器登陆用密码.
		/// </summary>
		public string Pwd;
		
		/// <summary>
		/// 服务器地址（Ip地址）.
		/// </summary>
		public string IpAddresss;

		/// <summary>
		/// 端口号.
		/// </summary>
		public int PortNo;

		/// <summary>
		/// 上传根目录.
		/// </summary>
		public string RootDir;

		/// <summary>
		/// 服务器禁用标志位.
		/// </summary>
		public bool Disable;

		/// <summary>
		/// 初始化.
		/// </summary>
		public override void Init() {
			ID = TServerID.Default;
			AccountId = null;
			Pwd = null;
			IpAddresss = null;
			PortNo = -1;
			RootDir = null;
			Disable = false;
		}

		/// <summary>
		/// 清空.
		/// </summary>
		public override void Clear() {
			this.Init ();
		}
	}

	/// <summary>
	/// 下载服务器信息.
	/// </summary>
	[System.Serializable]
	public class ServerDirInfo {
		/// <summary>
		/// 服务器ID.
		/// </summary>
		public TServerID ID = TServerID.Default;

		/// <summary>
		/// 文件夹列表.
		/// </summary>
		public List<string> Dirs = new List<string> ();
	}

	/// <summary>
	/// 下载服务器信息.
	/// </summary>
	[System.Serializable]
	public class DownloadServerInfo : JsonDataBase
	{
		/// <summary>
		/// 服务器ID.
		/// </summary>
		public TServerID ID;

		/// <summary>
		/// 下载地址.
		/// </summary>
		public string Url;

		/// <summary>
		/// 服务器禁用标志位.
		/// </summary>
		public bool Disable;

		/// <summary>
		/// 初始化.
		/// </summary>
		public override void Init() {
			ID = TServerID.Default;
			Url = null;
			Disable = false;
		}

		/// <summary>
		/// 清空.
		/// </summary>
		public override void Clear() {
			this.Init ();
		}
	}

	/// <summary>
	/// 服务器设定数据.
	/// </summary>
	[System.Serializable]
	public class ServersConfData : JsonDataBase {

		/// <summary>
		/// 子线程最大数.
		/// </summary>
		public int ThreadMaxCount;

		/// <summary>
		/// 因为网络失败或者错误等。重试次数.
		/// </summary>
		public int NetRetries;

		/// <summary>
		/// 网络超时时间（单位：秒）
		/// </summary>
		public int NetTimeOut;

		/// <summary>
		/// 下载方式.
		/// </summary>
		public TDownloadWay DownloadWay;

		/// <summary>
		/// 跳过下载标志位
		/// 备注：（用于，下载还没准备好，但是又想开发继续进行的时候，用于跳过下载界面）.
		/// </summary>
		public bool SkipDownload;

		/// <summary>
		/// AssetBundles本地备份标志位.
		/// </summary>
		public bool AssetBundlesBackUp;

		/// <summary>
		/// 上传服务器.
		/// </summary>
		public UploadServerInfo UploadServer = new UploadServerInfo ();

		/// <summary>
		/// 下载服务器.
		/// </summary>
		public DownloadServerInfo DownloadServer = new DownloadServerInfo ();

		/// <summary>
		/// ProgressTips.
		/// </summary>
		public ProgressTips ProgressTips = new ProgressTips();

		/// <summary>
		/// 服务器文件列表信息.
		/// </summary>
		public List<ServerDirInfo> ServersDirs = new List<ServerDirInfo>();

		/// <summary>
		/// 初始化.
		/// </summary>
		public override void Init() {
			ThreadMaxCount = -1;
			NetRetries = -1;
			NetTimeOut = -1;
			DownloadWay = TDownloadWay.Http;
			SkipDownload = false;
			AssetBundlesBackUp = false;
			if (null != UploadServer) {
				UploadServer.Init ();
			}
			if (null != DownloadServer) {
				DownloadServer.Init ();
			}
			if (null != ProgressTips) {
				ProgressTips.Init ();
			}
			if (null != ServersDirs) {
				ServersDirs.Clear ();
			}
		}

		/// <summary>
		/// 重置.
		/// </summary>
		public override void Reset() {

			ThreadMaxCount = 3;
			NetRetries = 3;
			NetTimeOut = 30;
			DownloadWay = TDownloadWay.Http;
			SkipDownload = false;
			AssetBundlesBackUp = false;
			if (null != UploadServer) {
				UploadServer.Reset ();
			}
			if (null != DownloadServer) {
				DownloadServer.Reset ();
			}
			if (null != ProgressTips) {
				ProgressTips.Reset ();
			}
			if (null != ServersDirs) {
				ServersDirs.Clear ();
			}
		}

		/// <summary>
		/// 清空.
		/// </summary>
		public override void Clear() {
			this.Init ();
		}
	}

	/// <summary>
	/// 服务器情报.
	/// </summary>
	public class ServersConf : AssetBase<ServersConf, ServersConfData> {

		/// <summary>
		/// 子线程最大数.
		/// </summary>
		public int ThreadMaxCount {
			get { 
				if (null != this.Data) {
					return this.Data.ThreadMaxCount;
				}
				return -1;
			}	
		}

		/// <summary>
		/// 因为网络失败或者错误等。重试次数.
		/// </summary>
		public int NetRetries {
			get { 
				if (null != this.Data) {
					return this.Data.NetRetries;
				}
				return -1;
			}	
		}

		/// <summary>
		/// 网络超时时间（单位：秒）
		/// </summary>
		public int NetTimeOut {
			get { 
				if (null != this.Data) {
					return this.Data.NetTimeOut;
				}
				return -1;
			}	
		}

		/// <summary>
		/// 下载方式.
		/// </summary>
		public TDownloadWay DownloadWay {
			get { 
				if (null != this.Data) {
					return this.Data.DownloadWay;
				}
				return TDownloadWay.None;
			}	
			set { 
				if (null != this.Data) {
					this.Data.DownloadWay = value;
				}
			}
		}

		/// <summary>
		/// 跳过下载标志位
		/// 备注：（用于，下载还没准备好，但是又想开发继续进行的时候，用于跳过下载界面）.
		/// </summary>
		public bool SkipDownload {
			get { 
				if (null != this.Data) {
					return this.Data.SkipDownload;
				}
				return false;
			}		
			set { 
				if (null != this.Data) {
					this.Data.SkipDownload = value;
				}
			}
		}

		/// <summary>
		/// AssetBundles本地备份标志位.
		/// </summary>
		public bool AssetBundlesBackUp {
			get { 
				if (null != this.Data) {
					return this.Data.AssetBundlesBackUp;
				}
				return false;
			}	
		}

		/// <summary>
		/// 上传服务器.
		/// </summary>
		public UploadServerInfo UploadServer {
			get { 
				if (null != this.Data) {
					return this.Data.UploadServer;
				}
				return null;
			}	
		}

		/// <summary>
		/// 下载服务器.
		/// </summary>
		public DownloadServerInfo DownloadServer {
			get { 
				if (null != this.Data) {
					return this.Data.DownloadServer;
				}
				return null;
			}	
		}

		/// <summary>
		/// ProgressTips.
		/// </summary>
		public ProgressTips ProgressTips {
			get { 
				if (null != this.Data) {
					return this.Data.ProgressTips;
				}
				return null;
			}	
		}

		/// <summary>
		/// 服务器文件列表信息.
		/// </summary>
		public List<ServerDirInfo> ServersDirs {
			get { 
				if (null != this.Data) {
					return this.Data.ServersDirs;
				}
				return null;
			}	
		}

		private static object _serversDirsLock = new object();

		/// <summary>
		/// 下载跟目录.
		/// </summary>
		public static string DownloadRootDir {
			get; private set;
		}

		/// <summary>
		/// 下载目录.
		/// </summary>
		public static string DownloadDir {
			get; private set;
		}

		/// <summary>
		/// 下载目录(Normal).
		/// </summary>
		public static string DownloadDirOfNormal {
			get; private set;
		}

		/// <summary>
		/// 下载目录(Scenes).
		/// </summary>
		public static string DownloadDirOfScenes {
			get; private set;
		}

		/// <summary>
		/// Bundles目录.
		/// </summary>
		public static string BundlesDir {
			get; private set;
		}

		/// <summary>
		/// Bundles目录(Normal).
		/// </summary>
		public static string BundlesDirOfNormal {
			get; private set;
		}

		/// <summary>
		/// Bundles目录(Scenes).
		/// </summary>
		public static string BundlesDirOfScenes {
			get; private set;
		}

		/// <summary>
		/// 解压缩目录.
		/// </summary>
		public static string DecompressedDir {
			get; private set;
		}

		/// <summary>
		/// 解压缩目录(Normal).
		/// </summary>
		public static string DecompressedDirOfNormal {
			get; private set;
		}

		/// <summary>
		/// 解压缩目录(Scenes).
		/// </summary>
		public static string DecompressedDirOfScenes {
			get; private set;
		}

		/// <summary>
		/// 判断上传服务器信息是否有效.
		/// </summary>
		/// <returns><c>true</c>, 有效, <c>false</c> 无效.</returns>
		/// <param name="iServer">上传服务器信息.</param>
		private bool isUploadServerValid(UploadServerInfo iServer) {
		
			// 禁用该服务器
			if (iServer.Disable == true) {
				return false;
			}

			// 服务器ID无效
			if ((TServerID.Invalid >= iServer.ID) ||
			   (TServerID.Max <= iServer.ID)) {
				return false;
			}

			return true;
		}

		/// <summary>
		/// 判断下载服务器信息是否有效.
		/// </summary>
		/// <returns><c>true</c>, 有效, <c>false</c> 无效.</returns>
		/// <param name="iServer">下载服务器信息.</param>
		private bool isDownloadServerValid(DownloadServerInfo iServer) {

			// 禁用该服务器
			if (iServer.Disable == true) {
				return false;
			}

			// 服务器ID无效
			if ((TServerID.Invalid >= iServer.ID) ||
				(TServerID.Max <= iServer.ID)) {
				return false;
			}

			return true;
		}

		/// <summary>
		/// 取得下载服务器信息.
		/// </summary>
		/// <returns>下载服务器信息.</returns>
		public UploadServerInfo GetUploadServerInfo() {
			if (isUploadServerValid (this.UploadServer) == false) {
				return null;
			}
			return this.UploadServer;
		}

		/// <summary>
		/// 取得下载服务器信息.
		/// </summary>
		/// <returns>下载服务器信息.</returns>
		public DownloadServerInfo GetDownloadServerInfo() {
			if (isDownloadServerValid (this.DownloadServer) == false) {
				return null;
			}
			return this.DownloadServer;
		}

		/// <summary>
		/// 取得上传服务器基础地址.
		/// 基础地址:<服务器地址>:<端口号>
		/// </summary>
		/// <returns>上传服务器基础地址.</returns>
		/// <param name="iServerInfo">上传服务器信息.</param>
		public static string GetUploadServerPostBaseURL (
			UploadServerInfo iServerInfo) {
			if (-1 == iServerInfo.PortNo) {
				return iServerInfo.IpAddresss;
			} else {
				return string.Format ("{0}:{1}",
					iServerInfo.IpAddresss, iServerInfo.PortNo);
			}
		}

		/// <summary>
		/// 取得下载服务器基础地址.
		/// 基础地址:<服务器地址>:<端口号>
		/// </summary>
		/// <returns>下载服务器基础地址.</returns>
		/// <param name="iServerInfo">下载服务器信息.</param>
		public static string GetDwonloadServerPostBaseURL (
			DownloadServerInfo iServerInfo) {
			if (string.IsNullOrEmpty (iServerInfo.Url) == true) {
				return null;
			} else {
				return iServerInfo.Url;
			}
		}

		/// <summary>
		/// 取得下载服务器传输用URL.
		/// <服务器地址Url>/<工程名（例：NFF）>
		/// </summary>
		/// <returns>下载服务器传输地址.</returns>
		/// <param name="iServerInfo">下载服务器信息.</param>
		private static string GetDownloadServerPostURL(
			DownloadServerInfo iServerInfo) {

			string serverPostUrl = GetDwonloadServerPostBaseURL (iServerInfo);
			string rootDir = GetInstance ().UploadServer.RootDir;
			if (false == string.IsNullOrEmpty (rootDir)) {
				serverPostUrl = string.Format ("{0}/{1}", serverPostUrl, rootDir);
			}
			if (string.IsNullOrEmpty (serverPostUrl) == true) {
				return null;
			} else {
				return string.Format ("{0}/{1}",
					serverPostUrl, BuildInfo.GetInstance ().BuildName);
			}
		}

		#region Upload Info

		/// <summary>
		/// 取得上传服务器传输用URL.
		/// <服务器地址>:<端口号>/<工程名（例：NFF）>
		/// </summary>
		/// <returns>服务器传输地址.</returns>
		/// <param name="iServerInfo">上传服务器信息.</param>
		private static string GetUploadServerPostURL(
			UploadServerInfo iServerInfo) {

			string serverPostUrl = GetUploadServerPostBaseURL (iServerInfo);
			string rootDir = GetInstance ().UploadServer.RootDir;
			if (false == string.IsNullOrEmpty (rootDir)) {
				serverPostUrl = string.Format ("{0}/{1}", serverPostUrl, rootDir);
			}
			serverPostUrl = string.Format ("{0}/{1}",
				serverPostUrl, BuildInfo.GetInstance ().BuildName);

//			 UtilsLog.Info ("GetUploadServerPostURL", "ServerPostURL:{0}", serverPostUrl);

			return serverPostUrl;
		}

		/// <summary>
		/// 取得上传列表文件Base URL.
		/// </summary>
		/// <returns>上传列表文件Base URL.</returns>
		/// <param name="iServerInfo">上传服务器信息.</param>
		public static string GetUploadListBaseUrl(UploadServerInfo iServerInfo) {
			string serverPostUrl = GetUploadBaseURL (iServerInfo);
			
			string uploadBaseUrl = string.Format ("{0}/bundles/{1}/{2}", 
				serverPostUrl,
				UploadList.GetInstance().BuildTarget,
				UploadList.GetInstance().AppVersion);
			return uploadBaseUrl;
		}

		/// <summary>
		/// 取得上传资源用URL(Ftp格式).
		/// <服务器地址>:<端口号></c>/<工程名（例：NFF）>/<上传时间>
		/// </summary>
		/// <returns>上传资源用URL(Ftp格式).</returns>
		/// <param name="iServerInfo">上传服务器信息.</param>
		public static string GetUploadBaseURL(UploadServerInfo iServerInfo) {
			string serverPostUrl = GetUploadServerPostURL (iServerInfo);
			string uploadBaseUrl = string.Format ("ftp://{0}", serverPostUrl);
			// UtilsLog.Info ("GetUploadBaseURL", "UploadBaseUrl:{0}", uploadBaseUrl);
			return uploadBaseUrl;
		}

		/// <summary>
		/// 取得Bundle的上传地址.
		/// 上传地址：<上传资源用URL>/<BuildMode>/<UploadDateTime>
		/// </summary>
		/// <returns>Bundle的上传地址.</returns>
		/// <param name="iServerInfo">上传服务器信息.</param>
		/// <param name="iUploadInfo">上传信息.</param>
		public static string GetBundleUploadBaseURL(
			UploadServerInfo iServerInfo, UploadItem iUploadInfo) {

			string uploadBaseUrl = GetUploadBaseURL (iServerInfo);

			string bundleUploadUrl = string.Format ("{0}/{1}", 
				uploadBaseUrl, 
				UploadList.GetBundleRelativePath((TBundleType.Scene == iUploadInfo.BundleType)));
			
			// UtilsLog.Info ("GetBundleUploadBaseURL", "BundleUploadUrl:{0}", bundleUploadUrl);
			return bundleUploadUrl;
		}

		#endregion

		#region Download Info 

		/// <summary>
		/// 取得下载资源用URL(Ftp格式).
		/// </summary>
		/// <returns>下载资源用URL(Ftp格式).</returns>
		/// <param name="iServerInfo">下载服务器信息.</param>
		public static string GetDownloadBaseURL(
			DownloadServerInfo iServerInfo) {
			string serverPostUrl = GetDownloadServerPostURL (iServerInfo);
			string downloadBaseUrl = string.Format ("http://{0}/bundles", serverPostUrl);

			// BuildTarget
			#if UNITY_ANDROID
				downloadBaseUrl = string.Format ("{0}/Android", downloadBaseUrl);
			#endif

			#if UNITY_IOS || UNITY_IOS
				downloadBaseUrl = string.Format ("{0}/iOS", downloadBaseUrl);
			#endif

			// AppVersion
			downloadBaseUrl = string.Format ("{0}/{1}", downloadBaseUrl, BuildInfo.GetInstance().BuildVersion);

			// UtilsLog.Info ("GetDownloadBaseURL", "DownloadBaseUrl:{0}", downloadBaseUrl);
			return downloadBaseUrl;
		}

		/// <summary>
		/// 取得Bundle的下载地址.
		/// 下载地址：<下载资源用URL>/<BundleID>/<BundleVersion>/<BundleFullName>
		/// </summary>
		/// <returns>Bundle的下载地址.</returns>
		/// <param name="iDownloadInfo">下载信息.</param>
		public static string GetBundleDownloadBaseURL(
			DownloadTargetInfo iDownloadInfo) {

			DownloadServerInfo dlServerInfo = ServersConf.GetInstance ().GetDownloadServerInfo ();
			if (dlServerInfo == null) {
				return null;
			}
			string downloadBaseUrl = GetDownloadBaseURL (dlServerInfo);
			string bundleDownloadUrl = downloadBaseUrl;
			if (TBundleType.Scene == iDownloadInfo.BundleType) {
				bundleDownloadUrl = string.Format ("{0}/{1}", 
					bundleDownloadUrl, UploadList.AssetBundleDirNameOfScenes);
			} else {
				bundleDownloadUrl = string.Format ("{0}/{1}", 
					bundleDownloadUrl, UploadList.AssetBundleDirNameOfNormal);
			}

			// UtilsLog.Info ("GetBundleDownloadBaseURL", "BundleDownloadUrl:{0}", bundleDownloadUrl);
			return bundleDownloadUrl;
		}

		/// <summary>
		/// 取得Bundle包依赖文件下载URL.
		/// </summary>
		/// <returns>Bundle包依赖文件下载URL.</returns>
		public string GetDownloadUrlOfBundlesMap() {

			DownloadServerInfo serverInfo = ServersConf.GetInstance ().GetDownloadServerInfo ();
			string downloadBaseUrl = ServersConf.GetDownloadBaseURL (serverInfo);

			string JsonFileFullPath = UtilsAsset.GetJsonFilePath<BundlesMapData>();
			int lastIndex = JsonFileFullPath.LastIndexOf ("/");
			string JsonFileName = JsonFileFullPath.Substring (lastIndex+1);

			return string.Format ("{0}/{1}", downloadBaseUrl, JsonFileName);
		}

		/// <summary>
		/// 取得上传列表文件的下载URL.
		/// </summary>
		/// <returns>BundlesInfo的下载URL.</returns>
		public string GetDownloadUrlOfUploadList() {

			DownloadServerInfo serverInfo = ServersConf.GetInstance ().GetDownloadServerInfo ();
			string downloadBaseUrl = ServersConf.GetDownloadBaseURL (serverInfo);

			string JsonFileFullPath = UtilsAsset.GetJsonFilePath<UploadListData>();
			int lastIndex = JsonFileFullPath.LastIndexOf ("/");
			string JsonFileName = JsonFileFullPath.Substring (lastIndex+1);

			return string.Format ("{0}/{1}", downloadBaseUrl, JsonFileName);
		}

		#endregion

		#region Dir of Server

		/// <summary>
		/// 在本地检测，指定文件夹是否已经在服务器上创建了
		/// </summary>
		/// <returns><c>true</c>, 已经创建, <c>false</c> 尚未创建.</returns>
		/// <param name="iServerId">服务器ID.</param>
		/// <param name="iDir">目录.</param>
		public bool isDirCreatedOnServerByLocal(TServerID iServerId, string iDir) {

			ServerDirInfo[] servers = this.ServersDirs
				.Where (o => (iServerId == o.ID))
				.ToArray ();
			if ((servers == null) || (servers.Length != 1)) {
				return false;
			}

			string[] dirs = (servers [0]).Dirs.Where (o => (o.Equals (iDir) == true)).ToArray ();
			return ((dirs != null) && (dirs.Length > 0));
		}

		/// <summary>
		/// 添加已经创建目录.
		/// </summary>
		/// <param name="iServerId">服务器ID.</param>
		/// <param name="iDir">I dir.</param>
		public void AddCreatedDir(TServerID iServerId, string iDir) {

			// 线程安全锁
			lock (_serversDirsLock) {

				ServerDirInfo targetServer = null;
				ServerDirInfo[] servers = this.ServersDirs
					.Where (o => (iServerId == o.ID))
					.OrderBy(o => o.ID)
					.ToArray ();
				if ((servers == null) || (servers.Length <= 0)) {
					targetServer = new ServerDirInfo ();
					targetServer.ID = iServerId;
					this.ServersDirs.Add (targetServer);
				} else {
					if (servers.Length > 1) {
						UtilsLog.Error ("AddCreatedDir", "There are multiple id exist!!![ID:{0}]", iServerId);
					}
					targetServer = servers [0];
				}

				string[] dirs = targetServer.Dirs.Where (o => (o.Equals (iDir) == true)).ToArray ();
				if ((dirs == null) || (dirs.Length <= 0)) {
					targetServer.Dirs.Add (iDir);
				}
			}

		}

		#endregion

		#region ProgressTips

		/// <summary>
		/// 随机取得一个ProgressTip.
		/// </summary>
		/// <returns>ProgressTip.</returns>
		public string GetProgressTipByRandom() {

			if ((this.ProgressTips == null) ||
				(this.ProgressTips.Tips == null) ||
				(this.ProgressTips.Tips.Count <= 0)) {
				return null;
			}

			int minValue = 0;
			int maxValue = this.ProgressTips.Tips.Count - 1;
			int value = Random.Range (minValue, maxValue);
			return this.ProgressTips.Tips[value];
		}

		#endregion

		public void ClearCreatedDir() {
			if (this.ServersDirs != null) {
				this.ServersDirs.Clear ();
			}
		}

		#region Implement

		/// <summary>
		/// 初始化Asset.
		/// </summary>
		public override bool InitAsset () {
			
			DownloadRootDir = Application.temporaryCachePath;
			UtilsLog.Info("InitAsset", "DownloadRootDir:{0}", DownloadRootDir);

			// 下载目录
			DownloadDir = string.Format ("{0}/Downloads", DownloadRootDir);
			UtilsLog.Info("InitAsset", "DownloadDir:{0}", DownloadDir);

			// 下载目录(Normal)
			DownloadDirOfNormal = string.Format ("{0}/{1}", DownloadDir, 
				UploadList.AssetBundleDirNameOfNormal);
			UtilsLog.Info("InitAsset", "DownloadDirOfNormal:{0}", DownloadDirOfNormal);

			// 下载目录(Scenes)
			DownloadDirOfScenes = string.Format ("{0}/{1}", DownloadDir, 
				UploadList.AssetBundleDirNameOfScenes);
			UtilsLog.Info("InitAsset", "DownloadDirOfScenes:{0}", DownloadDirOfScenes);

			// Bundles目录
			BundlesDir = string.Format ("{0}/Bundles", Application.persistentDataPath);
			UtilsLog.Info("InitAsset", "BundlesDir:{0}", BundlesDir);

			// Bundles目录(Normal)
			BundlesDirOfNormal = string.Format ("{0}/{1}", BundlesDir, 
				UploadList.AssetBundleDirNameOfNormal);
			UtilsLog.Info("InitAsset", "BundlesDirOfNormal:{0}", BundlesDirOfNormal);

			// Scene
			BundlesDirOfScenes = string.Format ("{0}/{1}", BundlesDir, 
				UploadList.AssetBundleDirNameOfScenes);
			UtilsLog.Info("InitAsset", "BundlesDirOfScenes:{0}", BundlesDirOfScenes);

			// 解压缩
			DecompressedDir = string.Format ("{0}/Decompressed", DownloadRootDir);
			UtilsLog.Info("InitAsset", "DecompressedDir:{0}", DecompressedDir);

			// 解压缩(Normal)
			DecompressedDirOfNormal = string.Format ("{0}/{1}", DecompressedDir, 
				UploadList.AssetBundleDirNameOfNormal);
			UtilsLog.Info("InitAsset", "DecompressedDirOfNormal:{0}", DecompressedDirOfNormal);

			// 解压缩(Scenes)
			DecompressedDirOfScenes = string.Format ("{0}/{1}", DecompressedDir, 
				UploadList.AssetBundleDirNameOfScenes);
			UtilsLog.Info("InitAsset", "DecompressedDirOfScenes:{0}", DecompressedDirOfScenes);

			UtilsAsset.SetAssetDirty (this);

			return base.InitAsset ();
		}

		/// <summary>
		/// 应用数据.
		/// </summary>
		/// <param name="iData">数据.</param>
		/// <param name="iForceClear">强制清空.</param>
		protected override void ApplyData(ServersConfData iData, bool iForceClear) {
			if (null == iData ) {
				return;
			}

			// 清空
			if (true == iForceClear) {
				this.Clear ();
			}
				
			this.Data.ThreadMaxCount = iData.ThreadMaxCount;
			this.Data.NetRetries = iData.NetRetries;
			this.Data.NetTimeOut = iData.NetTimeOut;
			this.Data.UploadServer = iData.UploadServer;
			this.Data.DownloadServer = iData.DownloadServer;
			this.Data.ProgressTips = iData.ProgressTips;
			this.Data.ServersDirs = iData.ServersDirs;

			UtilsAsset.SetAssetDirty (this);

		}

		#endregion
	}
}