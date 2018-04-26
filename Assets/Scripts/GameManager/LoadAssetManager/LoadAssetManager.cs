using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AssetBundles;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameManager
{
    public class LoadAssetManager : GameManagerBase<LoadAssetManager>
    {
        public Dictionary<string, LoadAssetInfo> AssetPathMap { get; private set; }

        #region Singleton
        protected override void SingletonAwake()
        {
            AssetPathMap = new Dictionary<string, LoadAssetInfo>();
            initialized = true;
        }
        #endregion

        #region 同步
        /// <summary>
        /// 同步加载资源，通过资源路径
        /// </summary>
        /// <param name="assetPath">资源路径</param>
        /// <returns>资源</returns>
        public T LoadAsset<T>(string assetPath) where T : Object
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                return null;
            }

            LoadAssetInfo loadAssetInfo;
            if (!AssetPathMap.TryGetValue(assetPath, out loadAssetInfo))
            {
                loadAssetInfo = new LoadAssetInfo(assetPath);
                AssetPathMap[loadAssetInfo.assetPath] = loadAssetInfo;
            }

            return LoadAsset<T>(loadAssetInfo);
        }

        /// <summary>
        /// 同步加载资源，通过资源信息
        /// </summary>
        /// <param name="loadAssetInfo">资源信息</param>
        /// <returns>资源</returns>
        public T LoadAsset<T>(LoadAssetInfo loadAssetInfo) where T : Object
        {
            if (loadAssetInfo == null)
            {
                return null;
            }

            if (loadAssetInfo.isLoadCompeleted)
            {
                if (loadAssetInfo.assetObject != null && loadAssetInfo.assetObjectType == typeof(T))
                {
                    return loadAssetInfo.assetObject as T;
                }
            }
            else
            {
                if (loadAssetInfo.assetStoreType == AssetStoreType.AssetBundle)
                {
                    // var asset = AssetBundlesManager.GetInstance().LoadAssetBundle<T>(loadAssetInfo.assetPath);
                    // if (asset != null)
                    // {
                    //     loadAssetInfo.SetAssetObject(asset, typeof(T));
                    //     return asset;
                    // }
                    loadAssetInfo.assetStoreType = AssetStoreType.Resources;
                }

                if (loadAssetInfo.assetStoreType == AssetStoreType.Resources)
                {
                    string path = GetResourcesPath(loadAssetInfo.assetPath);
                    var asset = Resources.Load<T>(path);
                    loadAssetInfo.SetAssetObject(asset, typeof(T));
                    return asset;
                }
            }

            return null;
        }

        /// <summary>
        /// 同步加载sprite，通过assetPath和spriteName
        /// </summary>
        /// <param name="atlasPath">atlas地址</param>
        /// <param name="spriteName">sprite名字</param>
        /// <returns>sprite</returns>
        public Sprite LoadSprite(string atlasPath, string spriteName)
        {
            // var atlas = LoadAsset<UGUIAtlas>(atlasPath);
            // if (atlas != null)
            // {
            //     return atlas[spriteName];
            // }
            return null;
        }

        /// <summary>
        /// 同步加载sprite，通过sprite资源数据
        /// </summary>
        /// <param name="spriteAsset">sprite资源数据</param>
        /// <returns>sprite</returns>
        public Sprite LoadSprite(SpriteAssetConfig spriteAsset)
        {
            return LoadSprite(spriteAsset.atlas, spriteAsset.sprite);
        }
        #endregion

        #region 异步
        /// <summary>
        /// 异步加载资源，通过资源路径
        /// </summary>
        /// <param name="assetPath">资源地址</param>
        /// <param name="loadCompleted">完成回调函数</param>
        /// <returns>加载请求信息</returns>
        public LoadAssetsRequest LoadAssetAsync(string assetPath, Action<LoadAssetInfo> loadCompleted)
        {
            string[] assetPaths = new string[] { assetPath };

            Action<LoadAssetInfo[]> func = null;
            if (loadCompleted != null)
            {
                func = delegate (LoadAssetInfo[] loadAssetInfos)
                {
                    if (loadAssetInfos != null && loadAssetInfos.Length > 0) loadCompleted(loadAssetInfos[0]);
                    else loadCompleted(null);
                };
            }

            return LoadAssetsAsync(assetPaths, func);
        }

        /// <summary>
        /// 异步加载资源，通过资源路径（多个）
        /// </summary>
        /// <param name="assetPaths">资源地址</param>
        /// <param name="loadCompleted">完成回调函数</param>
        /// <returns>加载请求信息</returns>
        public LoadAssetsRequest LoadAssetsAsync(string[] assetPaths, Action<LoadAssetInfo[]> loadCompleted)
        {
            var loadAssetsRequest = CreateLoadAssetsRequest(assetPaths, loadCompleted);

            if (loadAssetsRequest == null)
            {
                if (loadCompleted != null)
                {
                    loadCompleted(null);
                }
                return null;
            }

            StartCoroutine(LoadAssets_Routine(loadAssetsRequest));
            return loadAssetsRequest;
        }

        /// <summary>
        /// 协程加载资源（多个）
        /// </summary>
        /// <param name="loadAssetsRequest">加载请求信息</param>
        /// <returns>协程</returns>
        private IEnumerator LoadAssets_Routine(LoadAssetsRequest loadAssetsRequest)
        {
            for (int i = 0; i < loadAssetsRequest.loadAssetInfos.Length; i++)
            {
                var loadAssetInfo = loadAssetsRequest.loadAssetInfos[i];

                if (loadAssetInfo == null || loadAssetInfo.isLoadCompeleted)
                {
                    continue;
                }

                StartCoroutine(LoadAsset_Routine(loadAssetInfo));
            }

            while (!loadAssetsRequest.IsDone)
            {
                yield return null;
            }

            loadAssetsRequest.CallBack();
        }

        /// <summary>
        /// 协程加载资源
        /// </summary>
        /// <param name="loadAssetInfo">资源信息</param>
        /// <returns>协程</returns>
        private IEnumerator LoadAsset_Routine(LoadAssetInfo loadAssetInfo)
        {
            if (loadAssetInfo == null || loadAssetInfo.isLoadCompeleted)
            {
                yield break;
            }

            Type assetType = GetAssetTypeByPath(loadAssetInfo.assetPath);

            if (loadAssetInfo.assetStoreType == AssetStoreType.AssetBundle)
            {
                // Action<string, TAssetBundleType, Object> func = delegate (string path, TAssetBundleType t, Object asset)
                // {
                //     if (asset != null)
                //     {
                //         loadAssetInfo.SetAssetObject(asset, assetType);
                //         return;
                //     }
                loadAssetInfo.assetStoreType = AssetStoreType.Resources;
                // };
                // yield return AssetBundlesManager.GetInstance().LoadAssetBundleAsync(loadAssetInfo.assetPath, assetType, func);
            }

            if (loadAssetInfo.assetStoreType == AssetStoreType.Resources)
            {
                string path = GetResourcesPath(loadAssetInfo.assetPath);

                var request = Resources.LoadAsync(path, assetType);
                yield return request;
                loadAssetInfo.SetAssetObject(request.asset, assetType);
            }
        }

        /// <summary>
        /// 创建加载请求信息
        /// </summary>
        /// <param name="assetPaths">资源路径</param>
        /// <param name="loadCompleted">加载完成回调函数</param>
        /// <returns>加载请求信息</returns>
        private LoadAssetsRequest CreateLoadAssetsRequest(string[] assetPaths, Action<LoadAssetInfo[]> loadCompleted)
        {
            if (assetPaths == null)
            {
                return null;
            }

            var loadAssetsRequest = new LoadAssetsRequest();
            loadAssetsRequest.loadAssetInfos = new LoadAssetInfo[assetPaths.Length];
            loadAssetsRequest.loadCompleted = loadCompleted;

            LoadAssetInfo loadAssetInfo;
            for (int i = 0; i < assetPaths.Length; i++)
            {
                if (string.IsNullOrEmpty(assetPaths[i]))
                {
                    continue;
                }
                if (!AssetPathMap.TryGetValue(assetPaths[i], out loadAssetInfo))
                {
                    loadAssetInfo = new LoadAssetInfo(assetPaths[i]);
                    AssetPathMap[loadAssetInfo.assetPath] = loadAssetInfo;
                }
                loadAssetsRequest.loadAssetInfos[i] = loadAssetInfo;
            }

            return loadAssetsRequest;
        }
        #endregion

        #region 全局
        /// <summary>
        /// 获得Resources路径
        /// </summary>
        /// <param name="assetPath">资源路径</param>
        /// <returns>Resources路径</returns>
        public static string GetResourcesPath(string assetPath)
        {
            int index = assetPath.LastIndexOf("Resources");
            if (index != -1)
            {
                assetPath = assetPath.Substring(index + 10);
            }

            index = assetPath.LastIndexOf('.');
            if (index != -1)
            {
                assetPath = assetPath.Substring(0, index);
            }

            return assetPath;
        }

        /// <summary>
        /// 获得资源类型，根据资源路径
        /// </summary>
        /// <param name="assetPath">资源路径</param>
        /// <returns>资源类型</returns>
        public static Type GetAssetTypeByPath(string assetPath)
        {
            assetPath = assetPath.ToLower();
            if (assetPath.EndsWith(".prefab"))
            {
                return typeof(GameObject);
            }
            else if (assetPath.EndsWith(".asset"))
            {
                return typeof(ScriptableObject);
            }
            else if (assetPath.EndsWith(".wav") || assetPath.EndsWith(".mp3"))
            {
                return typeof(AudioClip);
            }
            else if (assetPath.EndsWith(".text") || assetPath.EndsWith(".json"))
            {
                return typeof(TextAsset);
            }
            else if (assetPath.EndsWith(".png") || assetPath.EndsWith(".jpg"))
            {
                return typeof(Texture);
            }
            else if (assetPath.EndsWith(".mat"))
            {
                return typeof(Material);
            }

            return typeof(Object);
        }

        /// <summary>
        /// 加载资源，通过资源路径
        /// </summary>
        /// <param name="assetPath">资源路径</param>
        /// <returns>资源</returns>
        public static T LoadAssetStatic<T>(string assetPath) where T : Object
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                return null;
            }

            var asset = AssetBundlesManager.Instance.LoadAssetBundle<T>(assetPath);
            if (asset == null)
            {
                string path = GetResourcesPath(assetPath);
                asset = Resources.Load<T>(path);
            }
            return asset;
        }
        #endregion

        #region 卸载
        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="loadAssetInfo">资源信息</param>
        public void UnLoadAsset(LoadAssetInfo loadAssetInfo)
        {
            if (loadAssetInfo.assetObjectType != typeof(GameObject))
            {
                Resources.UnloadAsset(loadAssetInfo.assetObject);
            }
            loadAssetInfo.assetObject = null;
            loadAssetInfo.isLoadCompeleted = false;
        }

        /// <summary>
        /// 卸载所有资源
        /// </summary>
        public void UnLoadAllAssets()
        {
            foreach (var element in AssetPathMap)
            {
                UnLoadAsset(element.Value);
            }

            // AssetBundlesManager.GetInstance().ReleaseAllBundle(true);
        }
        #endregion
    }
}