using System;

namespace IAP
{
	public class IAPDummy : IAPBase
	{	


		protected override void StartIAPProcessing () {
		}

		protected override void StopIAPProcessing () {
		}

		protected override void ValidateIAPProducts (string[] products)
		{
			foreach (string s in products)
			{
//				IIAPProduct product = new Product {title = s, description = "Test Product", productIdentifier = s,
//					price =  4.99m, priceAsString = "$4.99", currencySymbol = "$", currencyCode = "USD", localeIdentifier = "en_US", countryCode = "US"};
//				_updatedProducts.Add (s, product);
			}
//			finishProductsValidation();
		}
			
		protected override void BuyIAPWithProductID (string iProductId, Int32 iQuantity) {
		}

		protected override void FinalizeIAPTransaction (string iTransactionID) {
		}
	}
}
