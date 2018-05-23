#if UNITY_ANDROID
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Net;
using System.Collections;
using System.Runtime.InteropServices;
using Common;
using AndroidSDK.Common;

namespace AndroidSDK.Options.OneSDK {

	/// <summary>
	/// One SDK login check info.
	/// </summary>
	[Serializable]
	public class OneSDKLoginCheckInfo : JsonDataBase {
		/// <summary>
		/// Product Code.
		/// </summary>
		public string app;
		/// <summary>
		/// 渠道ID.
		/// </summary>
		public string sdk;
		/// <summary>
		/// 渠道用户ID.
		/// </summary>
		public string uin;
		/// <summary>
		/// 用户登录相关渠道后的Session ID.
		/// </summary>
		public string sess;
		/// <summary>
		///  渠道ID.
		/// </summary>
		public string channel;
		/// <summary>
		/// 平台类型.
		/// </summary>
		public string platform;
		/// <summary>
		/// 选项.
		/// </summary>
		public string option;

		/// <summary>
		/// 清空.
		/// </summary>
		public override void Clear() {
			base.Clear ();
			app = null;
			sdk = null;
			uin = null;
			sess = null;
			channel = null;
			platform = null;
			option = null;
		}

		/// <summary>
		/// 应用.
		/// </summary>
		/// <param name="iUserInfo">用户数据.</param>
		public void Apply(OneSDKUserInfo iUserInfo) {
			this.Reset ();

			this.app = iUserInfo.ProductCode;
			this.sdk = iUserInfo.ChannelId;
			this.uin = iUserInfo.ChannelUserId;
			this.sess = iUserInfo.Token;
			this.channel = iUserInfo.ChannelId;
			this.platform = BuildInfo.GetInstance ().PlatformType.ToString ();
			this.option = "OneSDK";
		}

		public override String ToString ()
		{
			return string.Format ("{0} app:{1} sdk:{2} uin:{3} sess:{4} channel:{5} platform:{6} option:{7}",
				base.ToString(), app, sdk, uin, sess, channel, platform, option);
		}
	}

	/// <summary>
	/// 易接用户信息.
	/// </summary>
	[Serializable]
	public class OneSDKUserInfo : SDKAccountBaseInfo {
		/// <summary>
		/// 易接内部 userid，该值可能为 0，请不要以此参数作为判定
		/// </summary>
		public long UserID;
		/// <summary>
		/// 易接平台标示的渠道SDK ID.
		/// </summary>
		public string ChannelId;
		/// <summary>
		/// 渠道SDK标示的用户ID.
		/// </summary>
		public string ChannelUserId;
		/// <summary>
		/// 渠道SDK的用户名称.
		/// </summary>
		public string UserName;
		/// <summary>
		/// 渠道SDK登录完成后的SessionID.
		/// 特别提醒醒:
		/// 部分渠道此参数会包含特殊值如‘+’，空格之类的，
		/// 如直接使用URL参数传输到游戏服务器请求校验，
		/// 请使用 URLEncoder 编码。
		/// </summary>
		public string Token;
		/// <summary>
		/// 易接平台创建的游戏ID（appId）.
		/// </summary>
		public string ProductCode;

		/// <summary>
		/// 清空.
		/// </summary>
		public override void Clear() {
			base.Clear ();
			UserID = -1;
			ChannelId = null;
			ChannelUserId = null;
			UserName = null;
			Token = null;
			ProductCode = null;
		}

		/// <summary>
		/// 初始化.
		/// </summary>
		public override void Init() {
			base.Init ();
			UserID = -1;
			ChannelId = null;
			ChannelUserId = null;
			UserName = null;
			Token = null;
			ProductCode = null;
		}

