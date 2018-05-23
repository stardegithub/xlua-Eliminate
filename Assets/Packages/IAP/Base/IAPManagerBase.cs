using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Common;
using Upload;
using NetWork;
using NetWork.Servers;

namespace IAP {

	/// <summary>
	/// IAP Item接口.
	/// 备注：
	/// 若干属性可能与Apple的iTunes Connect中设定的又相似的地方。
	/// 但是显示以此为准
	/// </summary>
	public interface IIAPItem {

		/// <summary>
		/// 初始化.
		/// </summary>
		void InitIAPItem ();

		#region Store Setting

		/// <summary>
		/// Product ID(必须与Apple的iTunes Connect中设置的ID一致).
		/// </summary>
		string ProductId { get; set; }

		/// <summary>
		/// 价格.
		/// </summary>
		decimal Price { get; set; }

		/// <summary>
		/// 货币符号.
		/// </summary>
		string CurrencySymbol {get; set; }

		/// <summary>
		/// 货币码.
		/// </summary>
		string CurrencyCode {get; set; }

		#endregion

		#region Game Setting

		/// <summary>
		/// IAP Item ID.
		/// 用于服务器标记用户购买记录用（全局唯一）
		/// </summary>
		int IAPItemID { get; set; }

		/// <summary>
		/// 显示No(画面显示顺序用).
		/// </summary>
		int DisplayNo { get; set; }

		/// <summary>
		/// 道具标题.
		/// </summary>
		string Title { get; set; }

		/// <summary>
		/// 道具描述.
		/// </summary>
		string Description { get; set; }

		/// <summary>
		/// 道具数量.
		/// 与服务器实际更新用户数据保持一致
		/// </summary>
		int Quantity { get; set; }

		/// <summary>
		/// 道具可用标志位.
		/// 备注：
		/// 不可用时，在商店不显示
		/// </summary>
		bool Enable { get; set; }

		/// <summary>
		/// 是否已验证合法.
		/// </summary>
		bool IsValidated { get; set; }

		/// <summary>
		/// 是否已经购买标志位.
		/// </summary>
		bool Bought { get; set; }

		/// <summary>
		/// 货币符号 + 显示价格.
		/// </summary>
		string DisplayPrice { get; set; }

		/// <summary>
		/// 推荐标志位.
		/// </summary>
		bool IsRecommend { get; set; }

		#endregion

		#region OnClick

		/// <summary>
		/// 点击函数.
		/// </summary>
		void OnItemClick();

		#endregion

		string ToString ();
	}
		
	/// <summary>
	/// 下订单请求数据基类.
	/// </summary>
	public class RequestOrderDataBase : RequestDataBase {

		/// <summary>
		/// 支付类型.
		/// </summary>
		public IAPPayType PayType = IAPPayType.None;

		/// <summary>
		/// 产品ID.
		/// </summary>
		public string ProductID;

		/// <summary>
		/// 数量.
		/// </summary>
		public int Quantity;

		/// <summary>
		/// 价格.
		/// </summary>
		public float Price;

		/// <summary>
		/// 货币符号.
		/// </summary>
		public string Currency;

		/// <summary>
		/// 初始化.
		/// </summary>
		public override void Init() {
			base.Init ();

			PayType = IAPPayType.None;
			ProductID = null;
			Quantity = 0;
			Price = 0.0f;
			Currency = null;
		}

		/// <summary>
		/// 重置.
		/// </summary>
		public override void Reset() {
			base.Reset ();
		}

		/// <summary>
		/// 清空.
		/// </summary>
		public override void Clear() {
			base.Clear ();

			PayType = IAPPayType.None;
			ProductID = null;
			Quantity = 0;
			Price = 0.0f;
			Currency = null;
		}

		public override string ToString() {
			return string.Format("PayType:{0} ProductID:{1}({2}) Price:{3} Currency:{4}",
				PayType, ProductID, Quantity, Price, Currency);
		}
	}

	/// <summary>
	/// 下订单回复数据基类.
	/// </summary>
	public class ResponseOrderDataBase : ResponseDataBase {

		/// <summary>
		/// 订单ID.
		/// </summary>
		public string OrderID;

		/// <summary>
		/// 产品ID.
		/// </summary>
		public string ProductID;

		/// <summary>
		/// 数量.
		/// </summary>
		public int Quantity;

		/// <summary>
		/// 构造函数.
		/// </summary>
		public ResponseOrderDataBase () {}

