
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CAF.WebSite.Mvc.Admin.Models.Settings
{
	public class MediaSettingsModel : ModelBase
    {
        public MediaSettingsModel()
        {
            this.AvailablePictureZoomTypes = new List<SelectListItem>();
            this.AvailableWatermarkTypes = new List<SelectListItem>();
            this.AvailableWatermarkPositions = new List<SelectListItem>();
            
            
        }

        [LangResourceDisplayName("Admin.Configuration.Settings.Media.PicturesStoredIntoDatabase")]
        public bool PicturesStoredIntoDatabase { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Media.AvatarPictureSize")]
        public int AvatarPictureSize { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Media.ArticleThumbPictureSize")]
        public int ArticleThumbPictureSize { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Media.ArticleDetailsPictureSize")]
        public int ArticleDetailsPictureSize { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.Media.ArticleThumbPictureSizeOnArticleDetailsPage")]
        public int ArticleThumbPictureSizeOnArticleDetailsPage { get; set; }

		[LangResourceDisplayName("Admin.Configuration.Settings.Media.AssociatedArticlePictureSize")]
        public int AssociatedArticlePictureSize { get; set; }


        [LangResourceDisplayName("Admin.Configuration.Settings.Media.CategoryThumbPictureSize")]
        public int CategoryThumbPictureSize { get; set; }


        [LangResourceDisplayName("Admin.Configuration.Settings.Media.MiniCartThumbPictureSize")]
        public int MiniCartThumbPictureSize { get; set; }
        
        [LangResourceDisplayName("Admin.Configuration.Settings.Media.MaximumImageSize")]
        public int MaximumImageSize { get; set; }

        // codehint: sm-add
        [LangResourceDisplayName("Admin.Configuration.Settings.Media.DefaultPictureZoomEnabled")]
        public bool DefaultPictureZoomEnabled { get; set; }

        // codehint: sm-add (window || inner || lens)
        [LangResourceDisplayName("Admin.Configuration.Settings.Media.PictureZoomType")]
        public string PictureZoomType { get; set; }

        public List<SelectListItem> AvailablePictureZoomTypes { get; set; }



        /// <summary>
        /// 图片水印类型
        /// </summary>
        [LangResourceDisplayName("Admin.Configuration.Settings.Media.WatermarkType")]
        public int WatermarkType { get; set; }
        public List<SelectListItem> AvailableWatermarkTypes { get; set; }
        /// <summary>
        /// 图片水印位置
        /// </summary>
        [LangResourceDisplayName("Admin.Configuration.Settings.Media.WatermarkPosition")]
        public int WatermarkPosition { get; set; }
        public List<SelectListItem> AvailableWatermarkPositions { get; set; }
        /// <summary>
        /// 图片生成质量
        /// </summary>
        [LangResourceDisplayName("Admin.Configuration.Settings.Media.WatermarkimgQuality")]
        public int WatermarkImgQuality { get; set; }
        /// <summary>
        /// 图片水印文件
        /// </summary>
        [LangResourceDisplayName("Admin.Configuration.Settings.Media.WatermarkName")]
        public string WatermarkName { get; set; }
        /// <summary>
        /// 水印透明度
        /// </summary>
        [LangResourceDisplayName("Admin.Configuration.Settings.Media.WatermarkTransparency")]
        public int WatermarkTransparency { get; set; }
        /// <summary>
        /// 水印文字
        /// </summary>
        [LangResourceDisplayName("Admin.Configuration.Settings.Media.WatermarkText")]
        public string WatermarkText { get; set; }
        /// <summary>
        /// 文字字体
        /// </summary>
        [LangResourceDisplayName("Admin.Configuration.Settings.Media.WatermarkFont")]
        public string WatermarkFont { get; set; }
        /// <summary>
        /// 文字大小(像素)
        /// </summary>
        [LangResourceDisplayName("Admin.Configuration.Settings.Media.WatermarkFontSize")]
        public int WatermarkFontSize { get; set; }

    }
}