		public override string ToString () {
			return string.Format("{0} UserID:{1} ChannelId:{2} ChannelUserId:{3} UserName:{4} Token:{5} ProductCode:{6}", 
				base.ToString(), UserID, 
				(true == string.IsNullOrEmpty(ChannelId)) ? "null" : ChannelId, 
				(true == string.IsNullOrEmpty(ChannelUserId)) ? "null" : ChannelUserId, 
				(true == string.IsNullOrEmpty(UserName)) ? "null" : UserName, 
				(true == string.IsNullOrEmpty(Token)) ? "null" : Token, 
				(true == string.IsNullOrEmpty(ProductCode)) ? "null" : ProductCode);
		}
	}

	/// <summary>
	/// 易接支付信息.
	/// </summary>
	[Serializable]
	public class OneSDKPaymentInfo : SDKPaymentBaseInfo {

		/// <summary>
		/// 订单号.
		/// </summary>
		public string OrderNo;

		/// <summary>
		/// 清空.
		/// </summary>
		public override void Clear() {
			base.Clear ();
			this.OrderNo = null;
		}

		public override string ToString () {
			return string.Format("{0} OrderNo:{1}", 
				base.ToString(), OrderNo);
		}
	}

	/// <summary>
	/// 易接角色信息.
	/// </summary>
	[Serializable]
	public class OneSDKRoleInfo : SDKRoleBaseInfo {

		/// <summary>
		/// 当前用户VIP等级，必须为数字，若无，传入1.
		/// </summary>
		public string Vip;

		/// <summary>
		/// 当前角色所属帮派，不能为空，不能为null，若无， 传入“无帮派”.
		/// </summary>
		public string PartyName;

		/// <summary>
		/// 角色等级变化时间(单位：秒).
		/// </summary>
		public string RoleLevelMTime;

		/// <summary>
		/// 清空.
		/// </summary>
		public override void Clear() {
			base.Clear ();
			Vip = "1";
			PartyName = "无帮派";
			RoleLevelMTime = null;
		}

		public override string ToString () {
			return string.Format("{0} Vip:{1} PartyName:{2} RoleLevelMTime:{3}", 
				base.ToString(),
				(true == string.IsNullOrEmpty(Vip)) ? "null" : Vip, 
				(true == string.IsNullOrEmpty(PartyName)) ? "null" : PartyName, 
				(true == string.IsNullOrEmpty(RoleLevelMTime)) ? "null" : RoleLevelMTime);
		}
	}
	
	/// <summary>
	/// 易接SDK库.
	/// </summary>
	public sealed class OneSDKLibs : SingletonBase<OneSDKLibs>, IDisposable {

		/// <summary>
		/// 登出.
		/// </summary>
		private static readonly string S_LOGOUT = "0";

		/// <summary>
		/// 登录成功.
		/// </summary>
		private static readonly string S_LOGIN_SUCCESS = "1";

		/// <summary>
		/// 登录失败.
		/// </summary>
		private static readonly string S_LOGIN_FAILED = "2";

		/// <summary>
		/// 支付成功.
		/// </summary>
		private static readonly string S_PAY_SUCCESS = "0";

		/// <summary>
		/// 支付失败.
		/// </summary>
		private static readonly string S_PAY_FAILED = "1";

		/// <summary>
		/// 支付订单No.
		/// </summary>
		private static readonly string S_PAY_ORDER_NO = "2";

		/// <summary>
		/// 用户信息(易接).
		/// </summary>
		private SFOnlineUser _sfUserInfo = null;

		/// <summary>
		/// 用户信息.
		/// </summary>
		private OneSDKUserInfo _userInfo = null;

		/// <summary>
		/// 支付信息.
		/// </summary>
		private OneSDKPaymentInfo _payment = null;

		/// <summary>
		/// 登录标志位.
		/// </summary>
		private bool _isLogin = false;

		/// <summary>
		/// 状态更新函数.
		/// </summary>
		private Action<SDKStatus> _updateStatusCallback = null;

		/// <summary>
		/// 目标游戏对象(挂接相关脚本的游戏对象).
		/// </summary>
		private GameObject _targetGameObject = null;
	
		/// <summary>
		/// 登录校验Base Url.
		/// </summary>
		private string _loginCheckBaseUrl = null;

