using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AutoMapper;
using AutoMapper.Mappers;
using CAF.Infrastructure.Core;
using CAF.WebSite.Mvc.Admin.Models.Channels;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.WebSite.Mvc.Admin.Models.Articles;
using CAF.Infrastructure.Core.Domain.Cms.Media;
using CAF.Infrastructure.Core.Domain.Sites;
using CAF.WebSite.Mvc.Admin.Models.Sites;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.WebSite.Mvc.Admin.Models.Users;
using CAF.Mvc.JQuery.Datatables.Models;
using CAF.WebSite.Mvc.Admin.Models.Localization;
using CAF.Infrastructure.Core.Domain.Localization;
using CAF.WebSite.Mvc.Admin.Models.Topics;
using CAF.Infrastructure.Core.Domain.Cms.Topic;
using CAF.WebSite.Mvc.Admin.Models.Links;
using CAF.Infrastructure.Core.Plugins;
using CAF.WebSite.Mvc.Admin.Models.Plugins;
using CAF.Infrastructure.Core.Domain.Logging;
using CAF.WebSite.Mvc.Admin.Models.Logging;
using CAF.Infrastructure.Core.Domain.Messages;
using CAF.WebSite.Mvc.Admin.Models.Messages;
using CAF.Infrastructure.Core.Email;
using CAF.Infrastructure.Core.Domain.Cms;
using CAF.WebSite.Mvc.Admin.Models.Settings;
using CAF.Infrastructure.Core.Domain.Common;
using CAF.WebSite.Mvc.Admin.Models.Common;
using CAF.WebSite.Mvc.Admin.Models.Directory;
using CAF.Infrastructure.Core.Domain.Directory;
using CAF.WebSite.Mvc.Admin.Models.Discounts;
using CAF.WebSite.Application.Services.Seo;
using CAF.Infrastructure.Core.Domain.Cms.Clients;
using CAF.WebSite.Mvc.Admin.Models.Clients;
using CAF.Infrastructure.Core.Domain.Cms.Forums;
using CAF.WebSite.Mvc.Admin.Models.Polls;
using CAF.Infrastructure.Core.Domain.Cms.Polls;
using CAF.Infrastructure.Core.Domain.Themes;
using CAF.WebSite.Mvc.Admin.Models.Themes;
using CAF.WebSite.Mvc.Admin.Models.ExtendedAttributes;
using CAF.Infrastructure.Core.Domain.Cms.Channels;
using CAF.WebSite.Mvc.Admin.Models.ModelTemplates;
using CAF.Infrastructure.Core.Domain.Cms.RegionalContents;
using CAF.WebSite.Mvc.Admin.Models.RegionalContents;
using CAF.WebFeedbacks.Mvc.Admin.Models.Feedbacks;

namespace CAF.WebSite.Mvc.Admin.Infrastructure
{
    public class AutoMapperStartupTask : IStartupTask
    {
        class OptionalFkConverter : ITypeConverter<int, int?>
        {
            public int? Convert(ResolutionContext context)
            {
                var srcName = context.PropertyMap.SourceMember.Name;

                if (context.PropertyMap.SourceMember.MemberType == MemberTypes.Property && srcName.EndsWith("Id") && !context.SourceType.IsNullable())
                {
                    var src = (int)context.SourceValue;
                    return src == 0 ? (int?)null : src;
                }

                return (int?)context.SourceValue;
            }
        }

