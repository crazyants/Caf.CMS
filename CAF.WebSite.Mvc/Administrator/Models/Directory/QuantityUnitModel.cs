using System;
using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using  CAF.WebSite.Mvc.Admin.Validators.Directory;

using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI.Localization;
using CAF.WebSite.Application.WebUI;

namespace  CAF.WebSite.Mvc.Admin.Models.Directory
{
    public class QuantityUnitModel : EntityModelBase, ILocalizedModel<QuantityUnitLocalizedModel>
    {
        public QuantityUnitModel()
        {
            Locales = new List<QuantityUnitLocalizedModel>();
        }

        [LangResourceDisplayName("Admin.Configuration.QuantityUnit.Fields.Name")]
        public string Name { get; set; }

		[LangResourceDisplayName("Common.Description")]
        [AllowHtml]
        public string Description { get; set; }

		[LangResourceDisplayName("Common.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [LangResourceDisplayName("Admin.Configuration.QuantityUnit.Fields.IsDefault")]
        public bool IsDefault { get; set; }

        public IList<QuantityUnitLocalizedModel> Locales { get; set; }
    }

    public class QuantityUnitLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [LangResourceDisplayName("Admin.Configuration.QuantityUnit.Fields.Name")]
        public string Name { get; set; }

		[LangResourceDisplayName("Common.Description")]
        public string Description { get; set; }
    }
}