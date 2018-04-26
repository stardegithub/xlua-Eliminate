
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameManager
{
    public enum AssetStoreType
    {
        Resources,
        AssetBundle,
        BinaryFile,//图片、声音（Unity5.2，Android播放Ogg、WP播放MP3出错）//TODO ZHAN
    }

    // public enum ObjectType
    // {
    //     Object,
    //     GameObject,
    //     TextAsset,
    //     Texture,
    //     Sprite,
    //     AudioClip,
    // }

    public class LoadAssetInfo
    {
        public string assetPath; //key

        public Object assetObject;

        public Type assetObjectType;

        public AssetStoreType assetStoreType;

        public AsyncOperation loadAsyncOperation;

        public bool isLoadCompeleted;

        public LoadAssetInfo(string assetPath)
        {
            this.assetPath = assetPath;
            assetStoreType = AssetStoreType.AssetBundle;
        }

        public void SetAssetObject(Object assetObject, Type assetObjectType)
        {
            this.assetObject = assetObject;
            this.assetObjectType = assetObjectType;
            isLoadCompeleted = true;
        }

        public void Reset()
        {
            assetObject = null;
            assetObjectType = null;
            isLoadCompeleted = false;
        }
    }

    public interface IProgress
    {
        bool IsDone { get; }
        float Progress { get; }
    }

    public class LoadAssetsRequest : IProgress
    {
        public float progress;
        public LoadAssetInfo[] loadAssetInfos;
        public Action<LoadAssetInfo[]> loadCompleted;

        public bool IsDone
        {
            get
            {
                return !Array.Exists(loadAssetInfos, c => c != null && !c.isLoadCompeleted);
            }
        }

        public float Progress
        {
            get
            {
                if (IsDone) return 1;

                float sum = 0;
                for (int i = 0; i < loadAssetInfos.Length; i++)
                {
                    if (loadAssetInfos[i] == null || loadAssetInfos[i].isLoadCompeleted)
                    {
                        sum++;
                    }
                    else if (loadAssetInfos[i].loadAsyncOperation != null)
                    {
                        sum += loadAssetInfos[i].loadAsyncOperation.progress;
                    }
                }
                return sum / loadAssetInfos.Length;
            }
        }

        public void CallBack()
        {
            if (loadCompleted != null)
            {
                loadCompleted(loadAssetInfos);
            }
        }
    }
}


[System.Serializable]
public class SpriteAssetConfig
{
    public string atlas;
    public string sprite;
}