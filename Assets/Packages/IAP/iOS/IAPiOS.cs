using UnityEngine;
using System;
using System.Collections.Generic;
using Common;

#if (UNITY_IOS || UNITY_IPHONE)

namespace IAP
{
		
	public class IAPiOS : IAPBase
	{
		protected override void ValidateIAPProducts(string[] products)
		{
			this.Info ("ValidateIAPProducts()");
			if(Application.isMobilePlatform == false) {
				if (null != this.OnProductsValidationCompleted) {
					this.OnProductsValidationCompleted(false, null);
				}
				return;
			}
			IAPMsgSwitch.IAPValidateProducts(products, (Int16)products.Length);
		}

		protected override void StartIAPProcessing() 
		{
			this.Info ("StartIAPProcessing()");
			if(Application.isMobilePlatform == false) {
				return;
			}
			_updatedProducts.Clear ();
			IAPMsgSwitch.IAPStartProcessing();
		}

		protected override void StopIAPProcessing() 
		{
			this.Info ("StopIAPProcessing()");
			if(Application.isMobilePlatform == false) {
				return;
			}
			_updatedProducts.Clear ();
			IAPMsgSwitch.IAPStopProcessing();
		}

		protected override void BuyIAPWithProductID(string productId, Int32 quantity)
		{
			this.Info ("BuyIAPWithProductID():ProductId::{0} Quantity::{1}", productId, quantity);
			if(Application.isMobilePlatform == false) {
				if (null != this.OnTransactionPurchased) {
					this.OnTransactionPurchased(productId, quantity, null, null);
				}
				return;
			}
			IAPMsgSwitch.IAPBuyWithProductID(productId, quantity);
		}

		protected override void RestoreCompletedIAPTransactions()
		{
			this.Info ("RestoreCompletedIAPTransactions()");
			if(Application.isMobilePlatform == false) {
				return;
			}
			IAPMsgSwitch.IAPRestoreTransactions();
		}

		protected override void FinalizeIAPTransaction(string transactionID)
		{
			this.Info ("FinalizeIAPTransaction():TransactionID::{0}", transactionID);
			if(Application.isMobilePlatform == false) {
				return;
			}
			IAPMsgSwitch.IAPFinalizeTransaction(transactionID);
		}

		protected override bool IsIAPEnabled()
		{
			if(Application.isMobilePlatform == false) {
				return false;
			}
			return IAPMsgSwitch.IAPIsEnabled();
		}
	}
}
#endif