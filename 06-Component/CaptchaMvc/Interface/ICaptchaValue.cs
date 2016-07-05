namespace CaptchaMvc.Interface
{
    /// <summary>
    /// Represents the base model for storing ​​captcha values.
    /// </summary>
    public interface ICaptchaValue
    {
        /// <summary>
        /// Gets the specified captcha text.
        /// </summary>
        string CaptchaText { get; }

        /// <summary>
        /// Gets the specified captcha value.
        /// </summary>
        object Value { get; }

        /// <summary>
        /// Serializes the <see cref="ICaptchaValue"/>, to the given string.
        /// </summary>
        /// <returns>The result string.</returns>
        string Serialize();

        /// <summary>
        /// Deserializes the specified <see cref="string"/> into an <see cref="ICaptchaValue"/>.
        /// </summary>
        /// <param name="serializeState">The specified serialize state.</param>
        void Deserialize(string serializeState);

        /// <summary>
        /// Determines whether the current captcha value is equal for the <c>inputText</c>.
        /// </summary>
        /// <param name="inputText">The specified input text.</param>
        /// <returns><c>True</c> if the value is equals; otherwise, <c>false</c>.</returns>
        bool IsEqual(string inputText);
    }
}