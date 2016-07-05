using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI;
using CAF.Infrastructure.Core.Domain.Seo;
using CAF.Infrastructure.Core.Domain.Localization;
using CAF.Infrastructure.Core.Domain.Common;
using CAF.WebSite.Mvc.Admin.Validators.Settings;
 

namespace CAF.WebSite.Mvc.Admin.Models.Settings
{
    [Validator(typeof(GeneralCommonSettingsValidator))]
	public partial class GeneralCommonSettingsModel : ModelBase
    {
        public GeneralCommonSettingsModel()
        {
            SiteInformationSettings = new SiteInformationSettingsModel();
            SeoSettings = new SeoSettingsModel();
            SecuritySettings = new SecuritySettingsModel();
			CaptchaSettings = new CaptchaSettingsModel();
            PdfSettings = new PdfSettingsModel();
            LocalizationSettings = new LocalizationSettingsModel(); 
            FullTextSettings = new FullTextSettingsModel();
            CompanyInformationSettings = new CompanyInformationSettingsModel();
            ContactDataSettings = new ContactDataSettingsModel();
            BankConnectionSettings = new BankConnectionSettingsModel();
            SocialSettings = new SocialSettingsModel();
        }

        public SiteInformationSettingsModel SiteInformationSettings { get; set; }
        public SeoSettingsModel SeoSettings { get; set; }
        public SecuritySettingsModel SecuritySettings { get; set; }
		public CaptchaSettingsModel CaptchaSettings { get; set; }
        public PdfSettingsModel PdfSettings { get; set; }
        public LocalizationSettingsModel LocalizationSettings { get; set; }
        public FullTextSettingsModel FullTextSettings { get; set; }
        public CompanyInformationSettingsModel CompanyInformationSettings { get; set; }
        public ContactDataSettingsModel ContactDataSettings { get; set; }
        public BankConnectionSettingsModel BankConnectionSettings { get; set; }
        public SocialSettingsModel SocialSettings { get; set; }

        #region Nested classes

		public partial class SiteInformationSettingsModel
        {
            public SiteInformationSettingsModel()
            {
            }
            
            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SiteClosed")]
			public bool SiteClosed { get; set; }
            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SiteContentShare","开启站点内容共享")]
            public bool SiteContentShare { get; set; }
            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SiteClosedAllowForAdmins")]
            public bool SiteClosedAllowForAdmins { get; set; }
        }

