using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Upload;
using Download;
using AssetBundles;
using Common;

namespace AssetBundles {

	/// <summary>
	/// Asset bundle 类型.
	/// </summary>
	public enum TAssetBundleType
	{
		/// <summary>
		/// 无.
		/// </summary>
		None,
		/// <summary>
		/// 场景.
		/// </summary>
		Scene,
		/// <summary>
		/// 预制体.
		/// </summary>
		Prefab,
		/// <summary>
		/// 音效.
		/// </summary>
		Audio,
		/// <summary>
		/// 材质.
		/// </summary>
		Mat,
		/// <summary>
		/// 纹理.
		/// </summary>
		Texture,
		/// <summary>
		/// (.asset)文件.
		/// </summary>
		Asset,
		/// <summary>
		/// (.text)文件.
		/// </summary>
		Text,
		/// <summary>
		/// (.json)文件.
		/// </summary>
		Json
	}

	/// <summary>
	/// 资源管理器.
	/// </summary>
	public class AssetBundlesManager : SingletonBase<AssetBundlesManager>, IDisposable {

		#region 文件后缀名

		private static readonly string _FILE_SUNFFIX_SCENE = ".unity";
		private static readonly string _FILE_SUNFFIX_PREFAB = ".prefab";
		private static readonly string _FILE_SUNFFIX_AUDIO_WAV = ".wav";
		private static readonly string _FILE_SUNFFIX_AUDIO_MP3 = ".mp3";
		private static readonly string _FILE_SUNFFIX_MAT = ".mat";
		private static readonly string _FILE_SUNFFIX_TEXTURE_PNG = ".png";
		private static readonly string _FILE_SUNFFIX_ASSET = ".asset";
		private static readonly string _FILE_SUNFFIX_TEXT = ".text";
		private static readonly string _FILE_SUNFFIX_JSON = ".json";

		#endregion

		/// <summary>
		/// 备份目录.
		/// </summary>
		public static readonly string BackUpDir = string.Format("{0}/../BackUp", Application.dataPath);

		/// <summary>
		/// 备份目录(Assets).
		/// </summary>
		public static readonly string BackUpDirOfAssets = string.Format("{0}/Assets", BackUpDir);

		/// <summary>
		/// 备份目录(Bundles).
		/// </summary>
		public static readonly string BackUpDirOfBundles = string.Format("{0}/Bundles", BackUpDir);

		/// <summary>
		/// Bundle包的文件依赖关系列表.
		/// 备注：
		///   key -> 文件路径
		///   vaule -> BundleID
		/// </summary>
		public Dictionary<string, string> _bundlesMap = new Dictionary<string, string>();

		/// <summary>
		/// 场景文件关系列表.
		/// 备注：
		///   key -> 场景名
		///   vaule -> BundleID
		/// </summary>
		public Dictionary<string, string> _scenesMap = new Dictionary<string, string>();

		/// <summary>
		/// 预制体列表.
		/// 备注：
		///   key -> 预制体名
		///   vaule -> BundleID
		/// </summary>
		public Dictionary<string, string> _prefabsMap = new Dictionary<string, string>();

		/// <summary>
		/// 音效列表.
		///   key -> 音效名
		///   vaule -> BundleID
		/// </summary>
		public Dictionary<string, string> _audiosMap = new Dictionary<string, string>();

		/// <summary>
		///	材质列表.
		///   key -> 材质名
		///   vaule -> BundleID
		/// </summary>
		public Dictionary<string, string> _matsMap = new Dictionary<string, string>();

		/// <summary>
		///	纹理列表.
		///   key -> 纹理名
		///   vaule -> BundleID
		/// </summary>
		public Dictionary<string, string> _texturesMap = new Dictionary<string, string>();

		/// <summary>
		///	(.asset)文件列表.
		///   key -> 文件名
		///   vaule -> BundleID
		/// </summary>
		public Dictionary<string, string> _assetFilesMap = new Dictionary<string, string>();

		/// <summary>
		///	(.text)文件列表.
		///   key -> 文件名
		///   vaule -> BundleID
		/// </summary>
		public Dictionary<string, string> _textsMap = new Dictionary<string, string>();

		/// <summary>
		///	(.json)文件列表.
		///   key -> 文件名
		///   vaule -> BundleID
		/// </summary>
		public Dictionary<string, string> _jsonFilesMap = new Dictionary<string, string>();

		/// <summary>
		/// 已经加载bundles列表.
		/// </summary>
		private Dictionary<string, AssetBundle> _bundles = new Dictionary<string, AssetBundle>();

		/// <summary>
		/// 主Bundle（StreamingAssets）.
		/// </summary>
		private AssetBundle _mainBundle = null;

		/// <summary>
		/// 主manifest（StreamingAssets）.
		/// </summary>
		private AssetBundleManifest _mainManifest = null;

		/// <summary>
		/// 释放函数.
		/// </summary>
		public void Dispose() {

			// 释放已有Bundle
			this.ReleaseAllBundle();

			if (_bundlesMap != null) {
				_bundlesMap.Clear ();
			}
			if (_scenesMap != null) {
				_scenesMap.Clear ();
			}
			if (_prefabsMap != null) {
				_prefabsMap.Clear ();
			}
			if (_audiosMap != null) {
				_audiosMap.Clear ();
			}
			if (_matsMap != null) {
				_matsMap.Clear ();
			}
			if (_texturesMap != null) {
				_texturesMap.Clear ();
			}
			if (_assetFilesMap != null) {
				_assetFilesMap.Clear ();
			}
			if (_textsMap != null) {
				_textsMap.Clear ();
			}
			if (_jsonFilesMap != null) {
				_jsonFilesMap.Clear ();
			}

			if (this._mainBundle != null) {
				this._mainBundle.Unload (false);
			}
		}

