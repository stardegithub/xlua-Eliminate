#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Xml;
using System.Collections;
using BuildSystem;
using AndroidSDK.Common.Manifest;
using AndroidSDK.Platforms.Huawei;

namespace AndroidSDK.Platforms.Huawei.Manifest {

	/// <summary>
	/// Huawei manifest类.
	/// </summary>
	public class HuaweiManifest : ManifestBase {

		/// <summary>
		/// 实例.
		/// </summary>
		private static HuaweiManifest _instance = null;

		/// <summary>
		/// 取得实例.
		/// </summary>
		/// <returns>实例.</returns>
		/// <param name="iHuaweiDir">华为路径.</param>
		/// <param name="iGameName">游戏名.</param>
		public static HuaweiManifest GetInstance(string iHuaweiDir, string iGameName = null) {
			try {
				if(_instance == null) {
					_instance = new HuaweiManifest ();
				}
				if(false == _instance.InitByHuaweiDir(iHuaweiDir, iGameName)) {
					_instance = null;
				}
			} catch (Exception e) {
				BuildLogger.LogException ("[HuaweiManifest Create Failed] Exeption : {0}",
					e.Message);
				_instance = null;
			}
			return _instance;
		}

		/// <summary>
		/// 初始化.
		/// </summary>
		/// <returns><c>true</c>, OK, <c>false</c> NG.</returns>
		/// <param name="iHuaweiDir">华为路径.</param>
		/// <param name="iGameName">游戏名.</param>
		public bool InitByHuaweiDir(string iHuaweiDir, string iGameName) {
			this.Dir = iHuaweiDir;

			string manifestPath = string.Format ("{0}/AndroidManifest.xml", iHuaweiDir);
			if (false == File.Exists (manifestPath)) {
				return false;
			}
			return this.Init (manifestPath, iGameName);
		}

		/// <summary>
		/// 初始化SDK版本信息.
		/// </summary>
		/// <returns>SDK版本信息节点.</returns>
		protected override XmlElement InitSDKVersions() {
			XmlElement _useSDKNode = base.InitSDKVersions ();
			if (null == _useSDKNode) {
				return null;
			}
			if (-1 >= HuaweiSDKSettings.GetInstance ().MinSdkVersion) {
				this.MinSdkVersion = GetNodeAttribute_i (_useSDKNode, "minSdkVersion");
			} else {
				this.MinSdkVersion = HuaweiSDKSettings.GetInstance ().MinSdkVersion;
				this.SetNodeAttribute (_useSDKNode, "minSdkVersion", this.MinSdkVersion.ToString());
			}
			if (-1 >= HuaweiSDKSettings.GetInstance ().MaxSdkVersion) {
				this.MaxSdkVersion = GetNodeAttribute_i (_useSDKNode, "android:maxSdkVersion");
			} else {
				this.MaxSdkVersion = HuaweiSDKSettings.GetInstance ().MaxSdkVersion;
				this.SetNodeAttribute (_useSDKNode, "maxSdkVersion", this.MaxSdkVersion.ToString());
			}
			if (-1 >= HuaweiSDKSettings.GetInstance ().TargetSdkVersion) {
				this.TargetSdkVersion = GetNodeAttribute_i (_useSDKNode, "android:targetSdkVersion");
			} else {
				this.TargetSdkVersion = HuaweiSDKSettings.GetInstance ().TargetSdkVersion;
				this.SetNodeAttribute (_useSDKNode, "targetSdkVersion", this.TargetSdkVersion.ToString());
			}
			return _useSDKNode;
		}
			
		/// <summary>
		/// 应用包名.
		/// </summary>
		/// <param name="iPackageName">游戏包名.</param>
		public override void ApplyPackageName(string iPackageName) {
			base.ApplyPackageName (iPackageName);

			// 游戏名

			// 注意：这个在targetSDK >= 24时，在游戏中必须申明，否则影响N版本下使用SDK安装华为游戏中心。
			// SDK安装华为游戏中心;如果targetSDK < 24, 则可以不配置Provider
			// 其中android:authorities里“游戏包名”必须要替换为游戏自己包名，否则会导致冲突，游戏无法安装！
			// 详细说明请参考SDK接口文档
			this.ApplyProviderNodeInfo (iPackageName);
		}

