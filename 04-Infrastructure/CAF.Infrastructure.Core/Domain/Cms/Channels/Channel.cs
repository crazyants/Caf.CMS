
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;


namespace CAF.Infrastructure.Core.Domain.Cms.Channels
{
    /// <summary>
    /// 系统频道表
    /// </summary>
    [Serializable]
    public partial class Channel : AuditedBaseEntity
    {
        /// <summary>
        ///系统频道表主体ID
        /// </summary>
        [DataMember]
        public Guid ChannelGuid { get; set; }


        /// <summary>
        ///频道名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        ///频道标题
        /// </summary>
        [DataMember]
        public string Title { get; set; }
        
        /// <summary>
        ///排序数字
        /// </summary>
        [DataMember]
        public int DisplayOrder { get; set; }
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
        /// 扩展属性
        /// </summary>
        private ICollection<ExtendedAttribute> _extendedAttributes;
        /// <summary>
        /// 扩展属性
        /// </summary>
        [DataMember]
        public virtual ICollection<ExtendedAttribute> ExtendedAttributes
        {
            get { return _extendedAttributes ?? (_extendedAttributes = new HashSet<ExtendedAttribute>()); }
            protected set { _extendedAttributes = value; }
        }
        /// <summary>
        /// 内容类别
        /// </summary>
        private ICollection<ArticleCategory> _articleCategory;
        /// <summary>
        /// 类别
        /// </summary>
        public virtual ICollection<ArticleCategory> ArticleCategorys
        {
            get { return _articleCategory ?? (_articleCategory = new HashSet<ArticleCategory>()); }
            protected set { _articleCategory = value; }
        }
    }
}
