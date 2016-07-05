using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;


namespace CAF.Infrastructure.Core.Domain.Cms.Articles
{
    /// <summary>
    /// 文章评论
    /// </summary>
    [Serializable]
    public partial class ArticleComment : UserContent
    {
        /// <summary>
        /// 主表ID
        /// </summary>
        [DataMember]
        public int ArticleId { get; set; }
        public virtual Article Article { get; set; }
        /// <summary>
        /// 父评论ID
        /// </summary>
        [DataMember]
        public int ParentId { get; set; }
        /// <summary>
        /// Gets or sets the comment title
        /// </summary>
        public string CommentTitle { get; set; }
        /// <summary>
        /// 评论内容
        /// </summary>
        [DataMember]
        public string CommentText { get; set; }


        /// <summary>
        /// 发表时间
        /// </summary>
        [DataMember]
        public DateTime AddTime { get; set; }
        /// <summary>
        /// 是否已答复
        /// </summary>
        [DataMember]
        public bool IsReply { get; set; }
        /// <summary>
        /// 答复内容
        /// </summary>
        [DataMember]
        public string ReplyContent { get; set; }
        /// <summary>
        /// 回复时间
        /// </summary>
        [DataMember]
        public DateTime? ReplyTime { get; set; }
    }
}
