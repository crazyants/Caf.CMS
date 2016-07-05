using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI.Localization;
using CAF.Infrastructure.Core;
using CAF.WebSite.Mvc.Admin.Validators.Plugins;


namespace CAF.WebSite.Mvc.Admin.Models.Plugins
{
    [Validator(typeof(PluginValidator))]
    public class PluginModel : ModelBase, ILocalizedModel<PluginLocalizedModel>
    {
        public PluginModel()
        {
            Locales = new List<PluginLocalizedModel>();
        }
        [LangResourceDisplayName("Admin.Configuration.Plugins.Fields.Group")]
        [AllowHtml]
        public string Group { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Plugins.Fields.FriendlyName")]
        [AllowHtml]
        public string FriendlyName { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Plugins.Fields.SystemName")]
        [AllowHtml]
        public string SystemName { get; set; }

        [LangResourceDisplayName("Common.Description")]
        [AllowHtml]
        public string Description { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Plugins.Fields.Version")]
        [AllowHtml]
        public string Version { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Plugins.Fields.Author")]
        [AllowHtml]
        public string Author { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Plugins.Fields.DisplayOrder")] 
        public int DisplayOrder { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Plugins.Fields.Configure")]
        public string ConfigurationUrl { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Plugins.Fields.Installed")]
        public bool Installed { get; set; }

		public bool IsConfigurable { get; set; }

		public RouteInfo ConfigurationRoute { get; set; }

        public string IconUrl { get; set; }

        public IList<PluginLocalizedModel> Locales { get; set; }

		public int[] SelectedSiteIds { get; set; }
    }


    public class PluginLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Plugins.Fields.FriendlyName")]
        [AllowHtml]
        public string FriendlyName { get; set; }

		[LangResourceDisplayName("Common.Description")]
		[AllowHtml]
		public string Description { get; set; }
    }
}