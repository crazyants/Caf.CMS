using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Localization;
using CAF.Infrastructure.Core.Domain.Sites;
using System;
using System.Collections.Generic;


namespace CAF.Infrastructure.Core.Domain.Cms.Polls
{
    /// <summary>
    /// Represents a poll
    /// </summary>
	public partial class Poll : BaseEntity, ISiteMappingSupported
    {
        private ICollection<PollAnswer> _pollAnswers;

        /// <summary>
        /// Gets or sets the language identifier
        /// </summary>
        public int LanguageId { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the system keyword
        /// </summary>
        public string SystemKeyword { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is published
        /// </summary>
        public bool Published { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity should be shown on home page
        /// </summary>
        public bool ShowOnHomePage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the anonymous votes are allowed
        /// </summary>
        public bool AllowGuestsToVote { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the poll start date and time
        /// </summary>
        public virtual DateTime? StartDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the poll end date and time
        /// </summary>
        public DateTime? EndDateUtc { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the entity is limited/restricted to certain Sites
		/// </summary>
		public bool LimitedToSites { get; set; }
        
        /// <summary>
        /// Gets or sets the news comments
        /// </summary>
        public virtual ICollection<PollAnswer> PollAnswers
        {
			get { return _pollAnswers ?? (_pollAnswers = new HashSet<PollAnswer>()); }
            protected set { _pollAnswers = value; }
        }
        
        /// <summary>
        /// Gets or sets the language
        /// </summary>
        public virtual Language Language { get; set; }
    }
}