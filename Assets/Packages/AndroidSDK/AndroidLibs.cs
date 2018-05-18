#if UNITY_ANDROID
using UnityEngine;
using System;
using System.Collections;
using Common;
using AndroidSDK.Common;
using AndroidSDK.Platforms.Huawei;
using AndroidSDK.Platforms.Tiange;

namespace AndroidSDK {

	/// <summary>
	/// Android libs.
	/// </summary>
	public sealed class AndroidLibs : SingletonBase<AndroidLibs>, IDisposable {

		/// <summary>
		/// 实例.
		/// </summary>
		private AndroidSDKBase _sdkInstance = null;

		private SDKStatus _status = SDKStatus.Invalid;
		/// <summary>
		/// 状态.
		/// </summary>
		public SDKStatus Status {
			get { 
				if (null == _sdkInstance) {
					return SDKStatus.Invalid;
				}
				return _status;
			}
			private set { 
				_status = value;
			}
		}

		/// <summary>
		/// 释放函数.
		/// </summary>
		public void Dispose() {
			this.Info ("Dispose ()");
			if (null != this._sdkInstance) {
				this._sdkInstance.Dispose ();
			}
		}

		/// <summary>
		/// 初始化.
		/// </summary>
		protected override void Init()
		{
			base.Init ();

			// 取得实例
			this._sdkInstance = this.GetAndroidSDKInstance();
			if (null != this._sdkInstance) {
				this._sdkInstance.SetUpdateStatusCallback (UpdateStatus);
			}
		}

		/// <summary>
		/// 更新状态.
		/// </summary>
		/// <param name="iStatus">状态.</param>
		private void UpdateStatus(SDKStatus iStatus) {
			this.Info ("UpdateStatus():: -> {0}", iStatus);
			this.Status = iStatus;
		}

		/// <summary>
		/// 取得安卓SDK实例.
		/// </summary>
		/// <returns>The android SDK instance.</returns>
		private AndroidSDKBase GetAndroidSDKInstance() {
		
			AndroidSDKBase _objRet = null;
			TPlatformType _platformType = BuildInfo.GetInstance ().PlatformType;
			switch (_platformType) {
			case TPlatformType.Huawei:
				{
					_objRet = new HuaweiSDK ();
				}
				break;
			case TPlatformType.Tiange:
				{
					_objRet = new TiangeSDK();
				}
				break;
			case TPlatformType.Android:
			case TPlatformType.iOS:
			case TPlatformType.None:
			default:
				{
					this.Error ("GetAndroidSDKInstance():The platformType is invalid setting in buildinfo.asset!!!(TPlatformType:{0})",
						_platformType);
				}
				break;
			}
			return _objRet;
		}

		/// <summary>
		/// 显示Debug信息.
		/// </summary>
		/// <param name="iDebugInfo">Debug信息.</param>
		public void ShowDebugInfo(string iDebugInfo) {
			if (null == this._sdkInstance) {
				this.Error ("ShowDebugInfo():The instance of android sdk is invalid!!!");
				return;
			}
			this.Info ("ShowDebugInfo():{0}", iDebugInfo);
			this._sdkInstance.ShowDebugInfo (iDebugInfo);
		}

		/// <summary>
		/// 取得状态.
		/// </summary>
		/// <returns>状态.</returns>
		/// <param name="iStatusCode">状态码.</param>
		public SDKStatus GetStatus(string iStatusCode) {
			if (null == this._sdkInstance) {
				this.Error ("GetStatus():The instance of android sdk is invalid!!!");
				return SDKStatus.Invalid;
			}
			int _statusValue = Convert.ToInt16 (iStatusCode);
			SDKStatus _status = (SDKStatus)_statusValue;
			this.Info ("GetStatus()::Status:{0}", _status);
			return _status;
		}

		/// <summary>
		/// 登录.
		/// </summary>
		/// <param name="iTarget">游戏对象.</param>
		/// <param name="iLoginCompleted">登录/登出完成回调函数.</param>
		/// <param name="iLoginCheckBaseUrl">登录校验Base Url.</param>
		/// <param name="iLoginCheckCallBack">登录检测回调函数.</param>
		/// <param name="iLoginCheckSuccessed">登录检测成功回调函数.</param>
		/// <param name="iLoginCheckFailed">登录检测失败回调函数.</param>
		/// <param name="iAutoReloginMaxCount">自动重登最大次数.</param>
		/// <param name="iAutoReloginCallback">自动重登回调函数.</param>
		public void Login (
			GameObject iTarget, 
			Action<string> iLoginCompleted, string iLoginCheckBaseUrl,
			Action<string, Action<string>, Action<string>> iLoginCheckCallBack,
			Action<string> iLoginCheckSuccessed, Action<string> iLoginCheckFailed,
			int iAutoReloginMaxCount, Action<float> iAutoReloginCallback) {

			if (null == this._sdkInstance) {
				this.Error ("Login():The instance of android sdk is invalid!!!");
				return;
			}
			if (null == iTarget) {
				this.Error ("Login():The target of game object is invalid!!!");
				return;
			}
			this.Info ("Login()");
			this._sdkInstance.Login (
				iTarget, iLoginCompleted, iLoginCheckBaseUrl, 
				iLoginCheckCallBack, iLoginCheckSuccessed, iLoginCheckFailed,
				iAutoReloginMaxCount, iAutoReloginCallback);
		}

