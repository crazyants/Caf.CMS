namespace CaptchaMvc.Interface
{
    /// <summary>
    /// Represents the base model with information for update a captcha.
    /// </summary>
    public interface IUpdateInfoModel
    {
        /// <summary>
        /// Gets the token element id in DOM.
        /// </summary>
        string TokenElementId { get; }

        /// <summary>
        /// Gets the image element id in DOM.
        /// </summary>
        string ImageElementId { get; }

        /// <summary>
        /// Gets the url with captcha image.
        /// </summary>
        string ImageUrl { get; }

        /// <summary>
        /// Gets the token value.
        /// </summary>
        string TokenValue { get; }
    }
}