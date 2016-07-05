using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using CAF.WebSite.Application.WebUI.Localization;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Mvc.Admin.Models.Sites;
using CAF.WebSite.Mvc.Admin.Validators.Users;

namespace CAF.WebSite.Mvc.Admin.Models.Links
{
    [Validator(typeof(LinkValidator))]
    public class LinkModel : EntityModelBase
    {
     
      
        public LinkModel()
		{
			this.AvailableSites = new List<SiteModel>();
		}

        [LangResourceDisplayName("Admin.ContentManagement.Links.Fields.Language")]
        public int LanguageId { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Links.Fields.Language")]
        [AllowHtml]
        public string LanguageName { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Links.Fields.Name")]
        public string Name { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.Links.Fields.Intro")]
        public string Intro { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.Links.Fields.LinkUrl")]
        public string LinkUrl { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.Links.Fields.LogoUrl")]
        public string LogoUrl { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.Links.Fields.SortId")]
        public string SortId { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.Links.Fields.IsHome")]
        public bool IsHome { get; set; }
        [UIHint("Picture")]
        [LangResourceDisplayName("Admin.ContentManagement.Links.Fields.Picture")]
        public int PictureId { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.Links.Fields.Picture")]
        public string PictureUrl { get; set; }

        [LangResourceDisplayName("Admin.Common.Site.LimitedTo")]
        public bool LimitedToSites { get; set; }
        [LangResourceDisplayName("Admin.Common.Site.AvailableFor")]
        public List<SiteModel> AvailableSites { get; set; }
        public int[] SelectedSiteIds { get; set; }
    }

   
}