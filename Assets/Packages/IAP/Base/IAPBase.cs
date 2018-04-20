using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Common;
using AOT;

namespace IAP
{
	/// <summary>
	/// 交易状态.
	/// </summary>
	public enum TransactionState {
		/// <summary>
		/// 购买中.
		/// </summary>
		Purchasing = 0,
		/// <summary>
		/// 购买完成.
		/// </summary>
		Purchased,
		/// <summary>
		/// 取消.
		/// </summary>
		Canceled,
		/// <summary>
		/// 购买恢复.
		/// </summary>
		Restored,
		/// <summary>
		/// 购买延迟.
		/// </summary>
		Deferred,
		/// <summary>
		/// 未知.
		/// </summary>
		Unknown
	}

	/// <summary>
	/// IAP消息交换机.
	/// </summary>
	public class IAPMsgSwitch {

#region native -> managed calls

		/// <summary>
		/// 创建Product回调函数.
		/// </summary>
		protected delegate void IAPCreateProductCallback (
			string iTitle, string iDescription, string iIdentifier,
			float iPrice,string iPriceString, string iCurrency,
			string iCode,string iLocale, string iCountry);

		/// <summary>
		/// 交易更新.
		/// </summary>
		protected delegate void IAPTransactionUpdatedCallback(
			string productID, TransactionState state, string transactionID, 
			string receipt, Int32 quantity);

		/// <summary>
		/// 恢复完成.
		/// </summary>
		protected delegate void IAPRestoreFinishedCallback ();

		/// <summary>
		/// IAP失败回调函数.
		/// </summary>
		protected delegate void IAPFailedCallback (
			IAPErrorCode iIAPStageType, Int32 iErrorDetailCode, 
			string iErrorDetailInfo);

#endregion

		/// <summary>
		/// IAP 实例.
		/// </summary>
		public static IAPBase IAPInstance = null;

		/// <summary>
		/// 构造函数初始化.
		/// </summary>
		static IAPMsgSwitch () {
			if(Application.isMobilePlatform == true) {
				IAPSetCallbacks (CreateProduct, TransactionUpdated, RestoreFinished, IAPFailed);
			}
		}

		/// <summary>
		/// 设定IAP实例.
		/// </summary>
		/// <param name="iIAPInstance">IAP实例.</param>
		public static void SetIAPInstance(IAPBase iIAPInstance) {
			IAPInstance = iIAPInstance;
		}

#region MonoPInvokeCallback

		[MonoPInvokeCallback (typeof (IAPCreateProductCallback))]
		static void CreateProduct(string title, string description, string identifier, float price, string priceString, 
			string currency, string code, string locale, string iCountryCode)
		{
			UtilsLog.Info ("IAPiOS", "[CreateProduct] Title:{0} Description:{1} Identifier:{2} Price:{3} PriceString:{4} Currency:{5} Code:{6} Locale:{7} CountryCode:{8}",
				title, description, identifier, price.ToString(), priceString, currency, code, locale, iCountryCode);
			if (string.IsNullOrEmpty(identifier) == false) 
			{	
				// this is an actual product info
				Product received = new Product { ProductID = identifier, Title = title, Description = description, 
					Price = (decimal)price, PriceAsString = priceString, CurrencySymbol = currency, CurrencyCode = code,
					LocaleIdentifier = locale, CountryCode = iCountryCode};
				if (null != IAPInstance) {
					IAPInstance.AddProduct (received);
				}
			}
			else 
			{	
				// special case to mark the end of the product list or a validation error
				if (string.IsNullOrEmpty(description) == true) 
				{
					if (null != IAPInstance) {
						IAPInstance.OnProductsValidationFinished ();
					}
				}
				// description contains error message, and price contains error code
				else 
				{
					if ((null != IAPInstance) && (null != IAPInstance.OnProductsValidationFailed)) {
						IAPInstance.OnProductsValidationFailed (description, (Int32)price);
					}
				}
			}
		}

