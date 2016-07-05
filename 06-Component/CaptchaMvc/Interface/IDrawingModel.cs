using System.Collections.Generic;

namespace CaptchaMvc.Interface
{
    /// <summary>
    /// Represents the base model with information for drawing a captcha.
    /// </summary>
    public interface IDrawingModel
    {
        /// <summary>
        /// Gets the specified attributes.
        /// </summary>
        IDictionary<string, object> Attributes { get; }

        /// <summary>
        /// Gets the specified text for render.
        /// </summary>
        string Text { get; }
    }
}