        public void Execute()
        {
            //TODO remove 'CreatedOnUtc' ignore mappings because now presentation layer models have 'CreatedOn' property and core entities have 'CreatedOnUtc' property (distinct names)

            // special mapper, that avoids DbUpdate exceptions in cases where
            // optional (nullable) int FK properties are 0 instead of null 
            // after mapping model > entity.
            Mapper.CreateMap<int, int?>().ConvertUsing(new OptionalFkConverter());

            //address
            Mapper.CreateMap<Address, AddressModel>()
                .ForMember(dest => dest.AddressHtml, mo => mo.Ignore())
                .ForMember(dest => dest.AvailableCountries, mo => mo.Ignore())
                .ForMember(dest => dest.AvailableStates, mo => mo.Ignore())
                .ForMember(dest => dest.FirstNameEnabled, mo => mo.Ignore())
                .ForMember(dest => dest.FirstNameRequired, mo => mo.Ignore())
                .ForMember(dest => dest.LastNameEnabled, mo => mo.Ignore())
                .ForMember(dest => dest.LastNameRequired, mo => mo.Ignore())
                .ForMember(dest => dest.EmailEnabled, mo => mo.Ignore())
                .ForMember(dest => dest.EmailRequired, mo => mo.Ignore())
                .ForMember(dest => dest.CompanyEnabled, mo => mo.Ignore())
                .ForMember(dest => dest.CompanyRequired, mo => mo.Ignore())
                .ForMember(dest => dest.CountryEnabled, mo => mo.Ignore())
                .ForMember(dest => dest.StateProvinceEnabled, mo => mo.Ignore())
                .ForMember(dest => dest.CityEnabled, mo => mo.Ignore())
                .ForMember(dest => dest.CityRequired, mo => mo.Ignore())
                .ForMember(dest => dest.StreetAddressEnabled, mo => mo.Ignore())
                .ForMember(dest => dest.StreetAddressRequired, mo => mo.Ignore())
                .ForMember(dest => dest.StreetAddress2Enabled, mo => mo.Ignore())
                .ForMember(dest => dest.StreetAddress2Required, mo => mo.Ignore())
                .ForMember(dest => dest.ZipPostalCodeEnabled, mo => mo.Ignore())
                .ForMember(dest => dest.ZipPostalCodeRequired, mo => mo.Ignore())
                .ForMember(dest => dest.PhoneEnabled, mo => mo.Ignore())
                .ForMember(dest => dest.PhoneRequired, mo => mo.Ignore())
                .ForMember(dest => dest.FaxEnabled, mo => mo.Ignore())
                .ForMember(dest => dest.FaxRequired, mo => mo.Ignore())
                .ForMember(dest => dest.CountryName, mo => mo.MapFrom(src => src.Country != null ? src.Country.Name : null))
                .ForMember(dest => dest.StateProvinceName, mo => mo.MapFrom(src => src.StateProvince != null ? src.StateProvince.Name : null));
            Mapper.CreateMap<AddressModel, Address>()
                .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                .ForMember(dest => dest.Country, mo => mo.Ignore())
                .ForMember(dest => dest.StateProvince, mo => mo.Ignore());

            Mapper.CreateMap<ArticleCategoryModel, ArticleCategory>()
                     .ForMember(dest => dest.Deleted, mo => mo.Ignore());
            Mapper.CreateMap<ArticleCategory, ArticleCategoryModel>()
                    .ForMember(dest => dest.AvailableModelTemplates, mo => mo.Ignore())
                .ForMember(dest => dest.AvailableDefaultViewModes, mo => mo.Ignore())
                .ForMember(dest => dest.Locales, mo => mo.Ignore())
                .ForMember(dest => dest.Breadcrumb, mo => mo.Ignore())
                .ForMember(dest => dest.ParentCategoryBreadcrumb, mo => mo.Ignore())
                .ForMember(dest => dest.SeName, mo => mo.MapFrom(src => src.GetSeName(0, true, false)))
                .ForMember(dest => dest.AvailableUserRoles, mo => mo.Ignore())
                .ForMember(dest => dest.SelectedUserRoleIds, mo => mo.Ignore())
                .ForMember(dest => dest.AvailableSites, mo => mo.Ignore())
                .ForMember(dest => dest.SelectedSiteIds, mo => mo.Ignore());

            Mapper.CreateMap<ArticleModel, Article>().ForMember(dest => dest.DisplayOrder, mo => mo.Ignore())
                .ForMember(dest => dest.ArticleTags, mo => mo.Ignore())
                .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                .ForMember(dest => dest.ModifiedOnUtc, mo => mo.Ignore())
                .ForMember(dest => dest.Deleted, mo => mo.Ignore())
                .ForMember(dest => dest.ArticleAlbum, mo => mo.Ignore());
            Mapper.CreateMap<Article, ArticleModel>()
                .ForMember(dest => dest.PictureThumbnailUrl, mo => mo.Ignore())
                .ForMember(dest => dest.NoThumb, mo => mo.Ignore())
                .ForMember(dest => dest.AvailableModelTemplates, mo => mo.Ignore())
                .ForMember(dest => dest.Locales, mo => mo.Ignore())
                .ForMember(dest => dest.SeName, mo => mo.MapFrom(src => src.GetSeName(0, true, false)))
                .ForMember(dest => dest.AvailableUserRoles, mo => mo.Ignore())
                .ForMember(dest => dest.SelectedUserRoleIds, mo => mo.Ignore())
                .ForMember(dest => dest.AvailableSites, mo => mo.Ignore())
                .ForMember(dest => dest.SelectedSiteIds, mo => mo.Ignore())
                .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                .ForMember(dest => dest.UpdatedOn, mo => mo.Ignore())
                .ForMember(dest => dest.Locales, mo => mo.Ignore())
                .ForMember(dest => dest.Url, mo => mo.Ignore()); ;

            Mapper.CreateMap<MediaSettings, MediaSettingsModel>()
              .ForMember(dest => dest.PicturesStoredIntoDatabase, mo => mo.Ignore())
              .ForMember(dest => dest.AvailablePictureZoomTypes, mo => mo.Ignore());
            Mapper.CreateMap<MediaSettingsModel, MediaSettings>()
                //.ForMember(dest => dest.DefaultPictureZoomEnabled, mo => mo.Ignore())
                .ForMember(dest => dest.DefaultImageQuality, mo => mo.Ignore())
                .ForMember(dest => dest.MultipleThumbDirectories, mo => mo.Ignore())
                .ForMember(dest => dest.AutoCompleteSearchThumbPictureSize, mo => mo.Ignore());

            Mapper.CreateMap<UserSettings, GeneralUserSettingsModel.UserSettingsModel>();
            Mapper.CreateMap<GeneralUserSettingsModel.UserSettingsModel, UserSettings>()
                .ForMember(dest => dest.HashedPasswordFormat, mo => mo.Ignore())
                .ForMember(dest => dest.PasswordMinLength, mo => mo.Ignore())
                .ForMember(dest => dest.AvatarMaximumSizeBytes, mo => mo.Ignore())
                .ForMember(dest => dest.DownloadableProductsValidateUser, mo => mo.Ignore())
                .ForMember(dest => dest.OnlineUserMinutes, mo => mo.Ignore())
                .ForMember(dest => dest.PrefillLoginUserName, mo => mo.Ignore())
                .ForMember(dest => dest.PrefillLoginPwd, mo => mo.Ignore());
            Mapper.CreateMap<AddressSettings, GeneralUserSettingsModel.AddressSettingsModel>();
            Mapper.CreateMap<GeneralUserSettingsModel.AddressSettingsModel, AddressSettings>();
            Mapper.CreateMap<ForumSettings, ForumSettingsModel>()
              .ForMember(dest => dest.ForumEditorValues, mo => mo.Ignore());
            Mapper.CreateMap<ForumSettingsModel, ForumSettings>()
                .ForMember(dest => dest.TopicSubjectMaxLength, mo => mo.Ignore())
                .ForMember(dest => dest.StrippedTopicMaxLength, mo => mo.Ignore())
                .ForMember(dest => dest.PostMaxLength, mo => mo.Ignore())
                .ForMember(dest => dest.TopicPostsPageLinkDisplayCount, mo => mo.Ignore())
                .ForMember(dest => dest.LatestUserPostsPageSize, mo => mo.Ignore())
                .ForMember(dest => dest.PrivateMessagesPageSize, mo => mo.Ignore())
                .ForMember(dest => dest.ForumSubscriptionsPageSize, mo => mo.Ignore())
                .ForMember(dest => dest.PMSubjectMaxLength, mo => mo.Ignore())
                .ForMember(dest => dest.PMTextMaxLength, mo => mo.Ignore())
                .ForMember(dest => dest.HomePageActiveDiscussionsTopicCount, mo => mo.Ignore())
                .ForMember(dest => dest.ActiveDiscussionsPageTopicCount, mo => mo.Ignore())
                .ForMember(dest => dest.ForumSearchTermMinimumLength, mo => mo.Ignore());
         
            Mapper.CreateMap<ArticleCatalogSettings, ArticleCatalogSettingsModel>()
                //.ForMember(dest => dest.AvailableSubCategoryDisplayTypes, mo => mo.Ignore())
              .ForMember(dest => dest.AvailableDefaultViewModes, mo => mo.Ignore());
            Mapper.CreateMap<ArticleCatalogSettingsModel, ArticleCatalogSettings>()
                .ForMember(dest => dest.PageShareCode, mo => mo.Ignore())
                .ForMember(dest => dest.DefaultArticleRatingValue, mo => mo.Ignore())
                .ForMember(dest => dest.ArticleSearchTermMinimumLength, mo => mo.Ignore())
                .ForMember(dest => dest.UseSmallArticleBoxOnHomePage, mo => mo.Ignore())
                .ForMember(dest => dest.DefaultCategoryPageSizeOptions, mo => mo.Ignore())
                .ForMember(dest => dest.DisplayTierPricesWithDiscounts, mo => mo.Ignore())
                .ForMember(dest => dest.FileUploadMaximumSizeBytes, mo => mo.Ignore())
                .ForMember(dest => dest.FileUploadAllowedExtensions, mo => mo.Ignore())
                .ForMember(dest => dest.ArticleSearchPageSize, mo => mo.Ignore())
                .ForMember(dest => dest.MostRecentlyUsedCategoriesMaxSize, mo => mo.Ignore());
            //polls
            Mapper.CreateMap<Poll, PollModel>()
                .ForMember(dest => dest.StartDate, mo => mo.Ignore())
                .ForMember(dest => dest.EndDate, mo => mo.Ignore())
                .ForMember(dest => dest.AvailableSites, mo => mo.Ignore())
                .ForMember(dest => dest.SelectedSiteIds, mo => mo.Ignore());
            Mapper.CreateMap<PollModel, Poll>()
                .ForMember(dest => dest.PollAnswers, mo => mo.Ignore())
                .ForMember(dest => dest.Language, mo => mo.Ignore())
                .ForMember(dest => dest.StartDateUtc, mo => mo.Ignore())
                .ForMember(dest => dest.EndDateUtc, mo => mo.Ignore());
            //sites
            Mapper.CreateMap<Site, SiteModel>();
            Mapper.CreateMap<SiteModel, Site>();
            //ModelTemplate
            Mapper.CreateMap<ModelTemplate, ModelTemplateModel>();
            Mapper.CreateMap<ModelTemplateModel, ModelTemplate>();
            //Channel
            Mapper.CreateMap<Channel, ChannelModel>()
                .ForMember(dest => dest.ExtendedAttributes, mo => mo.Ignore())
                .ForMember(dest => dest.AvailableDetailModelTemplates, mo => mo.Ignore())
                .ForMember(dest => dest.AvailableModelTemplates, mo => mo.Ignore());
            Mapper.CreateMap<ChannelModel, Channel>()
                  .ForMember(dest => dest.ExtendedAttributes, mo => mo.Ignore());


            Mapper.CreateMap<UserRole, UserRoleModel>()
              .ForMember(dest => dest.TaxDisplayTypes, mo => mo.Ignore())
                /*.ForMember(dest => dest.TaxDisplayType, mo => mo.MapFrom((src) => src.TaxDisplayType))*/;
            Mapper.CreateMap<UserRoleModel, UserRole>()
                .ForMember(dest => dest.PermissionRecords, mo => mo.Ignore())
                /*.ForMember(dest => dest.TaxDisplayType, mo => mo.MapFrom((src) => src.TaxDisplayType))*/;
            //language
            Mapper.CreateMap<Language, LanguageModel>()
                .ForMember(dest => dest.FlagFileNames, mo => mo.Ignore())
                .ForMember(dest => dest.AvailableSites, mo => mo.Ignore())
                .ForMember(dest => dest.SelectedSiteIds, mo => mo.Ignore());
            Mapper.CreateMap<LanguageModel, Language>();
            //countries
            Mapper.CreateMap<CountryModel, Country>()
                .ForMember(dest => dest.StateProvinces, mo => mo.Ignore());
            Mapper.CreateMap<Country, CountryModel>()
                .ForMember(dest => dest.NumberOfStates, mo => mo.MapFrom(src => src.StateProvinces != null ? src.StateProvinces.Count : 0))
                .ForMember(dest => dest.Locales, mo => mo.Ignore());
            //state/provinces
            Mapper.CreateMap<StateProvince, StateProvinceModel>()
                .ForMember(dest => dest.DisplayOrder1, mo => mo.MapFrom(src => src.DisplayOrder))
                .ForMember(dest => dest.Locales, mo => mo.Ignore());
            Mapper.CreateMap<StateProvinceModel, StateProvince>()
                .ForMember(dest => dest.DisplayOrder, mo => mo.MapFrom(src => src.DisplayOrder1))
                .ForMember(dest => dest.Country, mo => mo.Ignore());
            //email account
            Mapper.CreateMap<EmailAccount, EmailAccountModel>()
                .ForMember(dest => dest.IsDefaultEmailAccount, mo => mo.Ignore())
                .ForMember(dest => dest.SendTestEmailTo, mo => mo.Ignore());
            Mapper.CreateMap<EmailAccountModel, EmailAccount>();
            //message template
            Mapper.CreateMap<MessageTemplate, MessageTemplateModel>()
                .ForMember(dest => dest.TokensTree, mo => mo.Ignore())
                .ForMember(dest => dest.Locales, mo => mo.Ignore())
                .ForMember(dest => dest.AvailableEmailAccounts, mo => mo.Ignore())
                .ForMember(dest => dest.AvailableSites, mo => mo.Ignore())
                .ForMember(dest => dest.SelectedSiteIds, mo => mo.Ignore());
            Mapper.CreateMap<MessageTemplateModel, MessageTemplate>();
            //queued email
            Mapper.CreateMap<QueuedEmail, QueuedEmailModel>()
                .ForMember(dest => dest.EmailAccountName, mo => mo.MapFrom(src => src.EmailAccount != null ? src.EmailAccount.FriendlyName : string.Empty))
                .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                .ForMember(dest => dest.SentOn, mo => mo.Ignore());
            Mapper.CreateMap<QueuedEmailModel, QueuedEmail>()
                .ForMember(dest => dest.CreatedOnUtc, dt => dt.Ignore())
                .ForMember(dest => dest.SentOnUtc, mo => mo.Ignore())
                .ForMember(dest => dest.EmailAccount, mo => mo.Ignore())
                .ForMember(dest => dest.EmailAccountId, mo => mo.Ignore())
                .ForMember(dest => dest.ReplyTo, mo => mo.Ignore())
                .ForMember(dest => dest.ReplyToName, mo => mo.Ignore());
            //campaign
            Mapper.CreateMap<Campaign, CampaignModel>()
                .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                .ForMember(dest => dest.AllowedTokens, mo => mo.Ignore())
                .ForMember(dest => dest.TestEmail, mo => mo.Ignore())
                .ForMember(dest => dest.AvailableSites, mo => mo.Ignore())
                .ForMember(dest => dest.SelectedSiteIds, mo => mo.Ignore());
            Mapper.CreateMap<CampaignModel, Campaign>()
                .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore());
           
