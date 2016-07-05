using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Localization;
using CAF.WebSite.Mvc.Admin.Validators.ExtendAttributes;


namespace CAF.WebSite.Mvc.Admin.Models.ExtendedAttributes
{
    [Validator(typeof(ExtendedAttributeValueValidator))]
    public class ExtendedAttributeValueModel : EntityModelBase, ILocalizedModel<ExtendedAttributeValueLocalizedModel>
    {
        public ExtendedAttributeValueModel()
        {
            Locales = new List<ExtendedAttributeValueLocalizedModel>();
        }

        public int ExtendedAttributeId { get; set; }

        [LangResourceDisplayName("Admin.Catalog.Attributes.ExtendedAttributes.Values.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [LangResourceDisplayName("Admin.Catalog.Attributes.ExtendedAttributes.Values.Fields.IsPreSelected")]
        public bool IsPreSelected { get; set; }

        [LangResourceDisplayName("Admin.Catalog.Attributes.ExtendedAttributes.Values.Fields.DisplayOrder")]
        public int DisplayOrder {get;set;}

        public IList<ExtendedAttributeValueLocalizedModel> Locales { get; set; }

    }

    public class ExtendedAttributeValueLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [LangResourceDisplayName("Admin.Catalog.Attributes.ExtendedAttributes.Values.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }
    }
}