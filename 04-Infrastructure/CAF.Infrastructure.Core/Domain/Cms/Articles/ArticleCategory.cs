using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Cms.Media;
using CAF.Infrastructure.Core.Domain.Security;
using CAF.Infrastructure.Core.Domain.Sites;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using CAF.Infrastructure.Core.Domain.Localization;
using CAF.Infrastructure.Core.Domain.Seo;
using CAF.Infrastructure.Core.Domain.Cms.Channels;

namespace CAF.Infrastructure.Core.Domain.Cms.Articles
{
    /// <summary>
    /// 内容类别
    /// </summary>
    [DataContract]
    //[DebuggerDisplay("{Id}: {Name} (Parent: {ParentCategoryId})")]
    public partial class ArticleCategory : AuditedBaseEntity, ISoftDeletable, IAclSupported, ISiteMappingSupported, ILocalizedEntity, ISlugSupported
    {
         
        /// <summary>
        /// 类别标题
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// 别名
        /// </summary>
        [DataMember]
        public string Alias { get; set; }
        /// <summary>
        /// 全名
        /// </summary>
        [DataMember]
        public string FullName { get; set; }
        /// <summary>
        /// 父类别ID
        /// </summary>
        [DataMember]
        public int ParentCategoryId { get; set; }

        /// <summary>
        /// 排序数字
        /// </summary>
        [DataMember]
        public int DisplayOrder { get; set; }
        /// <summary>
        /// URL跳转地址
        /// </summary>
        [DataMember]
        public string LinkUrl { get; set; }
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
        /// 详细
        /// </summary>
        [DataMember]
        public string Description { get; set; }
        /// <summary>
        /// 获取或设置一个描述显示类别页面的底部
        /// </summary>
        [DataMember]
        public string BottomDescription { get; set; }
        /// <summary>
        /// SEO标题
        /// </summary>
        [DataMember]
        public string MetaTitle { get; set; }
        /// <summary>
        /// SEO关健字
        /// </summary>
        [DataMember]
        public string MetaKeywords { get; set; }
        /// <summary>
        /// SEO描述
        /// </summary>
        [DataMember]
        public string MetaDescription { get; set; }

        /// <summary>
        /// 获取或设置一个值,该值指示该实体是否发表
        /// </summary>
        [DataMember]
        public bool Published { get; set; }
        /// <summary>
        /// 获取或设置一个值,该值指是否显示在主页上的类别
        /// </summary>
        [DataMember]
        public bool ShowOnHomePage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is subject to ACL
        /// </summary>
        [DataMember]
        public bool SubjectToAcl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is limited/restricted to certain stores
        /// </summary>
        [DataMember]
        public bool LimitedToSites { get; set; }

        /// <summary>
        /// Gets or sets a value of used category template identifier
        /// </summary>
        [DataMember]
        public int ModelTemplateId { get; set; }
        /// <summary>
        /// Gets or sets a value of used category template identifier
        /// </summary>
        [DataMember]
        public int DetailModelTemplateId { get; set; }
        /// <summary>
        /// Gets or sets a value of used category channel identifier
        /// </summary>
        [DataMember]
        public int ChannelId { get; set; }
        public virtual Channel Channel { get; set; }
        /// <summary>
        /// Gets or sets the date and time of instance update
        /// </summary>
        [DataMember]
        public string DefaultViewMode { get; set; }

        /// <summary>
        /// Gets or sets the page size
        /// </summary>
        [DataMember]
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customers can select the page size
        /// </summary>
        [DataMember]
        public bool AllowUsersToSelectPageSize { get; set; }

        /// <summary>
        /// Gets or sets the available customer selectable page size options
        /// </summary>
        [DataMember]
        public string PageSizeOptions { get; set; }

        /// <summary>
        /// 文章列表
        /// </summary>
        private ICollection<Article> _articles;
        /// <summary>
        /// 文章列表
        /// </summary>
        public virtual ICollection<Article> Articles
        {
            get { return _articles ?? (_articles = new HashSet<Article>()); }
            protected set { _articles = value; }
        }


        /// <summary>
        /// 文章扩展
        /// </summary>
        private ICollection<ArticleCategoryExtend> _articleCategoryExtends;
        /// <summary>
        /// 文章扩展
        /// </summary>
        public virtual ICollection<ArticleCategoryExtend> ArticleCategoryExtends
        {
            get { return _articleCategoryExtends ?? (_articleCategoryExtends = new HashSet<ArticleCategoryExtend>()); }
            protected set { _articleCategoryExtends = value; }
        }
    }
}
