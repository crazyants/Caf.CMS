
using System;
﻿using System.Collections.Generic;
﻿using System.ComponentModel.DataAnnotations;
using System.Web.DynamicData;
using System.Web.Mvc;
using FluentValidation.Attributes;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Mvc.Admin.Models.Sites;
using CAF.WebSite.Mvc.Admin.Validators.ContentSlider;



namespace CAF.WebSite.Mvc.Admin.Models.Settings
{
    //[Validator(typeof(ContentSliderValidator))]
    public class ContentSliderSettingsModel : EntityModelBase
    {
        public ContentSliderSettingsModel()
        {
            Slides = new List<ContentSliderSlideModel>();
			AvailableSites = new List<SelectListItem>();
        }

        [LangResourceDisplayName("Admin.Configuration.ContentSlider.IsActive")]
        public bool IsActive { get; set; }

        [LangResourceDisplayName("Admin.Configuration.ContentSlider.SliderHeight")]
        public string ContentSliderHeight { get; set; }
        [LangResourceDisplayName("Admin.Configuration.ContentSlider.SliderWidth")]
        public string ContentSliderWidth { get; set; }

        [LangResourceDisplayName("Admin.Configuration.ContentSlider.Background")]
        public int BackgroundPictureId { get; set; }

        public string BackgroundPictureUrl { get; set; }

        [LangResourceDisplayName("Admin.Configuration.ContentSlider.AutoPlay")]
        public bool AutoPlay { get; set; }

        [LangResourceDisplayName("Admin.Configuration.ContentSlider.AutoPlayDelay")]
        public int AutoPlayDelay { get; set; }

        public IList<ContentSliderSlideModel> Slides { get; set; }
		public IList<SelectListItem> AvailableSites { get; set; }

		[LangResourceDisplayName("Admin.Common.Site.SearchFor")]
		public int SearchSiteId { get; set; }
    }

    [Validator(typeof(ContentSliderSlideValidator))]
    public class ContentSliderSlideModel : EntityModelBase
    {
        public ContentSliderSlideModel()
        {
            Button1 = new ContentSliderButtonModel();
            Button2 = new ContentSliderButtonModel();
            Button3 = new ContentSliderButtonModel();
        }
		public int SlideIndex { get; set; }

        [LangResourceDisplayName("Admin.Configuration.ContentSlider.Slide.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [LangResourceDisplayName("Admin.Configuration.ContentSlider.Slide.Color")]
        [UIHint("Color")]
        public string Color { get; set; }
        [LangResourceDisplayName("Admin.Configuration.ContentSlider.Slide.Title")]
        [AllowHtml]
        public string Title { get; set; }

        [LangResourceDisplayName("Admin.Configuration.ContentSlider.Slide.Text")]
        [AllowHtml]
        public string Text { get; set; }

        [LangResourceDisplayName("Admin.Configuration.ContentSlider.Slide.Picture")]
        public int PictureId { get; set; }

        public string PictureUrl { get; set; }

        [LangResourceDisplayName("Admin.Configuration.ContentSlider.Background")]
        public int BackgroundPictureId { get; set; }

        public string BackgroundPictureUrl { get; set; }

        [LangResourceDisplayName("Admin.Configuration.ContentSlider.Slide.Language")]
        public string LanguageName { get; set; }

        [LangResourceDisplayName("Admin.Configuration.ContentSlider.Slide.Language")]
        public string LanguageCulture { get; set; }

        [LangResourceDisplayName("Admin.Configuration.ContentSlider.Slide.Published")]
        public bool Published { get; set; }

		//Site mapping
		[LangResourceDisplayName("Admin.Common.Site.LimitedTo")]
		public bool LimitedToSites { get; set; }

		[LangResourceDisplayName("Admin.Common.Site.AvailableFor")]
		public List<SiteModel> AvailableSites { get; set; }
		public int[] SelectedSiteIds { get; set; }

        [LangResourceDisplayName("Admin.Configuration.ContentSlider.Slide.Button1")]
        public ContentSliderButtonModel Button1 { get; set; }

        [LangResourceDisplayName("Admin.Configuration.ContentSlider.Slide.Button2")]
        public ContentSliderButtonModel Button2 { get; set; }

        [LangResourceDisplayName("Admin.Configuration.ContentSlider.Slide.Button3")]
        public ContentSliderButtonModel Button3 { get; set; }
    }

    [Validator(typeof(ContentSliderButtonValidator))]
    public class ContentSliderButtonModel : EntityModelBase
    {
        [Display(Description = "Admin.Configuration.ContentSlider.Button.Text.Hint")]
        [LangResourceDisplayName("Admin.Configuration.ContentSlider.Button.Text")]
        public string Text { get; set; }

        [LangResourceDisplayName("Admin.Configuration.ContentSlider.Button.Type")]
        [Display(Description = "Admin.Configuration.ContentSlider.Button.Type.Hint")]
        [UIHint("ButtonType")]
        public string Type { get; set; }

        [Display(Description = "Admin.Configuration.ContentSlider.Button.Url.Hint")]
        [LangResourceDisplayName("Admin.Configuration.ContentSlider.Button.Url")]
        public string Url { get; set; }

        [Display(Description = "Admin.Configuration.ContentSlider.Button.Published.Hint")]
        [LangResourceDisplayName("Admin.Configuration.ContentSlider.Button.Published")]
        public bool Published { get; set; }
    }
}