		public void ApplyProviderNodeInfo(string iPackageName) {
			if (24 > this.TargetSdkVersion) {
				return;
			}
			XmlNode applicationNode = this.GetApplicationNode ();
			if (null == applicationNode) {
				return;
			}
			XmlNode _old = applicationNode.SelectSingleNode ("provider");
			if (null != _old) {
				applicationNode.RemoveChild (_old);
			}
			XmlElement _new = this.CreateElement ("provider");
			if (null == _new) {
				return;
			}
			applicationNode.AppendChild (_new);

			// android:name
			{
				string name = "name";
				string value = "android.support.v4.content.FileProvider";
				this.SetNodeAttribute (_new, name, value);
			}

			// android:authorities
			{
				string name = "authorities";
				string value = string.Format ("{0}.installnewtype.provider", iPackageName);
				this.SetNodeAttribute (_new, name, value);
			}

			// android:exported
			{
				string name = "exported";
				string value = "false";
				this.SetNodeAttribute (_new, name, value);
			}

			// android:grantUriPermissions
			{
				string name = "grantUriPermissions";
				string value = "true";
				this.SetNodeAttribute (_new, name, value);
			}

			// meta-data
			XmlElement metaData = this.CreateNode(_new, "meta-data");
			if (null != metaData) {
				// android:name
				{
					string name = "name";
					string value = "android.support.FILE_PROVIDER_PATHS";
					this.SetNodeAttribute (metaData, name, value);
				}
				// android:resource
				{
					string name = "resource";
					string value = "@xml/buoy_provider_paths";
					this.SetNodeAttribute (metaData, name, value);
				}
			}
		}

		/// <summary>
		/// 取得res/values目录下的strings.xml的文件路径.
		/// </summary>
		/// <returns>strings.xml文件路径.</returns>
		protected override string GetStringsXMLPath() {
			return string.Format ("{0}/res/values/huawei_strings.xml", this.Dir);
		}

		/// <summary>
		/// 应用用户自定义数据.
		/// </summary>
		/// <param name="iGameName">游戏名.</param>
		protected override void ApplyUserData(string iGameName) {

			// 本地设定
			{
				string name = "Huawei_Local";
				string value = HuaweiSDKSettings.GetInstance ().Local.ToString();
				if (false == string.IsNullOrEmpty (value)) {
					this.AddUserDefineNode (name, value, false);
				}
				if (HuaweiSDKSettings.GetInstance ().Local == false) {
					return;
				}
			}

			// 游戏名
			{
				string name = "Huawei_GameName";
				string value = iGameName;
				if (false == string.IsNullOrEmpty (value)) {
					this.AddUserDefineNode (name, value);
				}
			}

			// 自动登录
			{
				string name = "Huawei_AutoLogin";
				string value = HuaweiSDKSettings.GetInstance ().AutoLogin.ToString();
				if (false == string.IsNullOrEmpty (value)) {
					this.AddUserDefineNode (name, value, false);
				}
				if (HuaweiSDKSettings.GetInstance ().Local == false) {
					return;
				}
			}
				
			// AppID
			{
				string name = "Huawei_AppID";
				string value = HuaweiSDKSettings.GetInstance().AppID;
				if (false == string.IsNullOrEmpty (value)) {
					this.AddUserDefineNode (name, value);
				}
			}
			// 浮标密钥
			{
				string name = "Huawei_BuoySecret";
				string value = HuaweiSDKSettings.GetInstance().BuoySecret;
				if (false == string.IsNullOrEmpty (value)) {
					this.AddUserDefineNode (name, value);
				}
			}
			// 支付ID
			{
				string name = "Huawei_PayID";
				string value = HuaweiSDKSettings.GetInstance().PayID;
				if (false == string.IsNullOrEmpty (value)) {
					this.AddUserDefineNode (name, value);
				}
			}
			// 支付私钥
			{
				string name = "Huawei_PayPrivateRsa";
				string value = HuaweiSDKSettings.GetInstance().PayPrivateRsa;
				if (false == string.IsNullOrEmpty (value)) {
					this.AddUserDefineNode (name, value);
				}
			}
			// 支付公钥
			{
				string name = "Huawei_PayPublicRsa";
				string value = HuaweiSDKSettings.GetInstance().PayPublicRsa;
				if (false == string.IsNullOrEmpty (value)) {
					this.AddUserDefineNode (name, value);
				}
			}
			// CPID
			{
				string name = "Huawei_CPID";
				string value = HuaweiSDKSettings.GetInstance().CPID;
				if (false == string.IsNullOrEmpty (value)) {
					this.AddUserDefineNode (name, value);
				}
			}
			// 登录签名公钥
			{
				string name = "Huawei_LoginPublicRsa";
				string value = HuaweiSDKSettings.GetInstance().LoginPublicRsa;
				if (false == string.IsNullOrEmpty (value)) {
					this.AddUserDefineNode (name, value);
				}
			}
			// 屏幕方向
			{
				string name = "Huawei_Orientation";
				string value = "1";
				UIOrientation _Orientation = HuaweiSDKSettings.GetInstance ().Orientation;
				if ((UIOrientation.LandscapeLeft == _Orientation) ||
					(UIOrientation.LandscapeRight == _Orientation)) {
					value = "2";
				}
				if (false == string.IsNullOrEmpty (value)) {
					this.AddUserDefineNode (name, value, false);
				}
			}

			// 保存strings.xml
			if(null != this._stringsXml) {
				this._stringsXml.Save ();
			}
		}
	}
}
#endif
