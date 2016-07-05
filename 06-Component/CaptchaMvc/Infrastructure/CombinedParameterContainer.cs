using CaptchaMvc.Interface;

namespace CaptchaMvc.Infrastructure
{
    /// <summary>
    ///     Represents the implementation of <see cref="IParameterContainer" /> that can combine two
    ///     <see
    ///         cref="IParameterContainer" />
    ///     in one.
    /// </summary>
    public class CombinedParameterContainer : IParameterContainer
    {
        #region Fields

        private readonly IParameterContainer _firstContainer;
        private readonly IParameterContainer _secondContainer;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CombinedParameterContainer" /> class.
        /// </summary>
        public CombinedParameterContainer(IParameterContainer firstContainer, IParameterContainer secondContainer)
        {
            Validate.ArgumentNotNull(firstContainer, "firstContainer");
            Validate.ArgumentNotNull(secondContainer, "secondContainer");
            _firstContainer = firstContainer;
            _secondContainer = secondContainer;
            ParameterProvider = new object[] {_firstContainer, _secondContainer};
        }

        #endregion

        #region Implementation of IParameterContainer

        /// <summary>
        ///     The data source.
        /// </summary>
        public object ParameterProvider { get; private set; }

        /// <summary>
        ///     Determines whether the <see cref="IParameterContainer" /> contains a specific key.
        /// </summary>
        /// <param name="key">The specified key.</param>
        /// <returns>
        ///     <c>True</c> if the value is found in the <see cref="IParameterContainer" />; otherwise, <c>false</c>.
        /// </returns>
        public bool IsContains(string key)
        {
            return _firstContainer.IsContains(key) || _secondContainer.IsContains(key);
        }

        /// <summary>
        ///     Gets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="key">The specified key.</param>
        /// <returns>An instance of T.</returns>
        public T Get<T>(string key)
        {
            if (_firstContainer.IsContains(key))
                return _firstContainer.Get<T>(key);
            if (_secondContainer.IsContains(key))
                return _secondContainer.Get<T>(key);
            return default(T);
        }

        /// <summary>
        ///     Gets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="key">The specified key.</param>
        /// <param name="value">An instance of T.</param>
        /// <returns>
        ///     <c>True</c> if the value is found in the <see cref="IParameterContainer" />; otherwise, <c>false</c>.
        /// </returns>
        public bool TryGet<T>(string key, out T value)
        {
            if (_firstContainer.IsContains(key))
                return _firstContainer.TryGet(key, out value);
            if (_secondContainer.IsContains(key))
                return _secondContainer.TryGet(key, out value);
            value = default(T);
            return false;
        }

        /// <summary>
        ///     Gets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="key">The specified key.</param>
        /// <param name="value">An instance of T.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        ///     <c>True</c> if the value is found in the <see cref="IParameterContainer" />; otherwise, <c>false</c>.
        /// </returns>
        public bool TryGet<T>(string key, out T value, T defaultValue)
        {
            if (_firstContainer.IsContains(key))
                return _firstContainer.TryGet(key, out value, defaultValue);
            if (_secondContainer.IsContains(key))
                return _secondContainer.TryGet(key, out value, defaultValue);
            value = defaultValue;
            return false;
        }

        #endregion
    }
}