		[MonoPInvokeCallback (typeof (IAPTransactionUpdatedCallback))]
		static void TransactionUpdated(string productID, TransactionState state, string transactionID, string receipt, Int32 quantity)
		{
			UtilsLog.Info ("IAPiOS", "[TransactionUpdated] ProductID:{0}({1}) State:{2} TransactionID:{3} Receipt:{4}",
				((true == string.IsNullOrEmpty(productID)) ? "null" : productID), 
				quantity, state.ToString(), 
				((true == string.IsNullOrEmpty(transactionID)) ? "null" : transactionID), 
				((true == string.IsNullOrEmpty(receipt)) ? "null" : receipt));
			switch(state)
			{
			case TransactionState.Purchasing:
				if ((null != IAPInstance) && (null != IAPInstance.OnTransactionPurchasing)) {
					IAPInstance.OnTransactionPurchasing (productID, quantity);
				}
				break;
			case TransactionState.Purchased:
				{
					if ((null != IAPInstance) && (null != IAPInstance.OnTransactionPurchased)) {
						IAPInstance.OnTransactionPurchased (productID, quantity, transactionID, receipt);
					}
				}
				break;
			case TransactionState.Canceled:
				{
					if ((null != IAPInstance) && (null != IAPInstance.OnTransactionCanceled)) {
						IAPInstance.OnTransactionCanceled (productID, quantity);
					}
				}
				break;
			case TransactionState.Restored:
				if (IAPVerifyTransaction (transactionID)) 
				{
					if ((null != IAPInstance) && (null != IAPInstance.OnTransactionRestored)) {
						IAPInstance.OnTransactionRestored (productID, quantity, transactionID, receipt);
					}
				}
				break;
			case TransactionState.Deferred:
				{
					if ((null != IAPInstance) && (null != IAPInstance.OnTransactionDeferred)) {
						IAPInstance.OnTransactionDeferred (productID);
					}
				}
				break;
			}
		}

		[MonoPInvokeCallback (typeof (IAPRestoreFinishedCallback))]
		static void RestoreFinished()
		{
			UtilsLog.Info ("IAPiOS", "[RestoreFinished]");
			if ((null != IAPInstance) && (null != IAPInstance.OnRestoreFinished)) {
				IAPInstance.OnRestoreFinished ();
			}
		}

		[MonoPInvokeCallback (typeof (IAPFailedCallback))]
		static void IAPFailed(IAPErrorCode iErrorCode, Int32 iErrorDetailCode, string iErrorDetailInfo) {
			UtilsLog.Info ("IAPiOS", "[IAPFailed] ErrorCode::{0} ErrorDetailCode::{1} ErrorDetail::{2}",
				iErrorCode, iErrorDetailCode, iErrorDetailInfo);
			if ((null != IAPInstance) && (null != IAPInstance.OnFailed)) {
				IAPInstance.OnFailed (iErrorCode, iErrorDetailCode, iErrorDetailInfo);
			}
		}

#endregion

#region DllImport

		[DllImport ("__Internal")]
		static extern void IAPSetCallbacks(
			IAPCreateProductCallback callback, IAPTransactionUpdatedCallback callback2, 
			IAPRestoreFinishedCallback callback3, IAPFailedCallback callback4);

		[DllImport ("__Internal")]
		public static extern void IAPValidateProducts(string[] productIDs, Int16 count);

		[DllImport ("__Internal")]
		public static extern void IAPBuyWithProductID(string product, Int32 quantity);

		[DllImport ("__Internal")]
		public static extern void IAPRestoreTransactions();

		[DllImport ("__Internal")]
		public static extern void IAPFinalizeTransaction(string transactionID);

		[DllImport ("__Internal")]
		public static extern bool IAPVerifyTransaction(string transactionID);

		[DllImport ("__Internal")]
		public static extern void IAPStartProcessing();

		[DllImport ("__Internal")]
		public static extern void IAPStopProcessing();

		[DllImport ("__Internal")]
		public static extern bool IAPIsEnabled();

#endregion
	}

	public abstract class IAPBase : ClassExtension
	{
		/// <summary>
		/// 产品列表验证.
		/// </summary>
		public OnIAPProductsValidationStartedDelegate OnProductsValidationStarted = null;

