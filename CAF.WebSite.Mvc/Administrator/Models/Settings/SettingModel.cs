using System.Web.Mvc;
using FluentValidation.Attributes;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Mvc.Admin.Validators.Settings;
 
namespace CAF.WebSite.Mvc.Admin.Models.Settings
{ 
    [Validator(typeof(SettingValidator))]
    public class SettingModel : EntityModelBase
    {
        [LangResourceDisplayName("Admin.Configuration.Settings.AllSettings.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.AllSettings.Fields.Value")]
        [AllowHtml]
        public string Value { get; set; }

		[LangResourceDisplayName("Admin.Configuration.Settings.AllSettings.Fields.StoreName")]
		public string Store { get; set; }
		public int StoreId { get; set; }
    }
}