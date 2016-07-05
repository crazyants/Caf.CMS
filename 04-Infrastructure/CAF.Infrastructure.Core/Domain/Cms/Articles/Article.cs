using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Cms.Media;
using CAF.Infrastructure.Core.Domain.Localization;
using CAF.Infrastructure.Core.Domain.Security;
using CAF.Infrastructure.Core.Domain.Seo;
using CAF.Infrastructure.Core.Domain.Sites;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;


namespace CAF.Infrastructure.Core.Domain.Cms.Articles
{
    /// <summary>
    /// 文章内容实体类
    /// </summary>
    [Serializable]
    public partial class Article : AuditedBaseEntity, ILocalizedEntity, ISlugSupported, ISoftDeletable, IAclSupported, ISiteMappingSupported
    {
        /// <summary>
        ///文章主体ID
        /// </summary>
        [DataMember]
        public Guid ArticleGuid { get; set; }
        /// <summary>
        /// 类别ID
        /// </summary>
        [DataMember]
        public int CategoryId { get; set; }
        public ArticleCategory ArticleCategory { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        [DataMember]
        public string Title { get; set; }
        /// <summary>
        /// 获取或设置值指示是否这个话题是密码保护
        /// </summary>
        public bool IsPasswordProtected { get; set; }
        /// <summary>
        /// 获取或设置密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 外部链接
        /// </summary>
        [DataMember]
        public string LinkUrl { get; set; }
        /// <summary>
        /// 关联图片地址
        /// </summary>
        [DataMember]
        public string ImgUrl { get; set; }
        /// <summary>
        ///获取或设置图像标识符
        /// </summary>
        [DataMember]
        public int? PictureId { get; set; }
        /// <summary>
        /// Gets or sets the picture
        /// </summary>
        [DataMember]
        public virtual Picture Picture { get; set; }
        /// <summary>
        /// Gets or sets the meta keywords
        /// </summary>
        public string MetaKeywords { get; set; }

        /// <summary>
        /// Gets or sets the meta description
        /// </summary>
        public string MetaDescription { get; set; }

        /// <summary>
        /// Gets or sets the meta title
        /// </summary>
        public string MetaTitle { get; set; }
        /// <summary>
        /// 内容摘要
        /// </summary>
        [DataMember]
        public string ShortContent { get; set; }
        /// <summary>
        /// 详细内容
        /// </summary>
        [DataMember]
        public string FullContent { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [DataMember]
        public int DisplayOrder { get; set; }
        /// <summary>
        /// 浏览次数
        /// </summary>
        [DataMember]
        public int Click { get; set; }
        /// <summary>
        /// 状态 0正常1未审核2锁定
        public int StatusId { get; set; }
        /// <summary>
        /// Gets or sets the product type
        /// </summary>
        [DataMember]
        public ArticleStatus ArticleStatus
        {
            get
            {
                return (ArticleStatus)this.StatusId;
            }
            set
            {
                this.StatusId = (int)value;
            }
        }
        /// <summary>
        /// Gets or sets the password format
        /// </summary>
        public StatusFormat StatusFormat
        {
            get { return (StatusFormat)StatusId; }
            set { this.StatusId = (int)value; }
        }
        /// <summary>
        /// 阅读权限
        /// </summary>
        public string GroupidsView { get; set; }
        /// <summary>
        /// 关联投票ID
        /// </summary>
        [DataMember]
        public int VoteId { get; set; }
        /// <summary>
        /// 是否置顶
        /// </summary>
        [DataMember]
        public bool IsTop { get; set; }
        /// <summary>
        /// 是否推荐
        /// </summary>
        [DataMember]
        public bool IsRed { get; set; }
        /// <summary>
        /// 是否热门
        /// </summary>
        [DataMember]
        public bool IsHot { get; set; }
        /// <summary>
        /// 是否幻灯片
        /// </summary>
        [DataMember]
        public bool IsSlide { get; set; }
        /// <summary>
        /// 是否管理员发布0不是1是
        /// </summary>
        [DataMember]
        public bool IsSys { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        [DataMember]
        public string Author  { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the article is download
        /// </summary>
        [DataMember]
        public bool IsDownload { get; set; }

        /// <summary>
        /// Gets or sets the download identifier
        /// </summary>
        [DataMember]
        public int DownloadId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this downloadable article can be downloaded unlimited number of times
        /// </summary>
        [DataMember]
        public bool UnlimitedDownloads { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of downloads
        /// </summary>
        [DataMember]
        public int MaxNumberOfDownloads { get; set; }
        /// <summary>
        /// Gets or sets the download count
        /// </summary>
        [DataMember]
        public int DownloadCount { get; set; }

        /// <summary>
        /// 是否允许评论
        /// </summary>
        public bool AllowComments { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the article allows customer reviews
        /// </summary>
        [DataMember]
        public bool AllowUserReviews { get; set; }

        /// <summary>
        /// Gets or sets the rating sum (approved reviews)
        /// </summary>
        [DataMember]
        public int ApprovedRatingSum { get; set; }

        /// <summary>
        /// Gets or sets the rating sum (not approved reviews)
        /// </summary>
        [DataMember]
        public int NotApprovedRatingSum { get; set; }

        /// <summary>
        /// Gets or sets the total rating votes (approved reviews)
        /// </summary>
        [DataMember]
        public int ApprovedTotalReviews { get; set; }

        /// <summary>
        /// Gets or sets the total rating votes (not approved reviews)
        /// </summary>
        [DataMember]
        public int NotApprovedTotalReviews { get; set; }

        /// <summary>
        /// Gets or sets the total number of approved comments
        /// <remarks>The same as if we run newsItem.NewsComments.Where(n => n.IsApproved).Count()
        /// We use this property for performance optimization (no SQL command executed)
        /// </remarks>
        /// </summary>
        public int ApprovedCommentCount { get; set; }
        /// <summary>
        /// Gets or sets the total number of not approved comments
        /// <remarks>The same as if we run newsItem.NewsComments.Where(n => !n.IsApproved).Count()
        /// We use this property for performance optimization (no SQL command executed)
        /// </remarks>
        /// </summary>
        public int NotApprovedCommentCount { get; set; }

        /// <summary>
        /// Gets or sets a value of used article template identifier
        /// </summary>
        [DataMember]
        public int ModelTemplateId { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the entity has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is limited/restricted to certain stores
        /// </summary>
        public bool LimitedToSites { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is subject to ACL
        /// </summary>
        [DataMember]
        public bool SubjectToAcl { get; set; }

        /// <summary>
        /// Gets or sets the blog post start date and time
        /// </summary>
        public DateTime? StartDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the blog post end date and time
        /// </summary>
        public DateTime? EndDateUtc { get; set; }

        /// <summary>
        /// Review rating
        /// </summary>
        public int Rating { get; set; }

        /// <summary>
        /// Review helpful votes total
        /// </summary>
        public int HelpfulYesTotal { get; set; }

        /// <summary>
        /// Review not helpful votes total
        /// </summary>
        public int HelpfulNoTotal { get; set; }

        /// <summary>
        /// 评论
        /// </summary>
        private ICollection<ArticleComment> _articleComments;
        /// <summary>
        /// 扩展字段字典
        /// </summary>
        private ICollection<ArticleExtend> _articleExtends;
        /// <summary>
        /// 图片相册
        /// </summary>
        private ICollection<ArticleAlbum> _articleAlbums;
        /// <summary>
        /// 内容附件
        /// </summary>
        private ICollection<ArticleAttach> _articleAttachs;
        /// <summary>
        /// 标签
        /// </summary>
        private ICollection<ArticleTag> _articleTags;
        /// <summary>
        /// 赞
        /// </summary>
        private ICollection<ArticleReview> _articleReviews;
        /// <summary>
        /// 评论
        /// </summary>
        public virtual ICollection<ArticleComment> ArticleComments
        {
            get { return _articleComments ?? (_articleComments = new HashSet<ArticleComment>()); }
            protected set { _articleComments = value; }
        }
        /// <summary>
        /// 扩展字段字典
        /// </summary>
        public virtual ICollection<ArticleExtend> ArticleExtends
        {
            get { return _articleExtends ?? (_articleExtends = new HashSet<ArticleExtend>()); }
            protected set { _articleExtends = value; }
        }
        /// <summary>
        /// 图片相册
        /// </summary>
        public virtual ICollection<ArticleAlbum> ArticleAlbum
        {
            get { return _articleAlbums ?? (_articleAlbums = new HashSet<ArticleAlbum>()); }
            protected set { _articleAlbums = value; }
        }
        /// <summary>
        /// 内容附件
        /// </summary>
        public virtual ICollection<ArticleAttach> ArticleAttachs
        {
            get { return _articleAttachs ?? (_articleAttachs = new HashSet<ArticleAttach>()); }
            protected set { _articleAttachs = value; }
        }
        /// <summary>
        /// Gets or sets the article tags
        /// </summary>
        [DataMember]
        public virtual ICollection<ArticleTag> ArticleTags
        {
            get { return _articleTags ?? (_articleTags = new HashSet<ArticleTag>()); }
            protected set { _articleTags = value; }
        }

        /// <summary>
        /// Gets or sets the collection of article reviews
        /// </summary>
        public virtual ICollection<ArticleReview> ArticleReviews
        {
            get { return _articleReviews ?? (_articleReviews = new HashSet<ArticleReview>()); }
            protected set { _articleReviews = value; }
        }

    }
}
