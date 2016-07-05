using System;
﻿using System.Collections.Generic;
﻿using System.ComponentModel.DataAnnotations;
using System.Web.DynamicData;
using System.Web.Mvc;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Mvc.Admin.Models.Sites;
using CAF.WebSite.Mvc.Admin.Validators.ContentSlider;
using CAF.Infrastructure.Core.Domain.Cms.Articles;



namespace CAF.WebSite.Mvc.Admin.Models.Settings
{
    public class ArticleCatalogSettingsModel
    {
        public ArticleCatalogSettingsModel()
        {
            this.AvailableDefaultViewModes = new List<SelectListItem>();
           
        }

        #region General



        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.IgnoreFeaturedArticles")]
        public bool IgnoreFeaturedArticles { get; set; }


       

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowBestsellersOnHomepage")]
        public bool ShowBestsellersOnHomepage { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.NumberOfBestsellersOnHomepage")]
        public int NumberOfBestsellersOnHomepage { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.EnableHtmlTextCollapser")]
        public bool EnableHtmlTextCollapser { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.HtmlTextCollapsedHeight")]
        public int HtmlTextCollapsedHeight { get; set; }

     
        #endregion

        #region Article lists

        #region Navigation

        //filter
        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowArticlesFromSubcategories")]
        public bool ShowArticlesFromSubcategories { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.IncludeFeaturedArticlesInNormalLists")]
        public bool IncludeFeaturedArticlesInNormalLists { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowCategoryArticleNumber")]
        public bool ShowCategoryArticleNumber { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowCategoryArticleNumberIncludingSubcategories")]
        public bool ShowCategoryArticleNumberIncludingSubcategories { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.CategoryBreadcrumbEnabled")]
        public bool CategoryBreadcrumbEnabled { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.FilterEnabled")]
        public bool FilterEnabled { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.MaxFilterItemsToDisplay")]
        public int MaxFilterItemsToDisplay { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.ExpandAllFilterCriteria")]
        public bool ExpandAllFilterCriteria { get; set; }
        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowSubcategoriesAboveArticleLists")]
        public bool ShowSubcategoriesAboveArticleLists { get; set; }


        #endregion

        #region Article list

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.AllowArticleSorting")]
        public bool AllowArticleSorting { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.DefaultViewMode")]
        public string DefaultViewMode { get; set; }
        public IList<SelectListItem> AvailableDefaultViewModes { get; private set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.AllowArticleViewModeChanging")]
        public bool AllowArticleViewModeChanging { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.DefaultPageSizeOptions")]
        public string DefaultPageSizeOptions { get; set; }

        #endregion

        #region Articles

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowDeliveryTimesInArticleLists")]
        public bool ShowDeliveryTimesInArticleLists { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowBasePriceInArticleLists")]
        public bool ShowBasePriceInArticleLists { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowColorSquaresInLists")]
        public bool ShowColorSquaresInLists { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.HideBuyButtonInLists")]
        public bool HideBuyButtonInLists { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.LabelAsNewForMaxDays")]
        public int? LabelAsNewForMaxDays { get; set; }

        #endregion

        #region Article tags

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.NumberOfArticleTags")]
        public int NumberOfArticleTags { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.ArticlesByTagPageSize")]
        public int ArticlesByTagPageSize { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.ArticlesByTagAllowUsersToSelectPageSize")]
        public bool ArticlesByTagAllowUsersToSelectPageSize { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.ArticlesByTagPageSizeOptions")]
        public string ArticlesByTagPageSizeOptions { get; set; }

        #endregion

        #endregion

        #region Users

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowArticleReviewsInArticleLists")]
        public bool ShowArticleReviewsInArticleLists { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowArticleReviewsInArticleDetail")]
        public bool ShowArticleReviewsInArticleDetail { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.ArticleReviewsMustBeApproved")]
        public bool ArticleReviewsMustBeApproved { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.AllowAnonymousUsersToReviewArticle")]
        public bool AllowAnonymousUsersToReviewArticle { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.NotifySiteOwnerAboutNewArticleReviews")]
        public bool NotifySiteOwnerAboutNewArticleReviews { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.EmailAFriendEnabled")]
        public bool EmailAFriendEnabled { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.AskQuestionEnabled")]
        public bool AskQuestionEnabled { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.AllowAnonymousUsersToEmailAFriend")]
        public bool AllowAnonymousUsersToEmailAFriend { get; set; }

        #endregion

        #region Article detail

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.RecentlyViewedArticlesEnabled")]
        public bool RecentlyViewedArticlesEnabled { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.RecentlyViewedArticlesNumber")]
        public int RecentlyViewedArticlesNumber { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.RecentlyAddedArticlesEnabled")]
        public bool RecentlyAddedArticlesEnabled { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.RecentlyAddedArticlesNumber")]
        public int RecentlyAddedArticlesNumber { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.RecentlyAddedArticlesEnabled")]
        public bool RecentlyViewedSolutionsEnabled { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.RecentlyAddedArticlesNumber")]
        public int RecentlyViewedSolutionsNumber { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowShareButton")]
        public bool ShowShareButton { get; set; }


        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.DisplayAllImagesNumber")]
        public int DisplayAllImagesNumber { get; set; }


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
        #endregion

        #region Search

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.SearchPageArticlesPerPage")]
        public int SearchPageArticlesPerPage { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.ArticleSearchAllowUsersToSelectPageSize")]
        public bool ArticleSearchAllowUsersToSelectPageSize { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.ArticleSearchPageSizeOptions")]
        public string ArticleSearchPageSizeOptions { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.ArticleSearchAutoCompleteEnabled")]
        public bool ArticleSearchAutoCompleteEnabled { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowArticleImagesInSearchAutoComplete")]
        public bool ShowArticleImagesInSearchAutoComplete { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.ArticleSearchAutoCompleteNumberOfArticles")]
        public int ArticleSearchAutoCompleteNumberOfArticles { get; set; }
 

        #endregion

    }
}