using UnityEngine;
using System;
using System.Collections;
using Common;

namespace AndroidSDK.Common {

	/// <summary>
	/// AndroidSDK 接入基类.
	/// </summary>
	public abstract class AndroidSDKBase : ClassExtension, IAndroidSDK, IDisposable {

		/// <summary>
		/// 状态更新函数.
		/// </summary>
		private Action<SDKStatus> _updateStatusCallback = null;

		/// <summary>
		/// 登录校验Base Url.
		/// </summary>
		protected string loginCheckBaseUrl = null;

		/// <summary>
		/// SDK初始化完成回调函数.
		/// </summary>
		protected Action<string> sdkInitCompletedCallback = null;

		/// <summary>
		/// 登录校验回调函数.
		/// </summary>
		protected Action<string, Action<string>, Action<string>> loginCheckCallback = null;

		/// <summary>
		/// 登录校验成功回调函数.
		/// </summary>
		protected Action<string> loginCheckSuccessed = null;

		/// <summary>
		/// 登录校验失败回调函数.
		/// </summary>
		protected Action<string> loginCheckFailed = null;

		/// <summary>
		/// 自动重登最大次数.
		/// </summary>
		protected int autoReloginMaxCount = 3;

		/// <summary>
		/// 自动重登回调函数.
		/// </summary>
		protected Action<float> autoReloginCallback = null;

		/// <summary>
		/// 释放函数.
		/// </summary>
		public void Dispose() {
			this.Info ("Dispose ()");
			this.SDKDispose ();
		}

#region interface - implement

		/// <summary>
		/// 显示Debug信息.
		/// </summary>
		/// <param name="iDebugInfo">Debug信息.</param>
		public void ShowDebugInfo(string iDebugInfo) {

			//通过查看源码，我们可以发现UnityPlayer这个类可以获取当前的Activity
			//帮助手册上 AndroidJavaClass：通过指定类名可以构造出一个类
			AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

			//currentActivity字符串对应源码中UnityPlayer类下 的 Activity 变量名。
			//通过构造出的Activity根据字符串获取对象
			AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");

			//根据方法名调用方法，传入一个参数数组，这里我们只有一个，就只传一个
			jo.Call("UToA_ShowDebugInfo", iDebugInfo);
		}

		/// <summary>
		/// 设定状态更新回调函数.
		/// </summary>
		/// <param name="iUpdateStatus">I update status.</param>
		public virtual void SetUpdateStatusCallback(Action<SDKStatus> iUpdateStatus) {
			this._updateStatusCallback = iUpdateStatus;
		}

		/// <summary>
		/// 初始化.
		/// </summary>
		/// <param name="iTarget">游戏对象.</param>
		/// <param name="iOnCompleted">完成回调函数.</param>
		public void Init (GameObject iTarget, System.Action<string> iOnCompleted) {
			this.Info ("Init()");
			this.sdkInitCompletedCallback = iOnCompleted;
			this.SDKInit (iTarget, OnSDKInitCompleted);
		}

		/// <summary>
		/// SDK初始化完成.
		/// </summary>
		/// <param name="iDetail">详细信息.</param>
		protected virtual void OnSDKInitCompleted(string iDetail) {
			this.Info ("OnSDKInitCompleted()::Detail:{0}", iDetail);
			if (null != this.sdkInitCompletedCallback) {
				this.sdkInitCompletedCallback (iDetail);
			}
		}

