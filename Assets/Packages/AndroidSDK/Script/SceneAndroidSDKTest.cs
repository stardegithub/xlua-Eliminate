using UnityEngine;
using System.Collections;
using AndroidSDK.Common;
using AndroidSDK.Platforms.Huawei;
using Common;

public class SceneAndroidSDKTest : MonoBehaviour {

	public void OnUToATestBtn() {

		UtilsLog.Info ("OnUToATestBtn", " -> ");

#if UNITY_ANDROID
		// 显示Debug信息
		HuaweiSDK.Instance.ShowDebugInfo("Unity 调用了这个方法");
#endif
	}
		
	/// <summary>
	/// 登陆.
	/// </summary>
	public void Login () {

		UtilsLog.Info ("Login", " -> ");

#if UNITY_ANDROID
		// 登陆
		HuaweiSDK.Instance.Login(this.gameObject, OnLoginCompleted);
#endif
	}
		
	/// <summary>
	/// 登陆完成回调函数.
	/// </summary>
	/// <param name="iJson">返回详细（推荐恢复JSON报文）.</param>
	public void OnLoginCompleted (string iDetail) {
		UtilsLog.Info ("OnLoginCompleted", "Detail -> {0}", iDetail);

#if UNITY_ANDROID
		// 解析SDK账号信息
		SDKAccountInfo accountInfo = HuaweiSDK.Instance.ParserSDKAccountInfo (iDetail);
		// 显示Debug信息
		HuaweiSDK.Instance.ShowDebugInfo (string.Format("OnLoginCompleted:Status:{0} PlayerId:{1}", 
			accountInfo.Status, accountInfo.PlayerId));
#endif
	}

	/// <summary>
	/// 登出.
	/// </summary>
	public void Logout() {

		UtilsLog.Info ("Logout", " -> ");

#if UNITY_ANDROID
		// 登出
		HuaweiSDK.Instance.Logout();
#endif
	}

	/// <summary>
	/// SDK初始化.
	/// </summary>
	public void SDKinit() {

		UtilsLog.Info ("SDKinit", " -> ");

#if UNITY_ANDROID
		// 登陆
		HuaweiSDK.Instance.SDKInit(this.gameObject, OnSDKInitCompleted);
#endif

	}

	public void OnSDKInitCompleted(string iStatusCode) {
		UtilsLog.Info ("OnSDKInitCompleted", "Detail -> {0}", iStatusCode);
#if UNITY_ANDROID

		// 取得状态吗
		SDKResultStatus status = HuaweiSDK.Instance.GetStatus(iStatusCode);
		// 显示Debug信息
		HuaweiSDK.Instance.ShowDebugInfo (string.Format("StatusCode:{0}", status.ToString()));

#endif
	}
		
	/// <summary>
	/// SDK初始化.
	/// </summary>
	public void AddPlayerInfo() {

		UtilsLog.Info ("SDKinit", " -> ");

#if UNITY_ANDROID
		// 登陆
		HuaweiSDK.Instance.AddPlayerInfo(this.gameObject, 
			"99", "Hunter", "98", "Red Cliff 2",
			OnAddPlayerInfoCompleted);
#endif

	}

	public void OnAddPlayerInfoCompleted(string iStatusCode) {
		UtilsLog.Info ("OnSDKInitCompleted", "StatusCode -> {0}", iStatusCode);
#if UNITY_ANDROID

		// 取得状态吗
		SDKResultStatus status = HuaweiSDK.Instance.GetStatus(iStatusCode);
		// 显示Debug信息
		HuaweiSDK.Instance.ShowDebugInfo (string.Format("StatusCode:{0}", status.ToString()));

#endif
	}

	/// <summary>
	/// SDK初始化.
	/// </summary>
	public void GetPlayerInfo() {

		UtilsLog.Info ("SDKinit", " -> ");

#if UNITY_ANDROID
		// 登陆
		HuaweiSDK.Instance.GetPlayerInfo(this.gameObject, 
			OnGetPlayerInfoCompleted);
#endif

	}

	public void OnGetPlayerInfoCompleted(string iDetail) {
		UtilsLog.Info ("OnSDKInitCompleted", "Detail -> {0}", iDetail);
#if UNITY_ANDROID

		// 解析SDK账号信息
		SDKAccountInfo accountInfo = HuaweiSDK.Instance.ParserSDKAccountInfo (iDetail);
		// 显示Debug信息
		HuaweiSDK.Instance.ShowDebugInfo (string.Format("Detail:{0}", iDetail));

#endif
	}

}