		/// <summary>
		/// 登录校验回调函数.
		/// </summary>
		private Action<string, Action<string>, Action<string>> _loginCheckCallback = null;

		/// <summary>
		/// 登录校验成功回调函数.
		/// </summary>
		private Action<string> _loginCheckSuccessed = null;

		/// <summary>
		/// 登录校验失败回调函数.
		/// </summary>
		private Action<string> _loginCheckFailed = null;

		/// <summary>
		/// 自动重登最大次数.
		/// </summary>
		private int _autoReloginMaxCount = 3;

		/// <summary>
		/// 自动重登次数.
		/// </summary>
		private int _autoReloginCount = 0;

		/// <summary>
		/// 自动重登回调函数.
		/// </summary>
		private Action<float> _autoReloginCallback = null;

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
		/// 释放函数.
		/// </summary>
		public void Dispose() {
			this._targetGameObject = null;
			this._updateStatusCallback = null;
			this._loginCheckBaseUrl = null;
			this._loginCheckCallback = null;
			this._loginCheckSuccessed = null;
			this._loginCheckFailed = null;
			this._autoReloginCallback = null;
		}

		/// <summary>
		/// 更新状态.
		/// </summary>
		/// <param name="iStatus">状态.</param>
		private void UpdateStatus(SDKStatus iStatus) {
			if (null != this._updateStatusCallback) {
				this._updateStatusCallback (iStatus);
			}
			if (null != this._userInfo) {
				this._userInfo.Status = iStatus;
			}
			if (null != this._payment) {
				this._payment.Status = iStatus;
			}
		}
			
		/// <summary>
		/// 设定状态更新回调函数.
		/// </summary>
		/// <param name="iUpdateStatus">I update status.</param>
		public void SetUpdateStatusCallback(Action<SDKStatus> iUpdateStatus) {
			this._updateStatusCallback = iUpdateStatus;
		}

		/// <summary>
		/// 登录.
		/// </summary>
		/// <param name="iTarget">登陆启动的目标对象.</param>
		/// <param name="iLoginCompleted">登录/登出完成回调函数.</param>
		/// <param name="iLoginCheckBaseUrl">登录校验Base Url.</param>
		/// <param name="iLoginCheckCallBack">登录校验回调函数.</param>
		/// <param name="iLoginCheckSuccessed">登录校验成功回调函数.</param>
		/// <param name="iLoginCheckFailed">登录校验失败回调函数.</param>
		/// <param name="iAutoReloginMaxCount">自动重登最大次数.</param>
		/// <param name="iAutoReloginCallback">自动重登回调函数.</param>
		public void Login(
			GameObject iTarget = null, 
			Action<string> iLoginCompleted = null, string iLoginCheckBaseUrl = null,
			Action<string, Action<string>, Action<string>> iLoginCheckCallBack = null,
			Action<string> iLoginCheckSuccessed = null, Action<string> iLoginCheckFailed = null,
			int iAutoReloginMaxCount = 3, Action<float> iAutoReloginCallback = null) {

			if (null == iTarget) {
				return;
			}
			this._targetGameObject = iTarget;

			AndroidJavaClass _unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
			if (null == _unityPlayer) {
				this.Error ("Login():The unity player is invalid!!!");
				return;
			}
			AndroidJavaObject _curActivity = _unityPlayer.GetStatic<AndroidJavaObject> ("currentActivity");
			if (null == _curActivity) {
				this.Error ("Login():The current activity is invalid!!!");
				return;
			}
			this.Info ("Login()");
			this.UpdateStatus (SDKStatus.Logining);

			// 登录校验Base Url
			if(false == string.IsNullOrEmpty(iLoginCheckBaseUrl)) {
				this._loginCheckBaseUrl = iLoginCheckBaseUrl;

				this.Info ("Login()::LoginCheckBaseUrl:{0}", 
					this._loginCheckBaseUrl);
			}

			// 设置登录校验回调函数
			if (null != iLoginCheckCallBack) {
				this._loginCheckCallback = iLoginCheckCallBack;
			}
			if (null != iLoginCheckSuccessed) {
				this._loginCheckSuccessed = iLoginCheckSuccessed;
			}
			if (null != iLoginCheckFailed) {
				this._loginCheckFailed = iLoginCheckFailed;
			}
			this._autoReloginMaxCount = iAutoReloginMaxCount;
			if (null != iAutoReloginCallback) {
				this._autoReloginCallback = iAutoReloginCallback;
			}

			if ((null != iTarget) && (null != iLoginCompleted)) {
				
				this.Info ("Login()::TargetName:{0} LoginCallback:{1}", 
					iTarget.name, iLoginCompleted.Method.Name);
				
				// 设定回调函数
				setLoginListener (_curActivity.GetRawObject (), iTarget.name, iLoginCompleted.Method.Name);
			}
			this._isLogin = true;
			login (_curActivity.GetRawObject (), "Login");

		}

