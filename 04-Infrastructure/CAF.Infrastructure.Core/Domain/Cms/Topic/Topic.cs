using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Sites;
using CAF.Infrastructure.Core.Domain.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using CAF.Infrastructure.Core.Domain.Seo;


namespace CAF.Infrastructure.Core.Domain.Cms.Topic
{
    /// <summary>
    /// Represents a topic
    /// </summary>
    public partial class Topic : AuditedBaseEntity, ILocalizedEntity, ISiteMappingSupported, ISlugSupported
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether this topic should be included in sitemap
        /// </summary>
        public bool IncludeInSitemap { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether this topic is password protected
        /// </summary>
        public bool IsPasswordProtected { get; set; }

        /// <summary>
        /// Gets or sets the password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the body
        /// </summary>
        public string Body { get; set; }

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
		/// Gets or sets a value indicating whether the entity is limited/restricted to certain stores
		/// </summary>
		public bool LimitedToSites { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the topic should also be rendered as a generic html widget
        /// </summary>
        public bool RenderAsWidget { get; set; }

        /// <summary>
        /// Gets or sets the widget zone name
        /// </summary>
        public string WidgetZone { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the title should be displayed in the widget block
        /// </summary>
        public bool WidgetShowTitle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the widget block should have borders
        /// </summary>
        public bool WidgetBordered { get; set; }

        /// <summary>
        /// Gets or sets the sort order (relevant for widgets)
        /// </summary>
        public int Priority { get; set; }
        /// <summary>
        /// Gets or sets the title tag
        /// </summary>
        public string TitleTag { get; set; }

        /// <summary>
        /// Gets or sets a value of used article template identifier
        /// </summary>
        [DataMember]
        public int TopicTemplateId { get; set; }

        /// <summary>
        /// Helper function which gets the comma-separated <c>WidgetZone</c> property as list of strings
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetWidgetZones()
        {
            if (this.WidgetZone.IsEmpty())
            {
                return Enumerable.Empty<string>();
            }

            return this.WidgetZone.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim());
        }
 
    }
}
