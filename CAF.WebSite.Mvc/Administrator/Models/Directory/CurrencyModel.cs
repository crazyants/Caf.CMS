using System;
using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using  CAF.WebSite.Mvc.Admin.Models.Sites;
using  CAF.WebSite.Mvc.Admin.Validators.Directory;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI.Localization;

namespace  CAF.WebSite.Mvc.Admin.Models.Directory
{
    [Validator(typeof(CurrencyValidator))]
    public class CurrencyModel : EntityModelBase, ILocalizedModel<CurrencyLocalizedModel>
    {
        public CurrencyModel()
        {
            Locales = new List<CurrencyLocalizedModel>();

			AvailableDomainEndings = new List<SelectListItem>()
			{
				new SelectListItem() { Text = ".com", Value = ".com" },
				new SelectListItem() { Text = ".uk", Value = ".uk" },
				new SelectListItem() { Text = ".de", Value = ".de" },
				new SelectListItem() { Text = ".ch", Value = ".ch" }
			};
        }
        [LangResourceDisplayName("Admin.Configuration.Currencies.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Currencies.Fields.CurrencyCode")]
        [AllowHtml]
        public string CurrencyCode { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Currencies.Fields.DisplayLocale")]
        [AllowHtml]
        public string DisplayLocale { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Currencies.Fields.Rate")]
        public decimal Rate { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Currencies.Fields.CustomFormatting")]
        [AllowHtml]
        public string CustomFormatting { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Currencies.Fields.Published")]
        public bool Published { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Currencies.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Currencies.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Currencies.Fields.IsPrimaryExchangeRateCurrency")]
        public bool IsPrimaryExchangeRateCurrency { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Currencies.Fields.IsPrimarySiteCurrency")]
        public bool IsPrimarySiteCurrency { get; set; }

		[LangResourceDisplayName("Admin.Configuration.Currencies.Fields.DomainEndings")]
		public string DomainEndings { get; set; }
		public IList<SelectListItem> AvailableDomainEndings { get; set; }

        public IList<CurrencyLocalizedModel> Locales { get; set; }

		//Site mapping
		[LangResourceDisplayName("Admin.Common.Site.LimitedTo")]
		public bool LimitedToSites { get; set; }
		[LangResourceDisplayName("Admin.Common.Site.AvailableFor")]
		public List<SiteModel> AvailableSites { get; set; }
		public int[] SelectedSiteIds { get; set; }
    }

    public class CurrencyLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Currencies.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }
    }
}