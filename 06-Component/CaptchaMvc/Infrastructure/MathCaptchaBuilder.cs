using System;
using System.Web.Mvc;
using CaptchaMvc.Interface;
using CaptchaMvc.Models;

namespace CaptchaMvc.Infrastructure
{
    /// <summary>
    ///     Implementation of a <see cref="ICaptchaBulder" /> for build a math captcha.
    /// </summary>
    public class MathCaptchaBuilder : DefaultCaptchaBuilder
    {
        #region Fields

        private const string UpdateScript = @"
<script type=""text/javascript"">
$(function () {{$('#{0}').show();}})
function {5} {{ $('#{0}').hide(); $.post(""{1}"", {{ {2}: $('#{3}').val(), {4}: ""0"" }}, function(){{$('#{0}').show();}}); return false; }}</script>";

        #endregion

        #region Overrides of DefaultCaptchaBuilder

        /// <summary>
        ///     Creates a html string to represent the refresh button element.
        /// </summary>
        /// <param name="buildInfoModel">
        ///     The specified <see cref="IBuildInfoModel" />.
        /// </param>
        /// <returns>The html string with the refresh button element.</returns>
        protected override RefreshButton GenerateRefreshButton(IBuildInfoModel buildInfoModel)
        {
            var infoModel = buildInfoModel as MathBuildInfoModel;
            if (infoModel == null)
                throw new ArgumentException("A MathCaptchaBuilder can only work with the MathBuildInfoModel.");

            string id = Guid.NewGuid().ToString("N");
            string functionName = string.Format("______{0}________()", Guid.NewGuid().ToString("N"));
            var tagA = new TagBuilder("a");
            tagA.Attributes.Add("onclick", functionName);
            tagA.Attributes.Add("href", "#" + buildInfoModel.ImageElementId);
            tagA.Attributes.Add("style", "display:none;");
            tagA.SetInnerText(buildInfoModel.RefreshButtonText);
            tagA.Attributes.Add("id", id);
            string updateScript = string.Format(UpdateScript, id, infoModel.RefreshUrl, infoModel.TokenParameterName,
                                                infoModel.TokenElementId, infoModel.MathParamterName, functionName);
            return new RefreshButton(tagA.ToString(), updateScript);
        }

        #endregion
    }
}