using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using CAF.WebSite.Application.WebUI.Localization;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Mvc.Admin.Models.Sites;
using CAF.WebSite.Mvc.Admin.Validators.Users;

namespace CAF.WebSite.Mvc.Admin.Models.RegionalContents
{
    [Validator(typeof(RegionalContentValidator))]
    public class RegionalContentModel : EntityModelBase
    {
        public RegionalContentModel()
		{
			this.AvailableSites = new List<SiteModel>();
		}

        [LangResourceDisplayName("Admin.ContentManagement.RegionalContents.Fields.Language", "语言")]
        public int LanguageId { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.RegionalContents.Fields.Language", "语言")]
        [AllowHtml]
        public string LanguageName { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.RegionalContents.Fields.SystemName","调用名称")]
        public string SystemName { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.RegionalContents.Fields.Name", "名称")]
        public string Name { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.RegionalContents.Fields.Body", "HTML内容")]
        [AllowHtml]
        public string Body { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.RegionalContents.Fields.DisplayOrder", "排序")]
        public int DisplayOrder { get; set; }
        [LangResourceDisplayName("Admin.Common.Site.LimitedTo", "站点授权")]
        public bool LimitedToSites { get; set; }
        [LangResourceDisplayName("Admin.Common.Site.AvailableFor", "站点授权")]
        public List<SiteModel> AvailableSites { get; set; }
        public int[] SelectedSiteIds { get; set; }
    }

   
}