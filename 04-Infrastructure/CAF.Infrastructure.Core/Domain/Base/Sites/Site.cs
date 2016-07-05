
using CAF.Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace CAF.Infrastructure.Core.Domain.Sites
{
    /// <summary>
    /// Represents a User
    /// </summary>
    [DataContract]
    public partial class Site : AuditedBaseEntity, ISoftDeletable
    {

        public Site()
        {
            AllowDelete = true;
            AllowEdit = true;
        }
        /// <summary>
        /// 系统唯一标识
        /// </summary>
        public string SiteKey { get; set; }
        /// <summary>
        /// 系统编号
        /// </summary>
        public virtual string Code { get; set; }

        /// <summary>
        /// 系统名称
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public virtual string Email { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public virtual string Tel { get; set; }
        /// <summary>
        /// 管理人
        /// </summary>
        public virtual string Manager { get; set; }
        public string Url { get; set; }
        public string Hosts { get; set; }
        /// <summary>
        /// Logo图片ID
        /// </summary>
        [DataMember]
        public int LogoPictureId { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether SSL is enabled
        /// </summary>
        [DataMember]
        public bool SslEnabled { get; set; }

        /// <summary>
        /// Gets or sets the store secure URL (HTTPS)
        /// </summary>
        [DataMember]
        public string SecureUrl { get; set; }
        /// <summary>
        /// Gets or sets the CDN host name, if static media content should be served through a CDN.
        /// </summary>
        public string ContentDeliveryNetwork { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public virtual string Icon { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        public virtual bool Enabled { get; set; }

        /// <summary>
        /// 允许编辑
        /// </summary>
        public virtual bool AllowEdit { get; set; }

        /// <summary>
        /// 允许删除
        /// </summary>
        public virtual bool AllowDelete { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public virtual int DisplayOrder { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        [DataMember]
        public string HtmlBodyId { get; set; }
        /// <summary>
        /// Gets the security mode for the store
        /// </summary>
        public HttpSecurityMode GetSecurityMode(bool? useSsl = null)
        {
            if (useSsl ?? SslEnabled)
            {
                if (SecureUrl.HasValue() && Url.HasValue() && !Url.StartsWith("https"))
                {
                    return HttpSecurityMode.SharedSsl;
                }
                else
                {
                    return HttpSecurityMode.Ssl;
                }
            }
            return HttpSecurityMode.Unsecured;
        }
    }
}
