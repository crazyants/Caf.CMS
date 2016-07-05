using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI.Localization;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Mvc.Admin.Validators.Articles;
 
 

namespace CAF.WebSite.Mvc.Admin.Models.Articles
{
    [Validator(typeof(ArticleTagValidator))]
    public class ArticleTagModel : EntityModelBase, ILocalizedModel<ArticleTagLocalizedModel>
    {
        public ArticleTagModel()
        {
            Locales = new List<ArticleTagLocalizedModel>();
        }
        [LangResourceDisplayName("Admin.Catalog.ArticleTags.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [LangResourceDisplayName("Admin.Catalog.ArticleTags.Fields.ArticleCount")]
        public int ArticleCount { get; set; }

        public IList<ArticleTagLocalizedModel> Locales { get; set; }
    }

    public class ArticleTagLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [LangResourceDisplayName("Admin.Catalog.ArticleTags.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }
    }
}