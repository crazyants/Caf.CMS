using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using CAF.WebSite.Mvc.Admin.Validators.Localization;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Mvc.Admin.Models.Sites;
 

namespace CAF.WebSite.Mvc.Admin.Models.Localization
{
    [Validator(typeof(LanguageValidator))]
    public class LanguageModel : EntityModelBase
    {
        public LanguageModel()
        {
            FlagFileNames = new List<string>();
        }

        [LangResourceDisplayName("Admin.Configuration.Languages.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Languages.Fields.LanguageCulture")]
        [AllowHtml]
        public string LanguageCulture { get; set; }
        public List<SelectListItem> AvailableCultures { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Languages.Fields.UniqueSeoCode")]
        [AllowHtml]
        public string UniqueSeoCode { get; set; }
        public List<SelectListItem> AvailableTwoLetterLanguageCodes { get; set; }
        //flags
        [LangResourceDisplayName("Admin.Configuration.Languages.Fields.FlagImageFileName")]
        [AllowHtml]
        public string FlagImageFileName { get; set; }
        public IList<string> FlagFileNames { get; set; }
        public List<SelectListItem> AvailableFlags { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Languages.Fields.Rtl")]
        public bool Rtl { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Languages.Fields.Published")]
        public bool Published { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Languages.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

		//Store mapping
        [LangResourceDisplayName("Admin.Common.Site.LimitedTo")]
        public bool LimitedToSites { get; set; }
        [LangResourceDisplayName("Admin.Common.Site.AvailableFor")]
        public List<SiteModel> AvailableSites { get; set; }
        public int[] SelectedSiteIds { get; set; }
    }
}