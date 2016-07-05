using System;
using CaptchaMvc.Infrastructure;

namespace CaptchaMvc.Models
{
    /// <summary>
    ///     Represents the base model for captcha parameter.
    /// </summary>
    public class ParameterModel
    {
        #region Constructor

        /// <summary>
        ///     Initializes a new instance of the <see cref="ParameterModel" /> class.
        /// </summary>
        public ParameterModel(string name, object value)
        {
            Validate.ArgumentNotNullOrEmpty(name, "name");
            Name = name;
            Value = value;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     Gets the value.
        /// </summary>
        public object Value { get; private set; }

        #endregion
    }
}