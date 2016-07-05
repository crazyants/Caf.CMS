using System.Collections.Generic;
using System.Drawing;

namespace CaptchaMvc.Interface
{
    /// <summary>
    ///     Represents interface to generate captcha image.
    /// </summary>
    public interface IImageGenerator
    {
        /// <summary>
        ///     Gets or sets the font color.
        /// </summary>
        Color FontColor { get; set; }

        /// <summary>
        ///     Gets or sets the background color.
        /// </summary>
        Color Background { get; set; }

        /// <summary>
        ///     Gets or sets the width.
        /// </summary>
        ushort Width { get; set; }

        /// <summary>
        ///     Gets or sets the height.
        /// </summary>
        ushort Height { get; set; }

        /// <summary>
        ///     Gets the fonts.
        /// </summary>
        IList<FontFamily> Fonts { get; }

        /// <summary>
        ///     Creates a captcha image using the specified <see cref="IDrawingModel" />.
        /// </summary>
        /// <param name="drawingModel">
        ///     The specified <see cref="IDrawingModel" />.
        /// </param>
        /// <returns>
        ///     An instance of <see cref="Bitmap" />.
        /// </returns>
        Bitmap Generate(IDrawingModel drawingModel);
    }
}