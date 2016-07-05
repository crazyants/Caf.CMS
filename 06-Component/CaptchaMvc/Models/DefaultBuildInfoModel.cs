using System.Web.Mvc;
using CaptchaMvc.Interface;

namespace CaptchaMvc.Models
{
    /// <summary>
    /// Represents the default model with information for create a captcha.
    /// </summary>
    public class DefaultBuildInfoModel : BaseBuildInfoModel
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultBuildInfoModel"/> class.
        /// </summary>
        public DefaultBuildInfoModel(IParameterContainer parameterContainer, string tokenParameterName, string requiredMessage, bool isRequired,
                                     string refreshButtonText, string inputText, HtmlHelper htmlHelper,
                                     string inputElementId, string imageElementId, string tokenElementId,
                                     string refreshUrl, string imageUrl, string tokenValue)
            : base(parameterContainer,
                tokenParameterName, requiredMessage, isRequired, refreshButtonText, inputText, htmlHelper,
                inputElementId, imageElementId, tokenElementId, refreshUrl, imageUrl, tokenValue)
        {
        }

        #endregion
    }
}