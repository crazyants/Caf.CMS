using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Mvc.Models.Media;
using System.Collections.Generic;


namespace CAF.WebSite.Mvc.Models.Catalog
{
    public partial class CategoryModel : EntityModelBase
    {
        public CategoryModel()
        {
            PictureModel = new PictureModel();
            FeaturedProducts = new List<ProductOverviewModel>();
            Products = new List<ProductOverviewModel>();
            PagingFilteringContext = new CatalogPagingFilteringModel();
            SubCategories = new List<SubCategoryModel>();
            CategoryBreadcrumb = new List<MenuItem>();
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

        public CatalogPagingFilteringModel PagingFilteringContext { get; set; }

        public bool DisplayCategoryBreadcrumb { get; set; }
        public IList<MenuItem> CategoryBreadcrumb { get; set; }

        public bool DisplayFilter { get; set; }
        public bool ShowSubcategoriesAboveProductLists { get; set; }
        
        public IList<SubCategoryModel> SubCategories { get; set; }

        public IList<ProductOverviewModel> FeaturedProducts { get; set; }
        public IList<ProductOverviewModel> Products { get; set; }
        

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