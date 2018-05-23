using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using BuildSystem;
using Common;

namespace AssetBundles {

	/// <summary>
	/// 打包模式.
	/// </summary>
	public enum BundleMode
	{
		/// <summary>
		/// 无.
		/// </summary>
		None,
		/// <summary>
		/// 单一目录.
		/// </summary>
		OneDir = 1,
		/// <summary>
		/// 文件一对一.
		/// </summary>
		FileOneToOne = 2,
		/// <summary>
		/// 顶层目录一对一.
		/// </summary>
		TopDirOneToOne = 3,
		/// <summary>
		/// 场景一对一.
		/// </summary>
		SceneOneToOne = 4,
	}

	/// <summary>
	/// 匹配目录信息.
	/// </summary>
	public class MatchDirInfo {
		/// <summary>
		/// 前一个.
		/// </summary>
		public MatchDirInfo Prev = null;
		/// <summary>
		/// 后一个.
		/// </summary>
		public MatchDirInfo Next = null;

		/// <summary>
		/// 名字.
		/// </summary>
		public string Name = null;
	}

	/// <summary>
	/// 打包资源资源信息
	/// </summary>
	[System.Serializable]
	public class BundleResource : JsonDataBase
	{
		/// <summary>
		/// 路径.
		/// </summary>
		public string Path = null;
		/// <summary>
		/// 模式.
		/// </summary>
		public BundleMode Mode = BundleMode.OneDir;
		/// <summary>
		/// 忽略列表.
		/// </summary>
		public List<string> IgnoreList = new List<string>();

		/// <summary>
		/// 初始化.
		/// </summary>
		public override void Init() {
			base.Init ();
			Mode = BundleMode.OneDir;
		}

		/// <summary>
		/// 清空.
		/// </summary>
		public override void Clear() {
			base.Clear ();
			Path = null;
			Mode = BundleMode.None;
			IgnoreList.Clear ();
		}

		/// <summary>
		/// 判断忽略目标是否存在.
		/// </summary>
		/// <returns><c>true</c>,存在, <c>false</c> 不存在.</returns>
		/// <param name="iTarget">忽略目标.</param>
		public bool isIgnoreTargetExist(string iTarget) {

			bool isExist = false;
			if (null == IgnoreList) {
				IgnoreList = new List<string> ();
				return isExist;
			}
			if (0 >= IgnoreList.Count) {
				return isExist;
			}
			// 检测存不存在
			foreach (string ignore in IgnoreList) {
				if (false == iTarget.Equals (ignore)) {
					continue;
				}
				isExist = true;
				break;
			}

			return isExist;
		}

		/// <summary>
		/// 追加忽略目标.
		/// </summary>
		/// <param name="iTarget">忽略目标.</param>
		public void AddIgnoreTarget(string iTarget){
			// 文件夹的场合
			if(true == Directory.Exists(iTarget)) {
				DirectoryInfo _targetDir = new DirectoryInfo (iTarget);
				FileInfo[] _files = _targetDir.GetFiles ();
				DirectoryInfo _endDir = new DirectoryInfo (Application.dataPath);
				foreach (FileInfo file in _files) {
					// .DS_Store文件
					if(file.Name.EndsWith(".DS_Store") == true) {
						continue;
					}
					// *.meta文件
					if(file.Name.EndsWith(".meta") == true) {
						continue;
					}
					string _localFilePath = this.GetLocalFilePath (file, _endDir.Name);
					if (true == string.IsNullOrEmpty (_localFilePath)) {
						continue;
					}
					if (false == this.isIgnoreTargetExist (_localFilePath)) {
						this.IgnoreList.Add (_localFilePath);
					}
				}
			}

			if (true == File.Exists (iTarget)) {
				if (false == this.isIgnoreTargetExist (iTarget)) {
					this.IgnoreList.Add (iTarget);
				}
			}
		}

		/// <summary>
		/// 取得本地地址.
		/// </summary>
		/// <returns>本地地址.</returns>
		/// <param name="iFullPath">全路径.</param>
		/// <param name="iEndMark">结束位.</param>
		private string GetLocalFilePath(FileInfo iFileInfo, string iEndMark) {
			if (null == iFileInfo) {
				return null;
			}
			MatchDirInfo _matchedDirLink = this.CreateMatchDirLink (iFileInfo.Directory, iEndMark);
			string _localFilePath = null;
			while (null != _matchedDirLink) {
			
				if (true == string.IsNullOrEmpty (_localFilePath)) {
					_localFilePath = _matchedDirLink.Name;
				} else {
					_localFilePath = string.Format ("{0}/{1}", _localFilePath, _matchedDirLink.Name);
				}
				_matchedDirLink = _matchedDirLink.Next;
			}
			if (true == string.IsNullOrEmpty (_localFilePath)) {
				return null;
			}
			return string.Format ("{0}/{1}", _localFilePath, iFileInfo.Name);
		}

