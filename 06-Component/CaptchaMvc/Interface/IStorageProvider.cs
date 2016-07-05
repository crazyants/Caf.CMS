using System.Collections.Generic;

namespace CaptchaMvc.Interface
{
    /// <summary>
    ///     Represents the token type.
    /// </summary>
    public enum TokenType
    {
        /// <summary>
        ///     The drawing token.
        /// </summary>
        Drawing = 1,

        /// <summary>
        ///     The validation token.
        /// </summary>
        Validation = 2
    }

    /// <summary>
    ///     Represents the storage to save a captcha tokens.
    /// </summary>
    public interface IStorageProvider
    {
        /// <summary>
        ///     Determines whether the <see cref="IStorageProvider" /> contains a specific token.
        /// </summary>
        /// <param name="token">The specified token.</param>
        /// <param name="tokenType">The specified token type.</param>
        /// <returns>
        ///     <c>True</c> if the value is found in the <see cref="IStorageProvider" />; otherwise <c>false</c>.
        /// </returns>
        bool IsContains(string token, TokenType tokenType);

        /// <summary>
        ///     Adds the specified token and <see cref="ICaptchaValue" /> to the storage.
        /// </summary>
        /// <param name="captchaPair">
        ///     The specified <see cref="KeyValuePair{TKey,TValue}" />
        /// </param>
        void Add(KeyValuePair<string, ICaptchaValue> captchaPair);

        /// <summary>
        ///     Removes the specified token and <see cref="ICaptchaValue" /> to the storage.
        /// </summary>
        /// <param name="token">The specified token.</param>
        bool Remove(string token);

        /// <summary>
        ///     Gets an <see cref="ICaptchaValue" /> associated with the specified token.
        /// </summary>
        /// <param name="token">The specified token.</param>
        /// <param name="tokenType">The specified token type.</param>
        /// <returns>
        ///     An instance of <see cref="ICaptchaValue" />.
        /// </returns>
        ICaptchaValue GetValue(string token, TokenType tokenType);
    }
}