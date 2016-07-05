using System.ComponentModel;
using System.Linq;
using System.Web;
using CaptchaMvc.Interface;

namespace CaptchaMvc.Infrastructure
{
    /// <summary>
    /// Adapter for use the <see cref="HttpRequestBase"/> as a <see cref="IParameterContainer"/>.
    /// </summary>
    public class RequestParameterContainer : IParameterContainer
    {
        #region Fields

        /// <summary>
        /// The <see cref="HttpRequestBase"/> parameter key.
        /// </summary>
        public const string HttpRequestParameterKey = "RequestParameterKey";

        private readonly HttpRequestBase _requestBase;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestParameterContainer"/> class.
        /// </summary>
        public RequestParameterContainer(HttpRequestBase requestBase)
        {
            Validate.ArgumentNotNull(requestBase, "requestBase");
            _requestBase = requestBase;
        }

        #endregion

        #region Implementation of IParameterContainer

        /// <summary>
        /// The data source.
        /// </summary>
        public object ParameterProvider
        {
            get { return _requestBase; }
        }

        /// <summary>
        ///     Determines whether the <see cref="IParameterContainer" /> contains a specific key.
        /// </summary>
        /// <param name="key">The specified key.</param>
        /// <returns>
        ///     <c>True</c> if the value is found in the <see cref="IParameterContainer" />; otherwise, <c>false</c>.
        /// </returns>
        public bool IsContains(string key)
        {
            if (key == HttpRequestParameterKey)
                return true;
            return _requestBase.Params.AllKeys.Any(s => s.Equals(key));
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="key">The specified key.</param>
        /// <returns>An instance of T.</returns>
        public T Get<T>(string key)
        {
            if (key == HttpRequestParameterKey)
                return (T)((object)_requestBase);
            return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(_requestBase.Params[key]);
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="key">The specified key.</param>
        /// <param name="value">An instance of T.</param>
        /// <returns><c>True</c> if the value is found in the <see cref="IParameterContainer"/>; otherwise, <c>false</c>.</returns>
        public bool TryGet<T>(string key, out T value)
        {
            return TryGet(key, out value, default(T));
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="key">The specified key.</param>
        /// <param name="value">An instance of T.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns><c>True</c> if the value is found in the <see cref="IParameterContainer"/>; otherwise, <c>false</c>.</returns>
        public bool TryGet<T>(string key, out T value, T defaultValue)
        {
            if (!IsContains(key))
            {
                value = defaultValue;
                return false;
            }
            value = Get<T>(key);
            return true;
        }

        #endregion
    }
}