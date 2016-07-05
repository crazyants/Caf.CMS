
using CAF.Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;


namespace CAF.WebSite.Domain.Cms.Channels
{
    /// <summary>
    /// 频道分类表
    /// </summary>
     [Serializable]
    public partial class ChannelCategory : BaseEntity
    {
         /// <summary>
         /// 频道列表
         /// </summary>
         private ICollection<Channel> _channels;
        /// <summary>
        /// 频道分类主体ID
        /// </summary>
        [DataMember]
        public Guid ChannelCategoryGuid { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// 生成文件夹名称
        /// </summary>
        [DataMember]
        public string BuildPath { get; set; }
        /// <summary>
        /// 绑定域名
        /// </summary>
        [DataMember]
        public string Domain { get; set; }
        /// <summary>
        /// 是否默认
        /// </summary>
        [DataMember]
        public bool IsDefault { get; set; }
        /// <summary>
        /// 排序数字
        /// </summary>
        [DataMember]
        public int SortId { get; set; }
        /// <summary>
        /// 频道列表
        /// </summary>
        public virtual ICollection<Channel> Channels
        {
            get { return _channels ?? (_channels = new HashSet<Channel>()); }
            protected set { _channels = value; }
        }
    }
}