		/// <summary>
		/// 重登录.
		/// </summary>
		public void ReLogin() {
			this.Info ("ReLogin():: --> {0}/{1}", this._autoReloginCount, this._autoReloginMaxCount);
			this.UpdateStatus (SDKStatus.AutoReLogining);

			// 超过重登最大尝试次数
			if (this._autoReloginMaxCount < this._autoReloginCount) {
				this.Error ("ReLogin()::Over --> {0}/{1}", this._autoReloginCount, this._autoReloginMaxCount);
				this.UpdateStatus (SDKStatus.AutoReLoginFailed);
				return;
			}

			if (null != this._autoReloginCallback) {
				this._autoReloginCallback (0.2f);
			} else {
				++_autoReloginCount;
				this.Login (this._targetGameObject);
			}
		}

		/// <summary>
		/// 登出.
		/// </summary>
		public void Logout() {
			AndroidJavaClass _unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
			if (null == _unityPlayer) {
				this.Error ("Logout()::The unity player is invalid!!!");
				return;
			}
			AndroidJavaObject _curActivity = _unityPlayer.GetStatic<AndroidJavaObject> ("currentActivity");
			if (null == _curActivity) {
				this.Error ("Logout()::The current activity is invalid!!!");
				return;
			}
			this.Info ("Logout()");

			this._isLogin = false;
			logout(_curActivity.GetRawObject (), "Logout");
			this.UpdateStatus (SDKStatus.Logouted);
		}

