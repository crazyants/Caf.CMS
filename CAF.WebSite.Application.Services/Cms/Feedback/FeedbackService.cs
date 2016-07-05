using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core.Domain.Sites;
using System;
using System.Collections.Generic;
using System.Linq;
using CAF.Infrastructure.Core.Domain.Cms.Articles;


namespace CAF.WebSite.Application.Services.Feedbacks
{
    /// <summary>
    /// Feedback service
    /// </summary>
    public partial class FeedbackService : IFeedbackService
    {
        #region Fields

        private readonly IRepository<Feedback> _feedbackRepository;
		private readonly IRepository<SiteMapping> _siteMappingRepository;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        public FeedbackService(IRepository<Feedback> feedbackRepository,
			IRepository<SiteMapping> siteMappingRepository,
			IEventPublisher eventPublisher)
        {
            _feedbackRepository = feedbackRepository;
			_siteMappingRepository = siteMappingRepository;
            _eventPublisher = eventPublisher;

			this.QuerySettings = DbQuerySettings.Default;
		}

		public DbQuerySettings QuerySettings { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a feedback
        /// </summary>
        /// <param name="feedback">Feedback</param>
        public virtual void DeleteFeedback(Feedback feedback)
        {
            if (feedback == null)
                throw new ArgumentNullException("feedback");

            _feedbackRepository.Delete(feedback);

            //event notification
            _eventPublisher.EntityDeleted(feedback);
        }

        /// <summary>
        /// Gets a feedback
        /// </summary>
        /// <param name="feedbackId">The feedback identifier</param>
        /// <returns>Feedback</returns>
        public virtual Feedback GetFeedbackById(int feedbackId)
        {
            if (feedbackId == 0)
                return null;

            return _feedbackRepository.GetById(feedbackId);
        }


        /// <summary>
        /// Gets all feedbacks
        /// </summary>
		/// <param name="siteId">Site identifier; pass 0 to load all records</param>
        /// <returns>Feedbacks</returns>
		public virtual IList<Feedback> GetAllFeedbacks()
        {
			var query = _feedbackRepository.Table;
            query = query.OrderBy(t => t.AddTime);

			return query.ToList();
        }

        /// <summary>
        /// Inserts a feedback
        /// </summary>
        /// <param name="feedback">Feedback</param>
        public virtual void InsertFeedback(Feedback feedback)
        {
            if (feedback == null)
                throw new ArgumentNullException("feedback");

            _feedbackRepository.Insert(feedback);

            //event notification
            _eventPublisher.EntityInserted(feedback);
        }

        /// <summary>
        /// Updates the feedback
        /// </summary>
        /// <param name="feedback">Feedback</param>
        public virtual void UpdateFeedback(Feedback feedback)
        {
            if (feedback == null)
                throw new ArgumentNullException("feedback");

            _feedbackRepository.Update(feedback);

            //event notification
            _eventPublisher.EntityUpdated(feedback);
        }

        #endregion
    }
}