		/// <summary>
		/// 初始化.
		/// </summary>
		protected override void Init() {
			// 初始化Bundle包文件依赖关系列表
			List<BundleMap> maps = BundlesMap.GetInstance().Maps;
			if ((maps == null) || (maps.Count <= 0)) {
				this.Error ("Init():There is no info in BundlesMap!!!");
				return;
			}
			foreach (BundleMap loop in maps) {
				foreach (string filePath in loop.Targets) {
					
					string Key = this.GetKeyOfMapByFilePath(filePath.ToLower ());

					// Scene文件
					if (filePath.EndsWith (_FILE_SUNFFIX_SCENE) == true) {
						this._scenesMap [Key] = loop.ID;

						// 预制体
					} else if (filePath.EndsWith (_FILE_SUNFFIX_PREFAB) == true) {
						this._prefabsMap [Key] = loop.ID;

						// 音效
					} else if ((filePath.EndsWith (_FILE_SUNFFIX_AUDIO_WAV) == true) || 
						(filePath.EndsWith (_FILE_SUNFFIX_AUDIO_MP3) == true)) {
						this._audiosMap [Key] = loop.ID;

						// 材质
					} else if (filePath.EndsWith (_FILE_SUNFFIX_MAT) == true) {
						this._matsMap [Key] = loop.ID;

						// 纹理
					} else if (filePath.EndsWith (_FILE_SUNFFIX_TEXTURE_PNG) == true) {
						this._texturesMap [Key] = loop.ID;

						// (.asset)文件
					} else if (filePath.EndsWith (_FILE_SUNFFIX_ASSET) == true) {
						this._assetFilesMap [Key] = loop.ID;

						// (.text)文件
					} else if (filePath.EndsWith (_FILE_SUNFFIX_TEXT) == true) {
						this._textsMap [Key] = loop.ID;

						// (.json)文件
					} else if (filePath.EndsWith (_FILE_SUNFFIX_JSON) == true) {
						this._jsonFilesMap [Key] = loop.ID;
					}

					this._bundlesMap [filePath] = loop.ID;
				}
			}
		}

		/// <summary>
		/// 取得Map的Key.
		/// </summary>
		/// <returns>文件名.</returns>
		/// <param name="iFilePath">文件路径.</param>
		private string GetKeyOfMapByFilePath(string iFilePath) {
			int lastIndex = iFilePath.LastIndexOf ("/");
			string fileName = iFilePath.Substring(lastIndex + 1);
			lastIndex = fileName.LastIndexOf (".");
			if (-1 >= lastIndex) {
				return fileName;
			}
			fileName = fileName.Substring(0, lastIndex);
			return fileName;
		}

		#region 异步加载

		/// <summary>
		/// 加载预制体(多个).
		/// </summary>
		/// <param name="iPrefabName">预制体名.</param>
		/// <param name="iLoadCompleted">加载完成.</param>
		public IEnumerator LoadPrefabsAsync(
			string[] iPrefabNames, Action<bool, string, TAssetBundleType, GameObject> iLoadCompleted) {

			if ((iPrefabNames != null) &&
				(iPrefabNames.Length > 0)) {
				foreach (string prefabName in iPrefabNames) {
					yield return this.LoadPrefabAsync(prefabName, iLoadCompleted);
					yield return new WaitForEndOfFrame ();
				}
			}
			yield return null;
		}

		/// <summary>
		/// 加载预制体（单个）.
		/// </summary>
		/// <param name="iPrefabName">预制体名.</param>
		/// <param name="iLoadCompleted">加载完成.</param>
		/// <param name="iLoadFailed">加载失败.</param>
		public IEnumerator LoadPrefabAsync(
			string iPrefabName, 
			Action<bool, string, TAssetBundleType, GameObject> iLoadCompleted,
			Func<string, System.Action<bool, string, TAssetBundleType, GameObject>, IEnumerator> iLoadFailed = null) {

			if (string.IsNullOrEmpty (iPrefabName) == false) {
				yield return this.LoadFromAssetBundleAsync<GameObject> (
					iPrefabName, iLoadCompleted, iLoadFailed, TAssetBundleType.Prefab);
				yield return new WaitForEndOfFrame ();
			}
			yield return null;
		}

		/// <summary>
		/// 加载音效（单个）.
		/// </summary>
		/// <param name="iAudioName">音效名.</param>
		/// <param name="iLoadCompleted">加载完成.</param>
		/// <param name="iLoadFailed">加载失败.</param>
		public IEnumerator LoadAudioAsync(
			string iAudioName, 
			Action<bool, string, TAssetBundleType, AudioClip> iLoadCompleted,
			Func<string, System.Action<bool, string, TAssetBundleType, AudioClip>, IEnumerator> iLoadFailed = null) {

			if (string.IsNullOrEmpty (iAudioName) == false) {
				yield return this.LoadFromAssetBundleAsync<AudioClip> (
					iAudioName, iLoadCompleted, iLoadFailed, TAssetBundleType.Audio);
				yield return new WaitForEndOfFrame ();
			}
			yield return null;
		}

		/// <summary>
		/// 加载音效（多个）.
		/// </summary>
		/// <param name="iAudioNames">音效列表.</param>
		/// <param name="iLoadCompleted">加载完成.</param>
		/// <param name="iLoadFailed">加载失败.</param>
		public IEnumerator LoadAudiosAsync(
			string[] iAudioNames, 
			Action<bool, string, TAssetBundleType, AudioClip> iLoadCompleted,
			Func<string, System.Action<bool, string, TAssetBundleType, AudioClip>, IEnumerator> iLoadFailed = null) {

			if ((iAudioNames != null) &&
				(iAudioNames.Length > 0)) {
				foreach (string AudioName in iAudioNames) {
					yield return this.LoadAudioAsync (AudioName, iLoadCompleted, iLoadFailed);
					yield return new WaitForEndOfFrame ();
				}
			}
			yield return null;
		}

