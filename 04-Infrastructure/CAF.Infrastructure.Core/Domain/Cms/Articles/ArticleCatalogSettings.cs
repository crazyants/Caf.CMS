using CAF.Infrastructure.Core.Configuration;
using System.Collections.Generic;


namespace CAF.Infrastructure.Core.Domain.Cms.Articles
{
   
    public class ArticleCatalogSettings : ISettings
    {
        public ArticleCatalogSettings()
        {
            FileUploadAllowedExtensions = new List<string>();
			AllowArticleSorting = true;
			AllowArticleViewModeChanging = true;
			DefaultViewMode = "grid";
			CategoryBreadcrumbEnabled = true;
			ShowShareButton = true;
			PageShareCode = "<!-- AddThis Button BEGIN --><div class=\"addthis_toolbox addthis_default_style \"><a class=\"addthis_button_preferred_1\"></a><a class=\"addthis_button_preferred_2\"></a><a class=\"addthis_button_preferred_3\"></a><a class=\"addthis_button_preferred_4\"></a><a class=\"addthis_button_compact\"></a><a class=\"addthis_counter addthis_bubble_style\"></a></div><script type=\"text/javascript\">var addthis_config = {\"data_track_addressbar\":false};</script><script type=\"text/javascript\" src=\"//s7.addthis.com/js/300/addthis_widget.js#pubid=ra-50f6c18f03ecbb2f\"></script><!-- AddThis Button END -->";
			DefaultArticleRatingValue = 5;
			NotifySiteOwnerAboutNewArticleReviews = true;
			EmailAFriendEnabled = true;
			AskQuestionEnabled = true;
			RecentlyViewedArticlesNumber = 6;
			RecentlyViewedArticlesEnabled = true;
            RecentlyViewedSolutionsNumber = 6;
            RecentlyViewedSolutionsEnabled = true;
			RecentlyAddedArticlesNumber = 10;
			RecentlyAddedArticlesEnabled = true;
            RelativeDateTimeFormattingEnabled = true;
            FilterEnabled = true;
            MaxFilterItemsToDisplay = 4;

            ShowSubcategoriesAboveArticleLists = true;
			ArticleSearchAutoCompleteEnabled = true;
			ShowArticleImagesInSearchAutoComplete = true;
			ArticleSearchAutoCompleteNumberOfArticles = 10;
			ArticleSearchTermMinimumLength = 3;
			NumberOfBestsellersOnHomepage = 6;
			SearchPageArticlesPerPage = 6;
			NumberOfArticleTags = 15;
			ArticlesByTagPageSize = 12;
			UseSmallArticleBoxOnHomePage = true;
			DisplayTierPricesWithDiscounts = true;
			DefaultPageSizeOptions = "12, 18, 36, 72, 150";
			ArticlesByTagAllowUsersToSelectPageSize = true;
			ArticlesByTagPageSizeOptions = "12, 18, 36, 72, 150";
			FileUploadMaximumSizeBytes = 1024 * 200; //200KB
			ClientsBlockItemsToDisplay = 5;
			DisplayAllImagesNumber = 6;
			ShowColorSquaresInLists = true;
            ShowArticleReviewsInArticleDetail = true;
			HtmlTextCollapsedHeight = 260;
			MostRecentlyUsedCategoriesMaxSize = 6;
			MostRecentlyUsedClientsMaxSize = 4;

            ArticlePageSize = 10;
            AllowNotRegisteredUsersToLeaveComments = true;
            NumberOfTags = 15;
        }

  

