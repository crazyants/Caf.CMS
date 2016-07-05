using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using CaptchaMvc.Interface;

namespace CaptchaMvc.Infrastructure
{
    /// <summary>
    ///     Represents default implementation to generate captcha image.
    /// </summary>
    public class DefaultImageGenerator : IImageGenerator
    {
        #region Fields

        private const float V = 4F;

        #endregion

        #region Constructor

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public DefaultImageGenerator()
        {
            Width = 200;
            Height = 70;
            Fonts = new List<FontFamily>
                        {
                            new FontFamily("Times New Roman"),
                            new FontFamily("Georgia"),
                            new FontFamily("Arial"),
                            new FontFamily("Comic Sans MS")
                        };
            FontColor = Color.Blue;
            Background = Color.White;
        }

        #endregion

        #region Implementation of IImageGenerator

        /// <summary>
        ///     Gets or sets the font color.
        /// </summary>
        public Color FontColor { get; set; }

        /// <summary>
        ///     Gets or sets the background color.
        /// </summary>
        public Color Background { get; set; }

        /// <summary>
        ///     Gets or sets the width.
        /// </summary>
        public ushort Width { get; set; }

        /// <summary>
        ///     Gets or sets the height.
        /// </summary>
        public ushort Height { get; set; }

        /// <summary>
        ///     Gets the fonts.
        /// </summary>
        public IList<FontFamily> Fonts { get; private set; }

        /// <summary>
        ///     Creates a captcha image using the specified <see cref="IDrawingModel" />.
        /// </summary>
        /// <param name="drawingModel">
        ///     The specified <see cref="IDrawingModel" />.
        /// </param>
        /// <returns>
        ///     An instance of <see cref="Bitmap" />.
        /// </returns>
        public Bitmap Generate(IDrawingModel drawingModel)
        {
            var random = new Random();
            string text = drawingModel.Text;
            //Randomly choose the font name.
            FontFamily familyName = Fonts[random.Next(Fonts.Count - 1)];
            // Create a new 32-bit bitmap image.
            var bitmap = new Bitmap(
                Width,
                Height,
                PixelFormat.Format32bppArgb);

            // Create a graphics object for drawing.
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                var rect = new Rectangle(0, 0, Width, Height);

                // Fill in the background.
                using (var brush = new HatchBrush(
                    HatchStyle.SmallConfetti,
                    Color.LightGray,
                    Background))
                {
                    g.FillRectangle(brush, rect);

                    // Set up the text format.
                    var format = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center,
                        FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.NoClip,
                        Trimming = StringTrimming.None
                    };

                    format.SetMeasurableCharacterRanges(new[] { new CharacterRange(0, text.Length) });

                    // Set up the text font.
                    RectangleF size;
                    float fontSize = rect.Height + 1;
                    Font font = null;
                    // Adjust the font size until the text fits within the image.
                    do
                    {
                        if (font != null)
                            font.Dispose();
                        fontSize--;
                        font = new Font(
                            familyName,
                            fontSize,
                            FontStyle.Bold);
                        size = g.MeasureCharacterRanges(text, font, rect, format)[0].GetBounds(g);
                    } while (size.Width > rect.Width || size.Height > rect.Height);
                    // Check http://stackoverflow.com/questions/2292812/font-in-graphicspath-addstring-is-smaller-than-usual-font on why we have to convert to em
                    // Create a path using the text and warp it randomly.
                    var path = new GraphicsPath();
                    path.AddString(
                        text,
                        font.FontFamily,
                        (int)font.Style,
                        g.DpiY * font.Size / 72, rect,
                        format);

                    PointF[] points =
                        {
                            new PointF(
                                random.Next(rect.Width)/V,
                                random.Next(rect.Height)/V),
                            new PointF(
                                rect.Width - random.Next(rect.Width)/V,
                                random.Next(rect.Height)/V),
                            new PointF(
                                random.Next(rect.Width)/V,
                                rect.Height - random.Next(rect.Height)/V),
                            new PointF(
                                rect.Width - random.Next(rect.Width)/V,
                                rect.Height - random.Next(rect.Height)/V)
                        };
                    var matrix = new Matrix();
                    matrix.Translate(0F, 0F);
                    path.Warp(points, rect, matrix, WarpMode.Perspective, 0F);

                    // Draw the text.
                    using (var hatchBrush = new HatchBrush(
                        HatchStyle.LargeConfetti,
                        Color.LightGray,
                        FontColor))
                    {
                        g.FillPath(hatchBrush, path);

                        // Add some random noise.
                        int m = Math.Max(rect.Width, rect.Height);
                        for (int i = 0; i < (int)(rect.Width * rect.Height / 30F); i++)
                        {
                            int x = random.Next(rect.Width);
                            int y = random.Next(rect.Height);
                            int w = random.Next(m / 50);
                            int h = random.Next(m / 50);
                            g.FillEllipse(hatchBrush, x, y, w, h);
                        }
                    }
                    // Clean up.
                    font.Dispose();
                }
                // Set the image.
                return bitmap;
            }
        }

        #endregion
    }
}