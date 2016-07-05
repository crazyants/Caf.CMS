using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Exceptions;
using CAF.WebSite.AliPay.Models;
using CAF.WebSite.AliPay.Providers;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Configuration;
using CAF.Infrastructure.Core.Settings;
using CAF.WebSite.Application.Services;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Payments;
using CAF.WebSite.Application.Services.Sites;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.Infrastructure.Core.Domain.Cms.Payments;
using CAF.Infrastructure.Core.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.Mvc;


namespace CAF.WebSite.AliPay.Controllers
{
    public class PaymentAliPayController : PaymentControllerBase
    {
        private readonly ISettingService _settingService;
        private readonly IPaymentService _paymentService;
        //private readonly IOrderService _orderService;
        //private readonly IOrderProcessingService _orderProcessingService;
        private readonly ISiteContext _siteContext;
        private readonly ISiteService _siteService;
        private readonly IWorkContext _workContext;

        private readonly IWebHelper _webHelper;
        private readonly AliPayPaymentSettings _aliPayPaymentSettings;
        private readonly ILocalizationService _localizationService;
        private readonly ICommonServices _services;
        private readonly PaymentSettings _paymentSettings;

        public PaymentAliPayController(ISettingService settingService,
            IPaymentService paymentService,
            //IOrderService orderService, 
            //  IOrderProcessingService orderProcessingService, 
          IWebHelper webHelper,
            ISiteContext siteContext,
            IWorkContext workContext,
            AliPayPaymentSettings aliPayPaymentSettings,
               ILocalizationService localizationService,
            ICommonServices services,
            PaymentSettings paymentSettings,
             ISiteService siteService)
        {
            this._settingService = settingService;
            this._paymentService = paymentService;
            // this._orderService = orderService;
            // this._orderProcessingService = orderProcessingService;
            _siteContext = siteContext;
            _workContext = workContext;

            this._webHelper = webHelper;
            this._aliPayPaymentSettings = aliPayPaymentSettings;
            _localizationService = localizationService;
            _services = services;
            this._paymentSettings = paymentSettings;
            _siteService = siteService;
        }

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {

            var model = new ConfigurationModel();

            int siteScope = this.GetActiveSiteScopeConfiguration(_siteService, _services.WorkContext);
            var settings = _services.Settings.LoadSetting<AliPayPaymentSettings>(siteScope);

            model.Copy(settings, true);

            var siteDependingSettingHelper = new SiteDependingSettingHelper(ViewData);
            siteDependingSettingHelper.GetOverrideKeys(settings, model, siteScope, _services.Settings);

            return View(model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model, FormCollection form)
        {
            if (!ModelState.IsValid)
                return Configure();



            if (!ModelState.IsValid)
                return Configure();

            ModelState.Clear();

            var siteDependingSettingHelper = new SiteDependingSettingHelper(ViewData);
            int siteScope = this.GetActiveSiteScopeConfiguration(_siteService, _services.WorkContext);
            var settings = _services.Settings.LoadSetting<AliPayPaymentSettings>(siteScope);

            model.Copy(settings, false);

            siteDependingSettingHelper.UpdateSettings(settings, form, siteScope, _services.Settings);


            _services.Settings.SaveSetting(settings, 0);

            _services.Settings.ClearCache();

            NotifySuccess(_services.Localization.GetResource("Plugins.Payments.AliPay.ConfigSaveNote"));
            return Configure();
        }

        [ChildActionOnly]
        public ActionResult PaymentInfo()
        {
            var model = new PaymentInfoModel();
            return PartialView(model);

        }

        [NonAction]
        public override IList<string> ValidatePaymentForm(FormCollection form)
        {
            var warnings = new List<string>();
            return warnings;
        }

        [NonAction]
        public override ProcessPaymentRequest GetPaymentInfo(FormCollection form)
        {
            var paymentInfo = new ProcessPaymentRequest();
            return paymentInfo;
        }

        [ValidateInput(false)]
        public ActionResult Notify(FormCollection form)
        {
            var provider = _paymentService.LoadPaymentMethodBySystemName("Payments.AliPay", true);
            var processor = provider != null ? provider.Value as AliPayPaymentProvider : null;
            if (processor == null)
                throw new WorkException(_localizationService.GetResource("Plugins.Payments.AliPay.NoModuleLoading"));


            string alipayNotifyUrl = "https://www.alipay.com/cooperate/gateway.do?service=notify_verify";
            string partner = _aliPayPaymentSettings.Partner;
            if (string.IsNullOrEmpty(partner))
                throw new Exception("Partner is not set");
            string key = _aliPayPaymentSettings.Key;
            if (string.IsNullOrEmpty(key))
                throw new Exception("Partner is not set");
            string _input_charset = "utf-8";

            alipayNotifyUrl = alipayNotifyUrl + "&partner=" + partner + "&notify_id=" + Request.Form["notify_id"];
            string responseTxt = processor.Get_Http(alipayNotifyUrl, 120000);

            int i;
            NameValueCollection coll;
            coll = Request.Form;
            String[] requestarr = coll.AllKeys;
            string[] Sortedstr = processor.BubbleSort(requestarr);

            var prestr = new StringBuilder();
            for (i = 0; i < Sortedstr.Length; i++)
            {
                if (Request.Form[Sortedstr[i]] != "" && Sortedstr[i] != "sign" && Sortedstr[i] != "sign_type")
                {
                    if (i == Sortedstr.Length - 1)
                    {
                        prestr.Append(Sortedstr[i] + "=" + Request.Form[Sortedstr[i]]);
                    }
                    else
                    {
                        prestr.Append(Sortedstr[i] + "=" + Request.Form[Sortedstr[i]] + "&");

                    }
                }
            }

            prestr.Append(key);

            string mysign = processor.GetMD5(prestr.ToString(), _input_charset);

            string sign = Request.Form["sign"];

            if (mysign == sign && responseTxt == "true")
            {
                if (Request.Form["trade_status"] == "WAIT_BUYER_PAY")
                {
                    string strOrderNo = Request.Form["out_trade_no"];
                    string strPrice = Request.Form["total_fee"];
                }
                else if (Request.Form["trade_status"] == "TRADE_FINISHED" || Request.Form["trade_status"] == "TRADE_SUCCESS")
                {
                    string strOrderNo = Request.Form["out_trade_no"];
                    string strPrice = Request.Form["total_fee"];

                    int orderId = 0;
                    if (Int32.TryParse(strOrderNo, out orderId))
                    {
                        //更新订单状态
                        //var order = _orderService.GetOrderById(orderId);
                        //if (order != null && _orderProcessingService.CanMarkOrderAsPaid(order))
                        //{
                        //    _orderProcessingService.MarkOrderAsPaid(order);

                        //}
                    }
                }
                else
                {
                }

                Response.Write("success");
            }
            else
            {
                Response.Write("fail");
                string logStr = "MD5:mysign=" + mysign + ",sign=" + sign + ",responseTxt=" + responseTxt;
                Logger.Error(logStr);
            }

            return Content("");
        }

        [ValidateInput(false)]
        public ActionResult Return()
        {
            var provider = _paymentService.LoadPaymentMethodBySystemName("Payments.AliPay", true);
            var processor = provider != null ? provider.Value as AliPayPaymentProvider : null;
            if (processor == null || !processor.PluginDescriptor.Installed)
                throw new WorkException("AliPay module cannot be loaded");

            return RedirectToAction("Index", "Home", new { area = "" });
        }

        /// <summary>
        /// Gets a payment status
        /// </summary>
        /// <param name="paymentStatus">PayPal payment status</param>
        /// <param name="pendingReason">PayPal pending reason</param>
        /// <returns>Payment status</returns>
        public PaymentStatus GetPaymentStatus(string paymentStatus, string pendingReason)
        {
            var result = PaymentStatus.Pending;

            if (paymentStatus == null)
                paymentStatus = string.Empty;

            if (pendingReason == null)
                pendingReason = string.Empty;

            switch (paymentStatus.ToLowerInvariant())
            {
                case "pending":
                    switch (pendingReason.ToLowerInvariant())
                    {
                        case "authorization":
                            result = PaymentStatus.Authorized;
                            break;
                        default:
                            result = PaymentStatus.Pending;
                            break;
                    }
                    break;
                case "processed":
                case "completed":
                case "canceled_reversal":
                    result = PaymentStatus.Paid;
                    break;
                case "denied":
                case "expired":
                case "failed":
                case "voided":
                    result = PaymentStatus.Voided;
                    break;
                case "refunded":
                case "reversed":
                    result = PaymentStatus.Refunded;
                    break;
                default:
                    break;
            }
            return result;
        }

        public ActionResult CancelOrder(FormCollection form)
        {
            //var order = _orderService.SearchOrders(_siteContext.CurrentSite.Id, _workContext.CurrentUser.Id, null, null, null, null, null, null, null, null, 0, 1)
            //    .FirstOrDefault();

            //if (order != null)
            //{
            //    return RedirectToAction("Details", "Order", new { id = order.Id, area = "" });
            //}

            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}