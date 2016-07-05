using System;
using System.Collections.Concurrent;
using System.Web.Mvc;
using CaptchaMvc.Interface;
using CaptchaMvc.Models;

namespace CaptchaMvc.Infrastructure
{
    /// <summary>
    ///     Represents the policy which makes a captcha as intelligent using the difference between the time of the request and response.
    /// </summary>
    public class ResponseTimeIntelligencePolicy : IIntelligencePolicy
    {
        #region Fields

        private const string PolicyType = "rt";
        private const string SessionKey = "rtintelligencevalues";

        private const string InputHtml =
            @"<input type=""hidden"" value=""{0}"" name=""{1}"" id=""{1}""/>";

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="FakeInputIntelligencePolicy" /> class.
        /// </summary>
        public ResponseTimeIntelligencePolicy(TimeSpan minResponseTime)
            : this(StorageType.Session, minResponseTime)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FakeInputIntelligencePolicy" /> class.
        /// </summary>
        public ResponseTimeIntelligencePolicy(StorageType storageType, TimeSpan minResponseTime)
        {
            StorageType = storageType;
            MinResponseTime = minResponseTime;
            SessionValuesMaxCount = 15;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the type of storage that will be used for store a data.
        /// </summary>
        public StorageType StorageType { get; private set; }

        /// <summary>
        ///     Gets the minimum time at which the policy is valid.
        /// </summary>
        public TimeSpan MinResponseTime { get; private set; }

        /// <summary>
        ///     Gets or sets the maximum size of session values.
        /// </summary>
        public uint SessionValuesMaxCount { get; set; }

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
            DateTime? dateTime = GetTokenValue(controller, tokenValue.AttemptedValue);
            if (dateTime == null)
                return null;
            return captchaManager.StorageProvider.Remove(tokenValue.AttemptedValue) && DateTime.UtcNow - dateTime.Value > MinResponseTime;
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
            SaveTokenValue(captcha);
            var tokenName = captchaManager.TokenElementName + PolicyType;
            return new IntelligentCaptchaDecorator(captcha, c => RenderMarkup(c, tokenName), RenderScript, PolicyType);
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

        /// <summary>
        ///     Renders only captcha scripts, if any.
        /// </summary>
        /// <returns>
        ///     An instance of string.
        /// </returns>
        private static string RenderScript(ICaptcha captcha)
        {
            return string.Empty;
        }

        private void SaveTokenValue(ICaptcha captcha)
        {
            switch (StorageType)
            {
                case StorageType.TempData:
                    captcha.BuildInfo.HtmlHelper.ViewContext.TempData[captcha.BuildInfo.TokenValue] = DateTime.UtcNow;
                    break;
                case StorageType.Session:
                    var dictionary = CaptchaUtils.GetFromSession(SessionKey, () => new ConcurrentDictionary<KeyTimeEntry<string>, DateTime>());
                    dictionary.ClearIfNeed(SessionValuesMaxCount);
                    dictionary.TryAdd(captcha.BuildInfo.TokenValue, DateTime.UtcNow);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private DateTime? GetTokenValue(ControllerBase controller, string value)
        {
            switch (StorageType)
            {
                case StorageType.TempData:
                    if (controller.TempData.ContainsKey(value))
                    {
                        var result = (DateTime)controller.TempData[value];
                        controller.TempData.Remove(value);
                        return result;
                    }
                    return null;
                case StorageType.Session:
                    var dictionary = CaptchaUtils.GetFromSession(SessionKey, () => new ConcurrentDictionary<KeyTimeEntry<string>, DateTime>());
                    DateTime time;
                    if (dictionary.TryRemove(value, out time))
                        return time;
                    return null;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion
    }
}