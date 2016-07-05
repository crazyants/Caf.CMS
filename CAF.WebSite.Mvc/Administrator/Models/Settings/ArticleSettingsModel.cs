using CAF.WebSite.Application.WebUI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.DynamicData;
using System.Web.Mvc;


namespace CAF.WebSite.Mvc.Admin.Models.Settings
{
	public class ArticleSettingsModel
    {
        [LangResourceDisplayName("Admin.Configuration.Settings.Article.Enabled")]
        public bool Enabled { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Article.PostsPageSize")]
        public int PostsPageSize { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Article.AllowNotRegisteredUsersToLeaveComments")]
        public bool AllowNotRegisteredUsersToLeaveComments { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Article.NotifyAboutNewArticleComments")]
        public bool NotifyAboutNewArticleComments { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Article.NumberOfTags")]
        public int NumberOfTags { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Article.ShowHeaderRSSUrl")]
        public bool ShowHeaderRssUrl { get; set; }
    }
}