using AutoMapper;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Cms.Media;
using CAF.Infrastructure.Core.Domain.Cms.Topic;
using CAF.Infrastructure.Core.Domain.Sites;
using CAF.Infrastructure.Core.Domain.Localization;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.WebSite.Mvc.Admin.Models.Articles;
using CAF.WebSite.Mvc.Admin.Models.Channels;
using CAF.WebSite.Mvc.Admin.Models.Sites;
using CAF.WebSite.Mvc.Admin.Models.Localization;
using CAF.WebSite.Mvc.Admin.Models.Links;
using CAF.WebSite.Mvc.Admin.Models.Topics;
using CAF.WebSite.Mvc.Admin.Models.Users;
using CAF.Infrastructure.Core.Plugins;
using CAF.WebSite.Mvc.Admin.Models.Plugins;
using CAF.Infrastructure.Core.Domain.Logging;
using CAF.WebSite.Mvc.Admin.Models.Logging;
using CAF.WebSite.Mvc.Admin.Models.Messages;
using CAF.Infrastructure.Core.Domain.Messages;
using CAF.Infrastructure.Core.Email;
using CAF.WebSite.Mvc.Admin.Models.Settings;
using CAF.Infrastructure.Core.Domain.Cms;
using CAF.WebSite.Mvc.Admin.Models.Common;
using CAF.Infrastructure.Core.Domain.Common;
using CAF.Infrastructure.Core.Domain.Directory;
using CAF.WebSite.Mvc.Admin.Models.Directory;
using CAF.WebSite.Mvc.Admin.Models.Discounts;
using CAF.WebSite.Mvc.Admin.Models.Clients;
using CAF.Infrastructure.Core.Domain.Cms.Clients;
using CAF.Infrastructure.Core.Domain.Cms.Forums;
using CAF.WebSite.Mvc.Admin.Models.Polls;
using CAF.Infrastructure.Core.Domain.Cms.Polls;
using CAF.Infrastructure.Core.Domain.Themes;
using CAF.WebSite.Mvc.Admin.Models.Themes;
using CAF.WebSite.Mvc.Admin.Models.ExtendedAttributes;
using CAF.Infrastructure.Core.Domain.Cms.Channels;
using CAF.WebSite.Mvc.Admin.Models.ModelTemplates;
using CAF.WebSite.Mvc.Admin.Models.RegionalContents;
using CAF.Infrastructure.Core.Domain.Cms.RegionalContents;
using CAF.WebFeedbacks.Mvc.Admin.Models.Feedbacks;


namespace CAF.WebSite.Mvc.Admin
{
    public static class MappingExtensions
    {
        #region Client

        public static ClientModel ToModel(this Client entity)
        {
            return Mapper.Map<Client, ClientModel>(entity);
        }

        public static Client ToEntity(this ClientModel model)
        {
            return Mapper.Map<ClientModel, Client>(model);
        }

