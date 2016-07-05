using System;
using JetBrains.Annotations;

namespace CaptchaMvc.Infrastructure
{
    /// <summary>
    ///     A static helper class that includes various parameter checking routines.
    /// </summary>
    internal static class Validate
    {
        /// <summary>
        ///     Throws <see cref="ArgumentNullException" /> if the given argument is null.
        /// </summary>
        /// <exception cref="ArgumentNullException"> if tested value if null.</exception>
        /// <param name="argumentValue">Argument value to test.</param>
        /// <param name="paramName">Name of the parameter being tested. </param>
        internal static void ArgumentNotNull(object argumentValue, [InvokerParameterName] string paramName)
        {
            if (argumentValue == null)
                throw new ArgumentNullException(paramName);
        }

        /// <summary>
        ///     Throws <see cref="ArgumentNullException" /> if the given argument is null.
        /// </summary>
        /// <exception cref="ArgumentNullException"> if tested value if null.</exception>
        /// <param name="argumentValue">Argument value to test.</param>
        /// <param name="paramName">Name of the parameter being tested. </param>
        internal static void ArgumentNotNullOrEmpty(string argumentValue, [InvokerParameterName] string paramName)
        {
            if (string.IsNullOrEmpty(argumentValue))
                throw new ArgumentNullException(paramName);
        }

        /// <summary>
        ///     Throws <see cref="ArgumentNullException" /> if the given argument is null.
        /// </summary>
        /// <exception cref="ArgumentNullException"> if tested value if null.</exception>
        /// <param name="argumentValue">Argument value to test.</param>
        /// <param name="paramName">Name of the parameter being tested. </param>
        internal static void PropertyNotNull(object argumentValue, string paramName)
        {
            if (argumentValue == null)
                throw new ArgumentNullException(paramName,
                                                string.Format("The property with name '{0}' cannot be null.", paramName));
        }

        /// <summary>
        ///     Throws <see cref="ArgumentNullException" /> if the given argument is null.
        /// </summary>
        /// <exception cref="ArgumentNullException"> if tested value if null.</exception>
        /// <param name="argumentValue">Argument value to test.</param>
        /// <param name="paramName">Name of the parameter being tested. </param>
        internal static void PropertyNotNullOrEmpty(string argumentValue, string paramName)
        {
            if (string.IsNullOrEmpty(argumentValue))
                throw new ArgumentNullException(paramName,
                                                string.Format("The property with name '{0}' cannot be null or empty.", paramName));
        }
    }
}