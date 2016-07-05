using System.Web.Mvc;

namespace CaptchaMvc.Interface
{
    /// <summary>
    ///     Represents the policy which makes a captcha as intelligent.
    /// </summary>
    public interface IIntelligencePolicy
    {
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
        bool? IsValid(ICaptchaManager captchaManager, ControllerBase controller, IParameterContainer parameterContainer);

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
        ICaptcha MakeIntelligent(ICaptchaManager captchaManager, ICaptcha captcha, IParameterContainer parameterContainer);
    }
}