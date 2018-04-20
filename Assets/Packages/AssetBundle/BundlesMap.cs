using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using BuildSystem;
using Upload;
using Common;

namespace AssetBundles {

	/// <summary>
	/// T bundle type.
	/// </summary>
	public enum TBundleType {
		/// <summary>
		/// 一般.
		/// </summary>
		Normal,
		/// <summary>
		/// 场景.
		/// </summary>
		Scene
	}

	/// <summary>
	/// 资源包信息.
	/// </summary>
	[System.Serializable]
	public class BundleMap
	{
		/// <summary>
		/// ID.
		/// </summary>
		[SerializeField]public string ID = null;

		/// <summary>
		/// Bundle类型.
		/// </summary>
		[SerializeField]public TBundleType Type = TBundleType.Normal;

		/// <summary>
		/// 路径.
		/// </summary>
		[SerializeField]public string Path = null;

		/// <summary>
		/// 对象列表
		/// </summary>
		[SerializeField]public List<string> Targets = new List<string>();

		/// <summary>
		/// 添加文件.
		/// </summary>
		/// <param name="iFilePath">文件路径.</param>
		public void AddFile(string iFilePath) {
			if (string.IsNullOrEmpty (iFilePath) == true) {
				return;
			}
			if (Targets == null) {
				Targets = new List<string>();
			}
			foreach (string loop in Targets) {
				// 已经存在
				if (loop.Equals (iFilePath) == true) {
					return;
				}
			}
			this.Targets.Add (iFilePath);
		}

		/// <summary>
		/// 移除忽略文件.
		/// </summary>
		/// <param name="iFilePath">文件路径.</param>
		public void RemoveIgnorFile(string iFilePath) {
			if (string.IsNullOrEmpty (iFilePath) == true) {
				return;
			}
			if ((Targets == null) || (Targets.Count <= 0)) {
				return;
			}
			foreach (string loop in Targets) {
				if (loop.Equals (iFilePath) == true) {
					Targets.Remove (loop);
					break;
				}
			}
		}
	}

	/// <summary>
	/// 场景包信息.
	/// </summary>
	public class SceneBundleInfo {

		/// <summary>
		/// BundleId.
		/// </summary>
		public string BundleId = null;

		/// <summary>
		/// 目标文件一览
		/// </summary>
		public List<string> Targets = new List<string> ();

		public void init(string iBundleId) {
			this.BundleId = iBundleId;
		}

		public void AddTarget(string iScenePath) {
			if (string.IsNullOrEmpty (iScenePath) == true) {
				return;
			}
			bool isExist = false;
			foreach (string loop in this.Targets) {
				if (iScenePath.Equals (loop) == false) {
					continue;
				}
				isExist = true;
			}
			if (isExist == false) {
				this.Targets.Add (iScenePath);
			}
		}

		public string[] GetAllTargets() {
			if ((this.Targets == null) || (Targets.Count <= 0)) {
				return null;
			}
			return this.Targets.ToArray ();
		}


	}

	/// <summary>
	/// Bundles map data.
	/// </summary>
	[System.Serializable]
	public class BundlesMapData : JsonDataBase {

		/// <summary>
		/// 依赖关系列表.
		/// </summary>
		public List<BundleMap> Maps = new List<BundleMap> ();

		/// <summary>
		/// 清空.
		/// </summary>
		public override void Clear ()
		{
			Maps.Clear ();
		}
	}

	/// <summary>
	/// 资源包地图.
	/// </summary>
	[System.Serializable]
	public class BundlesMap : AssetBase<BundlesMap, BundlesMapData> {

		/// <summary>
		/// 依赖关系列表.
		/// </summary>
		public List<BundleMap> Maps {
			get { 
				if (null != this.Data) {
					return this.Data.Maps;
				}
				return null;
			}
			set { 
				if (null != this.Data) {
					this.Data.Maps = value;
				}
			}
		}

		/// <summary>
		/// 取得或者创建一个BundleMap.
		/// </summary>
		/// <returns>BundleMap.</returns>
		/// <param name="iBundleId">BundleId.</param>
		public BundleMap GetOrCreateBundlesMap(string iBundleId) {
		
			BundleMap objRet = null;
			if (this.isTargetExist (iBundleId, out objRet) == true) {
				if (objRet == null) {
					objRet = new BundleMap ();
				}
			} else {
				objRet = new BundleMap ();
			}
			return objRet;
		}

