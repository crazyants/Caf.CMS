using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System;
using System.Collections.Generic;
using System.Linq;


namespace CAF.WebSite.Application.Services.Articles
{
    public class ArticleSearchContext
    {
        #region Methods

        public virtual DateTime? GetParsedMonth()
        {
            DateTime? result = null;
            if (!String.IsNullOrEmpty(this.Month))
            {
                string[] tempDate = this.Month.Split(new char[] { '-' });
                if (tempDate.Length == 2)
                {
                    result = new DateTime(Convert.ToInt32(tempDate[0]), Convert.ToInt32(tempDate[1]), 1);
                }
            }
            return result;
        }
        public virtual DateTime? GetFromMonth()
        {
            var filterByMonth = GetParsedMonth();
            if (filterByMonth.HasValue)
                return filterByMonth.Value;
            return null;
        }
        public virtual DateTime? GetToMonth()
        {
            var filterByMonth = GetParsedMonth();
            if (filterByMonth.HasValue)
                return filterByMonth.Value.AddMonths(1).AddSeconds(-1);
            return null;
        }
        #endregion

        public ArticleSearchContext()
        {
            CategoryIds = new List<int>();
            PageSize = 12;
        }

        /// <summary>
        /// Optional query to use to build the Article query. Otherwise the repository of the Article service is used (default). 
        /// </summary>
        public IQueryable<Article> Query { get; set; }

        /// <summary>
        /// Category identifiers
        /// </summary>
        public IList<int> CategoryIds { get; set; }

        /// <summary>
        /// Filter by Article identifiers
        /// </summary>
        /// <remarks>Only implemented in LINQ mode at the moment</remarks>
        public IList<int> ArticleIds { get; set; }

        /// <summary>
        /// A value indicating whether ALL given <see cref="CategoryIds"/> must be assigned to the resulting Articles (default is ANY)
        /// </summary>
        /// <remarks>Only works in LINQ mode at the moment</remarks>
        public bool MatchAllcategories { get; set; }
        /// <summary>
        /// A value indicating whether to load products without any catgory mapping
        /// </summary>
        public bool WithoutCategories { get; set; }

        /// <summary>
        /// A value indicating whether loaded Articles are marked as featured (relates only to categories and manufacturers). 0 to load featured Articles only, 1 to load not featured Articles only, null to load all Articles
        /// </summary>
        public bool? FeaturedArticles { get; set; }

        /// <summary>
        /// Article tag identifier; 0 to load all records
        /// </summary>
        public int ArticleTagId { get; set; }

        /// <summary>
        /// Keywords
        /// </summary>
        public string Keywords { get; set; }

        /// <summary>
        /// A value indicating whether to search in descriptions
        /// </summary>
        public bool SearchDescriptions { get; set; }

        /// <summary>
        /// A value indicating whether to search by a specified "keyword" in Article tags
        /// </summary>
        public bool SearchArticleTags { get; set; }

        /// <summary>
        /// Language identifier
        /// </summary>
        public int LanguageId { get; set; }

        /// <summary>
        /// Order by
        /// </summary>
        public ArticleSortingEnum OrderBy { get; set; }

        /// <summary>
        /// Page index
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// Page size
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// A values indicating whether to load only products marked as "visible individually"; "false" to load all records; "true" to load "visible individually" only
        /// </summary>
        public bool VisibleIndividuallyOnly { get; set; }

        /// <summary>
        /// A value indicating whether to show hidden records
        /// </summary>
        public bool ShowHidden { get; set; }

        /// <summary>
        /// Site identifier; 0 to load all records
        /// </summary>
        public int SiteId { get; set; }

        public string Month { get; set; }

        /// <summary>
        /// Any value indicating the origin of the search request,
        /// e.g. the category id, if the caller is is a category page.
        /// Can be useful in customization scenarios.
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// 是否置顶
        /// </summary>
        public bool? IsTop { get; set; }
        /// <summary>
        /// 是否推荐
        /// </summary>
        public bool? IsRed { get; set; }
        /// <summary>
        /// 是否热门
        /// </summary>
        public bool? IsHot { get; set; }
        /// <summary>
        /// 是否幻灯片
        /// </summary>
        public bool? IsSlide { get; set; }

    }
}
