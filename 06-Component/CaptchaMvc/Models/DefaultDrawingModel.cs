using System;
using System.Collections.Generic;
using CaptchaMvc.Infrastructure;
using CaptchaMvc.Interface;

namespace CaptchaMvc.Models
{
    /// <summary>
    /// Represents the base model with information for drawing a captcha.
    /// </summary>
    public class DefaultDrawingModel : IDrawingModel
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDrawingModel"/> class.
        /// </summary>
        public DefaultDrawingModel(string text)
            : this(text, new Dictionary<string, object>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDrawingModel"/> class.
        /// </summary>
        public DefaultDrawingModel(string text, IDictionary<string, object> attributes)
        {
            Validate.ArgumentNotNullOrEmpty(text, "text");
            Validate.ArgumentNotNull(attributes, "attributes");
            Text = text;
            Attributes = attributes;
        }

        #endregion

        #region Implementation of IDrawingModel

        /// <summary>
        /// Gets the specified attributes.
        /// </summary>
        public IDictionary<string, object> Attributes { get; private set; }

        /// <summary>
        /// Gets the specified text for render.
        /// </summary>
        public string Text { get; private set; }

        #endregion
    }
}