		/// <summary>
		/// 加载材质（单个）.
		/// </summary>
		/// <param name="iMatName">材质名.</param>
		/// <param name="iLoadCompleted">加载完成.</param>
		/// <param name="iLoadFailed">加载失败.</param>
		public IEnumerator LoadMatAsync(
			string iMatName, 
			Action<bool, string, TAssetBundleType, Material> iLoadCompleted,
			Func<string, System.Action<bool, string, TAssetBundleType, Material>, IEnumerator> iLoadFailed = null) {

			if (string.IsNullOrEmpty (iMatName) == false) {
				yield return this.LoadFromAssetBundleAsync<Material> (
					iMatName, iLoadCompleted, iLoadFailed, TAssetBundleType.Mat);
				yield return new WaitForEndOfFrame ();
			}
			yield return null;
		}

		/// <summary>
		/// 加载材质（多个）.
		/// </summary>
		/// <param name="iMatNames">材质列表.</param>
		/// <param name="iLoadCompleted">加载完成.</param>
		/// <param name="iLoadFailed">加载失败.</param>
		public IEnumerator LoadMatsAsync(
			string[] iMatNames, 
			Action<bool, string, TAssetBundleType, Material> iLoadCompleted,
			Func<string, System.Action<bool, string, TAssetBundleType, Material>, IEnumerator> iLoadFailed = null) {

			if ((iMatNames != null) &&
				(iMatNames.Length > 0)) {
				foreach (string MatName in iMatNames) {
					yield return this.LoadMatAsync (MatName, iLoadCompleted, iLoadFailed);
					yield return new WaitForEndOfFrame ();
				}
			}
			yield return null;
		}

		/// <summary>
		/// 加载纹理（单个）.
		/// </summary>
		/// <param name="iTextureName">纹理名.</param>
		/// <param name="iLoadCompleted">加载完成.</param>
		/// <param name="iLoadFailed">加载失败.</param>
		public IEnumerator LoadTextureAsync(
			string iTextureName, 
			Action<bool, string, TAssetBundleType, Texture> iLoadCompleted,
			Func<string, System.Action<bool, string, TAssetBundleType, Texture>, IEnumerator> iLoadFailed = null) {

			if (string.IsNullOrEmpty (iTextureName) == false) {
				yield return this.LoadFromAssetBundleAsync<Texture> (
					iTextureName, iLoadCompleted, iLoadFailed, TAssetBundleType.Texture);
				yield return new WaitForEndOfFrame ();
			}
			yield return null;
		}

		/// <summary>
		/// 加载纹理（多个）.
		/// </summary>
		/// <param name="iTextureName">纹理列表.</param>
		/// <param name="iLoadCompleted">加载完成.</param>
		/// <param name="iLoadFailed">加载失败.</param>
		public IEnumerator LoadTexturesAsync(
			string[] iTextureNames, 
			Action<bool, string, TAssetBundleType, Texture> iLoadCompleted,
			Func<string, System.Action<bool, string, TAssetBundleType, Texture>, IEnumerator> iLoadFailed = null) {

			if ((iTextureNames != null) &&
				(iTextureNames.Length > 0)) {
				foreach (string TextureName in iTextureNames) {
					yield return this.LoadTextureAsync (TextureName, iLoadCompleted, iLoadFailed);
					yield return new WaitForEndOfFrame ();
				}
			}
			yield return null;
		}

		/// <summary>
		/// 加载(.asset)文件（单个）.
		/// </summary>
		/// <param name="iAssetFileName">(.asset)文件名.</param>
		/// <param name="iLoadCompleted">加载完成.</param>
		/// <param name="iLoadFailed">加载失败.</param>
		public IEnumerator LoadAssetFileAsync(
			string iAssetFileName, 
			Action<bool, string, TAssetBundleType, TextAsset> iLoadCompleted,
			Func<string, System.Action<bool, string, TAssetBundleType, TextAsset>, IEnumerator> iLoadFailed = null) {

			if (string.IsNullOrEmpty (iAssetFileName) == false) {
				yield return this.LoadFromAssetBundleAsync<TextAsset> (
					iAssetFileName, iLoadCompleted, iLoadFailed, TAssetBundleType.Asset);
				yield return new WaitForEndOfFrame ();
			}
			yield return null;
		}

		/// <summary>
		/// 加载(.asset)文件（多个）.
		/// </summary>
		/// <param name="iAssetFileNames">(.asset)文件列表.</param>
		/// <param name="iLoadCompleted">加载完成.</param>
		/// <param name="iLoadFailed">加载失败.</param>
		public IEnumerator LoadAssetFilesAsync(
			string[] iAssetFileNames, 
			Action<bool, string, TAssetBundleType, TextAsset> iLoadCompleted,
			Func<string, System.Action<bool, string, TAssetBundleType, TextAsset>, IEnumerator> iLoadFailed = null) {

			if ((iAssetFileNames != null) &&
				(iAssetFileNames.Length > 0)) {
				foreach (string AssetFileName in iAssetFileNames) {
					yield return this.LoadAssetFileAsync (AssetFileName, iLoadCompleted, iLoadFailed);
					yield return new WaitForEndOfFrame ();
				}
			}
			yield return null;
		}

		/// <summary>
		/// 加载(.text)文件（单个）.
		/// </summary>
		/// <param name="iTextFileName">(.text)文件名.</param>
		/// <param name="iLoadCompleted">加载完成.</param>
		/// <param name="iLoadFailed">加载失败.</param>
		public IEnumerator LoadTextFileAsync(
			string iTextFileName, 
			Action<bool, string, TAssetBundleType, TextAsset> iLoadCompleted,
			Func<string, System.Action<bool, string, TAssetBundleType, TextAsset>, IEnumerator> iLoadFailed = null) {

			if (string.IsNullOrEmpty (iTextFileName) == false) {
				yield return this.LoadFromAssetBundleAsync<TextAsset> (
					iTextFileName, iLoadCompleted, iLoadFailed, TAssetBundleType.Text);
				yield return new WaitForEndOfFrame ();
			}
			yield return null;
		}