		/// <summary>
		/// 解析登录信息.
		/// </summary>
		/// <returns>Json数据对象.</returns>
		/// <param name="iJsonDetail">Json详细.</param>
		public SDKAccountBaseInfo ParserLoginResponseInfo (string iJsonDetail) {

			// 登录完成
			this.UpdateStatus (SDKStatus.LoginCompleted);

			if (null == this._userInfo) {
				this._userInfo = new OneSDKUserInfo ();
			}
			if (null == this._userInfo) {
				this.Error ("ParserLoginResponseInfo()::Memory New Error!!!!");
				return this._userInfo;
			}
			this._userInfo.Reset ();

			SFJSONObject _sfjson = new SFJSONObject (iJsonDetail);
			if (null == _sfjson) {
				return null;
			}
			string _status = (string)_sfjson.get ("result");
			string _customParams = (string)_sfjson.get ("customParams");
			this.Info ("ParserLoginResponseInfo()::CustomParams:{0}", _customParams);

			// 登出
			if (true == S_LOGOUT.Equals(_status)) {

				this._sfUserInfo = null;
				this.UpdateStatus (SDKStatus.Logouted);

				// 重登录
				if(true == this._isLogin) {
					this.ReLogin ();
				}

				// 登录成功
			} else if (true == S_LOGIN_SUCCESS.Equals(_status)) {
				
				this.UpdateStatus (SDKStatus.LoginCompleted);

				SFJSONObject _userinfoTmp = (SFJSONObject)_sfjson.get ("userinfo");
				if (null != _userinfoTmp) {
					this._userInfo.UserID = long.Parse ((string)_userinfoTmp.get ("id"));
					this._userInfo.ChannelId = (string)_userinfoTmp.get ("channelid");
					this._userInfo.ChannelUserId = (string)_userinfoTmp.get ("channeluserid");
					this._userInfo.UserName = (string)_userinfoTmp.get ("username");
					this._userInfo.Token = (string)_userinfoTmp.get ("token");
					this._userInfo.ProductCode = (string)_userinfoTmp.get ("productcode");
					this._sfUserInfo = new SFOnlineUser (
						this._userInfo.UserID, 
						this._userInfo.ChannelId, 
						this._userInfo.ChannelUserId,
						this._userInfo.UserName, 
						this._userInfo.Token, 
						this._userInfo.ProductCode);
				}
				if (null != this._loginCheckCallback) {

					// 校验数据
					string _checkInfo = GetLoginCheckInfoJson(this._userInfo);
					if (true == string.IsNullOrEmpty (_checkInfo)) {
						this.Error ("ParserLoginResponseInfo():JsonConvert Failed!!!(Data:{0})",
							this._userInfo.ToString());
						this.UpdateStatus(SDKStatus.LoginCheckFailed);
						return null;
					}
					this.Info ("ParserLoginResponseInfo()::CheckInfo:{0}", _checkInfo);
					this._loginCheckCallback (_checkInfo, this._loginCheckSuccessed, this._loginCheckFailed);
				} else {

					// 登录校验&更新状态
					loginCheck (this._userInfo);
				}

				// 登录失败
			} else if (true == S_LOGIN_FAILED.Equals(_status)) { 
				this.UpdateStatus (SDKStatus.LoginFailed);
				// 重登录
				if(true == this._isLogin) {
					this.ReLogin ();
				}
			} 
			this.Info ("ParserLoginResponseInfo()::UserResultInfo:{0}", this._userInfo.ToString());
			return this._userInfo;
		}

		/// <summary>
		/// 取得登录校验用信息（Json格式）.
		/// </summary>
		/// <returns>登录校验用信息（Json格式）.</returns>
		/// <param name="iUserInfo">易接用户信息.</param>
		private string GetLoginCheckInfoJson(OneSDKUserInfo iUserInfo) {
			OneSDKLoginCheckInfo _checkInfo = new OneSDKLoginCheckInfo ();
			if (null == _checkInfo) {
				this.UpdateStatus(SDKStatus.LoginCheckFailed);
				return null;
			}
			_checkInfo.Apply (iUserInfo);

			// 详细数据
			string _jsonData = UtilsJson<OneSDKLoginCheckInfo>.ConvertToJsonString(_checkInfo);
			if (true == string.IsNullOrEmpty (_jsonData)) {
				this.Error ("createLoginCheckURL():JsonConvert Failed!!!(Data:{0})",
					_checkInfo.ToString());
				this.UpdateStatus(SDKStatus.LoginCheckFailed);
				return null;
			}
			return _jsonData;
		}

		/// <summary>
		/// 登录校验.
		/// </summary>
		/// <returns>校验状态.</returns>
		/// <param name="iUserInfo">用户信息.</param>
		private void loginCheck (OneSDKUserInfo iUserInfo) {

			this.UpdateStatus(SDKStatus.LoginChecking);
			if (true == string.IsNullOrEmpty (this._loginCheckBaseUrl)) {
				this.Error ("loginCheck()::The base url of login check is invalid!!!");
				this.UpdateStatus(SDKStatus.LoginCheckFailed);
				return;
			}

			string _loginCheckUrl = this._loginCheckBaseUrl;
			this.Info ("loginCheck()::Url:{0}", _loginCheckUrl);
			if (true == string.IsNullOrEmpty (_loginCheckUrl)) {
				this.Error ("loginCheck()::The url of login check is invalid!!!");
				this.UpdateStatus(SDKStatus.LoginCheckFailed);
				return;
			}

			// 详细数据
			string _jsonData = GetLoginCheckInfoJson(iUserInfo);
			if (true == string.IsNullOrEmpty (_jsonData)) {
				this.Error ("createLoginCheckURL():JsonConvert Failed!!!(Data:{0})",
					iUserInfo.ToString());
				this.UpdateStatus(SDKStatus.LoginCheckFailed);
				return;
			}

			HttpWebResponse _response = executeHttpPost (_loginCheckUrl, _jsonData);
			if (null == _response) {
				this.UpdateStatus(SDKStatus.LoginCheckFailed);
				return;
			}
			this.Info ("loginCheck()::Result:{0}", _response.StatusCode);
			if (HttpStatusCode.OK != _response.StatusCode) {
				if (null != this._loginCheckFailed) {
					this._loginCheckFailed (_response.StatusCode.ToString());
				}
				this.UpdateStatus(SDKStatus.LoginCheckFailed);
				return;
			}

			if (null != this._loginCheckSuccessed) { 
				this._loginCheckSuccessed (_response.StatusCode.ToString());
			}

			this.UpdateStatus(SDKStatus.LoginCheckSuccessed);
		}
			
