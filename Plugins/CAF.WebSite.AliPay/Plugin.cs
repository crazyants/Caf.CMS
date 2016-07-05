using System;
using System.Collections.Generic;
using System.Web.Routing;
using CAF.WebSite.AliPay.Controllers;
using CAF.Infrastructure.Core.Plugins;
using CAF.WebSite.Application.Services.Payments;
using CAF.WebSite.Application.Services;
using CAF.WebSite.Application.Services.Localization;



namespace CAF.WebSite.AliPay
{
    [DependentWidgets("Widgets.AliPay")]
    public class Plugin : BasePlugin
    {
        private readonly ILocalizationService _localizationService;
        //private readonly IAmazonPayService _apiService;
        //private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ICommonServices _services;

        public Plugin(
            //IAmazonPayService apiService,
            //IOrderTotalCalculationService orderTotalCalculationService,
                ILocalizationService localizationService,
            ICommonServices services)
        {
            //_apiService = apiService;
            //_orderTotalCalculationService = orderTotalCalculationService;
            _services = services;
            _localizationService = localizationService;
        }

        public override void Install()
        {
            _services.Settings.SaveSetting<AliPayPaymentSettings>(new AliPayPaymentSettings());

            _services.Localization.ImportPluginResourcesFromXml(this.PluginDescriptor);


            base.Install();
        }

        public override void Uninstall()
        {
            //_apiService.DataPollingTaskDelete();

            _services.Settings.DeleteSetting<AliPayPaymentSettings>();

            _services.Localization.DeleteLocaleStringResources(this.PluginDescriptor.ResourceRootKey);

            base.Uninstall();
        }


    }
}
