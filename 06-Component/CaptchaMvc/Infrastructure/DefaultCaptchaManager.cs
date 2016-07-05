using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CaptchaMvc.Interface;
using CaptchaMvc.Models;

namespace CaptchaMvc.Infrastructure
{
    /// <summary>
    ///     Provides a basic methods for manage captcha.
    /// </summary>
    public class DefaultCaptchaManager : ICaptchaManager
    {
        #region Fields

        private Func<IParameterContainer, ICaptchaValue, IDrawingModel> _drawingModelFactory;
        private Func<string> _getCharacters;
        private Func<UrlHelper, KeyValuePair<string, ICaptchaValue>, string> _getImageUrl;
        private Func<UrlHelper, KeyValuePair<string, ICaptchaValue>, string> _getRefreshUrl;
        private string _imageElementName;
        private string _inputElementName;
        private Func<KeyValuePair<string, ICaptchaValue>> _mathCaptchaPairFactory;
        private Func<int, KeyValuePair<string, ICaptchaValue>> _plainCaptchaPairFactory;
        private IStorageProvider _storageProvider;
        private string _tokenElementName;
        private string _tokenParameterName;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultCaptchaManager" /> class.
        /// </summary>
        public DefaultCaptchaManager(IStorageProvider storageProvider)
            : this(storageProvider, "t", "CaptchaInputText", "CaptchaImage", "CaptchaDeText")
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultCaptchaManager" /> class.
        /// </summary>
        public DefaultCaptchaManager(IStorageProvider storageProvider, string tokenParameterName,
                                     string inputElementName, string imageElementName,
                                     string tokenElementName)
        {
            Validate.ArgumentNotNull(storageProvider, "storageProvider");
            Validate.ArgumentNotNullOrEmpty(tokenParameterName, "tokenParameterName");
            Validate.ArgumentNotNullOrEmpty(inputElementName, "inputElementName");
            Validate.ArgumentNotNullOrEmpty(imageElementName, "imageElementName");
            Validate.ArgumentNotNullOrEmpty(tokenElementName, "tokenElementName");

            IntelligencePolicy = new FakeInputIntelligencePolicy();
            StorageProvider = storageProvider;
            TokenParameterName = tokenParameterName;
            InputElementName = inputElementName;
            ImageElementName = imageElementName;
            TokenElementName = tokenElementName;

            ImageUrlFactory = GenerateImageUrl;
            RefreshUrlFactory = GenerateRefreshUrl;
            MathCaptchaPairFactory = GenerateMathCaptcha;
            PlainCaptchaPairFactory = GenerateSimpleCaptcha;
            DrawingModelFactory = CreateDrawingModel;
            CharactersFactory = GetCharacters;
            AddAreaRouteValue = true;
        }

        #endregion

        #region Parameters

        /// <summary>
        ///     Gets the parameter-key that indicates need render span for validation.
        /// </summary>
        public const string IsNeedValidationSpanAttribute = "__________IsNeedValidationSpanAttribute____________";

        /// <summary>
        ///     Gets the parameter-key for required attribute.
        /// </summary>
        public const string IsRequiredAttribute = "__________IsRequired____________";

        /// <summary>
        ///     Gets the parameter-key for required message.
        /// </summary>
        public const string RequiredMessageAttribute = "__________RequiredMessage____________";

        /// <summary>
        ///     Gets the parameter-key for error message.
        /// </summary>
        public const string ErrorAttribute = "______ErrorAttribute______";

        /// <summary>
        ///     Gets the parameter-key for length of characters.
        /// </summary>
        public const string LengthAttribute = "_______LengthAttribute_______";

        /// <summary>
        ///     Gets the parameter-key for refresh button text.
        /// </summary>
        public const string RefreshTextAttribute = "_______RefreshTextAttribute_______";

        /// <summary>
        ///     Gets the parameter-key for input text.
        /// </summary>
        public const string InputTextAttribute = "_______InputTextAttribute_______";