		/// <summary>
		/// 产品列表验证完毕.
		/// </summary>
		public OnIAPProductsValidationCompletedDelegate OnProductsValidationCompleted = null;

		/// <summary>
		/// 验证失败.
		/// </summary>
		public OnIAPProductsValidationFailedDelegate OnProductsValidationFailed = null;
		/// <summary>
		/// 交易中.
		/// </summary>
		public OnIAPTransactionPurchasingDelegate OnTransactionPurchasing = null;

		/// <summary>
		/// 交易购买完成.
		/// </summary>
		public OnIAPTransactionPurchasedDelegate OnTransactionPurchased = null;

		/// <summary>
		/// 交易取消.
		/// </summary>
		public OnIAPTransactionCanceledDelegate OnTransactionCanceled = null;

		/// <summary>
		/// 交易延迟.
		/// </summary>
		public OnIAPTransactionDeferredDelegate OnTransactionDeferred = null;

		/// <summary>
		/// 购买恢复开始.
		/// </summary>
		public OnRestoreCompletedDelegate OnRestoreCompleted = null;

		/// <summary>
		/// 购买恢复.
		/// </summary>
		public OnTransactionRestoredDelegate OnTransactionRestored = null;


		/// <summary>
		/// 购买恢复结束.
		/// </summary>
		public OnRestoreFinishedDelegate OnRestoreFinished = null;
			
		/// <summary>
		/// 交易失败.
		/// </summary>
		public OnIAPFailedDelegate OnFailed = null;

		/// <summary>
		/// 验证所有产品信息.
		/// </summary>
		/// <param name="iProducts">产品列表.</param>
		public void ValidateProducts(string[] iProducts)
		{
			this.Info ("ValidateProducts()");
			if (iProducts == null) {
				this.Error ("ValidateProducts():Array of product identifiers must not be null!");
				return;
			}
			if (iProducts.Length == 0) {
				this.Error ("ValidateProducts():Array of product identifiers must not be empty!!");
				return;
			}
			if (null == this.OnProductsValidationStarted) {
				this.OnProductsValidationStarted = _OnProductsValidationStarted;
			}
			this.OnProductsValidationStarted(iProducts);
			ValidateIAPProducts(iProducts);
		}

		private void _OnProductsValidationStarted(string[] iProductIDs) {

			for(int idx = 0; idx < iProductIDs.Length; ++idx) {
				this.Info ("_OnProductsValidationStarted():({0}/{1})Product::{2}", 
					(idx+1), iProductIDs.Length, iProductIDs[idx]);
			}
		}

		/// <summary>
		/// 开始交易进程.
		/// </summary>
		public void StartProcessing() 
		{
			this.Info ("StartProcessing()");
			StartIAPProcessing();
		}

		/// <summary>
		/// 停止交易进程.
		/// </summary>
		public void StopProcessing() 
		{
			this.Info ("StopProcessing()");
			StopIAPProcessing();
		}

		/// <summary>
		/// 取得产品验证列表.
		/// </summary>
		/// <returns>产品验证列表.</returns>
		public IEnumerable<IIAPProduct> GetValidatedProducts()
		{
			return _validatedProducts.Values;
		}

		/// <summary>
		/// 取得产品验证数.
		/// </summary>
		/// <returns>产品验证数.</returns>
		public Int32 GetValidatedProductsCount()
		{
			return _validatedProducts.Count;
		}

		/// <summary>
		/// 取得产品验证信息.
		/// </summary>
		/// <returns>产品验证数.</returns>
		/// <param name="iProductId">产品ID.</param>
		public IIAPProduct GetValidatedProduct(string iProductId)
		{
			IIAPProduct result = null;
			_validatedProducts.TryGetValue(iProductId, out result);
			return result;
		}
	
