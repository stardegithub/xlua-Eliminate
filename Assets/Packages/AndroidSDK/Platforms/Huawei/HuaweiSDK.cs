using UnityEngine;
using System;
using System.Collections;
using Common;
using AndroidSDK.Common;

#if UNITY_ANDROID

namespace AndroidSDK.Platforms.Huawei {

	public sealed class HuaweiSDK : SingletonBase<HuaweiSDK>, IAndroidSDK {

		/// <summary>
		/// 取得状态.
		/// </summary>
		/// <returns>状态.</returns>
		/// <param name="iDetail">详细.</param>
		public SDKResultStatus GetStatus(string iDetail) {
			int _value = Convert.ToInt16 (iDetail);
			return (SDKResultStatus)_value;
		}

		/// <summary>
		/// 显示Debug信息.
		/// </summary>
		/// <param name="iInfo">信息.</param>
		public void ShowDebugInfo(string iInfo) {

			//通过查看源码，我们可以发现UnityPlayer这个类可以获取当前的Activity
			//帮助手册上 AndroidJavaClass：通过指定类名可以构造出一个类
			AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

			//currentActivity字符串对应源码中UnityPlayer类下 的 Activity 变量名。
			//通过构造出的Activity根据字符串获取对象
			AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");

			//根据方法名调用方法，传入一个参数数组，这里我们只有一个，就只传一个
			jo.Call("UToA_ShowDebugInfo", iInfo);
		}

		/// <summary>
		/// 初始化SDK.
		/// </summary>
		/// <param name="iTarget">对象.</param>
		/// <param name="iOnSDKInitCompleted">SDK初始化完成回调函数.</param>
		public void SDKInit(
			GameObject iTarget, 
			Action<string> iOnSDKInitCompleted) {

			if (TPlatformType.None == BuildInfo.GetInstance ().PlatformType) {
				UtilsLog.Error ("AndroidLibs", "SDKInit():The platformType is none in buildinfo.asset file!!!");
				return;
			}
			if (null == iTarget) {
				return;
			}

			AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
			jo.Call ("UToA_SDKInit", iTarget.name, iOnSDKInitCompleted.Method.Name);
		}

		/// <summary>
		/// 登陆.
		/// </summary>
		/// <param name="iTarget">登陆对象.</param>
		/// <param name="iOnLoginCompleted">登陆完成.</param>
		public void Login(
			GameObject iTarget, 
			Action<string> iOnLoginCompleted) {

			if (TPlatformType.None == BuildInfo.GetInstance ().PlatformType) {
				UtilsLog.Error ("AndroidLibs", "Login():The platformType is none in buildinfo.asset file!!!");
				return;
			}
			if (null == iTarget) {
				return;
			}

			AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
			jo.Call ("UToA_Login", iTarget.name, iOnLoginCompleted.Method.Name);
		}

		/// <summary>
		/// 登出.
		/// </summary>
		public void Logout() {
			if (TPlatformType.None == BuildInfo.GetInstance ().PlatformType) {
				UtilsLog.Error ("AndroidLibs", "Logout():The platformType is none in buildinfo.asset file!!!");
				return;
			}

			AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
			jo.Call ("UToA_Logout");
		}

		/// <summary>
		/// 登陆.
		/// </summary>
		/// <param name="iTarget">登陆对象.</param>
		/// <param name="iOnLoginCompleted">登陆完成.</param>
		public void AddPlayerInfo(
			GameObject iTarget, 
			String iGameRank,
			String iGameRole,
			String iGameArea,
			String iGameSociaty,
			Action<string> iOnAddPlayerInfoCompleted = null) {

			if (TPlatformType.None == BuildInfo.GetInstance ().PlatformType) {
				UtilsLog.Error ("AndroidLibs", "AddPlayerInfo():The platformType is none in buildinfo.asset file!!!");
				return;
			}

			if (false == Application.isMobilePlatform) {
				return;
			}

			AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
			if (null != iOnAddPlayerInfoCompleted) {
				jo.Call ("UToA_AddPlayerInfo", 
					iTarget.name, 
					iGameRank, iGameRole, iGameArea, iGameSociaty,
					iOnAddPlayerInfoCompleted.Method.Name);
			} else {
				jo.Call ("UToA_AddPlayerInfo", 
					iTarget.name, 
					iGameRank, iGameRole, iGameArea, iGameSociaty, null);
			}
		}



		/// <summary>
		/// 登陆.
		/// </summary>
		/// <param name="iTarget">登陆对象.</param>
		/// <param name="iOnLoginCompleted">登陆完成.</param>
		public void GetPlayerInfo(
			GameObject iTarget, 
			Action<string> iOnGetPlayerInfoCompleted = null) {

			if (TPlatformType.None == BuildInfo.GetInstance ().PlatformType) {
				UtilsLog.Error ("AndroidLibs", "GetPlayerInfo():The platformType is none in buildinfo.asset file!!!");
				return;
			}

			if (false == Application.isMobilePlatform) {
				return;
			}

			AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
			jo.Call ("UToA_GetPlayerInfo", 
				iTarget.name, 
				iOnGetPlayerInfoCompleted.Method.Name);
		}

		/// <summary>
		/// 解析SDK账号信息.
		/// </summary>
		/// <returns>SDK账号信息.</returns>
		/// <param name="iJsonDetail">Json详细.</param>
		public SDKAccountInfo ParserSDKAccountInfo(string iJsonDetail) {
			return JsonUtility.FromJson<SDKAccountInfo> (iJsonDetail);
		}
	}
}

#endif