		private static string ToUrlEncode (string strCode)
		{ 
			StringBuilder sb = new StringBuilder (); 
			byte[] byStr = System.Text.Encoding.UTF8.GetBytes (strCode); //默认是System.Text.Encoding.Default.GetBytes(str) 
			System.Text.RegularExpressions.Regex regKey = new System.Text.RegularExpressions.Regex ("^[A-Za-z0-9]+$"); 
			for (int i = 0; i < byStr.Length; i++) { 
				string strBy = Convert.ToChar (byStr [i]).ToString (); 
				if (regKey.IsMatch (strBy)) { 
					//是字母或者数字则不进行转换  
					sb.Append (strBy); 
				} else { 
					sb.Append (@"%" + Convert.ToString (byStr [i], 16)); 
				} 
			} 
			return (sb.ToString ()); 
		}

		private HttpWebResponse executeHttpPost (string iUrl, string iJsonDataDetail)
		{
			HttpWebRequest request = null;
			//如果是发送HTTPS请求  
			if (true == iUrl.StartsWith("https", StringComparison.OrdinalIgnoreCase))
			{
				request = WebRequest.Create(iUrl) as HttpWebRequest;
			}
			else
			{
				request = WebRequest.Create(iUrl) as HttpWebRequest;
			}
			request.Method = "POST";
			request.ContentType = "application/json";
			request.Headers ["X-MC-GAME-ID"] = "2";

			//发送POST数据  
			StringBuilder buffer = new StringBuilder();
			buffer.AppendFormat("data={0}", ToUrlEncode(iJsonDataDetail));
			this.Info ("executeHttpPost()::Params:{0}", buffer.ToString());

			byte[] data = Encoding.UTF8.GetBytes(buffer.ToString());
			using (Stream stream = request.GetRequestStream())
			{
				stream.Write(data, 0, data.Length);
			}
			// string[] values = request.Headers.GetValues("Content-Type");
			return request.GetResponse() as HttpWebResponse;
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
			Action<string> iOnPayCompleted) {if (null == iTarget) {
				return;
			}
			this._targetGameObject = iTarget;

			AndroidJavaClass _unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
			if (null == _unityPlayer) {
				this.Error ("Pay():The unity player is invalid!!!");
				return;
			}
			AndroidJavaObject _curActivity = _unityPlayer.GetStatic<AndroidJavaObject> ("currentActivity");
			if (null == _curActivity) {
				this.Error ("Pay():The current activity is invalid!!!");
				return;
			}
			this.Info ("Pay()");
			this.UpdateStatus (SDKStatus.Purchasing);

			// 支付
			pay (_curActivity.GetRawObject (), 
				iTarget.name, iIAPItemPrice, iIAPItemName, 
				iIAPItemCount, iOtherDIYInfo, iNotifyUrl, 
				iOnPayCompleted.Method.Name);

		}
			
