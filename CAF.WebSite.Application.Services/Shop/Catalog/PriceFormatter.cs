using System;
using System.Globalization;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core.Pages;
using CAF.Infrastructure.Core.Plugins;
using CAF.WebSite.Domain.Seedwork.Shop.Catalog;
using CAF.WebSite.Domain.Cms.Media;
using CAF.WebSite.Application.Services.Media;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Seedwork;
using CAF.WebSite.Application.Services.Directory;
using CAF.WebSite.Domain.Seedwork.Tax;
using CAF.WebSite.Domain.Seedwork.Directory;
using CAF.WebSite.Domain.Seedwork.Localization;

namespace CAF.WebSite.Application.Services.Catalog
{
    /// <summary>
    /// Price formatter
    /// </summary>
    public partial class PriceFormatter : IPriceFormatter
    {
        private readonly IWorkContext _workContext;
        private readonly ICurrencyService _currencyService;
        private readonly ILocalizationService _localizationService;
        private readonly TaxSettings _taxSettings;

        public PriceFormatter(IWorkContext workContext,
            ICurrencyService currencyService,
            ILocalizationService localizationService,
            TaxSettings taxSettings)
        {
            this._workContext = workContext;
            this._currencyService = currencyService;
            this._localizationService = localizationService;
            this._taxSettings = taxSettings;
        }

        #region Utilities

        /// <summary>
        /// Gets currency string
        /// </summary>
        /// <param name="amount">Amount</param>
        /// <returns>Currency string without exchange rate</returns>
        protected string GetCurrencyString(decimal amount)
        {
            bool showCurrency = true;
            var targetCurrency = _workContext.WorkingCurrency;
            return GetCurrencyString(amount, showCurrency, targetCurrency);
        }

        /// <summary>
        /// Gets currency string
        /// </summary>
        /// <param name="amount">Amount</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="targetCurrency">Target currency</param>
        /// <returns>Currency string without exchange rate</returns>
        protected string GetCurrencyString(decimal amount, bool showCurrency, Currency targetCurrency)
        {
            string result = string.Empty;

			var fmt = NumberFormatInfo.CurrentInfo;
			try
			{
				fmt = CultureInfo.CreateSpecificCulture(targetCurrency.DisplayLocale).NumberFormat;

				if (!showCurrency)
					fmt.CurrencySymbol = "";
			}
			catch { }


            if (targetCurrency.CustomFormatting.HasValue())
            {
                result = amount.ToString(targetCurrency.CustomFormatting, fmt);
            }
            else
            {
                if (targetCurrency.DisplayLocale.HasValue())
                {
                    result = amount.ToString("C", fmt);
                }
                else
                {
                    result = String.Format("{0} {1}", amount.ToString("N"), showCurrency ? targetCurrency.CurrencyCode : "").TrimEnd();
                    return result;
                }
            }

            return result;
        }

        #endregion

        #region Methods

        public string FormatPrice(decimal price)
        {
            return FormatPrice(price, true, _workContext.WorkingCurrency);
        }

        public string FormatPrice(decimal price, bool showCurrency, Currency targetCurrency)
        {
            var language = _workContext.WorkingLanguage;
            bool priceIncludesTax = false;
            switch (_workContext.TaxDisplayType)
            {
                case TaxDisplayType.ExcludingTax:
                    priceIncludesTax = false;
                    break;
                case TaxDisplayType.IncludingTax:
                    priceIncludesTax = true;
                    break;
            }
            return FormatPrice(price, showCurrency, targetCurrency, language, priceIncludesTax);
        }

        public string FormatPrice(decimal price, bool showCurrency, bool showTax)
        {
            var targetCurrency = _workContext.WorkingCurrency;
            var language = _workContext.WorkingLanguage;
            bool priceIncludesTax = false;
            switch (_workContext.TaxDisplayType)
            {
                case TaxDisplayType.ExcludingTax:
                    priceIncludesTax = false;
                    break;
                case TaxDisplayType.IncludingTax:
                    priceIncludesTax = true;
                    break;
            }
            return FormatPrice(price, showCurrency, targetCurrency, language, priceIncludesTax, showTax);
        }

        public string FormatPrice(decimal price, bool showCurrency, string currencyCode, bool showTax, Language language)
        {
			var currency = _currencyService.GetCurrencyByCode(currencyCode) ?? new Currency { CurrencyCode = currencyCode };
            bool priceIncludesTax = false;
            switch (_workContext.TaxDisplayType)
            {
                case TaxDisplayType.ExcludingTax:
                    priceIncludesTax = false;
                    break;
                case TaxDisplayType.IncludingTax:
                    priceIncludesTax = true;
                    break;
            }

            return FormatPrice(price, showCurrency, currency, 
                language, priceIncludesTax, showTax);
        }

        public string FormatPrice(decimal price, bool showCurrency, string currencyCode, Language language, bool priceIncludesTax)
        {
			bool showTax = _taxSettings.DisplayTaxSuffix;
			return FormatPrice(price, showCurrency, currencyCode, language, priceIncludesTax, showTax);
        }

		public string FormatPrice(decimal price, bool showCurrency, string currencyCode, Language language, bool priceIncludesTax, bool showTax)
		{
			var currency = _currencyService.GetCurrencyByCode(currencyCode) ?? new Currency { CurrencyCode = currencyCode };
			return FormatPrice(price, showCurrency, currency, language, priceIncludesTax, showTax);
		}