		/// <summary>
		/// 登录.
		/// </summary>
		/// <param name="iTarget">游戏对象.</param>
		/// <param name="iLoginCompleted">登录/登出完成回调函数.</param>
		/// <param name="iLoginCheckBaseUrl">登录校验Base Url.</param>
		/// <param name="iLoginCheckCallBack">登录校验回调函数.</param>
		/// <param name="iLoginCheckSuccessed">登录校验成功回调函数.</param>
		/// <param name="iLoginCheckFailed">登录校验失败回调函数.</param>
		/// <param name="iAutoReloginMaxCount">自动重登最大次数.</param>
		/// <param name="iAutoReloginCallback">自动重登回调函数.</param>
		public void Login (
			GameObject iTarget, 
			Action<string> iLoginCompleted, string iLoginCheckBaseUrl,
			Action<string, Action<string>, Action<string>> iLoginCheckCallBack,
			Action<string> iLoginCheckSuccessed, Action<string> iLoginCheckFailed, 
			int iAutoReloginMaxCount, Action<float> iAutoReloginCallback) {
			this.Info ("Login()");

			// 设置回调函数
			this.loginCheckBaseUrl = iLoginCheckBaseUrl;
			this.loginCheckCallback = iLoginCheckCallBack;
			this.loginCheckSuccessed = iLoginCheckSuccessed;
			this.loginCheckFailed = iLoginCheckFailed;
			this.autoReloginMaxCount = iAutoReloginMaxCount;
			this.autoReloginCallback = iAutoReloginCallback;

			// 登录
			this.SDKLogin (iTarget, iLoginCompleted);
		}
			
		/// <summary>
		/// 重登录.
		/// </summary>
		public void Relogin()  {
			this.Info ("Relogin()");
			this.SDKRelogin ();
		}

		/// <summary>
		/// 登录校验成功.
		/// </summary>
		/// <param name="iDetail">详细信息.</param>
		protected virtual void OnLoginCheckSuccessed(string iDetail) {
			this.Info ("OnLoginCheckSuccessed()::Detail:{0}", iDetail);
			if (null != this.loginCheckSuccessed) {
				this.loginCheckSuccessed (iDetail);
			}
		}

		/// <summary>
		/// 登录校验成功.
		/// </summary>
		/// <param name="iDetail">详细信息.</param>
		protected virtual void OnLoginCheckFailed(string iDetail) {
			this.Info ("OnLoginCheckFailed()::Detail:{0}", iDetail);
			if (null != this.loginCheckFailed) {
				this.loginCheckFailed (iDetail);
			}
		}

		/// <summary>
		/// 登出.
		/// </summary>
		public void Logout() {
			this.Info ("Logout()");
			this.SDKLogout ();

			// 释放
			this.Dispose ();
		}
			
		/// <summary>
		/// 添加玩家信息.
		/// </summary>
		/// <param name="iTarget">游戏对象.</param>
		/// <param name="iGameRank">游戏等级.</param>
		/// <param name="iGameRole">游戏角色.</param>
		/// <param name="iGameArea">游戏区.</param>
		/// <param name="iGameSociaty">游戏工会.</param>
		/// <param name="iOnCompleted">完成回调函数.</param>
		public void AddPlayerInfo (
			GameObject iTarget, 
			String iGameRank,
			String iGameRole,
			String iGameArea,
			String iGameSociaty,
			Action<string> iOnCompleted = null) {
			this.Info ("AddPlayerInfo()");
			this.SDKAddPlayerInfo (
				iTarget, iGameRank, iGameRole,
				iGameArea, iGameSociaty, iOnCompleted);
		}
			
		/// <summary>
		/// 取得玩家信息.
		/// </summary>
		/// <param name="iTarget">游戏对象.</param>
		/// <param name="iOnCompleted">完成回调函数.</param>
		public void GetPlayerInfo (
			GameObject iTarget, 
			Action<string> iOnCompleted = null) {
			this.Info ("GetPlayerInfo()");
			this.SDKGetPlayerInfo (iTarget, iOnCompleted);
		}

		/// <summary>
		/// 解析用户信息.
		/// </summary>
		/// <returns>用户信息.</returns>
		/// <param name="iUserInfo">用户信息(Json格式数据).</param>>
		public SDKAccountBaseInfo ParserAccountInfo(string iUserInfo) {
			this.Info ("ParserAccountInfo()");
			return this.SDKParserAccountInfo(iUserInfo);
		}
			