        /// <summary>
        /// Gets or sets a value indicating whether to display manufacturer part number of a product
        /// </summary>
        public bool ShowClientPartNumber { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether to display the delivery time of a product
        /// </summary>
        public bool ShowDeliveryTimesInArticleLists { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether to display the base price of a product
        /// </summary>
        public bool ShowBasePriceInArticleLists { get; set; }
 

        /// <summary>
        /// Gets or sets a value indicating whether product sorting is enabled
        /// </summary>
        public bool AllowArticleSorting { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether users are allowed to change product view mode
        /// </summary>
        public bool AllowArticleViewModeChanging { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether users are allowed to change product view mode
        /// </summary>
        public string DefaultViewMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a category details page should include products from subcategories
        /// </summary>
        public bool ShowArticlesFromSubcategories { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether number of products should be displayed beside each category
        /// </summary>
        public bool ShowCategoryArticleNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we include subcategories (when 'ShowCategoryArticleNumber' is 'true')
        /// </summary>
        public bool ShowCategoryArticleNumberIncludingSubcategories { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether category breadcrumb is enabled
        /// </summary>
        public bool CategoryBreadcrumbEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether filter is enabled
        /// </summary>
        public bool FilterEnabled { get; set; }
        
        /// <summary>
        /// Gets or sets a value which determines the maximum number of displayed filter items
        /// </summary>
        public int MaxFilterItemsToDisplay { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether all filter criterias should be expanded
        /// </summary>
        public bool ExpandAllFilterCriteria { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether sub categories above product lists are enabled
        /// </summary>
        public bool ShowSubcategoriesAboveArticleLists { get; set; }

        
        /// <summary>
        /// Gets or sets a value indicating whether a 'Share button' is enabled
        /// </summary>
        public bool ShowShareButton { get; set; }

        /// <summary>
        /// Gets or sets a share code (e.g. AddThis button code)
        /// </summary>
        public string PageShareCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display reviews in product lists
        /// </summary>
        public bool ShowArticleReviewsInArticleLists { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display reviews in product detail
        /// </summary>
        public bool ShowArticleReviewsInArticleDetail { get; set; }

        /// <summary>
        /// Gets or sets a value indicating product reviews must be approved
        /// </summary>
        public bool ArticleReviewsMustBeApproved { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the default rating value of the product reviews
        /// </summary>
        public int DefaultArticleRatingValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to allow anonymous users write product reviews.
        /// </summary>
        public bool AllowAnonymousUsersToReviewArticle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether notification of a store owner about new product reviews is enabled
        /// </summary>
        public bool NotifySiteOwnerAboutNewArticleReviews { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether product 'Email a friend' feature is enabled
        /// </summary>
        public bool EmailAFriendEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'ask product question' feature is enabled
        /// </summary>
        public bool AskQuestionEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to allow anonymous users to email a friend.
        /// </summary>
        public bool AllowAnonymousUsersToEmailAFriend { get; set; }

        /// <summary>
        /// Gets or sets a number of "Recently viewed products"
        /// </summary>
        public int RecentlyViewedArticlesNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether "Recently viewed Solutions" feature is enabled
        /// </summary>
        public bool RecentlyViewedArticlesEnabled { get; set; }
        /// <summary>
        /// Gets or sets a number of "Recently viewed products"
        /// </summary>
        public int RecentlyViewedSolutionsNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether "Recently viewed Solutions" feature is enabled
        /// </summary>
        public bool RecentlyViewedSolutionsEnabled { get; set; }

        /// <summary>
        /// Gets or sets a number of "Recently added products"
        /// </summary>
        public int RecentlyAddedArticlesNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether "Recently added products" feature is enabled
        /// </summary>
        public bool RecentlyAddedArticlesEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether relative date and time formatting is enabled (e.g. 2 hours ago, a month ago)
        /// </summary>
        public bool RelativeDateTimeFormattingEnabled { get; set; } 


        /// <summary>
        /// Gets or sets a value indicating whether autocomplete is enabled
        /// </summary>
        public bool ArticleSearchAutoCompleteEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show product images in the auto complete search
        /// </summary>
        public bool ShowArticleImagesInSearchAutoComplete { get; set; }

        /// <summary>
        /// Gets or sets a number of products to return when using "autocomplete" feature
        /// </summary>
        public int ArticleSearchAutoCompleteNumberOfArticles { get; set; }

        /// <summary>
        /// Gets or sets a minimum search term length
        /// </summary>
        public int ArticleSearchTermMinimumLength { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show bestsellers on home page
        /// </summary>
        public bool ShowBestsellersOnHomepage { get; set; }

        /// <summary>
        /// Gets or sets a number of bestsellers on home page
        /// </summary>
        public int NumberOfBestsellersOnHomepage { get; set; }

        /// <summary>
        /// Gets or sets a number of products per page on search products page
        /// </summary>
        public int SearchPageArticlesPerPage { get; set; }



        #region Article内容配置
        
     
        /// <summary>
        /// Gets or sets a number of product tags that appear in the tag cloud
        /// </summary>
        public int NumberOfArticleTags { get; set; }

        /// <summary>
        /// Gets or sets a number of products per page on 'products by tag' page
        /// </summary>
        public int ArticlesByTagPageSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether users can select the page size for 'products by tag'
        /// </summary>
        public bool ArticlesByTagAllowUsersToSelectPageSize { get; set; }

        /// <summary>
        /// Gets or sets the available user selectable page size options for 'products by tag'
        /// </summary>
        public string ArticlesByTagPageSizeOptions { get; set; }

        public int ArticleSearchPageSize { get; set; }

        public bool ArticleSearchAllowUsersToSelectPageSize { get; set; }

        public string ArticleSearchPageSizeOptions { get; set; }
		
		public int DisplayAllImagesNumber { get; set; }

        public bool ShowColorSquaresInLists { get; set; }

		public bool HideBuyButtonInLists { get; set; }

        public int? LabelAsNewForMaxDays { get; set; }


        /// <summary>
        /// 获取或设置页面大小
        /// </summary>
        public int ArticlePageSize { get; set; }

        /// <summary>
        /// 获取或设置一个值指示是否注册用户可以留言
        /// </summary>
        public bool AllowNotRegisteredUsersToLeaveComments { get; set; }

        /// <summary>
        /// 获取或设置一个值指示新评论是否通知
        /// </summary>
        public bool NotifyAboutNewArticleComments { get; set; }

        /// <summary>
        ///获取或设置一个标签数量出现在标签云
        /// </summary>
        public int NumberOfTags { get; set; }

        /// <summary>
        /// 使RSS提要链接客户浏览器地址栏
        /// </summary>
        public bool ShowHeaderRssUrl { get; set; }
        #endregion

        /// <summary>
        /// Gets or sets the available user selectable default page size options
        /// </summary>
        public string DefaultPageSizeOptions { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether to use small product boxes on home page
        /// </summary>
        public bool UseSmallArticleBoxOnHomePage { get; set; }

        /// <summary>
        /// An option indicating whether products on category and manufacturer pages should include featured products as well
        /// </summary>
        public bool IncludeFeaturedArticlesInNormalLists { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether tier prices should be displayed with applied discounts (if available)
        /// </summary>
        public bool DisplayTierPricesWithDiscounts { get; set; }
        

        /// <summary>
        /// Gets or sets a value indicating whether to ignore featured products (side-wide)
        /// </summary>
        public bool IgnoreFeaturedArticles { get; set; }

        /// <summary>
        /// Gets or set the default value to use for Category page size options (for new Categories)
        /// </summary>
        /// <remarks>Obsolete</remarks>
        public string DefaultCategoryPageSizeOptions { get; set; }

        /// <summary>
        /// Gets or set the default value to use for Client page size opitons (for new Clients)
        /// </summary>
        /// <remarks>Obsolete</remarks>
        public string DefaultClientPageSizeOptions { get; set; }



        /// <summary>
        /// Gets or sets a maximum file upload size in bytes for product attributes ('File Upload' type)
        /// </summary>
        public int FileUploadMaximumSizeBytes { get; set; }

        /// <summary>
        /// Gets or sets a list of allowed file extensions for user uploaded files
        /// </summary>
        public List<string> FileUploadAllowedExtensions { get; set; }

        /// <summary>
        /// Gets or sets the value indicating how many manufacturers to display in manufacturers block
        /// </summary>
        public int ClientsBlockItemsToDisplay { get; set; }

		/// <summary>
		/// Gets or sets a value indicating if html long text should be collapsed
		/// </summary>
		public bool EnableHtmlTextCollapser { get; set; }

		/// <summary>
		/// Gets or sets the height of collapsed text
		/// </summary>
		public int HtmlTextCollapsedHeight { get; set; }


		/// <summary>
		/// Gets or sets how many items to display maximally in the most recently used category list
		/// </summary>
		public int MostRecentlyUsedCategoriesMaxSize { get; set; }

		/// <summary>
		/// Gets or sets how many items to display maximally in the most recently used manufacturer list
		/// </summary>
		public int MostRecentlyUsedClientsMaxSize { get; set; }
    }
}