using System.Web;

namespace CaptchaMvc.Interface
{
    /// <summary>
    ///     Represents the captcha model.
    /// </summary>
    public interface ICaptcha : IHtmlString
    {
        /// <summary>
        ///     Gets the <see cref="IBuildInfoModel"/>.
        /// </summary>
        IBuildInfoModel BuildInfo { get; }

        /// <summary>
        ///     Renders only captcha markup, if any.
        /// </summary>
        /// <returns>
        ///     An instance of <see cref="IHtmlString" />.
        /// </returns>
        IHtmlString RenderMarkup();

        /// <summary>
        ///     Renders only captcha scripts, if any.
        /// </summary>
        /// <returns>
        ///     An instance of <see cref="IHtmlString" />.
        /// </returns>
        IHtmlString RenderScript();
    }
}