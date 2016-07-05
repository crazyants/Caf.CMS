using CAF.Infrastructure.Core.Plugins;
using CAF.WebSite.AliPay;
using CAF.WebSite.Application.Services;
using CAF.WebSite.Application.Services.Directory;
using CAF.WebSite.Application.Services.Payments;
using CAF.Infrastructure.Core.Domain.Cms.Payments;
using CAF.Infrastructure.Core.Domain.Directory;
using CAF.Infrastructure.Core.Logging;
using CAF.Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Routing;
using System.Security.Cryptography;
using CAF.Infrastructure.Core.Domain.Cms.Orders;
using CAF.WebSite.AliPay.Controllers;
using CAF.WebSite.Application.WebUI;


namespace CAF.WebSite.AliPay.Providers
{
    /// <summary>
    /// PayPalStandard provider
    /// </summary>
    [SystemName("Payments.AliPay")]
    [FriendlyName("PayPal Standard")]
    [DisplayOrder(2)]
    public partial class AliPayPaymentProvider : PaymentPluginBase, IConfigurable
    {
        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;
        //private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly AliPayPaymentSettings _aliPayPaymentSettings;
        private readonly HttpContextBase _httpContext;
        private readonly ICommonServices _commonServices;
        private readonly ILogger _logger;

        public AliPayPaymentProvider(ICurrencyService currencyService,
            AliPayPaymentSettings aliPayPaymentSettings,
            HttpContextBase httpContext,
            CurrencySettings currencySettings,
            //IOrderTotalCalculationService orderTotalCalculationService,
            ICommonServices commonServices,
            ILogger logger)
        {
            _currencyService = currencyService;
            _currencySettings = currencySettings;
            //_orderTotalCalculationService = orderTotalCalculationService;
            this._aliPayPaymentSettings = aliPayPaymentSettings;
            _httpContext = httpContext;
            _commonServices = commonServices;
            _logger = logger;
        }

        #region Utilities

        /// <summary>
        /// Gets MD5 hash
        /// </summary>
        /// <param name="Input">Input</param>
        /// <param name="Input_charset">Input charset</param>
        /// <returns>Result</returns>
        public string GetMD5(string Input, string Input_charset)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(Encoding.GetEncoding(Input_charset).GetBytes(Input));
            StringBuilder sb = new StringBuilder(32);
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Bubble sort
        /// </summary>
        /// <param name="Input">Input</param>
        /// <returns>Result</returns>
        public string[] BubbleSort(string[] Input)
        {
            int i, j;
            string temp;

            bool exchange;

            for (i = 0; i < Input.Length; i++)
            {
                exchange = false;

                for (j = Input.Length - 2; j >= i; j--)
                {
                    if (System.String.CompareOrdinal(Input[j + 1], Input[j]) < 0)
                    {
                        temp = Input[j + 1];
                        Input[j + 1] = Input[j];
                        Input[j] = temp;

                        exchange = true;
                    }
                }

                if (!exchange)
                {
                    break;
                }
            }
            return Input;
        }

        /// <summary>
        /// Create URL
        /// </summary>
        /// <param name="Para">Para</param>
        /// <param name="InputCharset">Input charset</param>
        /// <param name="Key">Key</param>
        /// <returns>Result</returns>
        public string CreatUrl(string[] Para, string InputCharset, string Key)
        {
            int i;
            string[] Sortedstr = BubbleSort(Para);
            StringBuilder prestr = new StringBuilder();

            for (i = 0; i < Sortedstr.Length; i++)
            {
                if (i == Sortedstr.Length - 1)
                {
                    prestr.Append(Sortedstr[i]);

                }
                else
                {
                    prestr.Append(Sortedstr[i] + "&");
                }

            }

            prestr.Append(Key);
            string sign = GetMD5(prestr.ToString(), InputCharset);
            return sign;
        }

        /// <summary>
        /// Gets HTTP
        /// </summary>
        /// <param name="StrUrl">Url</param>
        /// <param name="Timeout">Timeout</param>
        /// <returns>Result</returns>
        public string Get_Http(string StrUrl, int Timeout)
        {
            string strResult = string.Empty;
            try
            {
                HttpWebRequest myReq = (HttpWebRequest)HttpWebRequest.Create(StrUrl);
                myReq.Timeout = Timeout;
                HttpWebResponse HttpWResp = (HttpWebResponse)myReq.GetResponse();
                Stream myStream = HttpWResp.GetResponseStream();
                StreamReader sr = new StreamReader(myStream, Encoding.Default);
                StringBuilder strBuilder = new StringBuilder();
                while (-1 != sr.Peek())
                {
                    strBuilder.Append(sr.ReadLine());
                }

                strResult = strBuilder.ToString();
            }
            catch (Exception exc)
            {
                strResult = "Error: " + exc.Message;
            }
            return strResult;
        }
        #endregion