		/// <summary>
		/// 支付函数.
		/// </summary>
		/// <param name="iTarget">游戏对象.</param>
		/// <param name="iIAPItemPrice">购买道具价格.</param>
		/// <param name="iIAPItemName">购买道具名.</param>
		/// <param name="iIAPItemCount">购买道具数量.</param>
		/// <param name="iOtherDIYInfo">其他自定义信息.</param>
		/// <param name="iNotifyUrl">支付结果通知URL（一般与游戏服务器上设置该URL）.</param>
		/// <param name="iOnPayCompleted">支付完成回调函数.</param>
		public void Pay (
			GameObject iTarget, 
			int iIAPItemPrice,
			string iIAPItemName,
			int iIAPItemCount, 
			string iOtherDIYInfo, 
			string iNotifyUrl, 
			Action<string> iOnPayCompleted) {

			this.Info ("Pay()");
			this.SDKPay (
				iTarget, iIAPItemPrice, iIAPItemName,
				iIAPItemCount, iOtherDIYInfo, iNotifyUrl, iOnPayCompleted);
		}

		/// <summary>
		/// 解析支付信息.
		/// </summary>
		/// <returns>支付信息.</returns>
		/// <param name="iPayInfo">支付信息(Json格式数据).</param>
		/// <param name="iOnPaymentSuccessed">支付成功回调函数.</param>
		public SDKPaymentBaseInfo ParserPaymentInfo(string iPayInfo, 
			Action<SDKAccountBaseInfo, string> iOnPaymentSuccessed) {
			this.Info ("ParserPaymentInfo()");
			if (null == iOnPaymentSuccessed) {
				this.Warning ("ParserPaymentInfo()::OnPaymentSuccessed is null!!!");
			}
			return this.SDKParserPaymentInfo(iPayInfo, iOnPaymentSuccessed);
		}
			
		/// <summary>
		/// 创建SDK角色信息.
		/// </summary>
		/// <returns>SDK角色信息.</returns>
		/// <param name="iRoleId">角色ID（必须为数字）.</param>
		/// <param name="iRoleName">角色名（不能为空，不能为null）.</param>
		/// <param name="iRoleLevel">角色等级（必须为数字，不能为0，默认1）.</param>
		/// <param name="iZoneID">游戏区ID（必须为数字，不能为0，默认为1）.</param>
		/// <param name="iZoneName">游戏区名（不能为空，不能为null）.</param>
		/// <param name="iBalance">游戏币余额（必须为数字，默认0）.</param>
		/// <param name="iVip">VIP等级（必须为数字，默认诶1）.</param>
		/// <param name="iPartyName">当前所属帮派（不能为空，不能为null，默认：无帮派）.</param>
		/// <param name="iRoleCTime">角色创建时间（单位：秒）.</param>
		/// <param name="iRoleLevelMTime">角色等级变化时间（单位：秒）.</param>
		public SDKRoleBaseInfo CreateRoleInfo (
			string iRoleId, string iRoleName, string iRoleLevel, string iZoneID, string iZoneName,
			string iBalance, string iVip, string iPartyName, string iRoleCTime, string iRoleLevelMTime) {

			this.Info ("CreateRoleInfo()::RoleID:{0} RoleName:{1} RoleLevel:{2} ZoneID:{3} ZoneName:{4} " +
				"Balance:{5} Vip:{6} PartyName:{7} RoleCTime:{8} RoleLevelMTime:{9}",
				iRoleId, iRoleName, iRoleLevel, iZoneID, iZoneName, iBalance, iVip, iPartyName, iRoleCTime, iRoleLevelMTime);
			return this.SDKCreateRoleInfo (
				iRoleId, iRoleName, iRoleLevel, iZoneID, iZoneName, 
				iBalance, iVip, iPartyName, iRoleCTime, iRoleLevelMTime);
		}
			
