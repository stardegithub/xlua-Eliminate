using UnityEngine;
using System;
using System.IO;
using System.Xml;
using System.Collections;
using Common;
using BuildSystem;

namespace AndroidSDK.Common.Manifest {

	/// <summary>
	/// AndroidManifest.xml中 <meta-data/>节点数据定义.
	/// </summary>
	[System.Serializable]
	public class MetaDataInfo : JsonDataBase {

		/// <summary>
		/// 属性：android:name.
		/// </summary>
		public string Name = null;

		/// <summary>
		/// 属性：android:value.
		/// </summary>
		public string Value = null;
					
		/// <summary>
		/// 清空.
		/// </summary>
		public override void Clear() {
			base.Clear ();
			Name = null;
			Value = null;
		}

		public override string ToString () {
			return string.Format ("{0} Name:{1} Value:{2}",
				base.ToString(), this.Name, this.Value);	
		}
	}

	/// <summary>
	/// 安卓配置文件
	/// </summary>
	public abstract class ManifestBase : XmlDocument {

		/// <summary>
		/// 目录路径.
		/// </summary>
		protected string Dir {
			get;
			set;
		}

		protected int MinSdkVersion {
			get;
			set;
		}
		protected int MaxSdkVersion {
			get;
			set;
		}
		protected int TargetSdkVersion {
			get;
			set;
		}
		protected string AndroidNameSpace {
			get;
			set;
		}

		/// <summary>
		/// The strings xml.
		/// </summary>
		protected StringsXMLBase _stringsXml {
			get;
			set;
		}

		/// <summary>
		/// 初始化.
		/// </summary>
		/// <param name="iPath">路径.</param>
		/// <param name="iGameName">游戏名.</param>
		public bool Init(string iPath, string iGameName){

			try {
				this.Load(iPath);

				// 创建strings.xml对象
				this._stringsXml = this.CreateStringsXml();
//				if(null == this._stringsXml) {
//					return false;
//				}

				// 初始化Appliction
				this.InitApplicationInfo();

				// 初始化SDK版本信息.
				if (null == this.InitSDKVersions()) {
					return false;
				}

				// 应用用户自定义数据
				this.ApplyUserData(iGameName);

			} catch (Exception e) {
				UtilsLog.Exception ("Init", "Failed!!! Exeption:{0}",
					e.Message);
				return false;
			}

			return true;
		}

		/// <summary>
		/// 初始化SDK版本信息.
		/// </summary>
		protected virtual XmlElement InitSDKVersions() {
			XmlNode manifestNode = GetManifestNode ();
			if (null == manifestNode) {
				//this.Error ("InitSDKVersions():The root node(manifest) is not exist or invalid in the file AndroidManifest.xml!!!");
				return null;
			}

			XmlNode useSDKNode = manifestNode.SelectSingleNode ("uses-sdk");
			if (null == useSDKNode) {
				//this.Error ("InitSDKVersions():The node(uses-sdk) is not exist or invalid in the file AndroidManifest.xml!!!");
				return null;
			}
			XmlElement _useSDKNode = useSDKNode as XmlElement;
			if (null == _useSDKNode) {
				//this.Error ("InitSDKVersions():The node(uses-sdk) is invalid!!!");
				return null;
			}
			return _useSDKNode;
		}

		/// <summary>
		/// 设定节点属性.
		/// </summary>
		/// <param name="iNode">节点.</param>
		/// <param name="iAttributeName">节点属性名.</param>
		/// <param name="iValue">属性值.</param>
		/// <param name="iPrefix">前缀.</param>
		protected void SetNodeAttribute(XmlElement iNode, string iAttributeName, string iValue, string iPrefix = "android") {

			if (null == iNode) {
				return;
			}
			string name = iAttributeName;
			if (false == string.IsNullOrEmpty (iPrefix)) {
				name = string.Format ("{0}:{1}", iPrefix, name);
			}
			XmlAttribute attribute = iNode.GetAttributeNode (name);
			if (null == attribute) {
				if (true == string.IsNullOrEmpty (iPrefix)) {
					attribute = this.CreateAttribute (iAttributeName);
				} else {
					attribute = this.CreateAttribute (iPrefix, iAttributeName, this.AndroidNameSpace);
				}
			}
			attribute.Value = iValue;
			iNode.Attributes.Append (attribute);
		}

		/// <summary>
		/// 取得Node指定的属性值.
		/// </summary>
		/// <returns>属性值.</returns>
		/// <param name="iNode">节点.</param>
		/// <param name="iName">属性名.</param>
		protected int GetNodeAttribute_i(XmlElement iNode, string iName) {
			string _value = iNode.GetAttribute (iName);
			if (true == string.IsNullOrEmpty (_value)) {
				return -1;
			}
			return Convert.ToInt32 (_value);
		}

		/// <summary>
		/// 取得Node指定的属性值.
		/// </summary>
		/// <returns>属性值.</returns>
		/// <param name="iNode">节点.</param>
		/// <param name="iName">属性名.</param>
		protected string GetNodeAttribute_s(XmlElement iNode, string iName) {
			string _value = iNode.GetAttribute (iName);
			if (true == string.IsNullOrEmpty (_value)) {
				return null;
			}
			return _value;
		}

