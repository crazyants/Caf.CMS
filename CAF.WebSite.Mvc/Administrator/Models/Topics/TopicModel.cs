using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using CAF.WebSite.Application.WebUI.Localization;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Mvc.Admin.Models.Sites;
using CAF.WebSite.Mvc.Admin.Validators.Users;
using CAF.WebSite.Mvc.Admin.Validators.Topics;

namespace CAF.WebSite.Mvc.Admin.Models.Topics
{
    [Validator(typeof(TopicValidator))]
    public class TopicModel : TabbableModel, ILocalizedModel<TopicLocalizedModel>
    {
        #region widget zone names
        private readonly static string[] s_widgetZones = new string[] { 
            "main_column_before", 
            "main_column_after", 
            "left_side_column_before", 
            "left_side_column_before", 
            "right_side_column_before", 
            "right_side_column_before", 
            "notifications", 
            "body_start_html_tag_after",
            "content_before", 
            "content_after", 
            "body_end_html_tag_before"
        };
        #endregion
        
        public TopicModel()
        {
            Locales = new List<TopicLocalizedModel>();
			AvailableSites = new List<SiteModel>();
            AvailableTopicTemplates = new List<SelectListItem>();
            AvailableWidgetZones = s_widgetZones;

        }

        //Site mapping
		[LangResourceDisplayName("Admin.Common.Site.LimitedTo")]
		public bool LimitedToSites { get; set; }
		[LangResourceDisplayName("Admin.Common.Site.AvailableFor")]
		public List<SiteModel> AvailableSites { get; set; }
		public int[] SelectedSiteIds { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Topics.Fields.SystemName")]
        [AllowHtml]
        public string SystemName { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Topics.Fields.IncludeInSitemap")]
        public bool IncludeInSitemap { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Topics.Fields.IsPasswordProtected")]
        public bool IsPasswordProtected { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Topics.Fields.Password")]
		[DataType(DataType.Password)]
        public string Password { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Topics.Fields.URL")]
        [AllowHtml]
        public string Url { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Topics.Fields.Title")]
        [AllowHtml]
        public string Title { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Topics.Fields.Body")]
        [AllowHtml]
        public string Body { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Topics.Fields.MetaKeywords")]
        [AllowHtml]
        public string MetaKeywords { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Topics.Fields.MetaDescription")]
        [AllowHtml]
        public string MetaDescription { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Topics.Fields.MetaTitle")]
        [AllowHtml]
        public string MetaTitle { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Topics.Fields.RenderAsWidget")]
        public bool RenderAsWidget { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Topics.Fields.WidgetZone")]
        [UIHint("WidgetZone")]
        public string WidgetZone { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Topics.Fields.WidgetShowTitle")]
        public bool WidgetShowTitle { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Topics.Fields.WidgetBordered")]
        public bool WidgetBordered { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Topics.Fields.Priority")]
        public int Priority { get; set; }

        public string[] AvailableWidgetZones { get; private set; }

        public IList<TopicLocalizedModel> Locales { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Topics.Fields.TopicTemplateId")]
        [AllowHtml]
        public int TopicTemplateId { get; set; }
        public IList<SelectListItem> AvailableTopicTemplates { get; set; }
    }

    public class TopicLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Topics.Fields.Title")]
        [AllowHtml]
        public string Title { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Topics.Fields.Body")]
        [AllowHtml]
        public string Body { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Topics.Fields.MetaKeywords")]
        [AllowHtml]
        public string MetaKeywords { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Topics.Fields.MetaDescription")]
        [AllowHtml]
        public string MetaDescription { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Topics.Fields.MetaTitle")]
        [AllowHtml]
        public string MetaTitle { get; set; }
    }
}