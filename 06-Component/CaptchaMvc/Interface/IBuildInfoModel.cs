using System.Collections.Generic;
using System.Web.Mvc;

namespace CaptchaMvc.Interface
{
    /// <summary>
    ///     Represents the base model with information for create a captcha.
    /// </summary>
    public interface IBuildInfoModel
    {
        /// <summary>
        ///     Gets the attributes.
        /// </summary>
        IDictionary<string, string> Attributes { get; }

        /// <summary>
        ///     Gets the parameter container.
        /// </summary>
        IParameterContainer ParameterContainer { get; }

        /// <summary>
        ///     Gets the token parameter name.
        /// </summary>
        string TokenParameterName { get; }

        /// <summary>
        ///     Gets the required field message.
        /// </summary>
        string RequiredMessage { get; }

        /// <summary>
        ///     Gets the is required flag.
        /// </summary>
        bool IsRequired { get; }

        /// <summary>
        ///     Gets the refresh button text.
        /// </summary>
        string RefreshButtonText { get; }

        /// <summary>
        ///     Gets the input text.
        /// </summary>
        string InputText { get; }

        /// <summary>
        ///     Gets the specified <see cref="HtmlHelper" />.
        /// </summary>
        HtmlHelper HtmlHelper { get; }

        /// <summary>
        ///     Gets the input element id in DOM.
        /// </summary>
        string InputElementId { get; }

        /// <summary>
        ///     Gets the token element id in DOM.
        /// </summary>
        string TokenElementId { get; }

        /// <summary>
        ///     Gets the image element id in DOM.
        /// </summary>
        string ImageElementId { get; }

        /// <summary>
        ///     Gets the image url.
        /// </summary>
        string ImageUrl { get; }

        /// <summary>
        ///     Gets the refresh url.
        /// </summary>
        string RefreshUrl { get; }

        /// <summary>
        ///     Gets the token value.
        /// </summary>
        string TokenValue { get; }
    }
}