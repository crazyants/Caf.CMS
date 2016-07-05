using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using CaptchaMvc.Interface;

namespace CaptchaMvc.Infrastructure
{
    /// <summary>
    ///     Represents the storage to save a captcha tokens in cookie.
    /// </summary>
    public class CookieStorageProvider : IStorageProvider
    {
        #region Fields

        private const int MaxCookieLength = 2000;
        private const string CookieKey = "wrawrsatrsrweasrdxsf";
        private const string Separator = "|~|~~|~|";
        private const string DrawingKey = "w2ewasjret";
        private string _cookieName;
        private string _password;
        private byte[] _salt;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CookieStorageProvider" /> class.
        /// </summary>
        public CookieStorageProvider()
            : this(20)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CookieStorageProvider" /> class.
        /// </summary>
        /// <param name="expiresMinutes">The specified expires of the cookie in minutes.</param>
        public CookieStorageProvider(int expiresMinutes)
            : this(expiresMinutes, CookieKey)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CookieStorageProvider" /> class.
        /// </summary>
        /// <param name="cookieName">The specified cookie name.</param>
        public CookieStorageProvider(string cookieName)
            : this(20, cookieName)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CookieStorageProvider" /> class.
        /// </summary>
        /// <param name="expiresMinutes">The specified expires of the cookie in minutes.</param>
        /// <param name="cookieName">The specified cookie name.</param>
        public CookieStorageProvider(int expiresMinutes, string cookieName)
            : this(expiresMinutes, cookieName, typeof (CookieStorageProvider).FullName,
                   Encoding.UTF8.GetBytes(typeof (CookieStorageProvider).FullName))
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CookieStorageProvider" /> class.
        /// </summary>
        /// <param name="expiresMinutes">The specified expires of the cookie in minutes.</param>
        /// <param name="cookieName">The specified cookie name.</param>
        /// <param name="password">The specified password to encrypt cookie</param>
        /// <param name="salt">The specified salt to encrypt cookie</param>
        public CookieStorageProvider(int expiresMinutes, string cookieName, string password, byte[] salt)
        {
            Validate.ArgumentNotNullOrEmpty(cookieName, "cookieName");
            Validate.ArgumentNotNullOrEmpty(password, "password");
            Validate.ArgumentNotNull(salt, "salt");
            ExpiresMinutes = expiresMinutes;
            _salt = salt;
            _password = password;
            CookieName = cookieName;
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
            Validate.ArgumentNotNull(captchaPair, "captchaPair");
            Validate.ArgumentNotNull(captchaPair.Value, "captchaPair");
            string drawingKey = CookieName + DrawingKey;
            HttpCookie httpCookieDrawing = HttpContext.Current.Request.Cookies[drawingKey] ??
                                           new HttpCookie(drawingKey);
            HttpCookie httpCookie = HttpContext.Current.Request.Cookies[CookieName] ??
                                    new HttpCookie(CookieName);

            httpCookie.Expires = httpCookieDrawing.Expires = DateTime.Now.AddMinutes(ExpiresMinutes);
            httpCookie.HttpOnly = httpCookieDrawing.HttpOnly = true;

            string serialize = Serialize(captchaPair.Value);

            ClearCookieIfNeed(httpCookie);
            ClearCookieIfNeed(httpCookieDrawing);

            httpCookie.Values.Add(captchaPair.Key, serialize);
            httpCookieDrawing.Values.Add(captchaPair.Key, serialize);

            HttpContext.Current.Response.Cookies.Add(httpCookie);
            HttpContext.Current.Response.Cookies.Add(httpCookieDrawing);
        }

