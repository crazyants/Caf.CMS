using System;
using System.Web.Mvc;
using CaptchaMvc.Infrastructure;
using CaptchaMvc.Interface;

namespace CaptchaMvc.Controllers
{
    /// <summary>
    ///     Represents the controller that is responsible for creating and updating captcha.
    /// </summary>
#if !MVC3
    [AllowAnonymous]
#endif
    public class DefaultCaptchaController : Controller
    {
        #region Action methods

        /// <summary>
        ///     Generates a new captcha image.
        /// </summary>
        public virtual void Generate()
        {
            var parameterContainer = new RequestParameterContainer(Request);
            try
            {
                if (Request.UrlReferrer.AbsolutePath == Request.Url.AbsolutePath)
                    throw new InvalidOperationException();

                IDrawingModel drawingModel =
                    CaptchaUtils.CaptchaManagerFactory(parameterContainer).GetDrawingModel(parameterContainer);
                CaptchaUtils.BuilderProviderFactory(parameterContainer).WriteCaptchaImage(Response, drawingModel);
            }
            catch (Exception)
            {
                CaptchaUtils.BuilderProviderFactory(parameterContainer).WriteErrorImage(Response);
            }
        }

        /// <summary>
        ///     Refreshes a captcha.
        /// </summary>
        /// <returns>
        ///     An instance of <see cref="ActionResult" />.
        /// </returns>
        public virtual ActionResult Refresh()
        {
            var parameterContainer = new RequestParameterContainer(Request);
            if (Request.IsAjaxRequest())
            {
                IUpdateInfoModel updateInfoModel =
                    CaptchaUtils.CaptchaManagerFactory(parameterContainer).Update(parameterContainer);
                return CaptchaUtils.BuilderProviderFactory(parameterContainer).RefreshCaptcha(updateInfoModel);
            }
            return Redirect(Request.UrlReferrer.AbsolutePath);
        }

        #endregion
    }
}