            // Delivery times 
            Mapper.CreateMap<DeliveryTime, DeliveryTimeModel>()
                .ForMember(dest => dest.Locales, mo => mo.Ignore());
            Mapper.CreateMap<DeliveryTimeModel, DeliveryTime>();

            // Measure unit
            Mapper.CreateMap<QuantityUnit, QuantityUnitModel>()
                .ForMember(dest => dest.Locales, mo => mo.Ignore());
            Mapper.CreateMap<QuantityUnitModel, QuantityUnit>();

            // ContentSlider slides
            Mapper.CreateMap<ContentSliderSettings, ContentSliderSettingsModel>()
                .ForMember(dest => dest.Id, mo => mo.Ignore())
                .ForMember(dest => dest.AvailableSites, mo => mo.Ignore())
                .ForMember(dest => dest.SearchSiteId, mo => mo.Ignore());
            Mapper.CreateMap<ContentSliderSettingsModel, ContentSliderSettings>();

            Mapper.CreateMap<ContentSliderSlideSettings, ContentSliderSlideModel>()
                .ForMember(dest => dest.Id, mo => mo.Ignore())
                .ForMember(dest => dest.SlideIndex, mo => mo.Ignore())
                .ForMember(dest => dest.AvailableSites, mo => mo.Ignore());
            Mapper.CreateMap<ContentSliderSlideModel, ContentSliderSlideSettings>();

