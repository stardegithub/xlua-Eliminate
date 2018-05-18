using UnityEngine;
using System.Xml;
using System;
using System.Collections;
using BuildSystem;
using Common;

namespace AndroidSDK {

	/// <summary>
	/// Strings XML base.
	/// </summary>
	public class StringsXMLBase : XmlDocument {

		/// <summary>
		/// 保存地址.
		/// </summary>
		private string SavePath {
			get;
			set;
		}

		/// <summary>
		/// 加载res/values下的strings.XML对象.
		/// </summary>
		/// <returns>strings.XML对象.</returns>
		/// <param name="iPath">路径.</param>
		public static StringsXMLBase LoadXML(string iPath) {
			StringsXMLBase _xml = new StringsXMLBase ();
			if ((null != _xml) && (true == _xml.Init (iPath))) {
				return _xml;
			}
			return null;
		}

		/// <summary>
		/// 构造函数：禁止外部New.
		/// </summary>
		protected StringsXMLBase () {
			
		}

		/// <summary>
		/// 初始化.
		/// </summary>
		/// <param name="iPath">I path.</param>
		protected virtual bool Init(string iPath){

			try {
				
				// 保存路径
				this.SavePath = iPath;

				// 加载
				this.Load(iPath);

			} catch (Exception e) {
				UtilsLog.Exception ("StringsXMLBase", "Init():Failed!!! Exeption:{0}",
					e.Message);
				return false;
			}

			return true;
		}

		/// <summary>
		/// 添加字符串.
		/// </summary>
		/// <param name="iName">名字.</param>
		/// <param name="iValue">值.</param>
		public void AddString(string iName, string iValue) {
			
			if(true == string.IsNullOrEmpty(iName)){
				return;
			}
		
			XmlNode rootNode = this.GetResourcesNode ();
			if (null == rootNode) {
				return;
			}

			XmlNodeList list = rootNode.SelectNodes ("string");
			XmlElement child = null;
			foreach (XmlNode node in list) {
				XmlElement _node = node as XmlElement;
				if (null == _node) {
					continue;
				}
				string name = _node.GetAttribute("name");
				if ((false == string.IsNullOrEmpty(name)) && 
					(true == name.Equals (iName))) {
					child = _node;
					break;
				}
			}
			if (null == child) {
				child = this.CreateElement ("string");
				rootNode.AppendChild (child);
				child.SetAttribute ("name", iName);
			}
			child.InnerText = iValue;
		}
			
#region XML

		public XmlNode GetResourcesNode() {
			return SelectSingleNode("/resources");
		}

		/// <summary>
		/// 保存文件.
		/// </summary>
		public void Save() {
			if (true == string.IsNullOrEmpty (this.SavePath)) {
				return;
			}
			this.Save (this.SavePath);
		}

#endregion

	}

}
