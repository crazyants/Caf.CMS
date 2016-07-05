
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System.Collections.Generic;


namespace CAF.WebSite.Application.Services.Articles
{
    /// <summary>
    /// Category template service interface
    /// </summary>
    public partial interface IModelTemplateService
    {
        /// <summary>
        /// Delete category template
        /// </summary>
        /// <param name="modelTemplate">Category template</param>
        void DeleteModelTemplate(ModelTemplate modelTemplate);

        /// <summary>
        /// Gets all category templates
        /// </summary>
        /// <returns>Category templates</returns>
        IList<ModelTemplate> GetAllModelTemplates(int? templateType=null);

        /// <summary>
        /// Gets a category template
        /// </summary>
        /// <param name="modelTemplateId">Category template identifier</param>
        /// <returns>Category template</returns>
        ModelTemplate GetModelTemplateById(int modelTemplateId);

        /// <summary>
        /// Inserts category template
        /// </summary>
        /// <param name="modelTemplate">Category template</param>
        void InsertModelTemplate(ModelTemplate modelTemplate);

        /// <summary>
        /// Updates the category template
        /// </summary>
        /// <param name="modelTemplate">Category template</param>
        void UpdateModelTemplate(ModelTemplate modelTemplate);
    }
}
