using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CaptchaMvc.Infrastructure;
using CaptchaMvc.Interface;
using CaptchaMvc.Models;
using JetBrains.Annotations;

namespace CaptchaMvc.HtmlHelpers
{
    /// <summary>
    ///     Provides extension methods to work with a captcha.
    /// </summary>
    public static class CaptchaHelper
    {
        #region Public methods

        /// <summary>
        ///     Creates a new captcha with the specified arguments.
        /// </summary>
        /// <param name="htmlHelper">
        ///     The specified <see cref="HtmlHelper" />.
        /// </param>
        /// <param name="length">The specified length of characters.</param>
        /// <param name="parameters">The specified parameters, if any.</param>
        /// <returns>
        ///     An instance of <see cref="ICaptcha" />.
        /// </returns>
        public static ICaptcha Captcha(this HtmlHelper htmlHelper, int length, params ParameterModel[] parameters)
        {
            List<ParameterModel> list = CaptchaUtils.GetParameters(parameters);
            list.Add(new ParameterModel(DefaultCaptchaManager.LengthAttribute, length));
            return CaptchaUtils.GenerateCaptcha(htmlHelper, list);
        }

        /// <summary>
        ///     Creates a new captcha with the specified arguments.
        /// </summary>
        /// <param name="htmlHelper">
        ///     The specified <see cref="HtmlHelper" />.
        /// </param>
        /// <param name="refreshText">The specified refresh button text.</param>
        /// <param name="inputText">The specified input text.</param>
        /// <param name="length">The specified length of characters.</param>
        /// <param name="parameters">The specified parameters, if any.</param>
        /// <returns>
        ///     An instance of <see cref="ICaptcha" />.
        /// </returns>
        public static ICaptcha Captcha(this HtmlHelper htmlHelper, string refreshText, string inputText,
                                       int length, params ParameterModel[] parameters)
        {
            List<ParameterModel> list = CaptchaUtils.GetParameters(parameters);
            list.Add(new ParameterModel(DefaultCaptchaManager.InputTextAttribute, inputText));
            list.Add(new ParameterModel(DefaultCaptchaManager.RefreshTextAttribute, refreshText));
            list.Add(new ParameterModel(DefaultCaptchaManager.LengthAttribute, length));
            return CaptchaUtils.GenerateCaptcha(htmlHelper, list);
        }

        /// <summary>
        ///     Creates a new captcha with the specified arguments.
        /// </summary>
        /// <param name="htmlHelper">
        ///     The specified <see cref="HtmlHelper" />.
        /// </param>
        /// <param name="refreshText">The specified refresh button text.</param>
        /// <param name="inputText">The specified input text.</param>
        /// <param name="length">The specified length of characters.</param>
        /// <param name="requiredMessageText">The specified required message text.</param>
        /// <param name="addValidationSpan">
        ///     If <c>true</c> add a span for validation; otherwise <c>false</c>.
        /// </param>
        /// <param name="parameters">The specified parameters, if any.</param>
        /// <returns>
        ///     An instance of <see cref="ICaptcha" />.
        /// </returns>
        public static ICaptcha Captcha(this HtmlHelper htmlHelper, string refreshText, string inputText,
                                       int length, string requiredMessageText, bool addValidationSpan = false,
                                       params ParameterModel[] parameters)
        {
            List<ParameterModel> list = CaptchaUtils.GetParameters(parameters);
            list.Add(new ParameterModel(DefaultCaptchaManager.InputTextAttribute, inputText));
            list.Add(new ParameterModel(DefaultCaptchaManager.RefreshTextAttribute, refreshText));
            list.Add(new ParameterModel(DefaultCaptchaManager.LengthAttribute, length));
            list.Add(new ParameterModel(DefaultCaptchaManager.IsRequiredAttribute, true));
            list.Add(new ParameterModel(DefaultCaptchaManager.RequiredMessageAttribute, requiredMessageText));
            list.Add(new ParameterModel(DefaultCaptchaManager.IsNeedValidationSpanAttribute, addValidationSpan));
            return CaptchaUtils.GenerateCaptcha(htmlHelper, list);
        }

