using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI;
using CAF.Mvc.JQuery.Datatables;
using CAF.Mvc.JQuery.Datatables.Models;
using CAF.WebSite.Mvc.Admin.Validators.Categorys;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using FluentValidation.Attributes;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using CAF.Mvc.JQuery.Datatables.Core;
using CAF.WebSite.Application.WebUI.Localization;
using CAF.WebSite.Mvc.Admin.Models.Sites;
using CAF.WebSite.Mvc.Admin.Models.Users;
namespace CAF.WebSite.Mvc.Admin.Models.Articles
{
    /// <summary>
    /// 系统类型
    /// </summary>
    [Serializable]
    [Validator(typeof(ArticleCategoryValidator))]
    public partial class ArticleCategoryModel : TabbableModel, ILocalizedModel<ArticleCategoryLocalizedModel>
    {
        public ArticleCategoryModel()
        {
            if (PageSize < 1)
            {
                PageSize = 5;
            }
            Locales = new List<ArticleCategoryLocalizedModel>();
            AvailableModelTemplates = new List<SelectListItem>();
            AvailableDetailModelTemplates = new List<SelectListItem>();
            AvailableDefaultViewModes = new List<SelectListItem>();
            AvailableChannels = new List<SelectListItem>();
        }
       
        /// <summary>
        ///类别标题
        /// </summary>
        [DataMember]
        [LangResourceDisplayName("Admin.ContentManagement.ArticleCategory.Fields.Name")]
        public string Name { get; set; }
        /// <summary>
        /// 调用别名
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.ArticleCategory.Fields.Alias")]
        [AllowHtml]
        public string Alias { get; set; }
        /// <summary>
        /// 全名
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.ArticleCategory.Fields.FullName")]
        [AllowHtml]
        public string FullName { get; set; }
        /// <summary>
        /// 父类别ID
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.ArticleCategory.Fields.Parent")]
        [AllowHtml]
        public int? ParentCategoryId { get; set; }
        public string ParentCategoryBreadcrumb { get; set; }

        /// <summary>
        /// URL跳转地址
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.ArticleCategory.Fields.LinkUrl")]
        [AllowHtml]
        public string LinkUrl { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.ArticleCategory.Fields.Picture")]
        [AllowHtml]
        [UIHint("Picture")]
        public int PictureId { get; set; }
        /// <summary>
        /// 备注说明
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.ArticleCategory.Fields.Description")]
        [AllowHtml]
        public string Description { get; set; }
        /// <summary>
        /// 获取或设置一个描述显示类别页面的底部
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.ArticleCategory.Fields.BottomDescription")]
        [AllowHtml]
        public string BottomDescription { get; set; }
        /// <summary>
        /// SEO标题
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.ArticleCategory.Fields.MetaTitle")]
        [AllowHtml]
        public string MetaTitle { get; set; }
        /// <summary>
        /// SEO关健字
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.ArticleCategory.Fields.MetaKeywords")]
        [AllowHtml]
        public string MetaKeywords { get; set; }
        /// <summary>
        /// SEO描述
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.ArticleCategory.Fields.MetaDescription")]
        [AllowHtml]
        public string MetaDescription { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.ArticleCategory.Fields.SeName")]
        [AllowHtml]
        public string SeName { get; set; }

        /// <summary>
        /// 获取或设置一个值,该值指示该实体是否发表
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.ArticleCategory.Fields.Published")]
        public bool Published { get; set; }
        /// <summary>
        /// 获取或设置一个值,该值指是否显示在主页上的类别
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.ArticleCategory.Fields.ShowOnHomePage")]
        public bool ShowOnHomePage { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.ArticleCategory.Fields.Name")]
        public string Breadcrumb { get; set; }
        /// <summary>
        ///排序数字
        /// </summary>
        [DataMember]
        [LangResourceDisplayName("Admin.ContentManagement.ArticleCategory.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [LangResourceDisplayName("Common.CreatedOn")]
        public DateTime? CreatedOnUtc { get; set; }

        [LangResourceDisplayName("Common.UpdatedOn")]
        public DateTime? ModifiedOnUtc { get; set; }

        public ArticleCategory Category { get; set; }

        public IList<ArticleCategoryLocalizedModel> Locales { get; set; }
        //ACL
        [LangResourceDisplayName("Admin.ContentManagement.ArticleCategory.Fields.SubjectToAcl")]
        public bool SubjectToAcl { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.ArticleCategory.Fields.AclUserRoles")]
        public List<UserRoleModel> AvailableUserRoles { get; set; }
        public int[] SelectedUserRoleIds { get; set; }

        //Site mapping
        [LangResourceDisplayName("Admin.Common.Site.LimitedTo")]
        public bool LimitedToSites { get; set; }
        [LangResourceDisplayName("Admin.Common.Site.AvailableFor")]
        public List<SiteModel> AvailableSites { get; set; }
        public int[] SelectedSiteIds { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.ArticleCategory.Fields.ModelTemplate")]
        [AllowHtml]
        public int ModelTemplateId { get; set; }
        public IList<SelectListItem> AvailableModelTemplates { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.ArticleCategory.Fields.ModelDetailTemplate")]
        [AllowHtml]
        public int DetailModelTemplateId { get; set; }
        public IList<SelectListItem> AvailableDetailModelTemplates { get; set; }


        [LangResourceDisplayName("Admin.ContentManagement.ArticleCategory.Fields.Channel")]
        [AllowHtml]
        public int ChannelId { get; set; }
        public IList<SelectListItem> AvailableChannels { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.ArticleCategory.Fields.ChannelTitle")]
        public string ChannelName { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Catalog.DefaultViewMode")]
        public string DefaultViewMode { get; set; }
        public IList<SelectListItem> AvailableDefaultViewModes { get; private set; }
        [LangResourceDisplayName("Admin.ContentManagement.ArticleCategory.Fields.PageSize")]
        public int PageSize { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.ArticleCategory.Fields.AllowUsersToSelectPageSize")]
        public bool AllowUsersToSelectPageSize { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.ArticleCategory.Fields.PageSizeOptions")]
        public string PageSizeOptions { get; set; }

      
        public bool SiteContentShare { get; set; }

        public BatchCategoryModel BatchCategory { get; set; }
        public class BatchCategoryModel : ModelBase
        {
            public bool OpenTemplateCheckBox { get; set; }


            [LangResourceDisplayName("Admin.ContentManagement.Articles.List.SelectedIds")]
            [AllowHtml]
            public ICollection<int> SelectedIds { get; set; }
            [LangResourceDisplayName("Admin.ContentManagement.Articles.List.TemplateId")]
            [AllowHtml]
            public int? TemplateId { get; set; }

        }
    }

    public class ArticleCategoryLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.ArticleCategory.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.ArticleCategory.Fields.FullName")]
        public string FullName { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.ArticleCategory.Fields.Description")]
        [AllowHtml]
        public string Description { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.ArticleCategory.Fields.BottomDescription")]
        [AllowHtml]
        public string BottomDescription { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.ArticleCategory.Fields.MetaKeywords")]
        [AllowHtml]
        public string MetaKeywords { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.ArticleCategory.Fields.MetaDescription")]
        [AllowHtml]
        public string MetaDescription { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.ArticleCategory.Fields.MetaTitle")]
        [AllowHtml]
        public string MetaTitle { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.ArticleCategory.Fields.SeName")]
        [AllowHtml]
        public string SeName { get; set; }
    }


}