		public partial class SeoSettingsModel
        {
            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.PageTitleSeparator")]
            [AllowHtml]
            public string PageTitleSeparator { get; set; }
            
            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.PageTitleSeoAdjustment")]
            public PageTitleSeoAdjustment PageTitleSeoAdjustment { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DefaultTitle")]
            [AllowHtml]
            public string DefaultTitle { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DefaultMetaKeywords")]
            [AllowHtml]
            public string DefaultMetaKeywords { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DefaultMetaDescription")]
            [AllowHtml]
            public string DefaultMetaDescription { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.ConvertNonWesternChars")]
            public bool ConvertNonWesternChars { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CanonicalUrlsEnabled")]
            public bool CanonicalUrlsEnabled { get; set; }

			[LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CanonicalHostNameRule")]
			public CanonicalHostNameRule CanonicalHostNameRule { get; set; }
        }

		public partial class SecuritySettingsModel
        {
            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.EncryptionKey")]
            [AllowHtml]
            public string EncryptionKey { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.AdminAreaAllowedIpAddresses")]
            [AllowHtml]
            public string AdminAreaAllowedIpAddresses { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.HideAdminMenuItemsBasedOnPermissions")]
            public bool HideAdminMenuItemsBasedOnPermissions { get; set; }
        }

		public partial class CaptchaSettingsModel
		{
			[LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaEnabled")]
			public bool Enabled { get; set; }

			[LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnLoginPage")]
			public bool ShowOnLoginPage { get; set; }

			[LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnRegistrationPage")]
			public bool ShowOnRegistrationPage { get; set; }

			[LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnContactUsPage")]
			public bool ShowOnContactUsPage { get; set; }

			[LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnEmailWishlistToFriendPage")]
			public bool ShowOnEmailWishlistToFriendPage { get; set; }

			[LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnEmailProductToFriendPage")]
			public bool ShowOnEmailProductToFriendPage { get; set; }

			[LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnAskQuestionPage")]
			public bool ShowOnAskQuestionPage { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.ShowOnArticleCommentPage")]
            public bool ShowOnArticleCommentPage { get; set; }

			[LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.reCaptchaPublicKey")]
			[AllowHtml]
			public string ReCaptchaPublicKey { get; set; }

			[LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.reCaptchaPrivateKey")]
			[AllowHtml]
			public string ReCaptchaPrivateKey { get; set; }
		}

		public partial class PdfSettingsModel
        {
            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.PdfEnabled")]
            public bool Enabled { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.PdfLetterPageSizeEnabled")]
            public bool LetterPageSizeEnabled { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.PdfLogo")]
            [UIHint("Picture")]
            public int LogoPictureId { get; set; }
        }

		public partial class LocalizationSettingsModel
        {
            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.UseImagesForLanguageSelection")]
            public bool UseImagesForLanguageSelection { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SeoFriendlyUrlsForLanguagesEnabled")]
            public bool SeoFriendlyUrlsForLanguagesEnabled { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.LoadAllLocaleRecordsOnStartup")]
            public bool LoadAllLocaleRecordsOnStartup { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DefaultLanguageRedirectBehaviour")]
            public DefaultLanguageRedirectBehaviour DefaultLanguageRedirectBehaviour { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.InvalidLanguageRedirectBehaviour")]
            public InvalidLanguageRedirectBehaviour InvalidLanguageRedirectBehaviour { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DetectBrowserUserLanguage")]
            public bool DetectBrowserUserLanguage { get; set; }
        }

		public partial class FullTextSettingsModel
        {
            public bool Supported { get; set; }

            public bool Enabled { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.FullTextSettings.SearchMode")]
            public FulltextSearchMode SearchMode { get; set; }
            public SelectList SearchModeValues { get; set; }
        }

		public partial class CompanyInformationSettingsModel
        {

            public CompanyInformationSettingsModel()
            {
                AvailableCountries = new List<SelectListItem>();
                Salutations = new List<SelectListItem>();
                ManagementDescriptions = new List<SelectListItem>();
            }

            public IList<SelectListItem> AvailableCountries { get; set; }
            public IList<SelectListItem> Salutations { get; set; }
            public IList<SelectListItem> ManagementDescriptions { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.CompanyName")]
            public string CompanyName { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.Salutation")]
            public string Salutation { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.Title")]
            public string Title { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.Firstname")]
            public string Firstname { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.Lastname")]
            public string Lastname { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.CompanyManagementDescription")]
            public string CompanyManagementDescription { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.CompanyManagement")]
            public string CompanyManagement { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.Street")]
            public string Street { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.Street2")]
            public string Street2 { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.ZipCode")]
            public string ZipCode { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.Location")]
            public string City { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.Country")]
            public int CountryId { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.Country")]
            [AllowHtml]
            public string CountryName { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.State")]
            public string Region { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.VatId")]
            public string VatId { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.CommercialRegister")]
            public string CommercialRegister { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.TaxNumber")]
            public string TaxNumber { get; set; }
        }

		public partial class ContactDataSettingsModel
        {
            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.ContactDataSettings.CompanyTelephoneNumber")]
            public string CompanyTelephoneNumber { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.ContactDataSettings.HotlineTelephoneNumber")]
            public string HotlineTelephoneNumber { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.ContactDataSettings.MobileTelephoneNumber")]
            public string MobileTelephoneNumber { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.ContactDataSettings.CompanyFaxNumber")]
            public string CompanyFaxNumber { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.ContactDataSettings.CompanyEmailAddress")]
            public string CompanyEmailAddress { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.ContactDataSettings.WebmasterEmailAddress")]
            public string WebmasterEmailAddress { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.ContactDataSettings.SupportEmailAddress")]
            public string SupportEmailAddress { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.ContactDataSettings.ContactEmailAddress")]
            public string ContactEmailAddress { get; set; }
        }

		public partial class BankConnectionSettingsModel
        {
            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.BankConnectionSettings.Bankname")]
            public string Bankname { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.BankConnectionSettings.Bankcode")]
            public string Bankcode { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.BankConnectionSettings.AccountNumber")]
            public string AccountNumber { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.BankConnectionSettings.AccountHolder")]
            public string AccountHolder { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.BankConnectionSettings.Iban")]
            public string Iban { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.BankConnectionSettings.Bic")]
            public string Bic { get; set; }
        }

		public partial class SocialSettingsModel
        {
            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SocialSettings.ShowSocialLinksInFooter")]
            public bool ShowSocialLinksInFooter { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SocialSettings.FacebookLink")]
            public string FacebookLink { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SocialSettings.GooglePlusLink")]
            public string GooglePlusLink { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SocialSettings.TwitterLink")]
            public string TwitterLink { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SocialSettings.PinterestLink")]
            public string PinterestLink { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SocialSettings.YoutubeLink")]
            public string YoutubeLink { get; set; }
        }

        #endregion
    }
}