        /// <summary>
        ///     Creates a new captcha with the specified partial view.
        /// </summary>
        /// <param name="htmlHelper">
        ///     The specified <see cref="HtmlHelper" />.
        /// </param>
        /// <param name="length">The specified length of characters.</param>
        /// <param name="partialViewName">The name of the partial view to render.</param>
        /// <param name="viewData">The view data dictionary for the partial view.</param>
        /// <param name="parameters">The specified parameters, if any.</param>
        /// <returns>
        ///     An instance of <see cref="ICaptcha" />.
        /// </returns>
        public static ICaptcha Captcha(this HtmlHelper htmlHelper, int length,
                                       [AspMvcPartialView] string partialViewName,
                                       ViewDataDictionary viewData = null, params ParameterModel[] parameters)
        {
            Validate.ArgumentNotNullOrEmpty(partialViewName, "partialViewName");
            List<ParameterModel> list = CaptchaUtils.GetParameters(parameters);
            list.Add(new ParameterModel(DefaultCaptchaManager.LengthAttribute, length));
            list.Add(new ParameterModel(DefaultCaptchaManager.PartialViewNameAttribute, partialViewName));
            if (viewData != null)
                list.Add(new ParameterModel(DefaultCaptchaManager.PartialViewDataAttribute, viewData));
            return CaptchaUtils.GenerateCaptcha(htmlHelper, list);
        }

        /// <summary>
        ///     Creates a new captcha with the specified partial view.
        /// </summary>
        /// <param name="htmlHelper">
        ///     The specified <see cref="HtmlHelper" />.
        /// </param>
        /// <param name="length">The specified length of characters.</param>
        /// <param name="partialViewName">The name of the partial view to render.</param>
        /// <param name="scriptPartialViewName">The name of the partial view to render script.</param>
        /// <param name="viewData">The view data dictionary for the partial view.</param>
        /// <param name="parameters">The specified parameters, if any.</param>
        /// <returns>
        ///     An instance of <see cref="ICaptcha" />.
        /// </returns>
        public static ICaptcha Captcha(this HtmlHelper htmlHelper, int length,
                                       [AspMvcPartialView] string partialViewName,
                                       [AspMvcPartialView] string scriptPartialViewName,
                                       ViewDataDictionary viewData = null, params ParameterModel[] parameters)
        {
            Validate.ArgumentNotNullOrEmpty(partialViewName, "partialViewName");
            Validate.ArgumentNotNullOrEmpty(scriptPartialViewName, "scriptPartialViewName");
            List<ParameterModel> list = CaptchaUtils.GetParameters(parameters);
            list.Add(new ParameterModel(DefaultCaptchaManager.LengthAttribute, length));
            list.Add(new ParameterModel(DefaultCaptchaManager.PartialViewNameAttribute, partialViewName));
            list.Add(new ParameterModel(DefaultCaptchaManager.ScriptPartialViewNameAttribute, scriptPartialViewName));
            if (viewData != null)
                list.Add(new ParameterModel(DefaultCaptchaManager.PartialViewDataAttribute, viewData));
            return CaptchaUtils.GenerateCaptcha(htmlHelper, list);
        }

        /// <summary>
        ///     Creates a new math captcha with the specified arguments.
        /// </summary>
        /// <param name="htmlHelper">
        ///     The specified <see cref="HtmlHelper" />.
        /// </param>
        /// <param name="parameters">The specified parameters, if any.</param>
        /// <returns>
        ///     An instance of <see cref="ICaptcha" />.
        /// </returns>
        public static ICaptcha MathCaptcha(this HtmlHelper htmlHelper, params ParameterModel[] parameters)
        {
            List<ParameterModel> list = CaptchaUtils.GetParameters(parameters);
            list.Add(new ParameterModel(DefaultCaptchaManager.MathCaptchaAttribute, true));
            return CaptchaUtils.GenerateCaptcha(htmlHelper, list);
        }

        /// <summary>
        ///     Creates a new math captcha with the specified arguments.
        /// </summary>
        /// <param name="htmlHelper">
        ///     The specified <see cref="HtmlHelper" />.
        /// </param>
        /// <param name="refreshText">The specified refresh button text.</param>
        /// <param name="inputText">The specified input text.</param>
        /// <param name="parameters">The specified parameters, if any.</param>
        /// <returns>
        ///     An instance of <see cref="ICaptcha" />.
        /// </returns>
        public static ICaptcha MathCaptcha(this HtmlHelper htmlHelper, string refreshText, string inputText,
                                           params ParameterModel[] parameters)
        {
            List<ParameterModel> list = CaptchaUtils.GetParameters(parameters);
            list.Add(new ParameterModel(DefaultCaptchaManager.InputTextAttribute, inputText));
            list.Add(new ParameterModel(DefaultCaptchaManager.RefreshTextAttribute, refreshText));
            list.Add(new ParameterModel(DefaultCaptchaManager.MathCaptchaAttribute, true));
            return CaptchaUtils.GenerateCaptcha(htmlHelper, list);
        }