        /// <summary>
        ///     Gets the parameter-key for math parameter.
        /// </summary>
        public const string MathCaptchaAttribute = "__m__";

        /// <summary>
        ///     Gets the parameter-key for partial view name.
        /// </summary>
        public const string PartialViewNameAttribute = "____PartialViewNameAttribute____";

        /// <summary>
        ///     Gets the parameter-key for partial view data.
        /// </summary>
        public const string PartialViewDataAttribute = "____PartialViewDataAttribute____";

        /// <summary>
        ///     Gets the parameter-key for script partial view name.
        /// </summary>
        public const string ScriptPartialViewNameAttribute = "____ScriptPartialViewNameAttribute____";

        /// <summary>
        /// Gets the parameter-key for view data.
        /// </summary>
        public const string CaptchaNotValidViewDataKey = "IntelligentCaptchaNotValidaViewDataKey";

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the factory that generates a URL for a captcha image.
        /// </summary>
        public Func<UrlHelper, KeyValuePair<string, ICaptchaValue>, string> ImageUrlFactory
        {
            get { return _getImageUrl; }
            set
            {
                Validate.PropertyNotNull(value, "ImageUrlFactory");
                _getImageUrl = value;
            }
        }

        /// <summary>
        ///     Gets or sets the factory that generates a URL for a refresh captcha.
        /// </summary>
        public Func<UrlHelper, KeyValuePair<string, ICaptchaValue>, string> RefreshUrlFactory
        {
            get { return _getRefreshUrl; }
            set
            {
                Validate.PropertyNotNull(value, "RefreshUrlFactory");
                _getRefreshUrl = value;
            }
        }

        /// <summary>
        ///     Gets or sets the factory that generates a pair for a math captcha.
        /// </summary>
        public Func<KeyValuePair<string, ICaptchaValue>> MathCaptchaPairFactory
        {
            get { return _mathCaptchaPairFactory; }
            set
            {
                Validate.PropertyNotNull(value, "MathCaptchaPairFactory");
                _mathCaptchaPairFactory = value;
            }
        }

        /// <summary>
        ///     Gets or sets the factory that generates a pair for a plan captcha.
        /// </summary>
        public Func<int, KeyValuePair<string, ICaptchaValue>> PlainCaptchaPairFactory
        {
            get { return _plainCaptchaPairFactory; }
            set
            {
                Validate.PropertyNotNull(value, "PlainCaptchaPairFactory");
                _plainCaptchaPairFactory = value;
            }
        }

        /// <summary>
        ///     Gets or sets the factory that generates an instance of <see cref="IDrawingModel" />.
        /// </summary>
        public Func<IParameterContainer, ICaptchaValue, IDrawingModel> DrawingModelFactory
        {
            get { return _drawingModelFactory; }
            set
            {
                Validate.PropertyNotNull(value, "DrawingModelFactory");
                _drawingModelFactory = value;
            }
        }

