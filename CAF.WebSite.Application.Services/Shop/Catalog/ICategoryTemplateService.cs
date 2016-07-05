using CAF.WebSite.Domain.Seedwork.Shop.Catalog;
using System.Collections.Generic;


namespace CAF.WebSite.Application.Services.Catalog
{
    /// <summary>
    /// Category template service interface
    /// </summary>
    public partial interface ICategoryTemplateService
    {
        /// <summary>
        /// Delete category template
        /// </summary>
        /// <param name="categoryTemplate">Category template</param>
        void DeleteCategoryTemplate(CategoryTemplate categoryTemplate);

        /// <summary>
        /// Gets all category templates
        /// </summary>
        /// <returns>Category templates</returns>
        IList<CategoryTemplate> GetAllCategoryTemplates();

        /// <summary>
        /// Gets a category template
        /// </summary>
        /// <param name="categoryTemplateId">Category template identifier</param>
        /// <returns>Category template</returns>
        CategoryTemplate GetCategoryTemplateById(int categoryTemplateId);

        /// <summary>
        /// Inserts category template
        /// </summary>
        /// <param name="categoryTemplate">Category template</param>
        void InsertCategoryTemplate(CategoryTemplate categoryTemplate);

        /// <summary>
        /// Updates the category template
        /// </summary>
        /// <param name="categoryTemplate">Category template</param>
        void UpdateCategoryTemplate(CategoryTemplate categoryTemplate);
    }
}
