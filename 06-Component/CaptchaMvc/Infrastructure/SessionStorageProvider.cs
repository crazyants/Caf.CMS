using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using CaptchaMvc.Interface;
using CaptchaMvc.Models;

namespace CaptchaMvc.Infrastructure
{
    /// <summary>
    ///     Represents the storage to save a captcha tokens in session.
    /// </summary>
    public class SessionStorageProvider : IStorageProvider
    {
        #region Fields

        private const string SessionValidateKey = "____________SessionValidateKey_____________";
        private const string SessionDrawingKey = "____________SessionDrawingKey_____________";

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SessionStorageProvider" /> class.
        /// </summary>
        public SessionStorageProvider()
            : this(15)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SessionStorageProvider" /> class.
        /// </summary>
        /// <param name="maxCount">Gets or sets the maximum values.</param>
        public SessionStorageProvider(uint maxCount)
        {
            MaxCount = maxCount;
        }

        #endregion

        #region IStorageProvider Members

        /// <summary>
        ///     Adds the specified token and <see cref="ICaptchaValue" /> to the storage.
        /// </summary>
        /// <param name="captchaPair">
        ///     The specified <see cref="KeyValuePair{TKey,TValue}" />
        /// </param>
        public virtual void Add(KeyValuePair<string, ICaptchaValue> captchaPair)
        {
            Validate.ArgumentNotNull(captchaPair.Value, "captchaPair");
            DrawingKeys.ClearIfNeed(MaxCount);
            ValidateKeys.ClearIfNeed(MaxCount);
            var entry = new KeyTimeEntry<string>(captchaPair.Key);
            DrawingKeys.Add(entry, captchaPair.Value);
            ValidateKeys.Add(entry, captchaPair.Value);
        }

        /// <summary>
        ///     Removes the specified token and <see cref="ICaptchaValue" /> to the storage.
        /// </summary>
        /// <param name="token">The specified token.</param>
        public bool Remove(string token)
        {
            Validate.ArgumentNotNullOrEmpty(token, "token");
            var remove = DrawingKeys.Remove(token);
            var validation = ValidateKeys.Remove(token);
            return remove || validation;
        }

        /// <summary>
        ///     Gets an <see cref="ICaptchaValue" /> associated with the specified token.
        /// </summary>
        /// <param name="token">The specified token.</param>
        /// <param name="tokenType">The specified token type.</param>
        /// <returns>
        ///     An instance of <see cref="ICaptchaValue" />.
        /// </returns>
        public virtual ICaptchaValue GetValue(string token, TokenType tokenType)
        {
            Validate.ArgumentNotNullOrEmpty(token, "token");
            ICaptchaValue value;
            switch (tokenType)
            {
                case TokenType.Drawing:
                    if (DrawingKeys.TryGetValue(token, out value))
                        DrawingKeys.Remove(token);
                    break;
                case TokenType.Validation:
                    if (ValidateKeys.TryGetValue(token, out value))
                        ValidateKeys.Remove(token);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("tokenType");
            }
            return value;
        }

        /// <summary>
        ///     Determines whether the <see cref="IStorageProvider" /> contains a specific token.
        /// </summary>
        /// <param name="token">The specified token.</param>
        /// <param name="tokenType">The specified token type.</param>
        /// <returns>
        ///     <c>True</c> if the value is found in the <see cref="IStorageProvider" />; otherwise <c>false</c>.
        /// </returns>
        public virtual bool IsContains(string token, TokenType tokenType)
        {
            Validate.ArgumentNotNullOrEmpty(token, "token");
            switch (tokenType)
            {
                case TokenType.Drawing:
                    return DrawingKeys.ContainsKey(token);
                case TokenType.Validation:
                    return ValidateKeys.ContainsKey(token);
                default:
                    throw new ArgumentOutOfRangeException("tokenType");
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the maximum size of session values.
        /// </summary>
        public uint MaxCount { get; set; }

        /// <summary>
        ///     Contains tokens that have not yet been validated.
        /// </summary>
        protected IDictionary<KeyTimeEntry<string>, ICaptchaValue> ValidateKeys
        {
            get { return CaptchaUtils.GetFromSession(SessionValidateKey, () => new ConcurrentDictionary<KeyTimeEntry<string>, ICaptchaValue>()); }
        }

        /// <summary>
        ///     Contains tokens that have not yet been displayed.
        /// </summary>
        protected IDictionary<KeyTimeEntry<string>, ICaptchaValue> DrawingKeys
        {
            get { return CaptchaUtils.GetFromSession(SessionDrawingKey, () => new ConcurrentDictionary<KeyTimeEntry<string>, ICaptchaValue>()); }
        }

        #endregion
    }
}