using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Mvc.Models.Articles;
using System.Collections.Generic;
using System.Web.Mvc;


namespace CAF.WebSite.Mvc.Models.ArticleCatalog
{
    public partial class SearchModel : ModelBase
    {
        public SearchModel()
        {
            PagingFilteringContext = new SearchPagingFilteringModel();
            Articles = new List<ArticlePostModel>();
            this.AvailableArticleCategories = new List<SelectListItem>();
          
        }

        public string Warning { get; set; }

        public bool NoResults { get; set; }

        /// <summary>
        /// Query string
        /// </summary>
        [LangResourceDisplayName("Search.SearchTerm")]
        [AllowHtml]
        public string Q { get; set; }
        /// <summary>
        /// Category ID
        /// </summary>
        [LangResourceDisplayName("Search.Category")]
        public int Cid { get; set; }
        [LangResourceDisplayName("Search.IncludeSubCategories")]
        public bool Isc { get; set; }

        /// <summary>
        /// Price - From 
        /// </summary>
        [AllowHtml]
        public string Pf { get; set; }
        /// <summary>
        /// Price - To
        /// </summary>
        [AllowHtml]
        public string Pt { get; set; }
        /// <summary>
        /// A value indicating whether to search in descriptions
        /// </summary>
        [LangResourceDisplayName("Search.SearchInDescriptions")]
        public bool Sid { get; set; }
        /// <summary>
        /// A value indicating whether to search in descriptions
        /// </summary>
        [LangResourceDisplayName("Search.AdvancedSearch")]
        public bool As { get; set; }


        public IList<SelectListItem> AvailableArticleCategories { get; set; }
        public SearchPagingFilteringModel PagingFilteringContext { get; set; }
        public IList<ArticlePostModel> Articles { get; set; }
    }
}