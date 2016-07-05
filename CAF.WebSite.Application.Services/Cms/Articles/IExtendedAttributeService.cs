using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System.Collections.Generic;

namespace CAF.WebSite.Application.Services.Articles
{
    /// <summary>
    /// Extended attribute service
    /// </summary>
    public partial interface IExtendedAttributeService
    {
        #region Extended attributes

        /// <summary>
        /// Deletes a checkout attribute
        /// </summary>
        /// <param name="ExtendedAttribute">Extended attribute</param>
        void DeleteExtendedAttribute(ExtendedAttribute ExtendedAttribute);

        /// <summary>
        /// Gets all checkout attributes
        /// </summary>
        /// <returns>Extended attribute collection</returns>
        IList<ExtendedAttribute> GetAllExtendedAttributes(bool showHidden = false);

        /// <summary>
        /// Gets a checkout attribute 
        /// </summary>
        /// <param name="ExtendedAttributeId">Extended attribute identifier</param>
        /// <returns>Extended attribute</returns>
        ExtendedAttribute GetExtendedAttributeById(int ExtendedAttributeId);

        /// <summary>
        /// Inserts a checkout attribute
        /// </summary>
        /// <param name="ExtendedAttribute">Extended attribute</param>
        void InsertExtendedAttribute(ExtendedAttribute ExtendedAttribute);

        /// <summary>
        /// Updates the checkout attribute
        /// </summary>
        /// <param name="ExtendedAttribute">Extended attribute</param>
        void UpdateExtendedAttribute(ExtendedAttribute ExtendedAttribute);

        /// <summary>
        /// Gets ExtendedAttribute tag by name
        /// </summary>
        /// <param name="name">ExtendedAttribute   name</param>
        /// <returns>ExtendedAttribute  </returns>
        ExtendedAttribute GetExtendedAttributeByName(string name);

        #endregion

        #region Extended variant attribute values

        /// <summary>
        /// Deletes a checkout attribute value
        /// </summary>
        /// <param name="ExtendedAttributeValue">Extended attribute value</param>
        void DeleteExtendedAttributeValue(ExtendedAttributeValue ExtendedAttributeValue);

        /// <summary>
        /// Gets checkout attribute values by checkout attribute identifier
        /// </summary>
        /// <param name="ExtendedAttributeId">The checkout attribute identifier</param>
        /// <returns>Extended attribute value collection</returns>
        IList<ExtendedAttributeValue> GetExtendedAttributeValues(int ExtendedAttributeId);

        /// <summary>
        /// Gets a checkout attribute value
        /// </summary>
        /// <param name="ExtendedAttributeValueId">Extended attribute value identifier</param>
        /// <returns>Extended attribute value</returns>
        ExtendedAttributeValue GetExtendedAttributeValueById(int ExtendedAttributeValueId);

        /// <summary>
        /// Inserts a checkout attribute value
        /// </summary>
        /// <param name="ExtendedAttributeValue">Extended attribute value</param>
        void InsertExtendedAttributeValue(ExtendedAttributeValue ExtendedAttributeValue);

        /// <summary>
        /// Updates the checkout attribute value
        /// </summary>
        /// <param name="ExtendedAttributeValue">Extended attribute value</param>
        void UpdateExtendedAttributeValue(ExtendedAttributeValue ExtendedAttributeValue);

        #endregion
    }
}
