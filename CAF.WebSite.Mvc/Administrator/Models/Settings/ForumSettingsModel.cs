
using System;
﻿using System.Collections.Generic;
﻿using System.ComponentModel.DataAnnotations;
using System.Web.DynamicData;
using System.Web.Mvc;
using FluentValidation.Attributes;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Mvc.Admin.Models.Sites;
using CAF.WebSite.Mvc.Admin.Validators.ContentSlider;
using CAF.Infrastructure.Core.Domain.Cms.Forums;



namespace CAF.WebSite.Mvc.Admin.Models.Settings
{
	public class ForumSettingsModel
    {
        [LangResourceDisplayName("Admin.Configuration.Settings.Forums.ForumsEnabled")]
        public bool ForumsEnabled { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Forums.RelativeDateTimeFormattingEnabled")]
        public bool RelativeDateTimeFormattingEnabled { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Forums.ShowUsersPostCount")]
        public bool ShowUsersPostCount { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Forums.AllowGuestsToCreatePosts")]
        public bool AllowGuestsToCreatePosts { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Forums.AllowGuestsToCreateTopics")]
        public bool AllowGuestsToCreateTopics { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Forums.AllowUsersToEditPosts")]
        public bool AllowUsersToEditPosts { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Forums.AllowUsersToDeletePosts")]
        public bool AllowUsersToDeletePosts { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Forums.AllowUsersToManageSubscriptions")]
        public bool AllowUsersToManageSubscriptions { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Forums.TopicsPageSize")]
        public int TopicsPageSize { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Forums.PostsPageSize")]
        public int PostsPageSize { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Forums.ForumEditor")]
        public EditorType ForumEditor { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Forums.SignaturesEnabled")]
        public bool SignaturesEnabled { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Forums.AllowPrivateMessages")]
        public bool AllowPrivateMessages { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Forums.ShowAlertForPM")]
        public bool ShowAlertForPM { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Forums.NotifyAboutPrivateMessages")]
        public bool NotifyAboutPrivateMessages { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Forums.ActiveDiscussionsFeedEnabled")]
        public bool ActiveDiscussionsFeedEnabled { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Forums.ActiveDiscussionsFeedCount")]
        public int ActiveDiscussionsFeedCount { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Forums.ForumFeedsEnabled")]
        public bool ForumFeedsEnabled { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Forums.ForumFeedCount")]
        public int ForumFeedCount { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Forums.SearchResultsPageSize")]
        public int SearchResultsPageSize { get; set; }

        public SelectList ForumEditorValues { get; set; }
    }
}