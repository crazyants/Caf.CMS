using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CAF.WebSite.Application.Services.Articles
{
    /// <summary>
    /// Category template service
    /// </summary>
    public partial class ModelTemplateService : IModelTemplateService
    {

        #region Fields

        private readonly IRepository<ModelTemplate> _modelTemplateRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="modelTemplateRepository">Category template repository</param>
        /// <param name="eventPublisher">Event published</param>
        public ModelTemplateService(ICacheManager cacheManager,
            IRepository<ModelTemplate> modelTemplateRepository, IEventPublisher eventPublisher)
        {
            _cacheManager = cacheManager;
            _modelTemplateRepository = modelTemplateRepository;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete category template
        /// </summary>
        /// <param name="modelTemplate">Category template</param>
        public virtual void DeleteModelTemplate(ModelTemplate modelTemplate)
        {
            if (modelTemplate == null)
                throw new ArgumentNullException("modelTemplate");

            _modelTemplateRepository.Delete(modelTemplate);

            //event notification
            _eventPublisher.EntityDeleted(modelTemplate);
        }

        /// <summary>
        /// Gets all category templates
        /// </summary>
        /// <returns>Category templates</returns>
        public virtual IList<ModelTemplate> GetAllModelTemplates(int? templateType = null)
        {
            var query = _modelTemplateRepository.Table;
            if (templateType.HasValue)
                query = query.Where(c => c.TemplageTypeId == templateType.Value);

            query = query.OrderBy(c => c.Id).ThenBy(c => c.DisplayOrder);
            var templates = query.ToList();
            return templates;
        }

        /// <summary>
        /// Gets a category template
        /// </summary>
        /// <param name="modelTemplateId">Category template identifier</param>
        /// <returns>Category template</returns>
        public virtual ModelTemplate GetModelTemplateById(int modelTemplateId)
        {
            if (modelTemplateId == 0)
                return null;

            return _modelTemplateRepository.GetById(modelTemplateId);
        }

        /// <summary>
        /// Inserts category template
        /// </summary>
        /// <param name="modelTemplate">Category template</param>
        public virtual void InsertModelTemplate(ModelTemplate modelTemplate)
        {
            if (modelTemplate == null)
                throw new ArgumentNullException("modelTemplate");

            _modelTemplateRepository.Insert(modelTemplate);

            //event notification
            _eventPublisher.EntityInserted(modelTemplate);
        }

        /// <summary>
        /// Updates the category template
        /// </summary>
        /// <param name="modelTemplate">Category template</param>
        public virtual void UpdateModelTemplate(ModelTemplate modelTemplate)
        {
            if (modelTemplate == null)
                throw new ArgumentNullException("modelTemplate");

            _modelTemplateRepository.Update(modelTemplate);

            //event notification
            _eventPublisher.EntityUpdated(modelTemplate);
        }

        #endregion
    }
}