		/// <summary>
		/// 添加用户数据.
		/// </summary>
		/// <param name="iParent">父节点名.</param>
		/// <param name="iName">I name.</param>
		/// <param name="iValue">I value.</param>
		protected XmlElement CreateNode(XmlNode iParent, string iTagName) {
			if (null == iParent) {
				return null;
			}
			XmlElement metaData = this.CreateElement (iTagName);
			if (null == metaData) {
				return null;
			}
			iParent.AppendChild (metaData);
			return metaData;
		}

		/// <summary>
		/// 添加用户数据自定义节点.
		/// </summary>
		/// <returns>用户数据自定义节点.</returns>
		/// <param name="iAttributeName">节点属性名.</param>
		/// <param name="iValue">值.</param>
		/// <param name="iIsStrings">是否定义在strings.xml中.</param>
		protected XmlElement AddUserDefineNode(string iAttributeName, string iValue, bool iIsStrings = true) {
			XmlNode parent = this.GetApplicationNode ();
			if (null == parent) {
				return null;
			}

			XmlNodeList list = parent.SelectNodes ("meta-data");
			XmlElement child = null;
			foreach (XmlNode node in list) {
				XmlElement _node = node as XmlElement;
				if (null == _node) {
					continue;
				}
				string name = _node.GetAttribute("name", this.AndroidNameSpace);
				if ((false == string.IsNullOrEmpty(name)) && 
					(true == name.Equals (iAttributeName))) {
					child = _node;
					break;
				}
			}
			if (null == child) {
				child = this.CreateNode (parent, "meta-data");
			}
			if (null != child) {
				this.SetNodeAttribute (child, "name", iAttributeName);
				if (false == iIsStrings) {
					this.SetNodeAttribute (child, "value", iValue);
				} else {
					this.SetNodeAttribute (child, "value", string.Format("@string/{0}", iAttributeName));

					// 设定值到strings.xml
					if(null != this._stringsXml) {
						this._stringsXml.AddString (iAttributeName, iValue);
					}
				}
			}
			return child;
		}

#region virtual

		/// <summary>
		/// 初始化Appliction.
		/// </summary>
		protected void InitApplicationInfo() {

			XmlNode manifestNode = GetManifestNode ();
			if (null == manifestNode) {
				return;
			}
			XmlElement _manifestNode = manifestNode as XmlElement;
			if (null == _manifestNode) {
				return;
			}
			// Android NameSpace
			this.AndroidNameSpace = this.GetNodeAttribute_s(_manifestNode, "xmlns:android");

			XmlNode appNode = this.GetApplicationNode ();
			if (null == appNode) {
				return;
			}
			XmlElement _appNode = appNode as XmlElement;
			if (null == _appNode) {
				return;
			}
			// Icon
			this.SetNodeAttribute (_appNode, "icon", "@drawable/app_icon");

			// debug
			if (TBuildMode.Debug == BuildInfo.GetInstance ().BuildMode) {
				this.SetNodeAttribute (_appNode, "debuggable", "true");
			} else {
				this.SetNodeAttribute (_appNode, "debuggable", "false");
			}
		}

		/// <summary>
		/// 应用包名.
		/// </summary>
		/// <param name="iPackageName">游戏包名.</param>
		public virtual void ApplyPackageName(string iPackageName) {
			XmlNode manifestNode = this.GetManifestNode ();
			if (null == manifestNode) {
				return;
			}
			XmlElement _manifestNode = manifestNode as XmlElement;
			if (null == _manifestNode) {
				return;
			}
			this.SetNodeAttribute (_manifestNode, "package", iPackageName, null);
		}

		/// <summary>
		/// 应用用户自定义数据.
		/// </summary>
		/// <param name="iGameName">游戏名.</param>
		protected virtual void ApplyUserData(string iGameName) {}

		/// <summary>
		/// 创建 strings xml.
		/// </summary>
		/// <returns>string的XML文件对象.</returns>
		protected virtual StringsXMLBase CreateStringsXml() { 

			string _filePath = this.GetStringsXMLPath ();
			if (false == File.Exists (_filePath)) {
				return null;
			}
			StringsXMLBase _stringsXmlTmp = StringsXMLBase.LoadXML(_filePath);
			if (null == _stringsXmlTmp) {
				return null;
			}
			return _stringsXmlTmp;
		}

#endregion

#region XML

		public XmlNode GetManifestNode() {
			return SelectSingleNode("/manifest");
		}

		public XmlNode GetApplicationNode() {
			XmlNode RootNode = this.GetManifestNode ();
			if (null == RootNode) {
				return null;
			}
			return RootNode.SelectSingleNode("application");
		}

#endregion

#region abstract

		/// <summary>
		/// 取得res/values目录下的strings.xml的文件路径.
		/// </summary>
		/// <returns>strings.xml文件路径.</returns>
		protected abstract string GetStringsXMLPath();

#endregion

	}

}
