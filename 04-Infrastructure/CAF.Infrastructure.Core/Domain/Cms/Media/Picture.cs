using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Auditing;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System;
//using CAF.Infrastructure.Core.Domain.Shop.Catalog;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace CAF.Infrastructure.Core.Domain.Cms.Media
{
    /// <summary>
    /// Represents a picture
    /// </summary>
    [DataContract]
    public partial class Picture : BaseEntity 
    {
        //  private ICollection<ProductPicture> _productPictures;
        private ICollection<ArticleAlbum> _articleAlbums;
        /// <summary>
        /// Gets or sets the picture binary
        /// </summary>
        public byte[] PictureBinary { get; set; }

        /// <summary>
        /// Gets or sets the picture mime type
        /// </summary>
        [DataMember]
        public string MimeType { get; set; }

        /// <summary>
        /// Gets or sets the SEO friednly filename of the picture
        /// </summary>
        [DataMember]
        public string SeoFilename { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the picture is new
        /// </summary>
        [DataMember]
        public bool IsNew { get; set; }
        /// <summary>
        /// Gets or sets the product pictures
        /// </summary>
        [DataMember]
        public virtual ICollection<ArticleAlbum> ArticleAlbum
        {
            get { return _articleAlbums ?? (_articleAlbums = new HashSet<ArticleAlbum>()); }
            protected set { _articleAlbums = value; }
        }
        /// <summary>
        /// Gets or sets the product pictures
        /// </summary>
        // [DataMember]
        //public virtual ICollection<ProductPicture> ProductPictures
        //{
        //     get { return _productPictures ?? (_productPictures = new HashSet<ProductPicture>()); }
        //     protected set { _productPictures = value; }
        //  }
       

        /// <summary>
        /// Gets or sets a value indicating whether the entity transient/preliminary
        /// </summary>
        [DataMember]
        [Index("IX_UpdatedOn_IsTransient", 1)]
        public bool IsTransient { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance update
        /// </summary>
        [DataMember]
        [Index("IX_UpdatedOn_IsTransient", 0)]
        public DateTime UpdatedOnUtc { get; set; }

    }
}
