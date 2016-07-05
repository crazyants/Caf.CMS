using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
using System.Collections.Generic;
using System.Web.Mvc;


namespace CAF.WebSite.AliPay.Models
{
    public class ConfigurationModel : ModelBase
    {
        public string[] ConfigGroups { get; set; }

        [LangResourceDisplayName("Plugins.Payments.AliPay.SellerEmail")]
        public string SellerEmail { get; set; }

        [LangResourceDisplayName("Plugins.Payments.AliPay.Key")]
        public string Key { get; set; }

        [LangResourceDisplayName("Plugins.Payments.AliPay.Partner")]
        public string Partner { get; set; }

        [LangResourceDisplayName("Plugins.Payments.AliPay.AdditionalFee")]
        public decimal AdditionalFee { get; set; }



        public void Copy(AliPayPaymentSettings settings, bool fromSettings)
        {
            if (fromSettings)
            {
                SellerEmail = settings.SellerEmail;
                Key = settings.Key;
                Partner = settings.Partner;
                AdditionalFee = settings.AdditionalFee;

            }
            else
            {
                settings.SellerEmail = SellerEmail;
                settings.Key = Key;
                settings.Partner = Partner;
                settings.AdditionalFee = AdditionalFee;

            }

        }
    }
}
