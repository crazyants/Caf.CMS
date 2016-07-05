using System;
using System.Web;
using System.Web.Mvc;
using CaptchaMvc.Infrastructure;
using CaptchaMvc.Interface;

namespace CaptchaMvc.Models
{
    /// <summary>
    ///     Represents the default captcha model.
    /// </summary>
    public class CaptchaModel : ICaptcha
    {
        #region Fields

        private readonly string _markup;
        private readonly string _script;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CaptchaModel" /> class.
        /// </summary>
        public CaptchaModel(IBuildInfoModel buildInfoModel, string markup, string script)
        {
            Validate.ArgumentNotNull(buildInfoModel, "buildInfoModel");
            Validate.ArgumentNotNull(markup, "markup");
            _markup = markup;
            if (script == null)
                script = string.Empty;
            _script = script;
            BuildInfo = buildInfoModel;
        }

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
            if (string.IsNullOrEmpty(_script))
                return _markup;
            return string.Format("{0} {1}", _script, _markup);
        }

        #endregion

        #region Implementation of ICaptcha

        /// <summary>
        ///     Gets the <see cref="IBuildInfoModel"/>.
        /// </summary>
        public IBuildInfoModel BuildInfo { get; private set; }

        /// <summary>
        ///     Renders only captcha markup, if any.
        /// </summary>
        /// <returns>
        ///     An instance of <see cref="IHtmlString" />.
        /// </returns>
        public IHtmlString RenderMarkup()
        {
            return new MvcHtmlString(_markup);
        }

        /// <summary>
        ///     Renders only captcha scripts, if any.
        /// </summary>
        /// <returns>
        ///     An instance of <see cref="IHtmlString" />.
        /// </returns>
        public IHtmlString RenderScript()
        {
            return new MvcHtmlString(_script);
        }

        #endregion
    }
}