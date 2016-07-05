
using CAF.Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;


namespace CAF.Infrastructure.Core.Domain.Cms.Articles
{
    /// <summary>
    /// 附件表
    /// </summary>
     [Serializable]
    public partial class ArticleAttach : BaseEntity
    {
        /// <summary>
        /// 文章ID
        /// </summary>
         [DataMember]
         public int ArticleId { get; set; }
         public Article Article { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
         [DataMember]
         public string FileName { get; set; }
        /// <summary>
        /// 文件路径
        /// </summary>
         [DataMember]
         public string FilePath { get; set; }
        /// <summary>
        /// 文件大小(字节)
        /// </summary>
         [DataMember]
         public int FileSize { get; set; }
        /// <summary>
        /// 文件扩展名
        /// </summary>
         [DataMember]
         public string FileExt { get; set; }
        /// <summary>
        /// 下载次数
        /// </summary>
         [DataMember]
         public int DownNum { get; set; }
        /// <summary>
        /// 积分(正赠送负消费)
        /// </summary>
         [DataMember]
         public int Point { get; set; }
        /// <summary>
        /// 上传时间
        /// </summary>
         [DataMember]
         public DateTime AddTime { get; set; }
    }
}
