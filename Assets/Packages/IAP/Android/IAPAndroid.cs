using UnityEngine;
using System;
using System.Collections;

#if UNITY_ANDROID

namespace IAP
{
	public class IAPAndroid : IAPBase {

		protected override void ValidateIAPProducts(string[] products)
		{
			// IAPValidateProducts(products, (Int16)products.Length);
		}

		protected override void StartIAPProcessing() 
		{
			// IAPStartProcessing();
		}

		protected override void StopIAPProcessing() 
		{
			// IAPStopProcessing();
		}

		public override void BuyIAPWithProductID(string productId, Int32 quantity)
		{
			// IAPBuyWithProductID(productId, quantity);
		}

		protected override void RestoreCompletedIAPTransactions()
		{
			// IAPRestoreTransactions();
		}

		protected override void FinalizeIAPTransaction(string transactionID)
		{
			// IAPFinalizeTransaction(transactionID);
		}

		protected override bool IsIAPEnabled()
		{
			// return IAPIsEnabled();
			return true;
		}
	}
}

#endif