		/// <summary>
		/// 创建角色.
		/// </summary>
		/// <param name="iRoleInfo">角色信息.</param>
		public void CreateRole (SDKRoleBaseInfo iRoleInfo) {
			this.Info ("CreateRole()::Roinfo:{0}", iRoleInfo.ToString());
			this.SDKCreateRole (iRoleInfo);
		}

		/// <summary>
		/// 更新等级信息（升级时）.
		/// </summary>
		/// <param name="iRoleInfo">角色信息.</param>
		public void UpdateRoleInfoWhenLevelup (SDKRoleBaseInfo iRoleInfo) {
			this.Info ("UpdateRoleInfoWhenLevelup()::Roinfo:{0}", iRoleInfo.ToString());
			this.SDKUpdateRoleInfoWhenLevelup (iRoleInfo);
		}

		/// <summary>
		/// 更新等级信息（升级时）.
		/// </summary>
		/// <param name="iRoleInfo">角色信息.</param>
		public void UpdateRoleInfoWhenEnterServer (SDKRoleBaseInfo iRoleInfo) {
			this.Info ("UpdateRoleInfoWhenEnterServer()::Roinfo:{0}", iRoleInfo.ToString());
			this.SDKUpdateRoleInfoWhenEnterServer (iRoleInfo);
		}
			
		/// <summary>
		/// 退出.
		/// </summary>
		/// <param name="iTarget">游戏对象.</param>
		/// <param name="iOnExited">退出回调函数.</param>
		public void Exit(
			GameObject iTarget = null, 
			Action<string> iOnExited = null) {
		
			this.Info ("Exit()::Target:{0} Method:{1}",
				(null == iTarget) ? "null" : iTarget.name,
				(null == iOnExited) ? "null" : iOnExited.Method.Name);
			this.SDKExit (iTarget, iOnExited);
		}

#endregion

#region abstract

		/// <summary>
		/// 释放函数.
		/// </summary>
		protected abstract void SDKDispose();

		/// <summary>
		/// 初始化.
		/// </summary>
		/// <param name="iTarget">游戏对象.</param>
		/// <param name="iOnCompleted">完成回调函数.</param>
		protected abstract void SDKInit (GameObject iTarget, System.Action<string> iOnCompleted);

		/// <summary>
		/// 登录.
		/// </summary>
		/// <param name="iTarget">登陆启动的目标对象.</param>
		/// <param name="iLoginCompleted">登录/登出完成回调函数.</param>
		protected abstract void SDKLogin (
			GameObject iTarget, 
			Action<string> iLoginCompleted);

		/// <summary>
		/// 重登录.
		/// </summary>
		protected abstract void SDKRelogin();

		/// <summary>
		/// 登出.
		/// </summary>
		protected abstract void SDKLogout ();

		/// <summary>
		/// 添加玩家信息.
		/// </summary>
		/// <param name="iTarget">游戏对象.</param>
		/// <param name="iGameRank">游戏等级.</param>
		/// <param name="iGameRole">游戏角色.</param>
		/// <param name="iGameArea">游戏区.</param>
		/// <param name="iGameSociaty">游戏工会.</param>
		/// <param name="iOnCompleted">完成回调函数.</param>
		protected abstract void SDKAddPlayerInfo (
			GameObject iTarget, 
			String iGameRank,
			String iGameRole,
			String iGameArea,
			String iGameSociaty,
			Action<string> iOnCompleted = null);

		/// <summary>
		/// 取得玩家信息.
		/// </summary>
		/// <param name="iTarget">游戏对象.</param>
		/// <param name="iOnCompleted">完成回调函数.</param>
		protected abstract void SDKGetPlayerInfo (
			GameObject iTarget, 
			Action<string> iOnCompleted = null);

