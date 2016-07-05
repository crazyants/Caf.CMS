
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Cms.Media;
using CAF.Infrastructure.Core.Domain.Sites;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;


namespace CAF.Infrastructure.Core.Domain.Cms.Articles
{
    /// <summary>
    /// 友情链接
    /// </summary>
     [Serializable]
    public partial class Link : AuditedBaseEntity, ISiteMappingSupported
    {
        /// <summary>
        /// 名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// 介绍
        /// </summary>
        [DataMember]
        public string Intro { get; set; }
        /// <summary>
        /// 链接
        /// </summary>
        [DataMember]
        public string LinkUrl { get; set; }
        /// <summary>
        /// Logo地址
        /// </summary>
        [DataMember]
        public string LogoUrl { get; set; }
        /// <summary>
        /// 排序数字
        /// </summary>
        [DataMember]
        public int SortId { get; set; }
        /// <summary>
        /// 是否显示首页
        /// </summary>
        [DataMember]
        public bool IsHome { get; set; }
        /// <summary>
        ///获取或设置图像标识符
        /// </summary>
        [DataMember]
        public int? PictureId { get; set; }
        /// <summary>
        /// Gets or sets the picture
        /// </summary>
        [DataMember]
        public virtual Picture Picture { get; set; }

        /// <summary>
        /// Gets or sets the language identifier
        /// </summary>
        public int LanguageId { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the entity is limited/restricted to certain stores
        /// </summary>
        public bool LimitedToSites { get; set; }
    }
}
