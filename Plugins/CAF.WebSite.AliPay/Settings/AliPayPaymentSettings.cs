

using CAF.Infrastructure.Core.Configuration;
namespace CAF.WebSite.AliPay
{
    public class AliPayPaymentSettings : ISettings
    {
        public string SellerEmail { get; set; }
        public string Key { get; set; }
        public string Partner { get; set; }
        public decimal AdditionalFee { get; set; }
    }
}
