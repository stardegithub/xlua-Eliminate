using UnityEngine;
using System;
using System.Collections;

namespace AndroidSDK.Common {

	/// <summary>
	/// SDK状态.
	/// </summary>
	public enum SDKResultStatus {
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
	public class SDKAccountInfo
	{
		/// <summary>
		/// 成功标志位.
		/// </summary>
		public SDKResultStatus Status;

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
	/// AndroidSDK接口.
	/// </summary>
	public interface IAndroidSDK {

		/// <summary>
		/// 登录.
		/// </summary>
		/// <param name="iTarget">登陆启动的目标对象.</param>
		/// <param name="iLoginCallback">登录回调函数.</param>
		void Login (GameObject iTarget, System.Action<string> iLoginCallback);

		/// <summary>
		/// 登出.
		/// </summary>
		void Logout();
	}
}