		/// <summary>
		/// 加载(.text)文件（多个）.
		/// </summary>
		/// <param name="iTextFileNames">(.text)文件列表.</param>
		/// <param name="iLoadCompleted">加载完成.</param>
		/// <param name="iLoadFailed">加载失败.</param>
		public IEnumerator LoadTextFilesAsync(
			string[] iTextFileNames, 
			Action<bool, string, TAssetBundleType, TextAsset> iLoadCompleted,
			Func<string, System.Action<bool, string, TAssetBundleType, TextAsset>, IEnumerator> iLoadFailed = null) {

			if ((iTextFileNames != null) &&
				(iTextFileNames.Length > 0)) {
				foreach (string TextFileName in iTextFileNames) {
					yield return this.LoadTextFileAsync (TextFileName, iLoadCompleted, iLoadFailed);
					yield return new WaitForEndOfFrame ();
				}
			}
			yield return null;
		}

		/// <summary>
		/// 加载(.json)文件（单个）.
		/// </summary>
		/// <param name="iJsonFileName">(.json)文件名.</param>
		/// <param name="iLoadCompleted">加载完成.</param>
		/// <param name="iLoadFailed">加载失败.</param>
		public IEnumerator LoadJsonFileAsync(
			string iJsonFileName, 
			Action<bool, string, TAssetBundleType, TextAsset> iLoadCompleted,
			Func<string, System.Action<bool, string, TAssetBundleType, TextAsset>, IEnumerator> iLoadFailed = null) {

			if (string.IsNullOrEmpty (iJsonFileName) == false) {
				yield return this.LoadFromAssetBundleAsync<TextAsset> (
					iJsonFileName, iLoadCompleted, iLoadFailed, TAssetBundleType.Json);
				yield return new WaitForEndOfFrame ();
			}
			yield return null;
		}

		/// <summary>
		/// 加载(.json)文件（多个）.
		/// </summary>
		/// <param name="iJsonFileNames">(.json)文件列表.</param>
		/// <param name="iLoadCompleted">加载完成.</param>
		/// <param name="iLoadFailed">加载失败.</param>
		public IEnumerator LoadJsonFilesAsync(
			string[] iJsonFileNames, 
			Action<bool, string, TAssetBundleType, TextAsset> iLoadCompleted,
			Func<string, System.Action<bool, string, TAssetBundleType, TextAsset>, IEnumerator> iLoadFailed = null) {

			if ((iJsonFileNames != null) &&
				(iJsonFileNames.Length > 0)) {
				foreach (string JsonFileName in iJsonFileNames) {
					yield return this.LoadJsonFileAsync (JsonFileName, iLoadCompleted, iLoadFailed);
					yield return new WaitForEndOfFrame ();
				}
			}
			yield return null;
		}

		/// <summary>
		/// 异步加载asset bundle(多个).
		/// </summary>
		/// <param name="iFilePaths">文件路径列表.</param>
		/// <param name="iLoadCompleted">加载完成.</param>
		/// <param name="iLoadFailed">加载失败.</param>
		public IEnumerator LoadAssetBundlesAsync(
			string[] iFilePaths, 
			Action<bool, string, TAssetBundleType, UnityEngine.Object> iLoadCompleted,
			Func<string, System.Action<bool, string, TAssetBundleType, UnityEngine.Object>, IEnumerator> iLoadFailed = null) {

			if ((iFilePaths != null) || 
				(iFilePaths.Length <= 0)) {
				yield return null;
			}

			foreach (string filePath in iFilePaths) {
				if (string.IsNullOrEmpty (filePath) == true) {
					continue;
				}
				yield return this.LoadAssetBundleAsync(filePath, iLoadCompleted, iLoadFailed);
				yield return new WaitForEndOfFrame ();
			}
		}

		/// <summary>
		/// 异步加载asset bundle(单个).
		/// </summary>
		/// <param name="iFilePaths">文件路径列表.</param>
		/// <param name="iLoadCompleted">加载完成.</param>
		/// <param name="iLoadFailed">加载失败.</param>
		public IEnumerator LoadAssetBundleAsync(
			string iFilePath, 
			Action<bool, string, TAssetBundleType, UnityEngine.Object> iLoadCompleted,
			Func<string, System.Action<bool, string, TAssetBundleType, UnityEngine.Object>, IEnumerator> iLoadFailed = null) {
			if (string.IsNullOrEmpty (iFilePath) == false) {
				yield return this.LoadFromAssetBundleAsync<UnityEngine.Object> (
					iFilePath, iLoadCompleted, iLoadFailed);
				yield return new WaitForEndOfFrame ();
			}
			yield return null;
		}

		/// <summary>
		/// 异步加载asset bundle(单个).(add by zhan)
		/// </summary>
		/// <param name="iFilePaths">文件路径列表.</param>
		/// <param name="iLoadSuccess">加载成功.</param>
		/// <param name="iLoadFailed">加载失败.</param>
		public IEnumerator LoadAssetBundleAsync(
			string iFilePath, Type type, 
			Action<string, TAssetBundleType, UnityEngine.Object> iLoadSuccess){
			if (string.IsNullOrEmpty (iFilePath) == false) {
				yield return this.LoadFromAssetBundleAsync (iFilePath,type, iLoadSuccess);
				yield return new WaitForEndOfFrame ();
			}
			yield return null;
		}

		/// <summary>
		/// 异步加载AssetBundle.(add by zhan)
		/// </summary>
		/// <param name="iKey">Key.</param>
		/// <param name="iType">类型.</param>
		private IEnumerator LoadFromAssetBundleAsync(
			string iKey, 
			Type type, 
			Action<string, TAssetBundleType, UnityEngine.Object> iLoadSuccess, 
			TAssetBundleType iType = TAssetBundleType.None){

			string bundleId = this.GetBundleId (iKey, iType);
			if (string.IsNullOrEmpty (bundleId) == true) {
				yield return null;
			}
			// 预先加载依赖相关的bundle
			DownloadTargetInfo targetInfo = null;
			if (DownloadList.GetInstance ().isTargetExist (bundleId, out targetInfo) == false) {
				yield return null;
			}
			if (targetInfo != null) {
				yield return this.LoadAssetBundleAsync (
					bundleId, targetInfo.FileType);
				yield return new WaitForEndOfFrame ();

				AssetBundle bundle = this.GetBundleByID (bundleId);
				if (bundle == null) {
					this.Warning ("LoadFromAssetBundleAsync():Failed!!! Bundle Id:{0} Path:{1}",
						bundleId, iKey);
				} else {
					this.Info ("LoadFromAssetBundleAsync():Successed. Bundle Id:{0} Path:{1}",
						bundleId, iKey);

					var request = bundle.LoadAssetAsync(iKey, type);// fix by zhan
					yield return  request;
					if (request.asset == null) {
						this.Warning ("LoadFromAssetBundleAsync():There is no target in assetBundle!!!(BundleId:{0} Key:{1})", bundleId, iKey);
					}

					if (iLoadSuccess != null) {
						iLoadSuccess (iKey, iType, request.asset);
					}
				}
			} else {
				if (iLoadSuccess != null) {
					iLoadSuccess (iKey, iType, null);
				}
				this.Warning ("LoadFromAssetBundleAsync():There is no target in download list!!!(BundleId:{0})", bundleId);
			}
			yield return null;
		}

