# 概述

--------------------------------

## 目录

6.[加载](LoadAssetBundles.md#6%E5%8A%A0%E8%BD%BD)

6.1.[加载接口](LoadAssetBundles.md#61%E5%8A%A0%E8%BD%BD%E6%8E%A5%E5%8F%A3)

6.1.1.[同步加载接口](LoadAssetBundles.md#611%E5%90%8C%E6%AD%A5%E5%8A%A0%E8%BD%BD%E6%8E%A5%E5%8F%A3)

6.1.2.[异步加载接口](LoadAssetBundles.md#612%E5%BC%82%E6%AD%A5%E5%8A%A0%E8%BD%BD%E6%8E%A5%E5%8F%A3)

--------------------------------

## 6.加载

[返回目录](README.md#%E7%9B%AE%E5%BD%95)

流程如下：
打包 -> 上传 -> 下载 -> 加载

`详见：图 6-1`

`图 6-1 加载流程图`
![图 6-1](ReadMe/Load/Flow.png)

`说明事项`

> * 加载优先级。
>     - 下载资源 > 本地资源（`如果两者都没有，则视为加载失败`）
>     - 下载资源依赖列表。`详见`:[3.4.打包依赖Map](AssetBundle.md#34%E6%89%93%E5%8C%85%E4%BE%9D%E8%B5%96map)
> * 加载分同步接口和异步接口。`详见`:[6.1.加载接口](LoadAssetBundles.md#61%E5%8A%A0%E8%BD%BD%E6%8E%A5%E5%8F%A3)

### 6.1.加载接口

[返回目录](README.md#%E7%9B%AE%E5%BD%95)

加载分同步接口和异步接口。

### 6.1.1.同步加载接口

[返回目录](README.md#%E7%9B%AE%E5%BD%95)

同步加载接口`详见: 表 6-1-1-1`

`表 6-1-1-1 同步加载接口`

| 函数名 | 说明 | 参数列表 | 参数说明 | 返回值 | 备注 |
|:------|:-----|:--------|:-------|:-------|:----|
| bool LoadScene(string iSceneName) | 场景加载 | * iSceneName | * 场景名 | bool : 加载是否成功 | - |
| bool LoadPrefab(string iPrefabName, GameObject iParent,  Vector3 iPosition, Vector3 iScale) | 预制体加载 | * iPrefabName <BR/> * iParent <BR/> * iPosition <BR/> * iScale | * 预制体名 <BR/> * 预挂接的父对象 <BR/> * 坐标  <BR/> * 缩放 | bool : 加载是否成功 | - |
| UnityEngine.Object LoadPreFab(string iPrefabName) | 预制体加载 | * iPrefabName | * 预制体名 | UnityEngine.Object : 加载得到的预制体对象 | - | 
| AudioClip LoadAudio(string iAudioName) | 加载音效（无需后缀） | * iAudioName | * 音效名 | AudioClip : 加载得到的音效对象 | - | 
| Material LoadMaterial(string iMatName) | 加载材质（无需后缀） | * iMatName | * 材质名 | Material : 加载得到的材质对象 | - | 
| Texture2D LoadTexture(string iTextureName) | 加载纹理 | * iTextureName | * 纹理名 | Texture2D : 加载得到的纹理对象 | - | 
| UnityEngine.TextAsset LoadAssetFile(string iAssetFileName) | 加载Asset文件(.asset) | * iAssetFileName | * Asset文件名 | UnityEngine.TextAsset : 加载得到的Asset文件对象 | - | 
| string LoadTextFile(string iTextFileName) | 加载文本文件(.text) | * iTextFileName | * 文本文件名 | string : 文本文件内容 | - |
| string LoadJsonFile(string iJsonFileName) | 加载Json文件(.json) | * iJsonFileName | * Json文件名 | string : Json文件内容 | - |
| T LoadJsonFile<T>(string iJsonFileName) | 加载Json文件(.json)，并转化成指定的对象类型T | * iJsonFileName | * Json文件名 | T : Json文件对应的对象 | `Unity3d 5.0之后支持，Json文件跟对象的直接转换` <BR/> `Json -> Object`:JsonUtility.FromJson <BR/> `Object -> Json`:JsonUtility.ToJson |
| UnityEngine.Object LoadAssetBundle(string iFilePath) | 加载asset bundle | * iFilePath | * asset bundle 文件路径 | UnityEngine.Object : asset bundle 文件对象 | - |
| T LoadAssetBundle<T>(string iFilePath) where T : UnityEngine.Object | 加载asset bundle | * iFilePath | * asset bundle 文件路径 | UnityEngine.Object : asset bundle 文件对象 | T : 欲加载的资源类型 |
| T LoadFromAssetBundle<T>(string iKey, TAssetBundleType iType = TAssetBundleType.None) where T : UnityEngine.Object | 加载AssetBundle | * iKey <BR/> * iType | * Key <BR/> * 加载资源类型 | T : 欲加载的资源类型的对象 | - |

### 6.1.2.异步加载接口

[返回目录](README.md#%E7%9B%AE%E5%BD%95)

异步加载接口`详见: 表 6-1-2-1`

`表 6-1-2-1 异步加载接口`

| 函数名 | 说明 | 参数列表 | 参数说明 | 返回值 | 备注 |
|:------|:-----|:--------|:-------|:-------|:----|
| IEnumerator LoadPrefabsAsync(string[] iPrefabNames, <BR/>Action<bool, string, TAssetBundleType, GameObject> iLoadCompleted) | 加载预制体(多个) | * iPrefabNames <BR/> * iLoadCompleted | * 预制体列表 <BR/> * 加载完成回调 | - | - |
| IEnumerator LoadPrefabAsync(string iPrefabName, <BR/>Action<bool, string, TAssetBundleType, GameObject> iLoadCompleted, <BR/>Func<string, System.Action<bool, string, TAssetBundleType, GameObject>, IEnumerator> iLoadFailed = null) | 加载预制体（单个） | * iPrefabName <BR/> * iLoadCompleted <BR/> * iLoadFailed | * 预制体名 <BR/> * 加载完成回调 <BR/> * 加载失败回调（默认为null） | - | - | 
| IEnumerator LoadAudiosAsync(string[] iAudioNames, <BR/>Action<bool, string, TAssetBundleType, AudioClip> iLoadCompleted, <BR/>Func<string, System.Action<bool, string, TAssetBundleType, AudioClip>, IEnumerator> iLoadFailed = null) | 加载音效（多个）| * iAudioNames <BR/> * iLoadCompleted <BR/> * iLoadFailed | * 音效列表 <BR/> * 加载完成回调 <BR/> * 加载失败回调（默认为null） | - | - | 
| IEnumerator LoadAudioAsync(string iAudioName, <BR/>Action<bool, string, TAssetBundleType, AudioClip> iLoadCompleted, <BR/>Func<string, System.Action<bool, string, TAssetBundleType, AudioClip>, IEnumerator> iLoadFailed = null) | 加载音效（单个）| * iAudioName <BR/> * iLoadCompleted <BR/> * iLoadFailed | * 音效名 <BR/> * 加载完成回调 <BR/> * 加载失败回调（默认为null） | - | - | 
| IEnumerator LoadMatsAsync(string[] iMatNames, <BR/>Action<bool, string, TAssetBundleType, Material> iLoadCompleted, <BR/>Func<string, System.Action<bool, string, TAssetBundleType, Material>, IEnumerator> iLoadFailed = null) | 加载材质（多个）| * iMatNames <BR/> * iLoadCompleted <BR/> * iLoadFailed | * 材质列表 <BR/> * 加载完成回调 <BR/> * 加载失败回调（默认为null） | - | - | 
| IEnumerator LoadMatAsync(string iMatName, <BR/>Action<bool, string, TAssetBundleType, Material> iLoadCompleted, <BR/>Func<string, System.Action<bool, string, TAssetBundleType, Material>, IEnumerator> iLoadFailed = null) | 加载材质（单个）| * iMatName <BR/> * iLoadCompleted <BR/> * iLoadFailed | * 材质名 <BR/> * 加载完成回调 <BR/> * 加载失败回调（默认为null） | - | - | 
| IEnumerator LoadTexturesAsync(string[] iTextureNames, <BR/>Action<bool, string, TAssetBundleType, Texture> iLoadCompleted, <BR/>Func<string, System.Action<bool, string, TAssetBundleType, Texture>, IEnumerator> iLoadFailed = null)  |  加载纹理（多个）| * iTextureNames <BR/> * iLoadCompleted <BR/> * iLoadFailed | * 纹理列表 <BR/> * 加载完成回调 <BR/> * 加载失败回调（默认为null） | - | - | 
| IEnumerator LoadTextureAsync(string iTextureName, <BR/>Action<bool, string, TAssetBundleType, Texture> iLoadCompleted, <BR/>Func<string, System.Action<bool, string, TAssetBundleType, Texture>, IEnumerator> iLoadFailed = null)  |  加载纹理（单个）| * iTextureName <BR/> * iLoadCompleted <BR/> * iLoadFailed | * 纹理名 <BR/> * 加载完成回调 <BR/> * 加载失败回调（默认为null） | - | - | 
| IEnumerator LoadAssetFilesAsync(string[] iAssetFileNames, <BR/>Action<bool, string, TAssetBundleType, TextAsset> iLoadCompleted, <BR/>Func<string, System.Action<bool, string, TAssetBundleType, TextAsset>, IEnumerator> iLoadFailed = null)  |  加载(.asset)文件（多个） | * iAssetFileNames <BR/> * iLoadCompleted <BR/> * iLoadFailed | * (.asset)文件列表 <BR/> * 加载完成回调 <BR/> * 加载失败回调（默认为null） | - | - | 
| IEnumerator LoadAssetFileAsync(string iAssetFileName, <BR/>Action<bool, string, TAssetBundleType, TextAsset> iLoadCompleted, <BR/>Func<string, System.Action<bool, string, TAssetBundleType, TextAsset>, IEnumerator> iLoadFailed = null)  |  加载(.asset)文件（多个） | * iAssetFileName <BR/> * iLoadCompleted <BR/> * iLoadFailed | * (.asset)文件名 <BR/> * 加载完成回调 <BR/> * 加载失败回调（默认为null） | - | - | 
| IEnumerator LoadTextFilesAsync(string[] iTextFileNames, <BR/>Action<bool, string, TAssetBundleType, TextAsset> iLoadCompleted, <BR/>Func<string, System.Action<bool, string, TAssetBundleType, TextAsset>, IEnumerator> iLoadFailed = null)  |  加载(.text)文件（多个） | * iTextFileNames <BR/> * iLoadCompleted <BR/> * iLoadFailed | * (.text)文件列表 <BR/> * 加载完成回调 <BR/> * 加载失败回调（默认为null） | - | - | 
| IEnumerator LoadTextFileAsync(string iTextFileName, <BR/>Action<bool, string, TAssetBundleType, TextAsset> iLoadCompleted, <BR/>Func<string, System.Action<bool, string, TAssetBundleType, TextAsset>, IEnumerator> iLoadFailed = null)  |  加载(.text)文件（多个） | * iTextFileName <BR/> * iLoadCompleted <BR/> * iLoadFailed | * (.text)文件名 <BR/> * 加载完成回调 <BR/> * 加载失败回调（默认为null） | - | - | 
| IEnumerator LoadJsonFilesAsync(string[] iJsonFileNames, <BR/>Action<bool, string, TAssetBundleType, TextAsset> iLoadCompleted, <BR/>Func<string, System.Action<bool, string, TAssetBundleType, TextAsset>, IEnumerator> iLoadFailed = null)  |  加载(.json)文件（多个） | * iJsonFileNames <BR/> * iLoadCompleted <BR/> * iLoadFailed | * (.json)文件列表 <BR/> * 加载完成回调 <BR/> * 加载失败回调（默认为null） | - | - | 
| IEnumerator LoadJsonFileAsync(string iJsonFileName, <BR/>Action<bool, string, TAssetBundleType, TextAsset> iLoadCompleted, <BR/>Func<string, System.Action<bool, string, TAssetBundleType, TextAsset>, IEnumerator> iLoadFailed = null)  |  加载(.json)文件（多个） | * iTextFileName <BR/> * iLoadCompleted <BR/> * iLoadFailed | * (.json)文件名 <BR/> * 加载完成回调 <BR/> * 加载失败回调（默认为null） | - | - | 
| IEnumerator LoadAssetBundlesAsync(string[] iFilePaths, <BR/>Action<bool, string, TAssetBundleType, UnityEngine.Object> iLoadCompleted, <BR/>Func<string, System.Action<bool, string, TAssetBundleType, UnityEngine.Object>, IEnumerator> iLoadFailed = null)  | 加载asset bundle(多个) | * iFilePaths <BR/> * iLoadCompleted <BR/> * iLoadFailed | * 文件路径列表 <BR/> * 加载完成回调 <BR/> * 加载失败回调（默认为null） | - | - | 
| IEnumerator LoadAssetBundleAsync(string iFilePath, <BR/>Action<bool, string, TAssetBundleType, UnityEngine.Object> iLoadCompleted, <BR/>Func<string, System.Action<bool, string, TAssetBundleType, UnityEngine.Object>, IEnumerator> iLoadFailed = null)  | 加载asset bundle(单个) | * iFilePath <BR/> * iLoadCompleted <BR/> * iLoadFailed | * 文件路径 <BR/> * 加载完成回调 <BR/> * 加载失败回调（默认为null） | - | - | 
| IEnumerator LoadFromAssetBundleAsync<T>(string iKey, <BR/>Action<bool, string, TAssetBundleType, T> iLoadCompleted, <BR/>Func<string, System.Action<bool, string, TAssetBundleType, T>, IEnumerator> iLoadFailed, <BR/>TAssetBundleType iType = TAssetBundleType.None) where T : UnityEngine.Object | 加载AssetBundle | * iKey <BR/> * iLoadCompleted <BR/> * iLoadFailed <BR/> * iType | * 文件路径 <BR/> * 加载完成回调 <BR/> * 加载失败回调 <BR/> * 加载资源类型 | T : 欲加载的资源类型的对象 | - |