		/// <summary>
		/// 移除忽略目标.
		/// </summary>
		/// <param name="iTarget">忽略目标.</param>
		public void RemoveIgnoreTarget(string iTarget){
			// 文件夹的场合
			if (true == Directory.Exists (iTarget)) {
				DirectoryInfo _targetDir = new DirectoryInfo (iTarget);
				FileInfo[] _files = _targetDir.GetFiles ();
				DirectoryInfo _endDir = new DirectoryInfo (Application.dataPath);
				foreach (FileInfo file in _files) {
					// .DS_Store文件
					if(file.Name.EndsWith(".DS_Store") == true) {
						continue;
					}
					// *.meta文件
					if(file.Name.EndsWith(".meta") == true) {
						continue;
					}
					string _localFilePath = this.GetLocalFilePath (file, _endDir.Name);
					if (true == string.IsNullOrEmpty (_localFilePath)) {
						continue;
					}
					this.IgnoreList.Remove (_localFilePath);
				}
			}

			if (true == File.Exists (iTarget)) {
				this.IgnoreList.Remove (iTarget);
			}
		}

		/// <summary>
		/// 取得与目标匹配度（0.0f ~ 1.0f）.
		/// </summary>
		/// <returns>匹配度.</returns>
		/// <param name="iTarget">I target.</param>
		public float GetMatch(string iTarget) {
			float _match = 0.0f;
			if (true == string.IsNullOrEmpty (iTarget)) {
				return _match;
			}
			// 单一文件模式
			switch(this.Mode) {
			case BundleMode.OneDir:
			case BundleMode.TopDirOneToOne:
				{
					DirectoryInfo _selfDir = new DirectoryInfo (this.Path);
					DirectoryInfo _targetDir = new DirectoryInfo (iTarget);
					_match = this.GetMatchFromDir (_selfDir, _targetDir);
				}
				break;
			case BundleMode.FileOneToOne:
			case BundleMode.SceneOneToOne:
				{
					_match = (true == iTarget.Equals (this.Path)) ? 1.0f : 0.0f;
				}
				break;
			default:
				break;
			}

			return _match;
		}

		/// <summary>
		/// 取得与目标匹配度（0.0f ~ 1.0f）.
		/// </summary>
		/// <returns>与目标匹配度（0.0f ~ 1.0f）.</returns>
		/// <param name="iSelf">自身.</param>
		/// <param name="iTarget">目标.</param>
		private float GetMatchFromDir(DirectoryInfo iSelf, DirectoryInfo iTarget) {
			float _match = 0.0f;
			if ((null == iSelf) || (null == iTarget)) {
				return _match;
			}

			DirectoryInfo endDir = new DirectoryInfo (Application.dataPath);

			MatchDirInfo _selfMatch = this.CreateMatchDirLink (iSelf, endDir.Name);
			MatchDirInfo _targetMatch = this.CreateMatchDirLink (iTarget, endDir.Name);
			if ((null == _selfMatch) || (null == _selfMatch)) {
				return _match;
			}

			// 计算匹配度
			int maxCount = 0;
			int matchedCount = 0;
			while (null != _targetMatch) {

				if ((null != _targetMatch) &&
					(null != _selfMatch) &&
					(true == _targetMatch.Name.Equals (_selfMatch.Name))) {
					++matchedCount;
				}
				if (null != _targetMatch) {
					_targetMatch = _targetMatch.Next;
				}
				if (null != _selfMatch) {
					_selfMatch = _selfMatch.Next;
				}
				++maxCount;
			}
			_match = (0 >= maxCount) ? 0.0f : ((float)matchedCount / (float)maxCount);
			return _match;
		}

		/// <summary>
		/// 创建目录匹配链表.
		/// </summary>
		/// <returns>目录匹配链表.</returns>
		/// <param name="iDirInfo">目录信息.</param>
		/// <param name="iEndMark">结束标识.</param>
		private MatchDirInfo CreateMatchDirLink(DirectoryInfo iDirInfo, string iEndMark) {

			if (null == iDirInfo) {
				return null;
			}
			if (true == string.IsNullOrEmpty(iEndMark)) {
				return null;
			}

			MatchDirInfo _curDir = new MatchDirInfo ();
			_curDir.Name = iDirInfo.Name;

			DirectoryInfo _parent = iDirInfo.Parent;
			while(null != _parent) {
				
				MatchDirInfo _prevDir = new MatchDirInfo ();
				_prevDir.Name = _parent.Name;
				_curDir.Prev = _prevDir;
				_prevDir.Next = _curDir;

				if (true == iEndMark.Equals (_parent.Name)) {
					_parent = null;
				} else {
					_parent = _parent.Parent;
				}
				_curDir = _prevDir;
			}

			return _curDir;
		}
	}

