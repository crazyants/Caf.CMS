using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CaptchaMvc.Interface;
using CaptchaMvc.Models;

namespace CaptchaMvc.Infrastructure
{
    /// <summary>
    ///     Represents the policy which makes a captcha as intelligent using array of IIntelligencePolicy.
    /// </summary>
    public class MultiIntelligencePolicy : IIntelligencePolicy
    {
        #region Fields

        private const string InputHtml =
            @"<input type=""hidden"" value=""{0}"" name=""{1}"" id=""{1}""/>";
        private const string PolicyType = "mp";

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MultiIntelligencePolicy" /> class.
        /// </summary>
        public MultiIntelligencePolicy(params IIntelligencePolicy[] policies)
        {
            Validate.ArgumentNotNull(policies, "policies");
            if (policies.Length == 0)
                throw new ArgumentException("Argument cannot be empty.", "policies");
            if (policies.Any(policy => policy.GetType() == typeof (MultiIntelligencePolicy)))
                throw new ArgumentException("The argument can be of MultiIntelligencePolicy type.", "policies");
            Policies = policies;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the array of IIntelligencePolicy.
        /// </summary>
        public IList<IIntelligencePolicy> Policies { get; private set; }

        #endregion

        #region Implementation of IIntelligencePolicy

        /// <summary>
        ///     Determines whether the intelligent captcha is valid.
        /// </summary>
        /// <param name="captchaManager">The specified captcha manager.</param>
        /// <param name="controller">
        ///     The specified <see cref="ControllerBase" />.
        /// </param>
        /// <param name="parameterContainer">
        ///     The specified <see cref="IParameterContainer" />.
        /// </param>
        /// <returns>
        ///     <c>True</c> if the intelligent captcha is valid; <c>false</c> not valid; <c>null</c> is not intelligent captcha.
        /// </returns>
        public virtual bool? IsValid(ICaptchaManager captchaManager, ControllerBase controller, IParameterContainer parameterContainer)
        {
            Validate.ArgumentNotNull(captchaManager, "captchaManager");
            Validate.ArgumentNotNull(controller, "controller");
            ValueProviderResult tokenValue = controller
                .ValueProvider
                .GetValue(captchaManager.TokenElementName + PolicyType);
            if (tokenValue == null || string.IsNullOrEmpty(tokenValue.AttemptedValue))
                return null;
            var captchaValue = captchaManager.StorageProvider.GetValue(tokenValue.AttemptedValue, TokenType.Drawing);
            if (captchaValue == null)
                return null;

            foreach (IIntelligencePolicy intelligencePolicy in Policies)
            {
                bool? valid = intelligencePolicy.IsValid(captchaManager, controller, parameterContainer);
                if (valid == null)
                    return null;
                if (!valid.Value)
                    return false;
                captchaManager.StorageProvider.Remove(tokenValue.AttemptedValue);
                captchaManager.StorageProvider.Add(new KeyValuePair<string, ICaptchaValue>(tokenValue.AttemptedValue, captchaValue));
            }
            captchaManager.StorageProvider.Remove(tokenValue.AttemptedValue);
            return true;
        }

        /// <summary>
        ///     Makes the specified captcha "intelligent".
        /// </summary>
        /// <param name="captchaManager">The specified captcha manager.</param>
        /// <param name="captcha">
        ///     The specified <see cref="ICaptcha" />.
        /// </param>
        /// <param name="parameterContainer">
        ///     The specified <see cref="IParameterContainer" />.
        /// </param>
        /// <returns>
        ///     An instance of <see cref="ICaptcha" />.
        /// </returns>
        public virtual ICaptcha MakeIntelligent(ICaptchaManager captchaManager, ICaptcha captcha, IParameterContainer parameterContainer)
        {
            Validate.ArgumentNotNull(captchaManager, "captchaManager");
            Validate.ArgumentNotNull(captcha, "captcha");
            if (captcha.BuildInfo.HtmlHelper.ViewData[DefaultCaptchaManager.CaptchaNotValidViewDataKey] != null)
                return captcha;
            var captchaDecorator = captcha as IntelligentCaptchaDecorator;
            if (captchaDecorator != null && captchaDecorator.PolicyType.Equals(PolicyType))
                return captcha;

            var tokenName = captchaManager.TokenElementName + PolicyType;
            var markup = new StringBuilder(RenderMarkup(captcha, tokenName));
            var script = new StringBuilder();
            foreach (IIntelligencePolicy intelligencePolicy in Policies)
            {
                ICaptcha cp = intelligencePolicy.MakeIntelligent(captchaManager, captcha, parameterContainer);
                markup.AppendLine(cp.RenderMarkup().ToHtmlString());
                script.AppendLine(cp.RenderScript().ToHtmlString());
            }
            return new IntelligentCaptchaDecorator(captcha, captcha1 => markup.ToString(), captcha1 => script.ToString(),
                PolicyType);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Renders only captcha markup, if any.
        /// </summary>
        /// <returns>
        ///     An instance of string.
        /// </returns>
        private static string RenderMarkup(ICaptcha captcha, string tokenElementName)
        {
            return string.Format(InputHtml, captcha.BuildInfo.TokenValue, tokenElementName);
        }

        #endregion
    }
}