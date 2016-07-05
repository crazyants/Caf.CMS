using System.Web.Mvc;

namespace CaptchaMvc.Interface
{
    /// <summary>
    ///     Represents the interface that provides a basic methods for manage captcha.
    /// </summary>
    public interface ICaptchaManager
    {
        /// <summary>
        ///     Gets or sets the name for a token parameter.
        /// </summary>
        string TokenParameterName { get; set; }

        /// <summary>
        ///     Gets or sets the name for an input element in DOM.
        /// </summary>
        string InputElementName { get; set; }

        /// <summary>
        ///     Gets or sets the name for image element in DOM.
        /// </summary>
        string ImageElementName { get; set; }

        /// <summary>
        ///     Gets or sets the name for token element in DOM.
        /// </summary>
        string TokenElementName { get; set; }

        /// <summary>
        /// Gets or sets value that indicates that the current manager will add area route value in generated url.
        /// </summary>
        bool AddAreaRouteValue { get; set; }

        /// <summary>
        ///     Gets or sets the storage to save a captcha tokens.
        /// </summary>
        IStorageProvider StorageProvider { get; set; }

        /// <summary>
        ///     Gets or sets the intelligence policy.
        /// </summary>
        IIntelligencePolicy IntelligencePolicy { get; set; }

        /// <summary>
        ///     Creates a <see cref="IBuildInfoModel" /> for create a new captcha.
        /// </summary>
        /// <param name="htmlHelper">
        ///     The specified <see cref="HtmlHelper" />.
        /// </param>
        /// <param name="parameterContainer">
        ///     The specified <see cref="IParameterContainer" />.
        /// </param>
        /// <returns>
        ///     An instance of <see cref="IBuildInfoModel" />.
        /// </returns>
        IBuildInfoModel GenerateNew(HtmlHelper htmlHelper, IParameterContainer parameterContainer);

        /// <summary>
        ///     Creates a <see cref="IUpdateInfoModel" /> for create a new captcha.
        /// </summary>
        /// <param name="controller">
        ///     The specified <see cref="ControllerBase" />.
        /// </param>
        /// <param name="parameterContainer">
        ///     The specified <see cref="IParameterContainer" />.
        /// </param>
        /// <returns>
        ///     An instance of <see cref="IUpdateInfoModel" />.
        /// </returns>
        IUpdateInfoModel GenerateNew(ControllerBase controller, IParameterContainer parameterContainer);

        /// <summary>
        ///     Creates a new <see cref="IDrawingModel" /> for drawing a captcha.
        /// </summary>
        /// <param name="parameterContainer">
        ///     The specified <see cref="IParameterContainer" />.
        /// </param>
        /// <returns>
        ///     An instance of <see cref="IDrawingModel" />.
        /// </returns>
        IDrawingModel GetDrawingModel(IParameterContainer parameterContainer);

        /// <summary>
        ///     Creates a new <see cref="IBuildInfoModel" /> for update a captcha.
        /// </summary>
        /// <param name="parameterContainer">
        ///     The specified <see cref="IParameterContainer" />.
        /// </param>
        /// <returns>
        ///     An instance of <see cref="IUpdateInfoModel" />.
        /// </returns>
        IUpdateInfoModel Update(IParameterContainer parameterContainer);

        /// <summary>
        ///     Determines whether the captcha is valid, and write error message if need.
        /// </summary>
        /// <param name="controller">
        ///     The specified <see cref="ControllerBase" />.
        /// </param>
        /// <param name="parameterContainer">
        ///     The specified <see cref="IParameterContainer" />.
        /// </param>
        /// <returns>
        ///     <c>True</c> if the captcha is valid; otherwise, <c>false</c>.
        /// </returns>
        bool ValidateCaptcha(ControllerBase controller, IParameterContainer parameterContainer);
    }
}