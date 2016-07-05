

using CAF.Infrastructure.Core.Configuration;
namespace CAF.Infrastructure.Core.Domain.Cms.Media
{
    public class MediaSettings : ISettings
    {
        public MediaSettings()
        {
            AvatarPictureSize = 85;
            ProductThumbPictureSize = 100;
            ArticleThumbPictureSize = 195;
            DetailsPictureSize = 300;
            ThumbPictureSizeOnDetailsPage = 70;
            AssociatedPictureSize = 125;
            BundledPictureSize = 70;
            CategoryThumbPictureSize = 125;
            ManufacturerThumbPictureSize = 125;
            ClientThumbPictureSize = 125;
            CartThumbPictureSize = 80;
            CartThumbBundleItemPictureSize = 32;
            MiniCartThumbPictureSize = 32;
            AutoCompleteSearchThumbPictureSize = 20;
            MaximumImageSize = 1280;
            DefaultPictureZoomEnabled = true;
            PictureZoomType = "window";
            DefaultImageQuality = 90;
            MultipleThumbDirectories = true;
            WatermarkName = "Watermark.png";
        }

        public int AvatarPictureSize { get; set; }
        public int ProductThumbPictureSize { get; set; }
        public int ArticleThumbPictureSize { get; set; }
        public int DetailsPictureSize { get; set; }
        public int ThumbPictureSizeOnDetailsPage { get; set; }
        public int AssociatedPictureSize { get; set; }
        public int BundledPictureSize { get; set; }
        public int CategoryThumbPictureSize { get; set; }
        public int ManufacturerThumbPictureSize { get; set; }
        public int ClientThumbPictureSize { get; set; }
        public int CartThumbPictureSize { get; set; }
        public int CartThumbBundleItemPictureSize { get; set; }
        public int MiniCartThumbPictureSize { get; set; }
        public int AutoCompleteSearchThumbPictureSize { get; set; }

        public bool DefaultPictureZoomEnabled { get; set; }
        public string PictureZoomType { get; set; }

        public int MaximumImageSize { get; set; }

        /// <summary>
        /// Geta or sets a default quality used for image generation
        /// </summary>
        public int DefaultImageQuality { get; set; }

        /// <summary>
        /// Geta or sets a vaue indicating whether single (/media/thumbs/) or multiple (/media/thumbs/0001/ and /media/thumbs/0002/) directories will used for picture thumbs
        /// </summary>
        public bool MultipleThumbDirectories { get; set; }
        /// <summary>
        /// 图片水印类型
        /// </summary>
        public int WatermarkType { get; set; }

        /// <summary>
        /// 图片水印位置
        /// </summary>
        public int WatermarkPosition { get; set; }
        /// <summary>
        /// 图片生成质量
        /// </summary>
        public int WatermarkImgQuality { get; set; }
        /// <summary>
        /// 图片水印文件
        /// </summary>
        public string WatermarkName { get; set; }
        /// <summary>
        /// 水印透明度
        /// </summary>
        public int WatermarkTransparency { get; set; }
        /// <summary>
        /// 水印文字
        /// </summary>
        public string WatermarkText { get; set; }
        /// <summary>
        /// 文字字体
        /// </summary>
        public string WatermarkFont { get; set; }
        /// <summary>
        /// 文字大小(像素)
        /// </summary>
        public int WatermarkFontSize { get; set; }
    }
}