        /// <summary>
        ///     Removes the specified token and <see cref="ICaptchaValue" /> to the storage.
        /// </summary>
        /// <param name="token">The specified token.</param>
        public bool Remove(string token)
        {
            Validate.ArgumentNotNullOrEmpty(token, "token");
            bool removeDr = RemoveFromCookie(CookieName + DrawingKey, token);
            bool removeVal = RemoveFromCookie(CookieName, token);
            return removeDr || removeVal;
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
            switch (tokenType)
            {
                case TokenType.Drawing:
                    return GetFromCookie(CookieName + DrawingKey, token);
                case TokenType.Validation:
                    return GetFromCookie(CookieName, token);
                default:
                    throw new ArgumentOutOfRangeException("tokenType");
            }
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
                    return IsContains(CookieName + DrawingKey, token);
                case TokenType.Validation:
                    return IsContains(CookieName, token);
                default:
                    throw new ArgumentOutOfRangeException("tokenType");
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the cookie name.
        /// </summary>
        public string CookieName
        {
            get { return _cookieName; }
            set
            {
                Validate.PropertyNotNullOrEmpty(value, "CookieName");
                _cookieName = value;
            }
        }

        /// <summary>
        ///     Expires of the cookie in minutes.
        /// </summary>
        public int ExpiresMinutes { get; set; }

        /// <summary>
        ///     Gets or sets the password to encrypt cookie.
        /// </summary>
        public string Password
        {
            get { return _password; }
            set
            {
                Validate.PropertyNotNullOrEmpty(value, "Password");
                _password = value;
            }
        }

        /// <summary>
        ///     Gets or sets the salt to encrypt cookie.
        /// </summary>
        public byte[] Salt
        {
            get { return _salt; }
            set
            {
                Validate.PropertyNotNull(value, "Salt");
                _salt = value;
            }
        }

        #endregion

        #region Methods

        private static void ClearCookieIfNeed(HttpCookie httpCookie)
        {
            //Remove the two value from a cookie.
            if (httpCookie.Value == null) return;
            if (httpCookie.Value.Length < MaxCookieLength) return;
            httpCookie.Values.Remove(httpCookie.Values.GetKey(0));
            if (httpCookie.Values.Count == 0) return;
            httpCookie.Values.Remove(httpCookie.Values.GetKey(0));
        }

        private ICaptchaValue GetFromCookie(string cookieName, string token)
        {
            HttpCookie httpCookie = HttpContext.Current.Request.Cookies[cookieName];
            if (httpCookie == null)
                return null;
            string value = httpCookie.Values[token];
            if (string.IsNullOrEmpty(value))
                return null;
            httpCookie.Values.Remove(token);
            httpCookie.Expires = DateTime.Now.AddMinutes(ExpiresMinutes);
            httpCookie.HttpOnly = true;
            HttpContext.Current.Response.Cookies.Add(httpCookie);
            return Deserialize(value);
        }

        private static bool IsContains(string cookieName, string token)
        {
            HttpCookie httpCookie = HttpContext.Current.Request.Cookies[cookieName];
            if (httpCookie == null)
                return false;
            string value = httpCookie.Values[token];
            if (string.IsNullOrEmpty(value))
                return false;
            return true;
        }

        private bool RemoveFromCookie(string cookieName, string token)
        {
            HttpCookie httpCookie = HttpContext.Current.Request.Cookies[cookieName];
            if (httpCookie == null)
                return false;
            string value = httpCookie.Values[token];
            if (string.IsNullOrEmpty(value))
                return false;
            httpCookie.Values.Remove(token);
            httpCookie.Expires = DateTime.Now.AddMinutes(ExpiresMinutes);
            httpCookie.HttpOnly = true;
            HttpContext.Current.Response.Cookies.Add(httpCookie);
            return true;
        }

        /// <summary>
        ///     Serializes the <see cref="ICaptchaValue" />, to the given string.
        /// </summary>
        /// <param name="captchaValue">
        ///     The specified <see cref="ICaptchaValue" />.
        /// </param>
        /// <returns>The result string.</returns>
        protected virtual string Serialize(ICaptchaValue captchaValue)
        {
            Validate.ArgumentNotNull(captchaValue, "captchaValue");
            using (var pdb = new Rfc2898DeriveBytes(Password, Salt))
            {
                string value = captchaValue.GetType().FullName + Separator + captchaValue.Serialize();
                byte[] bytes = Encoding.Unicode.GetBytes(value);
                using (var ms = new MemoryStream())
                {
                    Rijndael alg = Rijndael.Create();

                    alg.Key = pdb.GetBytes(32);
                    alg.IV = pdb.GetBytes(16);

                    using (var cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytes, 0, bytes.Length);
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        /// <summary>
        ///     Deserializes the specified <see cref="string" /> into an <see cref="ICaptchaValue" />.
        /// </summary>
        /// <param name="value">The specified serialize state.</param>
        /// <returns>
        ///     The result <see cref="ICaptchaValue" />.
        /// </returns>
        protected virtual ICaptchaValue Deserialize(string value)
        {
            Validate.ArgumentNotNullOrEmpty(value, "value");
            byte[] inputBytes = Convert.FromBase64String(value);
            using (var pdb = new Rfc2898DeriveBytes(Password, Salt))
            {
                using (var ms = new MemoryStream())
                {
                    Rijndael alg = Rijndael.Create();

                    alg.Key = pdb.GetBytes(32);
                    alg.IV = pdb.GetBytes(16);

                    using (var cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(inputBytes, 0, inputBytes.Length);
                    }
                    value = Encoding.Unicode.GetString(ms.ToArray());
                    string[] strings = value.Split(new[] {Separator}, StringSplitOptions.RemoveEmptyEntries);
                    if (strings.Length != 2)
                        throw new ArgumentException(
                            string.Format("It is not possible to deserialize an object from a string '{0}'.", value));
                    Type type = Type.GetType(strings[0]);
                    if (type == null)
                        throw new ArgumentException(
                            string.Format(
                                "It is not possible to deserialize an object from a string '{0}', the type {1} not found.",
                                value, strings[0]));
                    ConstructorInfo constructorInfo =
                        type.GetConstructor(
                            BindingFlags.CreateInstance | BindingFlags.NonPublic | BindingFlags.Public |
                            BindingFlags.Instance, null, Type.EmptyTypes, null);
                    if (constructorInfo == null)
                        throw new ArgumentException(
                            string.Format(
                                "It is not possible to deserialize an object from a string '{0}', type {1}, the constructor with empty types not found.",
                                value, type));
                    var result = (ICaptchaValue) constructorInfo.Invoke(new object[0]);
                    result.Deserialize(strings[1]);
                    return result;
                }
            }
        }

        #endregion
    }
}