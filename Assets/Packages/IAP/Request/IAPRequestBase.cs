using UnityEngine;
using System.Collections;
using Common;

namespace IAP {

	/// <summary>
	/// IAP 请求基类.
	/// T1 : 请求数据模板
	/// T2 : 回复数据模板
	/// </summary>
	public abstract class IAPRequestBase<T1, T2> : RequestBase<T1, T2>
		where T1 : RequestDataBase, new()
		where T2 : ResponseDataBase, new() {

		/// <summary>
		/// 发送请求.
		/// </summary>
		/// <param name="iData">请求数据.</param>
		/// <param name="iOnResponsed">回复回调函数.</param>
		protected override void SendRequest(T1 iData, 
			OnResponsedDelegate iOnResponsed) {

			if (null == iOnResponsed) {
				this.Error ("SendRequest():The callback of response is null!!!");
				return;
			}

			// 发送请求
			this.SendIAPRequest (iData, iOnResponsed);
		}

		/// <summary>
		/// 初始化.
		/// </summary>
		/// <param name="iRequestData">请求数据.</param>
		/// <param name="iOnResponsed">回复回调函数.</param>
		protected virtual bool init(T1 iRequestData, OnResponsedDelegate iOnResponsed) {
			if (null == iRequestData) {
				return false;
			}
			if (null == iOnResponsed) {
				return false;
			}
			this.RequestData = iRequestData;
			this.OnResponsedCallback = iOnResponsed;
			return true;
		}

		/// <summary>
		/// 回复.
		/// </summary>
		/// <param name="iData">数据.</param>
		public override void OnResponsed(ResponseDataBase iData) {
			base.OnResponsed (iData);
		}

		/// <summary>
		/// 发送IAP请求.
		/// </summary>
		/// <param name="iData">发送用的数据.</param>
		/// <param name="iOnResponsed">回复回调函数.</param>
		protected abstract void SendIAPRequest(T1 iData, 
			OnResponsedDelegate iOnResponsed);

	}

	/// <summary>
	/// IAP 请求基类.
	/// T1 : 生成请求数据的配置数据
	/// T2 : 请求数据模板
	/// T3 : 回复数据模板
	/// </summary>
	public abstract class IAPRequestBase<T1, T2, T3> : RequestBase<T2, T3>
		where T1 : IIAPItem, new()
		where T2 : RequestDataBase, new()
		where T3 : ResponseDataBase, new() {

		/// <summary>
		/// 发送请求.
		/// </summary>
		/// <param name="iData">请求数据.</param>
		/// <param name="iOnResponsed">回复回调函数.</param>
		protected override void SendRequest(T2 iData, 
			OnResponsedDelegate iOnResponsed) {

			if (null == iOnResponsed) {
				this.Error ("SendRequest():The callback of response is null!!!");
				return;
			}

			// 发送请求
			this.SendIAPRequest (iData, iOnResponsed);
		}

		/// <summary>
		/// 初始化.
		/// </summary>
		/// <param name="iIAPItem">IAP Item.</param>
		/// <param name="iOnResponsed">回复回调函数.</param>
		protected virtual bool init(T1 iIAPItem, OnResponsedDelegate iOnResponsed) {
			if (null == iIAPItem) {
				return false;
			}
			if (null == iOnResponsed) {
				return false;
			}
			return true;
		}

		/// <summary>
		/// 回复.
		/// </summary>
		/// <param name="iData">数据.</param>
		public override void OnResponsed(ResponseDataBase iData) {
			base.OnResponsed (iData);
		}

		/// <summary>
		/// 创建请求数据.
		/// </summary>
		/// <returns>请求数据.</returns>
		/// <param name="iConfInfo">配置数据.</param>
		protected abstract void CreateRquestData (T1 iConfInfo);

		/// <summary>
		/// 发送IAP请求.
		/// </summary>
		/// <param name="iData">发送用的数据.</param>
		/// <param name="iOnResponsed">回复回调函数.</param>
		protected abstract void SendIAPRequest(T2 iData, 
			OnResponsedDelegate iOnResponsed);

	}
}