		/// <summary>
		/// 异步加载AssetBundle.
		/// </summary>
		/// <param name="iKey">Key.</param>
		/// <param name="iLoadCompleted">加载完成.</param>
		/// <param name="iLoadFailed">加载失败.</param>
		/// <param name="iType">类型.</param>
		public IEnumerator LoadFromAssetBundleAsync<T>(
			string iKey, 
			Action<bool, string, TAssetBundleType, T> iLoadCompleted, 
			Func<string, System.Action<bool, string, TAssetBundleType, T>, IEnumerator> iLoadFailed, 
			TAssetBundleType iType = TAssetBundleType.None) where T : UnityEngine.Object {

			string bundleId = this.GetBundleId (iKey, iType);
			if (string.IsNullOrEmpty (bundleId) == true) {
				if (null != iLoadFailed) {
					yield return iLoadFailed (iKey, iLoadCompleted);
				}
			} else {
				// 预先加载依赖相关的bundle
				DownloadTargetInfo targetInfo = null;
				if (DownloadList.GetInstance ().isTargetExist (bundleId, out targetInfo) == false) {
					if (null != iLoadFailed) {
						yield return iLoadFailed (iKey, iLoadCompleted);
					}
				} else {
					if (targetInfo != null) {
						yield return this.LoadAssetBundleAsync (bundleId, targetInfo.FileType);
						yield return new WaitForEndOfFrame ();

						AssetBundle bundle = this.GetBundleByID (bundleId);
						if (bundle == null) {
							this.Warning ("LoadFromAssetBundleAsync():Failed!!! Bundle Id:{0} Path:{1}",
								bundleId, iKey);
							if (null != iLoadFailed) {
								yield return iLoadFailed (iKey, iLoadCompleted);
							}
						} else {
							this.Info ("LoadFromAssetBundleAsync():Successed. Bundle Id:{0} Path:{1}",
								bundleId, iKey);

							AssetBundleRequest request = bundle.LoadAssetAsync<T> (iKey);
							yield return request;

							//获取加载的对象，并创建出来
							T objTmp = request.asset as T;
							if (objTmp != null) {
								if (iLoadCompleted != null) {
									iLoadCompleted (true, iKey, iType, objTmp);
									yield return new WaitForEndOfFrame ();
								}
								yield break;
							} else {
								this.Warning ("LoadFromAssetBundleAsync():The type of target is invalied!!!(BundleId:{0} Key:{1} Type:{2})", 
									bundleId, iKey, typeof(T).ToString ());
								if (null != iLoadFailed) {
									yield return iLoadFailed (iKey, iLoadCompleted);
								}
							}
						}
					} else {
						this.Warning ("LoadFromAssetBundleAsync():There is no target in download list!!!(BundleId:{0})", bundleId);
						if (null != iLoadFailed) {
							yield return iLoadFailed (iKey, iLoadCompleted);
						}
					}
				}
			}
			yield return null;
		}

		/// <summary>
		/// 加载AssetBundle(异步).
		/// </summary>
		/// <returns>AssetBundle.</returns>
		/// <param name="iBundleId">BundleId.</param>
		/// <param name="iFileType">文件类型.</param>
		private IEnumerator LoadAssetBundleAsync(
			string iBundleId, TUploadFileType iFileType) {

			if (string.IsNullOrEmpty (iBundleId) == true) {
				yield break;
			}

			if ((this.CheckMainManifest () == true) && 
				(this._mainManifest != null)) {
				yield return new WaitForEndOfFrame ();

				string dependName = UploadList.GetLocalBundleFileName (iBundleId, iFileType);
				if (string.IsNullOrEmpty (dependName) == true) {
					yield break;
				}
				string[] depends = this._mainManifest.GetAllDependencies(dependName);
				for(int index = 0;index < depends.Length; index++)
				{
					string dependBundleId = this.ConvertDependNameToBundleId(depends[index]);
					if (string.IsNullOrEmpty (dependBundleId) == true) {
						continue;
					}
					yield return this.LoadBundleByIdAsync (dependBundleId);
					yield return new WaitForEndOfFrame ();
				}
			
				yield return this.LoadBundleByIdAsync (iBundleId);
				yield return new WaitForEndOfFrame ();
			}
			yield return null;
		}

		/// <summary>
		/// 加载Bundle(异步加载).
		/// </summary>
		/// <returns>Bundle.</returns>
		/// <param name="iPath">路径.</param>
		/// <param name="iAssetBundle">AssetBundle包.</param>
		private IEnumerator LoadBundleByIdAsync(string iBundleId)
		{
			if (null == _bundles) {
				_bundles = new Dictionary<string, AssetBundle> ();
			}

			if (false == _bundles.ContainsKey(iBundleId)) {
				string path = DownloadList.GetInstance ().GetBundleFullPath (iBundleId);
				if (true == File.Exists (path)) {
					AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync (path);
					yield return request;
					_bundles[iBundleId] = request.assetBundle;
					yield return new WaitForEndOfFrame ();
				} else {
					this.Error ("LoadBundleByIdAsync():The assetbundle file is not exist!!!![Path:{0}]", path);
				}
			}
			yield return null;

		}