        /// <summary>
        ///     Creates a new math captcha with the specified arguments.
        /// </summary>
        /// <param name="htmlHelper">
        ///     The specified <see cref="HtmlHelper" />.
        /// </param>
        /// <param name="refreshText">The specified refresh button text.</param>
        /// <param name="inputText">The specified input text.</param>
        /// <param name="requiredMessageText">The specified required message text.</param>
        /// <param name="addValidationSpan">
        ///     If <c>true</c> add a span for validation; otherwise <c>false</c>.
        /// </param>
        /// <param name="parameters">The specified parameters, if any.</param>
        /// <returns>
        ///     An instance of <see cref="ICaptcha" />.
        /// </returns>
        public static ICaptcha MathCaptcha(this HtmlHelper htmlHelper, string refreshText, string inputText,
                                           string requiredMessageText, bool addValidationSpan = false,
                                           params ParameterModel[] parameters)
        {
            List<ParameterModel> list = CaptchaUtils.GetParameters(parameters);
            list.Add(new ParameterModel(DefaultCaptchaManager.InputTextAttribute, inputText));
            list.Add(new ParameterModel(DefaultCaptchaManager.RefreshTextAttribute, refreshText));
            list.Add(new ParameterModel(DefaultCaptchaManager.MathCaptchaAttribute, true));
            list.Add(new ParameterModel(DefaultCaptchaManager.IsRequiredAttribute, true));
            list.Add(new ParameterModel(DefaultCaptchaManager.RequiredMessageAttribute, requiredMessageText));
            list.Add(new ParameterModel(DefaultCaptchaManager.IsNeedValidationSpanAttribute, addValidationSpan));
            return CaptchaUtils.GenerateCaptcha(htmlHelper, list);
        }

        /// <summary>
        ///     Creates a new math captcha with the specified partial view.
        /// </summary>
        /// <param name="htmlHelper">
        ///     The specified <see cref="HtmlHelper" />.
        /// </param>
        /// <param name="partialViewName">The name of the partial view to render.</param>
        /// <param name="viewData">The view data dictionary for the partial view.</param>
        /// <param name="parameters">The specified parameters, if any.</param>
        /// <returns>
        ///     An instance of <see cref="ICaptcha" />.
        /// </returns>
        public static ICaptcha MathCaptcha(this HtmlHelper htmlHelper, [AspMvcPartialView] string partialViewName,
                                           ViewDataDictionary viewData = null, params ParameterModel[] parameters)
        {
            Validate.ArgumentNotNullOrEmpty(partialViewName, "partialViewName");
            List<ParameterModel> list = CaptchaUtils.GetParameters(parameters);
            list.Add(new ParameterModel(DefaultCaptchaManager.MathCaptchaAttribute, true));
            list.Add(new ParameterModel(DefaultCaptchaManager.PartialViewNameAttribute, partialViewName));
            if (viewData != null)
                list.Add(new ParameterModel(DefaultCaptchaManager.PartialViewDataAttribute, viewData));
            return CaptchaUtils.GenerateCaptcha(htmlHelper, list);
        }

        /// <summary>
        ///     Creates a new math captcha with the specified partial view.
        /// </summary>
        /// <param name="htmlHelper">
        ///     The specified <see cref="HtmlHelper" />.
        /// </param>
        /// <param name="partialViewName">The name of the partial view to render.</param>
        /// <param name="scriptPartialViewName">The name of the partial view to render script.</param>
        /// <param name="viewData">The view data dictionary for the partial view.</param>
        /// <param name="parameters">The specified parameters, if any.</param>
        /// <returns>
        ///     An instance of <see cref="ICaptcha" />.
        /// </returns>
        public static ICaptcha MathCaptcha(this HtmlHelper htmlHelper, [AspMvcPartialView] string partialViewName,
                                           [AspMvcPartialView] string scriptPartialViewName,
                                           ViewDataDictionary viewData = null,
                                           params ParameterModel[] parameters)
        {
            Validate.ArgumentNotNullOrEmpty(partialViewName, "partialViewName");
            Validate.ArgumentNotNullOrEmpty(scriptPartialViewName, "scriptPartialViewName");
            List<ParameterModel> list = CaptchaUtils.GetParameters(parameters);
            list.Add(new ParameterModel(DefaultCaptchaManager.MathCaptchaAttribute, true));
            list.Add(new ParameterModel(DefaultCaptchaManager.PartialViewNameAttribute, partialViewName));
            list.Add(new ParameterModel(DefaultCaptchaManager.ScriptPartialViewNameAttribute, scriptPartialViewName));
            if (viewData != null)
                list.Add(new ParameterModel(DefaultCaptchaManager.PartialViewDataAttribute, viewData));
            return CaptchaUtils.GenerateCaptcha(htmlHelper, list);
        }

