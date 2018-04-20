#if UNITY_ANDROID
using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using Common;
using AndroidSDK.Common;

namespace AndroidSDK.OneSDK {
	
	/// <summary>
	/// 易接SDK库.
	/// </summary>
	public class OneSDKLibs : SingletonBase<OneSDKLibs>, IAndroidSDK {

#region Login - Logout

		/// <summary>
		/// login接口用于SDK登陆.
		/// </summary>
		/// <param name="iContext">上下文Activity.</param>
		/// <param name="iCustomParams">自定义参数.</param>
		[DllImport("gangaOnlineUnityHelper")]
		private static extern void login(IntPtr iContext, string iCustomParams);

		/// <summary>
		/// setLoginListener方法用于设置登陆监听.
		/// </summary>
		/// <param name="iContext">上下文Activity.</param>
		/// <param name="iTargetGameObject">监听游戏对象.</param>
		/// <param name="iListener">监听器(方法名).</param>
		[DllImport("gangaOnlineUnityHelper")]
		private static extern void setLoginListener (IntPtr iContext, string iTargetGameObject, string iListener);

		/// <summary>
		/// logout接口用于SDK登出.
		/// </summary>
		/// <param name="iContext">上下文Activity.</param>
		/// <param name="iCustomParams">自定义参数.</param>
		[DllImport("gangaOnlineUnityHelper")]
		private static extern void logout(IntPtr iContext, string iCustomParams);

#endregion

		/// <summary>
		/// exit接口用于系统全局退出.
		/// </summary>
		/// <param name="iContext">上下文Activity.</param>
		/// <param name="iTargetGameObject">监听游戏对象.</param>
		/// <param name="iListener">监听器(方法名).</param>
		[DllImport("gangaOnlineUnityHelper")]
		private static extern void exit(IntPtr iContext, string iTargetGameObject, string iListener);

#region Payment

		/// <summary>
		/// pay接口用于用户触发定额计费.
		/// </summary>
		/// <param name="iContext">上下文Activity.</param>
		/// <param name="iTargetGameObject">监听游戏对象.</param>
		/// <param name="iUnitPrice">游戏道具单位价格(单位:分).</param>
		/// <param name="iUnitName">虚拟货币名称.</param>
		/// <param name="iCount">商品或道具数量.</param>
		/// <param name="iCallBackInfo">由游戏开发者定义传入的字符串，会与支付结果一同发送给游戏服务器.游戏服务器可通过该字段判断交易的详细内容（金额角色等）</param>
		/// <param name="iCallBackUrl">将支付结果通知给游戏服务器时的通知地址url，交易结束后，系统会向该url发送http请求，通知交易的结果金额callbackInfo等信息.</param>
		/// <param name="iPayResultListener">支付监听接口，隶属于gameObject对象的运行时脚本的方法名称，该方法会在收到通知后触发.</param>
		[DllImport("gangaOnlineUnityHelper")]
		private static extern void pay(
			IntPtr iContext, string iTargetGameObject, int iUnitPrice, 
			string iUnitName, int iCount, string iCallBackInfo, 
			string iCallBackUrl, string iPayResultListener);

		/// <summary>
		/// charge接口用于用户触发非定额计费.
		/// </summary>
		/// <param name="iContext">上下文Activity.</param>
		/// <param name="iTargetGameObject">监听游戏对象.</param>
		/// <param name="iUnitName">虚拟货币名称.</param>
		/// <param name="iUnitPrice">游戏道具单位价格(单位:分).</param>
		/// <param name="iCount">商品或道具数量.</param>
		/// <param name="iCallBackInfo">由游戏开发者定义传入的字符串，会与支付结果一同发送给游戏服务器.游戏服务器可通过该字段判断交易的详细内容（金额角色等）</param>
		/// <param name="iCallBackUrl">将支付结果通知给游戏服务器时的通知地址url，交易结束后，系统会向该url发送http请求，通知交易的结果金额callbackInfo等信息.</param>
		/// <param name="iPayResultListener">支付监听接口，隶属于gameObject对象的运行时脚本的方法名称，该方法会在收到通知后触发.</param>
		[DllImport("gangaOnlineUnityHelper")]
		private static extern void charge(
			IntPtr iContext, string iTargetGameObject, string iUnitName, 
			int iUnitPrice, int iCount, string iCallBackInfo, 
			string iCallBackUrl, string iPayResultListener);

#endregion

#region Player/Role info

