
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Cms.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;


namespace CAF.Infrastructure.Core.Domain.Cms.Articles
{
    /// <summary>
    /// 图片相册
    /// </summary>
     [Serializable]
    public partial class ArticleAlbum : BaseEntity
    {

        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        [DataMember]
         public int ArticleId { get; set; }

        /// <summary>
        /// Gets or sets the picture identifier
        /// </summary>
        [DataMember]
        public int PictureId { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        [DataMember]
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets the picture
        /// </summary>
        public virtual Picture Picture { get; set; }

        /// <summary>
        /// Gets the product
        /// </summary>
        public virtual Article Article { get; set; }
    }
}
