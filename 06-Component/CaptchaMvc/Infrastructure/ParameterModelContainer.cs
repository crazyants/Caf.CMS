using System.Collections.Generic;
using CaptchaMvc.Interface;
using CaptchaMvc.Models;

namespace CaptchaMvc.Infrastructure
{
    /// <summary>
    /// Adapter for use the collection of <see cref="ParameterModel"/> as a <see cref="IParameterContainer"/>.
    /// </summary>
    public class ParameterModelContainer : IParameterContainer
    {
        #region Fields

        private readonly IList<ParameterModel> _parameters;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterModelContainer"/> class.
        /// </summary>
        public ParameterModelContainer(IList<ParameterModel> parameters)
        {
            _parameters = parameters;
        }

        #endregion

        #region Implementation of IParameterContainer

        /// <summary>
        /// The data source.
        /// </summary>
        public object ParameterProvider
        {
            get { return _parameters; }
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
            return _parameters.IsContains(key);
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="key">The specified key.</param>
        /// <returns>An instance of <c>T</c>.</returns>
        public T Get<T>(string key)
        {
            return _parameters.FindParameter<T>(key);
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
            return _parameters.TryFindParameter(key, out value);
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
            return _parameters.TryFindParameter(key, out value, defaultValue);
        }

        #endregion
    }
}