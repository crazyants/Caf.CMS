using System;
using System.Web;
using CaptchaMvc.Infrastructure;
using CaptchaMvc.Interface;

namespace CaptchaMvc.Models
{
    /// <summary>
    ///     Represents the decorator to make captcha intelligent.
    /// </summary>
    public class IntelligentCaptchaDecorator : ICaptcha
    {
        #region Fields

        private readonly ICaptcha _captcha;
        private readonly Func<ICaptcha, string> _renderMarkup;
        private readonly Func<ICaptcha, string> _renderScript;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="IntelligentCaptchaDecorator" /> class.
        /// </summary>
        public IntelligentCaptchaDecorator(ICaptcha captcha, Func<ICaptcha, string> renderMarkup,
                                           Func<ICaptcha, string> renderScript, string policyType)
        {
            Validate.ArgumentNotNull(captcha, "captcha");
            Validate.ArgumentNotNull(renderMarkup, "renderMarkup");
            Validate.ArgumentNotNull(renderScript, "renderScript");
            Validate.ArgumentNotNull(policyType, "policyType");
            _captcha = captcha;
            _renderMarkup = renderMarkup;
            _renderScript = renderScript;
            PolicyType = policyType;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the type of the current decorator.
        /// </summary>
        public string PolicyType { get; private set; }

        #endregion

        #region Implementation of IHtmlString

        /// <summary>
        ///     Returns an HTML-encoded string.
        /// </summary>
        /// <returns>
        ///     An HTML-encoded string.
        /// </returns>
        public string ToHtmlString()
        {
            return string.Format("{0} {1}", RenderMarkup(), RenderScript());
        }

        #endregion

        #region Implementation of ICaptcha

        /// <summary>
        ///     Gets the <see cref="IBuildInfoModel" />.
        /// </summary>
        public IBuildInfoModel BuildInfo
        {
            get { return _captcha.BuildInfo; }
        }

        /// <summary>
        ///     Renders only captcha markup, if any.
        /// </summary>
        /// <returns>
        ///     An instance of <see cref="IHtmlString" />.
        /// </returns>
        public IHtmlString RenderMarkup()
        {
            return new HtmlString(_renderMarkup(_captcha));
        }

        /// <summary>
        ///     Renders only captcha scripts, if any.
        /// </summary>
        /// <returns>
        ///     An instance of <see cref="IHtmlString" />.
        /// </returns>
        public IHtmlString RenderScript()
        {
            return new HtmlString(_renderScript(_captcha));
        }

        #endregion
    }
}