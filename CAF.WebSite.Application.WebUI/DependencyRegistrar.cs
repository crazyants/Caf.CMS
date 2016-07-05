using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Module = Autofac.Module;
using CAF.Infrastructure.Core.DependencyManagement;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Data.Hooks;
using CAF.Infrastructure.Core.Fakes;
using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core.Configuration;
using CAF.Infrastructure.Core.Localization;
using CAF.Infrastructure.Core.Plugins;
using CAF.Infrastructure.Data;
using CAF.WebSite.Application.Services.Logging;
using CAF.WebSite.Application.Services.Events;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.WebUI.Localization;
using CAF.Infrastructure.Core.IO.Media;
using CAF.Infrastructure.Core.IO.VirtualPath;
using CAF.Infrastructure.Core.IO.WebSite;
using CAF.WebSite.Application.WebUI.Themes;
using CAF.WebSite.Application.Services.Themes;
using CAF.WebSite.Application.Services.Configuration;
using CAF.Infrastructure.Core.Email;
using CAF.WebSite.Application.Services.Messages;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.Services.Common;
using CAF.WebSite.Application.Services.Helpers;
using CAF.WebSite.Application.WebUI.WebApi;
using CAF.WebSite.Application.WebUI.Controllers;
using System.Web.Mvc;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Configuration;
using CAF.Infrastructure.Core.Logging;
using CAF.Infrastructure.Core.Themes;
using CAF.WebSite.Application.Services.Sites;
using CAF.WebSite.Application.Services.Authentication;
using CAF.WebSite.Application.Services.Directory;
using CAF.WebSite.Application.Services.Users;
using CAF.WebSite.Application.Services.ExportImport;
using CAF.WebSite.Application.Services.Media;
using CAF.WebSite.Application.Services.Articles;
using CAF.WebSite.Application.Services.Seo;
using CAF.WebSite.Application.Services.Topics;
using CAF.WebSite.Application.Services.Links;
using CAF.WebSite.Application.WebUI.UI;
using CAF.WebSite.Application.WebUI.Mvc.Bundles;
using CAF.WebSite.Application.WebUI.Mvc.Routes;
using CAF.WebSite.Application.Services.Forums;
using CAF.WebSite.Application.Services;
using CAF.WebSite.Application.WebUI.Plugins;
using CAF.WebSite.Application.Services.Cms;
using CAF.Infrastructure.Core.Utilities;
using CAF.WebSite.Application.Services.Affiliates;
using CAF.WebSite.Application.Services.Clients;
using CAF.WebSite.Application.Services.Pdf;
using CAF.WebSite.Application.Services.Polls;
using CAF.WebSite.Application.Services.Tasks;
using CAF.WebSite.Application.Services.Tables;
using CAF.WebSite.Application.Services.Channels;
using CAF.WebSite.Application.Services.Authentication.External;
using CAF.WebSite.Application.Services.RegionalContents;
using CAF.Infrastructure.Core.Caching;
using CAF.Infrastructure.Search.Analyzers;
using CAF.WebSite.Application.Services.Searchs;
using CAF.WebSite.Application.Services.PushNotifications;
using CAF.Infrastructure.Core.PushNotifications;
using Microsoft.AspNet.SignalR;
using CAF.WebSite.Application.WebUI.Theming;
using CAF.WebSite.Application.Services.Feedbacks;

