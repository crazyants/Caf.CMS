using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI.Localization;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Mvc.Admin.Models.Sites;
using CAF.WebSite.Mvc.Admin.Validators.Clients;


namespace CAF.WebSite.Mvc.Admin.Models.Clients
{
    [Validator(typeof(ClientValidator))]
    public class ClientModel : EntityModelBase, ILocalizedModel<ClientLocalizedModel>
    {
        public ClientModel()
        {

            Locales = new List<ClientLocalizedModel>();
            AvailableManufacturerTemplates = new List<SelectListItem>();
        }

        [LangResourceDisplayName("Admin.ArticleCategory.Clients.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [LangResourceDisplayName("Admin.ArticleCategory.Clients.Fields.Description")]
        [AllowHtml]
        public string Description { get; set; }

        [LangResourceDisplayName("Admin.ArticleCategory.Clients.Fields.ManufacturerTemplate")]
        [AllowHtml]
        public int ManufacturerTemplateId { get; set; }
        public IList<SelectListItem> AvailableManufacturerTemplates { get; set; }

        [LangResourceDisplayName("Admin.ArticleCategory.Clients.Fields.MetaKeywords")]
        [AllowHtml]
        public string MetaKeywords { get; set; }

        [LangResourceDisplayName("Admin.ArticleCategory.Clients.Fields.MetaDescription")]
        [AllowHtml]
        public string MetaDescription { get; set; }

        [LangResourceDisplayName("Admin.ArticleCategory.Clients.Fields.MetaTitle")]
        [AllowHtml]
        public string MetaTitle { get; set; }

        [LangResourceDisplayName("Admin.ArticleCategory.Clients.Fields.SeName")]
        [AllowHtml]
        public string SeName { get; set; }

        [UIHint("Picture")]
        [LangResourceDisplayName("Admin.ArticleCategory.Clients.Fields.Picture")]
        public int PictureId { get; set; }


        [LangResourceDisplayName("Admin.ArticleCategory.Clients.Fields.Published")]
        public bool Published { get; set; }

        [LangResourceDisplayName("Admin.ArticleCategory.Clients.Fields.Deleted")]
        public bool Deleted { get; set; }

        [LangResourceDisplayName("Admin.ArticleCategory.Clients.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [LangResourceDisplayName("Common.CreatedOn")]
        public DateTime? CreatedOn { get; set; }

        [LangResourceDisplayName("Common.UpdatedOn")]
        public DateTime? UpdatedOn { get; set; }

        public IList<ClientLocalizedModel> Locales { get; set; }

        //Site mapping
        [LangResourceDisplayName("Admin.Common.Site.LimitedTo")]
        public bool LimitedToSites { get; set; }

        [LangResourceDisplayName("Admin.Common.Site.AvailableFor")]
        public List<SiteModel> AvailableSites { get; set; }
        public int[] SelectedSiteIds { get; set; }

        #region Nested classes





        #endregion
    }

    public class ClientLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [LangResourceDisplayName("Admin.ArticleCategory.Clients.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [LangResourceDisplayName("Admin.ArticleCategory.Clients.Fields.Description")]
        [AllowHtml]
        public string Description { get; set; }

        [LangResourceDisplayName("Admin.ArticleCategory.Clients.Fields.MetaKeywords")]
        [AllowHtml]
        public string MetaKeywords { get; set; }

        [LangResourceDisplayName("Admin.ArticleCategory.Clients.Fields.MetaDescription")]
        [AllowHtml]
        public string MetaDescription { get; set; }

        [LangResourceDisplayName("Admin.ArticleCategory.Clients.Fields.MetaTitle")]
        [AllowHtml]
        public string MetaTitle { get; set; }

        [LangResourceDisplayName("Admin.ArticleCategory.Clients.Fields.SeName")]
        [AllowHtml]
        public string SeName { get; set; }
        [LangResourceDisplayName("Common.CreatedOn")]
        public DateTime? CreatedOn { get; set; }

        [LangResourceDisplayName("Common.UpdatedOn")]
        public DateTime? UpdatedOn { get; set; }
    }
}