		#endregion

		#region 同步加载

		/// <summary>
		/// 加载场景.
		/// </summary>
		/// <param name="iSceneName">场景名.</param>
		public bool LoadScene(string iSceneName) {

			if (string.IsNullOrEmpty (iSceneName) == true) {
				return false;
			}

			if (this.LoadSceneAssetBundle (iSceneName) == true) {
				// 加载场景
				SceneManager.LoadScene (iSceneName);

			}

			return true;
		}

		/// <summary>
		/// 加载预制体.
		/// </summary>
		/// <param name="iPrefabName">预制体名.</param>
		/// <param name="iParent">父节点.</param>
		/// <param name="iPosition">坐标.</param>
		/// <param name="iRotation">旋转.</param>
		/// <param name="iScale">缩放.</param>
		public bool LoadPrefab(
			string iPrefabName,
			GameObject iParent, 
			Vector3 iPosition, 
			Vector3 iScale) {

			if (string.IsNullOrEmpty (iPrefabName) == true) {
				return false;
			}

			if (iParent == null) {
				return false;
			}

			GameObject prefab = null;
			UnityEngine.Object objPrefab = this.LoadPreFab (iPrefabName);
			if (objPrefab == null) {
				return false;
			}
			prefab = GameObject.Instantiate(objPrefab) as GameObject;
			if (prefab == null) {
				return false;
			}
			prefab.transform.parent = iParent.transform;
			prefab.transform.localPosition = iPosition;
			prefab.transform.localScale = iScale;

			return true;
		}

		/// <summary>
		/// 加载预制体.
		/// </summary>
		/// <returns>预制体.</returns>
		/// <param name="iPrefabName">预制体名.</param>
		public UnityEngine.Object LoadPreFab(string iPrefabName) {
			return this.LoadFromAssetBundle<UnityEngine.Object> (
				iPrefabName, TAssetBundleType.Prefab);
		}

		/// <summary>
		/// 加载音效（无需后缀）.
		/// </summary>
		/// <returns>音效.</returns>
		/// <param name="iAudioName">音效名.</param>
		public AudioClip LoadAudio(string iAudioName) {
			return this.LoadFromAssetBundle<AudioClip> (
				iAudioName, TAssetBundleType.Audio);
		}

		/// <summary>
		/// 加载材质（无需后缀）.
		/// </summary>
		/// <returns>材质.</returns>
		/// <param name="iMatName">材质名.</param>
		public Material LoadMaterial(string iMatName) {
			return this.LoadFromAssetBundle<Material> (
				iMatName, TAssetBundleType.Mat);
		}

		/// <summary>
		/// 加载纹理.
		/// </summary>
		/// <returns>纹理.</returns>
		/// <param name="iTextureName">纹理名.</param>
		public Texture2D LoadTexture(string iTextureName) {
			return this.LoadFromAssetBundle<Texture2D> (
				iTextureName, TAssetBundleType.Texture);
		}

		/// <summary>
		/// 加载Asset文件(.asset).
		/// </summary>
		/// <returns>Asset文件(.asset).</returns>
		/// <param name="iAssetFileName">Asset文件名(.asset).</param>
		public UnityEngine.TextAsset LoadAssetFile(string iAssetFileName) {
			return this.LoadFromAssetBundle<UnityEngine.TextAsset> (
				iAssetFileName, TAssetBundleType.Asset);
		}

		/// <summary>
		/// 加载文本文件(.text).
		/// </summary>
		/// <returns>文本文件(.text).</returns>
		/// <param name="iTextFileName">文本文件名(.text).</param>
		public string LoadTextFile(string iTextFileName) {
			UnityEngine.TextAsset objTmp = this.LoadFromAssetBundle<UnityEngine.TextAsset> (
				iTextFileName, TAssetBundleType.Text);
			if (objTmp != null) {
				return objTmp.text;
			}
			return null;
		}

		/// <summary>
		/// 加载Json文件(.json).
		/// </summary>
		/// <returns>Json文件(.json).</returns>
		/// <param name="iTextFileName">Json文件名(.json).</param>
		public string LoadJsonFile(string iJsonFileName) {
			UnityEngine.TextAsset objTmp = this.LoadFromAssetBundle<UnityEngine.TextAsset> (
				iJsonFileName, TAssetBundleType.Json);
			if (objTmp != null) {
				return objTmp.text;
			}
			return null;
		}

		/// <summary>
		/// 加载Json文件(.json).
		/// </summary>
		/// <returns>Json文件(.json).</returns>
		/// <param name="iTextFileName">Json文件名(.json).</param>
		public T LoadJsonFile<T>(string iJsonFileName) {
			string jsonFileStr = this.LoadJsonFile (iJsonFileName);
			if (string.IsNullOrEmpty (jsonFileStr) == false) {
				return JsonUtility.FromJson<T>(jsonFileStr);
			}
			return default(T);
		}
			
		/// <summary>
		/// 加载asset bundle.
		/// </summary>
		/// <param name="iFilePath">文件路径.</param>
		public UnityEngine.Object LoadAssetBundle(string iFilePath) {

			UnityEngine.Object objTmp = this.LoadFromAssetBundle<UnityEngine.Object> (
				iFilePath, TAssetBundleType.None);
			if (objTmp != null) {
				return objTmp;
			}
			return null;
		}

		/// <summary>
		/// 加载asset bundle.(add by zhan)
		/// </summary>
		/// <param name="iFilePath">文件路径.</param>
		public T LoadAssetBundle<T>(string iFilePath) where T : UnityEngine.Object {

			T objTmp = this.LoadFromAssetBundle<T> (
				iFilePath, TAssetBundleType.None);
			if (objTmp != null) {
				return objTmp;
			}
			return null;
		}