		/// <summary>
		/// 解析支付信息.
		/// </summary>
		/// <returns>Json数据对象.</returns>
		/// <param name="iJsonDetail">Json详细.</param>
		/// <param name="iOnPaymentSuccessed">支付成功回调函数.</param>
		public SDKPaymentBaseInfo ParserPayResponseInfo (
			string iJsonDetail, 
			Action<SDKAccountBaseInfo, string> iOnPaymentSuccessed) {

			if (null == iOnPaymentSuccessed) {
				this.Warning ("SDKParserPaymentInfo()::OnPaymentSuccessed is null!!!");
			}

			this.UpdateStatus (SDKStatus.PurchaseCompleted);
			this.Info ("ParserPayResponseInfo()::Detail:{0}", iJsonDetail);
			SFJSONObject _sfjson = new SFJSONObject (iJsonDetail);
			if (null == _sfjson) {
				this.Error ("ParserPayResponseInfo()::(SFJSONObject)Memory New Error!!!!");
				return null;
			}
			string _status = (string)_sfjson.get ("result");
			if (true == string.IsNullOrEmpty (_status)) {
				this.Error ("ParserPayResponseInfo()::Data Format Invalid!!!!(Detail:{0})", iJsonDetail);
				return null;
			}
			string _data = (string)_sfjson.get ("data");
			if (true == string.IsNullOrEmpty (_data)) {
				this.Error ("ParserPayResponseInfo()::Data Format Invalid!!!!(Detail:{0})", iJsonDetail);
				return null;
			}

			if (null == this._payment) {
				this._payment = new OneSDKPaymentInfo ();
				this._payment.Reset ();
			}
			if (null == this._payment) {
				this.Error ("ParserPayResponseInfo()::(OneSDKPaymentInfo)Memory New Error!!!!");
				return null;
			}

			if (true == S_PAY_SUCCESS.Equals(_status)) {
				this.UpdateStatus (SDKStatus.PurchaseSuccessed);
				this._payment.Successed = true;

				if (null != iOnPaymentSuccessed) {
					iOnPaymentSuccessed (this._userInfo, this._payment.OrderNo);
				} else {
					this.Warning ("ParserPayResponseInfo()::OnPaymentSuccessed is null!!!!");
				}
			} else if (true == S_PAY_FAILED.Equals(_status)) {
				this.UpdateStatus (SDKStatus.PurchaseFailed);
				this._payment.Successed = false;
			} else if (true == S_PAY_ORDER_NO.Equals(_status)) {
				this.UpdateStatus (SDKStatus.PurchaseOrdered);
				this._payment.Successed = true;
				this._payment.OrderNo = _data;
			}
			this.Info ("ParserPayResponseInfo()::PayInfo:{0}", this._payment.ToString());
			return this._payment;
		}

		/// <summary>
		/// 设定角色信息.
		/// </summary>
		/// <param name="iRoleId">角色ID.</param>
		/// <param name="iRoleName">角色名.</param>
		/// <param name="roleLevel">角色等级.</param>
		/// <param name="iZoneId">游戏区ID.</param>
		/// <param name="iZoneName">游戏区名.</param>
		public void SetRoleData(
			string iRoleId, string iRoleName, 
			string iRoleLevel, string iZoneId, string iZoneName) {
			AndroidJavaClass _unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
			if (null == _unityPlayer) {
				this.Error ("SetRoleData():The unity player is invalid!!!");
				return;
			}
			AndroidJavaObject _curActivity = _unityPlayer.GetStatic<AndroidJavaObject> ("currentActivity");
			if (null == _curActivity) {
				this.Error ("SetRoleData():The current activity is invalid!!!");
				return;
			}
			this.Info ("SetRoleData()");

			setRoleData (_curActivity.GetRawObject (),
				iRoleId, iRoleName, iRoleLevel, iZoneId, iZoneName);

		}