        /// <summary>
        /// Process a payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public override ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();
            result.NewPaymentStatus = PaymentStatus.Pending;

            var settings = _commonServices.Settings.LoadSetting<AliPayPaymentSettings>(processPaymentRequest.Sited);

            if (settings.SellerEmail.IsEmpty() || settings.Key.IsEmpty())
            {
                result.AddError(T("Plugins.Payments.PayPalStandard.InvalidCredentials"));
            }

            return result;
        }

        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        public override void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            if (postProcessPaymentRequest.Order.PaymentStatus == PaymentStatus.Paid)
                return;

            var settings = _commonServices.Settings.LoadSetting<AliPayPaymentSettings>(postProcessPaymentRequest.Order.SiteId);

            //string orderNumber = postProcessPaymentRequest.Order.GetOrderNumber();


            var builder = new StringBuilder();
            //string gateway = "https://www.alipay.com/cooperate/gateway.do?";
            string service = "create_direct_pay_by_user";

            string seller_email = _aliPayPaymentSettings.SellerEmail;
            string sign_type = "MD5";
            string key = _aliPayPaymentSettings.Key;
            string partner = _aliPayPaymentSettings.Partner;
            string input_charset = "utf-8";

            string show_url = "http://www.alipay.com/";

            string out_trade_no = postProcessPaymentRequest.Order.OrderGuid.ToString();
            string subject = _commonServices.SiteContext.CurrentSite.Name;
            string body = "Order from " + _commonServices.SiteContext.CurrentSite.Name;
            string total_fee = postProcessPaymentRequest.Order.OrderTotal.ToString("0.00", CultureInfo.InvariantCulture);

            string notify_url = _commonServices.WebHelper.GetSiteLocation(false) + "Plugins/PaymentAliPay/Notify";
            string return_url = _commonServices.WebHelper.GetSiteLocation(false) + "Plugins/PaymentAliPay/Return";
            string[] para ={
                               "service="+service,
                               "partner=" + partner,
                               "seller_email=" + seller_email,
                               "out_trade_no=" + out_trade_no,
                               "subject=" + subject,
                               "body=" + body,
                               "total_fee=" + total_fee,
                               "show_url=" + show_url,
                               "payment_type=1",
                               "notify_url=" + notify_url,
                               "return_url=" + return_url,
                               "_input_charset=" + input_charset
                           };

            string aliay_url = CreatUrl(
                para,
                input_charset,
                key
                );
            var post = new RemotePost();
            post.FormName = "alipaysubmit";
            post.Url = "https://www.alipay.com/cooperate/gateway.do?_input_charset=utf-8";
            post.Method = "POST";

            post.Add("service", service);
            post.Add("partner", partner);
            post.Add("seller_email", seller_email);
            post.Add("out_trade_no", out_trade_no);
            post.Add("subject", subject);
            post.Add("body", body);
            post.Add("total_fee", total_fee);
            post.Add("show_url", show_url);
            post.Add("return_url", return_url);
            post.Add("notify_url", notify_url);
            post.Add("payment_type", "1");
            post.Add("sign", aliay_url);
            post.Add("sign_type", sign_type);

            post.Post();

            // _httpContext.Response.Redirect(builder.ToString());
        }

        /// <summary>
        /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Result</returns>
        public override bool CanRePostProcessPayment(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (order.PaymentStatus == PaymentStatus.Pending && (DateTime.UtcNow - order.CreatedOnUtc).TotalSeconds > 5)
            {
                return true;
            }
            return true;
        }

        public override Type GetControllerType()
        {
            return typeof(PaymentAliPayController);
        }

        public override decimal GetAdditionalHandlingFee(IList<OrganizedShoppingCartItem> cart)
        {
            return _aliPayPaymentSettings.AdditionalFee;
        }




        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public override void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "PaymentAliPa";
            routeValues = new RouteValueDictionary() { { "area", "CAF.WebSite.AliPay" } };
        }

        /// <summary>
        /// Gets a route for payment info
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public override void GetPaymentInfoRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PaymentInfo";
            controllerName = "PaymentAliPa";
            routeValues = new RouteValueDictionary() { { "area", "CAF.WebSite.AliPay" } };
        }

        #region Properties

        public override PaymentMethodType PaymentMethodType
        {
            get
            {
                return PaymentMethodType.Redirection;
            }
        }

        #endregion
    }
}
