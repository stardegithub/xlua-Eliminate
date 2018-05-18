//#define NFF_ASSET_BUNDLE

using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using AssetBundles;
using Common;

namespace AssetBundles {

	/// <summary>
	/// 数据加载器.
	/// </summary>
	public sealed class DataLoader
	{
		public static Object LoadFromResource(string path)
		{
			Object objRet = ResourcesLoad.Load(path) as Object;
			if(null == objRet) {
				UtilsLog.Warning ("DataLoadController", 
					"LoadFromResource():Load Failed!!(path:{0})", path);
			}
			return objRet;
		}

		/// <summary>
		/// 加载数据（旧版本）.
		/// </summary>
		/// <returns>加载队形.</returns>
		/// <param name="path">路径.</param>
		public static Object LoadData(string path)
		{
			Object objRet = Load<Object>(path);
			if(null == objRet) {
				UtilsLog.Warning ("DataLoadController", 
					"LoadData():Load Failed!!(path:{0})", path);
			}
			return objRet;
		}

		/// <summary>
		/// 释放所有已有的Bundles.
		/// </summary>
		public static void ReleaseAllBundles()
		{
			AssetBundlesManager.Instance.Dispose ();
		}

		/// <summary>
		/// 检测文件.
		/// </summary>
		/// <returns><c>true</c>, OK, <c>false</c> NG.</returns>
		/// <param name="iPath">路径.</param>
		public static bool CheckFile(string iPath)
		{
			if(ResourcesLoad.Load(iPath) != null)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// 加载场景.
		/// </summary>
		/// <param name="iSceneName">I scene name.</param>
		public static void LoadScene(string iSceneName)
		{
			if (false == AssetBundlesManager.Instance.LoadScene (iSceneName)) {
				UtilsLog.Warning ("DataLoadController", 
					"LoadScene():There is no scene({0}) in asset bundles manager!!", 
					iSceneName);

				// 加载场景
				SceneManager.LoadScene (iSceneName);
			} 
		}

		/// <summary>
		/// 加载函数(单个：同步).
		/// </summary>
		/// <returns>加载对象</returns>
		/// <param name="iPath">路径.</param>
		public static T Load<T>(string iPath) where T : UnityEngine.Object {
			if (true == string.IsNullOrEmpty (iPath)) {
				return default(T);
			}
			T objRet = default(T);

			string _fileName = AssetBundlesManager.GetKeyOfMapByFilePath (iPath);
			if (true == string.IsNullOrEmpty (_fileName)) {
				return default(T);
			}

			objRet = AssetBundlesManager.Instance.LoadFromAssetBundle<T>(_fileName);
			if (null == objRet) {
				objRet = ResourcesLoad.Load<T>(iPath);
			}
			return objRet;
		}

		/// <summary>
		/// 加载函数(复数：同步).
		/// </summary>
		/// <param name="iPath">路径.</param>
		/// <param name="iLoadCompleted">加载完毕回调函数.</param>
		public static void Load(string[] iPaths, 
			System.Action<bool, string, UnityEngine.Object> iLoadCompleted) {
			foreach (string path in iPaths) {
				UnityEngine.Object obj = Load<UnityEngine.Object> (path);
				// 加载失败
				if (null == obj) {
					if (null != iLoadCompleted) {
						iLoadCompleted (false, path, null);
					}
					// 加载成功
				} else {
					if (null != iLoadCompleted) {
						iLoadCompleted (true, path, obj);
					}
				}
			}
		}

		/// <summary>
		/// 加载函数(单个：异步).
		/// </summary>
		/// <param name="iPath">路径.</param>
		/// <param name="iLoadSuccess">加载成功回调函数.</param>
		public static IEnumerator LoadAsync<T>(string iPath, 
			System.Action<bool, string, TAssetBundleType, T> iLoadCompleted) where T : UnityEngine.Object {

			yield return AssetBundlesManager.Instance.LoadFromAssetBundleAsync<T> (
				iPath, iLoadCompleted, LoadFailedInAssetBundlesManager<T>);
			yield return new WaitForEndOfFrame ();
		}

		/// <summary>
		/// 从AssetBundlesManager加载失败回调函数.
		/// </summary>
		/// <param name="iPath">路径.</param>
		/// <param name="iLoadSuccess">加载成功回调函数.</param>
		private static IEnumerator LoadFailedInAssetBundlesManager<T> (
			string iPath,
			System.Action<bool, string, TAssetBundleType, T> iLoadCompleted)  where T : UnityEngine.Object {

			// 从AssetBundlesManager加载失败,则从本地加载
			yield return ResourcesLoad.LoadAsync<T>(iPath, iLoadCompleted);
			yield return new WaitForEndOfFrame ();
		}

		/// <summary>
		/// 加载函数(复数：异步).
		/// </summary>
		/// <param name="iPath">路径.</param>
		public static IEnumerator LoadAsync(string[] iPaths, 
			System.Action<bool, string, TAssetBundleType, UnityEngine.Object> iLoadCompleted) {

			// 遍历所有资源
			foreach (string path in iPaths) {
				yield return LoadAsync<UnityEngine.Object> (path, iLoadCompleted);
				yield return new WaitForEndOfFrame ();
			}
			yield return null;

		}
	}

