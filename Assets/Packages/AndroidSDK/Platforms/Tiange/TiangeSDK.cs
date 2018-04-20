#if UNITY_ANDROID
using UnityEngine;
using System.Collections;
using Common;
using AndroidSDK.Common;

namespace AndroidSDK.Platforms.Tiange {

	/// <summary>
	/// 天鸽SDK设定.
	/// </summary>
	public sealed class TiangeSDK : SingletonBase<TiangeSDK>, IAndroidSDK {

		/// <summary>
		/// 登录.
		/// </summary>
		/// <param name="iTarget">登陆启动的目标对象.</param>
		/// <param name="iLoginCallback">登录/登出回调函数.</param>
		public void Login (GameObject iTarget, System.Action<string> iLoginCallback) {
			
		}

		/// <summary>
		/// 登出.
		/// </summary>
		public void Logout() {
			
		}
	}
}

#endif

