using System;
using System.Runtime.Serialization;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Auditing;
using System.ComponentModel.DataAnnotations.Schema;
namespace CAF.Infrastructure.Core.Domain.Cms.Media
{
    /// <summary>
    /// Represents a download
    /// </summary>
    [DataContract]
    public partial class Download : BaseEntity 
    {
        public Download()
        {
            this.UpdatedOnUtc = DateTime.UtcNow;
        }
        /// <summary>
        /// Gets a sets a GUID
        /// </summary>
		[DataMember]
		public Guid DownloadGuid { get; set; }

        /// <summary>
        /// Gets a sets a value indicating whether DownloadUrl property should be used
        /// </summary>
		[DataMember]
		public bool UseDownloadUrl { get; set; }

        /// <summary>
        /// Gets a sets a download URL
        /// </summary>
		[DataMember]
		public string DownloadUrl { get; set; }

        /// <summary>
        /// Gets or sets the download binary
        /// </summary>
        public byte[] DownloadBinary { get; set; }

        /// <summary>
        /// The mime-type of the download
        /// </summary>
		[DataMember]
		public string ContentType { get; set; }

        /// <summary>
        /// The filename of the download
        /// </summary>
		[DataMember]
		public string Filename { get; set; }

        /// <summary>
        /// Gets or sets the extension
        /// </summary>
		[DataMember]
		public string Extension { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the download is new
        /// </summary>
		[DataMember]
		public bool IsNew { get; set; }

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
