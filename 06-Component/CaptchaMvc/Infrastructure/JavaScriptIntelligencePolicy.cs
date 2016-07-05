using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CaptchaMvc.Interface;
using CaptchaMvc.Models;

namespace CaptchaMvc.Infrastructure
{
    /// <summary>
    ///     Represents the policy which makes a captcha intelligent using the javascript.
    /// </summary>
    public class JavaScriptIntelligencePolicy : IIntelligencePolicy
    {
        #region Fields

        private const string PolicyType = "js";
        private const string SessionKey = "jsintelligencevalues";

        private const string InputHtml =
            @"<input type=""hidden"" value=""{0}"" name=""{1}"" id=""{1}""/>";

        private const string ScriptValue =
            @"<script>$(function () {{var tok = document.getElementById(""{0}"");var inv = tok.value.split('').reverse().join('');$('<input>').attr({{ type: 'hidden', name: '{1}', id: '{1}', value: inv }}).appendTo(tok.form);}});</script>";

        private string _validationInputName;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="JavaScriptIntelligencePolicy" /> class.
        /// </summary>
        public JavaScriptIntelligencePolicy(string validationInputName = null)
            : this(StorageType.Session, validationInputName)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="JavaScriptIntelligencePolicy" /> class.
        /// </summary>
        public JavaScriptIntelligencePolicy(StorageType storageType, string validationInputName = null)
        {
            if (string.IsNullOrEmpty(validationInputName))
                validationInputName = "validation_token";
            _validationInputName = validationInputName;
            StorageType = storageType;
            SessionValuesMaxCount = 15;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the name of input field, if you use the intelligent captcha.
        /// </summary>
        public string ValidationInputName
        {
            get { return _validationInputName; }
            set
            {
                Validate.PropertyNotNullOrEmpty(value, "ValidationInputName");
                _validationInputName = value;
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

            ValueProviderResult validationVal = controller.ValueProvider.GetValue(ValidationInputName);
            return captchaManager.StorageProvider.Remove(tokenValue.AttemptedValue) && validationVal != null
                && Reverse(validationVal.AttemptedValue) == tokenValue.AttemptedValue;
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
            return new IntelligentCaptchaDecorator(captcha, c => RenderMarkup(c, tokenName), c => RenderScript(tokenName), PolicyType);
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
        private string RenderScript(string tokenElementName)
        {
            return string.Format(ScriptValue, tokenElementName, ValidationInputName);
        }

        private static string Reverse(string st)
        {
            return new string(st.Reverse().ToArray());
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