		/// <summary>
		/// 加载AssetBundle.
		/// </summary>
		/// <returns>AssetBundle.</returns>
		/// <param name="iBundleId">BundleId.</param>
		/// <param name="iFileType">文件类型.</param>
		private AssetBundle LoadAssetBundle(
			string iBundleId, TUploadFileType iFileType) {

			if (true == string.IsNullOrEmpty (iBundleId)) {
				return null;
			}
				
			if (this.CheckMainManifest () == false) {
				return null;
			}
			if (this._mainManifest == null) {
				return null;
			}

			string dependName = UploadList.GetLocalBundleFileName (iBundleId, iFileType);
			if (string.IsNullOrEmpty (dependName) == true) {
				return null;
			}
			string[] depends = this._mainManifest.GetAllDependencies(dependName);
			DownloadTargetInfo dependInfo = null;
			for(int index = 0;index < depends.Length; index++)
			{
				string dependBundleId = this.ConvertDependNameToBundleId(depends[index]);
				if (true == string.IsNullOrEmpty (dependBundleId)) {
					continue;
				}
				AssetBundle objTemp = GetOrCreateBundle (dependBundleId);
				if (null == objTemp) {
					this.Error ("LoadAssetBundle():GetOrCreateBundle Failed!!! BundleId:{0}", dependBundleId);
				}
			}
			AssetBundle bundle = GetOrCreateBundle(iBundleId);
			if(null == bundle) {
				this.Error ("LoadAssetBundle():GetOrCreateBundle Failed!!! BundleId:{0}", iBundleId);
				return null;
			}
			return bundle;
		}

		/// <summary>
		/// 加载AssetBundle.
		/// </summary>
		/// <param name="iKey">Key.</param>
		/// <param name="iType">类型.</param>
		public T LoadFromAssetBundle<T>(
			string iKey, TAssetBundleType iType = TAssetBundleType.None) where T : UnityEngine.Object {

			T objRet = default(T);
			string bundleId = this.GetBundleId (iKey, iType);
			if (string.IsNullOrEmpty (bundleId) == true) {
				return default(T);
			}
			// 预先加载依赖相关的bundle
			DownloadTargetInfo targetInfo = null;
			if (DownloadList.GetInstance ().isTargetExist (bundleId, out targetInfo) == false) {
				return default(T);
			}
			if (targetInfo == null) {
				return default(T);
			}
			AssetBundle assetBundle = this.LoadAssetBundle (bundleId, targetInfo.FileType);

			if(assetBundle == null) {
				this.Error ("LoadFromAssetBundle():Failed!!! BundleId:{0} Key:{1}", bundleId, iKey);
				return default(T);
			}
			objRet = assetBundle.LoadAsset<T> (iKey);
			if(objRet == null) {
				this.Error ("LoadFromAssetBundle():Failed!!! BundleId:{0} Key:{1}", bundleId, iKey);
			}
			return objRet;
		}		

		/// <summary>
		/// 加载场景.
		/// </summary>
		/// <param name="iSceneName">场景名.</param>
		private bool LoadSceneAssetBundle(string iSceneName) {

			string bundleId = this.GetBundleId (iSceneName, TAssetBundleType.Scene);
			if (string.IsNullOrEmpty (bundleId) == true) {
				return false;
			}
			// 预先加载依赖相关的bundle
			DownloadTargetInfo targetInfo = null;
			if (DownloadList.GetInstance ().isTargetExist (bundleId, out targetInfo) == false) {
				return false;
			}
			if (targetInfo == null) {
				return false;
			}
			AssetBundle bundle = GetOrCreateBundle(bundleId);
			if(bundle == null) {
				this.Error ("LoadSceneAssetBundle():GetOrCreateBundle Failed!!! BundleId:{0}", bundleId);
				return false;
			}
			return true;
		}

		/// <summary>
		/// 取得或创建Bundle
		/// 备注：
		/// 1)若已经加载过了，则不重复加载.
		/// 2)若无加载过，则创建
		/// </summary>
		/// <returns>Bundle.</returns>
		/// <param name="iBundleId">BundleId.</param>
		private AssetBundle GetOrCreateBundle(string iBundleId) {
			AssetBundle bundle = GetBundleByID(iBundleId);
			if (null == bundle) {
				bundle = this.LoadBundleById (iBundleId);
				if (bundle != null) {
					this._bundles [iBundleId] = bundle;
				}
			}
			return bundle;
		}

		/// <summary>
		/// 取得AssetBundle.
		/// </summary>
		/// <returns>AssetBundle.</returns>
		/// <param name="iBundleId">BundleId.</param>
		private AssetBundle GetBundleByID(string iBundleId) {
			AssetBundle bundle = null;
			if (_bundles.TryGetValue (iBundleId, out bundle) == true) {
				return bundle;
			}
			return null;
		}

		/// <summary>
		/// 检测主Manifest文件.
		/// </summary>
		/// <returns><c>true</c>, OK, <c>false</c> NG.</returns>
		private bool CheckMainManifest() {

			if (this._mainBundle != null) {
				this._mainBundle.Unload (false);
			}

			string mainManifestID = UploadList.AssetBundleDirNameOfNormal;
			this._mainBundle = this.LoadMainManifestById (mainManifestID);
			if (this._mainBundle == null) {
				this.Error ("CheckMainManifest():ManifestBundle load failed!!!(BundleId:{0})", mainManifestID);
				return false;
			}
			this._mainManifest = this._mainBundle.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
			if (this._mainManifest == null) {
				this.Error ("CheckMainManifest():ManifestBundle load failed!!!(BundleId:{0})", mainManifestID);
				return false;
			}

			return true;
		}

		/// <summary>
		/// 加载MainManifest(同步加载).
		/// </summary>
		/// <returns>加载MainManifest的AssetBundle.</returns>
		/// <param name="iPath">路径.</param>
		private AssetBundle LoadMainManifestById(string iBundleId)
		{
			string path = DownloadList.GetInstance ().GetBundleFullPath (iBundleId, 
				TUploadFileType.MainManifest);
			if (File.Exists (path) == false) {
				this.Error ("LoadMainManifestById():The assetbundle file is not exist!!!![Path:{0}]", path);
				return null;
			}

			return AssetBundle.LoadFromFile(path);
		}

