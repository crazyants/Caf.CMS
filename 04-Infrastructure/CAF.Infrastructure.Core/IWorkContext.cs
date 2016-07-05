
using CAF.Infrastructure.Core.Domain.Directory;
using CAF.Infrastructure.Core.Domain.Localization;
using CAF.Infrastructure.Core.Domain.Security;
using CAF.Infrastructure.Core.Domain.Tax;
using CAF.Infrastructure.Core.Domain.Users;

namespace CAF.Infrastructure.Core
{
    /// <summary>
    /// Work context
    /// </summary>
    public interface IWorkContext
    {
        /// <summary>
        /// Gets or sets the current customer
        /// </summary>
        User CurrentUser { get; set; }

        /// <summary>
        /// Gets or sets the original customer (in case the current one is impersonated)
        /// </summary>
        User OriginalUserIfImpersonated { get; }

        /// <summary>
        /// Get or set current user working language
        /// </summary>
         Language WorkingLanguage { get; set; }

        /// <summary>
        /// Gets a value indicating whether a language exists and is published within a store's scope.
        /// </summary>
        /// <param name="seoCode">The unique seo code of the language to check for</param>
        /// <param name="siteId">The store id (will be resolved internally when 0)</param>
        /// <returns>Whether the language exists and is published</returns>
         bool IsPublishedLanguage(string seoCode, int siteId = 0);

        /// <summary>
        /// Gets the default (fallback) language for a store
        /// </summary>
        /// <param name="siteId">The store id (will be resolved internally when 0)</param>
        /// <returns>The unique seo code of the language to check for</returns>
        string GetDefaultLanguageSeoCode(int siteId = 0);


        /// <summary>
        /// Get or set current tax display type
        /// </summary>
        TaxDisplayType TaxDisplayType { get; set; }

        /// <summary>
        /// Gets the tax display type for a given customer
        /// </summary>
        TaxDisplayType GetTaxDisplayTypeFor(User user, int siteId);

        /// <summary>
        /// Get or set value indicating whether we're in admin area
        /// </summary>
        bool IsAdmin { get; set; }

      
    }
}
