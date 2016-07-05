using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using CaptchaMvc.Interface;
using CaptchaMvc.Models;

namespace CaptchaMvc.Infrastructure
{
    /// <summary>
    ///     Implementation of a <see cref="ICaptchaBulder" /> for build partial captcha.
    /// </summary>
    public class PartialCaptchaBuilder : ICaptchaBulder
    {
        #region Implementation of ICaptchaBulder

        /// <summary>
        ///     Creates a new captcha using the specified <see cref="IBuildInfoModel" />.
        /// </summary>
        /// <param name="buildInfoModel">
        ///     The specified <see cref="IBuildInfoModel" />.
        /// </param>
        /// <returns>An instance of <see cref="ICaptcha"/>.</returns>
        public ICaptcha Build(IBuildInfoModel buildInfoModel)
        {
            var infoModel = buildInfoModel as PartialBuildInfoModel;
            if (infoModel == null)
                throw new ArgumentException("A PartialCaptchaBuilder can only work with a PartialBuildInfoModel.");

            MvcHtmlString markup;
            MvcHtmlString script = null;
            if (infoModel.ViewData != null)
            {
                markup = infoModel.HtmlHelper.Partial(infoModel.PartialViewName, infoModel.BuildInfoModel,
                                                      infoModel.ViewData);
                if (!string.IsNullOrEmpty(infoModel.ScriptPartialViewName))
                    script = infoModel.HtmlHelper.Partial(infoModel.ScriptPartialViewName, infoModel.BuildInfoModel,
                                                          infoModel.ViewData);
            }
            else
            {
                markup = infoModel.HtmlHelper.Partial(infoModel.PartialViewName, infoModel.BuildInfoModel);
                if (!string.IsNullOrEmpty(infoModel.ScriptPartialViewName))
                    script = infoModel.HtmlHelper.Partial(infoModel.ScriptPartialViewName, infoModel.BuildInfoModel);
            }
            if (script == null)
                return new CaptchaModel(buildInfoModel, markup.ToHtmlString(), null);
            return new CaptchaModel(buildInfoModel, markup.ToHtmlString(), script.ToHtmlString());
        }

        #endregion
    }
}