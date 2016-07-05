using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Mvc.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace CAF.WebSite.Mvc.Models.ArticleCatalog
{
    // 类别筛选条件（界面）
    public partial class ArticleCatalogPagingFilteringModel : PagingFilteringModel //BasePageableModel
    {
        #region Constructors

        public ArticleCatalogPagingFilteringModel()
        {

        }

        #endregion

        #region Properties

        /// <summary>
        /// 是否置顶
        /// </summary>
        [LangResourceDisplayName("ArticleCategories.IsTop")]
        public bool? IsTop { get; set; }
        /// <summary>
        /// 是否推荐
        /// </summary>
        [LangResourceDisplayName("ArticleCategories.IsRed")]
        public bool? IsRed { get; set; }
        /// <summary>
        /// 是否热门
        /// </summary>
        [LangResourceDisplayName("ArticleCategories.IsHot")]
        public bool? IsHot { get; set; }
        /// <summary>
        /// 是否幻灯片
        /// </summary>
        [LangResourceDisplayName("ArticleCategories.IsSlide")]
        public bool? IsSlide { get; set; }
        /// <summary>
        /// 是否最新
        /// </summary>
        [LangResourceDisplayName("ArticleCategories.IsNew")]
        public bool? IsNew { get; set; }

        #endregion


    }
}