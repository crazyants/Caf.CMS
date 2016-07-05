using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CaptchaMvc.Interface;
using CaptchaMvc.Models;

namespace CaptchaMvc.Infrastructure
{
    /// <summary>
    ///     Represents the policy which makes a captcha as intelligent using a fake input element.
    /// </summary>
    public class FakeInputIntelligencePolicy : IIntelligencePolicy
    {
        #region Fields

        private const string PolicyType = "fi";
        private const string SessionKey = "fiintelligencevalues";

        private const string StyleCss = "<style>#{0} {{ display: none; }}</style>";

        private const string InputHtml =
            @"<input type=""hidden"" value=""{0}"" name=""{1}"" id=""{1}""/><input type=""text"" value="""" name=""{2}"" id=""{2}""/>";

        private string _fakeInputName;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="FakeInputIntelligencePolicy" /> class.
        /// </summary>
        public FakeInputIntelligencePolicy(string fakeInputName = null)
            : this(StorageType.Session, fakeInputName)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FakeInputIntelligencePolicy" /> class.
        /// </summary>
        public FakeInputIntelligencePolicy(StorageType storageType, string fakeInputName = null)
        {
            if (string.IsNullOrEmpty(fakeInputName))
                fakeInputName = "email_required_value";
            _fakeInputName = fakeInputName;
            StorageType = storageType;
            SessionValuesMaxCount = 15;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the name of input field, if you use the intelligent captcha.
        /// </summary>
        public string FakeInputName
        {
            get { return _fakeInputName; }
            set
            {
                Validate.PropertyNotNullOrEmpty(value, "FakeInputName");
                _fakeInputName = value;
            }
        }

        /// <summary>
        ///     Gets the type of storage that will be used for store a data.
        /// </summary>
        public StorageType StorageType { get; private set; }

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
            if (!RemoveTokenValue(controller, tokenValue.AttemptedValue))
                return null;

            ValueProviderResult fakeValue = controller.ValueProvider.GetValue(FakeInputName);
            return captchaManager.StorageProvider.Remove(tokenValue.AttemptedValue) && fakeValue != null &&
                   string.IsNullOrEmpty(fakeValue.AttemptedValue);
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
        private string RenderMarkup(ICaptcha captcha, string tokenElementName)
        {
            return string.Format(InputHtml, captcha.BuildInfo.TokenValue, tokenElementName, FakeInputName);
        }

        /// <summary>
        ///     Renders only captcha scripts, if any.
        /// </summary>
        /// <returns>
        ///     An instance of string.
        /// </returns>
        private string RenderScript(ICaptcha captcha)
        {
            return string.Format(StyleCss, FakeInputName);
        }

        private void SaveTokenValue(ICaptcha captcha)
        {
            switch (StorageType)
            {
                case StorageType.TempData:
                    captcha.BuildInfo.HtmlHelper.ViewContext.TempData[captcha.BuildInfo.TokenValue] = true;
                    break;
                case StorageType.Session:
                    var hashSet = CaptchaUtils.GetFromSession(SessionKey, () => new HashSet<KeyTimeEntry<string>>());
                    hashSet.ClearIfNeed(SessionValuesMaxCount);
                    hashSet.Add(captcha.BuildInfo.TokenValue);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool RemoveTokenValue(ControllerBase controller, string value)
        {
            switch (StorageType)
            {
                case StorageType.TempData:
                    return controller.TempData.Remove(value);
                case StorageType.Session:
                    var hashSet = CaptchaUtils.GetFromSession(SessionKey, () => new HashSet<KeyTimeEntry<string>>());
                    return hashSet.Remove(value);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion
    }
}