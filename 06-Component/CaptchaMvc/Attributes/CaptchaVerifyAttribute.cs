using System;
using System.Reflection;
using System.Web.Mvc;
using CaptchaMvc.Infrastructure;
using CaptchaMvc.Models;

namespace CaptchaMvc.Attributes
{
    /// <summary>
    ///     Represents the attribute to validate the captcha.
    /// </summary>
    public class CaptchaVerifyAttribute : ActionFilterAttribute
    {
        #region Constructor

        /// <summary>
        ///     Initializes a new instance of the <see cref="CaptchaVerifyAttribute" /> class.
        /// </summary>
        public CaptchaVerifyAttribute(string errorMessage)
        {
            Validate.ArgumentNotNullOrEmpty(errorMessage, "errorMessage");
            ErrorMessage = errorMessage;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CaptchaVerifyAttribute" /> class.
        /// </summary>
        public CaptchaVerifyAttribute(string resourceName, Type resourceType)
        {
            Validate.ArgumentNotNull(resourceName, "resourceName");
            Validate.ArgumentNotNull(resourceType, "resourceType");
            ResourceAccessor = FindResourceAccessor(resourceName, resourceType);
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets an error message to associate with a validation control if validation fails.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     Gets or sets a <see cref="MethodInfo" /> for access to the resource message.
        /// </summary>
        protected MethodInfo ResourceAccessor { get; set; }

        #endregion

        #region Overrides of ActionFilterAttribute

        /// <summary>
        ///     Called by the ASP.NET MVC framework before the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            CaptchaUtils.ValidateCaptcha(filterContext.Controller, new[]
                                                                       {
                                                                           new ParameterModel(
                                                                               DefaultCaptchaManager.ErrorAttribute,
                                                                               GetErrorMessage())
                                                                       });
        }

        #endregion

        #region Methods

        private static MethodInfo FindResourceAccessor(string resourceName, Type resourceType)
        {
            PropertyInfo propertyInfo = resourceType.GetProperty(resourceName,
                                                                 BindingFlags.Static | BindingFlags.Public |
                                                                 BindingFlags.NonPublic);
            if (propertyInfo == null)
                throw new InvalidOperationException(
                    string.Format("Resource with the name {0} is not found in the type of {1}.", resourceName,
                                  resourceType));
            if (propertyInfo.PropertyType != typeof(string))
                throw new InvalidOperationException(
                    string.Format("Resource with the name {0} in the type of {1}, is not string.", resourceName,
                                  resourceType));
            MethodInfo methodInfo = propertyInfo.GetGetMethod(true);
            if (methodInfo == null)
                throw new InvalidOperationException(
                    string.Format("Resource with the name {0} in the type of {1}, is not have a get method.",
                                  resourceName, resourceType));
            return methodInfo;
        }

        /// <summary>
        ///     Returns a error message.
        /// </summary>
        /// <returns>The error message.</returns>
        protected virtual string GetErrorMessage()
        {
            if (ResourceAccessor != null)
                return (string)ResourceAccessor.Invoke(null, null);

            return ErrorMessage;
        }

        #endregion
    }
}