
using CAF.Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;


namespace CAF.Infrastructure.Core.Domain.Cms.Articles
{
    /// <summary>
    /// 反馈
    /// </summary>
    [Serializable]
    public partial class Feedback : BaseEntity
    {
        /// <summary>
        /// 留言标题
        /// </summary>
        [DataMember]
        public string Title { get; set; }
        /// <summary>
        /// 留言内容
        /// </summary>
        [DataMember]
        public string Content { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        [DataMember]
        public string UserName { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        [DataMember]
        public string UserTel { get; set; }
        /// <summary>
        /// 联系QQ
        /// </summary>
        [DataMember]
        public string UserQQ { get; set; }
        /// <summary>
        /// 电子邮箱
        /// </summary>
        [DataMember]
        public string UserEmail { get; set; }
        /// <summary>
        /// 留言时间
        /// </summary>
        [DataMember]
        public DateTime AddTime { get; set; }
        /// <summary>
        /// 回复内容
        /// </summary>
        [DataMember]
        public string ReplyContent { get; set; }
        /// <summary>
        /// 回复时间
        /// </summary>
        [DataMember]
        public DateTime ReplyTime { get; set; }
        /// <summary>
        /// 是否锁定1是0否
        /// </summary>
        [DataMember]
        public bool IsLock { get; set; }
        /// <summary>
        /// 发布者IP地址
        /// </summary>
        [DataMember]
        public string IPAddress { get; set; }
        /// <summary>
        /// 是否审核
        /// </summary>
        [DataMember]
        public bool IsPass { get; set; }


    }
}