        /// <summary>
        ///     Gets or sets the factory that gets the characters for creating captcha.
        /// </summary>
        public Func<string> CharactersFactory
        {
            get { return _getCharacters; }
            set
            {
                Validate.PropertyNotNull(value, "CharactersFactory");
                _getCharacters = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Creates an <see cref="IBuildInfoModel" /> for the specified <see cref="KeyValuePair{TKey,TValue}" />.
        /// </summary>
        /// <param name="htmlHelper">
        ///     The specified <see cref="HtmlHelper" />.
        /// </param>
        /// <param name="parameterContainer">
        ///     The specified <see cref="IParameterContainer" />.
        /// </param>
        /// <param name="captchaPair">
        ///     The specified <see cref="KeyValuePair{TKey,TValue}" />.
        /// </param>
        /// <param name="imgUrl">The specified image url.</param>
        /// <param name="refreshUrl">The specified refresh url.</param>
        /// <returns>
        ///     An instance of <see cref="IBuildInfoModel" />.
        /// </returns>
        protected virtual IBuildInfoModel CreateBuildInfo(HtmlHelper htmlHelper, IParameterContainer parameterContainer,
                                                          KeyValuePair<string, ICaptchaValue> captchaPair, string imgUrl,
                                                          string refreshUrl)
        {
            string requiredText = null;
            string refreshText;
            string inputText;
            parameterContainer.TryGet(RefreshTextAttribute, out refreshText, "Refresh");
            bool findInputText = parameterContainer.TryGet(InputTextAttribute, out inputText);
            bool isRequired = parameterContainer.IsContains(IsRequiredAttribute);
            if (isRequired)
                parameterContainer.TryGet(RequiredMessageAttribute, out requiredText, "This is a required field.");

            IBuildInfoModel buildInfo;
            if (parameterContainer.IsContains(MathCaptchaAttribute))
                buildInfo = new MathBuildInfoModel(parameterContainer, TokenParameterName, MathCaptchaAttribute, isRequired, requiredText,
                                                   refreshText, findInputText ? inputText : "The answer is", htmlHelper,
                                                   InputElementName, TokenElementName,
                                                   ImageElementName, imgUrl, refreshUrl, captchaPair.Key);
            else
                buildInfo = new DefaultBuildInfoModel(parameterContainer, TokenParameterName, requiredText, isRequired,
                                                      refreshText, findInputText ? inputText : "Input symbols",
                                                      htmlHelper,
                                                      InputElementName, ImageElementName, TokenElementName, refreshUrl,
                                                      imgUrl,
                                                      captchaPair.Key);

            //If it a partial view.
            if (parameterContainer.IsContains(PartialViewNameAttribute))
            {
                ViewDataDictionary viewData;
                parameterContainer.TryGet(PartialViewDataAttribute, out viewData);
                string scriptPartialView;
                parameterContainer.TryGet(ScriptPartialViewNameAttribute, out scriptPartialView);

                return new PartialBuildInfoModel(htmlHelper, buildInfo,
                                                 parameterContainer.Get<string>(PartialViewNameAttribute),
                                                 scriptPartialView, viewData);
            }
            return buildInfo;
        }

        /// <summary>
        ///     Generates a specified <see cref="KeyValuePair{TKey,TValue}" /> for a captcha.
        /// </summary>
        /// <param name="parameterContainer">
        ///     The specified <see cref="IParameterContainer" />.
        /// </param>
        /// <param name="oldValue">The old value if any.</param>
        /// <returns>
        ///     An instance of <see cref="KeyValuePair{TKey,TValue}" />.
        /// </returns>
        protected virtual KeyValuePair<string, ICaptchaValue> CreateCaptchaPair(IParameterContainer parameterContainer,
                                                                                ICaptchaValue oldValue)
        {
            if (parameterContainer.IsContains(MathCaptchaAttribute))
                return MathCaptchaPairFactory();

            int length;
            if (oldValue != null)
                length = oldValue.CaptchaText.Length;
            else if (!parameterContainer.TryGet(LengthAttribute, out length))
                throw new ArgumentException("Parameter is not specified for the length of the captcha.");
            if (length <= 0)
                throw new ArgumentException("The length parameter can not be <= 0.");
            return PlainCaptchaPairFactory(length);
        }

        /// <summary>
        ///     Writes an error message.
        /// </summary>
        /// <param name="controllerBase">
        ///     The specified <see cref="ControllerBase" />.
        /// </param>
        /// <param name="parameterContainer">
        ///     The specified <see cref="IParameterContainer" />.
        /// </param>
        protected virtual void WriteError(ControllerBase controllerBase, IParameterContainer parameterContainer)
        {
            string errorText;
            parameterContainer.TryGet(ErrorAttribute, out errorText, "The captcha is not valid");
            controllerBase.ViewData.ModelState.AddModelError(InputElementName, errorText);
            controllerBase.ViewData[CaptchaNotValidViewDataKey] = true;
        }

        /// <summary>
        ///     Creates a new <see cref="IDrawingModel" /> for drawing a captcha.
        /// </summary>
        /// <param name="parameterContainer">
        ///     The specified <see cref="IParameterContainer" />.
        /// </param>
        /// <param name="captchaValue">
        ///     The specified <see cref="ICaptchaValue" />.
        /// </param>
        /// <returns>
        ///     An instance of <see cref="IDrawingModel" />.
        /// </returns>
        private static IDrawingModel CreateDrawingModel(IParameterContainer parameterContainer,
                                                           ICaptchaValue captchaValue)
        {
            return new DefaultDrawingModel(captchaValue.CaptchaText);
        }

        /// <summary>
        ///     Generates a specified <see cref="KeyValuePair{TKey,TValue}" /> for a math captcha.
        /// </summary>
        /// <returns>
        ///     An instance of <see cref="KeyValuePair{TKey,TValue}" />.
        /// </returns>
        private static KeyValuePair<string, ICaptchaValue> GenerateMathCaptcha()
        {
            int first, second;
            string text;
            int result;
            int next = RandomNumber.Next(0, 1);
            switch (next)
            {
                case 0:
                    first = RandomNumber.Next(1, 50);
                    second = RandomNumber.Next(1, 50);
                    text = string.Format("{0}+{1}=?", first, second);
                    result = first + second;
                    break;
                case 1:
                    first = RandomNumber.Next(50, 99);
                    second = RandomNumber.Next(1, 49);
                    text = string.Format("{0}-{1}=?", first, second);
                    result = first - second;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return new KeyValuePair<string, ICaptchaValue>(Guid.NewGuid().ToString("N"),
                                                           new NumberCaptchaValue(text, result));
        }

        /// <summary>
        ///     Generates a specified <see cref="KeyValuePair{TKey,TValue}" /> for a text captcha.
        /// </summary>
        /// <param name="length">The specified length of characters.</param>
        /// <returns>
        ///     An instance of <see cref="KeyValuePair{TKey,TValue}" />.
        /// </returns>
        private KeyValuePair<string, ICaptchaValue> GenerateSimpleCaptcha(int length)
        {
            string randomText = RandomText.Generate(CharactersFactory(), length);
            return new KeyValuePair<string, ICaptchaValue>(Guid.NewGuid().ToString("N"),
                                                           new StringCaptchaValue(randomText, randomText, true));
        }

        /// <summary>
        ///     Generates an URL for a captcha image.
        /// </summary>
        /// <param name="urlHelper">
        ///     The specified <see cref="UrlHelper" />.
        /// </param>
        /// <param name="captchaPair">
        ///     The specified <see cref="KeyValuePair{TKey,TValue}" />.
        /// </param>
        /// <returns>The url of captcha image.</returns>
        private string GenerateImageUrl(UrlHelper urlHelper, KeyValuePair<string, ICaptchaValue> captchaPair)
        {
            var routeValueDictionary = new RouteValueDictionary { { TokenParameterName, captchaPair.Key } };
            if (AddAreaRouteValue)
                routeValueDictionary.Add("area", "");
            var url = urlHelper.Action("Generate", "DefaultCaptcha", routeValueDictionary);
            if (string.IsNullOrEmpty(url))
                url = urlHelper.RouteUrl("Captcha", routeValueDictionary);
            return url;
        }

        /// <summary>
        ///     Generates an URL to refresh captcha.
        /// </summary>
        /// <param name="urlHelper">
        ///     The specified <see cref="UrlHelper" />.
        /// </param>
        /// <param name="captchaPair">
        ///     The specified <see cref="KeyValuePair{TKey,TValue}" />.
        /// </param>
        /// <returns>The url to refresh captcha.</returns>
        private string GenerateRefreshUrl(UrlHelper urlHelper, KeyValuePair<string, ICaptchaValue> captchaPair)
        {
            if (AddAreaRouteValue)
                return urlHelper.Action("Refresh", "DefaultCaptcha", new { area = "" });
            var url = urlHelper.Action("Refresh", "DefaultCaptcha");

            if (string.IsNullOrEmpty(url))
                url = urlHelper.RouteUrl("RefreshCaptcha");
            return url;
        }

        /// <summary>
        ///     Gets the characters to create captcha.
        /// </summary>
        /// <returns>
        ///     An instance of <see cref="string" />.
        /// </returns>
        private static string GetCharacters()
        {
            string chars = ConfigurationManager.AppSettings["CaptchaChars"];
            if (string.IsNullOrEmpty(chars))
                chars = "QWERTYUIPASDFGHJKLZCVBNM";
            return chars;
        }

        #endregion

        #region Implementation of ICaptchaManager

        /// <summary>
        ///     Gets or sets the name for a token parameter.
        /// </summary>
        public string TokenParameterName
        {
            get { return _tokenParameterName; }
            set
            {
                Validate.PropertyNotNullOrEmpty(value, "TokenParameterName");
                _tokenParameterName = value;
            }
        }

        /// <summary>
        ///     Gets or sets the name for an input element in DOM.
        /// </summary>
        public string InputElementName
        {
            get { return _inputElementName; }
            set
            {
                Validate.PropertyNotNullOrEmpty(value, "InputElementName");
                _inputElementName = value;
            }
        }

        /// <summary>
        ///     Gets or sets the name for image element in DOM.
        /// </summary>
        public string ImageElementName
        {
            get { return _imageElementName; }
            set
            {
                Validate.PropertyNotNullOrEmpty(value, "ImageElementName");
                _imageElementName = value;
            }
        }

        /// <summary>
        ///     Gets or sets the name for token element in DOM.
        /// </summary>
        public string TokenElementName
        {
            get { return _tokenElementName; }
            set
            {
                Validate.PropertyNotNullOrEmpty(value, "TokenElementName");
                _tokenElementName = value;
            }
        }

        /// <summary>
        /// Gets or sets value that indicates that the current manager will add area route value in generated url.
        /// </summary>
        public bool AddAreaRouteValue { get; set; }

        /// <summary>
        ///     Gets or sets the storage to save a captcha tokens.
        /// </summary>
        public IStorageProvider StorageProvider
        {
            get { return _storageProvider; }
            set
            {
                Validate.PropertyNotNull(value, "StorageProvider");
                _storageProvider = value;
            }
        }

        /// <summary>
        ///     Gets or sets the intelligence policy.
        /// </summary>
        public IIntelligencePolicy IntelligencePolicy { get; set; }

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
        public virtual IBuildInfoModel GenerateNew(HtmlHelper htmlHelper, IParameterContainer parameterContainer)
        {
            Validate.ArgumentNotNull(htmlHelper, "htmlHelper");
            Validate.ArgumentNotNull(parameterContainer, "parameterContainer");
            KeyValuePair<string, ICaptchaValue> captchaPair = CreateCaptchaPair(parameterContainer, null);
            StorageProvider.Add(captchaPair);
            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            string imgUrl = ImageUrlFactory(urlHelper, captchaPair);
            string refreshUrl = RefreshUrlFactory(urlHelper, captchaPair);
            return CreateBuildInfo(htmlHelper, parameterContainer, captchaPair, imgUrl, refreshUrl);
        }

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
        public virtual IUpdateInfoModel GenerateNew(ControllerBase controller, IParameterContainer parameterContainer)
        {
            Validate.ArgumentNotNull(controller, "controller");
            Validate.ArgumentNotNull(parameterContainer, "parameterContainer");
            KeyValuePair<string, ICaptchaValue> captchaPair = CreateCaptchaPair(parameterContainer, null);
            StorageProvider.Add(captchaPair);
            var urlHelper = new UrlHelper(controller.ControllerContext.RequestContext);
            string imgUrl = ImageUrlFactory(urlHelper, captchaPair);
            return new DefaultUpdateInfoModel(TokenElementName, captchaPair.Key, imgUrl, ImageElementName);
        }

        /// <summary>
        ///     Creates a new <see cref="IDrawingModel" /> for drawing a captcha.
        /// </summary>
        /// <param name="parameterContainer">
        ///     The specified <see cref="IParameterContainer" />.
        /// </param>
        /// <returns>
        ///     An instance of <see cref="IDrawingModel" />.
        /// </returns>
        public virtual IDrawingModel GetDrawingModel(IParameterContainer parameterContainer)
        {
            Validate.ArgumentNotNull(parameterContainer, "parameterContainer");
            string token;
            if (!parameterContainer.TryGet(TokenParameterName, out token) || string.IsNullOrEmpty(token))
                throw new KeyNotFoundException("The key is to generate not found.");
            ICaptchaValue captchaValue = StorageProvider.GetValue(token, TokenType.Drawing);
            if (captchaValue == null)
                throw new ArgumentException("The key is to generate incorrect.");
            return DrawingModelFactory(parameterContainer, captchaValue);
        }

        /// <summary>
        ///     Creates a new <see cref="IBuildInfoModel" /> for update a captcha.
        /// </summary>
        /// <param name="parameterContainer">
        ///     The specified <see cref="IParameterContainer" />.
        /// </param>
        /// <returns>
        ///     An instance of <see cref="IUpdateInfoModel" />.
        /// </returns>
        public IUpdateInfoModel Update(IParameterContainer parameterContainer)
        {
            Validate.ArgumentNotNull(parameterContainer, "parameterContainer");
            string token;
            parameterContainer.TryGet(TokenParameterName, out token, null);
            if (string.IsNullOrEmpty(token))
                throw new KeyNotFoundException("The key is to generate not found.");
            ICaptchaValue captchaValue = StorageProvider.GetValue(token, TokenType.Validation);
            if (captchaValue == null)
                throw new ArgumentException("The key is to update incorrect.");

            HttpRequestBase request;
            if (!parameterContainer.TryGet(RequestParameterContainer.HttpRequestParameterKey, out request) ||
                request == null)
                throw new InvalidOperationException(
                    "The parameterContainer does not contain a HttpRequestBase with key RequestParameterContainer.HttpRequestParameterKey.");
            KeyValuePair<string, ICaptchaValue> captchaPair = CreateCaptchaPair(parameterContainer, captchaValue);
            string newUrl = ImageUrlFactory(new UrlHelper(request.RequestContext), captchaPair);
            StorageProvider.Add(captchaPair);
            return new DefaultUpdateInfoModel(TokenElementName, captchaPair.Key, newUrl, ImageElementName);
        }

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
        public virtual bool ValidateCaptcha(ControllerBase controller, IParameterContainer parameterContainer)
        {
            Validate.ArgumentNotNull(controller, "controller");
            Validate.ArgumentNotNull(parameterContainer, "parameterContainer");
            if (IntelligencePolicy != null)
            {
                var isValid = IntelligencePolicy.IsValid(this, controller, parameterContainer);
                if (isValid.HasValue)
                {
                    if (isValid.Value)
                        return true;
                    WriteError(controller, parameterContainer);
                    return false;
                }
            }
            ValueProviderResult tokenValue = controller.ValueProvider.GetValue(TokenElementName);
            ValueProviderResult inputText = controller.ValueProvider.GetValue(InputElementName);
            if (tokenValue != null && !string.IsNullOrEmpty(tokenValue.AttemptedValue) && inputText != null)
            {
                ICaptchaValue captchaValue = StorageProvider.GetValue(tokenValue.AttemptedValue, TokenType.Validation);
                if (captchaValue != null && !string.IsNullOrEmpty(inputText.AttemptedValue) &&
                    captchaValue.IsEqual(inputText.AttemptedValue))
                    return true;
            }
            WriteError(controller, parameterContainer);
            return false;
        }

        #endregion
    }
}