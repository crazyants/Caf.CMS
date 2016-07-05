using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Plugins;
using CAF.WebSite.Application.Seedwork;
using CAF.WebSite.Application.Seedwork.Configuration;
using CAF.WebSite.Application.Services.Common;
using CAF.WebSite.Application.Services.Directory;
using CAF.WebSite.Domain.Seedwork.Common;
using CAF.WebSite.Domain.Seedwork.Shop.Catalog;
using CAF.WebSite.Domain.Seedwork.Shop.Orders;
using CAF.WebSite.Domain.Seedwork.Tax;
using CAF.WebSite.Domain.Seedwork.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace CAF.WebSite.Application.Services.Tax
{
    /// <summary>
    /// Tax service
    /// </summary>
    public partial class TaxService : ITaxService
	{
		#region Nested classes

		private class TaxAddressKey : Tuple<int, bool> // <UserId, IsEsd>
		{
			public TaxAddressKey(int userId, bool productIsEsd)
				: base(userId, productIsEsd)
			{
			}
		}

		#endregion

		#region Fields

		private static readonly DateTime _euEsdRegulationStart = new DateTime(2015, 01, 01);

		private readonly IAddressService _addressService;
        private readonly IWorkContext _workContext;
        private readonly TaxSettings _taxSettings;
		private readonly ShoppingCartSettings _cartSettings;
        private readonly IPluginFinder _pluginFinder;
        private readonly IDictionary<TaxRateCacheKey, decimal> _cachedTaxRates;
		private readonly IDictionary<TaxAddressKey, Address> _cachedTaxAddresses;
		private readonly ISettingService _settingService;
		private readonly IProviderManager _providerManager;
		private readonly IGeoCountryLookup _geoCountryLookup;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="addressService">Address service</param>
        /// <param name="workContext">Work context</param>
        /// <param name="taxSettings">Tax settings</param>
        /// <param name="pluginFinder">Plugin finder</param>
        public TaxService(
			IAddressService addressService,
            IWorkContext workContext,
            TaxSettings taxSettings,
			ShoppingCartSettings cartSettings,
            IPluginFinder pluginFinder,
			ISettingService settingService,
			IGeoCountryLookup geoCountryLookup,
			IProviderManager providerManager
			)
        {
            this._addressService = addressService;
			this._workContext = workContext;
			this._taxSettings = taxSettings;
			this._cartSettings = cartSettings;
			this._pluginFinder = pluginFinder;
			this._cachedTaxRates = new Dictionary<TaxRateCacheKey, decimal>();
			this._cachedTaxAddresses = new Dictionary<TaxAddressKey, Address>();
			this._settingService = settingService;
			this._providerManager = providerManager;
			this._geoCountryLookup = geoCountryLookup;
        }

        #endregion

        #region Nested class

        internal class TaxRateCacheKey : Tuple<int, int, int>
        {
            public TaxRateCacheKey(int variantId, int taxCategoryId, int userId)
                : base(variantId, taxCategoryId, userId)
            {
            }
        }

        #endregion

        #region Utilities

        internal TaxRateCacheKey CreateTaxRateCacheKey(Product product, int taxCategoryId, User user)
        {
            return new TaxRateCacheKey(
                product == null ? 0 : product.Id,
                taxCategoryId,
                user == null ? 0 : user.Id);
        }

        /// <summary>
        /// Create request for tax calculation
        /// </summary>
		/// <param name="product">Product</param>
        /// <param name="taxCategoryId">Tax category identifier</param>
        /// <param name="user">User</param>
        /// <returns>Package for tax calculation</returns>
        protected CalculateTaxRequest CreateCalculateTaxRequest(Product product, int taxCategoryId, User user)
        {
            var calculateTaxRequest = new CalculateTaxRequest();
            calculateTaxRequest.User = user;
            if (taxCategoryId > 0)
            {
                calculateTaxRequest.TaxCategoryId = taxCategoryId;
            }
            else
            {
                if (product != null)
                    calculateTaxRequest.TaxCategoryId = product.TaxCategoryId;
            }

            calculateTaxRequest.Address = this.GetTaxAddress(user, product);
            return calculateTaxRequest;
        }

		/// <summary>
		/// Gets a value indicating whether the given user is a consumer (NOT a business/company) within the EU
		/// </summary>
		/// <param name="user">User</param>
		/// <returns><c>true</c> if the user is a consumer, <c>false</c> otherwise</returns>
		/// <remarks>
		/// A user is assumed to be a consumer if the default tax address doesn't include a company name,
		/// OR if a company name was specified but the EU VAT number for this record is invalid.
		/// </remarks>
		protected virtual bool IsEuConsumer(User user)
		{
			if (user == null)
				return false;

			var address = user.BillingAddress;
			if (address != null && address.Company.IsEmpty())
			{
				// BillingAddress is explicitly set, but no CompanyName in there: so we assume a consumer 
				return true;
			}

			var country = address == null ? null : address.Country;

			if (country == null)
			{
				// No Country or BillingAddress set: try to resolve country from IP address
				_geoCountryLookup.IsEuIpAddress(user.LastIpAddress, out country);
			}

			if (country == null || !country.SubjectToVat)
			{
				return false;
			}

			// It's EU: check VAT number status
			var vatStatus = (VatNumberStatus)user.GetAttribute<int>(SystemUserAttributeNames.VatNumberStatusId);
			// companies with invalid VAT numbers are assumed to be consumers
			return vatStatus != VatNumberStatus.Valid;
		}

        protected virtual Address GetTaxAddress(User user, Product product = null)
        {
			int userId = user != null ? user.Id : 0;
			Address address = null;

			bool productIsEsd = product != null ? product.IsEsd : false;

			var cacheKey = new TaxAddressKey(userId, productIsEsd);
			if (_cachedTaxAddresses.TryGetValue(cacheKey, out address))
			{
				return address;
			}

			var basedOn = _taxSettings.TaxBasedOn;

			// According to the new EU VAT regulations for electronic services from 2015 on,
			// VAT must be charged in the EU country the user originates from (BILLING address).
			// In addition to this, the IP addresses' origin should also be checked for verification.
			if (DateTime.UtcNow > _euEsdRegulationStart)
			{
				if (_taxSettings.EuVatEnabled && productIsEsd)
				{
					if (IsEuConsumer(user))
					{
						basedOn = TaxBasedOn.BillingAddress;
					}
				}
			}

			if (basedOn == TaxBasedOn.BillingAddress && (user == null || user.BillingAddress == null))
			{
				basedOn = TaxBasedOn.DefaultAddress;
			}
			if (basedOn == TaxBasedOn.ShippingAddress && (user == null || user.ShippingAddress == null))
			{
				basedOn = TaxBasedOn.DefaultAddress;
			}

			switch (basedOn)
			{
				case TaxBasedOn.BillingAddress:
					address = user.BillingAddress;
					break;
				case TaxBasedOn.ShippingAddress:
					address = user.ShippingAddress;
					break;
				case TaxBasedOn.DefaultAddress:
				default:
					address = _addressService.GetAddressById(_taxSettings.DefaultTaxAddressId);
					break;
			}

			_cachedTaxAddresses[cacheKey] = address;

			return address;
        }

        /// <summary>
        /// Calculated price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="percent">Percent</param>
        /// <param name="increase">Increase</param>
        /// <returns>New price</returns>
        protected decimal CalculatePrice(decimal price, decimal percent, bool increase)
        {
            decimal result = decimal.Zero;
            if (percent == decimal.Zero)
                return price;

            if (increase)
            {
                result = price * (1 + percent / 100);
            }
            else
			{
				if (_cartSettings.RoundPricesDuringCalculation)
				{
					// Gross > Net RoundFix
					result = price - Math.Round((price) / (100 + percent) * percent, 2);
				}
				else
				{
					result = price - (price) / (100 + percent) * percent;
				}
			}
            return result;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load active tax provider
        /// </summary>
        /// <returns>Active tax provider</returns>
        public virtual Provider<ITaxProvider> LoadActiveTaxProvider()
        {
            var taxProvider = LoadTaxProviderBySystemName(_taxSettings.ActiveTaxProviderSystemName);
            if (taxProvider == null)
            {
                taxProvider = LoadAllTaxProviders().FirstOrDefault();
                _taxSettings.ActiveTaxProviderSystemName = taxProvider.Metadata.SystemName;
                _settingService.SaveSetting(_taxSettings);
            }
            return taxProvider;
        }

        /// <summary>
        /// Load tax provider by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Found tax provider</returns>
        public virtual Provider<ITaxProvider> LoadTaxProviderBySystemName(string systemName)
        {
			return _providerManager.GetProvider<ITaxProvider>(systemName);
        }

        /// <summary>
        /// Load all tax providers
        /// </summary>
        /// <returns>Tax providers</returns>
        public virtual IEnumerable<Provider<ITaxProvider>> LoadAllTaxProviders()
        {
			return _providerManager.GetAllProviders<ITaxProvider>();
        }


        private decimal GetOriginTaxRate(Product product)
        {
            return GetTaxRate(product, 0, null);
        }

        /// <summary>
        /// Gets tax rate
        /// </summary>
		/// <param name="product">Product</param>
        /// <param name="user">User</param>
        /// <returns>Tax rate</returns>
        public virtual decimal GetTaxRate(Product product, User user)
        {
            return GetTaxRate(product, product.TaxCategoryId, user);
        }

        /// <summary>
        /// Gets tax rate
        /// </summary>
        /// <param name="taxCategoryId">Tax category identifier</param>
        /// <param name="user">User</param>
        /// <returns>Tax rate</returns>
        public virtual decimal GetTaxRate(int taxCategoryId, User user)
        {
            return GetTaxRate(null, taxCategoryId, user);
        }

        /// <summary>
        /// Gets tax rate
        /// </summary>
		/// <param name="product">Product</param>
        /// <param name="taxCategoryId">Tax category identifier</param>
        /// <param name="user">User</param>
        /// <returns>Tax rate</returns>
        public virtual decimal GetTaxRate(Product product, int taxCategoryId, User user)
        {
            var cacheKey = this.CreateTaxRateCacheKey(product, taxCategoryId, user);
            decimal result;
            if (!_cachedTaxRates.TryGetValue(cacheKey, out result))
            {
                result = GetTaxRateCore(product, taxCategoryId, user);
                _cachedTaxRates[cacheKey] = result;
            }

            return result;
        }

        protected virtual decimal GetTaxRateCore(Product product, int taxCategoryId, User user)
        {
			// active tax provider
			var activeTaxProvider = LoadActiveTaxProvider();
			if (activeTaxProvider == null)
			{
				return decimal.Zero;
			}
			
			// tax request
            var calculateTaxRequest = CreateCalculateTaxRequest(product, taxCategoryId, user);

			#region Legacy
			////make EU VAT exempt validation (the European Union Value Added Tax) (VATFIX)
            //if (_taxSettings.EuVatEnabled && IsVatExempt(calculateTaxRequest.Address, calculateTaxRequest.User))
            //{
            //    //return zero if VAT is not chargeable
            //    return decimal.Zero;
			//}
			#endregion

			//get tax rate
            var calculateTaxResult = activeTaxProvider.Value.GetTaxRate(calculateTaxRequest);
            if (calculateTaxResult.Success)
            {
                // ensure that tax is equal or greater than zero
                return Math.Max(0, calculateTaxResult.TaxRate);
            }
            else
            {
                return decimal.Zero;
            }
        }


        /// <summary>
        /// Gets price
        /// </summary>
		/// <param name="product">Product</param>
        /// <param name="price">Price</param>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>Price</returns>
        public virtual decimal GetProductPrice(Product product, decimal price,
            out decimal taxRate)
        {
            var user = _workContext.CurrentUser;
            return GetProductPrice(product, price, user, out taxRate);
        }

        /// <summary>
        /// Gets price
        /// </summary>
		/// <param name="product">Product</param>
        /// <param name="price">Price</param>
        /// <param name="user">User</param>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>Price</returns>
        public virtual decimal GetProductPrice(Product product, decimal price,
            User user, out decimal taxRate)
        {
            bool includingTax = _workContext.TaxDisplayType == TaxDisplayType.IncludingTax;
            return GetProductPrice(product, price, includingTax, user, out taxRate);
        }

        /// <summary>
        /// Gets price
        /// </summary>
		/// <param name="product">Product</param>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="user">User</param>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>Price</returns>
        public virtual decimal GetProductPrice(Product product, decimal price,
            bool includingTax, User user, out decimal taxRate)
        {
            bool priceIncludesTax = _taxSettings.PricesIncludeTax;
            int taxCategoryId = product.TaxCategoryId; // 0; // (VATFIX)
            return GetProductPrice(product, taxCategoryId, price, includingTax,
                user, priceIncludesTax, out taxRate);
        }

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
        public virtual decimal GetProductPrice(
			Product product, 
			int taxCategoryId,
            decimal price, 
			bool includingTax, 
			User user,
            bool priceIncludesTax, 
			out decimal taxRate)
        {
			// don't calculate if price is 0
			if (price == decimal.Zero)
			{
				taxRate = decimal.Zero;
				return taxRate;
			}
			
			taxRate = GetTaxRate(product, taxCategoryId, user);

            // Admin: GROSS prices
            if (priceIncludesTax)
            {
                if (!includingTax)
                {
                    price = CalculatePrice(price, taxRate, false);
                }
            }
            // Admin: NET prices
            else
            {
                if (includingTax)
                {
                    price = CalculatePrice(price, taxRate, true);
                }
            }

            //allowed to support negative price adjustments
            //if (price < decimal.Zero)
            //    price = decimal.Zero;

            return price;
        }




        /// <summary>
        /// Gets shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="user">User</param>
        /// <returns>Price</returns>
        public virtual decimal GetShippingPrice(decimal price, User user)
        {
            bool includingTax = _workContext.TaxDisplayType == TaxDisplayType.IncludingTax;
            return GetShippingPrice(price, includingTax, user);
        }

        /// <summary>
        /// Gets shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="user">User</param>
        /// <returns>Price</returns>
        public virtual decimal GetShippingPrice(decimal price, bool includingTax, User user)
        {
            decimal taxRate = decimal.Zero;
            return GetShippingPrice(price, includingTax, user, out taxRate);
        }

        /// <summary>
        /// Gets shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="user">User</param>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>Price</returns>
        public virtual decimal GetShippingPrice(decimal price, bool includingTax, User user, out decimal taxRate)
        {
            taxRate = decimal.Zero;

            if (!_taxSettings.ShippingIsTaxable)
            {
                return price;
            }

            bool priceIncludesTax = _taxSettings.ShippingPriceIncludesTax;
            int taxClassId = _taxSettings.ShippingTaxClassId;
            return GetProductPrice(null, taxClassId, price, includingTax, user,
                priceIncludesTax, out taxRate);
        }





        /// <summary>
        /// Gets payment method additional handling fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="user">User</param>
        /// <returns>Price</returns>
        public virtual decimal GetPaymentMethodAdditionalFee(decimal price, User user)
        {
            bool includingTax = _workContext.TaxDisplayType == TaxDisplayType.IncludingTax;
            return GetPaymentMethodAdditionalFee(price, includingTax, user);
        }

        /// <summary>
        /// Gets payment method additional handling fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="user">User</param>
        /// <returns>Price</returns>
        public virtual decimal GetPaymentMethodAdditionalFee(decimal price, bool includingTax, User user)
        {
            decimal taxRate = decimal.Zero;
            return GetPaymentMethodAdditionalFee(price, includingTax,
                user, out taxRate);
        }

        /// <summary>
        /// Gets payment method additional handling fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="user">User</param>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>Price</returns>
        public virtual decimal GetPaymentMethodAdditionalFee(decimal price, bool includingTax, User user, out decimal taxRate)
        {
            taxRate = decimal.Zero;

            if (!_taxSettings.PaymentMethodAdditionalFeeIsTaxable)
            {
                return price;
            }

            bool priceIncludesTax = _taxSettings.PaymentMethodAdditionalFeeIncludesTax;
            int taxClassId = _taxSettings.PaymentMethodAdditionalFeeTaxClassId;
            return GetProductPrice(null, taxClassId, price, includingTax, user,
                priceIncludesTax, out taxRate);
        }





        /// <summary>
        /// Gets checkout attribute value price
        /// </summary>
        /// <param name="cav">Checkout attribute value</param>
        /// <returns>Price</returns>
        public virtual decimal GetCheckoutAttributePrice(CheckoutAttributeValue cav)
        {
            var user = _workContext.CurrentUser;
            return GetCheckoutAttributePrice(cav, user);
        }

        /// <summary>
        /// Gets checkout attribute value price
        /// </summary>
        /// <param name="cav">Checkout attribute value</param>
        /// <param name="user">User</param>
        /// <returns>Price</returns>
        public virtual decimal GetCheckoutAttributePrice(CheckoutAttributeValue cav, User user)
        {
            bool includingTax = _workContext.TaxDisplayType == TaxDisplayType.IncludingTax;
            return GetCheckoutAttributePrice(cav, includingTax, user);
        }

        /// <summary>
        /// Gets checkout attribute value price
        /// </summary>
        /// <param name="cav">Checkout attribute value</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="user">User</param>
        /// <returns>Price</returns>
        public virtual decimal GetCheckoutAttributePrice(CheckoutAttributeValue cav,
            bool includingTax, User user)
        {
            decimal taxRate = decimal.Zero;
            return GetCheckoutAttributePrice(cav, includingTax, user, out taxRate);
        }

        /// <summary>
        /// Gets checkout attribute value price
        /// </summary>
        /// <param name="cav">Checkout attribute value</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="user">User</param>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>Price</returns>
        public virtual decimal GetCheckoutAttributePrice(CheckoutAttributeValue cav,
            bool includingTax, User user, out decimal taxRate)
        {
            if (cav == null)
                throw new ArgumentNullException("cav");

            taxRate = decimal.Zero;

            bool priceIncludesTax = _taxSettings.PricesIncludeTax;

            decimal price = cav.PriceAdjustment;
            if (cav.CheckoutAttribute.IsTaxExempt)
            {
                return price;
            }

            int taxClassId = cav.CheckoutAttribute.TaxCategoryId;
            return GetProductPrice(null, taxClassId, price, includingTax, user,
                priceIncludesTax, out taxRate);
        }





        /// <summary>
        /// Gets VAT Number status
        /// </summary>
        /// <param name="fullVatNumber">Two letter ISO code of a country and VAT number (e.g. GB 111 1111 111)</param>
        /// <returns>VAT Number status</returns>
        public virtual VatNumberStatus GetVatNumberStatus(string fullVatNumber)
        {
            string name, address;
            return GetVatNumberStatus(fullVatNumber, out name, out address);
        }

        /// <summary>
        /// Gets VAT Number status
        /// </summary>
        /// <param name="fullVatNumber">Two letter ISO code of a country and VAT number (e.g. GB 111 1111 111)</param>
        /// <param name="name">Name (if received)</param>
        /// <param name="address">Address (if received)</param>
        /// <returns>VAT Number status</returns>
        public virtual VatNumberStatus GetVatNumberStatus(string fullVatNumber, out string name, out string address)
        {
            name = string.Empty;
            address = string.Empty;

            if (String.IsNullOrWhiteSpace(fullVatNumber))
                return VatNumberStatus.Empty;
            fullVatNumber = fullVatNumber.Trim();

            //GB 111 1111 111 or GB 1111111111
            //more advanced regex - http://codeigniter.com/wiki/European_Vat_Checker
            var r = new Regex(@"^(\w{2})(.*)");
            var match = r.Match(fullVatNumber);
            if (!match.Success)
                return VatNumberStatus.Invalid;
            var twoLetterIsoCode = match.Groups[1].Value;
            var vatNumber = match.Groups[2].Value;

            return GetVatNumberStatus(twoLetterIsoCode, vatNumber, out name, out address);
        }

        /// <summary>
        /// Gets VAT Number status
        /// </summary>
        /// <param name="twoLetterIsoCode">Two letter ISO code of a country</param>
        /// <param name="vatNumber">VAT number</param>
        /// <returns>VAT Number status</returns>
        public virtual VatNumberStatus GetVatNumberStatus(string twoLetterIsoCode, string vatNumber)
        {
            string name, address;
            return GetVatNumberStatus(twoLetterIsoCode, vatNumber, out name, out address);
        }

        /// <summary>
        /// Gets VAT Number status
        /// </summary>
        /// <param name="twoLetterIsoCode">Two letter ISO code of a country</param>
        /// <param name="vatNumber">VAT number</param>
        /// <param name="name">Name (if received)</param>
        /// <param name="address">Address (if received)</param>
        /// <returns>VAT Number status</returns>
        public virtual VatNumberStatus GetVatNumberStatus(string twoLetterIsoCode, string vatNumber,
            out string name, out string address)
        {
            name = string.Empty;
            address = string.Empty;

            if (String.IsNullOrEmpty(twoLetterIsoCode) || String.IsNullOrEmpty(vatNumber))
                return VatNumberStatus.Empty;

            if (!_taxSettings.EuVatUseWebService)
                return VatNumberStatus.Unknown;

            Exception exception = null;
            return DoVatCheck(twoLetterIsoCode, vatNumber, out name, out address, out exception);
        }

        /// <summary>
        /// Performs a basic check of a VAT number for validity
        /// </summary>
        /// <param name="twoLetterIsoCode">Two letter ISO code of a country</param>
        /// <param name="vatNumber">VAT number</param>
        /// <param name="name">Company name</param>
        /// <param name="address">Address</param>
        /// <param name="exception">Exception</param>
        /// <returns>VAT number status</returns>
        public virtual VatNumberStatus DoVatCheck(string twoLetterIsoCode, string vatNumber,
            out string name, out string address, out Exception exception)
        {
            name = string.Empty;
            address = string.Empty;

            if (vatNumber == null)
                vatNumber = string.Empty;
            vatNumber = vatNumber.Trim().Replace(" ", "");

            if (twoLetterIsoCode == null)
                twoLetterIsoCode = string.Empty;
            if (!String.IsNullOrEmpty(twoLetterIsoCode))
                //The service returns INVALID_INPUT for country codes that are not uppercase.
                twoLetterIsoCode = twoLetterIsoCode.ToUpper();

            EuropaCheckVatService.checkVatPortTypeClient s = null;

            try
            {
                bool valid;

                s = new EuropaCheckVatService.checkVatPortTypeClient(); 

                s.checkVat(ref twoLetterIsoCode, ref vatNumber, out valid, out name, out address);
                exception = null;
                return valid ? VatNumberStatus.Valid : VatNumberStatus.Invalid;
            }
            catch (Exception ex)
            {
                name = address = string.Empty;
                exception = ex;
                return VatNumberStatus.Unknown;
            }
            finally
            {
                if (name == null)
                    name = string.Empty;

                if (address == null)
                    address = string.Empty;

                if (s != null)
                    s.Close();
            }
        }


        /// <summary>
        /// Gets a value indicating whether tax exempt
        /// </summary>
		/// <param name="product">Product</param>
        /// <param name="user">User</param>
        /// <returns>A value indicating whether a product is tax exempt</returns>
        public virtual bool IsTaxExempt(Product product, User user)
        {
            if (user != null)
            {
                if (user.IsTaxExempt)
                    return true;

                if (user.UserRoles.Where(cr => cr.Active).Any(cr => cr.TaxExempt))
                    return true;
            }

            if (product == null)
            {
                return false;
            }

            if (product.IsTaxExempt)
            {
                return true;
            }

            return false;
        }

        public virtual bool IsVatExempt(Address address, User user)
        {
            if (!_taxSettings.EuVatEnabled)
            {
                return false;
            }

            if (user == null)
            {
                return false;
            }

            if (address == null)
            {
                address = GetTaxAddress(user);
            }

            if (address == null || address.Country == null)
            {
                return false;
            }

            if (!address.Country.SubjectToVat)
            {
                // VAT not chargeable if shipping outside VAT zone:
                return true;
            }
            else
            {
                // VAT not chargeable if address, user and config meet our VAT exemption requirements:
                // returns true if this user is VAT exempt because they are shipping within the EU but outside our shop country, 
                // they have supplied a validated VAT number, and the shop is configured to allow VAT exemption
                if (address.CountryId == _taxSettings.EuVatShopCountryId)
                    return false;

                var userVatStatus = (VatNumberStatus)user.GetAttribute<int>(SystemUserAttributeNames.VatNumberStatusId);
                return userVatStatus == VatNumberStatus.Valid && _taxSettings.EuVatAllowVatExemption;
            }
        }

        #endregion
    }
}