		/// <summary>
		/// 创建角色.
		/// </summary>
		/// <param name="iKey">Key.</param>
		/// <param name="iRoleInfo">角色信息.</param>
		private void SetData(
			string iKey, OneSDKRoleInfo iRoleInfo) {

			AndroidJavaClass _unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
			if (null == _unityPlayer) {
				this.Error ("SetData():The unity player is invalid!!!");
				return;
			}
			AndroidJavaObject _curActivity = _unityPlayer.GetStatic<AndroidJavaObject> ("currentActivity");
			if (null == _curActivity) {
				this.Error ("SetData():The current activity is invalid!!!");
				return;
			}
			this.Info ("SetData()::Key:{0} RoleInfo:{1}",iKey, iRoleInfo.ToString());
			SFJSONObject _roleInfo = new SFJSONObject ();
			if (null == _roleInfo) {this.Error ("CreateRole():Memory New Error(SFJSONObject)!!!");
				return;
			}
			_roleInfo.put("roleId", iRoleInfo.ID);
			_roleInfo.put("roleName", iRoleInfo.Name);
			_roleInfo.put("roleLevel", iRoleInfo.Level);
			_roleInfo.put("zoneId", iRoleInfo.ZoneID);
			_roleInfo.put("zoneName", iRoleInfo.ZoneName);
			_roleInfo.put("balance", iRoleInfo.Balance);
			_roleInfo.put("vip", iRoleInfo.Vip);
			_roleInfo.put("partyName", iRoleInfo.PartyName);
			_roleInfo.put("roleCTime", iRoleInfo.CTime);
			_roleInfo.put("roleLevelMTime", iRoleInfo.RoleLevelMTime);

			this.Info ("SetData()::RoleInfo:{0}", _roleInfo.toString ());
			// 设定信息
			setData (_curActivity.GetRawObject(), iKey, _roleInfo.toString ());
		}

		/// <summary>
		/// 创建角色.
		/// </summary>
		/// <param name="iRoleInfo">角色信息.</param>
		public void CreateRole(OneSDKRoleInfo iRoleInfo) {

			this.Info ("CreateRole()::RoleInfo:{0}", iRoleInfo.ToString());
			// 设定数据
			SetData("createrole", iRoleInfo);
		}

		/// <summary>
		/// 更新等级信息（升级时）.
		/// </summary>
		/// <param name="iRoleInfo">角色信息.</param>
		public void UpdateRoleInfoWhenLevelup(OneSDKRoleInfo iRoleInfo) {

			this.Info ("UpdateRoleInfoWhenLevelup()::RoleInfo:{0}", iRoleInfo.ToString ());
			
			// 设定数据
			SetData("levelup", iRoleInfo);
		}

		/// <summary>
		/// 更新角色信息（登录服务器后）.
		/// </summary>
		/// <param name="iRoleInfo">角色信息.</param>
		public void UpdateRoleInfoWhenEnterServer(OneSDKRoleInfo iRoleInfo) {

			this.Info ("UpdateRoleInfoWhenEnterServer()::RoleInfo:{0}", iRoleInfo.ToString ());

			// 设定数据
			SetData("enterServer", iRoleInfo);
		}

		/// <summary>
		/// 退出.
		/// </summary>
		/// <param name="iTarget">游戏对象.</param>
		/// <param name="iOnExited">退出回调函数.</param>
		public void Exit(
			GameObject iTarget = null, 
			Action<string> iOnExited = null) {
			AndroidJavaClass _unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
			if (null == _unityPlayer) {
				this.Error ("SetRoleData():The unity player is invalid!!!");
				return;
			}
			AndroidJavaObject _curActivity = _unityPlayer.GetStatic<AndroidJavaObject> ("currentActivity");
			if (null == _curActivity) {
				this.Error ("SetRoleData():The current activity is invalid!!!");
				return;
			}
			this.Info ("Exit()::Target:{0} Method:{1}",
				(null == iTarget) ? "null" : iTarget.name,
				(null == iOnExited) ? "null" : iOnExited.Method.Name);

			exit (_curActivity.GetRawObject (), 
				(null == iTarget) ? "" : iTarget.name, 
				(null == iOnExited) ? "" : iOnExited.Method.Name);
		}
	}
}

#endif
