using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System;
using System.Collections.Generic;
using System.Linq;
 

namespace CAF.WebSite.Application.Services.Articles
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class ExtendedAttributeExtensions
    {
        /// <summary>
        /// A value indicating whether this product variant attribute should have values
        /// </summary>
        /// <param name="ExtendedAttribute">Checkout attribute</param>
        /// <returns>Result</returns>
        public static bool ShouldHaveValues(this ExtendedAttribute extendedAttribute)
        {
            if (extendedAttribute == null)
                return false;

            if (extendedAttribute.AttributeControlType == AttributeControlType.TextBox ||
                extendedAttribute.AttributeControlType == AttributeControlType.MultilineTextbox ||
                extendedAttribute.AttributeControlType == AttributeControlType.Datepicker ||
                extendedAttribute.AttributeControlType == AttributeControlType.FileUpload || extendedAttribute.AttributeControlType == AttributeControlType.VideoUpload)
                return false;
            
            //other attribute controle types support values
            return true;
        }

        /// <summary>
        /// Remove attributes which require shippable products
        /// </summary>
        /// <param name="extendedAttributes">Checkout attributes</param>
        /// <returns>Result</returns>
        //public static IList<ExtendedAttribute> RemoveShippableAttributes(this IList<ExtendedAttribute> extendedAttributes)
        //{
        //    if (extendedAttributes == null)
        //        throw new ArgumentNullException("extendedAttributes");

        //    return extendedAttributes.Where(x => !x.ShippableProductRequired).ToList();
        //}
    }
}
