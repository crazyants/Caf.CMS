using CAF.WebSite.Application.Services.Localization;
using CAF.Infrastructure.Core.Domain.Messages;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CAF.WebSite.Application.Services.Messages
{
    public partial class MessageTemplateService: IMessageTemplateService
    {
        #region Constants

        private const string MESSAGETEMPLATES_ALL_KEY = "caf.messagetemplate.all-{0}";
        private const string MESSAGETEMPLATES_BY_NAME_KEY = "caf.messagetemplate.name-{0}-{1}";
        private const string MESSAGETEMPLATES_PATTERN_KEY = "caf.messagetemplate.";

        #endregion

        #region Fields

        private readonly IRepository<MessageTemplate> _messageTemplateRepository;
		private readonly ILanguageService _languageService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
		/// <param name="siteMappingRepository">Site mapping repository</param>
		/// <param name="languageService">Language service</param>
		/// <param name="localizedEntityService">Localized entity service</param>
		/// <param name="siteMappingService">Site mapping service</param>
        /// <param name="messageTemplateRepository">Message template repository</param>
        /// <param name="eventPublisher">Event published</param>
        public MessageTemplateService(ICacheManager cacheManager,
			ILanguageService languageService,
            IRepository<MessageTemplate> messageTemplateRepository,
            IEventPublisher eventPublisher)
        {
			this._cacheManager = cacheManager;
			this._languageService = languageService;
			this._messageTemplateRepository = messageTemplateRepository;
			this._eventPublisher = eventPublisher;

			this.QuerySettings = DbQuerySettings.Default;
		}

		public DbQuerySettings QuerySettings { get; set; }

        #endregion

        #region Methods

		/// <summary>
		/// Delete a message template
		/// </summary>
		/// <param name="messageTemplate">Message template</param>
		public virtual void DeleteMessageTemplate(MessageTemplate messageTemplate)
		{
			if (messageTemplate == null)
				throw new ArgumentNullException("messageTemplate");

			_messageTemplateRepository.Delete(messageTemplate);

			_cacheManager.RemoveByPattern(MESSAGETEMPLATES_PATTERN_KEY);

			//event notification
			_eventPublisher.EntityDeleted(messageTemplate);
		}

        /// <summary>
        /// Inserts a message template
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        public virtual void InsertMessageTemplate(MessageTemplate messageTemplate)
        {
            if (messageTemplate == null)
                throw new ArgumentNullException("messageTemplate");

            _messageTemplateRepository.Insert(messageTemplate);

            _cacheManager.RemoveByPattern(MESSAGETEMPLATES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(messageTemplate);
        }

        /// <summary>
        /// Updates a message template
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        public virtual void UpdateMessageTemplate(MessageTemplate messageTemplate)
        {
            if (messageTemplate == null)
                throw new ArgumentNullException("messageTemplate");

            _messageTemplateRepository.Update(messageTemplate);

            _cacheManager.RemoveByPattern(MESSAGETEMPLATES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(messageTemplate);
        }

        /// <summary>
        /// Gets a message template
        /// </summary>
        /// <param name="messageTemplateId">Message template identifier</param>
        /// <returns>Message template</returns>
        public virtual MessageTemplate GetMessageTemplateById(int messageTemplateId)
        {
            if (messageTemplateId == 0)
                return null;

            return _messageTemplateRepository.GetById(messageTemplateId);
        }

        public virtual MessageTemplate GetMessageTemplateByName(string messageTemplateName, int siteId)
        {
            if (string.IsNullOrWhiteSpace(messageTemplateName))
                throw new ArgumentException("messageTemplateName");

            string key = string.Format(MESSAGETEMPLATES_BY_NAME_KEY, messageTemplateName, siteId);
            return _cacheManager.Get(key, () =>
            {
                var query = _messageTemplateRepository.Table;
                query = query.Where(t => t.Name == messageTemplateName);
                query = query.OrderBy(t => t.Id);
                var templates = query.ToList();

                //store mapping
                if (siteId > 0)
                {
                     
                }

                return templates.FirstOrDefault();
            });

        }

        /// <summary>
        /// Gets all message templates
        /// </summary>
        /// <param name="siteId">Site identifier; pass 0 to load all records</param>
        /// <returns>Message template list</returns>
        public virtual IList<MessageTemplate> GetAllMessageTemplates(int siteId)
        {
            string key = string.Format(MESSAGETEMPLATES_ALL_KEY, siteId);
            return _cacheManager.Get(key, () =>
            {
                var query = _messageTemplateRepository.Table;
                query = query.OrderBy(t => t.Name);

                //Site mapping
                //if (siteId > 0 && !QuerySettings.IgnoreMultiSite)
                //{
                //    query = from t in query
                //            join sm in _siteMappingRepository.Table
                //            on new { c1 = t.Id, c2 = "MessageTemplate" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into t_sm
                //            from sm in t_sm.DefaultIfEmpty()
                //            where !t.LimitedToSites || siteId == sm.SiteId
                //            select t;

                //    //only distinct items (group by ID)
                //    query = from t in query
                //            group t by t.Id into tGroup
                //            orderby tGroup.Key
                //            select tGroup.FirstOrDefault();
                //    query = query.OrderBy(t => t.Name);
                //}

                return query.ToList();
            });
        }
		/// <summary>
		/// Create a copy of message template with all depended data
		/// </summary>
		/// <param name="messageTemplate">Message template</param>
		/// <returns>Message template copy</returns>
		public virtual MessageTemplate CopyMessageTemplate(MessageTemplate messageTemplate)
		{
			if (messageTemplate == null)
				throw new ArgumentNullException("messageTemplate");

			var mtCopy = new MessageTemplate()
			{
				Name = messageTemplate.Name,
				BccEmailAddresses = messageTemplate.BccEmailAddresses,
				Subject = messageTemplate.Subject,
				Body = messageTemplate.Body,
				IsActive = messageTemplate.IsActive,
				EmailAccountId = messageTemplate.EmailAccountId,
				LimitedToSites = messageTemplate.LimitedToSites
			};

			InsertMessageTemplate(mtCopy);

			return mtCopy;
		}

        #endregion
    }
}
