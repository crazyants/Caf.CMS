using CAF.Infrastructure.Core.Plugins;
using CAF.WebSite.Domain.Seedwork.Common;
using CAF.WebSite.Domain.Seedwork.Shop.Catalog;
using CAF.WebSite.Domain.Seedwork.Shop.Orders;
using CAF.WebSite.Domain.Seedwork.Tax;
using CAF.WebSite.Domain.Seedwork.Users;
using System;
using System.Collections.Generic;
 

namespace CAF.WebSite.Application.Services.Tax
{
    /// <summary>
    /// Tax service
    /// </summary>
    public partial interface ITaxService
    {
        /// <summary>
        /// Load active tax provider
        /// </summary>
        /// <returns>Active tax provider</returns>
        Provider<ITaxProvider> LoadActiveTaxProvider();

        /// <summary>
        /// Load tax provider by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Found tax provider</returns>
        Provider<ITaxProvider> LoadTaxProviderBySystemName(string systemName);

        /// <summary>
        /// Load all tax providers
        /// </summary>
        /// <returns>Tax providers</returns>
        IEnumerable<Provider<ITaxProvider>> LoadAllTaxProviders();
        



        /// <summary>
        /// Gets tax rate
        /// </summary>
		/// <param name="product">Product</param>
        /// <param name="user">User</param>
        /// <returns>Tax rate</returns>
        decimal GetTaxRate(Product product, User user);

        /// <summary>
        /// Gets tax rate
        /// </summary>
        /// <param name="taxCategoryId">Tax category identifier</param>
        /// <param name="user">User</param>
        /// <returns>Tax rate</returns>
        decimal GetTaxRate(int taxCategoryId, User user);
        
        /// <summary>
        /// Gets tax rate
        /// </summary>
		/// <param name="product">Product</param>
        /// <param name="taxCategoryId">Tax category identifier</param>
        /// <param name="user">User</param>
        /// <returns>Tax rate</returns>
        decimal GetTaxRate(Product product, int taxCategoryId,  User user);
        



        /// <summary>
        /// Gets price
        /// </summary>
		/// <param name="product">Product</param>
        /// <param name="price">Price</param>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>Price</returns>
        decimal GetProductPrice(Product product, decimal price, out decimal taxRate);

        /// <summary>
        /// Gets price
        /// </summary>
		/// <param name="product">Product</param>
        /// <param name="price">Price</param>
        /// <param name="user">User</param>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>Price</returns>
        decimal GetProductPrice(Product product, decimal price, User user, out decimal taxRate);

        /// <summary>
        /// Gets price
        /// </summary>
		/// <param name="product">Product</param>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="user">User</param>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>Price</returns>
        decimal GetProductPrice(Product product, decimal price, bool includingTax, User user, out decimal taxRate);

        /// <summary>
        /// Gets price
        /// </summary>
		/// <param name="product">Product</param>
        /// <param name="taxCategoryId">Tax category identifier</param>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="user">User</param>
        /// <param name="priceIncludesTax">A value indicating whether price already includes tax</param>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>Price</returns>
        decimal GetProductPrice(Product product, int taxCategoryId, decimal price,
            bool includingTax, User user,
            bool priceIncludesTax, out decimal taxRate);




        /// <summary>
        /// Gets shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="user">User</param>
        /// <returns>Price</returns>
        decimal GetShippingPrice(decimal price, User user);

        /// <summary>
        /// Gets shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="user">User</param>
        /// <returns>Price</returns>
        decimal GetShippingPrice(decimal price, bool includingTax, User user);

        /// <summary>
        /// Gets shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="user">User</param>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>Price</returns>
        decimal GetShippingPrice(decimal price, bool includingTax, User user, out decimal taxRate);





        /// <summary>
        /// Gets payment method additional handling fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="user">User</param>
        /// <returns>Price</returns>
        decimal GetPaymentMethodAdditionalFee(decimal price, User user);

        /// <summary>
        /// Gets payment method additional handling fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="user">User</param>
        /// <returns>Price</returns>
        decimal GetPaymentMethodAdditionalFee(decimal price, bool includingTax, User user);

