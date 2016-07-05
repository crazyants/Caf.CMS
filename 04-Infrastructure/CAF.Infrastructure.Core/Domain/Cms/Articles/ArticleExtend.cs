
using CAF.Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;


namespace CAF.Infrastructure.Core.Domain.Cms.Articles
{
    /// <summary>
    /// 文章扩展信息
    /// </summary>
    [Serializable]
    public partial class ArticleExtend : BaseEntity
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// 字段值
        /// </summary>
        [DataMember]
        public string Value { get; set; }
        /// <summary>
        /// 字段类型
        /// </summary>
        [DataMember]
        public string Type { get; set; }
        /// <summary>
        /// 排序数字
        /// </summary>
        [DataMember]
        public int SortId { get; set; }
        /// <summary>
        /// 文章ID
        /// </summary>
        [DataMember]
        public int ArticleId { get; set; }
        public Article Article { get; set; }
    }
}