	/// <summary>
	/// 资源加载类.
	/// </summary>
	public sealed class ResourcesLoad {

		/// <summary>
		/// 加载（旧版）.
		/// </summary>
		/// <param name="path">路径.</param>
		/// <param name="type">类型.</param>
		public static Object Load(string path,System.Type type)
		{
			if(path.StartsWith("Assets/Resources/"))
			{
				path = path.Substring(17);
			}

			if(path.StartsWith("Assets/"))
			{
				path = path.Substring(7);
			}

			bool beGB = false;
			//		if(path.EndsWith(".prefab"))
			//		{
			//			beGB = false;
			//		}
			//path = path.Replace ("Assets/", "");
			path = path.Replace (".prefab", "");
			//	Debug.Log (path);
			path = path.Replace (".asset", "");
			path = path.Replace (".mat", "");
			path = path.Replace (".wav", "");	
			path = path.Replace (".txt", "");	
			path = path.Replace (".mp3", "");
			path = path.Replace (".png", "");
			//Debug.Log (path);
			//Debug.Log ("---------------------------------------");
			if(beGB)
			{
				return Resources.Load<GameObject> (path) as Object;
			}
			else
			{
				return Resources.Load (path,type);
			}
			//return null;
		}

		/// <summary>
		/// 加载（旧版）.
		/// </summary>
		/// <param name="path">路径.</param>
		public static object Load(string path)
		{
			//		Debug.Log(path);
			if(path.StartsWith("Assets/Resources/"))
			{
				path = path.Substring(17);
			}

			if(path.StartsWith("Assets/"))
			{
				path = path.Substring(7);
			}

			bool beGB = false;
			if(path.EndsWith(".prefab"))
			{
				beGB = true;
			}
			//path = path.Replace ("Assets/", "");
			path = path.Replace (".prefab", "");
			//	Debug.Log (path);
			path = path.Replace (".asset", "");
			path = path.Replace (".mat", "");
			path = path.Replace (".wav", "");	
			path = path.Replace (".txt", "");	
			path = path.Replace (".mp3", "");
			path = path.Replace (".png", "");
			//Debug.Log (path);
			//Debug.Log ("---------------------------------------");
			if(beGB)
			{
				return Resources.Load<GameObject> (path) as Object;
			}
			else
			{
				return Resources.Load (path);
			}

		}