	/// <summary>
	/// 非资源信息.
	/// </summary>
	[System.Serializable]
	public class BundleUnResource : JsonDataBase
	{
		/// <summary>
		/// 路径.
		/// </summary>
		public string Path = null;

		/// <summary>
		/// 清空.
		/// </summary>
		public override void Clear() {
			base.Clear ();
			Path = null;
		}
	}

	/// <summary>
	/// 资源匹配类.
	/// </summary>
	public class MatchResource : JsonDataBase {
		/// <summary>
		/// 匹配度.
		/// </summary>
		public float Match = 0.0f;
		/// <summary>
		/// 目标.
		/// </summary>
		public BundleResource Target = null;
	}

	/// <summary>
	/// Bundles配置数据.
	/// </summary>
	[System.Serializable]
	public class BundlesConfigData : JsonDataBase {

		/// <summary>
		/// 资源列表.
		/// </summary>
		public List<BundleResource> Resources = new List<BundleResource>();

		/// <summary>
		/// 非资源列表.
		/// </summary>
		public List<BundleUnResource> UnResources = new List<BundleUnResource>();

		/// <summary>
		/// 清空.
		/// </summary>
		public override void Clear() {
			base.Clear ();
			Resources.Clear ();
			UnResources.Clear ();
		}

		/// <summary>
		/// 追加资源.
		/// </summary>
		/// <param name="iMode">打包模式.</param>
		/// <param name="iResourePath">资源路径.</param>
		/// <param name="iIgnoreList">忽略列表.</param>
		public BundleResource AddResource(
			BundleMode iMode, 
			string iResourcePath,
			List<string> iIgnoreList) {

			if (true == string.IsNullOrEmpty (iResourcePath)) {
				return null;
			}

			BundleResource target = null;
			foreach (BundleResource loop in Resources) {
				if (false == iResourcePath.Equals (loop)) {
					continue;
				}
				target = loop;
				break;
			}

			// 不存在
			if (null == target) {
				target = new BundleResource ();
				this.Resources.Add (target);
				target.Mode = iMode;
				target.Path = iResourcePath;
				target.IgnoreList = iIgnoreList;
			} else {

				target.Mode = iMode;
				if ((null != iIgnoreList) && (1 <= iIgnoreList.Count)) {
					foreach (string ignore in iIgnoreList) {
						target.AddIgnoreTarget (ignore);
					}
				}
			}
			if (null == target) {
				this.Error ("AddResource():Invalid target!!(Mode:{0} Path:{1})", 
					iMode, iResourcePath);
			}
			return target;
		}

		/// <summary>
		/// 追加资源.
		/// </summary>
		/// <param name="iResourePath">资源路径.</param>
		public bool RemoveResource(
			string iResourcePath) {

			if (true == string.IsNullOrEmpty (iResourcePath)) {
				return false;
			}

			foreach (BundleResource loop in Resources) {
				if (false == iResourcePath.Equals (loop)) {
					continue;
				}
				this.Resources.Remove(loop);
				break;
			}
			return true;
		}

		/// <summary>
		/// 追加忽略目标.
		/// </summary>
		/// <param name="iIgnoreTarget">忽略目标.</param>
		public BundleResource AddIgnoreTarget(string iIgnoreTarget) {
			BundleResource target = this.GetMatchTarget (iIgnoreTarget);
			if (null != target) {
				target.AddIgnoreTarget (iIgnoreTarget);
			} else {
				this.Error ("AddIgnorTarget():No match target for ignore!!(Ignore Target:{0})", 
					iIgnoreTarget);
			}
			return target;
		}

		/// <summary>
		/// 移除忽略目标.
		/// </summary>
		/// <param name="iIgnoreTarget">忽略目标.</param>
		public void RemoveIgnoreTarget(string iIgnoreTarget) {
			BundleResource target = this.GetMatchTarget (iIgnoreTarget);
			if (null != target ) {
				target.RemoveIgnoreTarget (iIgnoreTarget);
			} else {
				this.Error ("RemoveIgnoreTarget():No match target for ignore!!(Ignore Target:{0})", 
					iIgnoreTarget);
			}
		}

