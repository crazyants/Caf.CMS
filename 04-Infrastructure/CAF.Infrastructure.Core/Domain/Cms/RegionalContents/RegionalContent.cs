
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Sites;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;


namespace CAF.Infrastructure.Core.Domain.Cms.RegionalContents
{
    /// <summary>
    /// 系统区域内容
    /// </summary>
    [Serializable]
    public partial class RegionalContent : AuditedBaseEntity, ISiteMappingSupported
    {
        /// <summary>
        /// 系统名称
        /// </summary>
        [DataMember]
        public string SystemName { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }
      
        /// <summary>
        /// 排序数字
        /// </summary>
        [DataMember]
        public int DisplayOrder { get; set; }
        /// <summary>
        /// 详细
        /// </summary>
        [DataMember]
        public string Body { get; set; }
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
