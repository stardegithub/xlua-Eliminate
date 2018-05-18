#if UNITY_ANDROID
using UnityEngine;
using System;
using System.Collections;
using Common;
using AndroidSDK.Common;
using AndroidSDK.Options.OneSDK;
using BuildSystem.Options;
using BuildSystem.Options.OneSDK;

namespace AndroidSDK.Platforms.Tiange {

	/// <summary>
	/// 天鸽账号信息.
	/// </summary>
	[System.Serializable]
	public class TinageAccountInfo : SDKAccountBaseInfo {

		/// <summary>
		/// 用户ID.
		/// </summary>
		public JsonDataBase UserInfo;

		/// <summary>
		/// 清空.
		/// </summary>
		public override void Clear() {
			base.Clear ();

			if (null != this.UserInfo) {
				this.UserInfo.Clear ();
			}
		}
	}

	/// <summary>
	/// 天鸽支付信息.
	/// </summary>
	[System.Serializable]
	public class TiangePaymentInfo : SDKPaymentBaseInfo {
		
	}

	/// <summary>
	/// 天鸽SDK设定.
	/// </summary>
	public sealed class TiangeSDK : AndroidSDKBase {

		/// <summary>
		/// 释放函数.
		/// </summary>
		protected override void SDKDispose() {
			this.loginCheckBaseUrl = null;
			this.loginCheckCallback = null;
			this.loginCheckSuccessed = null;
			this.loginCheckFailed = null;

			// 易接SDK释放
			OneSDKLibs.Instance.Dispose ();
		}
			
		/// <summary>
		/// 设定状态更新回调函数.
		/// </summary>
		/// <param name="iUpdateStatus">I update status.</param>
		public override void SetUpdateStatusCallback(Action<SDKStatus> iUpdateStatus) {
			base.SetUpdateStatusCallback (iUpdateStatus);
			OneSDKLibs.Instance.SetUpdateStatusCallback (iUpdateStatus);
		}

		/// <summary>
		/// 初始化.
		/// </summary>
		/// <param name="iTarget">登陆启动的目标对象.</param>
		/// <param name="iOnCompleted">完成回调函数.</param>
		protected override void SDKInit (GameObject iTarget, System.Action<string> iOnCompleted) {
			if (null != iOnCompleted) {
				iOnCompleted (((int)SDKStatus.OK).ToString());
			}
		}

		/// <summary>
		/// 登录.
		/// </summary>
		/// <param name="iTarget">登陆启动的目标对象.</param>
		/// <param name="iOnCompleted">完成回调函数.</param>
		protected override void SDKLogin (
			GameObject iTarget, 
			Action<string> iOnCompleted) {
			this.Info ("SDKLogin()");

			// 接入易接SDK的场合
			if (true == BuildInfo.GetInstance ().Data.Options.isOptionValid (TSDKOptions.OneSDK)) {
				OneSDKLibs.Instance.Login (
					iTarget, iOnCompleted,
					this.loginCheckBaseUrl, this.loginCheckCallback,
					this.OnLoginCheckSuccessed, this.OnLoginCheckFailed,
					this.autoReloginMaxCount, this.autoReloginCallback);
			} else {
				this.Error("SDKLogin():There is invalid options settings in sdk settings!!!");
			}
		}
			
		/// <summary>
		/// 重登录.
		/// </summary>
		protected override void SDKRelogin() {
			this.Info ("SDKRelogin()");

			// 接入易接SDK的场合
			if (true == BuildInfo.GetInstance ().Data.Options.isOptionValid (TSDKOptions.OneSDK)) {
				OneSDKLibs.Instance.ReLogin ();
			} else {
				this.Error("SDKRelogin():There is invalid options settings in sdk settings!!!");
			}
		}

