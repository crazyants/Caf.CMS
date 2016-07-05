using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using AutoMapper;
using FluentValidation.Attributes;
using CAF.WebSite.Application.WebUI.Localization;
using CAF.WebSite.Application.WebUI;
using CAF.Mvc.JQuery.Datatables;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.WebSite.Mvc.Admin.Validators.Articles;
using CAF.Mvc.JQuery.Datatables.Models;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Mvc.Admin.Models.Sites;
using CAF.WebSite.Mvc.Admin.Models.Users;

namespace CAF.WebSite.Mvc.Admin.Models.Articles
{
    [Validator(typeof(ArticleValidator))]
    public class ArticleModel : TabbableModel, ILocalizedModel<ArticleLocalizedModel>
    {
        public ArticleModel()
        {
            Locales = new List<ArticleLocalizedModel>();
            ArticlePictureModels = new List<ArticlePictureModel>();
            AddPictureModel = new ArticlePictureModel();
            AvailableModelTemplates = new List<SelectListItem>();
            AvailableArticleTags = new List<SelectListItem>();
            ArticleExtendedAttributes = new List<ArticleExtendedAttributeModel>();
            AvailableCategorys = new List<SelectListItem>();
        }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.ID")]
        public override int Id { get; set; }

        /// <summary>
        /// 类别ID
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.CategoryId")]
        [AllowHtml]
        public int CategoryId { get; set; }
        public List<SelectListItem> AvailableCategorys { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.CategoryName")]
        public string CategoryBreadcrumb { get; set; }

        //picture thumbnail
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.PictureThumbnailUrl")]
        public string PictureThumbnailUrl { get; set; }
        public bool NoThumb { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.Picture")]
        [AllowHtml]
        [UIHint("Picture")]
        public int PictureId { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.IsPasswordProtected")]
        public bool IsPasswordProtected { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.Title")]
        [AllowHtml]
        public string Title { get; set; }
        /// <summary>
        /// 地址连接
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.Url")]
        [AllowHtml]
        public string Url { get; set; }
        /// <summary>
        /// 外部链接
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.LinkUrl")]
        [AllowHtml]
        public string LinkUrl { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.ImgUrl")]
        [AllowHtml]
        public string ImgUrl { get; set; }
        /// <summary>
        /// SEO标题
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.MetaTitle")]
        [AllowHtml]
        public string MetaTitle { get; set; }
        /// <summary>
        /// SEO关健字
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.MetaKeywords")]
        [AllowHtml]
        public string MetaKeywords { get; set; }
        /// <summary>
        /// SEO描述
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.MetaDescription")]
        [AllowHtml]
        public string MetaDescription { get; set; }
        /// <summary>
        /// 内容摘要
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.ShortContent")]
        [AllowHtml]
        public string ShortContent { get; set; }
        /// <summary>
        /// 详细内容
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.FullContent")]
        [AllowHtml]
        public string FullContent { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }
        /// <summary>
        /// 浏览次数
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.Click")]
        public int Click { get; set; }
        /// <summary>
        /// 状态0正常1未审核2锁定
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.Status")]
        public int StatusId { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.Status")]
        public string StatusName { get; set; }
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

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.AllowUserReviews")]
        public bool AllowUserReviews { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.Comments")]
        public int Comments { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.StartDate")]
        public DateTime? StartDate { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.EndDate")]
        public DateTime? EndDate { get; set; }

        [LangResourceDisplayName("Common.CreatedOn")]
        public DateTime? CreatedOn { get; set; }

        [LangResourceDisplayName("Common.UpdatedOn")]
        public DateTime? UpdatedOn { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.Author")]
        [AllowHtml]
        public string Author { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.IsDownload")]
        public bool IsDownload { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.Download")]
        [UIHint("Download")]
        public int DownloadId { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.UnlimitedDownloads")]
        public bool UnlimitedDownloads { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.MaxNumberOfDownloads")]
        public int MaxNumberOfDownloads { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.DownloadExpirationDays")]
        public int? DownloadExpirationDays { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.DownloadActivationType")]
        public int DownloadActivationTypeId { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.SeName")]
        [AllowHtml]
        public string SeName { get; set; }

        public int ChannelId { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.ArticleTags")]
        [AllowHtml]
        public string ArticleTags { get; set; }
        public IList<SelectListItem> AvailableArticleTags { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.ModelTemplate")]
        [AllowHtml]
        public int ModelTemplateId { get; set; }
        public IList<SelectListItem> AvailableModelTemplates { get; set; }

        public IList<ArticleLocalizedModel> Locales { get; set; }

        //pictures
        public ArticlePictureModel AddPictureModel { get; set; }
        public IList<ArticlePictureModel> ArticlePictureModels { get; set; }
        //Site mapping
        [LangResourceDisplayName("Admin.Common.Site.LimitedTo")]
        public bool LimitedToSites { get; set; }
        [LangResourceDisplayName("Admin.Common.Site.AvailableFor")]
        public List<SiteModel> AvailableSites { get; set; }
        public int[] SelectedSiteIds { get; set; }
        //ACL (customer roles)
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.SubjectToAcl")]
        public bool SubjectToAcl { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.AclUserRoles")]
        public List<UserRoleModel> AvailableUserRoles { get; set; }
        public int[] SelectedUserRoleIds { get; set; }

        public bool DisplayArticlePictures { get; set; }

        public string ExtendedAttributeInfo { get; set; }
        public IList<ArticleExtendedAttributeModel> ArticleExtendedAttributes { get; set; }

        public string CheckoutAttributeInfo { get; set; }

        public bool SiteContentShare { get; set; }

        #region Nested classes

        public class ArticlePictureModel : EntityModelBase
        {
            public int ArticleId { get; set; }
            [UIHint("Picture")]
            [LangResourceDisplayName("Admin.ContentManagement.Articles.Pictures.Fields.Picture")]
            public int PictureId { get; set; }
            [LangResourceDisplayName("Admin.ContentManagement.Articles.Pictures.Fields.Picture")]
            public string PictureUrl { get; set; }
            [LangResourceDisplayName("Admin.ContentManagement.Articles.Pictures.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }
            [LangResourceDisplayName("Admin.ContentManagement.Articles.Pictures.Fields.DisplayOrder")]
            public string SeoFilename { get; set; }
        }

        public class ArticleCategoryModel : EntityModelBase
        {
            [LangResourceDisplayName("Admin.ContentManagement.Articles.Categories.Fields.Category")]
            [UIHint("ArticleCategory")]
            public string Category { get; set; }

            public int ArticleId { get; set; }

            public int CategoryId { get; set; }

            [LangResourceDisplayName("Admin.ContentManagement.Articles.Categories.Fields.IsFeaturedArticle")]
            public bool IsFeaturedArticle { get; set; }

            [LangResourceDisplayName("Admin.ContentManagement.Articles.Categories.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }
        }

        public class RelatedArticleModel : EntityModelBase
        {
            public int ArticleId2 { get; set; }

            [LangResourceDisplayName("Admin.ContentManagement.Articles.RelatedArticles.Fields.Article")]
            public string Article2Name { get; set; }


            [LangResourceDisplayName("Admin.ContentManagement.Articles.RelatedArticles.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }


            [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.Published")]
            public bool Article2Published { get; set; }
        }

        public class AddRelatedArticleModel : ModelBase
        {
            public AddRelatedArticleModel()
            {
                AvailableCategories = new List<SelectListItem>();
                AvailableSites = new List<SelectListItem>();
            }
            public List<ArticleModel> Articles { get; set; }

            [LangResourceDisplayName("Admin.ContentManagement.Articles.List.SearchArticleName")]
            [AllowHtml]
            public string SearchArticleName { get; set; }

            [LangResourceDisplayName("Admin.ContentManagement.Articles.List.SearchCategory")]
            public int SearchCategoryId { get; set; }


            [LangResourceDisplayName("Admin.ContentManagement.Articles.List.SearchSite")]
            public int SearchSiteId { get; set; }

            public IList<SelectListItem> AvailableCategories { get; set; }
            public IList<SelectListItem> AvailableSites { get; set; }

            public int ArticleId { get; set; }

            public int[] SelectedArticleIds { get; set; }


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

    public class ArticleLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.Title")]
        [AllowHtml]
        public string Title { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.ShortDescription")]
        [AllowHtml]
        public string ShortContent { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.FullDescription")]
        [AllowHtml]
        public string FullContent { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.MetaKeywords")]
        [AllowHtml]
        public string MetaKeywords { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.MetaDescription")]
        [AllowHtml]
        public string MetaDescription { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.MetaTitle")]
        [AllowHtml]
        public string MetaTitle { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.SeName")]
        [AllowHtml]
        public string SeName { get; set; }



    }


}