		/// <summary>
		/// SDK解析用户信息.
		/// </summary>
		/// <returns>用户信息.</returns>
		/// <param name="iUserInfo">用户信息(Json格式数据).</param>>
		protected abstract SDKAccountBaseInfo SDKParserAccountInfo (string iUserInfo);

		/// <summary>
		/// 支付函数.
		/// </summary>
		/// <param name="iTarget">游戏对象.</param>
		/// <param name="iIAPItemPrice">购买道具价格.</param>
		/// <param name="iIAPItemName">购买道具名.</param>
		/// <param name="iIAPItemCount">购买道具数量.</param>
		/// <param name="iOtherDIYInfo">其他自定义信息.</param>
		/// <param name="iNotifyUrl">支付结果通知URL（一般与游戏服务器上设置该URL）.</param>
		/// <param name="iOnPayCompleted">支付完成回调函数.</param>
		protected abstract void SDKPay (
			GameObject iTarget, 
			int iIAPItemPrice,
			string iIAPItemName,
			int iIAPItemCount, 
			string iOtherDIYInfo, 
			string iNotifyUrl, 
			Action<string> iOnPayCompleted);

		/// <summary>
		/// 解析支付信息.
		/// </summary>
		/// <returns>支付信息.</returns>
		/// <param name="iPayInfo">支付信息(Json格式数据).</param>
		/// <param name="iOnPaymentSuccessed">支付成功回调函数.</param>
		protected abstract SDKPaymentBaseInfo SDKParserPaymentInfo(
			string iPayInfo, 
			Action<SDKAccountBaseInfo, string> iOnPaymentSuccessed);

		/// <summary>
		/// 创建SDK角色信息.
		/// </summary>
		/// <returns>SDK角色信息.</returns>
		/// <param name="iRoleId">角色ID（必须为数字）.</param>
		/// <param name="iRoleName">角色名（不能为空，不能为null）.</param>
		/// <param name="iRoleLevel">角色等级（必须为数字，不能为0，默认1）.</param>
		/// <param name="iZoneID">游戏区ID（必须为数字，不能为0，默认为1）.</param>
		/// <param name="iZoneName">游戏区名（不能为空，不能为null）.</param>
		/// <param name="iBalance">游戏币余额（必须为数字，默认0）.</param>
		/// <param name="iVip">VIP等级（必须为数字，默认诶1）.</param>
		/// <param name="iPartyName">当前所属帮派（不能为空，不能为null，默认：无帮派）.</param>
		/// <param name="iRoleCTime">角色创建时间（单位：秒）.</param>
		/// <param name="iRoleLevelMTime">角色等级变化时间（单位：秒）.</param>
		public abstract SDKRoleBaseInfo SDKCreateRoleInfo (
			string iRoleId, string iRoleName, string iRoleLevel, string iZoneID, string iZoneName,
			string iBalance, string iVip, string iPartyName, string iRoleCTime, string iRoleLevelMTime);

		/// <summary>
		/// 创建角色.
		/// </summary>
		/// <param name="iRoleInfo">角色信息.</param>
		public abstract void SDKCreateRole (SDKRoleBaseInfo iRoleInfo);

		/// <summary>
		/// 更新等级信息（升级时）.
		/// </summary>
		/// <param name="iRoleInfo">角色信息.</param>
		public abstract void SDKUpdateRoleInfoWhenLevelup (SDKRoleBaseInfo iRoleInfo);

		/// <summary>
		/// 更新等级信息（升级时）.
		/// </summary>
		/// <param name="iRoleInfo">角色信息.</param>
		public abstract void SDKUpdateRoleInfoWhenEnterServer (SDKRoleBaseInfo iRoleInfo);

		/// <summary>
		/// 退出.
		/// </summary>
		/// <param name="iTarget">游戏对象.</param>
		/// <param name="iOnExited">退出回调函数.</param>
		public abstract void SDKExit(
			GameObject iTarget = null, 
			Action<string> iOnExited = null);

#endregion

	}
}