		/// <summary>
		/// 登出.
		/// </summary>
		protected override void SDKLogout() {
			this.Info ("SDKLogout()");

			// 接入易接SDK的场合
			if (true == BuildInfo.GetInstance ().Data.Options.isOptionValid (TSDKOptions.OneSDK)) {
				OneSDKLibs.Instance.Logout ();
			} else {
				this.Error("SDKLogout():There is invalid options settings in sdk settings!!!");
			}

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
		protected override void SDKAddPlayerInfo(
			GameObject iTarget, 
			String iGameRank,
			String iGameRole,
			String iGameArea,
			String iGameSociaty,
			Action<string> iOnCompleted = null) {

			// 天鸽不添加角色信息
			if (null != iOnCompleted) {
				iOnCompleted (null);
			}
		}

		/// <summary>
		/// 取得玩家信息.
		/// </summary>
		/// <param name="iTarget">游戏对象.</param>
		/// <param name="iOnCompleted">完成回调函数.</param>
		protected override void SDKGetPlayerInfo (
			GameObject iTarget, Action<string> iOnCompleted)
		{
			if (null != iOnCompleted) {
				iOnCompleted (null);
			}
		}
			
		/// <summary>
		/// SDK解析用户信息.
		/// </summary>
		/// <returns>用户信息.</returns>
		/// <param name="iUserInfo">用户信息(Json格式数据).</param>>
		protected override SDKAccountBaseInfo SDKParserAccountInfo (string iUserInfo) {
			// 接入易接SDK的场合
			if (true == BuildInfo.GetInstance ().Data.Options.isOptionValid (TSDKOptions.OneSDK)) {
				return OneSDKLibs.Instance.ParserLoginResponseInfo (iUserInfo);
			} else {
				return JsonUtility.FromJson<TinageAccountInfo> (iUserInfo);
			}
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
		protected override void SDKPay (
			GameObject iTarget, 
			int iIAPItemPrice,
			string iIAPItemName,
			int iIAPItemCount, 
			string iOtherDIYInfo, 
			string iNotifyUrl, 
			Action<string> iOnPayCompleted) {
			this.Info ("SDKPay()::Price:{0} Name:{1} Count:{2} OtherDIYInfo:{3} NotifyUrl:{4}/ Target:{5} Callback:{6}",
				iIAPItemPrice, iIAPItemName, iIAPItemCount, iOtherDIYInfo, iNotifyUrl,
				iTarget.name, iOnPayCompleted.Method.Name);

			// 接入易接SDK的场合
			if (true == BuildInfo.GetInstance ().Data.Options.isOptionValid (TSDKOptions.OneSDK)) {
				OneSDKLibs.Instance.Pay (
					iTarget, iIAPItemPrice, iIAPItemName, iIAPItemCount,
					iOtherDIYInfo, iNotifyUrl, iOnPayCompleted);
			} else {
				this.Error("SDKLogout():There is invalid options settings in sdk settings!!!");
			}
		}

		/// <summary>
		/// 解析支付信息.
		/// </summary>
		/// <returns>支付信息.</returns>
		/// <param name="iPayInfo">支付信息(Json格式数据).</param>
		/// <param name="iOnPaymentSuccessed">支付成功回调函数.</param>
		protected override SDKPaymentBaseInfo SDKParserPaymentInfo(
			string iPayInfo, 
			Action<SDKAccountBaseInfo, string> iOnPaymentSuccessed) {

#if IAP_UI_TEST
			iOnPaymentSuccessed(null, null);
			return null;
#else
			if (null == iOnPaymentSuccessed) {
				this.Warning ("SDKParserPaymentInfo()::OnPaymentSuccessed is null!!!");
			}

			// 接入易接SDK的场合
			if (true == BuildInfo.GetInstance ().Data.Options.isOptionValid (TSDKOptions.OneSDK)) {
				return OneSDKLibs.Instance.ParserPayResponseInfo (iPayInfo, iOnPaymentSuccessed);
			} else {
				return JsonUtility.FromJson<TiangePaymentInfo> (iPayInfo);
			}
#endif
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
		public override SDKRoleBaseInfo SDKCreateRoleInfo (
			string iRoleId, string iRoleName, string iRoleLevel, string iZoneID, string iZoneName,
			string iBalance, string iVip, string iPartyName, string iRoleCTime, string iRoleLevelMTime) {
			// 接入易接SDK的场合
			if (true == BuildInfo.GetInstance ().Data.Options.isOptionValid (TSDKOptions.OneSDK)) {
				OneSDKRoleInfo _roleIfo = new OneSDKRoleInfo ();
				if (null == _roleIfo) {
					this.Error ("SDKCreateRoleInfo()::Memory New Error!!!(OneSDKRoleInfo)");
					return null;
				}
				_roleIfo.Reset ();

				_roleIfo.ID = iRoleId;
				_roleIfo.Name = iRoleName;
				_roleIfo.Level = iRoleLevel;
				_roleIfo.ZoneID = iZoneID;
				_roleIfo.ZoneName = iZoneName;
				_roleIfo.Balance = iBalance;
//				_roleIfo.Vip = iVip;
//				_roleIfo.PartyName = iPartyName;
				_roleIfo.CTime = iRoleCTime;
				_roleIfo.RoleLevelMTime = iRoleLevelMTime;
				return _roleIfo;
			}
			return null;
		}

		/// <summary>
		/// 创建角色信息.
		/// </summary>
		/// <returns>角色信息.</returns>
		public static SDKRoleBaseInfo CreateRoleInfo() {
			SDKRoleBaseInfo _roleInfo = null;
			if (true == BuildInfo.GetInstance ().Data.Options.isOptionValid (TSDKOptions.OneSDK)) {
				OneSDKRoleInfo _roleInfoTmp = new OneSDKRoleInfo ();


				_roleInfo = _roleInfoTmp;
			}
			return _roleInfo;
		}
			
		/// <summary>
		/// 创建角色.
		/// </summary>
		/// <param name="iRoleInfo">角色信息.</param>
		public override void SDKCreateRole (SDKRoleBaseInfo iRoleInfo) {
			// 接入易接SDK的场合
			if (true == BuildInfo.GetInstance ().Data.Options.isOptionValid (TSDKOptions.OneSDK)) {
				 OneSDKLibs.Instance.CreateRole (((OneSDKRoleInfo)iRoleInfo));
			}
		}

		/// <summary>
		/// 更新等级信息（升级时）.
		/// </summary>
		/// <param name="iRoleInfo">角色信息.</param>
		public override void SDKUpdateRoleInfoWhenLevelup (SDKRoleBaseInfo iRoleInfo) {
			// 接入易接SDK的场合
			if (true == BuildInfo.GetInstance ().Data.Options.isOptionValid (TSDKOptions.OneSDK)) {
				 OneSDKLibs.Instance.UpdateRoleInfoWhenLevelup (((OneSDKRoleInfo)iRoleInfo));
			}
		}

		/// <summary>
		/// 更新等级信息（升级时）.
		/// </summary>
		/// <param name="iRoleInfo">角色信息.</param>
		public override void SDKUpdateRoleInfoWhenEnterServer (SDKRoleBaseInfo iRoleInfo) {
			// 接入易接SDK的场合
			if (true == BuildInfo.GetInstance ().Data.Options.isOptionValid (TSDKOptions.OneSDK)) {
				 OneSDKLibs.Instance.UpdateRoleInfoWhenEnterServer (((OneSDKRoleInfo)iRoleInfo));
			}
		}

		/// <summary>
		/// 退出.
		/// </summary>
		/// <param name="iTarget">游戏对象.</param>
		/// <param name="iOnExited">退出回调函数.</param>
		public override void SDKExit(
			GameObject iTarget = null, 
			Action<string> iOnExited = null) {
			// 接入易接SDK的场合
			if (true == BuildInfo.GetInstance ().Data.Options.isOptionValid (TSDKOptions.OneSDK)) {
				OneSDKLibs.Instance.Exit (iTarget, iOnExited);
			}
		}
	}
}

#endif

