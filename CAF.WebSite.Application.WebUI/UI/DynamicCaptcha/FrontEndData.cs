using System;
using System.Collections.Generic;

namespace CAF.WebSite.Application.WebUI.DynamicCaptcha
{
    /// <summary>
    /// Serializable class for use by front-end library
    /// </summary>
    [Serializable]
    public sealed class FrontEndData
    {
        /// <summary>
        /// Collection of hashed image path values
        /// </summary>
        public List<string> Values { get; internal set; }

        /// <summary>
        /// A description of the valid image selection
        /// </summary>
        public string ImageName { get; internal set; }
        
        /// <summary>
        /// Name assigned to the HTML element representing the valid image
        /// </summary>
        public string ImageFieldName { get; internal set; }

        /// <summary>
        /// Name assigned to the HTML input for audio option
        /// </summary>
        public string AudioFieldName { get; internal set; }

        internal FrontEndData()
        {

        }
    }
}