
using CAF.Infrastructure.Core;
using CAF.Mvc.JQuery.Datatables;
using CAF.Mvc.JQuery.Datatables.Models;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Mvc.Admin.Validators.Channels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using FluentValidation.Attributes;
using CAF.WebSite.Application.WebUI;
using CAF.Mvc.JQuery.Datatables.Core;

namespace CAF.WebSite.Mvc.Admin.Models.Channels
{
    /// <summary>
    /// 频道分类表
    /// </summary>
    [Serializable]
    [Validator(typeof(ChannelCategoryValidator))]
    public partial class ChannelCategoryModel : EntityModelBase
    {
        /// <summary>
        /// 标题
        /// </summary>
        [LangResourceDisplayName("Admin.Channel.ChannelCategory.Fields.Title")]
        public string Title { get; set; }

        /// <summary>
        /// 生成文件夹名称
        /// </summary>
        [LangResourceDisplayName("Admin.Channel.ChannelCategory.Fields.BuildPath")]
        public string BuildPath { get; set; }
        /// <summary>
        /// 绑定域名
        /// </summary>
        [LangResourceDisplayName("Admin.Channel.ChannelCategory.Fields.Domain")]
        public string Domain { get; set; }
        /// <summary>
        /// 是否默认
        /// </summary>
        [LangResourceDisplayName("Admin.Channel.ChannelCategory.Fields.IsDefault")]
        public bool IsDefault { get; set; }
        /// <summary>
        /// 排序数字
        /// </summary>
        [LangResourceDisplayName("Admin.Channel.ChannelCategory.Fields.SortId")]
        public int SortId { get; set; }
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //public DateTime StartDate { get; set; }
        public bool DisplayPdfExport { get; set; }
    }

}
