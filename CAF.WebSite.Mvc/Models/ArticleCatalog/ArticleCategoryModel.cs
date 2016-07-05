using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Mvc.Models.Articles;
using CAF.WebSite.Mvc.Models.Media;
using System.Collections.Generic;


namespace CAF.WebSite.Mvc.Models.ArticleCatalog
{
    public partial class ArticleCategoryModel : EntityModelBase
    {
        public ArticleCategoryModel()
        {
            PictureModel = new PictureModel();
            SubCategories = new List<SubCategoryModel>();
            CategoryBreadcrumb = new List<MenuItem>();
            PagingFilteringContext = new ArticleCatalogPagingFilteringModel();
        }

        public string Name { get; set; }
		public string FullName { get; set; }
        public string Description { get; set; }
		public string BottomDescription { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string SeName { get; set; }
        
        public PictureModel PictureModel { get; set; }

        public ArticleCatalogPagingFilteringModel PagingFilteringContext { get; set; }

        public bool DisplayCategoryBreadcrumb { get; set; }
        public IList<MenuItem> CategoryBreadcrumb { get; set; }

        public bool DisplayFilter { get; set; }
        public bool ShowSubcategoriesAboveArticleLists { get; set; }
        

        public IList<SubCategoryModel> SubCategories { get; set; }

        public IList<ArticlePostModel> Articles { get; set; }
        

		#region Nested Classes

        public partial class SubCategoryModel : EntityModelBase
        {
            public SubCategoryModel()
            {
                PictureModel = new PictureModel();
            }

            public string Name { get; set; }

            public string SeName { get; set; }

            public PictureModel PictureModel { get; set; }
        }

		#endregion
    }
}