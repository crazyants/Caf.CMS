using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.Mvc;
using CaptchaMvc.Interface;
using CaptchaMvc.Models;

namespace CaptchaMvc.Infrastructure
{
    /// <summary>
    ///     Provides a basic methods to create a captcha.
    /// </summary>
    public class DefaultCaptchaBuilderProvider : ICaptchaBuilderProvider
    {
        #region Fields

        private static readonly byte[] ErrorBytes = CreateErrorBitmap();
        private Func<IBuildInfoModel, ICaptchaBulder> _captchaBuilderFactory;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultCaptchaBuilderProvider" /> class.
        /// </summary>
        public DefaultCaptchaBuilderProvider()
        {
#pragma warning disable 612,618
            CaptchaBuilderFactory = GetCaptchaBuilder;
#pragma warning restore 612,618
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the factory to create <c>ICaptchaBulder</c> for the specified <see cref="IBuildInfoModel" />
        /// </summary>
        public Func<IBuildInfoModel, ICaptchaBulder> CaptchaBuilderFactory
        {
            get { return _captchaBuilderFactory; }
            set
            {
                Validate.PropertyNotNull(value, "CaptchaBuilderFactory");
                _captchaBuilderFactory = value;
            }
        }

        #endregion

        #region Implementation of ICaptchaBuilderProvider

        /// <summary>
        ///     Creates a new captcha to the specified <see cref="IBuildInfoModel" />.
        /// </summary>
        /// <param name="buildInfoModel">
        ///     The specified <see cref="IBuildInfoModel" />.
        /// </param>
        /// <returns>The html string with the captcha.</returns>
        public virtual ICaptcha GenerateCaptcha(IBuildInfoModel buildInfoModel)
        {
            Validate.ArgumentNotNull(buildInfoModel, "buildInfoModel");
            return CaptchaBuilderFactory(buildInfoModel).Build(buildInfoModel);
        }

        /// <summary>
        ///     Creates a captcha image for specified <see cref="IDrawingModel" /> and write it in response.
        /// </summary>
        /// <param name="response">
        ///     The specified <see cref="HttpResponseBase" />.
        /// </param>
        /// <param name="drawingModel">
        ///     The specified <see cref="IDrawingModel" />.
        /// </param>
        public virtual void WriteCaptchaImage(HttpResponseBase response, IDrawingModel drawingModel)
        {
            Validate.ArgumentNotNull(response, "response");
            Validate.ArgumentNotNull(drawingModel, "drawingModel");
            using (Bitmap bitmap = CaptchaUtils.ImageGeneratorFactory(drawingModel).Generate(drawingModel))
            {
                response.ContentType = "image/gif";
                bitmap.Save(response.OutputStream, ImageFormat.Gif);
            }
        }

        /// <summary>
        ///     Creates a captcha error image and write it in response.
        /// </summary>
        /// <param name="response">
        ///     The specified <see cref="HttpResponse" />.
        /// </param>
        public virtual void WriteErrorImage(HttpResponseBase response)
        {
            Validate.ArgumentNotNull(response, "response");
            response.ContentType = "image/gif";
            response.OutputStream.Write(ErrorBytes, 0, ErrorBytes.Length);
        }

        /// <summary>
        ///     Generates a java-script to update the captcha.
        /// </summary>
        /// <param name="updateInfo">
        ///     The specified <see cref="IUpdateInfoModel" />.
        /// </param>
        /// <returns>
        ///     An instance of <see cref="ActionResult" /> to update the captcha.
        /// </returns>
        public virtual ActionResult RefreshCaptcha(IUpdateInfoModel updateInfo)
        {
            Validate.ArgumentNotNull(updateInfo, "updateInfo");
            string script = string.Format(@"$('#{0}').attr(""value"", ""{1}"");
$('#{2}').attr(""src"", ""{3}"");", updateInfo.TokenElementId,
                                          updateInfo.TokenValue,
                                          updateInfo.ImageElementId, updateInfo.ImageUrl);
            return new JavaScriptResult { Script = script };
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets the <see cref="ICaptchaBulder" /> for build captcha with specified <see cref="IBuildInfoModel" />.
        /// </summary>
        /// <param name="buildInfoModel">
        ///     The specified <see cref="IBuildInfoModel" />.
        /// </param>
        /// <returns>
        ///     An instance of <see cref="ICaptchaBulder" />.
        /// </returns>
        private static ICaptchaBulder GetCaptchaBuilder(IBuildInfoModel buildInfoModel)
        {
            if (buildInfoModel is DefaultBuildInfoModel)
                return new DefaultCaptchaBuilder();
            if (buildInfoModel is MathBuildInfoModel)
                return new MathCaptchaBuilder();
            if (buildInfoModel is PartialBuildInfoModel)
                return new PartialCaptchaBuilder();
            throw new NotSupportedException(
                "DefaultCaptchaBuilderProvider does not support the type of a IBuildInfoModel = " +
                buildInfoModel.GetType());
        }

        private static byte[] CreateErrorBitmap()
        {
            using (var errorBmp = new Bitmap(200, 70))
            {
                using (Graphics gr = Graphics.FromImage(errorBmp))
                {
                    gr.DrawLine(Pens.Red, 0, 0, 200, 70);
                    gr.DrawLine(Pens.Red, 0, 70, 200, 0);
                }
                using (var memoryStream = new MemoryStream())
                {
                    errorBmp.Save(memoryStream, ImageFormat.Gif);
                    return memoryStream.ToArray();
                }
            }
        }

        #endregion
    }
}