		/// <summary>
		/// 加载.
		/// </summary>
		/// <param name="iPath">路径.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T Load<T>(string iPath)  where T : UnityEngine.Object
		{
			if (true == string.IsNullOrEmpty (iPath)) {
				return default(T);
			}
			string _path = iPath;
			//		Debug.Log(path);
			if(_path.StartsWith("Assets/Resources/"))
			{
				_path = _path.Substring(17);
			}

			if(_path.StartsWith("Assets/"))
			{
				_path = _path.Substring(7);
			}

			// 初始化
			string _loadRes = null;

			// 根据文件后缀名处理
			_loadRes = _path.Replace (".prefab", "");
			_loadRes = _loadRes.Replace (".png", "");
			_loadRes = _loadRes.Replace (".jpg", "");
			_loadRes = _loadRes.Replace (".asset", "");
			_loadRes = _loadRes.Replace (".txt", "");
			_loadRes = _loadRes.Replace (".json", "");
			_loadRes = _loadRes.Replace (".mat", "");
			_loadRes = _loadRes.Replace (".wav", "");
			_loadRes = _loadRes.Replace (".mp3", "");
			_loadRes = _loadRes.Replace (".tga", "");
			return Resources.Load<T> (_loadRes);

		}

		/// <summary>
		/// 加载（异步）.
		/// </summary>
		/// <param name="iPath">路径.</param>
		/// <param name="iLoadCompleted">加载完毕回调函数.</param>
		public static IEnumerator LoadAsync<T>(string iPath,
			System.Action<bool, string, AssetBundles.TAssetBundleType, T> iLoadCompleted) where T : UnityEngine.Object
		{
			string _path = iPath;
			//		Debug.Log(path);
			if(_path.StartsWith("Assets/Resources/"))
			{
				_path = _path.Substring(17);
			}

			if(_path.StartsWith("Assets/"))
			{
				_path = _path.Substring(7);
			}

			// 初始化
			string _loadRes = null;
			ResourceRequest _res = null;
			AssetBundles.TAssetBundleType _bundleType = AssetBundles.TAssetBundleType.None;

			// 根据文件后缀名处理
			if (true == _path.EndsWith (".prefab")) {
				_loadRes = _path.Replace (".prefab", "");
				_bundleType = AssetBundles.TAssetBundleType.Prefab;
			} else if (
				(true == _path.EndsWith (".png")) ||
				(true == _path.EndsWith (".jpg"))) {
				_loadRes = _path.Replace (".png", "");
				_loadRes = _loadRes.Replace (".jpg", "");
				_bundleType = AssetBundles.TAssetBundleType.Texture;
			} else if (true == _path.EndsWith (".asset")) {
				_loadRes = _path.Replace (".asset", "");
				_bundleType = AssetBundles.TAssetBundleType.Asset;
			} else if (true == _path.EndsWith (".txt")) {
				_loadRes = _path.Replace (".txt", "");
				_bundleType = AssetBundles.TAssetBundleType.Text;
			} else if (true == _path.EndsWith (".json")) {
				_loadRes = _path.Replace (".json", "");
				_bundleType = AssetBundles.TAssetBundleType.Json;
			} else if (true == _path.EndsWith (".mat")) {
				_loadRes = _path.Replace (".mat", "");
				_bundleType = AssetBundles.TAssetBundleType.Mat;
			} else if (
				(true == _path.EndsWith (".wav")) ||
				(true == _path.EndsWith (".mp3"))) {
				_loadRes = _path.Replace (".wav", "");
				_loadRes = _loadRes.Replace (".mp3", "");
				_bundleType = AssetBundles.TAssetBundleType.Audio;
			}
			yield return new WaitForEndOfFrame ();

			// 加载资源
			_res = Resources.LoadAsync<T> (_loadRes);
			yield return _res;

			T obj = _res.asset as T;
			if (default(T) != obj) {
				if (null != iLoadCompleted) {
					iLoadCompleted (true, iPath, _bundleType, obj);
					yield return new WaitForEndOfFrame ();
				} 
				yield break;
			} else {
				if (null != iLoadCompleted) {
					iLoadCompleted (false, iPath, _bundleType, null);
					yield return new WaitForEndOfFrame ();
				}
			}

			yield return null;
		}
	}
}
