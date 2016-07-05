﻿using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
using System.Collections.Generic;
using System.Web.Mvc;


namespace CAF.WebSite.Mvc.Admin.Models.Articles
{
    public partial class ArticleListModel : ModelBase
    {
        public ArticleListModel()
        {
            AvailableCategories = new List<SelectListItem>();
            AvailableSites = new List<SelectListItem>();
            AvailableArticleTypes = new List<SelectListItem>();
            AvailableModelTemplates = new List<SelectListItem>();
            BatchCategory = new BatchCategoryModel();
            ArticleszCategoryModels = new List<ArticleCategoryModel>();
        }

        public List<ArticleModel> Articles { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.List.SearchArticleName")]
        [AllowHtml]
        public string SearchArticleName { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.List.SearchCategory")]
        public int SearchCategoryId { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.List.SearchWithoutCategories")]
        public bool SearchWithoutCategories { get; set; }

        [LangResourceDisplayName("Admin.Common.Site.SearchFor")]
        public int SearchSiteId { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.List.SearchArticleType")]
        public int SearchArticleTypeId { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.List.GoDirectlyToSku")]
        [AllowHtml]
        public string GoDirectlyToSku { get; set; }

        public bool DisplayArticlePictures { get; set; }
        public bool DisplayPdfExport { get; set; }
        public int GridPageSize { get; set; }
        public int SiteCount { get; set; }
        public object CategoriesObject { get; set; }
        public IList<ArticleCategoryModel> ArticleszCategoryModels { get; set; }
        public IList<SelectListItem> AvailableCategories { get; set; }
        public IList<SelectListItem> AvailableSites { get; set; }
        public IList<SelectListItem> AvailableArticleTypes { get; set; }
        public IList<SelectListItem> AvailableModelTemplates { get; set; }
        //BatchCategoryModel 
        public BatchCategoryModel BatchCategory { get; set; }
        public class BatchCategoryModel : ModelBase
        {
            public bool OpenCategorieCheckBox { get; set; }
            public bool OpenTemplateCheckBox { get; set; }
            public bool OpenCheckBox { get; set; }

            [LangResourceDisplayName("Admin.ContentManagement.Articles.List.CategoryId")]
            [AllowHtml]
            public int? CategoryId { get; set; }

            [LangResourceDisplayName("Admin.ContentManagement.Articles.List.SelectedIds")]
            [AllowHtml]
            public ICollection<int> SelectedIds { get; set; }
            [LangResourceDisplayName("Admin.ContentManagement.Articles.List.TemplateId")]
            [AllowHtml]
            public int? TemplateId { get; set; }
            /// <summary>
            /// 是否置顶
            /// </summary>
            [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.IsTop")]
            public bool IsTop { get; set; }
            /// <summary>
            /// 是否推荐
            /// </summary>
            [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.IsRed")]
            public bool IsRed { get; set; }
            /// <summary>
            /// 是否热门
            /// </summary>
            [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.IsHot")]
            public bool IsHot { get; set; }
            /// <summary>
            /// 是否幻灯片
            /// </summary>
            [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.IsSlide")]
            public bool IsSlide { get; set; }
            /// <summary>
            /// 是否管理员发布0不是1是
            /// </summary>
            [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.IsSys")]
            public bool IsSys { get; set; }
            /// <summary>
            /// 是否允许评论
            /// </summary>
            [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.AllowComments")]
            public bool AllowComments { get; set; }
        }
    }
}