        /// <summary>
        ///     Determines whether the captcha is valid, and write error message if need.
        /// </summary>
        /// <param name="controllerBase">
        ///     The specified <see cref="ControllerBase" />.
        /// </param>
        /// <param name="errorText">The specified error message.</param>
        /// <param name="parameters">The specified parameters, if any.</param>
        /// <returns>
        ///     <c>True</c> if the captcha is valid; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsCaptchaValid(this ControllerBase controllerBase, string errorText,
                                          params ParameterModel[] parameters)
        {
            List<ParameterModel> list = CaptchaUtils.GetParameters(parameters);
            list.Add(new ParameterModel(DefaultCaptchaManager.ErrorAttribute, errorText));
            return CaptchaUtils.ValidateCaptcha(controllerBase, list);
        }

        /// <summary>
        /// Gets the <see cref="ICaptchaManager"/> using the specified <see cref="ControllerBase"/>.
        /// </summary>
        /// <param name="controllerBase">The specified <see cref="ControllerBase"/>.</param>
        /// <returns>An instance of <see cref="ICaptchaManager"/>.</returns>
        public static ICaptchaManager GetCaptchaManager(this ControllerBase controllerBase)
        {
            return CaptchaUtils.CaptchaManagerFactory(
                new RequestParameterContainer(controllerBase.ControllerContext.HttpContext.Request));
        }

        /// <summary>
        /// Gets the <see cref="ICaptchaBuilderProvider"/> using the specified <see cref="ControllerBase"/>.
        /// </summary>
        /// <param name="controllerBase">The specified <see cref="ControllerBase"/>.</param>
        /// <returns>An instance of <see cref="ICaptchaBuilderProvider"/>.</returns>
        public static ICaptchaBuilderProvider GetCaptchaBuilderProvider(this ControllerBase controllerBase)
        {
            return CaptchaUtils.BuilderProviderFactory(
                new RequestParameterContainer(controllerBase.ControllerContext.HttpContext.Request));
        }

        /// <summary>
        ///     Creates a new captcha values with the specified arguments.
        /// </summary>
        /// <param name="controller">
        ///     The specified <see cref="ControllerBase" />.
        /// </param>
        /// <param name="length">The specified length of characters.</param>
        /// <param name="parameters">The specified parameters, if any.</param>
        /// <returns>
        ///     An instance of <see cref="IUpdateInfoModel" />.
        /// </returns>
        public static IUpdateInfoModel GenerateCaptchaValue(this ControllerBase controller, int length, params ParameterModel[] parameters)
        {
            List<ParameterModel> list = CaptchaUtils.GetParameters(parameters);
            list.Add(new ParameterModel(DefaultCaptchaManager.LengthAttribute, length));
            var container = new CombinedParameterContainer(new ParameterModelContainer(list), new RequestParameterContainer(controller.ControllerContext.HttpContext.Request));
            return controller.GetCaptchaManager().GenerateNew(controller, container);
        }

        /// <summary>
        ///     Creates a new math captcha values with the specified arguments.
        /// </summary>
        /// <param name="controller">
        ///     The specified <see cref="ControllerBase" />.
        /// </param>
        /// <param name="parameters">The specified parameters, if any.</param>
        /// <returns>
        ///     An instance of <see cref="IUpdateInfoModel" />.
        /// </returns>
        public static IUpdateInfoModel GenerateMathCaptchaValue(this ControllerBase controller, params ParameterModel[] parameters)
        {
            List<ParameterModel> list = CaptchaUtils.GetParameters(parameters);
            list.Add(new ParameterModel(DefaultCaptchaManager.MathCaptchaAttribute, true));
            var container = new CombinedParameterContainer(new ParameterModelContainer(list), new RequestParameterContainer(controller.ControllerContext.HttpContext.Request));
            return controller.GetCaptchaManager().GenerateNew(controller, container);
        }

        /// <summary>
        ///     Makes the captcha "intelligent".
        /// </summary>
        /// <param name="captcha">
        ///     The specified <see cref="ICaptcha" />.
        /// </param>
        /// <param name="parameters">The specified parameters, if any.</param>
        /// <returns>
        ///     An instance of <see cref="ICaptcha" />.
        /// </returns>
        public static ICaptcha AsIntelligent(this ICaptcha captcha, params ParameterModel[] parameters)
        {
            List<ParameterModel> list = CaptchaUtils.GetParameters(parameters);
            var container = new CombinedParameterContainer(new ParameterModelContainer(list), captcha.BuildInfo.ParameterContainer);
            var captchaManager = CaptchaUtils.CaptchaManagerFactory(container);
            if (captchaManager.IntelligencePolicy == null)
                throw new NullReferenceException("The IntelligencePolicy property is null.");
            return captchaManager.IntelligencePolicy.MakeIntelligent(captchaManager, captcha, container);
        }

        #endregion
    }
}