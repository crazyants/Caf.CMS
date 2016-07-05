using CAF.Infrastructure.Core.Configuration;
using CAF.Infrastructure.Core.Utilities;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Data;
using CAF.Infrastructure.Core.Email;
using CAF.WebSite.Application.Services.Common;
using CAF.Infrastructure.Core.Domain.Cms;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Cms.Channels;
using CAF.Infrastructure.Core.Domain.Cms.Forums;
using CAF.Infrastructure.Core.Domain.Cms.Media;
using CAF.Infrastructure.Core.Domain.Cms.Payments;
using CAF.Infrastructure.Core.Domain.Cms.Topic;
using CAF.Infrastructure.Core.Domain;
using CAF.Infrastructure.Core.Domain.Cms.Polls;
using CAF.Infrastructure.Core.Domain.Common;
using CAF.Infrastructure.Core.Domain.Directory;
using CAF.Infrastructure.Core.Domain.Localization;
using CAF.Infrastructure.Core.Domain.Logging;
using CAF.Infrastructure.Core.Domain.Messages;
using CAF.Infrastructure.Core.Domain.Security;
using CAF.Infrastructure.Core.Domain.Seo;
using CAF.Infrastructure.Core.Domain.Sites;
using CAF.Infrastructure.Core.Domain.Tasks;
using CAF.Infrastructure.Core.Domain.Tax;
using CAF.Infrastructure.Core.Domain.Themes;
using CAF.Infrastructure.Core.Domain.Users;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CAF.Infrastructure.Data.Setup;

 

namespace CAF.WebSite.Mvc.Infrastructure.Installation
{

    public class EnUSSeedData : InvariantSeedData
    {

		public EnUSSeedData()
        {
        }

        protected override void Alter(IList<ISettings> settings)
        {
            base.Alter(settings);

            settings
                .Alter<ContentSliderSettings>(x =>
                {
					var slidePics = base.DbContext.Set<Picture>().ToList();

					var slide1PicId = slidePics.Where(p => p.SeoFilename == base.GetSeName("slide-1")).First().Id;
					var slide2PicId = slidePics.Where(p => p.SeoFilename == base.GetSeName("slide-2")).First().Id;
					var slide3PicId = slidePics.Where(p => p.SeoFilename == base.GetSeName("slide-3")).First().Id;

                    //slide 1
                    x.Slides.Add(new ContentSliderSlideSettings
                    {
                        DisplayOrder = 1,
                        //LanguageName = "English",
                        Title = "The biggest thing that could ever happen to the iPhone",
                        Text = @"<ul>
                                    <li>Thinner, slighter design</li>
                                    <li>4"" retina display.</li>
                                    <li>Ultrafast mobile data.</li>                       
                                </ul>",
                        Published = true,
                        LanguageCulture = "en-US",
                        PictureId = slide1PicId,
                        Button1 = new ContentSliderButtonSettings
                        {
                            Published = true,
                            Text = "more...",
                            Type = "btn-primary",
                            Url = "~/iphone-5"
                        },
                        Button2 = new ContentSliderButtonSettings
                        {
                            Published = true,
                            Text = "Buy now",
                            Type = "btn-danger",
                            Url = "~/iphone-5"
                        },
                        Button3 = new ContentSliderButtonSettings
                        {
                            Published = false
                        }
                    });
                    //slide 2
                    x.Slides.Add(new ContentSliderSlideSettings
                    {
                        DisplayOrder = 2,
                        //LanguageName = "English",
                        Title = "Buy music online!",
                        Text = @"<p>Buy here & download at once.</p>
                                 <p>Best quality at 320 kbit/s.</p>
                                 <p>Sample and download at light speed.</p>",
                        Published = true,
                        LanguageCulture = "en-US",
                        PictureId = slide2PicId,
                        Button1 = new ContentSliderButtonSettings
                        {
                            Published = true,
                            Text = "more...",
                            Type = "btn-warning",
                            Url = "~/musik-kaufen-sofort-herunterladen"
                        },
                        Button2 = new ContentSliderButtonSettings
                        {
                            Published = false
                        },
                        Button3 = new ContentSliderButtonSettings
                        {
                            Published = false
                        }
                    });
                    //slide 3
                    x.Slides.Add(new ContentSliderSlideSettings
                    {
                        DisplayOrder = 3,
                        //LanguageName = "English",
                        Title = "Ready for the revolution?",
                        Text = @"<p>SmartStore.NET is the new dynamic E-Commerce solution from SmartStore.</p>
                                 <ul>
                                     <li>Order-, customer- and stock-management.</li>
                                     <li>SEO optimized | 100% mobile optimized.</li>
                                     <li>Reviews &amp; Ratings | SmartStore.biz Import.</li>
                                 </ul>",
                        Published = true,
                        LanguageCulture = "en-US",
                        PictureId = slide3PicId,
                        Button1 = new ContentSliderButtonSettings
                        {
                            Published = true,
                            Text = "and much more...",
                            Type = "btn-success",
                            Url = "http://www.mayisite.com"
                        },
                        Button2 = new ContentSliderButtonSettings
                        {
                            Published = false
                        },
                        Button3 = new ContentSliderButtonSettings
                        {
                            Published = false
                        }
                    });
                });
        }


    }

}