		/// <summary>
		/// 初始化.
		/// </summary>
		public override void Init() {
			base.Init ();

			OrderID = null;
			ProductID = null;
			Quantity = 0;
		}

		/// <summary>
		/// 重置.
		/// </summary>
		public override void Reset() {
			base.Reset ();
		}

		/// <summary>
		/// 清空.
		/// </summary>
		public override void Clear() {
			base.Clear ();

			OrderID = null;
			ProductID = null;
			Quantity = 0;
		}

		public override string ToString() {
			return string.Format("{0} OrderID::{1} ProductID::{2}({3})",
				base.ToString(), OrderID, 
				(true == string.IsNullOrEmpty(ProductID)) ? "null" : ProductID, 
				Quantity);
		}

	}

	/// <summary>
	/// 收据验证请求数据基类.
	/// </summary>
	public class RequestReceiptVerifyDataBase : RequestDataBase {

		/// <summary>
		/// 交易ID.
		/// </summary>
		public string TransactionID;

		/// <summary>
		/// 收据.
		/// </summary>
		public string Receipt;

		/// <summary>
		/// 初始化.
		/// </summary>
		public override void Init() {
			base.Init ();

			TransactionID = null;
			Receipt = null;
		}

		/// <summary>
		/// 重置.
		/// </summary>
		public override void Reset() {
			base.Reset ();
		}

		/// <summary>
		/// 清空.
		/// </summary>
		public override void Clear() {
			base.Clear ();

			TransactionID = null;
			Receipt = null;
		}

		public override string ToString() {
			return string.Format("TransactionID::{0} Receipt::{1}",
				(true == string.IsNullOrEmpty(TransactionID)) ? "-" : TransactionID, 
				(true == string.IsNullOrEmpty(Receipt)) ? "-" : Receipt);
		}
	}

	/// <summary>
	/// 收据验证回复数据基类.
	/// </summary>
	public class ResponseReceiptVerifyDataBase : ResponseDataBase {

		/// <summary>
		/// 交易ID.
		/// </summary>
		public string TransactionID;

		/// <summary>
		/// 初始化.
		/// </summary>
		public override void Init() {
			base.Init ();

			TransactionID = null;
		}

		/// <summary>
		/// 重置.
		/// </summary>
		public override void Reset() {
			base.Reset ();
		}

		/// <summary>
		/// 清空.
		/// </summary>
		public override void Clear() {
			base.Clear ();

			TransactionID = null;
		}

		public override string ToString() {
			return string.Format("{0} TransactionID::{1}",
				base.ToString(), TransactionID);
		}
	}