            Mapper.CreateMap<ContentSliderButtonSettings, ContentSliderButtonModel>()
                .ForMember(dest => dest.Id, mo => mo.Ignore());
            Mapper.CreateMap<ContentSliderButtonModel, ContentSliderButtonSettings>();

            //topcis
            Mapper.CreateMap<Topic, TopicModel>()
                .ForMember(dest => dest.Url, mo => mo.Ignore())
                .ForMember(dest => dest.Locales, mo => mo.Ignore())
                .ForMember(dest => dest.AvailableSites, mo => mo.Ignore())
                .ForMember(dest => dest.SelectedSiteIds, mo => mo.Ignore())
                .ForMember(dest => dest.AvailableWidgetZones, mo => mo.Ignore());
            Mapper.CreateMap<TopicModel, Topic>();
            //feedbacks
            Mapper.CreateMap<Feedback, FeedbackModel>();
            Mapper.CreateMap<FeedbackModel, Feedback>();
            //Link
            Mapper.CreateMap<Link, LinkModel>()
                  .ForMember(dest => dest.AvailableSites, mo => mo.Ignore())
                .ForMember(dest => dest.SelectedSiteIds, mo => mo.Ignore());
            Mapper.CreateMap<LinkModel, Link>();
            //RegionalContent
            Mapper.CreateMap<RegionalContent, RegionalContentModel>()
                  .ForMember(dest => dest.AvailableSites, mo => mo.Ignore())
                .ForMember(dest => dest.SelectedSiteIds, mo => mo.Ignore());
            Mapper.CreateMap<RegionalContentModel, RegionalContent>();

