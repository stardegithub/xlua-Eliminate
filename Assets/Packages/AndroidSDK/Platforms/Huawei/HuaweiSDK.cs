using UnityEngine;
using System;
using System.Collections;
using Common;
using AndroidSDK.Common;

#if UNITY_ANDROID

namespace AndroidSDK.Platforms.Huawei {

	/// <summary>
	/// 华为SDK状态.
	/// </summary>
	public enum HuaweiSDKResultStatus {
		/// <summary>
		/// 无效.
		/// </summary>
		Invalid = -1,
		/// <summary>
		/// OK.
		/// </summary>
		OK = 0,
		/// <summary>
		/// 签名错误.
		/// </summary>
		ErrSign = 1,
		/// <summary>
		/// 一般错误.
		/// </summary>
		ErrComm = 2,
		/// <summary>
		/// 游戏中心版本低，不支持.
		/// </summary>
		ErrUnSupport = 3,
		/// <summary>
		/// 参数错误(开发人员需要检查接口的 参数传入是否合法，参数是否为 null).
		/// </summary>
		ErrParam = 4,
		/// <summary>
		/// 网络错误.
		/// </summary>
		ErrNetWork = 5,
		/// <summary>
		/// 帐号鉴权失败(建议游戏收到此错误 码后退出游戏重新登录).
		/// </summary>
		ErrAuth = 6,
		/// <summary>
		/// 用户取消操作(比如用户取消安装游 戏中心，取消同意用户协议).
		/// </summary>
		ErrCancel = 7,
		/// <summary>
		/// 服务尚未初始化(游戏收到此错误码 后重新调用初始化接口).
		/// </summary>
		ErrNotInit = 8,
		/// <summary>
		/// 用户取消登录.
		/// </summary>
		ErrCancelLogin = 9,
		/// <summary>
		/// 无法拉起游戏中心.
		/// </summary>
		ErrBindHigame = 10,
		/// <summary>
		/// 未知类型.
		/// </summary>
		UnKnown
	}

	/// <summary>
	/// SDK账号信息.
	/// </summary>
	[Serializable]
	public class SDKHuaweiAccountInfo : SDKAccountBaseInfo
	{
		/// <summary>
		/// 玩家ID.
		/// </summary>
		public string PlayerId;

		/// <summary>
		/// 玩家等级.
		/// </summary>
		public int PlayerLevel;

		/// <summary>
		/// 玩家名.
		/// </summary>
		public string DisplayName;

		/// <summary>
		/// 账号切换标志位.
		/// </summary>
		public bool IsChange;

		public string toString() {
			return string.Format ("Status:{0} PlayerId:{1} PlayerLevel:{2} DisplayName:{3} IsChange:{4}",
				Status, PlayerId, PlayerLevel, DisplayName, IsChange);
		}
	}

	/// <summary>
	/// 华为SDK.
	/// </summary>
	public sealed class HuaweiSDK : AndroidSDKBase {

		/// <summary>
		/// 释放函数.
		/// </summary>
		protected override void SDKDispose() {
			this.loginCheckBaseUrl = null;
			this.loginCheckCallback = null;
			this.loginCheckSuccessed = null;
			this.loginCheckFailed = null;
		}

		/// <summary>
		/// 初始化SDK.
		/// </summary>
		/// <param name="iTarget">游戏对象.</param>
		/// <param name="iOnCompleted">完成回调函数.</param>
		protected override void SDKInit(
			GameObject iTarget, 
			Action<string> iOnCompleted) {

			if (null == iTarget) {
				this.Error ("SDKInit():The target is null!!!");
				return;
			}

			AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
			jo.Call ("UToA_SDKInit", iTarget.name, iOnCompleted.Method.Name);
		}

		/// <summary>
		/// 登陆.
		/// </summary>
		/// <param name="iTarget">游戏对象.</param>
		/// <param name="iOnCompleted">完成回调函数.</param>
		protected override void SDKLogin(
			GameObject iTarget, 
			Action<string> iOnCompleted) {

			if (null == iTarget) {
				return;
			}

			AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
			jo.Call ("UToA_Login", iTarget.name, iOnCompleted.Method.Name);
		}

		/// <summary>
		/// 重登录.
		/// </summary>
		protected override void SDKRelogin() {
			
		}

		/// <summary>
		/// 登出.
		/// </summary>
		protected override void SDKLogout() {
			AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
			jo.Call ("UToA_Logout");
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

			if (TPlatformType.None == BuildInfo.GetInstance ().PlatformType) {
				this.Error ("AddPlayerInfo():The platformType is none in buildinfo.asset file!!!");
				return;
			}

			if (false == Application.isMobilePlatform) {
				return;
			}

			AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
			if (null != iOnCompleted) {
				jo.Call ("UToA_AddPlayerInfo", 
					iTarget.name, 
					iGameRank, iGameRole, iGameArea, iGameSociaty,
					iOnCompleted.Method.Name);
			} else {
				jo.Call ("UToA_AddPlayerInfo", 
					iTarget.name, 
					iGameRank, iGameRole, iGameArea, iGameSociaty, null);
			}
		}

		/// <summary>
		/// 取得玩家信息.
		/// </summary>
		/// <param name="iTarget">游戏对象.</param>
		/// <param name="iOnCompleted">完成回调函数.</param>
		protected override void SDKGetPlayerInfo(
			GameObject iTarget, 
			Action<string> iOnCompleted = null) {

			if (TPlatformType.None == BuildInfo.GetInstance ().PlatformType) {
				this.Error ("GetPlayerInfo():The platformType is none in buildinfo.asset file!!!");
				return;
			}

			if (false == Application.isMobilePlatform) {
				return;
			}

			AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
			jo.Call ("UToA_GetPlayerInfo", 
				iTarget.name, 
				iOnCompleted.Method.Name);
		}

		/// <summary>
		/// SDK解析用户信息.
		/// </summary>
		/// <returns>用户信息.</returns>
		/// <param name="iUserInfo">用户信息(Json格式数据).</param>>
		protected override SDKAccountBaseInfo SDKParserAccountInfo (string iUserInfo) {
			return JsonUtility.FromJson<SDKHuaweiAccountInfo> (iUserInfo);
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
			return null;
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
			return null;
		}
			
		/// <summary>
		/// 创建角色.
		/// </summary>
		/// <param name="iRoleInfo">角色信息.</param>
		public override void SDKCreateRole (SDKRoleBaseInfo iRoleInfo) {}

		/// <summary>
		/// 更新等级信息（升级时）.
		/// </summary>
		/// <param name="iRoleInfo">角色信息.</param>
		public override void SDKUpdateRoleInfoWhenLevelup (SDKRoleBaseInfo iRoleInfo) {}

		/// <summary>
		/// 更新等级信息（升级时）.
		/// </summary>
		/// <param name="iRoleInfo">角色信息.</param>
		public override void SDKUpdateRoleInfoWhenEnterServer (SDKRoleBaseInfo iRoleInfo) {}

		/// <summary>
		/// 退出.
		/// </summary>
		/// <param name="iTarget">游戏对象.</param>
		/// <param name="iOnExited">退出回调函数.</param>
		public override void SDKExit(
			GameObject iTarget = null, 
			Action<string> iOnExited = null) {}
	}
}

#endif