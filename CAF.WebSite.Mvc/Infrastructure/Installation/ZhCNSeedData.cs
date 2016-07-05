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

    public class ZhCNSeedData : InvariantSeedData
    {

        public ZhCNSeedData()
        {
        }

        protected override void Alter(IList<ISettings> settings)
        {
            base.Alter(settings);

            settings
                .Alter<ContentSliderSettings>(x =>
                {
                    var slidePics = base.DbContext.Set<Picture>().ToList();
                    var slide1BG= slidePics.Where(p => p.SeoFilename == base.GetSeName("slider-bg")).First().Id;
                    var slide1PicId = slidePics.Where(p => p.SeoFilename == base.GetSeName("slide-1")).First().Id;
                    var slide2PicId = slidePics.Where(p => p.SeoFilename == base.GetSeName("slide-2")).First().Id;
                    var slide3PicId = slidePics.Where(p => p.SeoFilename == base.GetSeName("slide-3")).First().Id;

                    //slide 1
                    x.Slides.Add(new ContentSliderSlideSettings
                    {
                        DisplayOrder = 1,
                        BackgroundPictureId=slide1BG,
                        Title = "幻灯片1",
                        Text = @"<ul>
                                    <li>内容</li>                   
                                </ul>",
                        Published = true,
                        LanguageCulture = "zh-CN",
                        PictureId = slide1PicId,
                        Button1 = new ContentSliderButtonSettings
                        {
                            Published = true,
                            Text = "更多...",
                            Type = "btn-primary",
                            Url = "~/iphone-5"
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
                    //slide 2
                    x.Slides.Add(new ContentSliderSlideSettings
                    {
                        DisplayOrder = 2,
                        BackgroundPictureId = slide1BG,
                        Title = "幻灯片2",
                        Text = @"<ul>
                                    <li>内容</li>                   
                                </ul>",
                        Published = true,
                        LanguageCulture = "zh-CN",
                        PictureId = slide2PicId,
                        Button1 = new ContentSliderButtonSettings
                        {
                            Published = true,
                            Text = "更多...",
                            Type = "btn-primary",
                            Url = "~/iphone-5"
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
                        BackgroundPictureId = slide1BG,
                        Title = "幻灯片3",
                        Text = @"<ul>
                                    <li>内容</li>                   
                                </ul>",
                        Published = true,
                        LanguageCulture = "zh-CN",
                        PictureId = slide3PicId,
                        Button1 = new ContentSliderButtonSettings
                        {
                            Published = true,
                            Text = "更多...",
                            Type = "btn-primary",
                            Url = "~/iphone-5"
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
