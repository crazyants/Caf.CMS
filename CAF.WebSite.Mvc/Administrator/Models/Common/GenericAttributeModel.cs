using System.Web.Mvc;
using FluentValidation.Attributes;
using CAF.WebSite.Mvc.Admin.Validators.Localization;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI;
 

namespace CAF.WebSite.Mvc.Admin.Models.Common
{
    [Validator(typeof(GenericAttributeValidator))]
    public partial class GenericAttributeModel : EntityModelBase
    {
        [LangResourceDisplayName("Admin.Common.GenericAttributes.Fields.Name")]
        public string Key { get; set; }

        [AllowHtml]
        [LangResourceDisplayName("Admin.Common.GenericAttributes.Fields.Value")]
        public string Value { get; set; }

        public string EntityName { get; set; }

        public int EntityId { get; set; }
    }
}