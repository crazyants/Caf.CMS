namespace CaptchaMvc.Interface
{
    /// <summary>
    ///     Represents a container with values​​.
    /// </summary>
    public interface IParameterContainer
    {
        /// <summary>
        ///    Gets the data source of parameters.
        /// </summary>
        object ParameterProvider { get; }

        /// <summary>
        ///     Determines whether the <see cref="IParameterContainer" /> contains a specific key.
        /// </summary>
        /// <param name="key">The specified key.</param>
        /// <returns>
        ///     <c>True</c> if the value is found in the <see cref="IParameterContainer" />; otherwise, <c>false</c>.
        /// </returns>
        bool IsContains(string key);

        /// <summary>
        ///     Gets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="key">The specified key.</param>
        /// <returns>An instance of T.</returns>
        T Get<T>(string key);

        /// <summary>
        ///     Gets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="key">The specified key.</param>
        /// <param name="value">An instance of T.</param>
        /// <returns>
        ///     <c>True</c> if the value is found in the <see cref="IParameterContainer" />; otherwise, <c>false</c>.
        /// </returns>
        bool TryGet<T>(string key, out T value);

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
        bool TryGet<T>(string key, out T value, T defaultValue);
    }
}