		/// <summary>
		/// 清空目标忽略列表.
		/// </summary>
		/// <param name="iIgnoreTarget">忽略目标.</param>
		public void ClearIgnore(string iIgnoreTarget) {
			BundleResource target = this.GetMatchTarget (iIgnoreTarget);
			if (null != target) {
				target.IgnoreList.Clear ();
			} else {
				this.Error ("ClearIgnore():No match target for ignore!!(Ignore Target:{0})", 
					iIgnoreTarget);
			}
		}

		/// <summary>
		/// 取得匹配到的对象.
		/// </summary>
		/// <returns>匹配到的对象.</returns>
		/// <param name="iTarget">I target.</param>
		private BundleResource GetMatchTarget(string iTarget) {

			List<MatchResource> _matches = new List<MatchResource> ();
			foreach (BundleResource target in Resources) {
				MatchResource _match = new MatchResource ();
				_matches.Add (_match);

				_match.Match = target.GetMatch (iTarget);
				_match.Target = target;
			}

			MatchResource[] matches = _matches
				.Where(o => (0.0f < o.Match))
				.OrderByDescending(o => o.Match)
				.ToArray ();
			if ((null == matches) || (0 >= matches.Length)) {
				this.Error ("GetMatchTarget():No target matched!!(Target:{0})", 
					iTarget);
				return null;
			}
			return (matches[0].Target);
		}
	}

	/// <summary>
	/// 资源打包配置信息.
	/// </summary>
	[System.Serializable]
	public class BundlesConfig : AssetBase<BundlesConfig, BundlesConfigData> {

		/// <summary>
		/// 资源列表.
		/// </summary>
		public List<BundleResource> Resources {
			get { 
				if (null != this.Data) {
					return this.Data.Resources;
				}
				return null;
			}
			set { 
				if (null != this.Data) {
					this.Data.Resources = value;
				}
			}
		}

		/// <summary>
		/// 非资源列表.
		/// </summary>
		public List<BundleUnResource> UnResources {
			get { 
				if (null != this.Data) {
					return this.Data.UnResources;
				}
				return null;
			}
			set { 
				if (null != this.Data) {
					this.Data.UnResources = value;
				}
			}
		}

		#region Resource

		/// <summary>
		/// 添加资源信息.
		/// </summary>
		/// <param name="iResourceInfo">资源信息.</param>
		public BundleResource AddResource(BundleResource iResourceInfo) {
			if (null == this.Data ) {
				return null;
			}
			if (null == iResourceInfo ) {
				return null;
			}
			return this.Data.AddResource (
				iResourceInfo.Mode, iResourceInfo.Path, 
				iResourceInfo.IgnoreList);
		}

		/// <summary>
		/// 添加资源信息.
		/// </summary>
		/// <param name="iMode">打包模式.</param>
		/// <param name="iResourePath">资源路径.</param>
		/// <param name="iIgnoreList">忽略列表.</param>
		public BundleResource AddResource(
			BundleMode iMode, string iResourcePath, 
			List<string> iIgnoreList = null) {
		
			if (null == this.Data ) {
				return null;
			}

			if (true == string.IsNullOrEmpty (iResourcePath)) {
				return null;
			}
			return this.Data.AddResource(iMode, iResourcePath, iIgnoreList);
		}

		/// <summary>
		/// 移除资源信息.
		/// </summary>
		/// <returns><c>true</c>, 移除成功, <c>false</c> 移除失败.</returns>
		/// <param name="iResourcePath">资源路径.</param>
		public bool RemoveResource(string iResourcePath) {
			if (null == this.Data ) {
				return false;
			}

			if (true == string.IsNullOrEmpty (iResourcePath)) {
				return false;
			}
			return this.Data.RemoveResource(iResourcePath);
		}

		/// <summary>
		/// 清空资源列表.
		/// </summary>
		public void ClearResources() {
			if (this.Resources == null) {
				return;
			}
			this.Resources.Clear ();

			UtilsAsset.SetAssetDirty (this);
		}

		/// <summary>
		/// 添加忽略列表.
		/// </summary>
		/// <returns>资源信息.</returns>
		/// <param name="iResourePath">资源路径.</param>
		/// <param name="iIgnoreTarget">忽略对象.</param>
		public BundleResource AddIgnoreTarget(
			string iResourcePath, 
			string iIgnoreTarget)
		{
			if (null == this.Data ) {
				return null;
			}

			if (true == string.IsNullOrEmpty (iResourcePath)) {
				return null;
			}

			if (true == string.IsNullOrEmpty (iIgnoreTarget)) {
				return null;
			}
		
			return this.Data.AddIgnoreTarget(iIgnoreTarget);
		}

