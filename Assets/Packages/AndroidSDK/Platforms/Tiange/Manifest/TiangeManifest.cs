﻿#if (UNITY_EDITOR) && (UNITY_ANDROID)
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using BuildSystem;
using AndroidSDK.Common.Manifest;
using AndroidSDK.Platforms.Huawei;
using BuildSystem.Options;
using BuildSystem.Options.OneSDK;

namespace AndroidSDK.Platforms.Tiange.Manifest {

	/// <summary>
	/// 天鸽Manifest文件处理类.
	/// </summary>
	public class TiangeManifest : ManifestBase {

		/// <summary>
		/// 实例.
		/// </summary>
		private static TiangeManifest _instance = null;

		/// <summary>
		/// 取得实例.
		/// </summary>
		/// <returns>实例.</returns>
		/// <param name="iHuaweiDir">路径.</param>
		/// <param name="iGameName">游戏名.</param>
		public static TiangeManifest GetInstance(string iDir, string iGameName = null) {
			try {
				if(_instance == null) {
					_instance = new TiangeManifest ();
				}
				if(false == _instance.InitByHuaweiDir(iDir, iGameName)) {
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
		/// <param name="iDir">路径.</param>
		/// <param name="iGameName">游戏名.</param>
		public bool InitByHuaweiDir(string iDir, string iGameName) {
			this.Dir = iDir;

			string manifestPath = string.Format ("{0}/AndroidManifest.xml", iDir);
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
			if (-1 >= TiangeSDKSettings.GetInstance ().MinSdkVersion) {
				this.MinSdkVersion = GetNodeAttribute_i (_useSDKNode, "minSdkVersion");
			} else {
				this.MinSdkVersion = TiangeSDKSettings.GetInstance ().MinSdkVersion;
				this.SetNodeAttribute (_useSDKNode, "minSdkVersion", this.MinSdkVersion.ToString());
			}
			if (-1 >= TiangeSDKSettings.GetInstance ().MaxSdkVersion) {
				this.MaxSdkVersion = GetNodeAttribute_i (_useSDKNode, "android:maxSdkVersion");
			} else {
				this.MaxSdkVersion = TiangeSDKSettings.GetInstance ().MaxSdkVersion;
				this.SetNodeAttribute (_useSDKNode, "maxSdkVersion", this.MaxSdkVersion.ToString());
			}
			if (-1 >= TiangeSDKSettings.GetInstance ().TargetSdkVersion) {
				this.TargetSdkVersion = GetNodeAttribute_i (_useSDKNode, "android:targetSdkVersion");
			} else {
				this.TargetSdkVersion = TiangeSDKSettings.GetInstance ().TargetSdkVersion;
				this.SetNodeAttribute (_useSDKNode, "targetSdkVersion", this.TargetSdkVersion.ToString());
			}
			return _useSDKNode;
		}

		/// <summary>
		/// 取得res/values目录下的strings.xml的文件路径.
		/// </summary>
		/// <returns>strings.xml文件路径.</returns>
		protected override string GetStringsXMLPath() {
			return string.Format ("{0}/res/values/tiange_strings.xml", this.Dir);
		}
			
		/// <summary>
		/// 应用用户自定义数据.
		/// </summary>
		/// <param name="iGameName">游戏名.</param>
		protected override void ApplyUserData(string iGameName) {

			// 本地设定
			{
				string name = "Local";
				string value = TiangeSDKSettings.GetInstance ().Local.ToString();
				if (false == string.IsNullOrEmpty (value)) {
					this.AddUserDefineNode (name, value, false);
				}
				if (TiangeSDKSettings.GetInstance ().Local == false) {
					return;
				}
			}

			// 游戏名
			{
				string name = "GameName";
				string value = iGameName;
				if (false == string.IsNullOrEmpty (value)) {
					this.AddUserDefineNode (name, value);
				}
			}

			// SDK自动初始化
			{
				string name = "AutoSDKInit";
				string value = TiangeSDKSettings.GetInstance ().AutoSDKInit.ToString();
				if (false == string.IsNullOrEmpty (value)) {
					this.AddUserDefineNode (name, value, false);
				}
				if (TiangeSDKSettings.GetInstance ().Local == false) {
					return;
				}
			}

			// 自动登录
			{
				string name = "AutoLogin";
				string value = TiangeSDKSettings.GetInstance ().AutoLogin.ToString();
				if (false == string.IsNullOrEmpty (value)) {
					this.AddUserDefineNode (name, value, false);
				}
				if (TiangeSDKSettings.GetInstance ().Local == false) {
					return;
				}
			}
			// 屏幕方向
			{
				string name = "Orientation";
				string value = "1";
				UIOrientation _Orientation = TiangeSDKSettings.GetInstance ().Orientation;
				if ((UIOrientation.LandscapeLeft == _Orientation) ||
					(UIOrientation.LandscapeRight == _Orientation)) {
					value = "2";
				}
				if (false == string.IsNullOrEmpty (value)) {
					this.AddUserDefineNode (name, value, false);
				}
			}

			// 易接SDK 设定
			if (true == TiangeSDKSettings.GetInstance ().Data.Options.isOptionValid (TSDKOptions.OneSDK)) {

				// 易接SDK Key
				{
					string name = "zy_class_name";
					string value = TiangeSDKSettings.GetInstance ().Data.Options.OneSDK.ZyClassName;
					if (false == string.IsNullOrEmpty (value)) {
						this.AddUserDefineNode (name, value);
					}
				}

				// 易接SDK MetaDatas
				{

					List<MetaDataInfo> _metaDatas = TiangeSDKSettings.GetInstance ().Data.Options.OneSDK.MetaDatas;
					foreach (MetaDataInfo _metaData in _metaDatas) {
						if ((null != _metaData) && 
							(false == string.IsNullOrEmpty (_metaData.Name)) && 
							(false == string.IsNullOrEmpty (_metaData.Value))) {
							this.AddUserDefineNode (_metaData.Name, _metaData.Value, false);
						}
					}
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