		/// <summary>
		/// 开始交易（默认数量1）.
		/// </summary>
		/// <param name="iProductId">产品ID.</param>
		/// <param name="iQuantity">数量.</param>
		public void StartTransaction(string iProductID, Int32 iQuantity)
		{
			this.Info ("StartTransaction():ProductID::{0}({1})", iProductID, iQuantity);
			if (string.IsNullOrEmpty (iProductID) == true) {
				this.Error ("StartTransaction():Product identifier is null!!!");
				return;
			}
			if (iQuantity <= 0) {
				this.Error ("StartTransaction():The quantity of product is invalid!!!(<= 0)");
				return;
			}
			if (null == this.OnTransactionPurchasing) {
				this.OnTransactionPurchasing = _OnTransactionPurchasing;
			}
			this.OnTransactionPurchasing (iProductID, iQuantity);
			BuyIAPWithProductID(iProductID, iQuantity);
		}
			
		/// <summary>
		/// 交易中
		/// </summary>
		/// <param name="iProductID">产品ID.</param>
		/// <param name="iQuantity">数量.</param>
		private void _OnTransactionPurchasing(string iProductID, Int32 iQuantity) {
			this.Info ("_OnTransactionStarted():ProductID::{0}({1})", iProductID, iQuantity);
		}

		/// <summary>
		/// 交易恢复完成.
		/// </summary>
		public void RestoreCompletedTransactions()
		{
			this.Info ("RestoreCompletedTransactions()");
			if (null == this.OnRestoreCompleted) {
				this.OnRestoreCompleted = _OnRestoreCompleted;
			}
			this.OnRestoreCompleted ();
			RestoreCompletedIAPTransactions();
		}

		/// <summary>
		/// 购买恢复完成
		/// </summary>
		private void _OnRestoreCompleted() {
			this.Info ("_OnRestoreCompleted()");
		}

		/// <summary>
		/// 结束交易（务必在向玩家交付完商品后，结束）.
		/// </summary>
		/// <param name="iTransactionID">交易ID.</param>
		public void FinalizeTransaction(string iTransactionID)
		{
			this.Info ("FinalizeTransaction():TransactionID::{0}", iTransactionID);
			if (string.IsNullOrEmpty (iTransactionID) == true) {

				this.Error ("FinalizeTransaction():Transaction identifier is null!!!");
				return;
			}
			FinalizeIAPTransaction(iTransactionID);
		}

		/// <summary>
		/// 检测是否能够购买.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		public bool IsEnabled()
		{
			return IsIAPEnabled();
		}

		protected Dictionary<string,IIAPProduct> _validatedProducts = new Dictionary<string,IIAPProduct>();
		protected Dictionary<string,IIAPProduct> _updatedProducts = new Dictionary<string,IIAPProduct>();

		/// <summary>
		/// 追加产品.
		/// </summary>
		/// <param name="iProduct">产品.</param>
		public void AddProduct(IIAPProduct iProduct) {
			this.Info ("AddProduct():Add Product::{0}", iProduct.ToString());
			this._updatedProducts.Add (iProduct.ProductID, iProduct);
		}

		/// <summary>
		/// 产品列表验证完毕.
		/// </summary>
		public void OnProductsValidationFinished()
		{
			bool updated = false;
			if (_updatedProducts.Count == _validatedProducts.Count)
			{
				foreach (IIAPProduct product in _updatedProducts.Values)
				{
					if (!_validatedProducts.ContainsKey(product.ProductID))
					{
						updated = true;
						break;
					}
				}
			}
			else if (_updatedProducts.Count > 0)
			{
				updated = true;
			}
			if (updated)
			{
				_validatedProducts = _updatedProducts;
			}
			if (null != this.OnProductsValidationCompleted) {
				this.OnProductsValidationCompleted (updated, _validatedProducts);
			}
		}

#region virtual

		protected virtual bool IsIAPEnabled() { return false; }

		protected virtual void RestoreCompletedIAPTransactions() {}

#endregion


#region abstract

		protected abstract void StartIAPProcessing ();

		protected abstract void StopIAPProcessing ();

		protected abstract void ValidateIAPProducts(string[] iProducts);

		protected abstract void BuyIAPWithProductID (string iProductId, Int32 iQuantity);

		protected abstract void FinalizeIAPTransaction (string iTransactionID);

#endregion

	}
}