using System;
using System.Collections.Generic;

namespace IAP
{
	/// <summary>
	/// IAP支付类型.
	/// </summary>
	public enum IAPPayType {
		/// <summary>
		/// 无.
		/// </summary>
		None,
		/// <summary>
		/// 苹果支付.
		/// </summary>
		Apply,
		/// <summary>
		/// 支付宝.
		/// </summary>
		AliPay,
		/// <summary>
		/// 微信.
		/// </summary>
		WebChat
	}

	/// <summary>
	/// IAP状态.
	/// </summary>
	public enum IAPState {
		/// <summary>
		/// 无.
		/// </summary>
		None,
		/// <summary>
		/// 已闲置.
		/// </summary>
		Idled,
		/// <summary>
		/// 已超时.
		/// </summary>
		TimeOuted,
		/// <summary>
		/// 运行中.
		/// </summary>
		Runing,
		/// <summary>
		/// 已失败.
		/// </summary>
		Failed,
		/// <summary>
		/// 已取消.
		/// </summary>
		Canceled
	}

	/// <summary>
	/// IAP错误Code.
	/// </summary>
	public enum IAPErrorCode {
		/// <summary>
		/// 无.
		/// </summary>
		None,
		/// <summary>
		/// 产品验证失败.
		/// </summary>
		ProductsVerifyFailed,
		/// <summary>
		/// 下订单失败.
		/// </summary>
		OrderFailed,
		/// <summary>
		/// 购买失败.
		/// </summary>
		PurchaseFailed,
		/// <summary>
		/// 收据验证失败.
		/// </summary>
		ReceiptVerifyFailed,
		/// <summary>
		/// 收据重复验证.
		/// </summary>
		ReceiptReverify,
		/// <summary>
		/// 交易失败.
		/// </summary>
		TranscationFailed,
		/// <summary>
		/// 购买恢复失败.
		/// </summary>
		RestoreFailed,
		/// <summary>
		/// 未知类型.
		/// </summary>
		Unknown
	}

	/// <summary>
	/// IAP动作步骤.
	/// </summary>
	public enum IAPActionStep {
		/// <summary>
		/// 无.
		/// </summary>
		None,
		/// <summary>
		/// 产品列表验证中.
		/// </summary>
		ProductsVerifying,
		/// <summary>
		/// 下订单中.
		/// </summary>
		Ordering,
		/// <summary>
		/// 交易中.
		/// </summary>
		Purchasing,
		/// <summary>
		/// 收据验证中.
		/// </summary>
		ReceiptVerifying
	}

	/// <summary>
	/// 产品列表验证完毕.
	/// </summary>
	/// <param name="iProducts">产品列表.</param>
	public delegate void OnIAPProductsValidationStartedDelegate(string[] iProducts);

	/// <summary>
	/// 产品列表验证完毕.
	/// </summary>
	/// <param name="iUpdated">是否有更新.</param>
	/// <param name="iProducts">Products列表.</param>
	public delegate void OnIAPProductsValidationCompletedDelegate(bool iUpdated, Dictionary<string,IIAPProduct> iProducts);

	/// <summary>
	/// 验证失败.
	/// </summary>
	/// <param name="iProductId">ID.</param>
	/// <param name="iDescription">描述.</param>
	/// <param name="iPrice">价格.</param>
	public delegate void OnIAPProductsValidationFailedDelegate(string iDescription, Int32 iPrice);

	/// <summary>
	/// 交易中
	/// </summary>
	/// <param name="iProductID">产品ID.</param>
	/// <param name="iQuantity">购买数量.</param>
	public delegate void OnIAPTransactionPurchasingDelegate(string iProductID, int iQuantity);

	/// <summary>
	/// 交易取消.
	/// </summary>
	/// <param name="iProductID">产品ID.</param>
	/// <param name="iQuantity">购买数量.</param>
	public delegate void OnIAPTransactionCanceledDelegate(string iProductID, int iQuantity);

	/// <summary>
	/// 交易延迟.
	/// </summary>
	/// <param name="iProductID">产品ID.</param>
	public delegate void OnIAPTransactionDeferredDelegate(string iProductID);

	/// <summary>
	/// 交易购买完成回调事件委托.
	/// </summary>
	/// <param name="iProductID">产品ID.</param>
	/// <param name="iQuantity">数量.</param>
	/// <param name="iTransactionID">交易ID.</param>
	/// <param name="iReceipt">收据.</param>
	public delegate void OnIAPTransactionPurchasedDelegate(string iProductID, Int32 iQuantity, string iTransactionID, string iReceipt);

	/// <summary>
	/// 购买恢复完成.
	/// </summary>
	public delegate void OnRestoreCompletedDelegate();

	/// <summary>
	/// 购买恢复.
	/// </summary>
	/// <param name="iProductID">产品ID.</param>
	/// <param name="iQuantity">数量.</param>
	/// <param name="iTransactionID">交易ID.</param>
	/// <param name="iReceipt">收据.</param>
	public delegate void OnTransactionRestoredDelegate(string iProductID, int iQuantity, string iTransactionID, string iReceipt);

	/// <summary>
	/// 购买恢复结束.
	/// </summary>
	public delegate void OnRestoreFinishedDelegate();

	/// <summary>
	/// 交易失败.
	/// </summary>
	/// <param name="iErrorCode">Error Code.</param>
	/// <param name="iErrorDetailCode">错误详细Code.</param>
	/// <param name="iErrorDetailInfo">错误详细.</param>
	public delegate void OnIAPFailedDelegate(IAPErrorCode iErrorCode, int iErrorDetailCode, string iErrorDetailInfo);


}

