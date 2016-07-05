using CAF.Infrastructure.Core.Plugins;
using CAF.WebSite.Application.Services.Authentication.External;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.QQAuth.Core;
using System.Web.Routing;
 

namespace CAF.WebSite.QQAuth
{
    /// <summary>
    /// Facebook externalAuth processor
    /// </summary>
    public class QQExternalAuthMethod : BasePlugin, IExternalAuthenticationMethod, IConfigurable
    {
        #region Fields

        private readonly QQExternalAuthSettings _facebookExternalAuthSettings;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public QQExternalAuthMethod(QQExternalAuthSettings facebookExternalAuthSettings, ILocalizationService localizationService)
        {
            this._facebookExternalAuthSettings = facebookExternalAuthSettings;
            _localizationService = localizationService;
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
			actionName = "Configure";
			controllerName = "ExternalAuthQQ";
            routeValues = new RouteValueDictionary(new { Namespaces = "CAF.WebSite.QQAuth.Controllers", area = Provider.SystemName });
        }

        /// <summary>
        /// Gets a route for displaying plugin in public store
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetPublicInfoRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PublicInfo";
            controllerName = "ExternalAuthQQ";
            routeValues = new RouteValueDictionary(new { Namespaces = "CAF.WebSite.QQAuth.Controllers", area = Provider.SystemName });
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //locales
            _localizationService.ImportPluginResourcesFromXml(this.PluginDescriptor);

            base.Install();
        }

        public override void Uninstall()
        {
            //locales
            _localizationService.DeleteLocaleStringResources(this.PluginDescriptor.ResourceRootKey);

            base.Uninstall();
        }

        #endregion
        
    }
}
