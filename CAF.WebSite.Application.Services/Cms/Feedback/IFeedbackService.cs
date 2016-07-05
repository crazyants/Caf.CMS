using System.Collections.Generic;
using CAF.Infrastructure.Core.Domain.Cms.Articles;

namespace CAF.WebSite.Application.Services.Feedbacks
{
    /// <summary>
    /// Feedback service interface
    /// </summary>
    public partial interface IFeedbackService
    {
        /// <summary>
        /// Deletes a feedback
        /// </summary>
        /// <param name="feedback">Feedback</param>
        void DeleteFeedback(Feedback feedback);

        /// <summary>
        /// Gets a feedback
        /// </summary>
        /// <param name="feedbackId">The feedback identifier</param>
        /// <returns>Feedback</returns>
        Feedback GetFeedbackById(int feedbackId);


        /// <summary>
        /// Gets all feedbacks
        /// </summary>
		/// <param name="siteId">Site identifier; pass 0 to load all records</param>
        /// <returns>Feedbacks</returns>
		IList<Feedback> GetAllFeedbacks();

        /// <summary>
        /// Inserts a feedback
        /// </summary>
        /// <param name="feedback">Feedback</param>
        void InsertFeedback(Feedback feedback);

        /// <summary>
        /// Updates the feedback
        /// </summary>
        /// <param name="feedback">Feedback</param>
        void UpdateFeedback(Feedback feedback);
    }
}