        public static Client ToEntity(this ClientModel model, Client destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Category
        public static ArticleCategory ToEntity(this ArticleCategoryModel model)
        {
            return Mapper.Map<ArticleCategoryModel, ArticleCategory>(model);
        }
        public static ArticleCategory ToEntity(this ArticleCategoryModel model, ArticleCategory destination)
        {
            return Mapper.Map(model, destination);
        }
        public static ArticleCategoryModel ToModel(this ArticleCategory entity)
        {
            return Mapper.Map<ArticleCategory, ArticleCategoryModel>(entity);
        }
        #endregion

        #region Article
        public static Article ToEntity(this ArticleModel model)
        {
            return Mapper.Map<ArticleModel, Article>(model);
        }
        public static Article ToEntity(this ArticleModel model, Article destination)
        {
            return Mapper.Map(model, destination);
        }
        public static ArticleModel ToModel(this Article entity)
        {
            return Mapper.Map<Article, ArticleModel>(entity);
        }
        #endregion

        #region ChannelModel
          public static ChannelModel ToModel(this Channel entity)
        {
            return Mapper.Map<Channel, ChannelModel>(entity);
        }

        public static Channel ToEntity(this ChannelModel model)
        {
            return Mapper.Map<ChannelModel, Channel>(model);
        }

        public static Channel ToEntity(this ChannelModel model, Channel destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Polls

        //news items
        public static PollModel ToModel(this Poll entity)
        {
            return Mapper.Map<Poll, PollModel>(entity);
        }

        public static Poll ToEntity(this PollModel model)
        {
            return Mapper.Map<PollModel, Poll>(model);
        }

        public static Poll ToEntity(this PollModel model, Poll destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Languages

        public static LanguageModel ToModel(this Language entity)
        {
            return Mapper.Map<Language, LanguageModel>(entity);
        }

        public static Language ToEntity(this LanguageModel model)
        {
            return Mapper.Map<LanguageModel, Language>(model);
        }

        public static Language ToEntity(this LanguageModel model, Language destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Countries / states

        public static CountryModel ToModel(this Country entity)
        {
            return Mapper.Map<Country, CountryModel>(entity);
        }

        public static Country ToEntity(this CountryModel model)
        {
            return Mapper.Map<CountryModel, Country>(model);
        }

        public static Country ToEntity(this CountryModel model, Country destination)
        {
            return Mapper.Map(model, destination);
        }

        public static StateProvinceModel ToModel(this StateProvince entity)
        {
            return Mapper.Map<StateProvince, StateProvinceModel>(entity);
        }

        public static StateProvince ToEntity(this StateProvinceModel model)
        {
            return Mapper.Map<StateProvinceModel, StateProvince>(model);
        }

        public static StateProvince ToEntity(this StateProvinceModel model, StateProvince destination)
        {
            return Mapper.Map(model, destination);
        }


        #endregion

        #region Delivery Times

        public static DeliveryTimeModel ToModel(this DeliveryTime entity)
        {
            return Mapper.Map<DeliveryTime, DeliveryTimeModel>(entity);
        }

        public static DeliveryTime ToEntity(this DeliveryTimeModel model)
        {
            return Mapper.Map<DeliveryTimeModel, DeliveryTime>(model);
        }

        public static DeliveryTime ToEntity(this DeliveryTimeModel model, DeliveryTime destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Measure weights

        public static MeasureWeightModel ToModel(this MeasureWeight entity)
        {
            return Mapper.Map<MeasureWeight, MeasureWeightModel>(entity);
        }

        public static MeasureWeight ToEntity(this MeasureWeightModel model)
        {
            return Mapper.Map<MeasureWeightModel, MeasureWeight>(model);
        }

        public static MeasureWeight ToEntity(this MeasureWeightModel model, MeasureWeight destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Measure dimension

        public static MeasureDimensionModel ToModel(this MeasureDimension entity)
        {
            return Mapper.Map<MeasureDimension, MeasureDimensionModel>(entity);
        }

        public static MeasureDimension ToEntity(this MeasureDimensionModel model)
        {
            return Mapper.Map<MeasureDimensionModel, MeasureDimension>(model);
        }

        public static MeasureDimension ToEntity(this MeasureDimensionModel model, MeasureDimension destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Quantity units

        public static QuantityUnitModel ToModel(this QuantityUnit entity)
        {
            return Mapper.Map<QuantityUnit, QuantityUnitModel>(entity);
        }

        public static QuantityUnit ToEntity(this QuantityUnitModel model)
        {
            return Mapper.Map<QuantityUnitModel, QuantityUnit>(model);
        }

        public static QuantityUnit ToEntity(this QuantityUnitModel model, QuantityUnit destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Discounts

        //public static DiscountModel ToModel(this Discount entity)
        //{
        //    return Mapper.Map<Discount, DiscountModel>(entity);
        //}

        //public static Discount ToEntity(this DiscountModel model)
        //{
        //    return Mapper.Map<DiscountModel, Discount>(model);
        //}

        //public static Discount ToEntity(this DiscountModel model, Discount destination)
        //{
        //    return Mapper.Map(model, destination);
        //}

        #endregion

        #region Email account

        public static EmailAccountModel ToModel(this EmailAccount entity)
        {
            return Mapper.Map<EmailAccount, EmailAccountModel>(entity);
        }

        public static EmailAccount ToEntity(this EmailAccountModel model)
        {
            return Mapper.Map<EmailAccountModel, EmailAccount>(model);
        }

        public static EmailAccount ToEntity(this EmailAccountModel model, EmailAccount destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Message templates

        public static MessageTemplateModel ToModel(this MessageTemplate entity)
        {
            return Mapper.Map<MessageTemplate, MessageTemplateModel>(entity);
        }

        public static MessageTemplate ToEntity(this MessageTemplateModel model)
        {
            return Mapper.Map<MessageTemplateModel, MessageTemplate>(model);
        }

        public static MessageTemplate ToEntity(this MessageTemplateModel model, MessageTemplate destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Queued email

        public static QueuedEmailModel ToModel(this QueuedEmail entity)
        {
            return Mapper.Map<QueuedEmail, QueuedEmailModel>(entity);
        }

        public static QueuedEmail ToEntity(this QueuedEmailModel model)
        {
            return Mapper.Map<QueuedEmailModel, QueuedEmail>(model);
        }

        public static QueuedEmail ToEntity(this QueuedEmailModel model, QueuedEmail destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Campaigns

        public static CampaignModel ToModel(this Campaign entity)
        {
            return Mapper.Map<Campaign, CampaignModel>(entity);
        }

        public static Campaign ToEntity(this CampaignModel model)
        {
            return Mapper.Map<CampaignModel, Campaign>(model);
        }

        public static Campaign ToEntity(this CampaignModel model, Campaign destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Settings

        public static ContentSliderSettingsModel ToModel(this ContentSliderSettings entity)
        {
            return Mapper.Map<ContentSliderSettings, ContentSliderSettingsModel>(entity);
        }
        public static ContentSliderSettings ToEntity(this ContentSliderSettingsModel model)
        {
            return Mapper.Map<ContentSliderSettingsModel, ContentSliderSettings>(model);
        }
        public static ContentSliderSettings ToEntity(this ContentSliderSettingsModel model, ContentSliderSettings destination)
        {
            return Mapper.Map(model, destination);
        }

        public static ContentSliderSlideModel ToModel(this ContentSliderSlideSettings entity)
        {
            return Mapper.Map<ContentSliderSlideSettings, ContentSliderSlideModel>(entity);
        }
        public static ContentSliderSlideSettings ToEntity(this ContentSliderSlideModel model)
        {
            return Mapper.Map<ContentSliderSlideModel, ContentSliderSlideSettings>(model);
        }
        public static ContentSliderSlideSettings ToEntity(this ContentSliderSlideModel model, ContentSliderSlideSettings destination)
        {
            return Mapper.Map(model, destination);
        }


        public static MediaSettingsModel ToModel(this MediaSettings entity)
        {
            return Mapper.Map<MediaSettings, MediaSettingsModel>(entity);
        }
        public static MediaSettings ToEntity(this MediaSettingsModel model)
        {
            return Mapper.Map<MediaSettingsModel, MediaSettings>(model);
        }
        public static MediaSettings ToEntity(this MediaSettingsModel model, MediaSettings destination)
        {
            return Mapper.Map(model, destination);
        }

        //customer/user settings
        public static GeneralUserSettingsModel.UserSettingsModel ToModel(this UserSettings entity)
        {
            return Mapper.Map<UserSettings, GeneralUserSettingsModel.UserSettingsModel>(entity);
        }
        public static UserSettings ToEntity(this GeneralUserSettingsModel.UserSettingsModel model)
        {
            return Mapper.Map<GeneralUserSettingsModel.UserSettingsModel, UserSettings>(model);
        }
        public static UserSettings ToEntity(this GeneralUserSettingsModel.UserSettingsModel model, UserSettings destination)
        {
            return Mapper.Map(model, destination);
        }
        public static GeneralUserSettingsModel.AddressSettingsModel ToModel(this AddressSettings entity)
        {
            return Mapper.Map<AddressSettings, GeneralUserSettingsModel.AddressSettingsModel>(entity);
        }
        public static AddressSettings ToEntity(this GeneralUserSettingsModel.AddressSettingsModel model)
        {
            return Mapper.Map<GeneralUserSettingsModel.AddressSettingsModel, AddressSettings>(model);
        }
        public static AddressSettings ToEntity(this GeneralUserSettingsModel.AddressSettingsModel model, AddressSettings destination)
        {
            return Mapper.Map(model, destination);
        }

        public static ForumSettingsModel ToModel(this ForumSettings entity)
        {
            return Mapper.Map<ForumSettings, ForumSettingsModel>(entity);
        }
        public static ForumSettings ToEntity(this ForumSettingsModel model)
        {
            return Mapper.Map<ForumSettingsModel, ForumSettings>(model);
        }
        public static ForumSettings ToEntity(this ForumSettingsModel model, ForumSettings destination)
        {
            return Mapper.Map(model, destination);
        }

        public static ArticleCatalogSettingsModel ToModel(this ArticleCatalogSettings entity)
        {
            return Mapper.Map<ArticleCatalogSettings, ArticleCatalogSettingsModel>(entity);
        }
        public static ArticleCatalogSettings ToEntity(this ArticleCatalogSettingsModel model)
        {
            return Mapper.Map<ArticleCatalogSettingsModel, ArticleCatalogSettings>(model);
        }
        public static ArticleCatalogSettings ToEntity(this ArticleCatalogSettingsModel model, ArticleCatalogSettings destination)
        {
            return Mapper.Map(model, destination);
        }
        #endregion

        #region Sites

        public static SiteModel ToModel(this Site entity)
        {
            return Mapper.Map<Site, SiteModel>(entity);
        }

        public static Site ToEntity(this SiteModel model)
        {
            return Mapper.Map<SiteModel, Site>(model);
        }

        public static Site ToEntity(this SiteModel model, Site destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region ModelTemplates

        public static ModelTemplateModel ToModel(this ModelTemplate entity)
        {
            return Mapper.Map<ModelTemplate, ModelTemplateModel>(entity);
        }

        public static ModelTemplate ToEntity(this ModelTemplateModel model)
        {
            return Mapper.Map<ModelTemplateModel, ModelTemplate>(model);
        }

        public static ModelTemplate ToEntity(this ModelTemplateModel model, ModelTemplate destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Users/users/users roles
        //customer roles
        public static UserRoleModel ToModel(this UserRole entity)
        {
            return Mapper.Map<UserRole, UserRoleModel>(entity);
        }

        public static UserRole ToEntity(this UserRoleModel model)
        {
            return Mapper.Map<UserRoleModel, UserRole>(model);
        }

        public static UserRole ToEntity(this UserRoleModel model, UserRole destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Address

        public static AddressModel ToModel(this Address entity)
        {
            return Mapper.Map<Address, AddressModel>(entity);
        }

        public static Address ToEntity(this AddressModel model)
        {
            return Mapper.Map<AddressModel, Address>(model);
        }

        public static Address ToEntity(this AddressModel model, Address destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Topics

        public static TopicModel ToModel(this Topic entity)
        {
            return Mapper.Map<Topic, TopicModel>(entity);
        }

        public static Topic ToEntity(this TopicModel model)
        {
            return Mapper.Map<TopicModel, Topic>(model);
        }

        public static Topic ToEntity(this TopicModel model, Topic destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Links

        public static LinkModel ToModel(this Link entity)
        {
            return Mapper.Map<Link, LinkModel>(entity);
        }

        public static Link ToEntity(this LinkModel model)
        {
            return Mapper.Map<LinkModel, Link>(model);
        }

        public static Link ToEntity(this LinkModel model, Link destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Feedbacks

        public static FeedbackModel ToModel(this Feedback entity)
        {
            return Mapper.Map<Feedback, FeedbackModel>(entity);
        }

        public static Feedback ToEntity(this FeedbackModel model)
        {
            return Mapper.Map<FeedbackModel, Feedback>(model);
        }

        public static Feedback ToEntity(this FeedbackModel model, Feedback destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region RegionalContents

        public static RegionalContentModel ToModel(this RegionalContent entity)
        {
            return Mapper.Map<RegionalContent, RegionalContentModel>(entity);
        }

        public static RegionalContent ToEntity(this RegionalContentModel model)
        {
            return Mapper.Map<RegionalContentModel, RegionalContent>(model);
        }

        public static RegionalContent ToEntity(this RegionalContentModel model, RegionalContent destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Plugins

        public static PluginModel ToModel(this PluginDescriptor entity)
        {
            return Mapper.Map<PluginDescriptor, PluginModel>(entity);
        }

        #endregion

        #region Log

        public static LogModel ToModel(this Log entity)
        {
            return Mapper.Map<Log, LogModel>(entity);
        }

        public static Log ToEntity(this LogModel model)
        {
            return Mapper.Map<LogModel, Log>(model);
        }

        public static Log ToEntity(this LogModel model, Log destination)
        {
            return Mapper.Map(model, destination);
        }

        public static ActivityLogTypeModel ToModel(this ActivityLogType entity)
        {
            return Mapper.Map<ActivityLogType, ActivityLogTypeModel>(entity);
        }

        public static ActivityLogModel ToModel(this ActivityLog entity)
        {
            return Mapper.Map<ActivityLog, ActivityLogModel>(entity);
        }

        #endregion

        #region Settings
        public static ThemeListModel ToModel(this ThemeSettings entity)
        {
            return Mapper.Map<ThemeSettings, ThemeListModel>(entity);
        }
        public static ThemeSettings ToEntity(this ThemeListModel model)
        {
            return Mapper.Map<ThemeListModel, ThemeSettings>(model);
        }
        public static ThemeSettings ToEntity(this ThemeListModel model, ThemeSettings destination)
        {
            return Mapper.Map(model, destination);
        }
        #endregion

        #region Extended attributes

        //attributes
        public static ExtendedAttributeModel ToModel(this ExtendedAttribute entity)
        {
            return Mapper.Map<ExtendedAttribute, ExtendedAttributeModel>(entity);
        }

        public static ExtendedAttribute ToEntity(this ExtendedAttributeModel model)
        {
            return Mapper.Map<ExtendedAttributeModel, ExtendedAttribute>(model);
        }

        public static ExtendedAttribute ToEntity(this ExtendedAttributeModel model, ExtendedAttribute destination)
        {
            return Mapper.Map(model, destination);
        }

        //checkout attribute values
        public static ExtendedAttributeValueModel ToModel(this ExtendedAttributeValue entity)
        {
            return Mapper.Map<ExtendedAttributeValue, ExtendedAttributeValueModel>(entity);
        }

        public static ExtendedAttributeValue ToEntity(this ExtendedAttributeValueModel model)
        {
            return Mapper.Map<ExtendedAttributeValueModel, ExtendedAttributeValue>(model);
        }

        public static ExtendedAttributeValue ToEntity(this ExtendedAttributeValueModel model, ExtendedAttributeValue destination)
        {
            return Mapper.Map(model, destination);
        }
        #endregion
    }
}