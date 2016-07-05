
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;


namespace CAF.Infrastructure.Core.Domain.Cms.Articles
{
    /// <summary>
    /// 标签
    /// </summary>
    [Serializable]
    public partial class ArticleTag : BaseEntity, ILocalizedEntity
    {
        private ICollection<Article> _articles;
        /// <summary>
        /// 名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// 是否系统标签
        /// </summary>
        [DataMember]
        public bool IsSys { get; set; }
        /// <summary>
        /// Gets or sets the articles
        /// </summary>
        public virtual ICollection<Article> Articles
        {
            get { return _articles ?? (_articles = new HashSet<Article>()); }
            protected set { _articles = value; }
        }
    }
}