		/// <summary>
		/// 部分渠道如UC渠道，要对游戏人物数据进行统计，而且为接入规范，调用时间：在游戏角色登录成功后调用.
		/// </summary>
		/// <param name="iContext">上下文Activity.</param>
		/// <param name="iRoleId">角色唯一标识(类似于GUID/token).</param>
		/// <param name="iRoleName">角色名.</param>
		/// <param name="iRoleLevel">角色等级.</param>
		/// <param name="iZoneId">区域ID.</param>
		/// <param name="iZoneName">区域名.</param>
		[DllImport("gangaOnlineUnityHelper")]
		private static extern void setRoleData(
			IntPtr iContext, string iRoleId, string iRoleName, 
			string roleLevel, string iZoneId, string iZoneName);

#endregion

#region Data

		/// <summary>
		/// 设定数据（备用接口）.
		/// </summary>
		/// <param name="iContext">上下文Activity.</param>
		/// <param name="iKey">Key.</param>
		/// <param name="iValue">Value.</param>
		[DllImport("gangaOnlineUnityHelper")]
		private static extern void setData (IntPtr iContext, string iKey, string iValue);

#endregion

#region Interface - Extension

		/// <summary>
		/// extend扩展接口.
		/// 扩展接口，有些 SDK， 要求必须接入统计接口或者其它特殊的接口
		/// 并且有返回值或者回调的函数，用户可以使用此接口调用，具体可以参考易接工具上的SDK的参数填写帮助。
		/// </summary>
		/// <param name="iContext">上下文Activity.</param>
		/// <param name="data">Data.</param>
		/// <param name="gameObject">Game object.</param>
		/// <param name="listener">Listener.</param>
		[DllImport("gangaOnlineUnityHelper")]
		private static extern void extend (
			IntPtr context, string data, 
			string gameObject, string listener);

#endregion

		/// <summary>
		/// 登录.
		/// </summary>
		/// <param name="iTarget">登陆启动的目标对象.</param>
		/// <param name="iLoginCallback">登录回调函数.</param>
		public void Login(
			GameObject iTarget, System.Action<string> iLoginCallback) {
			AndroidJavaClass _unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
			if (null != _unityPlayer) {
				this.Error ("Login():The unity player is invalid!!!");
				return;
			}
			AndroidJavaObject _curActivity = _unityPlayer.GetStatic<AndroidJavaObject> ("currentActivity");
			if (null != _curActivity) {
				this.Error ("Login():The current activity is invalid!!!");
				return;
			}
			this.Info ("Login()");
			login (_curActivity.GetRawObject (), "Login");
			// 设定回调函数
			setLoginListener(_curActivity.GetRawObject (), iTarget.name, iLoginCallback.Method.Name);
		}

		/// <summary>
		/// 登出.
		/// </summary>
		public void Logout() {
			AndroidJavaClass _unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
			if (null != _unityPlayer) {
				this.Error ("Logout():The unity player is invalid!!!");
				return;
			}
			AndroidJavaObject _curActivity = _unityPlayer.GetStatic<AndroidJavaObject> ("currentActivity");
			if (null != _curActivity) {
				this.Error ("Logout():The current activity is invalid!!!");
				return;
			}
			this.Info ("Logout()");
			logout(_curActivity.GetRawObject (), "Logout");
		}

	}

}

#endif
