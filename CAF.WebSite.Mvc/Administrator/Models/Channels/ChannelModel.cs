
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using FluentValidation.Attributes;
using CAF.WebSite.Mvc.Admin.Validators.Channels;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI;
using CAF.Mvc.JQuery.Datatables;
using CAF.Mvc.JQuery.Datatables.Models;
using System.Web.Mvc;
using CAF.Mvc.JQuery.Datatables.Core;

namespace CAF.WebSite.Mvc.Admin.Models.Channels
{
    /// <summary>
    /// 系统频道表
    /// </summary>
    [Serializable]
    [Validator(typeof(ChannelValidator))]
    public partial class ChannelModel : EntityModelBase
    {
        public ChannelModel()
        {
            AvailableExtendedAttributes = new List<SelectListItem>();
            AvailableModelTemplates = new List<SelectListItem>();
            AvailableDetailModelTemplates = new List<SelectListItem>();
        }

        /// <summary>
        ///频道名称
        /// </summary>
        [DataMember]
        [LangResourceDisplayName("Admin.ContentManagement.Channels.Fields.Name")]
        public string Name { get; set; }

        /// <summary>
        ///频道标题
        /// </summary>
        [DataMember]
        [LangResourceDisplayName("Admin.ContentManagement.Channels.Fields.Title")]
        public string Title { get; set; }
        
        /// <summary>
        ///排序数字
        /// </summary>
        [DataMember]
        [LangResourceDisplayName("Admin.ContentManagement.Channels.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Channels.Fields.ExtendedAttributes")]
        [AllowHtml]
        public string ExtendedAttributes { get; set; }
        public IList<SelectListItem> AvailableExtendedAttributes { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.ArticleCategory.Fields.ModelTemplate")]
        [AllowHtml]
        public int ModelTemplateId { get; set; }
        public IList<SelectListItem> AvailableModelTemplates { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.ArticleCategory.Fields.ModelDetailTemplate")]
        [AllowHtml]
        public int DetailModelTemplateId { get; set; }
        public IList<SelectListItem> AvailableDetailModelTemplates { get; set; }

    }
}