        public string FormatPrice(decimal price, bool showCurrency, Currency targetCurrency, Language language, bool priceIncludesTax)
        {
            bool showTax = _taxSettings.DisplayTaxSuffix;
            return FormatPrice(price, showCurrency, targetCurrency, language, priceIncludesTax, showTax);
        }

        public string FormatPrice(decimal price, bool showCurrency, Currency targetCurrency, Language language, bool priceIncludesTax, bool showTax)
        {
            // Round before rendering (also take "BitCoin" into account, where more than 2 decimal places are relevant)
            price = targetCurrency.CurrencyCode.IsCaseInsensitiveEqual("btc") ? Math.Round(price, 6) : Math.Round(price, 2);
            
            string currencyString = GetCurrencyString(price, showCurrency, targetCurrency);
            if (showTax)
            {
                //show tax suffix
                string formatStr;
                if (priceIncludesTax)
                {
                    formatStr = _localizationService.GetResource("Products.InclTaxSuffix", language.Id, false);
                    if (String.IsNullOrEmpty(formatStr))
                        formatStr = "{0} incl tax";
                }
                else
                {
                    formatStr = _localizationService.GetResource("Products.ExclTaxSuffix", language.Id, false);
                    if (String.IsNullOrEmpty(formatStr))
                        formatStr = "{0} excl tax";
                }
                return string.Format(formatStr, currencyString);
            }
            else
                return currencyString;
        }



        public string FormatShippingPrice(decimal price, bool showCurrency)
        {
            var targetCurrency = _workContext.WorkingCurrency;
            var language = _workContext.WorkingLanguage;
            bool priceIncludesTax = false;
            switch (_workContext.TaxDisplayType)
            {
                case TaxDisplayType.ExcludingTax:
                    priceIncludesTax = false;
                    break;
                case TaxDisplayType.IncludingTax:
                    priceIncludesTax = true;
                    break;
            }
            return FormatShippingPrice(price, showCurrency, targetCurrency, language, priceIncludesTax);
        }

        public string FormatShippingPrice(decimal price, bool showCurrency, Currency targetCurrency, Language language, bool priceIncludesTax)
        {
            bool showTax = _taxSettings.ShippingIsTaxable && _taxSettings.DisplayTaxSuffix;
            return FormatShippingPrice(price, showCurrency, targetCurrency, language, priceIncludesTax, showTax);
        }

		public string FormatShippingPrice(decimal price, bool showCurrency, string currencyCode, Language language, bool priceIncludesTax, bool showTax)
		{
			var currency = _currencyService.GetCurrencyByCode(currencyCode) ?? new Currency { CurrencyCode = currencyCode };
			return FormatPrice(price, showCurrency, currency, language, priceIncludesTax, showTax);
		}

        public string FormatShippingPrice(decimal price, bool showCurrency, Currency targetCurrency, Language language, bool priceIncludesTax, bool showTax)
        {
            return FormatPrice(price, showCurrency, targetCurrency, language, priceIncludesTax, showTax);
        }
        
        public string FormatShippingPrice(decimal price, bool showCurrency, string currencyCode, Language language, bool priceIncludesTax)
        {
			var currency = _currencyService.GetCurrencyByCode(currencyCode) ?? new Currency { CurrencyCode = currencyCode };
            return FormatShippingPrice(price, showCurrency, currency, language, priceIncludesTax);
        }



        public string FormatPaymentMethodAdditionalFee(decimal price, bool showCurrency)
        {
            var targetCurrency = _workContext.WorkingCurrency;
            var language = _workContext.WorkingLanguage;
            bool priceIncludesTax = false;
            switch (_workContext.TaxDisplayType)
            {
                case TaxDisplayType.ExcludingTax:
                    priceIncludesTax = false;
                    break;
                case TaxDisplayType.IncludingTax:
                    priceIncludesTax = true;
                    break;
            }
            return FormatPaymentMethodAdditionalFee(price, showCurrency, targetCurrency, 
                language, priceIncludesTax);
        }

        public string FormatPaymentMethodAdditionalFee(decimal price, bool showCurrency, Currency targetCurrency, Language language, bool priceIncludesTax)
        {
            bool showTax = _taxSettings.PaymentMethodAdditionalFeeIsTaxable && _taxSettings.DisplayTaxSuffix;
            return FormatPaymentMethodAdditionalFee(price, showCurrency, targetCurrency, language, priceIncludesTax, showTax);
        }

		public string FormatPaymentMethodAdditionalFee(decimal price, bool showCurrency, string currencyCode, Language language, bool priceIncludesTax, bool showTax)
		{
			var currency = _currencyService.GetCurrencyByCode(currencyCode) ?? new Currency { CurrencyCode = currencyCode };
			return FormatPrice(price, showCurrency, currency, language, priceIncludesTax, showTax);
		}

        public string FormatPaymentMethodAdditionalFee(decimal price, bool showCurrency, Currency targetCurrency, Language language, bool priceIncludesTax, bool showTax)
        {
            return FormatPrice(price, showCurrency, targetCurrency, language, priceIncludesTax, showTax);
        }

        public string FormatPaymentMethodAdditionalFee(decimal price, bool showCurrency, string currencyCode, Language language, bool priceIncludesTax)
        {
			var currency = _currencyService.GetCurrencyByCode(currencyCode) ?? new Currency { CurrencyCode = currencyCode };
            return FormatPaymentMethodAdditionalFee(price, showCurrency, currency, language, priceIncludesTax);
        }



        public string FormatTaxRate(decimal taxRate)
        {
            return taxRate.ToString("G29");
        }

        #endregion
    }
}
