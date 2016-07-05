
using CAF.Infrastructure.Core.Exceptions;
using CAF.Infrastructure.Core.Plugins;
using CAF.WebSite.Application.Services.Localization;
using CAF.Infrastructure.Core.Logging;
using System;
using System.ServiceModel;
using System.Web.Routing;


namespace CAF.WebSite.Sms
{
    /// <summary>
    /// Represents the Clickatell SMS provider
    /// </summary>
    public class SmsProvider : BasePlugin, IConfigurable
    {
        private readonly ILogger _logger;
        private readonly SmsSettings _clickatellSettings;
        private readonly ILocalizationService _localizationService;

        public SmsProvider(SmsSettings clickatellSettings,
            ILogger logger,
            ILocalizationService localizationService)
        {
            this._clickatellSettings = clickatellSettings;
            this._logger = logger;
            _localizationService = localizationService;
        }

        /// <summary>
        /// Sends SMS
        /// </summary>
        /// <param name="text">Text</param>
        public bool SendSms(string text)
        {
            try
            {
                //using (var svc = new PushServerWSPortTypeClient(new BasicHttpBinding(), new EndpointAddress("http://api.clickatell.com/soap/webservice_vs.php")))
                //{
                //    string authRsp = svc.auth(Int32.Parse(_clickatellSettings.ApiId), _clickatellSettings.Username, _clickatellSettings.Password);

                //    if (!authRsp.ToUpperInvariant().StartsWith("OK"))
                //    {
                //        throw new WorkException(authRsp);
                //    }

                //    string ssid = authRsp.Substring(4);
                //    string[] sndRsp = svc.sendmsg(ssid,
                //        Int32.Parse(_clickatellSettings.ApiId), _clickatellSettings.Username,
                //        _clickatellSettings.Password, new string[1] { _clickatellSettings.PhoneNumber },
                //        String.Empty, text, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                //        String.Empty, 0, String.Empty, String.Empty, String.Empty, 0);

                //    if (!sndRsp[0].ToUpperInvariant().StartsWith("ID"))
                //    {
                //        throw new WorkException(sndRsp[0]);
                //    }
                //    return true;
                //}
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            return false;
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "SmsController";
            routeValues = new RouteValueDictionary() { { "area", "CAF.WebSite.Sms" } };
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

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //locales
            _localizationService.DeleteLocaleStringResources(this.PluginDescriptor.ResourceRootKey);
            _localizationService.DeleteLocaleStringResources("Plugins.FriendlyName.Mobile.SMS", false);

            base.Uninstall();
        }
    }
}
