using System;
using System.Web.Mvc;
using CaptchaMvc.Infrastructure;
using CaptchaMvc.Interface;

namespace CaptchaMvc.Models
{
    /// <summary>
    /// Represents the model with information for create a math captcha.
    /// </summary>
    public class MathBuildInfoModel : BaseBuildInfoModel
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MathBuildInfoModel"/> class.
        /// </summary>
        public MathBuildInfoModel(IParameterContainer parameterContainer, string tokenParameterName, string mathParamterName, bool isRequired,
                                  string requiredMessage, string refreshButtonText, string inputText,
                                  HtmlHelper htmlHelper, string inputElementId, string tokenElementId,
                                  string imageElementId, string imageUrl, string refreshUrl, string tokenValue)
            : base(parameterContainer,
                tokenParameterName, requiredMessage, isRequired, refreshButtonText, inputText, htmlHelper,
                inputElementId, imageElementId, tokenElementId, refreshUrl, imageUrl, tokenValue)
        {
            Validate.ArgumentNotNullOrEmpty(mathParamterName, "mathParamterName");
            MathParamterName = mathParamterName;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the math parameter name.
        /// </summary>
        public string MathParamterName { get; private set; }

        #endregion
    }
}