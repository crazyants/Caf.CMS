namespace CaptchaMvc.Interface
{
    /// <summary>
    ///     Represents the base model to create a new captcha.
    /// </summary>
    public interface ICaptchaBulder
    {
        /// <summary>
        ///     Creates a new captcha using the specified <see cref="IBuildInfoModel" />.
        /// </summary>
        /// <param name="buildInfoModel">
        ///     The specified <see cref="IBuildInfoModel" />.
        /// </param>
        /// <returns>An instance of <see cref="ICaptcha"/>.</returns>
        ICaptcha Build(IBuildInfoModel buildInfoModel);
    }
}