		/// <summary>
		/// 取得bundle ID.
		/// </summary>
		/// <returns>bundle名.</returns>
		/// <param name="iPath">路径.</param>
		public static string GetBundleID(string iPath)
		{
			string strResult = iPath;
			strResult = strResult.Replace("/", "_");
			strResult = strResult.Replace(".", "_");
			strResult = strResult.Replace(" ", "_");
			strResult = strResult.ToLower();
			return strResult;
		}

		/// <summary>
		/// 判断目标存不存在.
		/// </summary>
		/// <returns><c>true</c>, 存在, <c>false</c> 不存在.</returns>
		/// <param name="iTargetID">目标ID.</param>
		/// <param name="iTarget">目标.</param>
		private bool isTargetExist(string iTargetID, out BundleMap iTarget) {
			iTarget = null;
			foreach (BundleMap loop in this.Maps) {
				if (loop.ID.Equals (iTargetID) == true) {
					iTarget = loop;
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 目标是否改变了.
		/// </summary>
		/// <returns><c>true</c>, 已改变, <c>false</c> 未改变.</returns>
		/// <param name="iNewTarget">新目标.</param>
		/// <param name="iExist">既存标志位.</param>
		private bool isTargetChanged(BundleMap iNewTarget, ref BundleMap iOldTarget) {
			iOldTarget = null;
			bool isExist = this.isTargetExist (iNewTarget.ID, out iOldTarget);
			if (true == isExist){
				
				// Type
				if(iOldTarget.Type != iNewTarget.Type) {
					return true;
				}
 
				// Path
				if(false == iOldTarget.Path.Equals(iNewTarget.Path)) {
					return true;
				}

				// Targets
				if(iOldTarget.Targets.Count != iNewTarget.Targets.Count) {
					return true;
				}
				foreach (string newPath in iNewTarget.Targets) {
					bool _isExist = false;
					foreach (string path in iOldTarget.Targets) { 
						if (true == newPath.Equals (path)) {
							_isExist = true;
							break;
						}
					}
					if (false == _isExist) {
						return true;
					}
				}
			} else {
				return true;
			}
			return false;
		}

		/// <summary>
		/// 合并目标.
		/// </summary>
		/// <param name="iNewTarget">I new target.</param>
		private void MergeTarget(BundleMap iNewTarget) {
			BundleMap _oldTarget = null;
			bool isChanged = this.isTargetChanged (iNewTarget, ref _oldTarget);
			if (true == isChanged) {
				if (null == _oldTarget) {
					this.Maps.Add (iNewTarget);
				} else {
					_oldTarget.Type = iNewTarget.Type;
					_oldTarget.Path = iNewTarget.Path;

					foreach (string newPath in iNewTarget.Targets) {
						bool _isExist = false;
						foreach (string path in _oldTarget.Targets) { 
							if (true == newPath.Equals (path)) {
								_isExist = true;
								break;
							}
						}
						if (false == _isExist) {
							_oldTarget.Targets.Add(newPath);
						}
					}
				}
			}
		}
			
#if UNITY_EDITOR

		/// <summary>
		/// 取得所有打包对象（一般的AssetBundle）.
		/// </summary>
		/// <returns>取得所有对象.</returns>
		public AssetBundleBuild[] GetAllNormalBundleTargets() {

			BundleMap[] targets = this.Maps
				.Where (o => (TBundleType.Normal == o.Type))
				.OrderBy (o => o.ID)
				.ToArray ();
			if ((targets == null) || (targets.Length <= 0)) {
				return null;
			}
			AssetBundleBuild[] buildMap = new AssetBundleBuild[targets.Length];
			for(int i = 0; i < targets.Length; i++) {
				buildMap [i].assetBundleName = GetBundleFullName(targets[i].ID);
				buildMap [i].assetNames = targets[i].Targets.ToArray();
			}
			return buildMap;
		}

		/// <summary>
		/// 取得所有打包对象（scene的AssetBundle）.
		/// </summary>
		/// <returns>取得所有对象.</returns>
		public List<SceneBundleInfo> GetAllSceneBundleTargets() {

			BundleMap[] targets = this.Maps
				.Where (o => (TBundleType.Scene == o.Type))
				.OrderBy (o => o.ID)
				.ToArray ();
			if ((targets == null) || (targets.Length <= 0)) {
				return null;
			}

			List<SceneBundleInfo> scenesInfo = new List<SceneBundleInfo>();
			foreach (BundleMap loop in targets) {
				SceneBundleInfo sceneInfo = new SceneBundleInfo ();
				sceneInfo.init (loop.ID);
				foreach (string scene in loop.Targets) {
					sceneInfo.AddTarget (scene);
				}
				scenesInfo.Add (sceneInfo);
			}
			return scenesInfo;
		}

#endif
		/// <summary>
		/// 取得Bundle全名（包含后缀）.
		/// </summary>
		/// <returns>Bundle全名.</returns>
		/// <param name="iBundleId">Bundle ID.</param>
		public string GetBundleFullName(string iBundleId) {
			string strRet = iBundleId;
			string fileSuffix = UploadList.GetInstance ().FileSuffix;
			if (string.IsNullOrEmpty (fileSuffix) == false) {
				strRet = string.Format ("{0}.{1}", 
					strRet, fileSuffix);
			}
			return strRet;
		}

		/// <summary>
		/// 取得Bundle manifest 全名（包含后缀）.
		/// </summary>
		/// <returns>Bundle manifest 全名（包含后缀）.</returns>
		/// <param name="iBundleId">Bundle ID.</param>
		public string GetBundleManifestFullName(string iBundleId) {
			string strRet = this.GetBundleFullName(iBundleId);
			if (string.IsNullOrEmpty (strRet) == false) {
				return string.Format ("{0}.manifest", 
					strRet);
			} else {
				return null;
			}
		}

		/// <summary>
		/// 更新&生成上传列表信息.
		/// </summary>
		/// <param name="iBundleType">Bundle Type.</param>
		/// <param name="iHashCodes">HashCode列表（Unity3d打包生成）.</param>
		public void UpdateUploadList(TBundleType iBundleType, Dictionary<string, string> iHashCodes = null) {

			UploadList list = UploadList.GetInstance ();
			if (list == null) {
				return;
			}

			list.AppVersion = BuildInfo.GetInstance ().BuildVersion;
			list.CenterVersion = BuildInfo.GetInstance ().CenterVersion;

			// MainManifest
			if (TBundleType.Normal == iBundleType) {
				list.AddMainManifestAssetsTarget ();
			}

			// 遍历Bundles
			foreach (BundleMap loop in this.Maps) {

				if (loop.Type != iBundleType) {
					continue;
				}

				string hashCode = null;
				if (iHashCodes != null) {
					hashCode = this.GetHashCodeOfBundle (iHashCodes, loop.ID);
				}

				// Bundle
				list.AddTarget (loop, TUploadFileType.Bundle, hashCode);
				if ((true == list.ManifestUpload) && (TBundleType.Scene != loop.Type)) {
					// Manifest(Normal)
					list.AddTarget (loop, TUploadFileType.NormalManifest);
				}

			}

		}

		/// <summary>
		/// 取得Bundle的HashCode.
		/// </summary>
		/// <returns>Bundle的HashCode.</returns>
		/// <param name="iHashCodes">HashCode列表.</param>
		/// <param name="iBundleId">BundleId.</param>
		private string GetHashCodeOfBundle(Dictionary<string, string> iHashCodes, string iBundleId) {
			if ((iHashCodes == null) ||
			   (iHashCodes.Count <= 0)) {
				return null;
			}
			foreach (KeyValuePair<string,string> it in iHashCodes) {
				if (it.Key.Equals (iBundleId) == false) {
					continue;
				}
				return it.Value;
			}
			return null;
		}

		#region Implement

		/// <summary>
		/// 应用数据.
		/// </summary>
		/// <param name="iData">数据.</param>
		/// <param name="iForceClear">强制清空.</param>
		protected override void ApplyData(BundlesMapData iData, bool iForceClear) {
			if (null == iData) {
				return;
			}

			// 清空
			if(true == iForceClear) {
				this.Clear ();
			}
				
			// 添加资源信息
			foreach(BundleMap loop in iData.Maps) {
				// 合并目标
				this.MergeTarget (loop);
			}

			UtilsAsset.SetAssetDirty (this);

		}
			
		#endregion
	}
}
