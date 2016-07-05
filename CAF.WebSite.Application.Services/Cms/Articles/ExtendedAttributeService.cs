using System;
using System.Collections.Generic;
using System.Linq;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core.Pages;
using CAF.Infrastructure.Core.Domain.Cms.Articles;

namespace CAF.WebSite.Application.Services.Articles
{
    /// <summary>
    /// Extended attribute service
    /// </summary>
    public partial class ExtendedAttributeService : IExtendedAttributeService
    {
        #region Constants

        private const string EXTENDEDATTRIBUTES_ALL_KEY = "caf.extendedattribute.all-{0}";
        private const string EXTENDEDATTRIBUTEVALUES_ALL_KEY = "caf.extendedattributevalue.all-{0}";
        private const string EXTENDEDATTRIBUTES_PATTERN_KEY = "caf.extendedattribute.";
        private const string EXTENDEDATTRIBUTEVALUES_PATTERN_KEY = "caf.extendedattributevalue.";
        private const string EXTENDEDATTRIBUTES_BY_ID_KEY = "caf.extendedattribute.id-{0}";
        private const string EXTENDEDATTRIBUTEVALUES_BY_ID_KEY = "caf.extendedattributevalue.id-{0}";

        #endregion

        #region Fields

        private readonly IRepository<ExtendedAttribute> _extendedAttributeRepository;
        private readonly IRepository<ExtendedAttributeValue> _extendedAttributeValueRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="extendedAttributeRepository">Extended attribute repository</param>
        /// <param name="extendedAttributeValueRepository">Extended attribute value repository</param>
        /// <param name="eventPublisher">Event published</param>
        public ExtendedAttributeService(ICacheManager cacheManager,
            IRepository<ExtendedAttribute> extendedAttributeRepository,
            IRepository<ExtendedAttributeValue> extendedAttributeValueRepository,
            IEventPublisher eventPublisher)
        {
            _cacheManager = cacheManager;
            _extendedAttributeRepository = extendedAttributeRepository;
            _extendedAttributeValueRepository = extendedAttributeValueRepository;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        #region Extended attributes

        /// <summary>
        /// Deletes a extended attribute
        /// </summary>
        /// <param name="extendedAttribute">Extended attribute</param>
        public virtual void DeleteExtendedAttribute(ExtendedAttribute extendedAttribute)
        {
            if (extendedAttribute == null)
                throw new ArgumentNullException("extendedAttribute");

            _extendedAttributeRepository.Delete(extendedAttribute);

            _cacheManager.RemoveByPattern(EXTENDEDATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(EXTENDEDATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(extendedAttribute);
        }

        /// <summary>
        /// Gets all extended attributes
        /// </summary>
        /// <returns>Extended attribute collection</returns>
        public virtual IList<ExtendedAttribute> GetAllExtendedAttributes(bool showHidden = false)
        {
            string key = EXTENDEDATTRIBUTES_ALL_KEY.FormatInvariant(showHidden);

            return _cacheManager.Get(key, () =>
            {
                var query = _extendedAttributeRepository.Table;

                if (!showHidden)
                    query = query.Where(x => x.IsActive);

                query = query.OrderBy(x => x.DisplayOrder);

                var extendedAttributes = query.ToList();
                return extendedAttributes;
            });
        }

        /// <summary>
        /// Gets a extended attribute 
        /// </summary>
        /// <param name="extendedAttributeId">Extended attribute identifier</param>
        /// <returns>Extended attribute</returns>
        public virtual ExtendedAttribute GetExtendedAttributeById(int extendedAttributeId)
        {
            if (extendedAttributeId == 0)
                return null;

            string key = string.Format(EXTENDEDATTRIBUTES_BY_ID_KEY, extendedAttributeId);
            return _cacheManager.Get(key, () =>
            {
                return _extendedAttributeRepository.GetById(extendedAttributeId);
            });
        }

        /// <summary>
        /// Inserts a extended attribute
        /// </summary>
        /// <param name="extendedAttribute">Extended attribute</param>
        public virtual void InsertExtendedAttribute(ExtendedAttribute extendedAttribute)
        {
            if (extendedAttribute == null)
                throw new ArgumentNullException("extendedAttribute");

            _extendedAttributeRepository.Insert(extendedAttribute);

            _cacheManager.RemoveByPattern(EXTENDEDATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(EXTENDEDATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(extendedAttribute);
        }

        /// <summary>
        /// Updates the extended attribute
        /// </summary>
        /// <param name="extendedAttribute">Extended attribute</param>
        public virtual void UpdateExtendedAttribute(ExtendedAttribute extendedAttribute)
        {
            if (extendedAttribute == null)
                throw new ArgumentNullException("extendedAttribute");

            _extendedAttributeRepository.Update(extendedAttribute);

            _cacheManager.RemoveByPattern(EXTENDEDATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(EXTENDEDATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(extendedAttribute);
        }

        /// <summary>
        /// Gets ExtendedAttribute tag by name
        /// </summary>
        /// <param name="name">ExtendedAttribute   name</param>
        /// <returns>ExtendedAttribute  </returns>
        public virtual ExtendedAttribute GetExtendedAttributeByName(string name)
        {
            var query = from pt in _extendedAttributeRepository.Table
                        where pt.Name == name
                        select pt;

            var extendedAttribute = query.FirstOrDefault();
            return extendedAttribute;
        }

        #endregion

        #region Extended variant attribute values

        /// <summary>
        /// Deletes a extended attribute value
        /// </summary>
        /// <param name="extendedAttributeValue">Extended attribute value</param>
        public virtual void DeleteExtendedAttributeValue(ExtendedAttributeValue extendedAttributeValue)
        {
            if (extendedAttributeValue == null)
                throw new ArgumentNullException("extendedAttributeValue");

            _extendedAttributeValueRepository.Delete(extendedAttributeValue);

            _cacheManager.RemoveByPattern(EXTENDEDATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(EXTENDEDATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(extendedAttributeValue);
        }

        /// <summary>
        /// Gets extended attribute values by extended attribute identifier
        /// </summary>
        /// <param name="extendedAttributeId">The extended attribute identifier</param>
        /// <returns>Extended attribute value collection</returns>
        public virtual IList<ExtendedAttributeValue> GetExtendedAttributeValues(int extendedAttributeId)
        {
            string key = string.Format(EXTENDEDATTRIBUTEVALUES_ALL_KEY, extendedAttributeId);
            return _cacheManager.Get(key, () =>
            {
                var query = from cav in _extendedAttributeValueRepository.Table
                            orderby cav.DisplayOrder
                            where cav.ExtendedAttributeId == extendedAttributeId
                            select cav;
                var extendedAttributeValues = query.ToList();
                return extendedAttributeValues;
            });
        }

        /// <summary>
        /// Gets a extended attribute value
        /// </summary>
        /// <param name="extendedAttributeValueId">Extended attribute value identifier</param>
        /// <returns>Extended attribute value</returns>
        public virtual ExtendedAttributeValue GetExtendedAttributeValueById(int extendedAttributeValueId)
        {
            if (extendedAttributeValueId == 0)
                return null;

            string key = string.Format(EXTENDEDATTRIBUTEVALUES_BY_ID_KEY, extendedAttributeValueId);
            return _cacheManager.Get(key, () =>
            {
                return _extendedAttributeValueRepository.GetById(extendedAttributeValueId);
            });
        }

        /// <summary>
        /// Inserts a extended attribute value
        /// </summary>
        /// <param name="extendedAttributeValue">Extended attribute value</param>
        public virtual void InsertExtendedAttributeValue(ExtendedAttributeValue extendedAttributeValue)
        {
            if (extendedAttributeValue == null)
                throw new ArgumentNullException("extendedAttributeValue");

            _extendedAttributeValueRepository.Insert(extendedAttributeValue);

            _cacheManager.RemoveByPattern(EXTENDEDATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(EXTENDEDATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(extendedAttributeValue);
        }

        /// <summary>
        /// Updates the extended attribute value
        /// </summary>
        /// <param name="extendedAttributeValue">Extended attribute value</param>
        public virtual void UpdateExtendedAttributeValue(ExtendedAttributeValue extendedAttributeValue)
        {
            if (extendedAttributeValue == null)
                throw new ArgumentNullException("extendedAttributeValue");

            _extendedAttributeValueRepository.Update(extendedAttributeValue);

            _cacheManager.RemoveByPattern(EXTENDEDATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(EXTENDEDATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(extendedAttributeValue);
        }

        #endregion

        #endregion
    }
}
