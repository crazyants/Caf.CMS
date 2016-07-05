using System;
using CaptchaMvc.Infrastructure;
using CaptchaMvc.Interface;

namespace CaptchaMvc.Models
{
    /// <summary>
    /// Represents the base model with information for update a captcha.
    /// </summary>
    public class DefaultUpdateInfoModel : IUpdateInfoModel
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultUpdateInfoModel"/> class.
        /// </summary>
        public DefaultUpdateInfoModel(string tokenElementId, string tokenValue, string imageUrl, string imageElementId)
        {
            Validate.ArgumentNotNullOrEmpty(tokenElementId, "tokenElementId");
            Validate.ArgumentNotNullOrEmpty(tokenValue, "tokenValue");
            Validate.ArgumentNotNullOrEmpty(imageUrl, "imageUrl");
            Validate.ArgumentNotNullOrEmpty(imageElementId, "imageElementId");
            TokenElementId = tokenElementId;
            TokenValue = tokenValue;
            ImageUrl = imageUrl;
            ImageElementId = imageElementId;
        }

        #endregion

        #region Implementation of IUpdateInfoModel

        /// <summary>
        /// Gets the token element id in DOM.
        /// </summary>
        public string TokenElementId { get; set; }

        /// <summary>
        /// Gets the image element id in DOM.
        /// </summary>
        public string ImageElementId { get; set; }

        /// <summary>
        /// Gets the url with captcha image.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets the token value.
        /// </summary>
        public string TokenValue { get; set; }

        #endregion
    }
}