		/// <summary>
		/// 加载Bundle(同步加载).
		/// </summary>
		/// <returns>Bundle.</returns>
		/// <param name="iPath">路径.</param>
		private AssetBundle LoadBundleById(string iBundleId)
		{
			string path = DownloadList.GetInstance ().GetBundleFullPath (iBundleId);
			if (false == File.Exists (path)) {
				this.Error ("LoadBundleById():The assetbundle file is not exist!!!![Path:{0}]", path);
				return null;
			}
			return AssetBundle.LoadFromFile(path);
		}

		#endregion



		/// <summary>
		/// 将依赖名转换成BundleId.
		/// </summary>
		/// <returns>BundleId.</returns>
		/// <param name="iDependName">依赖名.</param>
		private string ConvertDependNameToBundleId(string iDependName) {
			if (string.IsNullOrEmpty (iDependName) == true) {
				return null;
			}
			string fileSuffix = UploadList.GetInstance ().FileSuffix;
			fileSuffix = fileSuffix.ToLower ();
			if (string.IsNullOrEmpty (fileSuffix) == true) {
				return iDependName;
			}
			fileSuffix = string.Format (".{0}", fileSuffix);
			return iDependName.Replace (fileSuffix, "");
		}

		/// <summary>
		/// 释放已有Bundle.
		/// </summary>
		private void ReleaseAllBundle() {
			if (_bundles != null) {
				foreach (AssetBundle bundle in _bundles.Values) {
					bundle.Unload (false);
				}
				_bundles.Clear();
			}
		}

		/// <summary>
		/// 释放已有Bundle.(add by zhan)
		/// </summary>
		public void ReleaseAllBundle(bool unloadAllLoadedObjects) {
			if (_bundles != null) {
				foreach (AssetBundle bundle in _bundles.Values) {
					bundle.Unload (unloadAllLoadedObjects);
				}
				_bundles.Clear();
			}
		} 

		/// <summary>
		/// 取得BundleID.
		/// </summary>
		/// <returns>BundleID.</returns>
		/// <param name="iKey">Key.</param>
		/// <param name="iType">类型.</param>
		private string GetBundleId(string iKey, TAssetBundleType iType = TAssetBundleType.None) {
			string bundleID = null;
			string[] fileSuffixs = null;

			if (string.IsNullOrEmpty (iKey) == true) {
				return null;
			}

			switch (iType) {
			case TAssetBundleType.Scene:
				{
					if(_scenesMap == null) {
						bundleID = null;
						break;
					}
					if(_scenesMap.TryGetValue(iKey, out bundleID) == false)
					{
						bundleID = null;
					}
					fileSuffixs = new string[] { _FILE_SUNFFIX_SCENE };
				}
				break;

			case TAssetBundleType.Prefab:
				{
					if(_prefabsMap == null) {
						bundleID = null;
						break;
					}
					if(_prefabsMap.TryGetValue(iKey, out bundleID) == false)
					{
						bundleID = null;
					}
					fileSuffixs = new string[] { _FILE_SUNFFIX_PREFAB };
				}
				break;

			case TAssetBundleType.Audio:
				{
					if(_audiosMap == null) {
						bundleID = null;
						break;
					}
					if(_audiosMap.TryGetValue(iKey, out bundleID) == false)
					{
						bundleID = null;
					}
					fileSuffixs = new string[] { _FILE_SUNFFIX_AUDIO_WAV, _FILE_SUNFFIX_AUDIO_MP3 };
				}
				break;

			case TAssetBundleType.Mat:
				{
					if(_matsMap == null) {
						bundleID = null;
						break;
					}
					if(_matsMap.TryGetValue(iKey, out bundleID) == false)
					{
						bundleID = null;
					}
					fileSuffixs = new string[] { _FILE_SUNFFIX_MAT };
				}
				break;

			case TAssetBundleType.Texture:
				{
					if(_texturesMap == null) {
						bundleID = null;
						break;
					}
					if(_texturesMap.TryGetValue(iKey, out bundleID) == false)
					{
						bundleID = null;
					}
					fileSuffixs = new string[] { _FILE_SUNFFIX_TEXTURE_PNG };
				}
				break;

			case TAssetBundleType.Asset:
				{
					if(_assetFilesMap == null) {
						bundleID = null;
						break;
					}
					if(_assetFilesMap.TryGetValue(iKey, out bundleID) == false)
					{
						bundleID = null;
					}
					fileSuffixs = new string[] { _FILE_SUNFFIX_ASSET };
				}
				break;

			case TAssetBundleType.Text:
				{
					if(_textsMap == null) {
						bundleID = null;
						break;
					}
					if(_textsMap.TryGetValue(iKey, out bundleID) == false)
					{
						bundleID = null;
					}
					fileSuffixs = new string[] { _FILE_SUNFFIX_TEXT };
				}
				break;


			case TAssetBundleType.Json:
				{
					if(_jsonFilesMap == null) {
						bundleID = null;
						break;
					}
					if(_jsonFilesMap.TryGetValue(iKey, out bundleID) == false)
					{
						bundleID = null;
					}
					fileSuffixs = new string[] { _FILE_SUNFFIX_JSON };
				}
				break;

			case TAssetBundleType.None:
			default:
				{
					if(_bundlesMap == null) {
						bundleID = null;
					}
					bundleID = null;
					if(_bundlesMap.TryGetValue(iKey, out bundleID) == false)
					{
						bundleID = null;
					}
				}
				break;
			}

			if ((TAssetBundleType.None != iType) && 
				(string.IsNullOrEmpty (bundleID) == true)) {

				bool isBreak = false;
				foreach(string fileSuffix in fileSuffixs) {
					isBreak = false;
					string fileName = string.Format ("{0}{1}", iKey, fileSuffix);
					foreach (KeyValuePair<string, string> loop in this._bundlesMap) {
						if (loop.Key.EndsWith (fileName) == true) {
							bundleID = loop.Value;
							isBreak = true;
							break;
						}
					}
					if (isBreak == true) {
						break;
					}
				}
			}
			return bundleID;
		}

	}
}
