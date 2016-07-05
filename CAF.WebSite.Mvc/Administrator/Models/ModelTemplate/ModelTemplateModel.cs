using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using CAF.WebSite.Application.WebUI.Localization;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Mvc.Admin.Models.Sites;
using CAF.WebSite.Mvc.Admin.Validators.Users;
using CAF.WebSite.Mvc.Admin.Validators.ModelTemplate;

namespace CAF.WebSite.Mvc.Admin.Models.ModelTemplates
{
    [Validator(typeof(ModelTemplateValidator))]
    public class ModelTemplateModel : EntityModelBase
    {

        public ModelTemplateModel()
        {


        }

        [LangResourceDisplayName("Admin.ContentManagement.ModelTemplates.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.ModelTemplates.Fields.ViewPath")]
        [AllowHtml]
        public string ViewPath { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.ModelTemplates.Fields.DisplayOrder")]
        [AllowHtml]
        public int DisplayOrder { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.ModelTemplates.Fields.TemplageType")]
        public int TemplageTypeId { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.ModelTemplates.Fields.TemplageType")]
        public string TemplageTypeName { get; set; }
        public string TemplageTypeLabelHint { get; set; }
     
    }
     
}