		/// <summary>
		/// 重登录.
		/// </summary>
		public void Relogin()  {
			if (null == this._sdkInstance) {
				this.Error ("Login():The instance of android sdk is invalid!!!");
				return;
			}
			this.Info ("Relogin()");
			this._sdkInstance.Relogin ();
		}

		/// <summary>
		/// 登出.
		/// </summary>
		public void Logout() {
			if (null == this._sdkInstance) {
				this.Error ("Logout():The instance of android sdk is invalid!!!");
				return;
			}
			this.Info ("Logout()");
			this._sdkInstance.Logout ();
		}

		/// <summary>
		/// SDK初始化.
		/// </summary>
		public void SDKInit(
			GameObject iTarget, 
			Action<string> iOnSDKInitCompleted) {
			if (null == this._sdkInstance) {
				this.Error ("SDKInit():The instance of android sdk is invalid!!!");
				return;
			}
			if (null == iTarget) {
				this.Error ("SDKInit():The target of game object is invalid!!!");
				return;
			}
			this.Info ("SDKInit()");
			this._sdkInstance.Init (iTarget, iOnSDKInitCompleted);
		}

		/// <summary>
		/// 添加玩家信息.
		/// </summary>
		/// <param name="iTarget">目标对象.</param>
		/// <param name="iGameRank">游戏等级.</param>
		/// <param name="iGameRole">游戏角色.</param>
		/// <param name="iGameArea">游戏区.</param>
		/// <param name="iGameSociaty">游戏工会.</param>
		/// <param name="iOnCompleted">完成回调函数.</param>
		public void AddPlayerInfo(
			GameObject iTarget, 
			String iGameRank,
			String iGameRole,
			String iGameArea,
			String iGameSociaty,
			Action<string> iOnCompleted = null) {
			if (null == this._sdkInstance) {
				this.Error ("AddPlayerInfo():The instance of android sdk is invalid!!!");
				return;
			}
			if (null == iTarget) {
				this.Error ("AddPlayerInfo():The target of game object is invalid!!!");
				return;
			}
			this.Info ("AddPlayerInfo()");
			this._sdkInstance.AddPlayerInfo (
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

			if (null == this._sdkInstance) {
				this.Error ("GetPlayerInfo():The instance of android sdk is invalid!!!");
				return;
			}
			if (null == iTarget) {
				this.Error ("GetPlayerInfo():The target of game object is invalid!!!");
				return;
			}
			this.Info ("GetPlayerInfo()");
			this._sdkInstance.GetPlayerInfo (
				iTarget, iOnCompleted);
		}

		/// <summary>
		/// 解析用户信息.
		/// </summary>
		/// <returns>用户信息.</returns>
		/// <param name="iUserInfo">用户信息(Json格式数据).</param>
		public SDKAccountBaseInfo ParserAccountInfo(string iUserInfo)  {
			if (null == this._sdkInstance) {
				this.Error ("ParserAccountInfo():The instance of android sdk is invalid!!!");
				return null;
			}
			this.Info ("ParserAccountInfo():{0}", iUserInfo);
			return this._sdkInstance.ParserAccountInfo (iUserInfo);

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

			if (null == this._sdkInstance) {
				this.Error ("Pay():The instance of android sdk is invalid!!!");
				return;
			}

			this.Info ("Pay()::Price:{0} Name:{1} Count:{2} OtherDIYInfo:{3} NotifyUrl:{4}/ Target:{5} Callback:{6}",
				iIAPItemPrice, iIAPItemName, iIAPItemCount, iOtherDIYInfo, iNotifyUrl,
				iTarget.name, iOnPayCompleted.Method.Name);

			// 支付
			this._sdkInstance.Pay (
				iTarget, iIAPItemPrice, iIAPItemName, 
				iIAPItemCount, iOtherDIYInfo, iNotifyUrl, iOnPayCompleted);
		}
			
		/// <summary>
		/// 解析支付信息.
		/// </summary>
		/// <returns>用户信息.</returns>
		/// <param name="iPayInfo">支付信息(Json格式数据).</param>
		/// <param name="iOnPaymentSuccessed">支付成功回调函数.</param>
		public SDKPaymentBaseInfo ParserPaymentInfo(
			string iPayInfo,
			Action<SDKAccountBaseInfo, string> iOnPaymentSuccessed)  {

			if (null == iOnPaymentSuccessed) {
				this.Warning ("ParserPaymentInfo()::OnPaymentSuccessed is null!!!");
			}

			if (null == this._sdkInstance) {
				this.Error ("ParserPaymentInfo():The instance of android sdk is invalid!!!");
				return null;
			}
			this.Info ("ParserPaymentInfo():{0}", iPayInfo);
			return this._sdkInstance.ParserPaymentInfo (iPayInfo, iOnPaymentSuccessed);

		}
			
		/// <summary>
		/// 创建角色.
		/// </summary>
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
		public void CreateRole(
			string iRoleId, string iRoleName, string iRoleLevel, string iZoneID, string iZoneName,
			string iBalance, string iVip, string iPartyName, string iRoleCTime, string iRoleLevelMTime) {

			if (null == this._sdkInstance) {
				this.Error ("CreateRole():The instance of android sdk is invalid!!!");
				return;
			}
			SDKRoleBaseInfo _roleInfo = this._sdkInstance.CreateRoleInfo (
				iRoleId, iRoleName, iRoleLevel, iZoneID, iZoneName, iBalance, iVip, iPartyName, iRoleCTime, iRoleLevelMTime);
			if (null == _roleInfo) {
				this.Error ("CreateRole():Create Role Info Failed!!!");
				return;
			}
			this.Info ("CreateRole()::RoleInfo:{0}", _roleInfo.ToString());
			// 设定数据
			this._sdkInstance.CreateRole(_roleInfo);
		}

		/// <summary>
		/// 更新等级信息（升级时）.
		/// </summary>
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
		public void UpdateRoleInfoWhenLevelup(
			string iRoleId, string iRoleName, string iRoleLevel, string iZoneID, string iZoneName,
			string iBalance, string iVip, string iPartyName, string iRoleCTime, string iRoleLevelMTime) {

			if (null == this._sdkInstance) {
				this.Error ("UpdateRoleInfoWhenLevelup():The instance of android sdk is invalid!!!");
				return;
			}
			SDKRoleBaseInfo _roleInfo = this._sdkInstance.CreateRoleInfo (
				iRoleId, iRoleName, iRoleLevel, iZoneID, iZoneName, iBalance, iVip, iPartyName, iRoleCTime, iRoleLevelMTime);
			if (null == _roleInfo) {
				this.Error ("UpdateRoleInfoWhenLevelup():Create Role Info Failed!!!");
				return;
			}
			this.Info ("UpdateRoleInfoWhenLevelup()::RoleInfo:{0}", _roleInfo.ToString());

			// 设定数据
			this._sdkInstance.UpdateRoleInfoWhenLevelup(_roleInfo);
		}

		/// <summary>
		/// 更新角色信息（登录服务器后）.
		/// </summary>
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
		public void UpdateRoleInfoWhenEnterServer(
			string iRoleId, string iRoleName, string iRoleLevel, string iZoneID, string iZoneName,
			string iBalance, string iVip, string iPartyName, string iRoleCTime, string iRoleLevelMTime) {

			if (null == this._sdkInstance) {
				this.Error ("UpdateRoleInfoWhenEnterServer():The instance of android sdk is invalid!!!");
				return;
			}
			SDKRoleBaseInfo _roleInfo = this._sdkInstance.CreateRoleInfo (
				iRoleId, iRoleName, iRoleLevel, iZoneID, iZoneName, iBalance, iVip, iPartyName, iRoleCTime, iRoleLevelMTime);
			if (null == _roleInfo) {
				this.Error ("UpdateRoleInfoWhenEnterServer():Create Role Info Failed!!!");
				return;
			}
			this.Info ("UpdateRoleInfoWhenEnterServer()::RoleInfo:{0}", _roleInfo.ToString());

			// 设定数据
			this._sdkInstance.UpdateRoleInfoWhenEnterServer(_roleInfo);
		}

		/// <summary>
		/// 退出.
		/// </summary>
		/// <param name="iTarget">游戏对象.</param>
		/// <param name="iOnExited">退出回调函数.</param>
		public void Exit(
			GameObject iTarget = null, 
			Action<string> iOnExited = null) {
			if (null == this._sdkInstance) {
				this.Error ("Exit():The instance of android sdk is invalid!!!");
				return;
			}
			this._sdkInstance.Exit (iTarget, iOnExited);
		}
	}
}

#endif