	/// <summary>
	/// IAP 管理器脚本基类.
	/// </summary>
	public abstract class IAPManagerBase<T1, T2, T3, T4, T5> : MonoBehaviourExtension 
		where T1 : IIAPItem, new()
		where T2 : RequestOrderDataBase, new()
		where T3 : ResponseOrderDataBase, new()
		where T4 : RequestReceiptVerifyDataBase, new()
		where T5 : ResponseReceiptVerifyDataBase, new() {

		/// <summary>
		/// 超时最大时间（单位：秒）.
		/// </summary>
		private float _timeoutMaxValue = 60.0f;

		/// <summary>
		/// 加载的Mask是否正在显示中.
		/// </summary>
		protected bool isLoadingMaskShowing = false;
			
		/// <summary>
		/// IAP状态.
		/// </summary>
		protected IAPState State { 
			get; 
			set; 
		}		

		/// <summary>
		/// IAP上一状态.
		/// </summary>
		protected IAPState LastState { 
			get; 
			set; 
		}

		/// <summary>
		/// 当前Action Step.
		/// </summary>
		protected IAPActionStep ActiveStep { 
			get; 
			set; 
		}

		/// <summary>
		/// 当前IAP item.
		/// </summary>
		protected T1 ActiveIAPItem { 
			get; 
			set; 
		}

		/// <summary>
		/// 当前订单ID.
		/// </summary>
		protected string ActiveOrderID { 
			get; 
			set; 
		}

		/// <summary>
		/// 当前产品ID.
		/// </summary>
		protected string ActiveProductID { 
			get; 
			set; 
		}

		/// <summary>
		/// 当前购买数量.
		/// </summary>
		protected int ActiveQuantity { 
			get; 
			set; 
		}

		/// <summary>
		/// 当前交易ID.
		/// </summary>
		protected string ActiveTransactionID { 
			get; 
			set; 
		}

		/// <summary>
		/// 当前收据.
		/// </summary>
		protected string ActiveReceipt { 
			get; 
			set; 
		}

		/// <summary>
		/// 错误Code.
		/// </summary>
		protected IAPErrorCode ErrorCode { 
			get; 
			set; 
		}

		/// <summary>
		/// 详细错误Code.
		/// </summary>
		protected int ErrorDetailCode { 
			get; 
			set; 
		}

		/// <summary>
		/// 错误详细.
		/// </summary>
		protected string ErrorDetail { 
			get; 
			set; 
		}

		/// <summary>
		/// 时间计数器.
		/// </summary>
		protected TimeCounter _timeCounter = null;

		/// <summary>
		/// IAP items列表.
		/// </summary>
		public Dictionary<string, T1> _IAPItems = new Dictionary<string, T1> ();

		/// <summary>
		/// IAP实例对象（iOS/Android）.
		/// </summary>
		protected IAPBase _IAPInstance = null;

#region MonoBehaviour Method

		protected virtual void Awake () {

			this.Info ("Awake()");

			// 初始化
			if(false == this.InitInfo()) {
				return;
			}

			// 产品列表
			if(false == this.initProducts ()) {
				return;
			}

			// IAP实例
			if(false == this.initIAPInstance()) {
				return;
			}
			this._IAPInstance.StartProcessing ();

			// 时间计数器初始化
			this._timeCounter = TimeCounter.Create(
				this._timeoutMaxValue, OnTimeCounterCountOver);
		}
		protected virtual void OnEnable() {
			this.Info ("OnEnable()");

			// 开始验证产品列表
			this.UToIAPProductsVerify ();
		}
			
		protected virtual void OnDisable() {

			// 停止
			if(this._IAPInstance != null) {
				this._IAPInstance.StopProcessing ();
			}
			// 清空动作列表
			this.InitInfo();
			this.Info ("OnDisable()");
		}

		protected virtual void Start() {

			this.Info ("Start()");

		}

#endregion

		/// <summary>
		/// 取得验证产品列表.
		/// </summary>
		/// <returns>验证产品列表.</returns>
		protected string[] GetAllProductsForValidation() {
		
			// 重新排序
			KeyValuePair<string, T1>[] _items =
				this._IAPItems
					.OrderBy(o => o.Value.DisplayNo)
					.ToArray ();

			List<string> productIds = new List<string> ();
			foreach (KeyValuePair<string, T1> it in _items) {
				productIds.Add (it.Value.ProductId);
			}
			if ((null == productIds) || (0 >= productIds.Count)) {
				return null;
			}
			return productIds.ToArray();
		}

		protected virtual void Update() {

			// 非执行状态时，停止超时计时
			if (IAPState.Runing == this.State) {
				if ((null != this._timeCounter) && 
					(true == this._timeCounter.UpdateCounter (Time.deltaTime))) {
					return;
				}
			} else {
				// 停止超时计数
				if (null != this._timeCounter) {
					this._timeCounter.EndCounter ();
				}
			}

			// 状态无变化时
			if(this.LastState == this.State) {
				return;
			}
			this.LastState = this.State;

			// 刷新UI界面
			this.RefreshUIByState (this.State);

			// 执行动作
			if (IAPState.Runing == this.State) {
				// 开始计时
				if (null != this._timeCounter) {
					this._timeCounter.RestartCounter ();
				}
			}

		}

#region Response

		/// <summary>
		/// 订单回复回调事件委托.
		/// </summary>
		/// <param name="iResponseData">回复数据.</param>
		protected void OnResponsedOrdered(
			ResponseDataBase iResponseData) { 
			if (iResponseData == null) {
				this.Error ("OnResponsedOrdered():The data of response is null!!");
				return;
			}
			this.Info ("OnResponsedOrdered():{0}", iResponseData.ToString());

			if (true == iResponseData.isOK ) {

				T3 _response = iResponseData as T3;
				if (null == _response) {
					this.Error ("OnResponsedOrdered():The data of response is invalid!!!");
					return;
				}
				this.ActiveOrderID = _response.OrderID;
				this.Info ("OnResponsedOrdered():ActiveOrderID::{0}", this.ActiveOrderID);
				this.SwitchState (IAPState.Runing);

				if (null != this._IAPInstance) {
					// 开始计时
					if (null != this._timeCounter) {
						this._timeCounter.RestartCounter ();
					}
					this._IAPInstance.StartTransaction (
						_response.ProductID, _response.Quantity);
				}

			} else {
				// 设置错误消息
				this.SetErrorInfoByStatus (iResponseData);
			}
		}

		/// <summary>
		/// 收据验证事件委托.
		/// </summary>
		/// <param name="iSelf">自身Action.</param>
		/// <param name="iTransactionID">交易ID.</param>
		/// <param name="iOK">OK标志位.</param>
		public void OnResponsedReceiptVerify(
			ResponseDataBase iResponseData) {
			if (null == iResponseData) {
				return;
			}
			this.Info ("OnResponsedReceiptVerify():{0}", iResponseData.ToString());
			T5 _response = iResponseData as T5;
			if (null == _response) {
				this.Error ("OnResponsedReceiptVerify():The data of response is invalid!!!");
				return;
			}

			if (true == _response.isOK ) {

				this.Info ("OnResponsedReceiptVerify():TransactionID::{0}", _response.TransactionID);

				// 交易完成设置成闲置状态
				this.SwitchState (IAPState.Idled);

			} else {
				
				// 设置错误消息
				this.SetErrorInfoByStatus (_response);
			}

			// 结束交易
			this.UToIAPFinalizeTransaction (_response);
		}

#endregion

#region TimeOut

		/// <summary>
		/// 计时超过最大计数时回调函数.
		/// </summary>
		protected virtual void OnTimeCounterCountOver() {
			this.Warning ("OnTimeCounterCountOver()");
			// 切换状态:超时
			this.SwitchState (IAPState.TimeOuted);
		}
		/// <summary>
		/// 超时确认回调函数.
		/// </summary>
		protected virtual void OnTimeoutRetry() {
			this.Info ("OnTimeoutRetry():ActiveStep::{0}", this.ActiveStep);

			// 切换状态 -> 继续执行
			this.SwitchState (IAPState.Runing);

			switch (this.ActiveStep) {
			case IAPActionStep.ProductsVerifying:
				{
					// 开始验证产品列表
					this.UToIAPProductsVerify ();
				}
				break;
			case IAPActionStep.Ordering:
				{
					// 追加订单 Action
					IAPRequestBase<T1, T2, T3> request = 
						CreateOrderRequest(this.ActiveIAPItem, this.OnResponsedOrdered);
					if (null != request) {
						request.Send ();
					}
				}
				break;
			case IAPActionStep.Purchasing:
				{
					if (null != this._IAPInstance) {
						this._IAPInstance.StartTransaction (
							this.ActiveProductID, this.ActiveQuantity);
					}
				}
				break;
			case IAPActionStep.ReceiptVerifying:
				{
					IAPRequestBase<T4, T5> request = this.CreateReceiptVerifyRequest (
						this.ActiveOrderID, this.ActiveTransactionID, this.ActiveReceipt, 
						this.OnResponsedReceiptVerify);
					if (null != request) {
						request.Send ();
					}
				}
				break;
			default :
				{}
				break;
			}
		}
		/// <summary>
		/// 超时确认回调函数.
		/// </summary>
		protected virtual void OnTimeoutConfirm() {
			this.Warning ("OnTimeoutConfirm()");
			// 结束交易
//			this.FinalizeTransaction();

			this.SwitchState (IAPState.Idled);
		}

#endregion

#region Unity To IAP

		/// <summary>
		/// 开始验证产品列表.
		/// </summary>
		protected void UToIAPProductsVerify() {

			// 执行动作
			this.SwitchState (IAPState.Runing);
			// 验证产品列表中
			this.ActiveStep = IAPActionStep.ProductsVerifying;

			if (null != this._IAPInstance) {
			
				string[] productIDs = this.GetAllProductsForValidation ();
				if ((null == productIDs) || (0 >= productIDs.Length)) {
					this.Error ("UToIAPProductsVerify():There is no product to verify!!!");
					return;
				}
				this._IAPInstance.ValidateProducts (productIDs);
			} else {
				this.SwitchState (IAPState.Idled);
			}

		}

		/// <summary>
		/// 购买IAP Item.
		/// </summary>
		/// <param name="iIAPItemId">IAP Item ID.</param>
		/// <param name="iProductID">产品ID.</param>
		/// <param name="iQuantity">数量.</param>
		protected virtual void UToIAPBuyItem(T1 iIAPItem) {

			// 执行动作
			this.SwitchState (IAPState.Runing);
			// 下订单中
			this.ActiveStep = IAPActionStep.Ordering;

			this.ActiveProductID = iIAPItem.ProductId;
			this.ActiveQuantity = iIAPItem.Quantity;

			this.Info ("UToIAPBuyItem():{0}", iIAPItem.ToString());

			// 追加订单 Action
			IAPRequestBase<T1, T2, T3> request = 
				CreateOrderRequest(iIAPItem, this.OnResponsedOrdered);
			if (null != request) {
				request.Send ();
			}
		}
			
		/// <summary>
		/// 结束交易.
		/// </summary>
		public virtual void UToIAPFinalizeTransaction(T5 iReceiptVerifiedData) {
			this.Info ("UToIAPFinalizeTransaction():{0}", iReceiptVerifiedData.ToString());
			if ((this._IAPInstance == null) || (default(T5) == iReceiptVerifiedData)) {
				this.Error ("UToIAPFinalizeTransaction():The instance of iap or response data is null!!!");
				return;
			}
			if (string.IsNullOrEmpty (iReceiptVerifiedData.TransactionID) == true) {
				this.Error ("UToIAPFinalizeTransaction():The active transactionID is empty!!!");
				return;
			}
			this._IAPInstance.FinalizeTransaction (iReceiptVerifiedData.TransactionID);
		}

#endregion

#region IAP To Unity(Callback)

		/// <summary>
		/// 初始化IAP 实例.
		/// </summary>
		/// <returns><c>true</c>, OK, <c>false</c> NG.</returns>
		private bool initIAPInstance() {

			// 创建IAP实例
			this._IAPInstance = this.CreateInstance ();
			if (null == this._IAPInstance) {
				return false;
			}

			// 绑定委托事件
			// 产品验证
			this._IAPInstance.OnProductsValidationStarted = null;
			this._IAPInstance.OnProductsValidationCompleted = this.OnIAPProductsValidationCompleted;
			this._IAPInstance.OnProductsValidationFailed = this.OnIAPProductsValidationFailed;

			// 购买
			this._IAPInstance.OnTransactionPurchasing = this.OnTransactionPurchasing;
			this._IAPInstance.OnTransactionCanceled = this.OnIAPTransactionCanceled;
			this._IAPInstance.OnTransactionDeferred = null;

			// 购买或者补单
			this._IAPInstance.OnTransactionPurchased = this.OnIAPTransactionPurchased;

			// 购买恢复
			this._IAPInstance.OnRestoreCompleted = null;
			this._IAPInstance.OnTransactionRestored = this.OnIAPTransactionRestored;
			this._IAPInstance.OnRestoreFinished = null;

			// 错误
			this._IAPInstance.OnFailed = this.OnIAPFailed;

			// 切换当前IAP实例
			IAPMsgSwitch.SetIAPInstance(this._IAPInstance);

			return true;
		}

		/// <summary>
		/// 更新IAP Item信息.
		/// </summary>
		/// <param name="iUpdated">是否有更新.</param>
		/// <param name="iProducts">Products列表.</param>
		private void OnIAPProductsValidationCompleted(bool iUpdated, Dictionary<string,IIAPProduct> iProducts) {

			this.Info ("OnIAPProductsValidationCompleted():Updated:{0}(Count:{1})", iUpdated, (null == iProducts) ? "-" : iProducts.Count.ToString());

			if ((iProducts != null) && (iProducts.Count > 0)) {
				foreach (KeyValuePair<string,IIAPProduct> it in iProducts) {

					if (this._IAPItems.ContainsKey (it.Value.ProductID) == false) {
						this.Warning ("OnActionUpdated():This product is not exist!!!(ProductID:{0})",
							it.Value.ProductID);
						continue;
					}

					T1 product = this._IAPItems [it.Value.ProductID];
					product.Price = it.Value.Price;
					product.CurrencySymbol = it.Value.CurrencySymbol;
					product.CurrencyCode = it.Value.CurrencyCode;
					product.DisplayPrice = string.Format ("{0} {1}", it.Value.CurrencySymbol, (int)it.Value.Price);
					product.IsValidated = true;

					this.Info ("OnIAPProductsValidationCompleted():{0}", product.ToString());
				}
			}

			// 产品列表验证完成，回复闲置状态
			this.SwitchState (IAPState.Idled);

		}

		/// <summary>
		/// 验证失败.
		/// </summary>
		/// <param name="iProductId">ID.</param>
		/// <param name="iDescription">描述.</param>
		/// <param name="iPrice">价格.</param>
		private void OnIAPProductsValidationFailed(string iDescription, Int32 iPrice) {
			this.Info ("OnIAPProductsValidationFailed():Description:{0} Price:{1}", iDescription, iPrice);

			this.ErrorCode = IAPErrorCode.ProductsVerifyFailed;
			this.ErrorDetailCode = -1;
			this.ErrorDetail = iDescription;

			this.SwitchState (IAPState.Failed);
		}

		/// <summary>
		/// 交易中
		/// </summary>
		/// <param name="iProductID">产品ID.</param>
		/// <param name="iQuantity">数量.</param>
		private void OnTransactionPurchasing(string iProductID, Int32 iQuantity) {
			this.Info ("OnTransactionPurchasing():ProductID::{0}({1})", iProductID, iQuantity);

			this.SwitchState (IAPState.Runing);

			this.ActiveProductID = iProductID;
			this.ActiveQuantity = iQuantity;

			this.Info ("OnTransactionPurchasing():ActiveProductID:{0}", this.ActiveProductID);
			this.Info ("OnTransactionPurchasing():ActiveQuantity:{0}", this.ActiveQuantity);
		}

		/// <summary>
		/// 交易取消.
		/// </summary>
		/// <param name="iProductID">产品ID.</param>
		/// <param name="iQuantity">购买数量.</param>
		private void OnIAPTransactionCanceled(string iProductID, int iQuantity) {

			this.Warning ("OnIAPTransactionCanceled():ProductID:{0} Quantity:{1}",
				iProductID, iQuantity);

			// 重新初始化信息
			this.InitInfo ();

			this.SwitchState (IAPState.Canceled);
		}

		/// <summary>
		/// 交易购买完成.
		/// </summary>
		/// <param name="iProductID">产品ID.</param>
		/// <param name="iQuantity">数量.</param>
		/// <param name="iTransactionID">交易ID.</param>
		/// <param name="iReceipt">收据.</param>
		private void OnIAPTransactionPurchased(string iProductID, Int32 iQuantity, string iTransactionID, string iReceipt) {

			this.Info ("OnIAPTransactionPurchased():ProductID:{0}({1}) TransactionID::{2} Receipt::{3}", 
				iProductID, iQuantity, iTransactionID, iReceipt);

			// 切换状态 -> 完成
//			this.SwitchState (IAPState.Runing);
			this.ActiveStep = IAPActionStep.ReceiptVerifying;

			this.ActiveTransactionID = iTransactionID;
			this.ActiveReceipt = iReceipt;
		
			this.Info ("OnIAPTransactionPurchased():ActiveTransactionID:{0}", this.ActiveTransactionID);
			this.Info ("OnIAPTransactionPurchased():ActiveReceipt:{0}", this.ActiveReceipt);

			// 开始计时
			if (null != this._timeCounter) {
				this._timeCounter.RestartCounter ();
			}

			IAPRequestBase<T4, T5> request = this.CreateReceiptVerifyRequest (
				this.ActiveOrderID, iTransactionID, iReceipt, 
				this.OnResponsedReceiptVerify);
			if (null != request) {
				request.Send ();
			}
		}

		/// <summary>
		/// 交易恢复完成.
		/// </summary>
		/// <param name="iProductID">产品ID.</param>
		/// <param name="iQuantity">数量.</param>
		/// <param name="iTransactionID">交易ID.</param>
		/// <param name="iReceipt">收据.</param>
		private void OnIAPTransactionRestored (string iProductID, Int32 iQuantity, string iTransactionID, string iReceipt) {
			this.Info ("OnTransactionRestored():ProductID:{0}({1}) TransactionID::{2} Receipt::{3}", 
				iProductID, iQuantity, iTransactionID, iReceipt);
		}

		/// <summary>
		/// 交易失败.
		/// </summary>
		/// <param name="iErrorCode">Error Code.</param>
		/// <param name="iErrorDetailCode">错误详细Code.</param>
		/// <param name="iErrorDetailInfo">错误详细.</param>
		private void OnIAPFailed(IAPErrorCode iErrorCode, int iErrorDetailCode, string iErrorDetailInfo) {

			this.Error ("OnIAPFailed():ErrorCode:{0} ErrorDetailCode:{1} ErrorDetail:{2}",
				iErrorCode, iErrorDetailCode, iErrorDetailInfo);

			this.ErrorCode = iErrorCode;
			this.ErrorDetailCode = iErrorDetailCode;
			this.ErrorDetail = iErrorDetailInfo;

			this.SwitchState (IAPState.Failed);
		}

#endregion

		/// <summary>
		/// 切换状态（切换状态后，在Update中将被执行一次）.
		/// </summary>
		/// <param name="iState">状态.</param>
		/// <param name="iErrorMessageID">错误消息ID.</param>
		protected void SwitchState(IAPState iState) {

			this.Info ("SwitchState(): -> ::{0}", iState);

			if (IAPState.Failed == iState) {
			}
			this.State = iState;
		}

		/// <summary>
		/// 初始化信息.
		/// </summary>
		/// <returns><c>true</c>, OK, <c>false</c> NG.</returns>
		private bool InitInfo() {
		
			// 超时状态
			this._timeoutMaxValue = ServersConf.GetInstance().NetTimeOut;

			// 状态
			this.LastState = IAPState.None;
			this.State = IAPState.None;

			this.ActiveStep = IAPActionStep.None;
			this.ActiveIAPItem = default(T1);
			this.ActiveProductID = null;
			this.ActiveQuantity = 0;
			this.ActiveOrderID = null;
			this.ActiveTransactionID = null;
			this.ActiveReceipt = null;

			// 错误信息
			this.ErrorCode = IAPErrorCode.None;
			this.ErrorDetailCode = -1;
			this.ErrorDetail = null;

			return true;
		}

#region Manager Abstract

		/// <summary>
		/// 创建IAP实例.
		/// </summary>
		/// <returns>IAP实例.</returns>
		protected abstract IAPBase CreateInstance();

		/// <summary>
		/// 初始化Products列表.
		/// </summary>
		/// <returns><c>true</c>, 初始化成功, <c>false</c> 初始化失败.</returns>
		protected abstract bool initProducts ();

		/// <summary>
		/// 创建下订单请求.
		/// </summary>
		/// <returns>下订单请求.</returns>
		/// <param name="iIAPItem">IAP Item.</param>
		/// <param name="iResponseCallback">回复事件委托.</param>
		protected abstract IAPRequestBase<T1, T2, T3> CreateOrderRequest (
			T1 iIAPItem,  OnResponsedDelegate iResponseCallback);

		/// <summary>
		/// 创建收据验证请求.
		/// </summary>
		/// <returns>交易Action.</returns>
		/// <param name="iOrderID">订单ID（为空时：补单）.</param>
		/// <param name="iTransactionID">交易ID.</param>
		/// <param name="iReceipt">收据.</param>
		/// <param name="iResponseCallback">回复事件委托.</param>
		protected abstract IAPRequestBase<T4, T5> CreateReceiptVerifyRequest (
			string iOrderID,  string iTransactionID,  string iReceipt,  
			OnResponsedDelegate iResponseCallback);

		/// <summary>
		/// 根据状态设置错误信息.
		/// </summary>
		/// <param name="iResponseData">回复报文数据.</param>
		protected abstract void SetErrorInfoByStatus (ResponseDataBase iResponseData);

		/// <summary>
		/// 根据ErrorCode取得MessageID.
		/// </summary>
		/// <returns>MessageID.</returns>
		/// <param name="iErrorCode">ErrorCode.</param>
		protected abstract string GetMessageIDByErrCode(IAPErrorCode iErrorCode);

		/// <summary>
		/// 根据状态刷新商店UI界面(遮罩除外).
		/// </summary>
		/// <param name="iState">状态.</param>
		/// <param name="iParams">备用参数.</param>
		protected abstract void  RefreshUIByState(IAPState iState, params object[] iParams);

		/// <summary>
		/// 显示进行中遮罩.
		/// </summary>
		/// <param name="iVisible">是否显示标志位.</param>
		protected abstract void ShowLoadingMask (bool iVisible);

		/// <summary>
		/// 点击IAP Item函数.
		/// </summary>
		/// <param name="iIAPItem">IAP Item.</param>
		protected abstract void OnIAPItemClick (T1 iIAPItem);

#endregion

	}
}