using System;
using System.Collections.Generic;
using FluentValidation.Attributes;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Mvc.Validators.Articles;
using CAF.WebSite.Mvc.Models.ArticleCatalog;
using CAF.WebSite.Mvc.Models.Media;
using CAF.WebSite.Application.WebUI;
using CAF.Infrastructure.Core.Domain.Cms.Articles;


namespace CAF.WebSite.Mvc.Models.Articles
{
    [Validator(typeof(ArticlePostValidator))]
    public partial class ArticlePostModel : EntityModelBase
    {
        private ArticleDetailsPictureModel _detailsPictureModel;
        public ArticlePostModel()
        {
            Tags = new List<ArticleTagModel>();
            Comments = new List<ArticleCommentModel>();
            AddNewComment = new AddArticleCommentModel();
            PagingFilteringContext = new ArticleCatalogPagingFilteringModel();
            ArticleExtendedAttributes = new List<ArticleExtendedAttributeModel>();
        }
        //picture(s)
        public ArticleDetailsPictureModel DetailsPictureModel
        {
            get
            {
                if (_detailsPictureModel == null)
                    _detailsPictureModel = new ArticleDetailsPictureModel();
                return _detailsPictureModel;
            }
        }
        public string ShortContent { get; set; }
        public string FullContent { get; set; }
        public string SeName { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string ModelTemplateViewPath { get; set; }
        public string Title { get; set; }
        public string LinkUrl { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        public string ImgUrl { get; set; }
        public int Click { get; set; }
        public int Status { get; set; }
        public bool IsTop { get; set; }
        public bool IsRed { get; set; }
        public bool IsHot { get; set; }
        public bool IsSlide { get; set; }
        public bool IsNew { get; set; }
        public bool AllowComments { get; set; }
        public bool DisplayArticleReviews { get; set; }
        public int NumberOfComments { get; set; }
        public int ApprovedCommentCount { get; set; }
        public int NotApprovedCommentCount { get; set; }
        public DateTime? StartDateUtc { get; set; }
        public DateTime? EndDateUtc { get; set; }

        public string Body { get; set; }

        public string Author { get; set; }

        public string PostCreatedOnStr { get; set; }

        public DateTime CreatedOn { get; set; }

        public int? PreId { get; set; }
        public int? NextId { get; set; }
        public PictureModel PictureModel { get; set; }
        public IList<ArticleTagModel> Tags { get; set; }

        public IList<ArticleCommentModel> Comments { get; set; }
        public AddArticleCommentModel AddNewComment { get; set; }
        public ArticleCatalogPagingFilteringModel PagingFilteringContext { get; set; }

        public IList<ArticleExtendedAttributeModel> ArticleExtendedAttributes { get; set; }

        // codehint: sm-add
        public int AvatarPictureSize { get; set; }
        public bool AllowUsersToUploadAvatars { get; set; }
        public bool HasSampleDownload { get; set; }
        public PictureModel DefaultPictureModel { get; set; }

        public ArticlePrevAndNextModel ArticlePNModel { get; set; }

        #region Nested Classes
        public partial class ArticlePrevAndNextModel
        {
            public int? PrevId { get; set; }
            public string PrevSeName { get; set; }
            public string PrevTitle { get; set; }
            public int? NextId { get; set; }
            public string NextSeName { get; set; }
            public string NextTitle { get; set; }
        }
        public partial class ArticleBreadcrumbModel : ModelBase
        {
            public ArticleBreadcrumbModel()
            {
                CategoryBreadcrumb = new List<MenuItem>();
            }

            public int ArticleId { get; set; }
            public string ArticleName { get; set; }
            public string ArticleSeName { get; set; }
            public bool OnlyCurrentCategory { get; set; }
            public IList<MenuItem> CategoryBreadcrumb { get; set; }
        }

        public partial class ArticleExtendedAttributeModel : EntityModelBase
        {
            public ArticleExtendedAttributeModel()
            {
                Values = new List<ArticleExtendedAttributeValueModel>();
                AllowedFileExtensions = new List<string>();
            }

            public string Name { get; set; }

            public string DefaultValue { get; set; }

            public string TextPrompt { get; set; }

            public bool IsRequired { get; set; }

            /// <summary>
            /// Selected day value for datepicker
            /// </summary>
            public int? SelectedDay { get; set; }
            /// <summary>
            /// Selected month value for datepicker
            /// </summary>
            public int? SelectedMonth { get; set; }
            /// <summary>
            /// Selected year value for datepicker
            /// </summary>
            public int? SelectedYear { get; set; }
            public AttributeControlType AttributeControlType { get; set; }
            /// <summary>
            /// Allowed file extensions for customer uploaded files
            /// </summary>
            public IList<string> AllowedFileExtensions { get; set; }

            public IList<ArticleExtendedAttributeValueModel> Values { get; set; }
        }

        public partial class ArticleExtendedAttributeValueModel : EntityModelBase
        {
            public string Name { get; set; }

            //public string PriceAdjustment { get; set; }

            public bool IsPreSelected { get; set; }
        }

        #endregion
    }

    public partial class ArticleDetailsPictureModel : ModelBase
    {
        public ArticleDetailsPictureModel()
        {
            PictureModels = new List<PictureModel>();
        }

        public string Name { get; set; }
        public string AlternateText { get; set; }
        public bool DefaultPictureZoomEnabled { get; set; }
        public string PictureZoomType { get; set; }
        public PictureModel DefaultPictureModel { get; set; }
        public IList<PictureModel> PictureModels { get; set; }
        public int GalleryStartIndex { get; set; }
    }


}