namespace CAF.WebSite.Application.WebUI
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, bool isActiveModule)
        {

            //开发环境下，使用Stub类
            // builder.RegisterAssemblyTypes(typeof(MvcApplication).Assembly).Where(
            //     t => t.Name.EndsWith("Repository") && t.Name.StartsWith("Stub")).AsImplementedInterfaces();
            //发布环境下，使用真实的数据访问层
            //builder.RegisterAssemblyTypes(typeof(MvcApplication).Assembly).Where(
            //   t => t.Name.EndsWith("Repository")).AsImplementedInterfaces();

            // plugins
            var pluginFinder = new PluginFinder();
            builder.RegisterInstance(pluginFinder).As<IPluginFinder>().SingleInstance();

            builder.RegisterType<PluginMediator>();
            // modules
            builder.RegisterModule(new DbModule(typeFinder));
            builder.RegisterModule(new EventModule(typeFinder, pluginFinder));
            builder.RegisterModule(new CachingModule());
            builder.RegisterModule(new LocalizationModule());
            builder.RegisterModule(new LoggingModule());
            builder.RegisterModule(new MessagingModule());
            builder.RegisterModule(new WebModule(typeFinder));
            builder.RegisterModule(new UiModule(typeFinder));
            builder.RegisterModule(new IOModule());
            builder.RegisterModule(new ProvidersModule(typeFinder, pluginFinder));
            builder.RegisterModule(new TasksModule(typeFinder));
            builder.RegisterModule(new GridsModule(typeFinder));
            builder.RegisterModule(new PushNotificationModule(typeFinder));
            // sources
            builder.RegisterSource(new SettingsSource());

            // web helper
            builder.RegisterType<WebHelper>().As<IWebHelper>().InstancePerRequest();

            // plugins
            builder.RegisterType<PluginFinder>().As<IPluginFinder>().SingleInstance(); // xxx (http)

            // work context
            builder.RegisterType<WebWorkContext>().As<IWorkContext>().WithStaticCache().InstancePerRequest();

            //// store context
            builder.RegisterType<WebSiteContext>().As<ISiteContext>().InstancePerRequest();

       
            #region CMS
            builder.RegisterType<RecentlyViewedArticlesService>().As<IRecentlyViewedArticlesService>().InstancePerRequest();
            builder.RegisterType<ArticleCategoryService>().As<IArticleCategoryService>().InstancePerRequest();
            builder.RegisterType<ArticleCategoryMappingService>().As<IArticleCategoryMappingService>().WithStaticCache().InstancePerRequest();
            builder.RegisterType<ArticleCategoryService>().Named<IArticleCategoryService>("nocache")
                .WithNullCache()
                .InstancePerRequest();
            builder.RegisterType<ModelTemplateService>().As<IModelTemplateService>().InstancePerRequest();
            builder.RegisterType<ArticleService>().As<IArticleService>().InstancePerRequest();
            builder.RegisterType<ArticleTagService>().As<IArticleTagService>().InstancePerRequest();
            builder.RegisterType<ClientService>().As<IClientService>().InstancePerRequest();
            builder.RegisterType<ArticleAttributeService>().As<IArticleAttributeService>().InstancePerRequest();

            builder.RegisterType<ExtendedAttributeFormatter>().As<IExtendedAttributeFormatter>().InstancePerRequest();
            builder.RegisterType<ExtendedAttributeParser>().As<IExtendedAttributeParser>().InstancePerRequest();
            builder.RegisterType<ExtendedAttributeService>().As<IExtendedAttributeService>().InstancePerRequest();

            builder.RegisterType<RegionalContentService>().As<IRegionalContentService>().InstancePerRequest();
            builder.RegisterType<FeedbackService>().As<IFeedbackService>().InstancePerRequest();
            
            #endregion

            builder.RegisterType<AffiliateService>().As<IAffiliateService>().InstancePerRequest();
            builder.RegisterType<AddressService>().As<IAddressService>().InstancePerRequest();
            builder.RegisterType<GenericAttributeService>().As<IGenericAttributeService>().InstancePerRequest();
            builder.RegisterType<FulltextService>().As<IFulltextService>().InstancePerRequest();
            builder.RegisterType<MaintenanceService>().As<IMaintenanceService>().InstancePerRequest();

            builder.RegisterType<ScheduleTaskService>().As<IScheduleTaskService>().InstancePerRequest();

            builder.RegisterType<SettingService>().As<ISettingService>().WithStaticCache().InstancePerRequest();

            builder.RegisterType<EncryptionService>().As<IEncryptionService>().InstancePerRequest();
            builder.RegisterType<FormsAuthenticationService>().As<IAuthenticationService>().InstancePerRequest();

            builder.RegisterType<AclService>().As<IAclService>().WithStaticCache().InstancePerRequest();
            builder.RegisterType<PermissionService>().As<IPermissionService>().WithStaticCache().InstancePerRequest();

            builder.RegisterType<DateTimeHelper>().As<IDateTimeHelper>().InstancePerRequest();
            builder.RegisterType<SitemapGenerator>().As<ISitemapGenerator>().InstancePerRequest();
            builder.RegisterType<PageAssetsBuilder>().As<IPageAssetsBuilder>().InstancePerRequest();

            builder.RegisterType<VisitRecordService>().As<IVisitRecordService>().InstancePerRequest();
            builder.RegisterType<SiteService>().As<ISiteService>().InstancePerRequest();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerRequest();
            builder.RegisterType<UserContentService>().As<IUserContentService>().InstancePerRequest();
            builder.RegisterType<UserReportService>().As<IUserReportService>().InstancePerRequest();
            builder.RegisterType<UserRegistrationService>().As<IUserRegistrationService>().InstancePerRequest();
            builder.RegisterType<SerialRuleService>().As<ISerialRuleService>().InstancePerRequest();

            builder.RegisterType<ImageCache>().As<IImageCache>().InstancePerRequest();
            builder.RegisterType<PictureService>().As<IPictureService>().InstancePerRequest();
            builder.RegisterType<DownloadService>().As<IDownloadService>().InstancePerRequest();
            builder.RegisterType<ImageResizerService>().As<IImageResizerService>().InstancePerRequest();
            builder.RegisterType<ImageResizerService>().As<IImageResizerService>().InstancePerRequest();
            builder.RegisterType<SiteMappingService>().As<ISiteMappingService>().WithStaticCache().InstancePerRequest();
            builder.RegisterType<UrlRecordService>().As<IUrlRecordService>().InstancePerRequest();

            builder.RegisterType<MeasureService>().As<IMeasureService>().WithStaticCache().InstancePerRequest();

            builder.RegisterType<GeoCountryLookup>().As<IGeoCountryLookup>().InstancePerRequest();
            builder.RegisterType<CountryService>().As<ICountryService>().WithStaticCache().InstancePerRequest();
            builder.RegisterType<DeliveryTimeService>().As<IDeliveryTimeService>().WithStaticCache().InstancePerRequest();

            builder.RegisterType<StateProvinceService>().As<IStateProvinceService>().WithStaticCache().InstancePerRequest();
            builder.RegisterType<TopicService>().As<ITopicService>().WithStaticCache().InstancePerRequest();
            builder.RegisterType<WidgetService>().As<IWidgetService>().InstancePerRequest();
            builder.RegisterType<LinkService>().As<ILinkService>().WithStaticCache().InstancePerRequest();
            builder.RegisterType<ChannelService>().As<IChannelService>().WithStaticCache().InstancePerRequest();

            builder.RegisterType<ForumService>().As<IForumService>().InstancePerRequest();
            builder.RegisterType<PollService>().As<IPollService>().InstancePerRequest();


            builder.RegisterType<ExportManager>().As<IExportManager>()
        .InstancePerRequest();

            builder.RegisterType<ExportManager>().As<IExportManager>().InstancePerRequest();

            builder.RegisterType<MobileDeviceHelper>().As<IMobileDeviceHelper>().InstancePerRequest();
            builder.RegisterType<UAParserUserAgent>().As<IUserAgent>().InstancePerRequest();
            builder.RegisterType<WkHtmlToPdfConverter>().As<IPdfConverter>().InstancePerRequest();

            builder.RegisterType<CommonServices>().As<ICommonServices>().WithStaticCache().InstancePerRequest();

            builder.RegisterType<ExternalAuthorizer>().As<IExternalAuthorizer>().InstancePerRequest();
            builder.RegisterType<OpenAuthenticationService>().As<IOpenAuthenticationService>().InstancePerRequest();

            builder.RegisterType<ArticleSearchProvider>().As<ISearchProvider>().InstancePerRequest();
            builder.RegisterType<CWSharpAnalyzer>().As<IAnalyzer>().InstancePerRequest();
        }

        public int Order
        {
            get { return -100; }
        }
    }

    #region Modules
    public class PushNotificationModule : Module
    {
        private readonly ITypeFinder _typeFinder;
        public PushNotificationModule(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }
        protected override void Load(ContainerBuilder builder)
        {
            var hubSignalR = GlobalHost.ConnectionManager.GetHubContext<ClientPushHub>();
            builder.Register<IPushNotificationManager>(c => new InMemoryPushNotificationManager(hubSignalR))
               .PropertiesAutowired(PropertyWiringOptions.None)
               .InstancePerRequest();
        }
    }
    public class GridsModule : Module
    {
        private readonly ITypeFinder _typeFinder;

        public GridsModule(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }

        protected override void Load(ContainerBuilder builder)
        {
            if (!DataSettings.DatabaseIsInstalled())
                return;


            var taskTypes = _typeFinder.FindClassesOfType<IGridRegistration>(ignoreInactivePlugins: true).ToList();

            foreach (var type in taskTypes)
            {
                var typeName = type.FullName;
                builder.RegisterType(type).Named<IGridRegistration>(typeName).Keyed<IGridRegistration>(type).InstancePerRequest();
                //builder.RegisterType(type).Named<IGridRegistration>(typeName).InstancePerRequest().PropertiesAutowired(PropertyWiringOptions.None);

            }

            // Register resolving delegate
            builder.Register<Func<Type, IGridRegistration>>(c =>
            {
                var cc = c.Resolve<IComponentContext>();
                return keyed => cc.ResolveKeyed<IGridRegistration>(keyed);
            });

            builder.Register<Func<string, IGridRegistration>>(c =>
            {
                var cc = c.Resolve<IComponentContext>();
                return named => cc.ResolveNamed<IGridRegistration>(named);
            });


        }

    }
    public class DbModule : Module
    {
        private readonly ITypeFinder _typeFinder;

        public DbModule(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => DataSettings.Current).As<DataSettings>().InstancePerDependency();
            builder.Register(x => new EfDataProviderFactory(x.Resolve<DataSettings>())).As<DataProviderFactory>().InstancePerDependency();

            builder.Register(x => x.Resolve<DataProviderFactory>().LoadDataProvider()).As<IDataProvider>().InstancePerDependency();
            builder.Register(x => (IEfDataProvider)x.Resolve<DataProviderFactory>().LoadDataProvider()).As<IEfDataProvider>().InstancePerDependency();

            if (DataSettings.Current.IsValid())
            {
                // register DB Hooks (only when app was installed properly)

                Func<Type, Type> findHookedType = (t) =>
                {
                    var x = t;
                    while (x != null)
                    {
                        if (x.IsGenericType)
                        {
                            return x.GetGenericArguments()[0];
                        }
                        x = x.BaseType;
                    }

                    return typeof(object);
                };

                var hooks = _typeFinder.FindClassesOfType<IHook>(ignoreInactivePlugins: true);
                foreach (var hook in hooks)
                {
                    var hookedType = findHookedType(hook);

                    var registration = builder.RegisterType(hook)
                        .As(typeof(IPreActionHook).IsAssignableFrom(hook) ? typeof(IPreActionHook) : typeof(IPostActionHook))
                        .InstancePerRequest();

                    registration.WithMetadata<HookMetadata>(m =>
                    {
                        m.For(em => em.HookedType, hookedType);
                    });
                }

                builder.Register<IDbContext>(c => new DefaultObjectContext(DataSettings.Current.DataConnectionString))
                    .PropertiesAutowired(PropertyWiringOptions.None)
                    .InstancePerRequest();
            }
            else
            {
                builder.Register<IDbContext>(c => new DefaultObjectContext(DataSettings.Current.DataConnectionString)).InstancePerRequest();
            }

            builder.Register<Func<string, IDbContext>>(c =>
            {
                var cc = c.Resolve<IComponentContext>();
                return named => cc.ResolveNamed<IDbContext>(named);
            });

            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).InstancePerRequest();

            builder.Register<DbQuerySettings>(c =>
            {
                var siteService = c.Resolve<ISiteService>();
                var aclService = c.Resolve<IAclService>();

                return new DbQuerySettings(!aclService.HasActiveAcl, siteService.IsSingleSiteMode());
            })
            .InstancePerRequest();

            //if (MongoDbDataSettings.Current.IsValid())
            //{

            //    var mongoDBDataProviderManager = new MongoDBDataProviderFactory(MongoDbDataSettings.Current);
            //    var dataProvider = mongoDBDataProviderManager.LoadDataProvider();
            //    builder.Register(c => new MongoClient(MongoDbDataSettings.Current.DataConnectionString)).As(typeof(IMongoClient)).SingleInstance();

            //}

            //builder.Register(c => MongoDbDataSettings.Current).As<MongoDbDataSettings>().InstancePerDependency();
            //builder.Register(x => new MongoDBDataProviderFactory(x.Resolve<MongoDbDataSettings>())).As<MongoDBProviderFactory>().InstancePerDependency();

            //builder.Register(x => x.Resolve<MongoDBProviderFactory>().LoadDataProvider()).As<IMongoDBProvider>().InstancePerDependency();
            //builder.Register(x => (IMongoDBProvider)x.Resolve<MongoDBProviderFactory>().LoadDataProvider()).As<IMongoDBProvider>().InstancePerDependency();
            //builder.RegisterGeneric(typeof(MongoDBRepository<>)).As(typeof(IMongoDBRepository<>)).InstancePerRequest();
        }

        protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
        {
            var querySettingsProperty = FindQuerySettingsProperty(registration.Activator.LimitType);

            if (querySettingsProperty == null)
                return;

            registration.Activated += (sender, e) =>
            {
                if (DataSettings.DatabaseIsInstalled())
                {
                    var querySettings = e.Context.Resolve<DbQuerySettings>();
                    querySettingsProperty.SetValue(e.Instance, querySettings, null);
                }
            };
        }

        private static PropertyInfo FindQuerySettingsProperty(Type type)
        {
            return type.GetProperty("QuerySettings", typeof(DbQuerySettings));
        }
    }

    public class CachingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<StaticCache>().As<ICache>().Keyed<ICache>(typeof(StaticCache)).SingleInstance();
            builder.RegisterType<AspNetCache>().As<ICache>().Keyed<ICache>(typeof(AspNetCache)).InstancePerRequest();
            builder.RegisterType<RequestCache>().As<ICache>().Keyed<ICache>(typeof(RequestCache)).InstancePerRequest();
            builder.RegisterType<RedisCacheManager>().As<ICache>().Keyed<ICache>(typeof(RedisCacheManager)).InstancePerRequest();
            // builder.RegisterType<EnyimMemcachedContext>().As<ICache>().Keyed<ICache>(typeof(EnyimMemcachedContext)).InstancePerRequest();

            //builder.RegisterType<CacheManager<EnyimMemcachedContext>>()
            //   .As<ICacheManager>()
            //   .Named<ICacheManager>("memcached")
            //   .SingleInstance();
            builder.RegisterType<CacheManager<RedisCacheManager>>()
               .As<ICacheManager>()
               .Named<ICacheManager>("redis")
               .SingleInstance();
            builder.RegisterType<CacheManager<StaticCache>>()
                .As<ICacheManager>()
                .Named<ICacheManager>("static")
                .SingleInstance();
            builder.RegisterType<CacheManager<AspNetCache>>()
                .As<ICacheManager>()
                .Named<ICacheManager>("aspnet")
                .InstancePerRequest();
            builder.RegisterType<CacheManager<RequestCache>>()
                .As<ICacheManager>()
                .Named<ICacheManager>("request")
                .InstancePerRequest();

            // Register resolving delegate
            builder.Register<Func<Type, ICache>>(c =>
            {
                var cc = c.Resolve<IComponentContext>();
                return keyed => cc.ResolveKeyed<ICache>(keyed);
            });

            builder.Register<Func<string, ICacheManager>>(c =>
            {
                var cc = c.Resolve<IComponentContext>();
                return named => cc.ResolveNamed<ICacheManager>(named);
            });

            builder.Register<Func<string, Lazy<ICacheManager>>>(c =>
            {
                var cc = c.Resolve<IComponentContext>();
                return named => cc.ResolveNamed<Lazy<ICacheManager>>(named);
            });
        }
    }

    public class LoggingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Notifier>().As<INotifier>().InstancePerRequest();
            builder.RegisterType<DefaultLogger>().As<ILogger>().InstancePerRequest();
            builder.RegisterType<UserActivityService>().As<IUserActivityService>().InstancePerRequest();
        }

        protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
        {
            if (!DataSettings.DatabaseIsInstalled())
                return;

            var implementationType = registration.Activator.LimitType;

            // build an array of actions on this type to assign loggers to member properties
            var injectors = BuildLoggerInjectors(implementationType).ToArray();

            // if there are no logger properties, there's no reason to hook the activated event
            if (!injectors.Any())
                return;

            // otherwise, whan an instance of this component is activated, inject the loggers on the instance
            registration.Activated += (s, e) =>
            {
                foreach (var injector in injectors)
                    injector(e.Context, e.Instance);
            };
        }

        private IEnumerable<Action<IComponentContext, object>> BuildLoggerInjectors(Type componentType)
        {
            if (DataSettings.DatabaseIsInstalled())
            {
                // Look for settable properties of type "ILogger" 
                var loggerProperties = componentType
                    .GetProperties(BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance)
                    .Select(p => new
                    {
                        PropertyInfo = p,
                        p.PropertyType,
                        IndexParameters = p.GetIndexParameters(),
                        Accessors = p.GetAccessors(false)
                    })
                    .Where(x => x.PropertyType == typeof(ILogger)) // must be a logger
                    .Where(x => x.IndexParameters.Count() == 0) // must not be an indexer
                    .Where(x => x.Accessors.Length != 1 || x.Accessors[0].ReturnType == typeof(void)); //must have get/set, or only set

                // Return an array of actions that resolve a logger and assign the property
                foreach (var entry in loggerProperties)
                {
                    var propertyInfo = entry.PropertyInfo;

                    yield return (ctx, instance) =>
                    {
                        string component = componentType.ToString();
                        var logger = ctx.Resolve<ILogger>();
                        propertyInfo.SetValue(instance, logger, null);
                    };
                }
            }
        }
    }

    public class LocalizationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LanguageService>().As<ILanguageService>().InstancePerRequest();

            builder.RegisterType<LocalizationService>().As<ILocalizationService>()
                .WithStaticCache() // pass StaticCache as ICache (cache settings between requests)
                .InstancePerRequest();

            builder.RegisterType<Text>().As<IText>().InstancePerRequest();

            builder.RegisterType<LocalizedEntityService>().As<ILocalizedEntityService>()
                .WithStaticCache() // pass StaticCache as ICache (cache settings between requests)
                .InstancePerRequest();
        }

        protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
        {
            var userProperty = FindUserProperty(registration.Activator.LimitType);

            if (userProperty == null)
                return;

            registration.Activated += (sender, e) =>
            {
                if (DataSettings.DatabaseIsInstalled())
                {
                    Localizer localizer = e.Context.Resolve<IText>().Get;
                    userProperty.SetValue(e.Instance, localizer, null);
                }
            };
        }

        private static PropertyInfo FindUserProperty(Type type)
        {
            return type.GetProperty("T", typeof(Localizer));
        }
    }

    public class EventModule : Module
    {
        private readonly ITypeFinder _typeFinder;
        private readonly IPluginFinder _pluginFinder;

        public EventModule(ITypeFinder typeFinder, IPluginFinder pluginFinder)
        {
            _typeFinder = typeFinder;
            _pluginFinder = pluginFinder;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EventPublisher>().As<IEventPublisher>().SingleInstance();
            builder.RegisterGeneric(typeof(DefaultConsumerFactory<>)).As(typeof(IConsumerFactory<>)).InstancePerDependency();

            // Register event consumers
            var consumerTypes = _typeFinder.FindClassesOfType(typeof(IConsumer<>));
            foreach (var consumerType in consumerTypes)
            {
                Type[] implementedInterfaces = consumerType.FindInterfaces(IsConsumerInterface, typeof(IConsumer<>));

                var registration = builder.RegisterType(consumerType).As(implementedInterfaces);

                var pluginDescriptor = _pluginFinder.GetPluginDescriptorByAssembly(consumerType.Assembly);
                var isActive = PluginManager.IsActivePluginAssembly(consumerType.Assembly);
                var shouldExecuteAsync = consumerType.GetAttribute<AsyncConsumerAttribute>(false) != null;

                registration.WithMetadata<EventConsumerMetadata>(m =>
                {
                    m.For(em => em.IsActive, isActive);
                    m.For(em => em.ExecuteAsync, shouldExecuteAsync);
                    m.For(em => em.PluginDescriptor, pluginDescriptor);
                });

                if (!shouldExecuteAsync)
                    registration.InstancePerRequest();

            }
        }

        private static bool IsConsumerInterface(Type type, object criteria)
        {
            var isMatch = type.IsGenericType && ((Type)criteria).IsAssignableFrom(type.GetGenericTypeDefinition());
            return isMatch;
        }
    }

    public class MessagingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {

            builder.RegisterType<MessageTemplateService>().As<IMessageTemplateService>().InstancePerRequest();
            builder.RegisterType<QueuedEmailService>().As<IQueuedEmailService>().InstancePerRequest();
            builder.RegisterType<NewsLetterSubscriptionService>().As<INewsLetterSubscriptionService>().InstancePerRequest();
            builder.RegisterType<CampaignService>().As<ICampaignService>().InstancePerRequest();
            builder.RegisterType<EmailAccountService>().As<IEmailAccountService>().InstancePerRequest();
            builder.RegisterType<WorkflowMessageService>().As<IWorkflowMessageService>().InstancePerRequest();
            builder.RegisterType<MessageTokenProvider>().As<IMessageTokenProvider>().InstancePerRequest();
            builder.RegisterType<Tokenizer>().As<ITokenizer>().InstancePerRequest();
            builder.RegisterType<DefaultEmailSender>().As<IEmailSender>().SingleInstance(); // xxx (http)
        }
    }

    public class WebModule : Module
    {
        private readonly ITypeFinder _typeFinder;

        public WebModule(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }

        //protected override void Load(ContainerBuilder builder)
        //{
        //    var foundAssemblies = _typeFinder.GetAssemblies().ToArray();

        //    builder.RegisterModule(new AutofacWebTypesModule());
        //    builder.Register(c =>
        //        //register FakeHttpContext when HttpContext is not available
        //        HttpContext.Current != null ?
        //        (new HttpContextWrapper(HttpContext.Current) as HttpContextBase) :
        //        (new FakeHttpContext("~/") as HttpContextBase))
        //        .As<HttpContextBase>()
        //        .InstancePerRequest();

        //    // register all controllers
        //    builder.RegisterControllers(foundAssemblies);

        //    //builder.RegisterType<EmbeddedViewResolver>().As<IEmbeddedViewResolver>().SingleInstance();
        //    builder.RegisterType<RoutePublisher>().As<IRoutePublisher>().SingleInstance();
        //    builder.RegisterType<BundlePublisher>().As<IBundlePublisher>().SingleInstance();
        //    builder.RegisterType<BundleBuilder>().As<IBundleBuilder>().InstancePerRequest();


        //}
        protected override void Load(ContainerBuilder builder)
        {
            var foundAssemblies = _typeFinder.GetAssemblies(ignoreInactivePlugins: true).ToArray();

            builder.RegisterModule(new AutofacWebTypesModule());
            builder.Register(HttpContextBaseFactory).As<HttpContextBase>();

            // register all controllers
            builder.RegisterControllers(foundAssemblies);

            builder.RegisterType<RoutePublisher>().As<IRoutePublisher>().SingleInstance();
            builder.RegisterType<BundlePublisher>().As<IBundlePublisher>().SingleInstance();
            builder.RegisterType<BundleBuilder>().As<IBundleBuilder>().InstancePerRequest();
            builder.RegisterType<FileDownloadManager>().InstancePerRequest();

            builder.RegisterFilterProvider();


            // global exception handling
            if (DataSettings.DatabaseIsInstalled())
            {
                builder.RegisterType<HandleExceptionFilter>().AsActionFilterFor<Controller>();
            }
        }

        static HttpContextBase HttpContextBaseFactory(IComponentContext ctx)
        {
            if (IsRequestValid())
            {
                return new HttpContextWrapper(HttpContext.Current);
            }

            // TODO: determine store url

            // register FakeHttpContext when HttpContext is not available
            return new FakeHttpContext("~/");
        }

        static bool IsRequestValid()
        {
            if (HttpContext.Current == null)
                return false;

            try
            {
                // The "Request" property throws at application startup on IIS integrated pipeline mode
                var req = HttpContext.Current.Request;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }


    }


    public class UiModule : Module
    {
        private readonly ITypeFinder _typeFinder;

        public UiModule(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }

        protected override void Load(ContainerBuilder builder)
        {
            // register theming services


            builder.Register<DefaultThemeRegistry>(x => new DefaultThemeRegistry(x.Resolve<IEventPublisher>(), null, null, true)).As<IThemeRegistry>().SingleInstance();
            builder.RegisterType<ThemeFileResolver>().As<IThemeFileResolver>().SingleInstance();

            builder.RegisterType<ThemeContext>().As<IThemeContext>().InstancePerRequest();
            builder.RegisterType<ThemeVariablesService>().As<IThemeVariablesService>().InstancePerRequest();

            // register UI component renderers
            builder.RegisterType<TabStripRenderer>().As<ComponentRenderer<TabStrip>>();
            builder.RegisterType<PagerRenderer>().As<ComponentRenderer<Pager>>();
            builder.RegisterType<WindowRenderer>().As<ComponentRenderer<Window>>();

            builder.RegisterType<WidgetProvider>().As<IWidgetProvider>().InstancePerRequest();
            builder.RegisterType<MenuPublisher>().As<IMenuPublisher>().InstancePerRequest();
        }
    }

    public class IOModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FileSystemStorageProvider>().As<IStorageProvider>().InstancePerRequest();
            builder.RegisterType<DefaultVirtualPathProvider>().As<IVirtualPathProvider>().InstancePerRequest();
            builder.RegisterType<WebSiteFolder>().As<IWebSiteFolder>().InstancePerRequest();
        }
    }

    public class ProvidersModule : Module
    {
        private readonly ITypeFinder _typeFinder;
        private readonly IPluginFinder _pluginFinder;

        public ProvidersModule(ITypeFinder typeFinder, IPluginFinder pluginFinder)
        {
            _typeFinder = typeFinder;
            _pluginFinder = pluginFinder;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ProviderManager>().As<IProviderManager>().InstancePerRequest();

            if (!DataSettings.DatabaseIsInstalled())
                return;

            var providerTypes = _typeFinder.FindClassesOfType<IProvider>(ignoreInactivePlugins: true).ToList();

            foreach (var type in providerTypes)
            {
                var pluginDescriptor = _pluginFinder.GetPluginDescriptorByAssembly(type.Assembly);
                var groupName = ProviderTypeToKnownGroupName(type);
                var systemName = GetSystemName(type, pluginDescriptor);
                var friendlyName = GetFriendlyName(type, pluginDescriptor);
                var displayOrder = GetDisplayOrder(type, pluginDescriptor);
                var dependentWidgets = GetDependentWidgets(type);
                var resPattern = (pluginDescriptor != null ? "Plugins" : "Providers") + ".{1}.{0}"; // e.g. Plugins.FriendlyName.MySystemName
                var settingPattern = (pluginDescriptor != null ? "Plugins" : "Providers") + ".{0}.{1}"; // e.g. Plugins.MySystemName.DisplayOrder
                var isConfigurable = typeof(IConfigurable).IsAssignableFrom(type);
                var isEditable = typeof(IUserEditable).IsAssignableFrom(type);

                var registration = builder.RegisterType(type).Named<IProvider>(systemName).InstancePerRequest().PropertiesAutowired(PropertyWiringOptions.None);
                registration.WithMetadata<ProviderMetadata>(m =>
                {
                    m.For(em => em.PluginDescriptor, pluginDescriptor);
                    m.For(em => em.GroupName, groupName);
                    m.For(em => em.SystemName, systemName);
                    m.For(em => em.ResourceKeyPattern, resPattern);
                    m.For(em => em.SettingKeyPattern, settingPattern);
                    m.For(em => em.FriendlyName, friendlyName.Item1);
                    m.For(em => em.Description, friendlyName.Item2);
                    m.For(em => em.DisplayOrder, displayOrder);
                    m.For(em => em.DependentWidgets, dependentWidgets);
                    m.For(em => em.IsConfigurable, isConfigurable);
                    m.For(em => em.IsEditable, isEditable);
                });

                // register specific provider type
                //  RegisterAsSpecificProvider<ITaxProvider>(type, systemName, registration);
                // RegisterAsSpecificProvider<IDiscountRequirementRule>(type, systemName, registration);
                RegisterAsSpecificProvider<IExchangeRateProvider>(type, systemName, registration);
                // RegisterAsSpecificProvider<IShippingRateComputationMethod>(type, systemName, registration);
                RegisterAsSpecificProvider<IWidget>(type, systemName, registration);
                RegisterAsSpecificProvider<IExternalAuthenticationMethod>(type, systemName, registration);
                //RegisterAsSpecificProvider<IPaymentMethod>(type, systemName, registration);
            }

        }

        #region Helpers

        private void RegisterAsSpecificProvider<T>(Type implType, string systemName, IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> registration) where T : IProvider
        {
            if (typeof(T).IsAssignableFrom(implType))
            {
                try
                {
                    registration.As<T>().Named<T>(systemName);
                    registration.WithMetadata<ProviderMetadata>(m =>
                    {
                        m.For(em => em.ProviderType, typeof(T));
                    });
                }
                catch (Exception) { }
            }
        }

        private string GetSystemName(Type type, PluginDescriptor descriptor)
        {
            var attr = type.GetAttribute<SystemNameAttribute>(false);
            if (attr != null)
            {
                return attr.Name;
            }

            if (typeof(IPlugin).IsAssignableFrom(type) && descriptor != null)
            {
                return descriptor.SystemName;
            }

            return type.FullName;
            //throw Error.Application("The 'SystemNameAttribute' must be applied to a provider type if the provider does not implement 'IPlugin' (provider type: {0}, plugin: {1})".FormatInvariant(type.FullName, descriptor != null ? descriptor.SystemName : "-"));
        }

        private int GetDisplayOrder(Type type, PluginDescriptor descriptor)
        {
            var attr = type.GetAttribute<DisplayOrderAttribute>(false);
            if (attr != null)
            {
                return attr.DisplayOrder;
            }

            if (typeof(IPlugin).IsAssignableFrom(type) && descriptor != null)
            {
                return descriptor.DisplayOrder;
            }

            return 0;
        }

        private Tuple<string/*Name*/, string/*Description*/> GetFriendlyName(Type type, PluginDescriptor descriptor)
        {
            string name = null;
            string description = name;

            var attr = type.GetAttribute<FriendlyNameAttribute>(false);
            if (attr != null)
            {
                name = attr.Name;
                description = attr.Description;
            }
            else if (typeof(IPlugin).IsAssignableFrom(type) && descriptor != null)
            {
                name = descriptor.FriendlyName;
                description = descriptor.Description;
            }
            else
            {
                name = Inflector.Titleize(type.Name);
                //throw Error.Application("The 'FriendlyNameAttribute' must be applied to a provider type if the provider does not implement 'IPlugin' (provider type: {0}, plugin: {1})".FormatInvariant(type.FullName, descriptor != null ? descriptor.SystemName : "-"));
            }

            return new Tuple<string, string>(name, description);
        }

        private string[] GetDependentWidgets(Type type)
        {
            if (!typeof(IWidget).IsAssignableFrom(type))
            {
                // don't let widgets depend on other widgets
                var attr = type.GetAttribute<DependentWidgetsAttribute>(false);
                if (attr != null)
                {
                    return attr.WidgetSystemNames;
                }
            }

            return new string[] { };
        }

        private string ProviderTypeToKnownGroupName(Type implType)
        {
            //if (typeof(ITaxProvider).IsAssignableFrom(implType))
            //{
            //    return "Tax";
            //}
            //else if (typeof(IDiscountRequirementRule).IsAssignableFrom(implType))
            //{
            //    return "Marketing";
            //}
            //else if (typeof(IExchangeRateProvider).IsAssignableFrom(implType))
            //{
            //    return "Payment";
            //}
            //else if (typeof(IShippingRateComputationMethod).IsAssignableFrom(implType))
            //{
            //    return "Shipping";
            //}
            //else if (typeof(IPaymentMethod).IsAssignableFrom(implType))
            //{
            //    return "Payment";
            //}
            if (typeof(IExternalAuthenticationMethod).IsAssignableFrom(implType))
            {
                return "Security";
            }
            else if (typeof(IWidget).IsAssignableFrom(implType))
            {
                return "CMS";
            }

            return null;
        }

        #endregion

    }

    public class TasksModule : Module
    {
        private readonly ITypeFinder _typeFinder;

        public TasksModule(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }

        protected override void Load(ContainerBuilder builder)
        {
            if (!DataSettings.DatabaseIsInstalled())
                return;

            builder.RegisterType<DefaultTaskScheduler>().As<ITaskScheduler>().SingleInstance();
            builder.RegisterType<TaskExecutor>().As<ITaskExecutor>().InstancePerRequest();

            var taskTypes = _typeFinder.FindClassesOfType<ITask>(ignoreInactivePlugins: true).ToList();

            foreach (var type in taskTypes)
            {
                var typeName = type.FullName;
                builder.RegisterType(type).Named<ITask>(typeName).Keyed<ITask>(type).InstancePerRequest();
            }

            // Register resolving delegate
            builder.Register<Func<Type, ITask>>(c =>
            {
                var cc = c.Resolve<IComponentContext>();
                return keyed => cc.ResolveKeyed<ITask>(keyed);
            });

            builder.Register<Func<string, ITask>>(c =>
            {
                var cc = c.Resolve<IComponentContext>();
                return named => cc.ResolveNamed<ITask>(named);
            });

        }

    }
    #endregion

    #region Sources

    public class SettingsSource : IRegistrationSource
    {
        static readonly MethodInfo BuildMethod = typeof(SettingsSource).GetMethod(
            "BuildRegistration",
            BindingFlags.Static | BindingFlags.NonPublic);

        public IEnumerable<IComponentRegistration> RegistrationsFor(
                Service service,
                Func<Service, IEnumerable<IComponentRegistration>> registrations)
        {
            var ts = service as TypedService;
            if (ts != null && typeof(ISettings).IsAssignableFrom(ts.ServiceType))
            {
                //var buildMethod = BuildMethod.MakeGenericMethod(ts.ServiceType);
                //yield return (IComponentRegistration)buildMethod.Invoke(null, null);

                // Perf with Fasterflect
                yield return (IComponentRegistration)Fasterflect.TryInvokeWithValuesExtensions.TryCallMethodWithValues(
                    typeof(SettingsSource),
                    null,
                    "BuildRegistration",
                    new Type[] { ts.ServiceType },
                    BindingFlags.Static | BindingFlags.NonPublic);
            }
        }

        static IComponentRegistration BuildRegistration<TSettings>() where TSettings : ISettings, new()
        {
            return RegistrationBuilder
                .ForDelegate((c, p) =>
                {
                    int currentSiteId = 0;
                    ISiteContext storeContext;
                    if (c.TryResolve<ISiteContext>(out storeContext))
                    {
                        var store = storeContext.CurrentSite;

                        currentSiteId = store.Id;
                        //uncomment the code below if you want load settings per store only when you have two stores installed.
                        //var currentSiteId = c.Resolve<ISiteService>().GetAllSites().Count > 1
                        //    c.Resolve<ISiteContext>().CurrentSite.Id : 0;

                        ////although it's better to connect to your database and execute the following SQL:
                        //DELETE FROM [Setting] WHERE [SiteId] > 0

                        return c.Resolve<ISettingService>().LoadSetting<TSettings>(currentSiteId);
                    }

                    // Unit tests
                    return new TSettings();
                })
                .InstancePerRequest()
                .CreateRegistration();
        }

        public bool IsAdapterForIndividualComponents { get { return false; } }
    }

    #endregion

}
