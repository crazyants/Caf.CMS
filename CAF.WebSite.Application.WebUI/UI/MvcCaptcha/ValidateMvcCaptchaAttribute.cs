using CAF.WebSite.Application.WebUI.Captcha;
using CAF.Infrastructure.Core;
using System;
using System.Web.Mvc;

namespace CAF.WebSite.Application.WebUI.MvcCaptcha
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class ValidateMvcCaptchaAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the CaptchaValidationAttribute class.
        /// </summary>
        public ValidateMvcCaptchaAttribute()
            : this("_mvcCaptchaText") { }

        /// <summary>
        /// Initializes a new instance of the CaptchaValidationAttribute class.
        /// </summary>
        /// <param name="field">The field.</param>
        public ValidateMvcCaptchaAttribute(string field)
        {
            Field = field;
        }

        /// <summary>
        /// Gets or sets the field.
        /// </summary>
        /// <value>The field.</value>
        public string Field { get; private set; }


        /// <summary> 
        /// Called when [action executed]. 
        /// </summary> 
        /// <param name="filterContext">The filter filterContext.</param> 
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            bool isValid = false;
            // get the guid from the post back 
            string guid = filterContext.HttpContext.Request.Form["_MvcCaptchaGuid"];
            var captchaSettings = EngineContext.Current.Resolve<CaptchaSettings>();
            if (captchaSettings.Enabled)
            {
                // get values 
                var image = MvcCaptchaImage.GetCachedCaptcha(guid);
                string actualValue = filterContext.HttpContext.Request.Form[Field];
                string expectedValue = image == null ? String.Empty : image.Text;

                // removes the captch from Session so it cannot be used again 
                filterContext.HttpContext.Session.Remove(guid);

                isValid = !String.IsNullOrEmpty(actualValue)
                              && !String.IsNullOrEmpty(expectedValue)
                              && String.Equals(actualValue, expectedValue, StringComparison.OrdinalIgnoreCase);
                if (!isValid)
                    ((Controller)filterContext.Controller).ModelState.AddModelError(Field, "验证码不匹配");

            }
            filterContext.ActionParameters["captchaValid"] = isValid;

            base.OnActionExecuting(filterContext);//(string)filterContext.HttpContext.GetGlobalResourceObject("LangPack","ValidationCode_Not_Match"));
        }

    }
}