            //plugins
            Mapper.CreateMap<PluginDescriptor, PluginModel>()
                .ForMember(dest => dest.ConfigurationUrl, mo => mo.Ignore())
                .ForMember(dest => dest.SelectedSiteIds, mo => mo.Ignore())
                .ForMember(dest => dest.Locales, mo => mo.Ignore())
                .ForMember(dest => dest.IconUrl, mo => mo.Ignore())
                .ForMember(dest => dest.ConfigurationRoute, mo => mo.Ignore());
            //logs
            Mapper.CreateMap<Log, LogModel>()
                .ForMember(dest => dest.UserEmail, mo => mo.Ignore())
                .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                .ForMember(dest => dest.UpdatedOn, mo => mo.Ignore())
                .ForMember(dest => dest.LogLevelHint, mo => mo.Ignore());
            Mapper.CreateMap<LogModel, Log>()
                .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                .ForMember(dest => dest.UpdatedOnUtc, mo => mo.Ignore())
                .ForMember(dest => dest.LogLevelId, mo => mo.Ignore())
                .ForMember(dest => dest.User, mo => mo.Ignore());
            //ActivityLogType
            Mapper.CreateMap<ActivityLogTypeModel, ActivityLogType>()
                .ForMember(dest => dest.SystemKeyword, mo => mo.Ignore());
            Mapper.CreateMap<ActivityLogType, ActivityLogTypeModel>();
            Mapper.CreateMap<ActivityLog, ActivityLogModel>()
                .ForMember(dest => dest.ActivityLogTypeName, mo => mo.MapFrom(src => src.ActivityLogType.Name))
                .ForMember(dest => dest.UserEmail, mo => mo.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.CreatedOn, mo => mo.Ignore());
            //client
            Mapper.CreateMap<Client, ClientModel>()
                .ForMember(dest => dest.Locales, mo => mo.Ignore())
                .ForMember(dest => dest.SeName, mo => mo.MapFrom(src => src.GetSeName(0, true, false)))
                .ForMember(dest => dest.AvailableSites, mo => mo.Ignore())
                .ForMember(dest => dest.SelectedSiteIds, mo => mo.Ignore())
                .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                .ForMember(dest => dest.UpdatedOn, mo => mo.Ignore());
            Mapper.CreateMap<ClientModel, Client>()
                .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                .ForMember(dest => dest.ModifiedOnUtc, mo => mo.Ignore())
                .ForMember(dest => dest.Deleted, mo => mo.Ignore())
                .ForMember(dest => dest.Picture, mo => mo.Ignore());
            Mapper.CreateMap<ThemeSettings, ThemeListModel>()
                .ForMember(dest => dest.AvailableBundleOptimizationValues, mo => mo.Ignore())
                .ForMember(dest => dest.DesktopThemes, mo => mo.Ignore())
                .ForMember(dest => dest.MobileThemes, mo => mo.Ignore())
                .ForMember(dest => dest.SiteId, mo => mo.Ignore())
                .ForMember(dest => dest.AvailableSites, mo => mo.Ignore());
            Mapper.CreateMap<ThemeListModel, ThemeSettings>()
                .ForMember(dest => dest.EmulateMobileDevice, mo => mo.Ignore());

            //checkout attributes
            Mapper.CreateMap<ExtendedAttribute, ExtendedAttributeModel>()
                .ForMember(dest => dest.AttributeControlTypeName, mo => mo.Ignore())
                .ForMember(dest => dest.Locales, mo => mo.Ignore());
            Mapper.CreateMap<ExtendedAttributeModel, ExtendedAttribute>()
                .ForMember(dest => dest.AttributeControlType, mo => mo.Ignore())
                .ForMember(dest => dest.ExtendedAttributeValues, mo => mo.Ignore());
            Mapper.CreateMap<ExtendedAttributeValue, ExtendedAttributeValueModel>()
                .ForMember(dest => dest.Locales, mo => mo.Ignore());
            Mapper.CreateMap<ExtendedAttributeValueModel, ExtendedAttributeValue>()
                .ForMember(dest => dest.ExtendedAttribute, mo => mo.Ignore());
        }

        public int Order
        {
            get { return 0; }
        }
    }
}