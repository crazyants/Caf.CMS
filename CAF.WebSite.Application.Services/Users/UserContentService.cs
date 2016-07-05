
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
 

namespace CAF.WebSite.Application.Services.Users
{
    /// <summary>
    /// User content service
    /// </summary>
    public partial class UserContentService : IUserContentService
    {
        #region Fields

        private readonly IRepository<UserContent> _contentRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="contentRepository">User content repository</param>
        /// <param name="eventPublisher">Event published</param>
        public UserContentService(ICacheManager cacheManager,
            IRepository<UserContent> contentRepository,
            IEventPublisher eventPublisher)
        {
            _cacheManager = cacheManager;
            _contentRepository = contentRepository;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a user content
        /// </summary>
        /// <param name="content">User content</param>
        public virtual void DeleteUserContent(UserContent content)
        {
            if (content == null)
                throw new ArgumentNullException("content");

            _contentRepository.Delete(content);

            //event notification
            _eventPublisher.EntityDeleted(content);
        }

        /// <summary>
        /// Gets all user content
        /// </summary>
        /// <param name="userId">User identifier; 0 to load all records</param>
        /// <param name="approved">A value indicating whether to content is approved; null to load all records</param>
        /// <returns>User content</returns>
        public virtual IList<UserContent> GetAllUserContent(int userId, bool? approved)
        {
            var query = from c in _contentRepository.Table
                        orderby c.CreatedOnUtc descending
                        where !approved.HasValue || c.IsApproved == approved &&
                        (userId == 0 || c.UserId == userId)
                        select c;
            var content = query.ToList();
            return content;
        }

        /// <summary>
        /// Gets all user content
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="userId">User identifier; 0 to load all records</param>
        /// <param name="approved">A value indicating whether to content is approved; null to load all records</param> 
        /// <param name="fromUtc">Item creation from; null to load all records</param>
        /// <param name="toUtc">Item item creation to; null to load all records</param>
        /// <returns>User content</returns>
        public virtual IList<T> GetAllUserContent<T>(int userId, bool? approved,
            DateTime? fromUtc = null, DateTime? toUtc = null) where T : UserContent
        {
            var query = _contentRepository.Table;
            if (approved.HasValue)
                query = query.Where(c => c.IsApproved == approved);
            if (userId > 0)
                query = query.Where(c => c.UserId == userId);
            if (fromUtc.HasValue)
                query = query.Where(c => fromUtc.Value <= c.CreatedOnUtc);
            if (toUtc.HasValue)
                query = query.Where(c => toUtc.Value >= c.CreatedOnUtc);
            query = query.OrderByDescending(c => c.CreatedOnUtc);
            var content = query.OfType<T>().ToList();
            return content;
        }

        /// <summary>
        /// Gets a user content
        /// </summary>
        /// <param name="contentId">User content identifier</param>
        /// <returns>User content</returns>
        public virtual UserContent GetUserContentById(int contentId)
        {
            if (contentId == 0)
                return null;

            return _contentRepository.GetById(contentId);
                                          
        }

        /// <summary>
        /// Inserts a user content
        /// </summary>
        /// <param name="content">User content</param>
        public virtual void InsertUserContent(UserContent content)
        {
            if (content == null)
                throw new ArgumentNullException("content");

            _contentRepository.Insert(content);

            //event notification
            _eventPublisher.EntityInserted(content);
        }

        /// <summary>
        /// Updates a user content
        /// </summary>
        /// <param name="content">User content</param>
        public virtual void UpdateUserContent(UserContent content)
        {
            if (content == null)
                throw new ArgumentNullException("content");

            _contentRepository.Update(content);

            //event notification
            _eventPublisher.EntityUpdated(content);
        }

        #endregion
    }
}
