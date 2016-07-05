using CAF.WebSite.Domain.Seedwork.Shop.Catalog;
using CAF.WebSite.Domain.Seedwork.Shop.Orders;
using CAF.WebSite.Domain.Seedwork.Users;
using System.Collections.Generic;
using System.Collections.Specialized;
 

namespace CAF.WebSite.Application.Services.Orders
{
	public class AddToCartContext
	{
		public AddToCartContext()
		{
			Warnings = new List<string>();
			UserEnteredPrice = decimal.Zero;
			ChildItems = new List<ShoppingCartItem>();
		}

		public List<string> Warnings { get; set; }

		public ShoppingCartItem Item { get; set; }
		public List<ShoppingCartItem> ChildItems { get; set; }
		public ProductBundleItem BundleItem { get; set; }

		public User User { get; set; }
		public Product Product { get; set; }
		public ShoppingCartType CartType { get; set; }
		public NameValueCollection AttributeForm { get; set; }
		public string Attributes { get; set; }
		public decimal UserEnteredPrice { get; set; }
		public int Quantity { get; set; }
		public bool AddRequiredProducts { get; set; }
		public int? SiteId { get; set; }

		public int BundleItemId
		{
			get
			{
				return (BundleItem == null ? 0 : BundleItem.Id);
			}
		}
	}
}
