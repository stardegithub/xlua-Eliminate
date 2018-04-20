namespace IAP
{
	/// <summary>
	/// 产品接口定义.
	/// </summary>
	public interface IIAPProduct 
	{
		/// <summary>
		/// 产品ID.
		/// </summary>
		string ProductID { get; set; }
		/// <summary>
		/// 标题.
		/// </summary>
		string Title { get; set; }	
		/// <summary>
		/// 描述.
		/// </summary>
		string Description { get; set; }
		/// <summary>
		/// 价格.
		/// </summary>
		decimal Price { get; set; }
		/// <summary>
		/// 价格描述.
		/// </summary>
		string PriceAsString { get; set; }
		/// <summary>
		/// 货币符号.
		/// </summary>
		string CurrencySymbol { get; set; }
		/// <summary>
		/// 货币代码.
		/// </summary>
		string CurrencyCode { get; set; }
		/// <summary>
		/// 本地ID.
		/// </summary>
		string LocaleIdentifier { get; set; }
		/// <summary>
		/// 国家代码.
		/// </summary>
		string CountryCode { get; set; }

		/// <summary>
		/// 转换为文字串.
		/// </summary>
		/// <returns>文字串</returns>
		string ToString();
	}

	/// <summary>
	/// 产品定义.
	/// </summary>
	public class Product : IIAPProduct
	{
		/// <summary>
		/// 产品ID.
		/// </summary>
		public string ProductID { get; set; }
		/// <summary>
		/// 标题.
		/// </summary>
		public string Title { get; set; }	
		/// <summary>
		/// 描述.
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// 价格.
		/// </summary>
		public decimal Price { get; set; }
		/// <summary>
		/// 价格描述.
		/// </summary>
		public string PriceAsString { get; set; }
		/// <summary>
		/// 货币符号.
		/// </summary>
		public string CurrencySymbol { get; set; }
		/// <summary>
		/// 货币代码.
		/// </summary>
		public string CurrencyCode { get; set; }
		/// <summary>
		/// 本地ID.
		/// </summary>
		public string LocaleIdentifier { get; set; }
		/// <summary>
		/// 国家代码.
		/// </summary>
		public string CountryCode { get; set; }

		public override bool Equals(System.Object righthand)
		{
			if (righthand.GetType() == this.GetType()) 
			{
				Product other = (Product)righthand;
				if (other == null) {
					return false;
				}
				return this.ProductID == other.ProductID &&
					this.Title == other.Title &&
					this.Description == other.Description &&
					this.Price == other.Price &&
					this.PriceAsString == other.PriceAsString &&
					this.CurrencySymbol == other.CurrencySymbol &&
					this.CurrencyCode == other.CurrencyCode &&
					this.LocaleIdentifier == other.LocaleIdentifier &&
					this.CountryCode == other.CountryCode;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return ProductID != null ? ProductID.GetHashCode() : 0;
		}
			
		/// <summary>
		/// 转换为文字串.
		/// </summary>
		/// <returns>文字串</returns>
		public string ToString() {
			return string.Format ("ProductID::{0} Title::{1} Description::{2} Price::{3} PriceAsString::{4} CurrencySymbol::{5} CurrencyCode::{6} LocaleIdentifier::{7} CountryCode::{8}",
				this.ProductID, this.Title, this.Description, this.Price, this.PriceAsString,
				this.CurrencySymbol, this.CurrencyCode, this.LocaleIdentifier, this.CountryCode);
		}
	}
}