
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Events;
using CAF.Mvc.JQuery.Datatables;
using CAF.Mvc.JQuery.Datatables.Core;
using CAF.WebSite.Application.WebUI.Mvc.Bundles;
using CAF.WebSite.Application.WebUI.Mvc.Routes;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Themes;
using CAF.Infrastructure.Core.Domain.Common;
using FluentValidation.Mvc;
using Metrics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.WebPages;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI.Plugins;
using CAF.WebSite.Application.Services.Tasks;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.WebSite.Application.WebUI.Theming;

namespace CAF.WebSite.Mvc
{
    public class MvcApplication : System.Web.HttpApplication
    {
        #region Methods
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            var eventPublisher = EngineContext.Current.Resolve<IEventPublisher>();
            eventPublisher.Publish(new AppRegisterGlobalFiltersEvent
            {
                Filters = filters
            });
        }
        public static void RegisterRoutes(RouteCollection routes, bool databaseInstalled = true)
        {
            routes.IgnoreRoute("favicon.ico");
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute(".db/{*virtualpath}");


            // register custom routes (plugins, etc)
            var routePublisher = EngineContext.Current.Resolve<IRoutePublisher>();
            routePublisher.RegisterRoutes(routes);

        }
        public static void RegisterBundles(BundleCollection bundles)
        {
            // register custom bundles
            var bundlePublisher = EngineContext.Current.Resolve<IBundlePublisher>();
            bundlePublisher.RegisterBundles(bundles);
        }
        protected void Application_Start()
        {
            // we use our own mobile devices support (".Mobile" is reserved). that's why we disable it.
            var mobileDisplayMode = DisplayModeProvider.Instance.Modes.FirstOrDefault(x => x.DisplayModeId == DisplayModeProvider.MobileDisplayModeId);
            if (mobileDisplayMode != null)
                DisplayModeProvider.Instance.Modes.Remove(mobileDisplayMode);

            // DisplayModeProvider.Instance.Modes.Add(new DefaultDisplayMode("iPhone") { ContextCondition = context => context.Request.UserAgent.Contains("iPhone") });

            bool installed = DataSettings.DatabaseIsInstalled();

            if (installed)
            {
                // remove all view engines
                ViewEngines.Engines.Clear();
            }

            // initialize engine context
            EngineContext.Initialize(false);

            // model binders
            ModelBinders.Binders.DefaultBinder = new DefinedModelBinder();

            // Add some functionality on top of the default ModelMetadataProvider
            ModelMetadataProviders.Current = new DefinedMetadataProvider();

            // Register MVC areas
            AreaRegistration.RegisterAllAreas();

            // fluent validation
            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
            ModelValidatorProviders.Providers.Add(new FluentValidationModelValidatorProvider(new ValidatorFactory()));

            // Routes
            RegisterRoutes(RouteTable.Routes, installed);

            // localize MVC resources
            ClientDataTypeModelValidatorProvider.ResourceClassKey = "MvcLocalization";
            DefaultModelBinder.ResourceClassKey = "MvcLocalization";
            ErrorMessageProvider.SetResourceClassKey("MvcLocalization");

            if (installed)
            {
                // register our themeable razor view engine we use
                ViewEngines.Engines.Add(new ThemeableRazorViewEngine());

                // Global filters
                RegisterGlobalFilters(GlobalFilters.Filters);

                // Bundles
                RegisterBundles(BundleTable.Bundles);

                // register virtual path provider for theming (file inheritance & variables handling)
                HostingEnvironment.RegisterVirtualPathProvider(new ThemingVirtualPathProvider(HostingEnvironment.VirtualPathProvider));
                BundleTable.VirtualPathProvider = HostingEnvironment.VirtualPathProvider;

                // register plugin debug view virtual path provider
                if (HttpContext.Current.IsDebuggingEnabled)
                {
                    HostingEnvironment.RegisterVirtualPathProvider(new PluginDebugViewVirtualPathProvider());
                }



                if (!ModelBinders.Binders.ContainsKey(typeof(DataTablesParam)))
                    ModelBinders.Binders.Add(typeof(DataTablesParam), new DataTablesModelBinder());
                //if (!ModelBinders.Binders.ContainsKey(typeof(DataTablesEditorRequest)))
                //    ModelBinders.Binders.Add(typeof(DataTablesEditorRequest), new DataTablesEditorModelBinder());

                // Install filter
                GlobalFilters.Filters.Add(new InitializeSchedulerFilter());
                //GlobalFilters.Filters.Add(new InitializeTablesFilter());
           
            
            }
            else
            {
                // app not installed

                // Install filter
                GlobalFilters.Filters.Add(new HandleInstallFilter());
            }



        }
        public override string GetVaryByCustomString(HttpContext context, string custom)
        {
            string result = string.Empty;

            if (DataSettings.DatabaseIsInstalled())
            {
                custom = custom.ToLower();

                switch (custom)
                {
                    case "theme":
                        result = EngineContext.Current.Resolve<IThemeContext>().CurrentTheme.ThemeName;
                        break;
                    case "site":
                        result = EngineContext.Current.Resolve<ISiteContext>().CurrentSite.Id.ToString();
                        break;
                    case "theme_site":
                        result = "{0}-{1}".FormatInvariant(
                            EngineContext.Current.Resolve<IThemeContext>().CurrentTheme.ThemeName,
                            EngineContext.Current.Resolve<ISiteContext>().CurrentSite.Id.ToString());
                        break;

                }
            }

            if (result.HasValue())
            {
                return result;
            }

            return base.GetVaryByCustomString(context, custom);
        }

        #endregion
    }
}
