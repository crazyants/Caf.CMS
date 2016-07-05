using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core.Pages;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Cms.Polls;
using CAF.Infrastructure.Core.Domain.Sites;
using System;
using System.Linq;
 
namespace CAF.WebSite.Application.Services.Polls
{
    /// <summary>
    /// Poll service
    /// </summary>
    public partial class PollService : IPollService
    {

        #region Fields

        private readonly IRepository<Poll> _pollRepository;
        private readonly IRepository<PollAnswer> _pollAnswerRepository;
        private readonly IRepository<PollVotingRecord> _pollVotingRecords;
		private readonly IRepository<SiteMapping> _SiteMappingRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;
		private readonly ISiteContext _SiteContext;

        #endregion

        #region Ctor

        public PollService(IRepository<Poll> pollRepository, 
            IRepository<PollAnswer> pollAnswerRepository,
            IRepository<PollVotingRecord> pollVotingRecords,
			IRepository<SiteMapping> SiteMappingRepository,
            ICacheManager cacheManager, IEventPublisher eventPublisher,
			ISiteContext SiteContext)
        {
            this._pollRepository = pollRepository;
            this._pollAnswerRepository = pollAnswerRepository;
            this._pollVotingRecords = pollVotingRecords;
			this._SiteMappingRepository = SiteMappingRepository;
            this._cacheManager = cacheManager;
            this._eventPublisher = eventPublisher;
			this._SiteContext = SiteContext;

			this.QuerySettings = DbQuerySettings.Default;
        }

		public DbQuerySettings QuerySettings { get; set; }

        #endregion

		#region Utilities

		private IQueryable<Poll> Filter(IQueryable<Poll> query, bool showHidden)
		{
			if (!showHidden)
			{
				var utcNow = DateTime.UtcNow;
				query = query.Where(p => p.Published);
				query = query.Where(p => !p.StartDateUtc.HasValue || p.StartDateUtc <= utcNow);
				query = query.Where(p => !p.EndDateUtc.HasValue || p.EndDateUtc >= utcNow);

				if (!QuerySettings.IgnoreMultiSite)
				{
					var currentSiteId = _SiteContext.CurrentSite.Id;

					query =
						from p in query
						join sm in _SiteMappingRepository.Table
						on new { c1 = p.Id, c2 = "Poll" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into p_sm
						from sm in p_sm.DefaultIfEmpty()
						where !p.LimitedToSites || currentSiteId == sm.SiteId
						select p;
				}
			}

			return query;
		}

		#endregion Utilities

		#region Methods

		/// <summary>
        /// Gets a poll
        /// </summary>
        /// <param name="pollId">The poll identifier</param>
        /// <returns>Poll</returns>
        public virtual Poll GetPollById(int pollId)
        {
            if (pollId == 0)
                return null;

            return _pollRepository.GetById(pollId);
        }

        /// <summary>
        /// Gets a poll
        /// </summary>
        /// <param name="systemKeyword">The poll system keyword</param>
        /// <param name="languageId">Language identifier. 0 if you want to get all polls</param>
		/// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Poll</returns>
		public virtual Poll GetPollBySystemKeyword(string systemKeyword, int languageId, bool showHidden = false)
        {
            if (String.IsNullOrWhiteSpace(systemKeyword))
                return null;

            var query = 
				from p in _pollRepository.Table
				where p.SystemKeyword == systemKeyword && p.LanguageId == languageId
				select p;

			query = Filter(query, showHidden);

            var poll = query.FirstOrDefault();
            return poll;
        }
        
        /// <summary>
        /// Gets poll collection
        /// </summary>
        /// <param name="languageId">Language identifier. 0 if you want to get all polls</param>
        /// <param name="loadShownOnHomePageOnly">Retrieve only shown on home page polls</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Poll collection</returns>
        public virtual IPagedList<Poll> GetPolls(int languageId, bool loadShownOnHomePageOnly, int pageIndex, int pageSize, bool showHidden = false)
        {
            var query = _pollRepository.Table;
			query = Filter(query, showHidden);
            
			if (loadShownOnHomePageOnly)
            {
                query = query.Where(p => p.ShowOnHomePage);
            }
            
			if (languageId > 0)
            {
                query = query.Where(p => p.LanguageId == languageId);
            }
            
			query = query.OrderBy(p => p.DisplayOrder);

            var polls = new PagedList<Poll>(query, pageIndex, pageSize);
            return polls;
        }

        /// <summary>
        /// Deletes a poll
        /// </summary>
        /// <param name="poll">The poll</param>
        public virtual void DeletePoll(Poll poll)
        {
            if (poll == null)
                throw new ArgumentNullException("poll");

            _pollRepository.Delete(poll);

            //event notification
            _eventPublisher.EntityDeleted(poll);
        }

        /// <summary>
        /// Inserts a poll
        /// </summary>
        /// <param name="poll">Poll</param>
        public virtual void InsertPoll(Poll poll)
        {
            if (poll == null)
                throw new ArgumentNullException("poll");

            _pollRepository.Insert(poll);

            //event notification
            _eventPublisher.EntityInserted(poll);
        }

        /// <summary>
        /// Updates the poll
        /// </summary>
        /// <param name="poll">Poll</param>
        public virtual void UpdatePoll(Poll poll)
        {
            if (poll == null)
                throw new ArgumentNullException("poll");

            _pollRepository.Update(poll);

            //event notification
            _eventPublisher.EntityUpdated(poll);
        }
        
        /// <summary>
        /// Gets a poll answer
        /// </summary>
        /// <param name="pollAnswerId">Poll answer identifier</param>
        /// <returns>Poll answer</returns>
        public virtual PollAnswer GetPollAnswerById(int pollAnswerId)
        {
            if (pollAnswerId == 0)
                return null;

            var query = from pa in _pollAnswerRepository.Table
                        where pa.Id == pollAnswerId
                        select pa;
            var pollAnswer = query.SingleOrDefault();
            return pollAnswer;
        }
        
        /// <summary>
        /// Deletes a poll answer
        /// </summary>
        /// <param name="pollAnswer">Poll answer</param>
        public virtual void DeletePollAnswer(PollAnswer pollAnswer)
        {
            if (pollAnswer == null)
                throw new ArgumentNullException("pollAnswer");

            _pollAnswerRepository.Delete(pollAnswer);

            //event notification
            _eventPublisher.EntityDeleted(pollAnswer);
        }

        /// <summary>
        /// Gets a value indicating whether user already vited for this poll
        /// </summary>
        /// <param name="pollId">Poll identifier</param>
        /// <param name="userId">User identifier</param>
        /// <returns>Result</returns>
        public virtual bool AlreadyVoted(int pollId, int userId)
        {
            if (pollId == 0 || userId == 0)
                return false;

            var result = (from pa in _pollAnswerRepository.Table
                          join pvr in _pollVotingRecords.Table on pa.Id equals pvr.PollAnswerId
                          where pa.PollId == pollId && pvr.UserId == userId
                          select pvr).Count() > 0;
            return result;
        }

        #endregion
    }
}
