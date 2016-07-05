using CAF.WebSite.Application.WebUI;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CAF.WebSite.Mvc.Admin.Models.Settings
{
    public partial class GeneralUserSettingsModel
    {
        public GeneralUserSettingsModel()
        {
            UserSettings = new UserSettingsModel();
            AddressSettings = new AddressSettingsModel();
            DateTimeSettings = new DateTimeSettingsModel();
            ExternalAuthenticationSettings = new ExternalAuthenticationSettingsModel();
        }
        public UserSettingsModel UserSettings { get; set; }
        public AddressSettingsModel AddressSettings { get; set; }
        public DateTimeSettingsModel DateTimeSettings { get; set; }
        public ExternalAuthenticationSettingsModel ExternalAuthenticationSettings { get; set; }

        #region Nested classes

        public partial class UserSettingsModel
        {
            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.UserNamesEnabled")]
            public bool UserNamesEnabled { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.AllowUsersToChangeUserNames")]
            public bool AllowUsersToChangeUserNames { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.CheckUserNameAvailabilityEnabled")]
            public bool CheckUserNameAvailabilityEnabled { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.UserRegistrationType")]
            public int UserRegistrationType { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.AllowUsersToUploadAvatars")]
            public bool AllowUsersToUploadAvatars { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.DefaultAvatarEnabled")]
            public bool DefaultAvatarEnabled { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.ShowUsersLocation")]
            public bool ShowUsersLocation { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.ShowUsersJoinDate")]
            public bool ShowUsersJoinDate { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.AllowViewingProfiles")]
            public bool AllowViewingProfiles { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.NotifyNewUserRegistration")]
            public bool NotifyNewUserRegistration { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.HideDownloadableArticlesTab")]
            public bool HideDownloadableArticlesTab { get; set; }


            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.UserNameFormat")]
            public int UserNameFormat { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.UserNameFormatMaxLength")]
            public int UserNameFormatMaxLength { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.DefaultPasswordFormat")]
            public int DefaultPasswordFormat { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.NewsletterEnabled")]
            public bool NewsletterEnabled { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.HideNewsletterBlock")]
            public bool HideNewsletterBlock { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.SiteLastVisitedPage")]
            public bool SiteLastVisitedPage { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.GenderEnabled")]
            public bool GenderEnabled { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.DateOfBirthEnabled")]
            public bool DateOfBirthEnabled { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.CompanyEnabled")]
            public bool CompanyEnabled { get; set; }
            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.CompanyRequired")]
            public bool CompanyRequired { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.StreetAddressEnabled")]
            public bool StreetAddressEnabled { get; set; }
            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.StreetAddressRequired")]
            public bool StreetAddressRequired { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.StreetAddress2Enabled")]
            public bool StreetAddress2Enabled { get; set; }
            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.StreetAddress2Required")]
            public bool StreetAddress2Required { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.ZipPostalCodeEnabled")]
            public bool ZipPostalCodeEnabled { get; set; }
            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.ZipPostalCodeRequired")]
            public bool ZipPostalCodeRequired { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.CityEnabled")]
            public bool CityEnabled { get; set; }
            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.CityRequired")]
            public bool CityRequired { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.CountryEnabled")]
            public bool CountryEnabled { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.StateProvinceEnabled")]
            public bool StateProvinceEnabled { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.PhoneEnabled")]
            public bool PhoneEnabled { get; set; }
            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.PhoneRequired")]
            public bool PhoneRequired { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.FaxEnabled")]
            public bool FaxEnabled { get; set; }
            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.FaxRequired")]
            public bool FaxRequired { get; set; }
        }

        public partial class AddressSettingsModel
        {
            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.AddressFormFields.CompanyEnabled")]
            public bool CompanyEnabled { get; set; }
            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.AddressFormFields.CompanyRequired")]
            public bool CompanyRequired { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.AddressFormFields.StreetAddressEnabled")]
            public bool StreetAddressEnabled { get; set; }
            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.AddressFormFields.StreetAddressRequired")]
            public bool StreetAddressRequired { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.AddressFormFields.StreetAddress2Enabled")]
            public bool StreetAddress2Enabled { get; set; }
            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.AddressFormFields.StreetAddress2Required")]
            public bool StreetAddress2Required { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.AddressFormFields.ZipPostalCodeEnabled")]
            public bool ZipPostalCodeEnabled { get; set; }
            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.AddressFormFields.ZipPostalCodeRequired")]
            public bool ZipPostalCodeRequired { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.AddressFormFields.CityEnabled")]
            public bool CityEnabled { get; set; }
            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.AddressFormFields.CityRequired")]
            public bool CityRequired { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.AddressFormFields.CountryEnabled")]
            public bool CountryEnabled { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.AddressFormFields.StateProvinceEnabled")]
            public bool StateProvinceEnabled { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.AddressFormFields.PhoneEnabled")]
            public bool PhoneEnabled { get; set; }
            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.AddressFormFields.PhoneRequired")]
            public bool PhoneRequired { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.AddressFormFields.FaxEnabled")]
            public bool FaxEnabled { get; set; }
            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.AddressFormFields.FaxRequired")]
            public bool FaxRequired { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.ValidateEmailAddress")]
            public bool ValidateEmailAddress { get; set; }
        }

        public partial class DateTimeSettingsModel
        {
            public DateTimeSettingsModel()
            {
                AvailableTimeZones = new List<SelectListItem>();
            }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.AllowUsersToSetTimeZone")]
            public bool AllowUsersToSetTimeZone { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.DefaultSiteTimeZone")]
            public string DefaultSiteTimeZoneId { get; set; }

            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.DefaultSiteTimeZone")]
            public IList<SelectListItem> AvailableTimeZones { get; set; }
        }

        public partial class ExternalAuthenticationSettingsModel
        {
            [LangResourceDisplayName("Admin.Configuration.Settings.GeneralUser.ExternalAuthenticationAutoRegisterEnabled")]
            public bool AutoRegisterEnabled { get; set; }
        }
        #endregion
    }
}