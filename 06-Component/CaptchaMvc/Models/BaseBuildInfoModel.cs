using System.Collections.Generic;
using System.Web.Mvc;
using CaptchaMvc.Infrastructure;
using CaptchaMvc.Interface;

namespace CaptchaMvc.Models
{
    /// <summary>
    ///     Represents the base model with information for create a captcha.
    /// </summary>
    public abstract class BaseBuildInfoModel : IBuildInfoModel
    {
        #region Constructor

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseBuildInfoModel" /> class.
        /// </summary>
        protected BaseBuildInfoModel(IParameterContainer parameterContainer, string tokenParameterName,
                                     string requiredMessage, bool isRequired,
                                     string refreshButtonText, string inputText, HtmlHelper htmlHelper,
                                     string inputElementId, string imageElementId, string tokenElementId,
                                     string refreshUrl,
                                     string imageUrl, string tokenValue)
        {
            Validate.ArgumentNotNull(parameterContainer, "parameterContainer");
            Validate.ArgumentNotNullOrEmpty(tokenParameterName, "tokenParameterName");
            Validate.ArgumentNotNullOrEmpty(inputElementId, "inputElementId");
            Validate.ArgumentNotNullOrEmpty(imageElementId, "imageElementId");
            Validate.ArgumentNotNullOrEmpty(tokenElementId, "tokenElementId");
            Validate.ArgumentNotNullOrEmpty(tokenValue, "tokenValue");
            ParameterContainer = parameterContainer;
            TokenParameterName = tokenParameterName;
            RequiredMessage = requiredMessage;
            IsRequired = isRequired;
            RefreshButtonText = refreshButtonText;
            InputText = inputText;
            HtmlHelper = htmlHelper;
            InputElementId = inputElementId;
            ImageElementId = imageElementId;
            TokenElementId = tokenElementId;
            RefreshUrl = refreshUrl;
            ImageUrl = imageUrl;
            TokenValue = tokenValue;
            Attributes = new Dictionary<string, string>();
        }

        #endregion

        #region Implementation of IBuildInfoModel

        /// <summary>
        ///     Gets the attributes.
        /// </summary>
        public IDictionary<string, string> Attributes { get; private set; }

        /// <summary>
        ///     Gets the parameter container.
        /// </summary>
        public IParameterContainer ParameterContainer { get; private set; }

        /// <summary>
        ///     Gets the token parameter name.
        /// </summary>
        public string TokenParameterName { get; set; }

        /// <summary>
        ///     Gets the required field message.
        /// </summary>
        public string RequiredMessage { get; set; }

        /// <summary>
        ///     Gets the is required flag.
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        ///     Gets the refresh button text.
        /// </summary>
        public string RefreshButtonText { get; set; }

        /// <summary>
        ///     Gets the input text.
        /// </summary>
        public string InputText { get; set; }

        /// <summary>
        ///     Gets the specified <see cref="HtmlHelper" />.
        /// </summary>
        public HtmlHelper HtmlHelper { get; set; }

        /// <summary>
        ///     Gets the input element id in DOM.
        /// </summary>
        public string InputElementId { get; set; }

        /// <summary>
        ///     Gets the token element id in DOM.
        /// </summary>
        public string TokenElementId { get; set; }

        /// <summary>
        ///     Gets the image element id in DOM.
        /// </summary>
        public string ImageElementId { get; set; }

        /// <summary>
        ///     Gets the image url.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        ///     Gets the refresh url.
        /// </summary>
        public string RefreshUrl { get; set; }

        /// <summary>
        ///     Gets the token value.
        /// </summary>
        public string TokenValue { get; set; }

        #endregion
    }
}