        /// <summary>
        /// Gets payment method additional handling fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="user">User</param>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>Price</returns>
        decimal GetPaymentMethodAdditionalFee(decimal price, bool includingTax, User user, out decimal taxRate);







        /// <summary>
        /// Gets checkout attribute value price
        /// </summary>
        /// <param name="cav">Checkout attribute value</param>
        /// <returns>Price</returns>
        decimal GetCheckoutAttributePrice(CheckoutAttributeValue cav);

        /// <summary>
        /// Gets checkout attribute value price
        /// </summary>
        /// <param name="cav">Checkout attribute value</param>
        /// <param name="user">User</param>
        /// <returns>Price</returns>
        decimal GetCheckoutAttributePrice(CheckoutAttributeValue cav, User user);

        /// <summary>
        /// Gets checkout attribute value price
        /// </summary>
        /// <param name="cav">Checkout attribute value</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="user">User</param>
        /// <returns>Price</returns>
        decimal GetCheckoutAttributePrice(CheckoutAttributeValue cav,
            bool includingTax, User user);

        /// <summary>
        /// Gets checkout attribute value price
        /// </summary>
        /// <param name="cav">Checkout attribute value</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="user">User</param>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>Price</returns>
        decimal GetCheckoutAttributePrice(CheckoutAttributeValue cav,
            bool includingTax, User user, out decimal taxRate);




        

        /// <summary>
        /// Gets VAT Number status
        /// </summary>
        /// <param name="fullVatNumber">Two letter ISO code of a country and VAT number (e.g. GB 111 1111 111)</param>
        /// <returns>VAT Number status</returns>
        VatNumberStatus GetVatNumberStatus(string fullVatNumber);

        /// <summary>
        /// Gets VAT Number status
        /// </summary>
        /// <param name="fullVatNumber">Two letter ISO code of a country and VAT number (e.g. GB 111 1111 111)</param>
        /// <param name="name">Name (if received)</param>
        /// <param name="address">Address (if received)</param>
        /// <returns>VAT Number status</returns>
        VatNumberStatus GetVatNumberStatus(string fullVatNumber,
            out string name, out string address);
        /// <summary>
        /// Gets VAT Number status
        /// </summary>
        /// <param name="twoLetterIsoCode">Two letter ISO code of a country</param>
        /// <param name="vatNumber">VAT number</param>
        /// <returns>VAT Number status</returns>
        VatNumberStatus GetVatNumberStatus(string twoLetterIsoCode, string vatNumber);
        
        /// <summary>
        /// Gets VAT Number status
        /// </summary>
        /// <param name="twoLetterIsoCode">Two letter ISO code of a country</param>
        /// <param name="vatNumber">VAT number</param>
        /// <param name="name">Name (if received)</param>
        /// <param name="address">Address (if received)</param>
        /// <returns>VAT Number status</returns>
        VatNumberStatus GetVatNumberStatus(string twoLetterIsoCode, string vatNumber, 
            out string name, out string address);

        /// <summary>
        /// Performs a basic check of a VAT number for validity
        /// </summary>
        /// <param name="twoLetterIsoCode">Two letter ISO code of a country</param>
        /// <param name="vatNumber">VAT number</param>
        /// <param name="name">Company name</param>
        /// <param name="address">Address</param>
        /// <param name="exception">Exception</param>
        /// <returns>VAT number status</returns>
        VatNumberStatus DoVatCheck(string twoLetterIsoCode, string vatNumber, 
            out string name, out string address, out Exception exception);



        /// <summary>
        /// Gets a value indicating whether tax exempt
        /// </summary>
		/// <param name="product">Product</param>
        /// <param name="user">User</param>
		/// <returns>A value indicating whether a product is tax exempt</returns>
        bool IsTaxExempt(Product product, User user);

        /// <summary>
        /// Gets a value indicating whether EU VAT exempt (the European Union Value Added Tax)
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="user">User</param>
        /// <returns>Result</returns>
        bool IsVatExempt(Address address, User user);
    }
}