		/// <summary>
		/// 判断当前文件是否为指定目标的忽略文件.
		/// </summary>
		/// <returns><c>true</c>, 忽略文件, <c>false</c> 非忽略文件.</returns>
		/// <param name="iTarget">目标.</param>
		/// <param name="iTargetFile">目标文件.</param>
		public bool isIgnoreFile(BundleResource iTarget, string iTargetFile) {
			if ((null == iTarget) || 
				(null == iTarget.IgnoreList) || 
				(true == string.IsNullOrEmpty(iTargetFile)) ) {
				return false;
			}
			return iTarget.isIgnoreTargetExist(iTargetFile);
		}

		/// <summary>
		/// 清空指定资源对象的忽略列表.
		/// </summary>
		/// <param name="iTarget">目标.</param>
		/// <param name="iIgnoreInfo">忽略信息.</param>
		public void RemoveIgnoreInfo(string iTarget, string iIgnoreTarget) {
			if (null == this.Data) {
				return;
			}
			if (true == string.IsNullOrEmpty(iTarget)) {
				return;
			}
			if (true == string.IsNullOrEmpty(iIgnoreTarget)) {
				return;
			}
			this.Data.RemoveIgnoreTarget (iIgnoreTarget);
		}

		/// <summary>
		/// 清空指定资源对象的忽略列表.
		/// <param name="iTarget">目标.</param>
		/// </summary>
		public void ClearAllIgnoreInfo(string iTarget) {
			if (null == this.Data) {
				return;
			}
			this.Data.ClearIgnore (iTarget);
		}

		#endregion

		#region UnResource

		/// <summary>
		/// 添加非资源信息.
		/// </summary>
		/// <param name="iUnResourePath">非资源路径.</param>
		public BundleUnResource AddUnResource(BundleUnResource iUnResourceInfo) {
			if (iUnResourceInfo == null) {
				return null;
			}
			return AddUnResource(iUnResourceInfo.Path);
		}

		/// <summary>
		/// 添加非资源信息.
		/// </summary>
		/// <param name="iUnResourePath">非资源路径.</param>
		public BundleUnResource AddUnResource(string iUnResourcePath) {
			BundleUnResource bur = null;

			// 不存在存在
			if (this.isUnResoureExist (iUnResourcePath, out bur) == false) {
				bur = new BundleUnResource();
				this.UnResources.Add (bur);
			}
			if (bur != null) {
				bur.Path = iUnResourcePath;
				UtilsAsset.SetAssetDirty (this);
			} else {
				this.Error ("AddUnResource()::Failed!!!(Path:{0})",
					iUnResourcePath);
			}
			return bur;
		}

		/// <summary>
		/// 移除非资源信息.
		/// </summary>
		/// <returns><c>true</c>, 移除成功, <c>false</c> 移除失败.</returns>
		/// <param name="iUnResourcePath">非资源信息.</param>
		public bool RemoveUnResource(string iUnResourcePath) {
			BundleUnResource bur = null;
			// 不存在存在
			if (this.isUnResoureExist (iUnResourcePath, out bur) == true) {            
				if (bur != null) {
					this.UnResources.Remove (bur);
					UtilsAsset.SetAssetDirty (this);
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// 清空非资源列表.
		/// </summary>
		public void ClearUnResources() {
			if (this.UnResources == null) {
				return;
			}
			this.UnResources.Clear ();
			UtilsAsset.SetAssetDirty (this);
		}

		/// <summary>
		/// 判断非资源信息是否存在.
		/// </summary>
		/// <returns><c>true</c>, 村贼, <c>false</c> 不存在.</returns>
		/// <param name="iUnResourcePath">非资源路径.</param>
		/// <param name="iBur">非资源信息.</param>
		private bool isUnResoureExist(string iUnResourcePath, out BundleUnResource iBur) {

			bool bolRet = false;
			iBur = null;
			foreach(BundleUnResource bur in this.UnResources)
			{
				if(bur.Path == iUnResourcePath)
				{
					iBur = bur;
					bolRet = true;
					break;
				}
			}

			return bolRet;
		}

		#endregion

		#region Implement

		/// <summary>
		/// 用用数据.
		/// </summary>
		/// <param name="iData">数据.</param>
		/// <param name="iForceClear">强制清空标志位.</param>
		protected override void ApplyData (BundlesConfigData iData, bool iForceClear) {

			if (null == iData) {
				return;
			}

			// 清空
			if (true == iForceClear) {
				this.Clear ();
			}

			this.Data.Resources = iData.Resources;
			this.Data.UnResources = iData.UnResources;

			UtilsAsset.SetAssetDirty (this);
		}

		#endregion
	}

}